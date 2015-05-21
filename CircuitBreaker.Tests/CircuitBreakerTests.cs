using System;
using CircuitBreakerPattern.Core;
using NUnit.Framework;

namespace CircuitBreakerPattern.Tests
{
    /// <summary>
    /// http://particular.net/blog/protect-your-software-with-the-circuit-breaker-design-pattern
    /// </summary>
    [TestFixture]
    public class CircuitBreakerTests
    {
        [Test]
        public void When_the_circuit_breaker_is_tripped_the_trip_action_is_called_after_reaching_max_threshold()
        {
            var circuitBreakerTripActionCalled = false;
            var connectionException = new Exception("Something bad happened.");

            var circuitBreaker = new CircuitBreaker("CheckServiceConnection", exception =>
            {
                Console.WriteLine("Circuit breaker tripped - fail fast");
                circuitBreakerTripActionCalled = true;
                
                //You would normally fail fast here in the action to faciliate the process shutdown by calling"
                Console.WriteLine("do stuff!!!");
            }, 3, TimeSpan.FromSeconds(1));

            circuitBreaker.Trip(connectionException);
            System.Threading.Thread.Sleep(5000);
            Assert.IsTrue(circuitBreakerTripActionCalled);
        }
    }
}
