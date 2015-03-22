using System;
using System.Reactive.Disposables;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    public class CatchupSubscriptionObservable<T> : CatchupSubscriptionObservableBase<T>, IObservable<T> where T : class
    {
        private readonly Position? _lastCheckpoint;
        private readonly Action<Position?> _setLastCheckpoint;
        private EventStoreAllCatchUpSubscription _subscription;

        public CatchupSubscriptionObservable(IEventStoreConnection connection, Position? lastCheckpoint,
            bool resolveLinkTos, Action<Position?> setLastCheckpoint) : base(connection, resolveLinkTos)
        {
            _lastCheckpoint = lastCheckpoint;
            _setLastCheckpoint = setLastCheckpoint;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Observer = observer;
            _subscription = Connection.SubscribeToAllFrom(_lastCheckpoint, ResolveLinkTos, EventAppeared,
                subscriptionDropped: SubscriptionDropped);
            return Disposable.Create(Stop);
        }

        private void Stop()
        {
            _subscription.Stop();
            if (_setLastCheckpoint != null)
                _setLastCheckpoint(_subscription.LastProcessedPosition);
        }
    }
}