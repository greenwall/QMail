using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Profiling
{
    public class Timers
    {
        private static readonly Dictionary<string, Timer> Infos = new Dictionary<string, Timer>();

        public static IDisposable Time(string blockName)
        {
            Timer info;
            if (!Infos.ContainsKey(blockName))
            {
                info = new Timer(blockName);
                Infos[blockName] = info;
            }
            else
            {
                info = Infos[blockName];
            }
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

        private readonly string name;
        private long _numberOfExecutions;
        private long _totalTime;
        private long _minTime;
        private long _maxTime;

        public Timer(string Name) 
        {
            name = Name;
        }

        public StartTime StartExecution()
        {
            return new StartTime(this);
        }

        public void AddExecution(StartTime timer) 
        {
            _numberOfExecutions++;

            _minTime = Math.Min(_minTime, timer.Millis());
            _maxTime = Math.Max(_maxTime, timer.Millis());
            _totalTime += timer.Millis();
        }

        public long Executions()
        {
            return _numberOfExecutions;
        }
        public String Name()
        {
            return name;
        }

        public long MinimumTime()
        {
            return _minTime;
        }

        public long MaximumTime()
        {
            return _maxTime;
        }

        public long AverageTime()
        {
            return _totalTime / _numberOfExecutions;
        }
        public long TotalTime()
        {
            return _totalTime;
        }
    }

    class StartTime : IDisposable
    {
        private readonly Timer _info;
        private readonly long _startMillis = DateTime.Now.Ticks/10000;
        private long _passedMillis;

        // Timer needs to hold reference to TimingInfo in order to update it on disposal.
        public StartTime(Timer info)
        {
            _info = info;
        }
        public void Dispose()
        {
            _passedMillis = System.DateTime.Now.Ticks/10000 - _startMillis;
            _info.AddExecution(this);
        }

        public long Millis()
        {
            return _passedMillis;
        }

        override public string ToString()
        {
            return ""+_passedMillis;
        }
    }
}
