using MySql.Data.MySqlClient;
using System;

namespace Core.Server.Database
{
    public class DatabaseContext : IDisposable
    {
        #region Properties
        private MySqlConnection mSqlConnection;
        private MySqlCommand mCommand;
        #endregion

        #region Methods
        public DatabaseContext(String connectionString)
        {
            mSqlConnection = new MySqlConnection(connectionString);
            mSqlConnection.Open();
        }

        public void ExecuteQuery(String query, params MySqlParameter[] parameters)
        {
            CreateCommand(query, parameters).ExecuteNonQuery();
        }

        public MySqlDataReader ExecuteReader(String query, params MySqlParameter[] parameters)
        {
            return CreateCommand(query, parameters).ExecuteReader();
        }

        public void Dispose()
        {
            mSqlConnection.Close();
            mSqlConnection.Dispose();
        }
        #endregion

        #region Private
        private MySqlCommand CreateCommand(String query, params MySqlParameter[] parameters)
        {
            if (mCommand == null)
            {
                mCommand = new MySqlCommand(query, mSqlConnection);
            }

            mCommand.CommandText = query;

            mCommand.Parameters.Clear();
            mCommand.Parameters.AddRange(parameters);

            return mCommand;
        }
        #endregion
    }
}
