using Chan.Net.Captchas;
using Chan.Net.JsonModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
            {
                return _cachedBoardInfo;
            }

            if (_cachedBoardList == null)
            {
                string json = await Internet.DownloadString(@"https://a.4cdn.org/boards.json").ConfigureAwait(false);

                _cachedBoardList = JsonDeserializer.Deserialize<BoardListModel>(json);
            }

            _cachedBoardInfo = _cachedBoardList.Boards.FirstOrDefault(b => b.ShortName == BoardId);

            return _cachedBoardInfo;
        }

        public BoardInfo GetBoardInfo()
        {
            return GetBoardInfoAsync().GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Thread>> GetThreadsAsync()
        {
            List<PageModel> pages;

            var req =
                WebRequest.Create(string.Format("https://a.4cdn.org/{0}/catalog.json", BoardId)) as HttpWebRequest;

            using (var resp = await req.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse)
            {
                using (Stream stream = resp.GetResponseStream())
                {
                    pages = JsonDeserializer.Deserialize<List<PageModel>>(stream);
                }
            }

            return from page in pages
                   from thread in page.Threads
                   select new Thread(this, unchecked((uint)thread.No))
                   {
                       Name = thread.Name,
                       Message = thread.Com == null ? null : PostManager.CleanPostMessage((string)thread.Com),
                       Image = !string.IsNullOrEmpty(thread.Ext) ? new PostedImage(BoardId, (long)thread.Tim, (string)thread.Ext)
                       {
                           Filename = thread.Filename,
                           Height = thread.H,
                           Width = thread.W,
                           Filesize = thread.Fsize,
                           Md5Hash = thread.MD5,
                       } : null,
                       TimeCreated = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(TimeSpan.FromSeconds(thread.Time)),
                       Id = thread.Id,
                       Country = thread.Country,
                       CountryName = thread.CountryName,
                       EpochTimeCreated = thread.Time,
                       Trip = thread.Trip,

                       Subject = WebUtility.HtmlDecode(thread.Sub ?? ""),
                       Sticky = thread.Sticky == 1,
                       Closed = thread.Closed == 1,
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
            uint resp = await PostManager.CreatePost(this, null, message, captcha, args).ConfigureAwait(false);

            var newThread = new Thread(this, resp);

            newThread.AutoUpdater.SuscribePost(newThread);

            return newThread;
        }

        public async Task<Thread> GetThreadAsync(uint threadId)
        {
            var t = new Thread(this, threadId);
            Post op = (await t.GetPostsAsync().ConfigureAwait(false)).FirstOrDefault();

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
