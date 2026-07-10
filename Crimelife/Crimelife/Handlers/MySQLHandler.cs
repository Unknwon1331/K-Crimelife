using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;

namespace Crimelife
{
    public static class MySqlHandler
    {
        public static void ExecuteSync(MySqlQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            using MySqlConnection connection =
                new MySqlConnection(Configuration.connectionString);

            using MySqlCommand command =
                connection.CreateCommand();

            try
            {
                connection.Open();

                command.CommandText = query.Query;

                foreach (Crimelife.MySqlParameter parameter
                         in query.Parameters)
                {
                    command.Parameters.AddWithValue(
                        parameter.Name,
                        parameter.Obj ?? DBNull.Value
                    );
                }

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Print(
                    "[EXCEPTION ExecuteSync] Query: " +
                    command.CommandText
                );

                Logger.Print(
                    "[EXCEPTION ExecuteSync] Message: " +
                    ex.Message
                );

                Logger.Print(
                    "[EXCEPTION ExecuteSync] StackTrace: " +
                    ex.StackTrace
                );
            }
        }

        public static MySqlResult GetQuery(MySqlQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            MySqlConnection connection =
                new MySqlConnection(Configuration.connectionString);

            MySqlCommand command =
                connection.CreateCommand();

            try
            {
                connection.Open();

                command.CommandText = query.Query;

                foreach (Crimelife.MySqlParameter parameter
                         in query.Parameters)
                {
                    command.Parameters.AddWithValue(
                        parameter.Name,
                        parameter.Obj ?? DBNull.Value
                    );
                }

                MySqlDataReader reader =
                    command.ExecuteReader();

                return new MySqlResult(
                    reader,
                    connection
                );
            }
            catch (Exception ex)
            {
                Logger.Print(
                    "[EXCEPTION GetQuery] Query: " +
                    command.CommandText
                );

                Logger.Print(
                    "[EXCEPTION GetQuery] Message: " +
                    ex.Message
                );

                Logger.Print(
                    "[EXCEPTION GetQuery] StackTrace: " +
                    ex.StackTrace
                );

                command.Dispose();
                connection.Dispose();

                return null;
            }
        }
    }
}