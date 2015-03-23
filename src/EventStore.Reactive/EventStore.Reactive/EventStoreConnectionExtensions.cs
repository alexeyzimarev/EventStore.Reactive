using System;
using EventStore.ClientAPI;

namespace EventStore.Reactive
{
    public static class EventStoreConnectionExtensions
    {
        /// <summary>
        ///     Creates new typed observable for an online subscription from all streams
        /// </summary>
        /// <typeparam name="T">Event type for events to subscribe to</typeparam>
        /// <param name="connection">EventStore connection</param>
        /// <param name="resolveLinkTos">Resolve link-to</param>
        /// <returns></returns>
        public static IObservable<T> CreateSubscriptionObservable<T>(this IEventStoreConnection connection,
            bool resolveLinkTos) where T : class
        {
            return new SubscriptionObservable<T>(connection, resolveLinkTos);
        }

        /// <summary>
        ///     Creates new typed observable for an online subscription from one stream
        /// </summary>
        /// <typeparam name="T">Event type for events to subscribe to</typeparam>
        /// <param name="connection">EventStore connection</param>
        /// <param name="resolveLinkTos">Resolve link-to</param>
        /// <param name="streamName">Stream to subscribe to</param>
        /// <returns></returns>
        public static IObservable<T> CreateStreamSubscriptionObservable<T>(this IEventStoreConnection connection,
            string streamName, bool resolveLinkTos) where T : class
        {
            return new SubscriptionStreamObservable<T>(connection, resolveLinkTos, streamName);
        }

        /// <summary>
        ///     Creates new typed observable for a catch-up subscription from all streams
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">EventStore connection</param>
        /// <param name="lastPosition">Previous last processed event position</param>
        /// <param name="resolveLinkTos">Resolve link-to</param>
        /// <param name="setLastPosition">Delegate to be called to record the new last processed event position</param>
        /// <returns></returns>
        public static IObservable<T> CreateCatchUpSubscriptionObservable<T>(this IEventStoreConnection connection,
            Position? lastPosition, bool resolveLinkTos, Action<Position?> setLastPosition) where T : class
        {
            return new CatchUpSubscriptionObservable<T>(connection, lastPosition, resolveLinkTos, setLastPosition);
        }

        /// <summary>
        ///     Creates new typed observable for a catch-up subscription from one stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">EventStore connection</param>
        /// <param name="streamName">Stream to subscribe to</param>
        /// <param name="lastPosition">Previous last processed event position</param>
        /// <param name="resolveLinkTos">Resolve link-to</param>
        /// <param name="setLastPosition">Delegate to be called to record the new last processed event position</param>
        /// <returns></returns>
        public static IObservable<T> CreateStreamCatchUpSubscriptionObservable<T>(this IEventStoreConnection connection,
            string streamName, int? lastPosition, bool resolveLinkTos, Action<int?> setLastPosition) where T : class
        {
            return new CatchUpSubscriptionStreamObservable<T>(connection, lastPosition, resolveLinkTos, streamName,
                setLastPosition);
        }
    }
}