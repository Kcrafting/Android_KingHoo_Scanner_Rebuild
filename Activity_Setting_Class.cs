﻿using System;
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
        Spinner m_DatabaseList = null,m_software_select = null,m_PDA_select = null;
        List<Tools_Tables_Adapter_Class.Account_Detail> m_AccountList = new List<Tools_Tables_Adapter_Class.Account_Detail>();
        List<Tools_Tables_Adapter_Class.Software_Version> software_VersionsList = new List<Tools_Tables_Adapter_Class.Software_Version>();
        List<Tools_Tables_Adapter_Class.PDA_Version> PDA_VersionsList = new List<Tools_Tables_Adapter_Class.PDA_Version>();
        Tools_Extend_Storage m_Tes = null;
        Button m_auth = null;
        //是否第一次启动
        bool m_first_start = false;
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
            //软件版本选择
            m_software_select = FindViewById<Spinner>(Resource.Id.activity_setting_server_software_version_select);
            m_software_select.ItemSelected += M_software_select_ItemSelected;
            //激活设备
            m_auth = FindViewById<Button>(Resource.Id.CertifieDevice);
            m_auth.Click += M_auth_Click;
            //PDA版本选择
            m_PDA_select = FindViewById<Spinner>(Resource.Id.activity_setting_pda_version_select);
            m_PDA_select.ItemSelected += M_PDA_select_ItemSelected;

            software_VersionsList.Add(new Tools_Tables_Adapter_Class.Software_Version(0, ""));
            software_VersionsList.Add(new Tools_Tables_Adapter_Class.Software_Version(1, "Kis旗舰版6.0+"));
            software_VersionsList.Add(new Tools_Tables_Adapter_Class.Software_Version(2, "K3 Wise 14.0+"));

            PDA_VersionsList.Add(new Tools_Tables_Adapter_Class.PDA_Version(0, ""));
            PDA_VersionsList.Add(new Tools_Tables_Adapter_Class.PDA_Version(1, "U8000S"));
            PDA_VersionsList.Add(new Tools_Tables_Adapter_Class.PDA_Version(2, "AMS-N60"));

            m_software_select.Adapter = new Tools_Tables_Adapter_Class.Software_Version_List(this,  software_VersionsList);
            m_PDA_select.Adapter = new Tools_Tables_Adapter_Class.PDA_Version_List(this, PDA_VersionsList);

            if (m_Tes.getValueInt(Tools_Extend_Storage.ValueType.login_software_version) != 0)
            {
                m_software_select.SetSelection(m_Tes.getValueInt(Tools_Extend_Storage.ValueType.login_software_version));
            }
            var ret = m_Tes.getValueInt(Tools_Extend_Storage.ValueType.login_pda_version);
            if (ret != 0)
            {
                m_PDA_select.SetSelection(ret);
            }

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

        private void M_PDA_select_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_pda_version, ((Spinner)sender).SelectedItemPosition);
        }

        private void M_auth_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(Activity_Authentication)));
        }

        private void M_software_select_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_software_version, ((Spinner)sender).SelectedItemPosition);
        }

        public override void OnBackPressed()
        {
            StartActivity(new Intent(Application.Context, typeof(Activity_Splash_Class)));
        }

        private void CancelAutoLogin_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
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
            //m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_pda_version,)
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
                Tools_Tables_Adapter_Class.ShowMsg(this,"错误","设置成功！返回后将再次尝试登录！");
            }
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            var prograss = new Tools_Tables_Adapter_Class.ShowPrograss(this);
            int t_i = m_Tes.getValueInt(Tools_Extend_Storage.ValueType.login_software_version);
            prograss.Show();
           if (m_HostIP.Text != "" &&
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
                        if ((t_i==2 && Tools_SQL_Class.DatabaseExist("KDAcctDB")) || (t_i==1 && Tools_SQL_Class.DatabaseExist("AcctCtl_KEE")))
                        {
                            if(t_i == 1)
                            {
                                Tools_SQL_Class.m_DbName = "AcctCtl_KEE";
                            }
                            if(t_i == 2)
                            {
                                Tools_SQL_Class.m_DbName = "KDAcctDB";
                            }
                            
                            if (Tools_SQL_Class.connect())
                            {
                                if (Tools_SQL_Class.ifObjectExists("dbo.t_ad_kdAccount_gl"))
                                {
                                    var DBList = Tools_SQL_Class.getTable("select FAcctName,FDBName from t_ad_kdAccount_gl");
                                    if(DBList==null && DBList.Rows.Count == 0)
                                    {
                                        return;
                                    }
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
                                        if (!m_first_start)
                                        {
                                            m_DatabaseList.SetSelection(m_AccountList.FindIndex(a => a.FDBName == m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseName)));
                                            m_first_start = true;
                                        }
                                        prograss.Dismiss();
                                    });
                                }
                            }
                        }
                        else
                        {
                            RunOnUiThread(() => {
                                prograss.Dismiss();
                                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "没有找到账套管理数据库！");
                            });
                        }
                    }
                    else
                    {
                        RunOnUiThread(()=> {
                            Tools_Tables_Adapter_Class.ShowMsg(this,"错误",Tools_SQL_Class.ErrorString());
                            prograss.Dismiss();
                        });
                    }
                }
                )).Start();
            }
            else
            {
                prograss.Dismiss();
            }
        }
    }
}