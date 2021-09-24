using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStreamReader.PulseChecks
{
    public class StatisticsCalculator : IStatisticsCalculator
    {
        const int minToMs = 60000;
        private long _IterationCounter = 0;
        private long _TotalElapsedMilliseconds = 0;

        /// <summary>
        /// Total tweets read
        /// </summary>
        public long totalIterationCount
        {
            get { return _IterationCounter; }
        }

        /// <summary>
        /// Total elapsed milliseconds 
        /// </summary>
        public long totalElapsedMilliseconds
        {
            get { return _TotalElapsedMilliseconds; }
            set { _TotalElapsedMilliseconds = value; }
        }

        /// <summary>
        /// Increment total tweet count by 1. 
        /// Return true if total tweets is an exact multiple of interval value. 
        /// This notifies the caller to display tweet count and rates. 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool incrementIterationCount(int interval)
        {
            // default to 1 if interval is 0 or negative, else use interval value
            int i = 1;
            if (interval > 0)
                i = interval;

            _IterationCounter++;      

            if ((_IterationCounter % i) == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculate tweets per minute.
        /// If less than a minute has elapsed, return total tweets.
        /// </summary>
        /// <returns></returns>
        public  decimal iterationsPerMinute()
        {
            if (totalElapsedMilliseconds < minToMs)
                return (decimal)totalIterationCount;
            else
            {
                return ((decimal)totalIterationCount / (decimal)totalElapsedMilliseconds) * minToMs;
            }

        }
    }
}
