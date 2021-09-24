using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStreamReader.PulseChecks
{
    public interface IStatisticsCalculator
    {
        long totalIterationCount { get; }

        public long totalElapsedMilliseconds
        {
            get;
            set;
        }

        bool incrementIterationCount(int interval);

        decimal iterationsPerMinute(); 
    }
}
