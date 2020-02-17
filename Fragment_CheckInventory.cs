using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Fragment_CheckInventory: Android.Support.V4.App.Fragment
    {
        private static Fragment_CheckInventory m_instance = null;
        EditText m_scancode = null;
        TextView m_FNumber = null,m_FName = null,m_FModel = null;
        bool m_ThreadRunning = false;
        TextView m_selectitem_button = null;
        LinearLayout m_sub_main_layout = null;
        
        public static Fragment_CheckInventory Instance()
        {
            if(m_instance == null)
            {
                m_instance = new Fragment_CheckInventory();
            }
            return m_instance;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
                if (resultCode == 0)
                {
                    if (data != null)
                    {
                        var ret = data.GetStringExtra("FNumber");
                        m_scancode.Text = ret;
                        Ms_thisPointer_g_ProcessReciveData(ret);
                    }
                }
            }
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.activity_main_check_inventory_layout, container, false);
            m_scancode = v.FindViewById<EditText>(Resource.Id.activity_main_check_inventory_layout_item_scan_fnumber);
            m_container = v.FindViewById<RecyclerView>(Resource.Id.activity_main_check_inventory_container);
            m_selectitem_button = v.FindViewById<TextView>(Resource.Id.activity_main_check_inventory_layout_item_select);
            MainActivity.ms_thisPointer.g_ProcessReciveData += Ms_thisPointer_g_ProcessReciveData;
            m_FNumber = v.FindViewById<TextView>(Resource.Id.activity_main_check_inventory_layout_item_fnumber);
            m_FName = v.FindViewById<TextView>(Resource.Id.activity_main_check_inventory_layout_item_fname);
            m_FModel = v.FindViewById<TextView>(Resource.Id.activity_main_check_inventory_layout_item_fmodel);
            m_sub_main_layout = v.FindViewById<LinearLayout>(Resource.Id.activity_main_check_inventory_layout_sub_main);
            //m_sub_main_layout.SetMinimumHeight(0);
            m_selectitem_button.Click += M_selectitem_button_Click;
            //m_scancode.FocusChange += M_scancode_FocusChange;
            //new Thread(new ThreadStart(() => { 
            //})).Start();
            return v;
        }

        private void M_scancode_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                //m_scancode.ClearFocus();
                StartActivityForResult(new Intent(Application.Context, typeof(Activity_ItemSelect_Class)), 0);
                
            }
        }

        private void M_selectitem_button_Click(object sender, EventArgs e)
        {
            var intent = new Intent(Application.Context, typeof(Activity_ItemSelect_Class));
            intent.PutExtra("Type",Tools_Tables_Adapter_Class.ItemType.ICItem);
            StartActivityForResult(intent, 0);
            //StartActivity(new Intent(Application.Context, typeof(Activity_ItemSelect_Class)));
        }

        List<Tools_Tables_Adapter_Class.Inventroy_Detail_Row> m_List_Inventroy_Detail_Row = new List<Tools_Tables_Adapter_Class.Inventroy_Detail_Row>();
        private RecyclerView m_container = null;
        private void Ms_thisPointer_g_ProcessReciveData(string data)
        {
            m_scancode.Text = data;
            //处理扫描的条码
            if (!m_ThreadRunning)
            {
                m_ThreadRunning = true;
                var Thr = new Thread(new ThreadStart(() => {
                    try
                    {
                        if (Tools_SQL_Class.getTable("select 1 from t_item where fnumber = '" + data + "'").Rows.Count > 0)
                        {
                            var ret1 = Tools_SQL_Class.getTable("select FNumber,FName,FModel from t_ICItem where FNumber='" + data + "'");
                            if (ret1 != null && ret1.Rows.Count>=1)
                            {
                                Activity.RunOnUiThread(() => {
                                    m_FNumber.Text = ret1.Rows[0]["FNumber"].ToString();
                                    m_FName.Text = ret1.Rows[0]["FName"].ToString();
                                    m_FModel.Text = ret1.Rows[0]["FModel"].ToString();
                                });
                            }
                            var ret = Tools_SQL_Class.getTable("exec ZZ_KIngHoo_LookUpInventory '" + data + "'");
                            m_List_Inventroy_Detail_Row.Clear();
                            if (ret != null && ret.Rows.Count>=1)
                            {
                                for (int i = 0; i < ret.Rows.Count; i++)
                                {
                                    var row = new Tools_Tables_Adapter_Class.Inventroy_Detail_Row();
                                    row.m_Bill_Stock = ret.Rows[i]["仓库/单据"].ToString();
                                    row.m_MainUint = ret.Rows[i]["主计量"].ToString();
                                    row.m_SecondUint = ret.Rows[i]["辅助计量"].ToString();
                                    row.m_StockPlace = ret.Rows[i]["仓位"].ToString();
                                    row.m_Batch = ret.Rows[i]["批号"].ToString();
                                    m_List_Inventroy_Detail_Row.Add(row);
                                }
                                var adapter = new Tools_Tables_Adapter_Class.CheckInventory_Adapter(Activity, m_List_Inventroy_Detail_Row);
                                Activity.RunOnUiThread(()=> {
                                    m_container.SetLayoutManager(new LinearLayoutManager(Activity));
                                    m_container.SetAdapter(adapter);
                                });
                            }
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        m_ThreadRunning = false;
                    }

                }));
                Thr.IsBackground = true;
                Thr.Start();
            }
            
        }

        public override void OnPause()
        {
            MainActivity.ms_thisPointer.g_ProcessReciveData -= Ms_thisPointer_g_ProcessReciveData;
            // Code omitted for clarity
            base.OnPause();
        }
    }
}