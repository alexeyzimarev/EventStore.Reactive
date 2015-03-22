using System;
using System.Linq;
using System.Net;
using EventModels;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

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
                new EventStoreReactive.EventStoreObservable<TestEvent>(connection, false)
                    .Subscribe(x => DumpEvent(x, "EventStoreObservable")),
                new EventStoreReactive.EventStoreCatchupObservable<TestEvent>(connection, null, false, DumpPosition)
                    .Subscribe(x => DumpEvent(x, "EventStoreCatchupObservable")),
                new EventStoreReactive.EventStoreStreamObservable<TestEvent>(connection, false, "test-stream-1")
                    .Subscribe(x => DumpEvent(x, "EventStoreStreamObservable for test-stream-1")),
                new EventStoreReactive.EventStoreCatchupStreamObservable<TestEvent>(connection, null, false, "test-stream-2", DumpPosition)
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
