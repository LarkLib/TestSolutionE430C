using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Common.Utilities;

namespace Common.DB
{
    public sealed class DbHelper
    {
        private string ConnectionString { get; set; }

        private IsolationLevel? defaultIsolationLevel;

        public DbHelper(string connectionString, string cacheName = null, IsolationLevel? defaultIsolationLevel = null)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }
            this.ConnectionString = connectionString;
            this.defaultIsolationLevel = defaultIsolationLevel;
        }

        public int ExecuteNonQueryHelper(string sprocName, CommandType commandType = CommandType.Text, int timeout = 0, SqlConnection connection = null, SqlTransaction dbTran = null, IsolationLevel? isoLevel = null, params SqlParameter[] commandParams)
        {
            return (int)ExecuteSprocHelper(sprocName: sprocName, isScalar: false, timeout: timeout, connection: connection, dbTran: dbTran, isoLevel: isoLevel, commandParams: commandParams);
        }

        public object ExecuteScalarHelper(string sprocName, CommandType commandType = CommandType.Text, int timeout = 0, SqlConnection connection = null, SqlTransaction transaction = null, IsolationLevel? isoLevel = null, params SqlParameter[] commandParams)
        {
            if (String.IsNullOrWhiteSpace(sprocName))
            {
                throw new ArgumentNullException("sprocName");
            }
            var cmd = new SqlCommand(sprocName) { CommandType = commandType };
            if (timeout > 0)
            {
                cmd.CommandTimeout = timeout;
            }
            if (!commandParams.IsNullOrEmpty())
            {
                cmd.Parameters.AddRange(commandParams);
            }
            return this.ExecuteScalarHelper(command: cmd, connection: connection, transaction: transaction, isoLevel: isoLevel);
        }
        public object ExecuteScalarHelper(SqlCommand command, SqlConnection connection = null, SqlTransaction transaction = null, IsolationLevel? isoLevel = null)
        {
            return ExecuteSprocHelper(cmd: command, isScalar: true, connection: connection, dbTran: transaction, isoLevel: isoLevel);
        }

        public delegate T ProcessingDelegate<T>(IDataReader reader);

        public T ExecuteReaderHelper<T>(string sprocName,
                                        ProcessingDelegate<T> processMethod,
                                        CommandType commandType = CommandType.Text,
                                        SqlConnection connection = null,
                                        SqlTransaction dbTran = null,
                                        IsolationLevel? isoLevel = null,
                                        params SqlParameter[] commandParams) where T : class
        {
            SqlConnection dbConnection = null;
            var tran = dbTran;
            try
            {
                var sqlRet = default(T);
                dbConnection = connection ?? new SqlConnection(this.ConnectionString);
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                if (connection == null && (isoLevel.HasValue || defaultIsolationLevel.HasValue))
                {
                    tran = dbConnection.BeginTransaction(isoLevel ?? defaultIsolationLevel.Value);
                }
                using (var cmd = dbConnection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandType = commandType;
                    cmd.CommandText = sprocName;
                    if (!commandParams.IsNullOrEmpty())
                    {
                        cmd.Parameters.AddRange(commandParams);
                    }
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sqlRet = processMethod(reader);
                        }
                    }
                }
                if (dbTran == null && tran != null)
                {
                    tran.Commit();
                }
                return sqlRet;
            }
            catch (Exception e)
            {
                if (null != tran)
                {
                    tran.Rollback();
                }
                throw e;
            }
            finally
            {
                if (null == connection && null != dbConnection)
                {
                    if (null != tran)
                    {
                        tran.Dispose();
                    }
                    dbConnection.Dispose();
                }
            }
        }
        public T ExecuteMultiReaderHelper<T>(string sprocName,
                                        ProcessingDelegate<T> processMethod,
                                        CommandType commandType = CommandType.Text,
                                        SqlConnection connection = null,
                                        SqlTransaction dbTran = null,
                                        IsolationLevel? isoLevel = null,
                                        int? timeout = null,
                                        params SqlParameter[] commandParams) where T : class
        {
            SqlConnection dbConnection = null;
            var tran = dbTran;
            try
            {
                var sqlRet = default(T);
                dbConnection = connection ?? new SqlConnection(this.ConnectionString);
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                if (connection == null && (isoLevel.HasValue || defaultIsolationLevel.HasValue))
                {
                    tran = dbConnection.BeginTransaction(isoLevel ?? defaultIsolationLevel.Value);
                }
                using (var cmd = dbConnection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandType = commandType;
                    cmd.CommandText = sprocName;
                    if (timeout != null)
                    {
                        cmd.CommandTimeout = timeout.Value;
                    }
                    if (!commandParams.IsNullOrEmpty())
                    {
                        cmd.Parameters.AddRange(commandParams);
                    }

                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        sqlRet = processMethod(reader);
                    }
                }
                if (dbTran == null && tran != null)
                {
                    tran.Commit();
                }
                return sqlRet;
            }
            catch (Exception e)
            {
                if (null != tran)
                {
                    tran.Rollback();
                }
                throw e;
            }
            finally
            {
                if (null == connection && null != dbConnection)
                {
                    if (null != tran)
                    {
                        tran.Dispose();
                    }
                    dbConnection.Dispose();
                }
            }
        }

        public IList<T> ExecuteReaderToListHelper<T>(string sprocName,
                                            ProcessingDelegate<T> processMethod,
                                            CommandType commandType = CommandType.Text,
                                            SqlConnection connection = null,
                                            SqlTransaction dbTran = null,
                                            IsolationLevel? isoLevel = null,
                                            int? timeout = null,
                                            params SqlParameter[] commandParams)
        {
            SqlConnection dbConnection = null;
            var tran = dbTran;
            try
            {
                IList<T> collection = new List<T>();
                dbConnection = connection ?? new SqlConnection(this.ConnectionString);
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                if (connection == null && (isoLevel.HasValue || defaultIsolationLevel.HasValue))
                {
                    tran = dbConnection.BeginTransaction(isoLevel ?? defaultIsolationLevel.Value);
                }
                using (var cmd = dbConnection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandType = commandType;
                    cmd.CommandText = sprocName;
                    if (timeout.HasValue && timeout.Value >= 0)
                    {
                        cmd.CommandTimeout = timeout.Value;
                    }
                    if (!commandParams.IsNullOrEmpty())
                    {
                        cmd.Parameters.AddRange(commandParams);
                    }
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(processMethod(reader));
                        }
                    }
                }
                if (dbTran == null && tran != null)
                {
                    tran.Commit();
                }
                return collection;
            }
            catch (Exception e)
            {
                if (null != tran)
                {
                    tran.Rollback();
                }
                throw e;
            }
            finally
            {
                if (null == connection && null != dbConnection)
                {
                    if (null != dbTran)
                    {
                        dbTran.Dispose();
                    }
                    dbConnection.Dispose();
                }
            }
        }

        private object ExecuteSprocHelper(string sprocName, bool isScalar, CommandType commandType = CommandType.Text, int timeout = 0, SqlConnection connection = null, SqlTransaction dbTran = null, IsolationLevel? isoLevel = null, params SqlParameter[] commandParams)
        {
            if (String.IsNullOrWhiteSpace(sprocName))
            {
                throw new ArgumentNullException("sprocName");
            }
            var dbConnection = connection ?? new SqlConnection(this.ConnectionString);
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            var tran = dbTran;
            if (connection == null && (isoLevel.HasValue || defaultIsolationLevel.HasValue))
            {
                tran = dbConnection.BeginTransaction(isoLevel ?? defaultIsolationLevel.Value);
            }
            object ret = null;
            using (var cmd = new SqlCommand(sprocName))
            {
                cmd.Transaction = tran;
                cmd.CommandType = commandType;
                if (timeout > 0)
                {
                    cmd.CommandTimeout = timeout;
                }
                if (!commandParams.IsNullOrEmpty())
                {
                    cmd.Parameters.AddRange(commandParams);
                }
                ret = this.ExecuteSprocHelper(cmd, isScalar, connection, dbTran);
            }
            if (null == connection && null != dbConnection)
            {
                if (null != tran)
                {
                    tran.Commit();
                    tran.Dispose();
                }
                dbConnection.Dispose();
            }
            return ret;
        }

        private object ExecuteSprocHelper(SqlCommand cmd, bool isScalar, SqlConnection connection = null, SqlTransaction dbTran = null, IsolationLevel? isoLevel = null)
        {
            SqlConnection dbConnection = null;
            var tran = dbTran;
            try
            {
                dbConnection = connection ?? new SqlConnection(this.ConnectionString);
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                if (connection == null && (isoLevel.HasValue || defaultIsolationLevel.HasValue))
                {
                    tran = dbConnection.BeginTransaction(isoLevel ?? defaultIsolationLevel.Value);
                }
                object ret = null;
                using (cmd)
                {
                    cmd.Connection = dbConnection;
                    if (tran != null)
                    {
                        cmd.Transaction = tran;
                    }
                    ret = isScalar ? cmd.ExecuteScalar() : cmd.ExecuteNonQuery();
                }
                if (dbTran == null && tran != null)
                {
                    tran.Commit();
                }
                return ret;
            }
            catch (Exception e)
            {
                if (null != tran)
                {
                    tran.Rollback();
                }
                throw e;
            }
            finally
            {
                if (null == connection && null != dbConnection)
                {
                    if (null != tran)
                    {
                        tran.Dispose();
                    }
                    dbConnection.Dispose();
                }
            }
        }
    }
}
