using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Util.Config;

namespace Util.Data
{
    public class DbTemplate
    {
        private readonly string _connectionName;

        public DbTemplate(string connectionName)
        {
            _connectionName = connectionName;
        }

        private SqlConnection GetConnection()
        {
            return ConfigHelper.SqlConnection(_connectionName);
        }

        public delegate T ObjectReader<T>(SqlDataReader reader);

        public T SelectOne<T>(string statement, ObjectReader<T> objectReader)
        {
            T result = default(T);

            using (SqlConnection connection = GetConnection())
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = statement;
                cmd.Connection = connection;

                connection.Open();

                Debug(cmd.CommandText);

                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            result = objectReader(reader);
                        }
                        if (reader.Read())
                        {
                            throw new Exception("QueryOne encountered multiple rows on query: " + statement);
                        }
                    }
                }
            }
            return result;
        }

        public IList<T> Select<T>(string statement, ObjectReader<T> objectReader, DbParameter[] parameters = null)
        {
            IList<T> result = new List<T>();

            using (SqlConnection connection = GetConnection())
            {
                var cmd = new SqlCommand();
                cmd.CommandText = statement;
                cmd.Connection = connection;

                connection.Open();

                Debug(cmd.CommandText);

                if (parameters != null)
                {
                    foreach (DbParameter param in parameters)
                    {
                        cmd.Parameters.Add(param.Name, param.Type).Value = param.Value;
                    }
                }
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(objectReader(reader));
                    }
                }
            }
            return result;
        }

        public int Count(string statement)
        {
            return (int)Scalar(statement);
        }

        public object Scalar(string statement, DbParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (var cmd = new SqlCommand(statement, connection))
                {
                    if (parameters != null)
                    {
                        foreach (DbParameter param in parameters)
                        {
                            cmd.Parameters.Add(param.Name, param.Type).Value = param.Value;
                        }
                    }
                    Debug(cmd.CommandText);

                    return cmd.ExecuteScalar();
                }
            }
        }

        public int Update(string statement, DbParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (var cmd = new SqlCommand(statement, connection))
                {
                    foreach (DbParameter param in parameters)
                    {
                        cmd.Parameters.Add(param.Name, param.Type).Value = param.Value;
                    }
                    Debug(cmd.CommandText);
                    try
                    {
                        return cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Exception executing update:"+statement, ex);
                    }
                }
            }
        }

        private void Debug(string msg)
        {
            //log.Debug(cmd.CommandText);
            Console.WriteLine(msg);
        }
    }

    public class DbParameter
    {
        private string name;
        private SqlDbType type;
        private Object value;

        public DbParameter(string name, SqlDbType type, Object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }

        public string Name { get { return name; } }
        public SqlDbType Type { get { return type; } }
        public Object Value { get { return value; } }
    }

}
