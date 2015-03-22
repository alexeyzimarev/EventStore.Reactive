using System;
using System.IO;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStore.Reactive
{
    internal static class EventDeserializer
    {
        public static T Deserialize<T>(ResolvedEvent resolvedEvent) where T : class
        {
            if (typeof (T) != Type.GetType(resolvedEvent.Event.EventType, false))
                return null;

            using (var stream = new MemoryStream(resolvedEvent.Event.Data))
            using (var reader = new StreamReader(stream))
                return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
        }

    }
}

