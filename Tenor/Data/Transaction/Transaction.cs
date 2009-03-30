using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Tenor.BLL;
using System.Data.Common;

namespace Tenor.Data
{
    public class Transaction : IDisposable
    {
        

        public Transaction()
        {
            this.connection = BLLBase.SystemConnection;
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


        public void Include(BLLBase item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            item.transaction = this;
        }

        public void Include(IList<BLLBase> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            foreach (BLLBase item in items)
                Include(item);
        }

        private DbProviderFactory factory;
        private DbConnection conn;
        internal DbTransaction transaction;

        private void Begin()
        {
            factory = Helper.GetFactory(this.connection);
            conn = factory.CreateConnection();
            conn.ConnectionString = connection.ConnectionString;
            conn.Open();
            transaction = conn.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
            this.Dispose();
        }

        public void Rollback()
        {
            transaction.Rollback();
            this.Dispose();
        }

        public void Rollback(Exception ex)
        {
            Rollback();
            throw ex;
        }

        public void Dispose()
        {
            try
            {

                transaction.Dispose();
                transaction = null;
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                conn = null;
            }
            catch { }
        }
    }
}
