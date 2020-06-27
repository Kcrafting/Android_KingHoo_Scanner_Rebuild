using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using System.Threading;
using System.Threading.Tasks;
using Android.Util;
using System.Data;
using System.Data.SqlClient;


namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Theme = "@style/KingHooTheme.Splash", MainLauncher = true, NoHistory = true)]
    class Activity_Splash_Class : AppCompatActivity
    {
        //延迟时间
        //int m_timeDelay = 15;
        #region override
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            
        }
        protected override void OnResume()
        {
            base.OnResume();
            Tools_Extend_Storage tes = new Tools_Extend_Storage(this);
            //SetContentView(Resource.Layout.activity_splash);
            new Thread(new ThreadStart(()=> {
                var host = tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseAddress);
                var dbname = tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseName);
                var dbuser = tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseUserName);
                var software = tes.getValueInt(Tools_Extend_Storage.ValueType.login_software_version);
                    if (host != "" && dbname != "" && dbuser != "" && software !=0 )
                {
                    Tools_SQL_Class.m_Host = host;
                    Tools_SQL_Class.m_DbName = dbname;
                    Tools_SQL_Class.m_UserPassword = tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseUserPassword);
                    Tools_SQL_Class.m_UserName = dbuser;
                    if (Tools_SQL_Class.connect())
                    {
                        try
                        {
                            if(Tools_SQL_Class.DatabaseExist(tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseName)))
                            {
                                Tools_Tables_Adapter_Class.m_DatabaseExist = true;
                                if (Tools_SQL_Class.CheckDB())
                                {
                                    Tools_Tables_Adapter_Class.m_login_userList = Tools_SQL_Class.getTable("select FUserID,FName from t_Base_User;");
                                    Tools_Tables_Adapter_Class.m_DatabaseLegal = true;
                                }
                                else
                                {
                                    Tools_Tables_Adapter_Class.m_DatabaseLegal = false;
                                }
                            }
                            else
                            {
                                Tools_Tables_Adapter_Class.m_DatabaseExist = false;
                            }
                           
                        }catch//(SqlException sex)
                        {
                            
                        }
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    Thread.Sleep(3000);
                }
                RunOnUiThread(()=> {
                    StartActivity(new Intent(Application.Context, typeof(Activity_Login_Class)));
                });
            })).Start();
            //await Tools_Github.GetServerMsg();

            //var t = new Thread(async () => {
            // await   Tools_Github.GetServerMsg();
            //});
            //t.IsBackground = false;
            //t.Start();

        }
        public override void OnBackPressed() { }

        #endregion
        #region otherfunction
        //async void SimulateStartup()
        //{
        //    await Task.Delay(m_timeDelay); // Simulate a bit of startup work.
        //}
        #endregion
    }
}