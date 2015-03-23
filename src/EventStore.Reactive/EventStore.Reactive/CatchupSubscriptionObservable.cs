using System;
using System.Reactive.Disposables;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    /// <summary>
    /// Typed observable for EventStore catch up subscription from all streams
    /// </summary>
    /// <typeparam name="T">Event type that you are subscribing to</typeparam>
    public class CatchUpSubscriptionObservable<T> : IObservable<T> where T : class
    {
        private readonly Position? _lastCheckpoint;
        private readonly Action<Position?> _setLastCheckpoint;
        private EventStoreAllCatchUpSubscription _subscription;
        private IObserver<T> _observer;
        private readonly IEventStoreConnection _connection;
        private readonly bool _resolveLinkTos;

        public CatchUpSubscriptionObservable(IEventStoreConnection connection, Position? lastCheckpoint,
            bool resolveLinkTos, Action<Position?> setLastCheckpoint)
        {
            _lastCheckpoint = lastCheckpoint;
            _setLastCheckpoint = setLastCheckpoint;
            _connection = connection;
            _resolveLinkTos = resolveLinkTos;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observer = observer;
            _subscription = _connection.SubscribeToAllFrom(_lastCheckpoint, _resolveLinkTos, EventAppeared,
                subscriptionDropped: SubscriptionDropped);
            return Disposable.Create(Stop);
        }

        private void Stop()
        {
            _subscription.Stop();
            if (_setLastCheckpoint != null)
                _setLastCheckpoint(_subscription.LastProcessedPosition);
        }

        private void EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
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

        private void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason,
            Exception e)
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
    }
}