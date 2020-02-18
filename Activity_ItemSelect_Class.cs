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
using Android.Support.V7.Widget;
using System.Threading;
//using System.Data;
using System.Data;
using Android.Util;
//  using System.Data;
//using System.Data;


namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/company_main_title", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.AdjustPan, MainLauncher = false)]

    class Activity_ItemSelect_Class : AppCompatActivity
    {
        RecyclerView m_litemlist = null;
        LinearLayout m_indexContainer = null;
        ListView m_listview = null;
        EditText m_search = null;
        string m_ItemType = "";
        public Activity_ItemSelect_Class()
        {

        }
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            m_ItemType = savedInstanceState.GetString("Type");
        }
        List<TextView> m_TextView_list = new List<TextView>();
        List<string> m_groupKey = new List<string>();
        List<Tools_Tables_Adapter_Class.Item> m_ItemList = new List<Tools_Tables_Adapter_Class.Item>();
        List<TextView> m_indexItem = new List<TextView>();
        bool m_AdapterInitFinsh = false;
        TextView m_search_textview = null;
        protected override void OnResume()
        {
            m_ItemType = this.Intent.GetStringExtra("Type");
            base.OnResume();

            SetContentView(Resource.Layout.activity_itemSelect);
            m_indexContainer = FindViewById<LinearLayout>(Resource.Id.activity_item_select_index);
            m_listview = FindViewById<ListView>(Resource.Id.activity_item_select_list_item);
            m_listview.ChoiceMode = ChoiceMode.Single;
            m_listview.ItemClick += M_listview_ItemClick;
            m_search = FindViewById<EditText>(Resource.Id.activity_item_select_search);
            m_search_textview = FindViewById<TextView>(Resource.Id.activity_item_select_search_tv);
            //m_search.KeyPress += M_search_KeyPress;
            m_search.AfterTextChanged += M_search_AfterTextChanged;
            m_search_textview.Click += M_search_textview_Click; ;
            //m_listview.Scroll += M_listview_Scroll;
            m_AdapterInitFinsh = false;
            //m_listview.Adapter

            //m_timer.Enabled = true;
            m_timer.Elapsed += M_timer_Elapsed;
            m_search.Enabled = false;

            switch (m_ItemType)
            {
                case Tools_Tables_Adapter_Class.ItemType.ICItem:
                    {
                        this.Title = "物料选择";
                        //ProcessItemInAnotherThread();
                        ProcessItemsInAnotherThread("select FNumber,FName from t_Item where FItemClassID=4 and FDetail = 0", "select B.FNumber,B.FName,A.FModel,A.FitemID from t_ICItem A full join t_Item B on A.FItemID=B.FItemID where B.FItemClassID=4", "FNumber", "FName", "FModel");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.ICStock:
                    {
                        this.Title = "仓库选择";
                        ProcessItemsInAnotherThread("select FNumber,FName from t_Item where FItemClassID=5 and FDetail = 0", "select B.FNumber,B.FName,B.FFullName,B.FitemID from t_Stock A full join t_Item B on A.FItemID=B.FItemID where B.FItemClassID=5", "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.ICStockPlace:
                    {
                        this.Title = "仓位选择";
                        ProcessItemsInAnotherThread("select fnumber,FName,FSPID FItemID from t_StockPlace where FDetail=0 and FSPID!=0", "select fnumber,FName,FFullName,FSPID FItemID from t_StockPlace where FSPID!=0", "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.User:
                    {
                        this.Title = "用户选择";
                        ProcessItemsInAnotherThread("select FNumber,FName from t_Item where FItemClassID=3 and FDetail = 0", "select FNumber,FName,FFullName,FitemID from t_Item where FItemClassID=3", "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.Supply:
                    {
                        this.Title = "选择供应商";
                        ProcessItemsInAnotherThread("select FNumber,FName,ffullname from t_Item where FItemClassID=8 and FDetail=0", "select FNumber,FName,ffullname,FItemID from t_Item where FItemClassID=8", "FNumber", "FName", "ffullname");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.Customer:
                    {
                        this.Title = "选择客户";
                        ProcessItemsInAnotherThread("select FNumber,FName,ffullname from t_Item where FItemClassID=1 and FDetail=0", "select FNumber,FName,ffullname,FItemID from t_Item where FItemClassID=1", "FNumber", "FName", "ffullname");
                    }
                    break;
                default:
                    break;
            }
        }

        private void M_search_textview_Click(object sender, EventArgs e)
        {
           
        }

        private DataTable m_dataTable_Group = null;
        private DataTable m_dataTable_Items = null;


        private void search_()
        {
            var words = m_search.Text;
            switch (m_ItemType)
            {
                case Tools_Tables_Adapter_Class.ItemType.ICItem:
                    {

                        words.Replace("'", "").Replace(",", "").Replace("[", "");
                        updateItemsInAnotherThread(words, "FNumber", "FName", "FModel");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.ICStock:
                    {

                        words.Replace("'", "").Replace(",", "").Replace("[", "");
                        updateItemsInAnotherThread(words, "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.ICStockPlace:
                    {

                        words.Replace("'", "").Replace(",", "").Replace("[", "");
                        updateItemsInAnotherThread(words, "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.User:
                    {

                        words.Replace("'", "").Replace(",", "").Replace("[", "");
                        updateItemsInAnotherThread(words, "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.Supply:
                    {

                        words.Replace("'", "").Replace(",", "").Replace("[", "");
                        updateItemsInAnotherThread(words, "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.Customer:
                    {

                        words.Replace("'", "").Replace(",", "").Replace("[", "");
                        updateItemsInAnotherThread(words, "FNumber", "FName", "FFullName");
                    }
                    break;
                default:
                    break;
            }
        }
        private void ProcessItemsInAnotherThread(string getGroupSql, string getItemsSql, string fnumber, string fname, string fextend,string fitemid = "FItemID")
        {
            var T = new Thread(new ThreadStart(() =>
            {
                if (Tools_SQL_Class.Status())
                {
                    m_dataTable_Group = Tools_SQL_Class.getTable(getGroupSql);
                    m_dataTable_Items = Tools_SQL_Class.getTable(getItemsSql);
                    m_ItemList.Clear();
                    if (m_dataTable_Items != null && m_dataTable_Items.Rows.Count >= 1)
                    {
                        for (int i = 0; i < m_dataTable_Items.Rows.Count; i++)
                        {
                            var it = new Tools_Tables_Adapter_Class.Item();
                            it.m_fitemid = m_dataTable_Items.Rows[i][fitemid] == null ? "" : m_dataTable_Items.Rows[i][fitemid].ToString();
                            it.m_fnumber = m_dataTable_Items.Rows[i][fnumber].ToString();
                            it.m_fname = m_dataTable_Items.Rows[i][fname].ToString();
                            it.m_fextend = m_dataTable_Items.Rows[i][fextend].ToString();
                            m_ItemList.Add(it);
                        }
                    }
                    var adapter = new Tools_Tables_Adapter_Class.ItemAdapter(this, m_ItemList, m_groupKey);
                    RunOnUiThread(() =>
                    {

                        m_listview.Adapter = adapter;
                        m_search.Enabled = true;
                    });
                    m_indexItem.Clear();
                    m_groupKey.Clear();
                    if (m_dataTable_Group != null && m_dataTable_Group.Rows.Count >= 1)
                    {
                        for (int i = 0; i < m_dataTable_Group.Rows.Count; i++)
                        {
                            m_groupKey.Add(m_dataTable_Group.Rows[i][fnumber].ToString());
                            var tv = new TextView(this);
                            tv.Text = m_dataTable_Group.Rows[i][fname].ToString() + "  " + m_dataTable_Group.Rows[i][fnumber].ToString();
                            tv.Click += Tv_Click;
                            tv.Gravity = GravityFlags.Right;
                            tv.TextAlignment = TextAlignment.Gravity;
                            RunOnUiThread(() =>
                            {
                                tv.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Gray));
                                m_indexContainer.AddView(tv);
                            });

                            m_indexItem.Add(tv);

                        }
                    }

                    m_AdapterInitFinsh = true;
                }
            }));
            T.IsBackground = true;
            T.Start();
        }
        System.Timers.Timer m_timer = new System.Timers.Timer(1000);
        private void M_search_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            
            m_timer.Stop();
            m_timer.Start();
        }

        private void M_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_timer.Stop();
            RunOnUiThread(()=> {
                m_search.Enabled = false;
            });
            search_();
        }

        //private void M_search_KeyPress(object sender, View.KeyEventArgs e)
        //{
        //    ((EditText)sender).Text += e.KeyCode.ToString();
        //    e.KeyCode.
        //        updateInAnotherThread(e.KeyCode , ((EditText)sender).Text);
        //}
        //Thread m_Last_Running_Thread = null;
        //Thread m_Current_Running_Thread = null;
        //string _Tag = "----------------------->";
        private void updateItemsInAnotherThread(string words, string fnumber, string fname, string fextend,string fitemid = "FItemID")
        {
            //Log.Error(_Tag, "enter updateItemsInAnotherThread ");
            var T = new Thread(new ThreadStart(() =>
            {
                //Log.Error(_Tag + m_Current_Running_Thread.ManagedThreadId.ToString(),"enter a new Thread" ) ;
                //if (m_Last_Running_Thread != null && m_Last_Running_Thread.IsAlive)
                //{
                //    Log.Error(_Tag + m_Current_Running_Thread.ManagedThreadId.ToString(), "开始处理上次未运行完成的线程" );
                //    while (m_Last_Running_Thread.IsAlive) ;
                //    Log.Error(_Tag + m_Current_Running_Thread.ManagedThreadId.ToString(), "上次运行的线程处理完成");
                //}
                //Log.Error(_Tag + m_Current_Running_Thread.ManagedThreadId.ToString(), "设置 m_Last_Running_Thread 值");
                //m_Last_Running_Thread = m_Current_Running_Thread;
                //Log.Error(_Tag + m_Current_Running_Thread.ManagedThreadId.ToString(), "进入过滤阶段");

                if (m_dataTable_Items != null && m_dataTable_Items.Rows.Count > 0)
                {
                    var items = m_dataTable_Items.AsEnumerable();
                    var ret = items.Where(t => t.Field<string>(fnumber).Contains(words) || t.Field<string>(fname).Contains(words) || t.Field<string>(fextend).Contains(words)).Select(i => new Tools_Tables_Adapter_Class.Item { m_fnumber = i.Field<string>(fnumber), m_fname = i.Field<string>(fname), m_fextend = i.Field<string>(fextend),m_fitemid = i.Field<int>(fitemid).ToString() }).ToList<Tools_Tables_Adapter_Class.Item>();
                    //DataTable _ret = items.OrderByDescending(t => t.Field<string>(fnumber).Contains(words));
                    //List<Tools_Tables_Adapter_Class.Item> _ret_List = items.Select(i => new Tools_Tables_Adapter_Class.Item { m_fnumber = i.Field<string>(fnumber), m_fname = i.Field<string>(fname), m_fextend = i.Field<string>(fextend) }).ToList<Tools_Tables_Adapter_Class.Item>();
                    var adapter = new Tools_Tables_Adapter_Class.ItemAdapter(this, ret, m_groupKey);
                    //Log.Error(_Tag + m_Current_Running_Thread.ManagedThreadId.ToString(), "开始更新Adapter");
                    RunOnUiThread(() =>
                    {
                        m_listview.Adapter = adapter;
                        m_search.Enabled = true;
                    });
                }
                //Log.Error(_Tag + m_Current_Running_Thread.ManagedThreadId.ToString(), "结束");

            }));
            T.IsBackground = true;

            T.Start();
        }

        //废弃，每次搜索数据库延迟严重，改为搜索datatable，优化为 search_()
        private void updateItemsInAnotherThread__(string getItemLikesql, string fnumber, string fname, string fextend)
        {
            //if (keycode == Keycode.Enter)
            //{
            var th = new Thread(new ThreadStart(() =>
            {
                //var sqlTxt = "select B.FNumber,B.FName,A.FModel from t_ICItem A full join t_Item B on A.FItemID=B.FItemID where B.FItemClassID=4 " +
                //             "and (B.FNumber like '%" + words + "%' or B.FName like '%" + words + "%' or A.FModel like '%" + words + "%' )order by B.FNumber";
                var ret_table = Tools_SQL_Class.getTable(getItemLikesql);
                m_ItemList.Clear();
                if (ret_table != null && ret_table.Rows.Count >= 1)
                {
                    for (int i = 0; i < ret_table.Rows.Count; i++)
                    {
                        var it = new Tools_Tables_Adapter_Class.Item();
                        it.m_fnumber = ret_table.Rows[i][fnumber].ToString();
                        it.m_fname = ret_table.Rows[i][fname].ToString();
                        it.m_fextend = ret_table.Rows[i][fextend].ToString();
                        m_ItemList.Add(it);
                    }
                    //搜索时，不再显示索引
                    var adapter = new Tools_Tables_Adapter_Class.ItemAdapter(this, m_ItemList, m_groupKey);
                    RunOnUiThread(() =>
                    {
                        m_listview.Adapter = adapter;
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        m_listview.Adapter = null;
                    });
                }
                
            }));
            th.IsBackground = true;
            th.Start();
            //}


        }


        int m_lastSelectItem = -1;
        private void M_listview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == m_lastSelectItem && !m_groupKey.Contains(((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fnumber))
            //if (m_listview.IsItemChecked(e.Position))
            {
                Intent intent = new Intent();
                //int index_ = Convert.ToInt32(((ListView)sender).SelectedItemId);
                intent.PutExtra("FNumber", ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fnumber);
                intent.PutExtra("FName", ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fname);
                intent.PutExtra("FExtend", ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fextend);
                var temp = ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fitemid;
                intent.PutExtra("FItemID", temp );
                SetResult(0, intent);
                this.Finish();
            }
            //m_listview.CheckedItemPosition
            m_lastSelectItem = e.Position;
        }

        private void Tv_Click(object sender, EventArgs e)
        {
            //m_listview.SetItemChecked(m_listview.CheckedItemPosition, false);

            m_listview.SetSelection(m_ItemList.FindIndex(a => a.m_fname + "  " + a.m_fnumber == ((TextView)sender).Text));
        }

        bool m_ScrollUpdate = false;
        private void Bt_Click(object sender, EventArgs e)
        {
            foreach (var item in m_TextView_list)
            {
                item.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Gray));
            }
            ((TextView)sender).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
        }

        private void Bt1_Click(object sender, EventArgs e)
        {
            Tools_Tables_Adapter_Class.ShowMsg(this, "asd", "asd");
        }

        //保存状态
        protected override void OnSaveInstanceState(Bundle outState)
        {

        }

        //取出状态
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {

        }
    }
}