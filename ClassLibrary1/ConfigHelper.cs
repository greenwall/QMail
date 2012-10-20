using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Util.Config
{
    public class ConfigHelper
    {
        public static string RequiredString(string configName)
        {
            string value = ConfigurationManager.AppSettings[configName];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("No value for required configuration '" + configName + "' found.");
            }
            return value;
        }

        public static int DefaultInt(string configName, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[configName];
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    return int.Parse(value.Trim());
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid value=" + value + " for '" + configName + "'. Default value=" + defaultValue, ex);
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool DefaultBool(string configName, bool defaultValue)
        {
            string value = ConfigurationManager.AppSettings[configName];
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    return bool.Parse(value.Trim());
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid value=" + value + " for '" + configName + "'. Default value=" + defaultValue, ex);
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public static string DefaultString(string configName, string defaultValue, bool allowEmpty = false)
        {
            string value = ConfigurationManager.AppSettings[configName];
            if (value!=null && (allowEmpty || !string.IsNullOrWhiteSpace(value)))
            {
                try
                {
                    return value.Trim();
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid value=" + value + " for '" + configName + "'. Default value=" + defaultValue, ex);
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public static SqlConnection SqlConnection(string key)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[key];
            if (connectionStringSettings == null || string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
            {
                throw new ConfigurationErrorsException("The connectionStrings section is missing the " + key + " connection string");
            }
            string connectionString = connectionStringSettings.ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}