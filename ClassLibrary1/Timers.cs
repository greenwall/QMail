using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Profiling
{
    public class Timers
    {
        private static Dictionary<string, Timer> Infos = new Dictionary<string, Timer>();

        public static IDisposable Time(string BlockName)
        {
            Timer info;
            if (!Infos.ContainsKey(BlockName))
            {
                info = new Timer(BlockName);
                Infos[BlockName] = info;
            }
            else
            {
                info = Infos[BlockName];
            }
            //Timer timer = new Timer();

            return info.StartExecution();
        }

        public static string TimersToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Timer info  in Infos.Values) {
                builder.Append(info.Name());
                builder.Append("#");
                builder.Append(info.Executions());
                builder.Append("=");
                builder.Append(info.TotalTime());
                builder.Append("(");
                builder.Append(info.AverageTime());
                builder.AppendLine(")");
            }
            return builder.ToString();
        }
    }

    class Timer {

        private string name;
        private long NumberOfExecutions;
        private long _TotalTime;
        private long MinTime;
        private long MaxTime;

        public Timer(string Name) 
        {
            this.name = Name;
        }

        public StartTime StartExecution()
        {
            return new StartTime(this);
        }

        public void AddExecution(StartTime timer) 
        {
            NumberOfExecutions++;

            MinTime = Math.Min(MinTime, timer.Millis());
            MaxTime = Math.Max(MaxTime, timer.Millis());
            _TotalTime += timer.Millis();
        }

        public long Executions()
        {
            return NumberOfExecutions;
        }
        public String Name()
        {
            return name;
        }

        public long MinimumTime()
        {
            return MinTime;
        }

        public long MaximumTime()
        {
            return MaxTime;
        }

        public long AverageTime()
        {
            return _TotalTime / NumberOfExecutions;
        }
        public long TotalTime()
        {
            return _TotalTime;
        }
    }

    class StartTime : IDisposable
    {
        private Timer info;
        private long StartMillis = System.DateTime.Now.Ticks/10000;
        private long PassedMillis;

        // Timer needs to hold reference to TimingInfo in order to update it on disposal.
        public StartTime(Timer info)
        {
            this.info = info;
        }
        public void Dispose()
        {
            PassedMillis = System.DateTime.Now.Ticks/10000 - StartMillis;
            info.AddExecution(this);
        }

        public long Millis()
        {
            return PassedMillis;
        }

        override public string ToString()
        {
            return ""+PassedMillis;
        }
    }
}
