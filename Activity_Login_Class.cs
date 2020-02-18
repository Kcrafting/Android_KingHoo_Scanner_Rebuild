using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/activity_splash_title", Theme = "@style/KingHooTheme.NoTitle", NoHistory = true)]
    class Activity_Login_Class : AppCompatActivity
    {
        private Spinner m_AccountSelect = null;
        Tools_Extend_Storage m_Tes = null;
        bool m_autoLogin = false, m_recordPassword = false;
        List<Tools_Tables_Adapter_Class.Login_Account> m_AccountsList_ = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        protected override void OnResume()
        {
            base.OnResume();
            m_Tes = new Tools_Extend_Storage(this);

            //var sqlite = new Tools_SQLite_Class();
            //sqlite.InitializeDatabase();
            //sqlite.AddData("hello world hello world");
            //sqlite.GetData();

            Tools_SQL_Class.m_Host = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseAddress);
            Tools_SQL_Class.m_UserName = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseUserName);
            Tools_SQL_Class.m_UserPassword = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseUserPassword);
            Tools_SQL_Class.m_DbName = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_databaseName);

            SetContentView(Resource.Layout.activity_login);
            var button_setting = FindViewById<Button>(Resource.Id.acticity_login_setting);
            button_setting.Click += Button_setting_Click;
            var button_login = FindViewById<Button>(Resource.Id.acticity_login_login);
            button_login.Click += Button_login_Click;
            m_AccountSelect = FindViewById<Spinner>(Resource.Id.acticity_login_account_select);
            var checkbox_autoLogin = FindViewById<CheckBox>(Resource.Id.acticity_login_auto_loin);
            var checkbox_recordPassword = FindViewById<CheckBox>(Resource.Id.acticity_login_record_password);
            m_autoLogin = m_Tes.getValueBoolean(Tools_Extend_Storage.ValueType.login_autoLogin);
            m_recordPassword = m_Tes.getValueBoolean(Tools_Extend_Storage.ValueType.login_recordPassword);
            checkbox_recordPassword.Checked = m_recordPassword;
            checkbox_recordPassword.CheckedChange += Checkbox_recordPassword_CheckedChange;
            checkbox_autoLogin.Checked = m_autoLogin ;
            checkbox_autoLogin.CheckedChange += Checkbox_autoLogin_CheckedChange;

            var editText_password = FindViewById<AutoCompleteTextView>(Resource.Id.acticity_login_account_password);

            new Thread(new ThreadStart(()=> {
                if (Tools_SQL_Class.Status() && Tools_Tables_Adapter_Class.m_DatabaseExist && Tools_Tables_Adapter_Class.m_DatabaseLegal)
                {
                    if (Tools_Tables_Adapter_Class.m_login_userList != null)
                    {
                        setUserList();
                        RunOnUiThread(()=> {
                            m_AccountSelect.SetSelection(m_AccountsList_.FindIndex(a => a.UserID == m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_autoLogin_Name)));
                            if (m_recordPassword)
                            {
                                editText_password.Text = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_recordPassword_password);
                            }
                            if (m_autoLogin)
                            {
                                Button_login_Click(null, null);
                            }
                        });
                        
                    }
                    else
                    {
                        Tools_Tables_Adapter_Class.m_login_userList = Tools_SQL_Class.getTable("select FUserID,FName from t_Base_User;");
                        if (Tools_Tables_Adapter_Class.m_login_userList != null)
                        {
                            setUserList();
                            RunOnUiThread(()=> {
                                m_AccountSelect.SetSelection(m_AccountsList_.FindIndex(a => a.UserID == m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_autoLogin_Name)));
                                if (m_recordPassword)
                                {
                                    editText_password.Text = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_recordPassword_password);
                                }
                                if (m_autoLogin)
                                {
                                    Button_login_Click(null,null);
                                }

                            });
                            
                        }
                    }
                }
                else
                {
                    RunOnUiThread(()=> {
                        Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "连接还没有准备就绪");
                    });
                    
                }

                //while (!Tools_SQL_Class.Status())
                //{
                //    Tools_SQL_Class.connect();
                //    if (Tools_SQL_Class.Status())
                //    {
                //        break;
                //    }
                //    else
                //    {
                //        Thread.Sleep(6000);
                //    }
                //}
                //if (Tools_Tables_Adapter_Class.m_login_userList != null)
                //{
                //    setUserList();
                //}
                //else
                //{
                    
                //}
            })).Start();
            if (checkbox_recordPassword.Checked)
            {
                editText_password.Text = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_recordPassword_password);
                if (checkbox_autoLogin.Checked)
                {
                    //自动登录
                    Button_login_Click(null, null);
                }
            }

        }

        private void Checkbox_recordPassword_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_recordPassword, ((CheckBox)sender).Checked);
            m_recordPassword = ((CheckBox)sender).Checked;
        }

        private void Checkbox_autoLogin_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_autoLogin, ((CheckBox)sender).Checked);
            m_autoLogin = ((CheckBox)sender).Checked;
        }

        private void Button_login_Click(object sender, EventArgs e)
        {
            if (Tools_Tables_Adapter_Class.m_AccountListed && m_AccountSelect.SelectedItem!=null)
            {
                var UserID = ((Tools_Tables_Adapter_Class.Login_Account)m_AccountSelect.SelectedItem).UserID;
                MainActivity.m_CurrentUserID = UserID;
                var password = FindViewById<AutoCompleteTextView>(Resource.Id.acticity_login_account_password).Text;
                var userProfile = Tools_SQL_Class.getTable("select B.FName,B.FUserID,B.FDescription from t_user_pda_password A join t_Base_User B  on  A.FUserFitemID=B.FUserID where A.FUserFitemID=" + UserID.ToString() + " and A.FPassword='" +
                    password + "'");
                if(userProfile == null)
                {

                }
                else
                {
                    if (userProfile.Rows.Count > 0)
                    {
                        m_Tes.saveValue(Tools_Extend_Storage.ValueType.userProfile_ID, userProfile.Rows[0]["FUserID"].ToString());
                        m_Tes.saveValue(Tools_Extend_Storage.ValueType.userProfile_Name, userProfile.Rows[0]["FName"].ToString());
                        m_Tes.saveValue(Tools_Extend_Storage.ValueType.userProfile_Description, userProfile.Rows[0]["FDescription"].ToString());

                        if (m_recordPassword)
                        {
                            m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_recordPassword_password, password);
                        }
                        if (m_autoLogin)
                        {
                           
                        }
                        m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_autoLogin_Name, userProfile.Rows[0]["FUserID"].ToString());
                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }
                    else
                    {
                        Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "账户信息错误！");
                    }
                }
            }
            else
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "还没能成功获取账号信息");
            }
            return;

        }

        private void Button_setting_Click(object sender, EventArgs e)
        {
            var userid = m_AccountSelect.SelectedItem;
            if (userid != null)
            {
                m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_selectedUserID, ((Tools_Tables_Adapter_Class.Login_Account)userid).UserID);
                m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_selectedUserName, ((Tools_Tables_Adapter_Class.Login_Account)userid).UserName);
            }
            StartActivity(new Intent(Application.Context, typeof(Activity_Setting_Class)));
        }

        public override void OnBackPressed()
        {

        }

        #region function
        private void setUserList()
        {
            //Log.Debug(m_Tag, "查询成功！");
            m_AccountsList_ = new List<Tools_Tables_Adapter_Class.Login_Account>();
            for (int i = 0; i < Tools_Tables_Adapter_Class.m_login_userList.Rows.Count; i++)
            {
                var item = new Tools_Tables_Adapter_Class.Login_Account();
                item.UserName = Tools_Tables_Adapter_Class.m_login_userList.Rows[i]["FName"].ToString();
                item.UserID = Tools_Tables_Adapter_Class.m_login_userList.Rows[i]["FUserID"].ToString();
                m_AccountsList_.Add(item);
            }
            Tools_Tables_Adapter_Class.Login_AccountList userslist = new Tools_Tables_Adapter_Class.Login_AccountList(this, m_AccountsList_);
            RunOnUiThread(() =>
            {
                m_AccountSelect.Adapter = userslist;
            });
            Tools_Tables_Adapter_Class.m_AccountListed = true;
        }
        #endregion
    }
}