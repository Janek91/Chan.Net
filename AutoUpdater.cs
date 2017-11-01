using System.Collections.Concurrent;
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
            _trackedPosts = new List<IQuotable>();
        }

        public Thread Thread { get; }
        private List<IQuotable> _trackedPosts { get; }
        private uint _lastThreadId { get; set; }
        private Timer _timer;


        public void Start()
        {
            if (_timer != null) return;

            Thread.SetLastViewedNow();
            _lastThreadId = Thread.GetNewPosts(_lastThreadId, true).LastOrDefault()?.PostNumber ?? 0;
            _timer = new Timer(Tick, null, 0, Thread.RefreshRate * 1000);
        }

        public void Stop()
        {
            if (_timer == null) return;

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
            var newPosts = Thread.GetNewPosts(_lastThreadId, true).ToArray();

            if (newPosts.Length > 0)
            {
                _lastThreadId = newPosts[newPosts.Length - 1].PostNumber;

                lock (_trackedPosts)
                {
                    foreach (var post in _trackedPosts)
                    {
                        post.Update(newPosts);
                    }
                }
            }
        }

        public void SuscribePost(IQuotable p)
        {
            lock (_trackedPosts)
                if (!_trackedPosts.Contains(p))
                    _trackedPosts.Add(p);
        }

        public void UnsuscribePost(IQuotable p)
        {
            lock (_trackedPosts)
                _trackedPosts.Remove(p);
        }

        public bool IsPostSuscribed(IQuotable p)
        {
            lock (_trackedPosts)
                return _trackedPosts.Contains(p);
        }
    }
}