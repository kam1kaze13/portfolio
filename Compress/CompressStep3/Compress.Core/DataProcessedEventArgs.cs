namespace Compress.Core
{
    public class DataProcessedEventArgs
    {
        public int ProcessedNow { get; set; }

        public long Total { get; internal set; }

        public long TotalProcessed { get; internal set; }
    };
}
