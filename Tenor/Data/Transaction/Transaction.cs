using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;

namespace Tenor.Data
{
    /// <summary>
    /// Represents a database transaction.
    /// </summary>
    public class Transaction : IDisposable
    {

        public Transaction()
        {
            this.connection = EntityBase.SystemConnection;
            this.Begin();
        }

        public Transaction(ConnectionStringSettings connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            this.connection = connection;
            this.Begin();
        }

        private ConnectionStringSettings connection;

        public ConnectionStringSettings Connection
        {
            get { return connection; }
        }

        /// <summary>
        /// Includes an instance to this transaction.
        /// </summary>
        /// <param name="item">A EntityBase instance.</param>
        public void Include(EntityBase item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            item.tenorTransaction = this;
        }

        /// <summary>
        /// Includes a list of EntityBase to this transaction.
        /// </summary>
        /// <param name="items"></param>
        public void Include(IList<EntityBase> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            foreach (EntityBase item in items)
                Include(item);
        }

        private DbConnection conn;
        internal DbTransaction dbTransaction;

        private void Begin()
        {
            conn = Helper.CreateConnection(connection);

            conn.Open();
            dbTransaction = conn.BeginTransaction();
        }

        /// <summary>
        /// Commits previous operations of included classes to the database.
        /// </summary>
        public void Commit()
        {
            if (dbTransaction == null)
                throw new ObjectDisposedException(this.GetType().Name);
            dbTransaction.Commit();
            this.Dispose();
        }

        /// <summary>
        /// Undoes previous operations.
        /// </summary>
        public void Rollback()
        {
            if (dbTransaction == null)
                throw new ObjectDisposedException(this.GetType().Name);
            dbTransaction.Rollback();
            this.Dispose();
        }

        /// <summary>
        /// Undoes previous operations and throws the following exception.
        /// </summary>
        /// <param name="ex">A Exception to be thrown.</param>
        public void Rollback(Exception ex)
        {
            Rollback();
            if (ex != null)
                throw ex;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {

                dbTransaction.Dispose();
                dbTransaction = null;
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                conn = null;
            }
            catch { }
        }
    }
}
