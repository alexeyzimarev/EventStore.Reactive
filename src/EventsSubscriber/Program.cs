using System;
using System.Linq;
using System.Net;
using EventModels;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Reactive;

namespace EventsSubscriber
{
    class Program
    {
        static void Main()
        {
            var connection = GetConnection();
            connection.ConnectAsync();

            var subscriptions = new[]
            {
                connection.CreateSubscriptionObservable<TestEvent>(false)
                    .Subscribe(x => DumpEvent(x, "EventStoreObservable")),
                connection.CreateStreamSubscriptionObservable<TestEvent>("test-stream-1", false)
                    .Subscribe(x => DumpEvent(x, "EventStoreStreamObservable for test-stream-1")),
                connection.CreateCatchUpSubscriptionObservable<TestEvent>(null, false, DumpPosition)
                    .Subscribe(x => DumpEvent(x, "EventStoreCatchupObservable")),
                connection.CreateStreamCatchUpSubscriptionObservable<TestEvent>("test-stream-2", null, false, DumpPosition)
                    .Subscribe(x => DumpEvent(x, "EventStoreCatchupStreamObservable for test-stream-2"))
            };

            Console.ReadLine();

            subscriptions.ForEach(s => s.Dispose());
            connection.Close();

            Console.WriteLine("Press Enter to close");
            Console.ReadLine();
        }

        private static void DumpPosition(int? obj)
        {
            Console.WriteLine("Last processed position; " + obj);
        }

        private static void DumpPosition(Position? obj)
        {
            Console.WriteLine("Last processed position: " + (obj == null ? "null" : ((Position)obj).CommitPosition.ToString()));
        }

        static void DumpEvent(TestEvent testEvent, string what)
        {
            Console.WriteLine("{0}: {1}", what, testEvent.SomeString);
        }

        static IEventStoreConnection GetConnection()
        {
            var credentials = new UserCredentials("admin", "changeit");
            var connection =
                EventStoreConnection.Create(
                    ConnectionSettings.Create()
                        .UseConsoleLogger()
                        .SetDefaultUserCredentials(credentials),
                    new IPEndPoint(IPAddress.Loopback, 1113), "EventSubscriber");
            return connection;
        }
    }
}
