using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Views;
using Android.Widget;

namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/acticity_login_setting_label", Theme = "@style/AppTheme", NoHistory = true)]
    class Activity_Authentication : AppCompatActivity
    {
        public EditText m_projectname = null;
        TextView m_showstate = null;
        public Button m_submit = null,m_updateusedate=null,m_updateservicedate=null;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
           
            //var aa = "0xff";
        }
        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.activity_authentication);
            m_projectname = FindViewById<EditText>(Resource.Id.ProjectName);
            m_submit = FindViewById<Button>(Resource.Id.authdevice);
            m_submit.Click += M_submit_Click;
            m_showstate = FindViewById<TextView>(Resource.Id.showAuthState);
            m_updateusedate = FindViewById<Button>(Resource.Id.updateauthusedate);
            m_updateservicedate = FindViewById<Button>(Resource.Id.updateauthservicedate);
            var tes = new Tools_Extend_Storage(this);
            if (tes.getValueBoolean(Tools_Extend_Storage.ValueType.CertifiedFinish))
            {
                m_submit.Enabled = false;
                m_projectname.Enabled = false;
                m_submit.Text = m_submit.Text + "(已激活)";
            }
            var pj = tes.getValueString(Tools_Extend_Storage.ValueType.CertifiedProjectName);
            if (pj != "")
            {
                m_projectname.Text = pj;
            }
        }
        public string getsn()
        {
            TelephonyManager telephonyMgr = this.GetSystemService(Context.TelephonyService) as TelephonyManager;
            string deviceId = telephonyMgr.DeviceId == null ? "UNAVAILABLE" : telephonyMgr.DeviceId;
            return deviceId;
        }

        private void M_submit_Click(object sender, EventArgs e)
        {
            Tools_Tables_Adapter_Class.ShowDialog(this, "注意", "是否确认激活？","确定","取消", () => {
                var projectname = m_projectname.Text;
                if (projectname != "")
                {
                    var tes = new Tools_Extend_Storage(this);

                    tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedProjectName, projectname);
                    var dialog = new Tools_Tables_Adapter_Class.ShowPrograss(this);
                    var sn = getsn();
                    dialog.Show();
                    var t = new Thread(async () => {
                        await Tools_Github.AuthDevice(projectname, sn, this, m_showstate, dialog);
                        //RunOnUiThread(() =>
                        //{
                        //    m_showstate.Text = "激活成功！";
                        //    dialog.Hide();
                        //});
                    });
                    t.IsBackground = false;
                    t.Start();

                }
            }, () => { });

        }
    }
}