using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Android_KingHoo_Scanner_Rebuild
{
    [Activity(Label = "@string/company_main_title", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.AdjustPan, MainLauncher = false)]
    class Activity_BillSelect_Class : AppCompatActivity
    {
        //public static MainActivity m_main = null;
        private string sqlTxt = "";
        private EditText m_search = null;
        private ListView m_billlist = null;
        private DataTable m_datatable = null;
        private Button m_selectBill = null;
        static Activity_BillSelect_Class m_instance = null;
        //private List<Tools_Tables_Adapter_Class.Source_Bill> m_adapterList = new List<Tools_Tables_Adapter_Class.Source_Bill>();
        private System.Timers.Timer timer = null;
        string m_ItemType = "";
        string m_currentFitemID = "";
        List<Tools_Tables_Adapter_Class.Source_Bill> LIstOfSourceBill = null;
        //public Activity_BillSelect_Class(){}
        public static Activity_BillSelect_Class instance()
        {
            if (m_instance != null)
            {
                return m_instance;
            }
           // m_instance = new Activity_BillSelect_Class();
            return m_instance;
        }
        Thread initDataThread = null;
        void initData()
        {
            if (initDataThread == null || initDataThread.ThreadState == ThreadState.Unstarted || initDataThread.ThreadState == ThreadState.Stopped)
            {
                initDataThread = new Thread(() =>
                {
                    switch (m_ItemType)
                    {
                        case Tools_Tables_Adapter_Class.ItemType.SourceBill_POORDER:
                            {
                                sqlTxt =
                                "select A.FQty,A.FCommitQty,A.FItemID,A.FInterID, " +
                                "A.FEntryID,B.FUnitID,B.FName FItemID_FName, " +
                                "B.FNumber FItemID_FNumber, B.FModel FItemID_FModel " +
                                " , C.FName FUnitID_FName, D.FBillNo,D.FSupplyID, " +
                                "E.FName FSupplyID_FName from POOrderEntry A join " +
                                "t_ICItem B on A.FItemID = B.FItemID " +
                                "join t_MeasureUnit C on B.FUnitID = C.FItemID " +
                                "join POOrder D on A.FInterID = D.FInterID " +
                                "join t_Supplier E on E.FItemID = D.FSupplyID " +
                                "where FMrpClosed != 1 and FQty-FCommitQty > 0 and A.FItemID = " + m_currentFitemID;
                                var t = new Thread(() =>
                                {
                                    var ret = Tools_SQL_Class.getTable(sqlTxt);
                                    m_datatable = ret != null && ret.Rows.Count > 0 ? ret : null;
                                    if (m_datatable == null)
                                    {
                                        this.RunOnUiThread(() =>
                                        {
                                            m_search.Text = "该物料没有采购订单！";
                                        });
                                        return;
                                    }
                                    var bills = m_datatable.AsEnumerable();
                                    LIstOfSourceBill = bills
                                    .Select(
                                        i => new Tools_Tables_Adapter_Class.Source_Bill
                                        {
                                            FBillNo = i.Field<string>("FBillNo"),
                                            FInterID = i.Field<int>("FInterID"),
                                            F_Dep_Cust_Sup = i.Field<int>("FSupplyID"),
                                            F_Dep_Cust_Sup_Name = i.Field<string>("FSupplyID_FName"),
                                            FItemID = i.Field<int>("FItemID"),
                                            FUnitID = i.Field<int>("FUnitID"),
                                            FUnitID_FName = i.Field<string>("FUnitID_FName"),
                                            FItemID_FNumber = i.Field<string>("FItemID_FNumber"),
                                            FItemID_FName = i.Field<string>("FItemID_FName"),
                                            FItemID_FModel = i.Field<string>("FItemID_FModel"),
                                            FEntryID = i.Field<int>("FEntryID"),
                                            FQty = i.Field<decimal>("FQty"),
                                            FCommitQty = i.Field<decimal>("FCommitQty")
                                        })
                                    .ToList<Tools_Tables_Adapter_Class.Source_Bill>();

                                    var adapter_ = new Tools_Tables_Adapter_Class.SourceBillListAdapter(this, LIstOfSourceBill);
                                    adapter_.__ClickCallBack += Adapter____ClickCallBack;
                                    this.RunOnUiThread(() =>
                                    {
                                        m_billlist.Adapter = adapter_;
                                        m_search.Enabled = true;
                                    });
                                })
                                { IsBackground = true };
                                t.Start();

                            }
                            break;
                        case Tools_Tables_Adapter_Class.ItemType.SourceBill_PPBOM:
                            {

                            }
                            break;
                        case Tools_Tables_Adapter_Class.ItemType.SourceBill_SEORDER:
                            {

                            }
                            break;
                        default:
                            break;
                    }
                })
                { IsBackground = true };
                initDataThread.Start();
            }
        }

        private void Adapter____ClickCallBack(string data)
        {
            //(string data) => {
            try
            {
                Intent intent = new Intent();
                var clickedItem = LIstOfSourceBill.Where(item => item.m_uuid.ToString() == data).ToList()[0];
                //var finterid = clickedItem.FInterID.ToString()
                intent.PutExtra("FInterID", clickedItem.FInterID.ToString());
                intent.PutExtra("FEntryID", clickedItem.FEntryID.ToString());
                intent.PutExtra("FBillNo", clickedItem.FBillNo.ToString());
                intent.PutExtra("FItemID_FNumber", clickedItem.FItemID_FNumber.ToString());

                SetResult(0, intent);
                this.Finish();
            }
            catch
            {

            }

            //};
        }

        Thread ProcessDataingThread = null;
        private void ProcessDataing()
        {
            m_search.Enabled = false;
            if(ProcessDataingThread == null || ProcessDataingThread.ThreadState == ThreadState.Unstarted || ProcessDataingThread.ThreadState == ThreadState.Stopped)
            {
                ProcessDataingThread = new Thread(() =>
                {
                    var keyword = m_search == null ? "" : m_search.Text;
                    if (/*keyword!="" &&*/ m_datatable != null)
                    {
                        var bills = m_datatable.AsEnumerable();
                        var ret = bills.Where(
                            t =>
                            t.Field<string>("FBillNo").Contains(keyword) ||
                            t.Field<string>("FItemID_FName").Contains(keyword) ||
                            t.Field<string>("FItemID_FNumber").Contains(keyword) ||
                            t.Field<string>("FSupplyID_FName").Contains(keyword) ||
                            t.Field<string>("FItemID_FModel").Contains(keyword)
                            )
                        .Select(
                            i => new Tools_Tables_Adapter_Class.Source_Bill
                            {
                                FBillNo = i.Field<string>("FBillNo"),
                                FInterID = i.Field<int>("FInterID"),
                                F_Dep_Cust_Sup = i.Field<int>("FSupplyID"),
                                F_Dep_Cust_Sup_Name = i.Field<string>("FSupplyID_FName"),
                                FItemID = i.Field<int>("FItemID"),
                                FUnitID = i.Field<int>("FUnitID"),
                                FUnitID_FName = i.Field<string>("FUnitID_FName"),
                                FItemID_FNumber = i.Field<string>("FItemID_FNumber"),
                                FItemID_FName = i.Field<string>("FItemID_FName"),
                                FItemID_FModel = i.Field<string>("FItemID_FModel"),
                                FEntryID = i.Field<int>("FEntryID"),
                                FQty = i.Field<decimal>("FQty"),
                                FCommitQty = i.Field<decimal>("FCommitQty")
                            })
                        .ToList<Tools_Tables_Adapter_Class.Source_Bill>();

                        var adapter_ = new Tools_Tables_Adapter_Class.SourceBillListAdapter(this, ret);
                        adapter_.__ClickCallBack += Adapter____ClickCallBack;
                        this.RunOnUiThread(() =>
                        {
                            m_billlist.Adapter = adapter_;
                            m_search.Enabled = true;
                        });

                    }
                })
                { IsBackground = true };
                ProcessDataingThread.Start();
                timer.Stop();
            }
        }

        public Activity_BillSelect_Class(/* MainActivity main,Tools_Tables_Adapter_Class.SourceBillType __Type*/)
        {
            
            m_instance = this;
            timer = new System.Timers.Timer(2000);
            //m_search.Enabled = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(
            (obj,args)=>{
                ProcessDataing();
            });
            //m_context = main;
 
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
 
        }
        protected override void OnResume()
        {
            base.OnResume();
            m_ItemType = this.Intent.GetStringExtra("Type");
            m_currentFitemID = this.Intent.GetStringExtra("FItemID");
            SetContentView(Resource.Layout.activity_sourcebillselect);
            m_search = FindViewById<EditText>(Resource.Id.sourceSelect_searchText);
            m_search.Enabled = false;
            m_billlist = FindViewById<ListView>(Resource.Id.sourceSelect_billlist);


            var t = new Thread(() =>
            {
                if (sqlTxt != "")
                {
                    var ret = Tools_SQL_Class.getTable(sqlTxt);
                    if (ret != null && ret.Rows.Count > 0)
                    {
                        for (int i = 0; i < ret.Rows.Count; i++)
                        {
                            var item = new Tools_Tables_Adapter_Class.Source_Bill();
                            item.FBillNo = ret.Rows[i]["FBillNo"].ToString();
                        }
                    }
                }
            });
            t.IsBackground = true;
            t.Start();
            m_search.AfterTextChanged += new EventHandler<Android.Text.AfterTextChangedEventArgs>((obj, args) =>
            {
                timer.Stop();
                timer.Start();
                // var Txt = ((EditText)obj).Text;
            });
            initData();
            //ProcessDataing();
        }


    }
}


