using MySql.Data.MySqlClient;
using System;

namespace Core.Server.Database
{
    public class DatabaseContext : IDisposable
    {
        #region Properties
        private MySqlConnection mSqlConnection;
        #endregion

        #region Methods
        public DatabaseContext(String connectionString)
        {
            mSqlConnection = new MySqlConnection(connectionString);
            mSqlConnection.Open();
        }

        public void Dispose()
        {
            mSqlConnection.Close();
            mSqlConnection.Dispose();
        }
        #endregion
    }
}
