using System;

namespace EventModels
{
    public class TestEvent
    {
        public long SomeLong { get; set; }
        public DateTime SomeTime { get; set; }
        public string SomeString { get; set; }

        public static TestEvent Create(long value, string stream)
        {
            return new TestEvent
            {
                SomeLong = value,
                SomeTime = DateTime.Now,
                SomeString = string.Format("Event {0} in stream {1}", value, stream)
            };
        }
    }

}