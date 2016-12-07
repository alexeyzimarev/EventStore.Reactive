using System;
using System.Reactive.Disposables;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    /// <summary>
    /// Typed observable for EventStore catch up subscription from one stream
    /// </summary>
    /// <typeparam name="T">Event type that you are subscribing to</typeparam>
    public class CatchUpSubscriptionStreamObservable<T> : IObservable<T> where T : class
    {
        private readonly string _stream;
        private readonly Action<int?> _setLastPosition;
        private readonly int? _lastCheckpoint;
        private EventStoreStreamCatchUpSubscription _subscription;
        private IObserver<T> _observer;
        private readonly IEventStoreConnection _connection;
        private readonly bool _resolveLinkTos;

        protected internal CatchUpSubscriptionStreamObservable(IEventStoreConnection connection, int? lastPosition,
            bool resolveLinkTos, string streamName, Action<int?> setLastPosition)
        {
            _lastCheckpoint = lastPosition;
            _stream = streamName;
            _setLastPosition = setLastPosition;
            _connection = connection;
            _resolveLinkTos = resolveLinkTos;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _subscription = _connection.SubscribeToStreamFrom(_stream, _lastCheckpoint, _resolveLinkTos, EventAppeared,
                subscriptionDropped: SubscriptionDropped);
            _observer = observer;
            return Disposable.Create(Stop);
        }

        private void Stop()
        {
            _subscription.Stop();
            if (_setLastPosition != null)
                _setLastPosition(_subscription.LastProcessedEventNumber);
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