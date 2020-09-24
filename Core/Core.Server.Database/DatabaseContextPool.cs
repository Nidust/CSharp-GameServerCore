using System;
using System.Collections.Generic;

namespace Core.Server.Database
{
    public class DatabaseContextPool : IDisposable
    {
        #region Properties
        private Stack<DatabaseContext> mContexts;
        private String mConnectionString;
        #endregion

        #region Methods
        public DatabaseContextPool(String connectionString)
        {
            mContexts = new Stack<DatabaseContext>();
            mConnectionString = connectionString;
        }

        public void Push(DatabaseContext context)
        {
            mContexts.Push(context);
        }

        public DatabaseContext Pop()
        {
            if (mContexts.Count == 0)
            {
                return new DatabaseContext(mConnectionString);
            }

            return mContexts.Pop();
        }

        public void Dispose()
        {
            foreach (DatabaseContext context in mContexts)
            {
                context.Dispose();
            }
        }
        #endregion
    }
}
