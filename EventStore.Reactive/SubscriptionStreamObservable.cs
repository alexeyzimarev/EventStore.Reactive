using System;
using System.Reactive.Disposables;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    public class SubscriptionStreamObservable<T>: SubscriptionObservableBase<T>, IObservable<T> where T: class
    {
        private readonly string _streamName;

        public SubscriptionStreamObservable(IEventStoreConnection connection, bool resolveLinkTos, string streamName) : base(connection, resolveLinkTos)
        {
            _streamName = streamName;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Observer = observer;
            Subscription = Connection.SubscribeToStreamAsync(_streamName, ResolveLinkTos, EventAppeared, SubscriptionDropped).Result;
            return Disposable.Create(Stop);
        }
         
    }
}