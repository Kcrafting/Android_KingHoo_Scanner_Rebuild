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


namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/acticity_setting_changepassword_title", Theme = "@style/AppTheme", NoHistory = true)]
    class Activity_Setting_ChangePassword_Class : AppCompatActivity
    {
        Tools_Extend_Storage m_Tes = null;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

        }
        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.activity_setting_changepassword);
            m_Tes = new Tools_Extend_Storage(this);
            var origin = FindViewById<EditText>(Resource.Id.acticity_setting_changepassword_origin_password);
            var newPassword = FindViewById<EditText>(Resource.Id.acticity_setting_changepassword_new_password);
            var confirmPassword = FindViewById<EditText>(Resource.Id.acticity_setting_changepassword_repeat_password);
            var saveChange = FindViewById<EditText>(Resource.Id.acticity_setting_changepassword_save_change);
            var account = FindViewById<TextView>(Resource.Id.acticity_setting_changepassword_account);

            var userId = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_selectedUserID);
            var userName = m_Tes.getValueString(Tools_Extend_Storage.ValueType.login_selectedUserName);

            account.Text = userName;

            if (saveChange != null)
            {
                saveChange.Click += new EventHandler(delegate {
                    if(newPassword.Text!= confirmPassword.Text)
                    {
                        Tools_Tables_Adapter_Class.ShowMsg(this,"错误","您新密码两次的输入不一致！");
                    }
                    else
                    {
                        if (Tools_SQL_Class.Status())
                        {
                           
                            if(userId == "")
                            {
                                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "新还没有选择任何的账户！");
                                return;
                            }
                            var ret = Tools_SQL_Class.getTable("select 1 from t_user_pda_password where FUserFitemID = " + userId + " and FPassword = '" + origin.Text + "'");
                            if (ret != null)
                            {
                                if (ret.Rows.Count > 0)
                                {
                                    var _ret = Tools_SQL_Class.getTable("update t_user_pda_password set FPassword = '" + confirmPassword.Text + "' where FUserFitemID=" + userId);
                                    if( _ret != null && _ret.Rows.Count > 0)
                                    {
                                        //var __ret = Tools_SQL_Class.getTable("");
                                        //Tools_Tables_Adapter_Class.ShowMsg
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        public override void OnBackPressed()
        {
            StartActivity(new Intent(Application.Context, typeof(Activity_Login_Class)));
        }
    }
}
