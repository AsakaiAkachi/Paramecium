namespace Paramecium.Libraries
{
    public static class TimeWait
    {
        public static void Wait(int WaitTimeMillisecond)
        {
            DateTime waitStart = DateTime.Now;

            while((DateTime.Now - waitStart).TotalMilliseconds < WaitTimeMillisecond) { }

            return;
        }
    }
}
