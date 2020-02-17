using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Fragment_Setting_InApp : Android.Support.V4.App.Fragment
    {
        private static Fragment_Setting_InApp m_instance = null;
        Switch m_autoLogin = null;
        Tools_Extend_Storage m_Tes = null;
        public static Fragment_Setting_InApp Instance()
        {
            if (m_instance == null)
            {
                m_instance = new Fragment_Setting_InApp();
            }
            return m_instance;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.activity_setting_inapp, container, false);
            m_autoLogin = v.FindViewById<Switch>(Resource.Id.activity_setting_inapp_autologin);
            m_autoLogin.CheckedChange += M_autoLogin_CheckedChange;
            m_Tes = new Tools_Extend_Storage(Activity);
            if (m_Tes.getValueBoolean(Tools_Extend_Storage.ValueType.login_autoLogin))
            {
                m_autoLogin.Checked = true;
            }
            else
            {
                m_autoLogin.Checked = false;
            }

            return v;
        }

        private void M_autoLogin_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (((Switch)sender).Checked)
            {
                m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_autoLogin, true);
            }
            else
            {
                m_Tes.saveValue(Tools_Extend_Storage.ValueType.login_autoLogin, false);
            }
        }
    }
}