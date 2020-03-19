using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Data;
using System.Data.SqlClient;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Tools_SQL_Class
    {
        public static string m_Host { get; set; }
        public static string m_DbName { get; set; }
        public static string m_UserName { get; set; }
        public static string m_UserPassword { get; set; }
        public static string m_errorString;
        public static int m_timeOut = 5;
        private static readonly string TAG = "SQLStaticClass -> ";
        public static bool m_legalDB { get; private set; } = false;
        public void setTimeOut(int delay)
        {
            m_timeOut = delay;
        }
        public static string ErrorString()
        {
            return m_errorString;
        }
        private static SqlConnection m_Connect = null;
        static bool connStatus = false;
        public static bool Status()
        {
            return connStatus;
        }
        //public SQLStaticClass(string host,string dbname,string username,string userpassword)
        //{
        //    m_Host = host;
        //    m_DbName = dbname;
        //    m_UserName = username;
        //    m_UserPassword = userpassword;
        //}
        private static Mutex g_mut = new Mutex();
        public static bool connect()
        {
            //string constr = "Data Source=" + m_Host + ";instanceName=MSSQLSERVER;Initial Catalog=master;User ID=" + m_UserName + ";pwd=" + m_UserPassword;
            g_mut.WaitOne();
            if (m_Host == "" || m_UserName == "")
            {
                m_errorString = "连接字符串未设置！";
                g_mut.ReleaseMutex();
                return false;
            }
            string constr = "Data Source=" + m_Host + ";Initial Catalog=master;User ID=" + m_UserName + ";pwd=" + m_UserPassword + ";Connect Timeout =" + m_timeOut.ToString() + ";MultipleActiveResultSets=true;";
            Log.Debug(TAG,"Sql_Ready " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            m_Connect = new SqlConnection(constr);
            
            try
            {
                if (m_Connect.State != ConnectionState.Connecting && m_Connect.State == ConnectionState.Closed)
                {
                    m_Connect.Open();
                    Log.Debug(TAG, "Sql_Finish " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }   
            }
            catch (SqlException sex)
            {
                Log.Debug(TAG, "Sql_Error " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + sex.Message);
                m_errorString = sex.Message;
                connStatus = false;
                g_mut.ReleaseMutex();
                return false;
            }
            connStatus = true;
            g_mut.ReleaseMutex();
            return true;

        }

        public static bool directconnect()
        {
            //string constr = "Data Source=" + m_Host + ";instanceName=MSSQLSERVER;Initial Catalog=master;User ID=" + m_UserName + ";pwd=" + m_UserPassword;

            if (m_Host == "" || m_UserName == "")
            {
                m_errorString = "连接字符串未设置！";
                return false;
            }
            string constr = "Data Source=" + m_Host + ";Initial Catalog= " + m_DbName + ";User ID=" + m_UserName + ";pwd=" + m_UserPassword + ";Connect Timeout =" + m_timeOut.ToString() + ";MultipleActiveResultSets=true;";
            Log.Debug(TAG, "Sql_Ready " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            m_Connect = new SqlConnection(constr);

            try
            {
                if (m_Connect.State != ConnectionState.Connecting && m_Connect.State == ConnectionState.Closed)
                {
                    m_Connect.Open();
                    Log.Debug(TAG, "Sql_Finish " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            catch (SqlException sex)
            {
                Log.Debug(TAG, "Sql_Error " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + sex.Message);
                m_errorString = sex.Message;
                connStatus = false;
                return false;
            }
            connStatus = true;
            return true;

        }
        public static DataTable getTable(string sqlTxt)
        {
            m_errorString = "";
            if (!connStatus)
            {
                //int timeLimit = 3;
                //int timeTry = 0;
                if (!connect())
                {
                    //Thread.Sleep(sleep)
                    //timeTry++;
                    //if(timeTry >= timeLimit)
                    //{
                    //    if(m_errorString != "")
                    //    {
                    //        return null;
                    //    }
                    //}
                    //return null;
                    return null;
                }
            }
            SqlDataReader reader;
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = m_timeOut;
                cmd.Connection = m_Connect;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "use " + m_DbName + ";" + sqlTxt;
                if (m_Connect.State == ConnectionState.Closed)
                {
                    m_Connect = null;
                    connStatus = false;
                    return null;
                }
                reader = cmd.ExecuteReader(CommandBehavior.Default);
                dt.Load(reader);
                reader.Close();
            }
            catch (SqlException sqlex)
            {

                if (m_Connect.State == ConnectionState.Broken || m_Connect.State == ConnectionState.Closed)
                {
                    connStatus = false;
                    m_Connect = null;
                }
                m_errorString = "执行错误!" + sqlex.Message;
                return null;
            }
            m_errorString = "";
            return dt;
        }
        public static DataTable directGetTable(string sqlTxt)
        {
            m_errorString = "";
            if (!connStatus)
            {
                //int timeLimit = 3;
                //int timeTry = 0;
                if (!connect())
                {
                    //Thread.Sleep(sleep)
                    //timeTry++;
                    //if(timeTry >= timeLimit)
                    //{
                    //    if(m_errorString != "")
                    //    {
                    //        return null;
                    //    }
                    //}
                    //return null;
                    return null;
                }
            }
            SqlDataReader reader;
            DataTable dt = new DataTable();
            
            try
            {
                SqlCommand cmd = new SqlCommand();
                
                cmd.CommandTimeout = m_timeOut;
                cmd.Connection = m_Connect;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlTxt;
                if (m_Connect.State == ConnectionState.Closed)
                {
                    m_Connect = null;
                    connStatus = false;
                    return null;
                }
                reader = cmd.ExecuteReader(CommandBehavior.Default);
                dt.Load(reader);
                reader.Close();
            }
            catch (SqlException sqlex)
            {

                if (m_Connect.State == ConnectionState.Broken || m_Connect.State == ConnectionState.Closed)
                {
                    connStatus = false;
                    m_Connect = null;
                }
                m_errorString = "执行错误!" + sqlex.Message;
                return null;
            }
            m_errorString = "";
            return dt;
        }
        public static bool ifObjectExists(string objectName)
        {
            return getTable("select 1 from sysObjects where Id=OBJECT_ID(N'" + objectName + "')").Rows.Count > 0 ?true:false;
        }

        public static bool DatabaseExist(string DbName)
        {

            return getTable("select 1 from sys.databases where name=N'" + DbName + "'").Rows.Count > 0 ? true : false;
        }
        public static bool CheckDB()
        {
            //检测需要的表，存储哦过程是否存在
            try
            {
                if (Tools_SQL_Class.ifObjectExists("t_Base_User") &&
                Tools_SQL_Class.ifObjectExists("icstockbill") &&
                Tools_SQL_Class.ifObjectExists("icstockbillentry"))
                {
                    if (!Tools_SQL_Class.ifObjectExists("t_user_pda_password"))
                    {
                        getTable(
                            "create table t_user_pda_password" +
                            "(" +
                            "fid  int identity(0, 1)," +
                            "FUserFitemID int not null unique," +
                            "FPassword nvarchar(255) default('')," +
                            "foreign key(FUserFitemID)  references t_Base_User(FUserID)" +
                            ");");
                        getTable(
                            "insert into t_user_pda_password(FUserFitemID)" +
                            "select FUserID from t_Base_User;");
                    }
                    if (!Tools_SQL_Class.ifObjectExists("ZZ_KIngHoo_LookUpInventory"))
                    {
                        var sqlTxt = Resource_Res.ZZ_KIngHoo_LookUpInventory;
                        if (Tools_SQL_Class.directconnect())
                        {
                            var ret = Tools_SQL_Class.directGetTable(sqlTxt);
                            if (ret == null)
                            {
                                Log.Debug("Ms", Tools_SQL_Class.m_errorString);
                            }
                        }
                    }
                    if (!Tools_SQL_Class.ifObjectExists("ZZ_KIngHoo_LookUpInventory_Summary"))
                    {
                        var sqlTxt = Resource_Res.ZZ_KIngHoo_LookUpInventory_Summary;
                        if (Tools_SQL_Class.directconnect())
                        {
                            var ret = Tools_SQL_Class.directGetTable(sqlTxt);
                            if (ret == null)
                            {
                                Log.Debug("Ms", Tools_SQL_Class.m_errorString);
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
                return false;
            }
            catch
            {
                return false;
            }
            

        }

        //
        public static string TransationAutoCommit(string [] sqlTxt)
        {
            if (!Status())
            {
                connect();
            }
            if (!Status()) return "not connected";

            SqlCommand command = m_Connect.CreateCommand();
            SqlTransaction transaction;

            transaction = m_Connect.BeginTransaction("_Transaction");

            command.Connection = m_Connect;
            command.Transaction = transaction;

            string _sql = "use " + m_DbName + ";";
            foreach(var i in sqlTxt)
            {
                _sql += i;
            }
            try
            {
                command.CommandText = _sql;
                command.ExecuteNonQuery();
                // Attempt to commit the transaction.
                transaction.Commit();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                m_errorString = ex.Message;

                // Attempt to roll back the transaction.
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    m_errorString = ex2.Message;
                    return ex2.Message;
                }
                return ex.Message;
            }
            return "";
        }

        private static void SetCommand(SqlCommand cmd, string cmdText, CommandType cmdType, SqlParameter[] cmdParms)
        {
            cmd.Connection = m_Connect;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                cmd.Parameters.AddRange(cmdParms);
            }
        }
        public static object ExecutProcedure(string cmdText, CommandType cmdType, SqlParameter[] cmdParms) 
        {
            if (!Status())
            {
                connect();
            }
            if (!Status()) return null;
            object Ret;
            try
            {
                var cmd = new SqlCommand();
                SetCommand(cmd, cmdText, cmdType, cmdParms);
                Ret = cmd.ExecuteScalar();
            }
            catch(SqlException sex)
            {
                return null;
            }
            return Ret;
        }
        //
    }
}