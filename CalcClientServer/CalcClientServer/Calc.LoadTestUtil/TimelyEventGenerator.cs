namespace Calc.LoadTestUtil
{
    using System;
    using System.Timers;

    /// <summary>
    /// Генерирует необходимое количество вызовов делегата в секунду
    /// </summary>
    public class TimelyEventGenerator
    {
        private static readonly TimeSpan TimerTimeout = TimeSpan.FromMilliseconds(20);

        private readonly TimeSpan singleEventTime;
        private readonly double dispersionInMs;

        private Timer timer;
        private Random random = new Random();
        private TimeSpan counter;

        public TimelyEventGenerator(double eventPerSecond)
            : this(eventPerSecond, TimeSpan.Zero)
        {
        }

        public TimelyEventGenerator(double eventPerSecond, TimeSpan dispersion)
        {
            this.dispersionInMs = dispersion.TotalMilliseconds;
            this.singleEventTime = TimeSpan.FromSeconds(1.0 / eventPerSecond);

            this.timer = new Timer
            {
                Interval = TimerTimeout.TotalMilliseconds,
            };
            this.timer.Elapsed += this.timer_Elapsed;
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.counter += TimerTimeout;
            if (this.dispersionInMs > 0)
            {
                var dispersion =
                    TimeSpan.FromMilliseconds(
                        this.random.Next(int.MinValue, int.MaxValue) / (double) int.MaxValue * this.dispersionInMs);
                this.counter += dispersion;
            }

            while (this.counter > this.singleEventTime)
            {
                this.counter -= this.singleEventTime;

                if (this.TimelyEvent != null)
                    this.TimelyEvent();
            }
        }

        public event Action TimelyEvent;
    }
}
