using System;
using System.Reactive.Disposables;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    /// <summary>
    /// Typed observable for EventStore online subscription from all streams
    /// </summary>
    /// <typeparam name="T">Event type that you are subscribing to</typeparam>
    public class SubscriptionObservable<T> : IObservable<T> where T : class
    {
        private IObserver<T> _observer;
        private readonly IEventStoreConnection _connection;
        private readonly bool _resolveLinkTos;
        private EventStoreSubscription _subscription;

        protected internal SubscriptionObservable(IEventStoreConnection connection, bool resolveLinkTos)
        {
            _connection = connection;
            _resolveLinkTos = resolveLinkTos;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observer = observer;
            _subscription = _connection.SubscribeToAllAsync(_resolveLinkTos, EventAppeared, SubscriptionDropped).Result;
            return Disposable.Create(Stop);
        }

        private void EventAppeared(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
        {
            try
            {
                var @event = EventDeserializer.Deserialize<T>(resolvedEvent);
                if (@event != null) _observer.OnNext(@event);
            }
            catch (Exception e)
            {
                _observer.OnError(e);
            }
        }

        private void SubscriptionDropped(EventStoreSubscription subscription, SubscriptionDropReason dropReason, Exception e)
        {
            if (e != null)
            {
                _observer.OnError(e);
            }
            else
            {
                _observer.OnCompleted();
            }
        }

        private void Stop()
        {
            _subscription.Close();
            _subscription.Dispose();
        }
    }
}