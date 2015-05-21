using System;
using System.Threading;

namespace CircuitBreakerPattern.Core
{
    public class CircuitBreaker
    {
        private readonly String _name;
        private readonly Action<Exception> _tripAction;
        private readonly Int32 _maxTimesToRetry;
        private readonly TimeSpan _delayBetweenRetries;
        private readonly Timer _timer;
        private Exception _lastException;
        private Int64 _failureCount;

        public CircuitBreaker(String name, Action<Exception> tripAction, Int32 maxTimesToRetry, TimeSpan delayBetweenRetries)
        {
            _name = name;
            _tripAction = tripAction;
            _maxTimesToRetry = maxTimesToRetry;
            _delayBetweenRetries = delayBetweenRetries;

            _timer = new Timer(CircuitBreakerTripped, null, Timeout.Infinite, (Int32)delayBetweenRetries.TotalMilliseconds);
        }

        private void CircuitBreakerTripped(Object state)
        {
            Console.WriteLine("Check to see if we need to trip the circuit breaker. Retry:{0}", _failureCount);
            if (Interlocked.Increment(ref _failureCount) > _maxTimesToRetry)
            {
                Console.WriteLine("The circuit breaker for {0} is now tripped. Calling specified action", _name);
                _tripAction(_lastException);
                return;
            }
            _timer.Change(_delayBetweenRetries, TimeSpan.FromMilliseconds(-1));
        }

        public void Trip(Exception exception)
        {
            _lastException = exception;
            var newValue = Interlocked.Increment(ref _failureCount);

            if (newValue == 1)
            {
                _timer.Change(_delayBetweenRetries, TimeSpan.FromMilliseconds(-1));
                Console.WriteLine("The circuit breaker for {0} is now in the armed state", _name);
            }
        }

        public void Reset()
        {
            var oldValue = Interlocked.Exchange(ref _failureCount, 0);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            Console.WriteLine("The circuit breaker for {0} is now disarmed", _name);
        }
    }
}