using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Chan.Net.Captchas;
using Chan.Net.JsonModel;
using Newtonsoft.Json.Linq;

namespace Chan.Net
{
    public class Thread : Post
    {
        public Thread(Board parent, uint postNumber)
        {
            PostNumber = postNumber;
            Board = parent;
            RefreshRate = 10;
            UseCache = true;
            
            AutoUpdater = new AutoUpdater(this);
        }

        private DateTime _lastRefresh;
        private ThreadModel _cachedJson;
        private uint _lastId;
        
        public Board Board { get; }
        public AutoUpdater AutoUpdater { get; }

        public bool Sticky { get; set; }
        public bool Closed { get; set; }

        public int RefreshRate { get; set; }
        public bool UseCache { get; set; }

        public async Task<Post> CreatePostAsync(string message, ICaptcha captcha, OptionalPostArgs args)
        {
            uint id = await PostManager.CreatePost(Board, this, message, captcha, args).ConfigureAwait(false);

            Post newPost =  new Post()
            {
                Thread = this,
                Message = message,
                PostNumber = id,
                Name = args.Name
            };

            AutoUpdater.SuscribePost(newPost);

            return newPost;
        }

        public Post CreatePost(string message, ICaptcha captcha, OptionalPostArgs args)
        {
            return CreatePostAsync(message, captcha, args).GetAwaiter().GetResult();
        }
        
        public async Task SetLastViewedNowAsync()
        {
            _lastId = (await GetPostsAsync().ConfigureAwait(false)).LastOrDefault()?.PostNumber ?? 0;
        }

        public void SetLastViewedNow()
        {
            SetLastViewedNowAsync().GetAwaiter().GetResult();
        }

        public async Task<Post> GetPostAsync(uint id)
        {
            return (await GetPostsAsync().ConfigureAwait(false)).FirstOrDefault(post => post.PostNumber == id);
        }

        public Post GetPost(uint id)
        {
            return GetPostAsync(id).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Post>> GetRepliesAsync(Post post)
        {
            return (await GetPostsAsync().ConfigureAwait(false)).Where(p => p.Message.Contains(">>" + post.PostNumber));
        }

        public IEnumerable<Post> GetReplies(Post post)
        {
            return GetRepliesAsync(post).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            ThreadModel data = _cachedJson;

            if (!UseCache || _cachedJson == null || DateTime.Now.Subtract(_lastRefresh).TotalSeconds > RefreshRate)
            {
                _lastRefresh = DateTime.Now;
                HttpWebRequest req =
                    WebRequest.Create(string.Format("https://a.4cdn.org/{0}/thread/{1}.json", Board.BoardId,
                        PostNumber)) as HttpWebRequest;
                HttpWebResponse resp = await req.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;

                using (var stream = resp.GetResponseStream())
                {
                    data = JsonDeserializer.Deserialize<ThreadModel>(stream);
                }

                resp.Dispose();

                _cachedJson = data;
            }

            return data.Posts.Select(post => new Post()
            {
                PostNumber = unchecked((uint) post.no),
                Thread = this,
                Name = post.name,
                Message = post.com == null ? null : PostManager.CleanPostMessage(post.com),
                Image = string.IsNullOrEmpty(post.ext) ? null : new PostedImage(Board.BoardId, post.tim, post.ext)
                {
                    Filename = post.filename,
                    Height = post.h,
                    Width = post.w,
                    Filesize = post.fsize,
                    Md5Hash = post.md5,
                },
                TimeCreated = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(TimeSpan.FromSeconds(post.time)),
                Id = post.id,
                Country = post.country,
                CountryName = post.country_name,
                EpochTimeCreated = post.time,
                Trip = post.trip,
                Subject = WebUtility.HtmlDecode(post.sub ?? ""),
            });
        }

        public IEnumerable<Post> GetPosts()
        {
            return GetPostsAsync().GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Post>> GetNewPostsAsync(bool ignoreCache = false)
        {
            if (ignoreCache)
                _cachedJson = null;

            return (await GetPostsAsync().ConfigureAwait(false)).SkipWhile(p => p.PostNumber <= _lastId).Select(p =>
            {
                _lastId = p.PostNumber;
                return p;
            });
        }

        public IEnumerable<Post> GetNewPosts(bool ignoreCache = false)
        {
            return GetNewPostsAsync(ignoreCache).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<Post>> GetNewPostsAsync(uint lastId, bool ignoreCache = false)
        {
            if (ignoreCache)
                _cachedJson = null;

            return (await GetNewPostsAsync().ConfigureAwait(false)).SkipWhile(p => p.PostNumber <= lastId);
        }

        public IEnumerable<Post> GetNewPosts(uint lastId, bool ignoreCache = false)
        {
            return GetNewPostsAsync(lastId, ignoreCache).GetAwaiter().GetResult();
        }
    }
}
