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
using Android.Preferences;
using Android.Util;

namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/acticity_login_setting_label", Theme = "@style/AppTheme", NoHistory = true)]
    class Activity_Setting_Class: AppCompatActivity
    {
        AutoCompleteTextView m_HostIP = null;
        EditText m_UserName = null,m_UserPassword = null,m_Port = null;
        Spinner m_DatabaseList = null;
        List<Tools_Tables_Adapter_Class.Account_Detail> m_AccountList = new List<Tools_Tables_Adapter_Class.Account_Detail>();
        Tools_Extend_Storage m_Tes = null;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

        }
        protected override void OnResume()
        {
            base.OnResume();

            m_Tes = new Tools_Extend_Storage(this);
            SetContentView(Resource.Layout.activity_setting);
            var connect = FindViewById<Button>(Resource.Id.acticity_setting_connect);
            var save = FindViewById<Button>(Resource.Id.acticity_setting_save);
            var changePassword = FindViewById<Button>(Resource.Id.acticity_setting_changepassword);
            var cancelAutoLogin = FindViewById<Button>(Resource.Id.acticity_setting_cancelautologin);

            m_HostIP = FindViewById<AutoCompleteTextView>(Resource.Id.activity_setting_server_ip);
            m_UserName = FindViewById<EditText>(Resource.Id.acticity_setting_userName);
            m_UserPassword = FindViewById<EditText>(Resource.Id.acticity_setting_userpassword);
            m_Port = FindViewById<EditText>(Resource.Id.acticity_setting_port);
            m_DatabaseList = FindViewById<Spinner>(Resource.Id.acticity_setting_database);

            m_HostIP.Text = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseAddress);
            m_UserName.Text = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseUserName);
            m_UserPassword.Text = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseUserPassword);
            Connect_Click(null, null);
            //new Thread(new ThreadStart(()=> {
            //    if (!Tools_SQL_Class.Status())
            //    {
            //        Tools_SQL_Class.m_Host = m_Tes.getValueString(this, Tools_Extend_Storage.ValueType.login_databaseAddress);
            //        Tools_SQL_Class.m_UserName= m_Tes.getValueString(this, Tools_Extend_Storage.ValueType.login_databaseUserName);
            //        Tools_SQL_Class.m_UserPassword = m_Tes.getValueString(this, Tools_Extend_Storage.ValueType.login_databaseUserPassword); 
            //        if (!Tools_SQL_Class.connect())
            //        {
            //            return;
            //        }
            //    }
            //    Connect_Click(null,null);
            //})).Start();

            connect.Click += Connect_Click;
            save.Click += Save_Click;
            changePassword.Click += ChangePassword_Click;
            cancelAutoLogin.Click += CancelAutoLogin_Click;


        }

        public override void OnBackPressed()
        {
            StartActivity(new Intent(Application.Context, typeof(Activity_Splash_Class)));
        }

        private void CancelAutoLogin_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChangePassword_Click(object sender, EventArgs e)
        {
            var userId = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_selectedUserID);
            //var userName = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_selectedUserName);
            if(userId != "")
            {
                StartActivity(new Intent(Application.Context, typeof(Activity_Setting_ChangePassword_Class)));
            }
            else
            {
                Tools_Tables_Adapter_Class.ShowMsg(this,"错误","您还没有选择任何的账户，无法修改密码");
            }
           
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (Tools_SQL_Class.Status())
            {
                m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_databaseAddress, Tools_SQL_Class.m_Host);
                m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_databaseUserName, Tools_SQL_Class.m_UserName);
                m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_databaseUserPassword, Tools_SQL_Class.m_UserPassword);
                if (m_DatabaseList.SelectedItemPosition >= 0)
                {
                    var ret = m_AccountList.ElementAt<Tools_Tables_Adapter_Class.Account_Detail>(m_DatabaseList.SelectedItemPosition).FDBName;
                    m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_databaseName, ret);
                }
            }
        }

        private void Connect_Click(object sender, EventArgs e)
        {
           if(m_HostIP.Text != "" &&
              m_UserName.Text != "")
            {
                Tools_SQL_Class.m_Host = m_HostIP.Text;
                Tools_SQL_Class.m_UserName = m_UserName.Text;
                Tools_SQL_Class.m_UserPassword = m_UserPassword.Text;
                Tools_SQL_Class.m_DbName = "master";
                new Thread(new ThreadStart(() =>
                {
                    if (Tools_SQL_Class.connect())
                    {
                        if (Tools_SQL_Class.DatabaseExist("KDAcctDB"))
                        {
                            Tools_SQL_Class.m_DbName = "KDAcctDB";
                            if (Tools_SQL_Class.connect())
                            {
                                if (Tools_SQL_Class.ifObjectExists("dbo.t_ad_kdAccount_gl"))
                                {
                                    var DBList = Tools_SQL_Class.getTable("select FAcctName,FDBName from t_ad_kdAccount_gl");
                                    m_AccountList.Clear();
                                    for (int i = 0; i < DBList.Rows.Count; i++)
                                    {
                                        var item = new Tools_Tables_Adapter_Class.Account_Detail();
                                        item.FAcctName = DBList.Rows[i]["FAcctName"].ToString();
                                        item.FDBName = DBList.Rows[i]["FDBName"].ToString();
                                        m_AccountList.Add(item);
                                    }
                                    var adapter = new Tools_Tables_Adapter_Class.DataBaseAdapter<Tools_Tables_Adapter_Class.Account_Detail>(this,m_AccountList, Resource.Layout.activity_login_account_list_layout);
                                    RunOnUiThread(() =>
                                    {
                                        m_DatabaseList.Adapter = adapter;
                                        m_DatabaseList.SetSelection(m_AccountList.FindIndex(a => a.FDBName == m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseName)));
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        RunOnUiThread(()=> {
                            Tools_Tables_Adapter_Class.ShowMsg(this,"错误",Tools_SQL_Class.ErrorString());
                        });
                    }
                })).Start();
            }
        }
    }
}