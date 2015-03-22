using System;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using EventModels;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;

namespace EventsEmiter
{
    class Program
    {
        static void Main()
        {
            var connection = GetConnection();
            connection.ConnectAsync();

            Console.WriteLine("Press Enter to start emiting events.\nPress Enter once more to stop.");
            Console.ReadLine();

            var s1 = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(3))
                .Subscribe(x => EmitEvent(connection, "test-stream-1", x));
            var s2 = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(5))
                .Subscribe(x => EmitEvent(connection, "test-stream-2", x));

            Console.ReadLine();
            s1.Dispose();
            s2.Dispose();
            connection.Close();
        }

        static void EmitEvent(IEventStoreConnection connection, string stream, long number)
        {
            Console.WriteLine("Senging to " + stream);
            connection.AppendToStreamAsync(stream, ExpectedVersion.Any, CreateEvent(number, stream));
        }

        static EventData[] CreateEvent(long number, string stream)
        {
            return new[]
            {
                new EventData(Guid.NewGuid(), typeof(TestEvent).AssemblyQualifiedName, true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(TestEvent.Create(number, stream))), null)
            };
        }

        static IEventStoreConnection GetConnection()
        {
            var credentials = new UserCredentials("admin", "changeit");
            var connection =
                EventStoreConnection.Create(
                    ConnectionSettings.Create()
                        .UseConsoleLogger()
                        .SetDefaultUserCredentials(credentials),
                    new IPEndPoint(IPAddress.Loopback, 1113), "EventEmitter");
            return connection;
        }
    }
   
}
