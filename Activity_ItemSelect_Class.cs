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
using System.Data;
using Android.Util;


namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/company_main_title", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.AdjustPan, MainLauncher = false)]

    class Activity_ItemSelect_Class : AppCompatActivity
    {
        //索引
        LinearLayout m_indexContainer = null;
        //基础资料列表
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
        List<Tools_Tables_Adapter_Class.Item> m_ItemList = new List<Tools_Tables_Adapter_Class.Item>();
        List<TextView> m_indexItem = new List<TextView>();
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
            m_search.AfterTextChanged += M_search_AfterTextChanged;
            m_search_textview.Click += M_search_textview_Click; 
            m_timer.Elapsed += M_timer_Elapsed;
            m_search.Enabled = false;

            switch (m_ItemType)
            {
                case Tools_Tables_Adapter_Class.ItemType.ICItem:
                    {
                        this.Title = "物料选择";
                        ProcessItemsInAnotherThread("select B.FNumber,B.FDetail,B.FName,isnull(A.FModel,'') FModel ,isnull(A.FitemID,0) FitemID from t_ICItem A full join t_Item B on A.FItemID=B.FItemID where B.FItemClassID=4 order by B.FNumber", "FNumber", "FName", "FModel");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.ICStock:
                    {
                        this.Title = "仓库选择";
                        ProcessItemsInAnotherThread("select B.FNumber,B.FDetail,B.FName,B.FFullName,B.FitemID from t_Item B where B.FItemClassID=5", "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.ICStockPlace:
                    {
                        this.Title = "仓位选择";
                        ProcessItemsInAnotherThread("select fnumber,FDetail,FName,FFullName,FSPID FItemID from t_StockPlace where FSPID!=0", "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.User:
                    {
                        this.Title = "用户选择";
                        ProcessItemsInAnotherThread( "select FNumber,FName,FDetail,FFullName,FitemID from t_Item where FItemClassID=3", "FNumber", "FName", "FFullName");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.Supply:
                    {
                        this.Title = "选择供应商";
                        ProcessItemsInAnotherThread( "select FNumber,FDetail,FName,ffullname,FItemID from t_Item where FItemClassID=8", "FNumber", "FName", "ffullname");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.Customer:
                    {
                        this.Title = "选择客户";
                        ProcessItemsInAnotherThread( "select FNumber,FDetail,FName,ffullname,FItemID from t_Item where FItemClassID=1", "FNumber", "FName", "ffullname");
                    }
                    break;
                case Tools_Tables_Adapter_Class.ItemType.Dep:
                    {
                        this.Title = "选择部门";
                        ProcessItemsInAnotherThread("select FNumber,FDetail,FName,ffullname,FItemID from t_Item where FItemClassID=2", "FNumber", "FName", "ffullname");
                    }
                    break;
                default:
                    break;
            }
        }

        private void M_search_textview_Click(object sender, EventArgs e)
        {
           
        }

        //private DataTable m_dataTable_Group = null;
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
        private void ProcessItemsInAnotherThread(/*string getGroupSql,*/ string getItemsSql, string fnumber, string fname, string fextend,string fitemid = "FItemID")
        {
            var T = new Thread(new ThreadStart(() =>
            {
                if (Tools_SQL_Class.Status())
                {
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
                            it.m_IfDetail = m_dataTable_Items.Rows[i]["FDetail"].ToString()=="True"?true:false;
                            m_ItemList.Add(it);
                        }
                    }
                    var adapter = new Tools_Tables_Adapter_Class.ItemAdapter(this, m_ItemList/*, m_groupKey*/);
                    RunOnUiThread(() =>
                    {
                        //设置索引项
                        m_listview.Adapter = adapter;
                        m_search.Enabled = true;
                    });
                    m_indexItem.Clear();
                    var m_dataTable_Group = m_ItemList.Where(item => item.m_IfDetail == false).ToList();
                    if (m_dataTable_Group.Count >= 1)
                    {
                        for (int i = 0; i < m_dataTable_Group.Count; i++)
                        {
                            var tv = new TextView(this);
                            tv.Text = m_dataTable_Group[i].m_fname + "  " + m_dataTable_Group[i].m_fnumber;
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
            var T = new Thread(new ThreadStart(() =>
            {
                if (m_dataTable_Items != null && m_dataTable_Items.Rows.Count > 0)
                {
                    var items = m_dataTable_Items.AsEnumerable();
                    var ret = items.Where(t => t.Field<string>(fnumber).Contains(words) || t.Field<string>(fname).Contains(words) || t.Field<string>(fextend).Contains(words)).Select(i => new Tools_Tables_Adapter_Class.Item { m_fnumber = i.Field<string>(fnumber), m_fname = i.Field<string>(fname), m_fextend = i.Field<string>(fextend), m_fitemid = i.Field<int>(fitemid).ToString(), m_IfDetail = i.Field<bool>("FDetail") }).ToList<Tools_Tables_Adapter_Class.Item>();
                    var adapter = new Tools_Tables_Adapter_Class.ItemAdapter(this, ret);
                    RunOnUiThread(() =>
                    {
                        m_listview.Adapter = adapter;
                        m_search.Enabled = true;
                    });
                }
            }));
            T.IsBackground = true;

            T.Start();
        }

        //废弃，每次搜索数据库延迟严重，改为搜索datatable，优化为 search_()
        private void updateItemsInAnotherThread__(string getItemLikesql, string fnumber, string fname, string fextend)
        {
            var th = new Thread(new ThreadStart(() =>
            {
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
                        it.m_IfDetail = ret_table.Rows[i]["FDetail"].ToString()=="True"?true:false;
                        m_ItemList.Add(it);
                    }
                    //搜索时，不再显示索引
                    var adapter = new Tools_Tables_Adapter_Class.ItemAdapter(this, m_ItemList/*, m_groupKey*/);
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
        }


        int m_lastSelectItem = -1;
        private void M_listview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == m_lastSelectItem && ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_IfDetail)
            {
                Intent intent = new Intent();
                intent.PutExtra("FNumber", ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fnumber);
                intent.PutExtra("FName", ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fname);
                intent.PutExtra("FExtend", ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fextend);
                var temp = ((Tools_Tables_Adapter_Class.Item)m_listview.Adapter.GetItem(e.Position)).m_fitemid;
                intent.PutExtra("FItemID", temp );
                SetResult(0, intent);
                this.Finish();
            }
            m_lastSelectItem = e.Position;
        }

        private void Tv_Click(object sender, EventArgs e)
        {
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