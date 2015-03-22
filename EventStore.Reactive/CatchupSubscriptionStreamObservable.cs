using System;
using System.Reactive.Disposables;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    public class CatchupSubscriptionStreamObservable<T> : CatchupSubscriptionObservableBase<T>, IObservable<T> where T : class
    {
        private readonly string _stream;
        private readonly Action<int?> _setLastPosition;
        private readonly int? _lastCheckpoint;
        private EventStoreStreamCatchUpSubscription _subscription;

        public CatchupSubscriptionStreamObservable(IEventStoreConnection connection, int? lastPosition,
            bool resolveLinkTos, string streamName, Action<int?> setLastPosition) : base(connection, resolveLinkTos)
        {
            _lastCheckpoint = lastPosition;
            _stream = streamName;
            _setLastPosition = setLastPosition;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _subscription = Connection.SubscribeToStreamFrom(_stream, _lastCheckpoint, ResolveLinkTos, EventAppeared,
                subscriptionDropped: SubscriptionDropped);
            Observer = observer;
            return Disposable.Create(Stop);
        }

        private void Stop()
        {
            _subscription.Stop();
            if (_setLastPosition != null)
                _setLastPosition(_subscription.LastProcessedEventNumber);
        }
    }
}