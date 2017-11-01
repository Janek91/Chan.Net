using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Chan.Net.Captchas;
using Chan.Net.JsonModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chan.Net
{
    public class Board
    {
        private static BoardListModel _cachedBoardList;
        private BoardInfo _cachedBoardInfo;

        public Board(string board)
        {
            BoardId = board.ToLower();
        }

        public string BoardId { get; set; }

        public async Task<BoardInfo> GetBoardInfoAsync()
        {
            if (_cachedBoardInfo != null)
                return _cachedBoardInfo;

            if (_cachedBoardList == null)
            {
                var json = await Internet.DownloadString(@"https://a.4cdn.org/boards.json").ConfigureAwait(false);

                _cachedBoardList = JsonDeserializer.Deserialize<BoardListModel>(json);
            }

            _cachedBoardInfo = _cachedBoardList.boards.FirstOrDefault(b => b.ShortName == BoardId);

            return _cachedBoardInfo;
        }

        public BoardInfo GetBoardInfo()
        {
            return GetBoardInfoAsync().GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Thread>> GetThreadsAsync()
        {
            List<PageModel> pages;

            HttpWebRequest req =
                WebRequest.Create(string.Format("https://a.4cdn.org/{0}/catalog.json", BoardId)) as HttpWebRequest;

            using (HttpWebResponse resp = await req.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse)
            {
                using (var stream = resp.GetResponseStream())
                {
                    pages = JsonDeserializer.Deserialize<List<PageModel>>(stream);
                }
            }

            return from page in pages
                from thread in page.threads
                select new Thread(this, unchecked((uint)thread.no))
                {
                    Name = thread.name,
                    Message = thread.com == null ? null : PostManager.CleanPostMessage((string)thread.com),
                    Image = !string.IsNullOrEmpty(thread.ext) ? new PostedImage(BoardId, (long)thread.tim, (string)thread.ext)
                    {
                        Filename = thread.filename,
                        Height = thread.h,
                        Width = thread.w,
                        Filesize = thread.fsize,
                        Md5Hash = thread.md5,
                    } : null,
                    TimeCreated = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(TimeSpan.FromSeconds(thread.time)),
                    Id = thread.id,
                    Country = thread.country,
                    CountryName = thread.country_name,
                    EpochTimeCreated = thread.time,
                    Trip = thread.trip,

                    Subject = WebUtility.HtmlDecode(thread.sub ?? ""),
                    Sticky = thread.sticky == 1,
                    Closed = thread.closed == 1,
                };
        }

        public IEnumerable<Thread> GetThreads()
        {
            return GetThreadsAsync().GetAwaiter().GetResult();
        }

        public Thread CreateThread(string message, ICaptcha captcha, OptionalPostArgs args)
        {
            return CreateThreadAsync(message, captcha, args).GetAwaiter().GetResult();
        }

        public async Task<Thread> CreateThreadAsync(string message, ICaptcha captcha, OptionalPostArgs args)
        {
            var resp = await PostManager.CreatePost(this, null, message, captcha, args).ConfigureAwait(false);

            var newThread = new Thread(this, resp);

            newThread.AutoUpdater.SuscribePost(newThread);

            return newThread;
        }

        public async Task<Thread> GetThreadAsync(uint threadId)
        {
            var t =  new Thread(this, threadId);
            var op = (await t.GetPostsAsync().ConfigureAwait(false)).FirstOrDefault();

            if (op != null)
            {
                t.Name = op.Name;
                t.Message = op.Message;
                t.Image = op.Image;
                t.TimeCreated = op.TimeCreated;
                t.Id = op.Id;
                t.Country = op.Country;
                t.CountryName = op.CountryName;
                t.EpochTimeCreated = op.EpochTimeCreated;
                t.Trip = op.Trip;
                t.Subject = op.Subject;
            }

            return t;
        }

        public Thread GetThread(uint threadId)
        {
            return GetThreadAsync(threadId).GetAwaiter().GetResult();
        }
    }
}