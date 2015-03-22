using System;
using System.Reactive.Disposables;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    public class SubscriptionObservable<T> : SubscriptionObservableBase<T>, IObservable<T> where T : class
    {
        public SubscriptionObservable(IEventStoreConnection connection, bool resolveLinkTos)
            : base(connection, resolveLinkTos) { }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Observer = observer;
            Subscription = Connection.SubscribeToAllAsync(ResolveLinkTos, EventAppeared, SubscriptionDropped).Result;
            return Disposable.Create(Stop);
        }
    }
}