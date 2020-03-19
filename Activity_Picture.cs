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
using Android.Support.V7.Widget;
using Android.Support.V7.App;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Activity_Picture : AppCompatActivity
    {
        public Activity_Picture()
        {

        }
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            

        }
        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.activity_picture);

        }
    }
}