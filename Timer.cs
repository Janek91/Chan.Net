#if NETSTANDARD1_1

using System;
using System.Threading.Tasks;

namespace Chan.Net
{
    public class Timer : IDisposable
    {
        private object _state;
        private int _dueTime;
        private int _period;
        private Action<object> _callback;

        private DateTime _start;
        private Task _thread;
        private bool _running;

        public Timer(Action<object> callback, object state, int dueTime, int period)
        {
            _callback = callback;
            _state = state;
            _dueTime = dueTime;
            _period = period;

            _state = DateTime.Now;

            _thread = Task.Run((Action) TimerLoop);
        }

        private void TimerLoop()
        {
            if (DateTime.Now.Subtract(_start).TotalMilliseconds > _dueTime)
                InvokeCallback();

            while (_running)
            {
                if (DateTime.Now.Subtract(_start).TotalMilliseconds > _period)
                    InvokeCallback();
            }
        }

        private void InvokeCallback()
        {
            _start = DateTime.Now;
            _callback?.Invoke(_state);
        }

        public async void Dispose()
        {
            _running = false;

            await _thread;
        }
    }
}
#endif