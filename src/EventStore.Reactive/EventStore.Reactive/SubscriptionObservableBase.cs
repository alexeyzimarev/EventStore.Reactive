using System;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    public abstract class SubscriptionObservableBase<T> where T: class
    {
        protected IObserver<T> Observer;
        protected readonly IEventStoreConnection Connection;
        protected readonly bool ResolveLinkTos;
        protected EventStoreSubscription Subscription;

        protected SubscriptionObservableBase(IEventStoreConnection connection, bool resolveLinkTos)
        {
            Connection = connection;
            ResolveLinkTos = resolveLinkTos;
        }

        protected void EventAppeared(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
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

        protected void SubscriptionDropped(EventStoreSubscription subscription, SubscriptionDropReason dropReason, Exception e)
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

        protected void Stop()
        {
            Subscription.Close();
            Subscription.Dispose();
        }
         
    }
}