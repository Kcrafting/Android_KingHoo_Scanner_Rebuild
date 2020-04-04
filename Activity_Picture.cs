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
using System.Threading;
using Java.Net;
using Android.Graphics;
//using Java.IO;

using Java.Util.Zip;
using System.IO;
using System.IO.Compression;
using Android.Media;
//using System.Drawing;

namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/company_main_title", Theme = "@style/KingHooTheme.ShowPicture", MainLauncher = false)]
    class Activity_Picture : AppCompatActivity
    {
        public static string m_Fnumber = "";
        public static string m_picturepath = "";
        private ImageView m_image;
        public Activity_Picture()
        {
            //"activity_itemSelect_lookpicture_imageView"
        }
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

            //var aa = "0xff";
        }
        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.activity_picture);
            m_image = FindViewById<ImageView>(Resource.Id.activity_itemSelect_lookpicture_imageView);
            //m_image.SetImageURI(Android.Net.Uri.Parse("dss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo_top-e3b63a0b1b.png"));
            //setPicture(@"https://dss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo_top-e3b63a0b1b.png");
            var t = new Thread(()=> {
                var table = Tools_SQL_Class.getTable("SELECT top 1 FImageData FROM t_Accessory_Decrypt where FNumber='" + m_Fnumber + "'");
                if (table != null && table.Rows.Count > 0)
                {
                    var image = (byte[])table.Rows[0]["FImageData"];
                    MemoryStream ms = new MemoryStream(image);
                    //Bitmap bmpt = new Bitmap(ms);
                    Android.Graphics.Bitmap bitmap = BitmapFactory.DecodeStream(ms);
                    RunOnUiThread(() => {
                        m_image.SetImageBitmap(bitmap);
                    });
                }
            });
            t.IsBackground = true;
            t.Start();
        }



        private void setPicture(string netpath)
        {
            var t = new Thread(()=> {
                URL url = new URL(netpath);
                HttpURLConnection connection = (HttpURLConnection)url.OpenConnection();
                connection.RequestMethod = "GET";
                connection.ConnectTimeout = 10000;
                //connection.SetRequestProperty("GET");
                //超时时间为10秒s
                //connection.SetConnectTimeout(10000);
                var code = connection.ResponseCode;
                if(code == HttpStatus.Ok)
                {
                    var inputStream = connection.InputStream;
                    Android.Graphics.Bitmap bitmap = BitmapFactory.DecodeStream(inputStream);
                    RunOnUiThread(()=> {
                        m_image.SetImageBitmap(bitmap);
                    });
                   
                }
                else
                {

                }

            });
            t.IsBackground = true;
            t.Start();
        }
    }
}