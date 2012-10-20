using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ams.sf
{
    public class Installation : EnvironmentActivity
    {
        public Installation(String env, String version) : base(env, version)
        {
        }

        public string Version() 
        {
            return base.activity;
        }
    }

    public class EnvironmentActivity : Activity
    {
        private readonly string env;

        public string Environment()
        {
            return env;
        }

        public EnvironmentActivity(String env, String activity) : base(activity)
        {
            this.env = env;
        }
    }

    public class Activity
    {
        protected readonly string activity;

        public Activity(string act)
        {
            activity = act;
        }

        public override string ToString() {
            return activity;
        }

        public static Activity Parse(String s) 
        {
            char[] delimiters = new char[] { ':' };
	        string[] parts = s.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length==2) {
                // Env, installation
                string env = parts[0].Trim();
                string activity = parts[1].Trim();

                //string activities 

                return new Installation(parts[0], activity);
            } else if (parts.Length==1) {
                // Activity
                return new Activity(parts[0]);
            } else {
                throw new Exception("Unable to parse activity = " + s); 
            }
        }
    }
}
