# EventStore.Reactive

##Reactive subscriptions for EventStore (http://geteventstore.com)

EventStore subscriptions are by nature reactive and asyncronous. This project lets you create a typed observable EventStore subscription and use Reactive Extensions (https://rx.codeplex.com/) to work with it.

You get four generic classes that create different typed observables.
- EventStore.Ractive.SubscriptionObservable<T> - to subscribe to all events of type T
- EventStore.Ractive.SubscriptionStreamObservable<T> - to subscribe to events of type T from one stream
- EventStore.Ractive.CatchupSubscriptionObservable<T> - to create a catch up subscription from all events of type T
- EventStore.Ractive.CatchupSubscriptionStreamObservable<T> - to create a catch up subscription for type T from one stream

Each class implements IObservable<T>, so you can use all Rx extension methods like Buffer, Window, Distinct and so on, and subscribe to events using Subscribe(IObserver observer).

To get an observable, call one of the extension methods on IEventStoreConnection:

connection.CreateSubscriptionObservable<TestEvent>(false)
	.Subscribe(x => DumpEvent(x, "EventStoreObservable")),
connection.CreateStreamSubscriptionObservable<TestEvent>("test-stream-1", false)
    .Subscribe(x => DumpEvent(x, "EventStoreStreamObservable for test-stream-1")),
connection.CreateCatchUpSubscriptionObservable<TestEvent>(null, false, DumpPosition)
    .Subscribe(x => DumpEvent(x, "EventStoreCatchupObservable")),
connection.CreateStreamCatchUpSubscriptionObservable<TestEvent>("test-stream-2", null, false, DumpPosition)
    .Subscribe(x => DumpEvent(x, "EventStoreCatchupStreamObservable for test-stream-2"))

Remember that these subscriptions receive all events from EventStore but your observer will only receive events of type T, all other events will b swallowed.

When EventStore subscription is dropped, your observer will either be called OnError if there was an exception, or OnComplete otherwise.

All observable subscription stops its underlying EventStore subscription when being disposed. For catch up subscriptions you can supply a delegate that will receive the last processed event position so you can save it for future use. This delegate is called when the subscription is stopped.

