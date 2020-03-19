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
using Android.Util;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Tools_Extend_Storage
    {
        private ISharedPreferences m_shareData = null;
        private ISharedPreferencesEditor m_shareDataEditor = null;
        public Tools_Extend_Storage(Context context)
        {
            m_shareData = PreferenceManager.GetDefaultSharedPreferences(context);
            m_shareDataEditor = m_shareData.Edit();
        }
        public enum ValueType{
            login_userName = 1,
            login_userPassword = 2,
            login_databaseName = 3,
            login_databaseAddress = 4,
            login_autoLogin = 5, 
            login_autoLogin_Name = 6,
            login_recordPassword = 7, 
            login_recordPassword_password = 8,
            userProfile_ID = 9,
            userProfile_Name = 10, 
            userProfile_Description = 11, 
            login_databaseUserName = 12, 
            login_databaseUserPassword =13,
            login_selectedUserID = 14,
            login_selectedUserName = 15,
            login_software_version =16
        }
        //private static ISharedPreferences m_shareData = PreferenceManager.GetDefaultSharedPreferences(this);
        public void saveValue( ValueType type, string value) {
            m_shareDataEditor.PutString(type.ToString(), value);
            m_shareDataEditor.Apply();
            
        }
        public void saveValue( ValueType type, bool value)
        {
            m_shareDataEditor.PutBoolean(type.ToString(), value);
            m_shareDataEditor.Apply();
        }
        public void saveValue( ValueType type, float value)
        {
            m_shareDataEditor.PutFloat(type.ToString(), value);
            m_shareDataEditor.Apply();
        }
        public void saveValue( ValueType type, ICollection<string> value)
        {
            m_shareDataEditor.PutStringSet(type.ToString(), value);
            m_shareDataEditor.Apply();
        }
        public void saveValue( ValueType type, long value)
        {
            m_shareDataEditor.PutLong(type.ToString(), value);
            m_shareDataEditor.Apply();
        }
        public void saveValue( ValueType type, int value)
        {
            m_shareDataEditor.PutInt(type.ToString(), value);
            m_shareDataEditor.Commit();
        }

        public string getValueString( ValueType type)
        {
            return m_shareData.GetString(type.ToString(), "");
        }
        public int getValueInt( ValueType type)
        {
            return m_shareData.GetInt(type.ToString(), 0);
        }
        public float getValueFloat( ValueType type)
        {
            return m_shareData.GetFloat(type.ToString(), 0);
        }
        public long getValueLong( ValueType type)
        {
            return m_shareData.GetLong(type.ToString(), 0);
        }
        public ICollection<string> getValueStringSet( ValueType type)
        {
            return m_shareData.GetStringSet(type.ToString(), null);
        }
        public bool getValueBoolean( ValueType type)
        {
            return m_shareData.GetBoolean(type.ToString(), false);
        }
    }
}