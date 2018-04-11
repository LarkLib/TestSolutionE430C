using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestSqliteConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestMehtodSqliteEF();
            //TestMehtodSqliteConnection();
            //TestMehtodSqliteMemoryDatabase();
            //TestMehtodLoadSqliteDbToMemory();
            //TestMehtodLoadSqliteTransaction();
            //TestMehtodTestSqliteUserDefinedFunction();
            //TestMehtodTestLoadJsonExtension();
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
        private static void TestMehtodSqliteEF()
        {
            Console.WriteLine(MethodBase.GetCurrentMethod().Name);
            using (var context = new TestSqliteEntities())
            {
                foreach (var item in context.EmployeeDemoes)
                {
                    Console.WriteLine($"{item.EmployeeID},{item.Title},{item.LoginID},{item.HireDate},{item.ManagerID}");
                }
            }
        }
        private static void TestMehtodSqliteConnection()
        {
            Console.WriteLine(MethodBase.GetCurrentMethod().Name);

            using (var conn = new SQLiteConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["TestSqliteConnectionString"].ConnectionString;
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT SQLITE_VERSION()";
                conn.Open();
                var version = cmd.ExecuteScalar() as string;
                conn.Close();
                Console.WriteLine(version);
                cmd.CommandText = "PRAGMA table_info(sqlite_master);";
                //cmd.Parameters.Add(new SQLiteParameter("@tableName", DbType.String) { Value = "sqlite_master" });
                conn.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.Default);
                DataTable schemaTable = reader.GetSchemaTable();
                //foreach (DataColumn col in schemaTable.Columns)
                for (var i = 0; i < schemaTable.Columns.Count; i++)
                    Console.WriteLine($"{i,-3}{schemaTable.Columns[i].ColumnName,-20}{schemaTable.Columns[i].DataType,-20}");
                Console.WriteLine();
                for (int i = 0; i < schemaTable.Rows.Count; i++)
                {
                    foreach (DataColumn col in schemaTable.Columns)
                        Console.WriteLine($"{i,-3}{schemaTable.Rows[i][col],-20}");
                    Console.WriteLine();
                }

                while (reader.Read())
                {
                    //Console.WriteLine($"{reader.GetInt32(0),-4}{reader.GetString(1),-24} {reader[2],-4}{reader.GetString(3),-30}{reader.GetDateTime(4)}");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i]);
                        Console.Write("    ");
                    }
                    Console.WriteLine();
                }
                reader.Close();
                conn.Close();
                cmd.CommandText = "SELECT * FROM sqlite_master;";
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.Default);
                while (reader.Read())
                {
                    //Console.WriteLine($"{reader.GetInt32(0),-4}{reader.GetString(1),-24} {reader[2],-4}{reader.GetString(3),-30}{reader.GetDateTime(4)}");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i]);
                        Console.Write("    ");
                    }
                    Console.WriteLine();
                }
                reader.Close();
                conn.Close();

            }

        }

        private static void TestMehtodSqliteMemoryDatabase()
        {
            Console.WriteLine(MethodBase.GetCurrentMethod().Name);
            string connctionstring = "FullUri=file:memorydb.db?mode=memory&cache=shared";
            //string cs = "URI=file:test.db";

            int nrows = 10;

            using (SQLiteConnection conn = new SQLiteConnection(connctionstring))
            {
                conn.Open();

                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Friends (Id INT PRIMARY KEY, Name TEXT)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT OR REPLACE INTO Friends VALUES(1, 'Tom')";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT OR REPLACE INTO Friends VALUES(2, 'Jane')";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT OR REPLACE INTO Friends VALUES(3, 'Rebekka')";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT OR REPLACE INTO Friends VALUES(4, 'Lucy')";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT OR REPLACE INTO Friends VALUES(5, 'Robert')";
                    cmd.ExecuteNonQuery();


                    cmd.CommandText = "SELECT * FROM Friends LIMIT @Id";
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@Id", nrows);

                    int cols = 0;
                    int rows = 0;

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        cols = reader.FieldCount;
                        rows = 0;

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader.GetInt32(0),-5}{reader.GetString(1)}");
                            rows++;
                        }

                        Console.WriteLine("The query fetched {0} rows", rows);
                        Console.WriteLine("Each row has {0} cols", cols);
                    }
                }

                conn.Close();
            }
        }

        private static void TestMehtodLoadSqliteDbToMemory()
        {
            Console.WriteLine(MethodBase.GetCurrentMethod().Name);
            using (var source = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TestSqliteConnectionString"].ConnectionString))
            using (var destination = new SQLiteConnection("FullUri=file:memorydb.db?mode=memory&cache=shared"))
            {
                source.Open();
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, 0);
                var cmd = destination.CreateCommand();
                cmd.CommandText = "SELECT  EmployeeID, LoginID, Title, ManagerID, HireDate  FROM EmployeeDemo Limit 10";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetInt32(0),-5}{reader.GetString(2),-35}{reader[4]}");
                }
            }
        }
        private static void TestMehtodLoadSqliteTransaction()
        {
            Console.WriteLine(MethodBase.GetCurrentMethod().Name);
            using (var conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TestSqliteConnectionString"].ConnectionString))
            {
                int eId1 = 0;
                int eId2 = 0;
                conn.Open();
                var transaction = conn.BeginTransaction();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT  EmployeeID FROM EmployeeDemo order by EmployeeID desc Limit 2";
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //Console.WriteLine($"{reader.GetInt32(0),-5}{reader.GetString(2),-35}{reader[4]}");
                    eId1 = reader.GetInt32(0);
                }
                if (reader.Read())
                {
                    //Console.WriteLine($"{reader.GetInt32(0),-5}{reader.GetString(2),-35}{reader[4]}");
                    eId2 = reader.GetInt32(0);
                }
                reader.Close();
                cmd.CommandText = $"update EmployeeDemo set modifytime='{DateTime.Now.ToString()}' where EmployeeID={eId1}";
                cmd.ExecuteNonQuery();
                transaction.Commit();
                transaction = conn.BeginTransaction();
                cmd.CommandText = $"update EmployeeDemo set modifytime='{DateTime.Now.ToString()}' where EmployeeID={eId2}";
                cmd.ExecuteNonQuery();
                transaction.Rollback();

                cmd.CommandText = "SELECT  EmployeeID, ModifyTime FROM EmployeeDemo order by EmployeeID desc Limit 2";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader.GetInt32(0),-5}{reader.GetString(1)}");
                }
            }
        }

        private static void TestMehtodTestLoadJsonExtension()
        {
            Console.WriteLine(MethodBase.GetCurrentMethod().Name);
            using (var conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TestSqliteConnectionString"].ConnectionString))
            {
                conn.Open();
                conn.EnableExtensions(true);
                //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "x86", "SQLite.Interop.dll");
                //if (File.Exists(path))
                //{
                //    conn.LoadExtension(path, "sqlite3_json_init");
                //}
                //copy SQLite.Interop.dll to GAC of C:\Windows\Microsoft.Net\assembly\GAC_32\System.Data.SQLite\
                //Try to run testapplication  in menu and check if it is working
                //now it is still don't working
                /*
                  System.AccessViolationException was unhandled
                  HResult=-2147467261
                  Message=Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
                  Source=System.Data.SQLite
                  StackTrace:
                       at System.Data.SQLite.UnsafeNativeMethods.sqlite3_load_extension(IntPtr db, Byte[] fileName, Byte[] procName, IntPtr& pError)
                       at System.Data.SQLite.SQLite3.LoadExtension(String fileName, String procName)
                       at System.Data.SQLite.SQLiteConnection.LoadExtension(String fileName, String procName)
                       at TestSqliteConsoleApplication.Program.TestMehtodTestLoadJsonExtension() in D:\MySpace\Work\Projects\TestSolution\TestSqliteConsoleApplication\Program.cs:line 226
                       at TestSqliteConsoleApplication.Program.Main(String[] args) in D:\MySpace\Work\Projects\TestSolution\TestSqliteConsoleApplication\Program.cs:line 24
                       at System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)
                       at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)
                       at Microsoft.VisualStudio.HostingProcess.HostProc.RunUsersAssembly()
                       at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
                       at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
                       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
                       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
                       at System.Threading.ThreadHelper.ThreadStart()
                  InnerException: 
                */
                //Todo: LoadJsonExtension don't working
                conn.LoadExtension("SQLite.Interop.dll", "sqlite3_json_init");
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select json_array(1,2,'3',4)";
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine(reader[0]);
                }
            }
        }

        private static void TestMehtodTestSqliteUserDefinedFunction()
        {
            Console.WriteLine(MethodBase.GetCurrentMethod().Name);
            using (var conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TestSqliteConnectionString"].ConnectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT  DoubleString(EmployeeID) FROM EmployeeDemo order by EmployeeID desc Limit 1";
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine(reader[0]);
                }
            }
        }

        [SQLiteFunction(Name = "DoubleString", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class DoubleString : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return $"{args[0].ToString()}|{args[0].ToString()}";
            }
        }

        private static void TestMehtodSqliteDatabase()
        {
            var sqlliteDb = new SQLiteDb("");
        }
    }
}