//m_selectBill = FindViewById<Button>(Resource.Id.sourceselect_select);
//if (m_selectBill != null)
//{
//    m_selectBill.Click += new EventHandler((sender, args) =>
//    {
//        //m_billlist.Adapter. [m_billlist.SelectedItemId]
//        var msg = ((Button)sender).Tag.ToString().Split('|');
//        if (msg.Length != 2) return;
//        var finterid = Convert.ToInt32(msg[0]);
//        var fentryid = Convert.ToInt32(msg[1]);
//        Intent intent = new Intent();
//        intent.PutExtra("Type", Tools_Tables_Adapter_Class.SourceBillType.PPOREDER);
//        intent.PutExtra("FInterID", finterid);
//        intent.PutExtra("FEntryID", fentryid);
//        var showmsg = m_datatable.AsEnumerable()
//        .Where(item => item.Field<int>("FInterID") == finterid && item.Field<int>("FEntryID") == fentryid)
//        .Select(i => new Tools_Tables_Adapter_Class.Source_Bill
//        {
//            FBillNo = i.Field<string>("FBillNo"),
//            FInterID = i.Field<int>("FInterID"),
//            F_Dep_Cust_Sup = i.Field<int>("FSupplyID"),
//            F_Dep_Cust_Sup_Name = i.Field<string>("FSupplyID_FName"),
//            FItemID = i.Field<int>("FItemID"),
//            FUnitID = i.Field<int>("FUnitID"),
//            FUnitID_FName = i.Field<string>("FUnitID_FName"),
//            FItemID_FNumber = i.Field<string>("FItemID_FNumber"),
//            FItemID_FName = i.Field<string>("FItemID_FName"),
//            FItemID_FModel = i.Field<string>("FItemID_FModel"),
//            FEntryID = i.Field<int>("FEntryID"),
//            FQty = i.Field<decimal>("FQty"),
//            FCommitQty = i.Field<decimal>("FCommitQty")
//        }).ToList<Tools_Tables_Adapter_Class.Source_Bill>();
//        string returnMsg = "";
//        if (showmsg.Count >= 1)
//        {
//            returnMsg = showmsg[0].FBillNo + showmsg[0].FItemID_FName;
//        }
//        intent.PutExtra("FMsg", returnMsg);
//        SetResult(0, intent);
//        this.Finish();
//    });
//}