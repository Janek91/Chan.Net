using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Chan.Net
{
    public class AutoUpdater
    {
        public AutoUpdater(Thread t)
        {
            Thread = t;
            TrackedPosts = new List<IQuotable>();
        }

        public Thread Thread { get; }
        private List<IQuotable> TrackedPosts { get; }
        private uint LastThreadId { get; set; }
        private Timer _timer;

        public void Start()
        {
            if (_timer != null)
            {
                return;
            }

            Thread.SetLastViewedNow();
            LastThreadId = Thread.GetNewPosts(LastThreadId, true).LastOrDefault()?.PostNumber ?? 0;
            _timer = new Timer(Tick, null, 0, Thread.RefreshRate * 1000);
        }

        public void Stop()
        {
            if (_timer == null)
            {
                return;
            }

            //_timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        public bool IsActive
        {
            get { return _timer != null; }
        }

        private void Tick(object state)
        {
            Post[] newPosts = Thread.GetNewPosts(LastThreadId, true).ToArray();

            if (newPosts.Length > 0)
            {
                LastThreadId = newPosts[newPosts.Length - 1].PostNumber;

                lock (TrackedPosts)
                {
                    foreach (IQuotable post in TrackedPosts)
                    {
                        post.Update(newPosts);
                    }
                }
            }
        }

        public void SuscribePost(IQuotable p)
        {
            lock (TrackedPosts)
            {
                if (!TrackedPosts.Contains(p))
                {
                    TrackedPosts.Add(p);
                }
            }
        }

        public void UnsuscribePost(IQuotable p)
        {
            lock (TrackedPosts)
            {
                TrackedPosts.Remove(p);
            }
        }

        public bool IsPostSuscribed(IQuotable p)
        {
            lock (TrackedPosts)
            {
                return TrackedPosts.Contains(p);
            }
        }
    }
}
