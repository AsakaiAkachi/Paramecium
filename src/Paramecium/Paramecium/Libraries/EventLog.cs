namespace Paramecium.Libraries
{
    public class EventLog
    {
        public static List<EventLog> EventLogs = new List<EventLog>();
        public static object EventLogsLockObj = new object();

        public DateTime IssuedDateTime;
        public string EventLogText;

        public EventLog(string eventLogText)
        {
            IssuedDateTime = DateTime.Now;
            EventLogText = eventLogText;
        }

        public static void PushEventLog(string eventLogText)
        {
            lock (EventLogsLockObj)
            {
                for (int i = EventLogs.Count - 1; i >= 0; i--)
                {
                    if ((DateTime.Now - EventLogs[i].IssuedDateTime).TotalMilliseconds >= 10000)
                    {
                        EventLogs.RemoveAt(i);
                    }
                }
                EventLogs.Add(new EventLog(eventLogText));
            }
        }
        public static List<EventLog> LoadEventLog()
        {
            lock (EventLogsLockObj)
            {
                for (int i = EventLogs.Count - 1; i >= 0; i--)
                {
                    if ((DateTime.Now - EventLogs[i].IssuedDateTime).TotalMilliseconds >= 10000)
                    {
                        EventLogs.RemoveAt(i);
                    }
                }
                return new List<EventLog>(EventLogs);
            }
        }
    }
}
