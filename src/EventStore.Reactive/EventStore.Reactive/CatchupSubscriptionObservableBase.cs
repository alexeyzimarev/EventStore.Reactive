using System;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    public abstract class CatchupSubscriptionObservableBase<T> where T : class
    {
        protected IObserver<T> Observer;
        protected readonly IEventStoreConnection Connection;
        protected readonly bool ResolveLinkTos;

        protected CatchupSubscriptionObservableBase(IEventStoreConnection connection, bool resolveLinkTos)
        {
            ResolveLinkTos = resolveLinkTos;
            Connection = connection;
        }

        protected void EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            try
            {
                var @event = EventDeserializer.Deserialize<T>(resolvedEvent);
                if (@event != null) Observer.OnNext(@event);
            }
            catch (Exception e)
            {
                Observer.OnError(e);
            }
        }

        protected void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason,
            Exception e)
        {
            if (e != null)
            {
                Observer.OnError(e);
            }
            else
            {
                Observer.OnCompleted();
            }
        }
         
    }
}