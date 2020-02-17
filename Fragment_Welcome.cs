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

namespace Android_KingHoo_Scanner_Rebuild
{
    class Fragment_Welcome :Android.Support.V4.App.Fragment
    {
        private static Fragment_Welcome m_instance = null;

        public static Fragment_Welcome Instance()
        {
            if (m_instance == null)
            {
                m_instance = new Fragment_Welcome();
            }
            return m_instance;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            m_instance = this;
            View v = inflater.Inflate(Resource.Layout.activity_main_welcome, container, false);
            return v;
        }
    }
}