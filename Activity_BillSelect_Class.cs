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
        class Bill__
        {
            public string FBillNo { get; set; }
            public int FInterID { get; set; }
            public Bill__(string billno,int interid)
            {
                FBillNo = billno;
                FInterID = interid;
            }
            public int F_Dep_Cust_Sup { get; set; }
            public string F_Dep_Cust_Sup_Name { get; set; }
        }
        //public static MainActivity m_main = null;
        private string sqlTxt = "";
        private EditText m_search = null;
        private ListView m_billlist = null;
        private DataTable m_datatable = null;
        private DataTable m_head_datatable = null;
        //private Button m_selectBill = null;
        static Activity_BillSelect_Class m_instance = null;
        //private List<Tools_Tables_Adapter_Class.Source_Bill> m_adapterList = new List<Tools_Tables_Adapter_Class.Source_Bill>();
        private System.Timers.Timer timer = null;
        string m_ItemType = "";
        //string m_currentFitemID = "";
        /// <summary>
        /// 供应商字段
        /// </summary>
        string m_FNumber = "";
        List<Tools_Tables_Adapter_Class.Source_Bill> LIstOfSourceBill = null;
        //
        EnumerableRowCollection<DataRow> m_head_ = null;
        EnumerableRowCollection<DataRow> m_body_ = null;
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
                                "select replace(str(A.FQty, len(A.FQty), B.FQtyDecimal), ' ', '') FQty," +
                                "case when A.FCommitQty!=0 then replace(str(A.FCommitQty, len(A.FCommitQty), B.FQtyDecimal), ' ', '') else '' end FCommitQty,A.FItemID,A.FInterID, " +
                                "A.FEntryID,B.FUnitID,B.FName FItemID_FName, " +
                                "B.FNumber FItemID_FNumber, B.FModel FItemID_FModel " +
                                " , C.FName FUnitID_FName, D.FBillNo,D.FSupplyID, " +
                                "E.FName FSupplyID_FName from POOrderEntry A join " +
                                "t_ICItem B on A.FItemID = B.FItemID " +
                                "join t_MeasureUnit C on B.FUnitID = C.FItemID " +
                                "join POOrder D on A.FInterID = D.FInterID " +
                                "join t_Supplier E on E.FItemID = D.FSupplyID " +
                                "where FMrpClosed != 1 and FQty-FCommitQty > 0 " +
                                (m_FNumber=="" || m_FNumber==null ? "":"and E.FNumber = " + m_FNumber);

                                var sqlTxt__Head =
                                "SELECT A.FInterID " +
                                ",D.FBillNo " +
                                ",D.FSupplyID " +
                                ",E.FNumber FSupplyID_FNumber" +
                                ",E.FName FSupplyID_FName " +
                                "FROM POOrder A " +
                                "JOIN POOrder D ON A.FInterID = D.FInterID " +
                                "JOIN t_Supplier E ON E.FItemID = D.FSupplyID " +
                                "WHERE D.FClosed != 1";
                                var t = new Thread(() =>
                                {
                                    var ret = Tools_SQL_Class.getTable(sqlTxt);
                                    var ret_Detail = Tools_SQL_Class.getTable(sqlTxt__Head);
                                    m_datatable = ret != null && ret.Rows.Count > 0 ? ret : null;
                                    m_head_datatable = ret_Detail != null && ret_Detail.Rows.Count > 0 ? ret_Detail : null;

                                    if (m_datatable == null)
                                    {
                                        this.RunOnUiThread(() =>
                                        {
                                            m_search.Text = "系统没有未关闭的采购订单！";
                                        });
                                        return;
                                    }

                                   // m_body_ = m_datatable.AsEnumerable();
                                    m_body_ = DataTableExtensions.AsEnumerable(ret);
                                    //EnumerableRowCollection<DataRow> d
                                    int a1 = m_body_.Count();
                                    //m_head_ = m_head_datatable.AsEnumerable();

                                    m_head_ = DataTableExtensions.AsEnumerable(ret_Detail);
                                    int a2 = m_head_.Count();
                                    //var head = bills.Select(i => new Bill__(i.Field<string>("FBillNo"), i.Field<int>("FInterID"))).ToList<Bill__>();
                                    //var afterfilter = //head.Distinct<Bill__>().ToList();
                                    //var entrys = bills;
                                    Console.WriteLine("last rows " + a1.ToString() + "  :  " + a2.ToString());
                                    LIstOfSourceBill = m_head_
                                    .Select(
                                        i => new Tools_Tables_Adapter_Class.Source_Bill
                                        {
                                            FBillNo = i.Field<string>("FBillNo"),
                                            FInterID = i.Field<int>("FInterID"),
                                            F_Dep_Cust_Sup = i.Field<int>("FSupplyID"),
                                            F_Dep_Cust_Sup_Name = i.Field<string>("FSupplyID_FName"),
                                            F_Dep_Cust_Sup_Number = i.Field<string>("FSupplyID_FNumber"),
                                            m_Entry = m_body_.Where(
                                                i1 => i1.Field<int>("FInterID") == i.Field<int>("FInterID")).Select(
                                                i2 => new Tools_Tables_Adapter_Class.Source_Bill_Body
                                                {

                                                    FItemID = i2.Field<int>("FItemID"),
                                                    FUnitID = i2.Field<int>("FUnitID"),
                                                    FUnitID_FName = i2.Field<string>("FUnitID_FName"),
                                                    FItemID_FNumber = i2.Field<string>("FItemID_FNumber"),
                                                    FItemID_FName = i2.Field<string>("FItemID_FName"),
                                                    FItemID_FModel = i2.Field<string>("FItemID_FModel"),
                                                    FEntryID = i2.Field<int>("FEntryID"),
                                                    FQty = i2.Field<string>("FQty"),
                                                    FCommitQty = i2.Field<string>("FCommitQty")
                                                }).ToList<Tools_Tables_Adapter_Class.Source_Bill_Body>()
                                        })
                                    .ToList<Tools_Tables_Adapter_Class.Source_Bill>();
                                    //Source_Bill 将作为单据的分录
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
                                sqlTxt =    "SELECT replace(str(A.FQty, len(A.FQty), B.FQtyDecimal), ' ', '') FQty " +
                                            ",CASE  " +
                                            "WHEN A.FCommitQty != 0 " +
                                            "THEN replace(str(A.FCommitQty, len(A.FCommitQty), B.FQtyDecimal), ' ', '') " +
                                            "ELSE '' " +
                                            "END FCommitQty " +
                                            ",A.FItemID " +
                                            ",A.FInterID " +
                                            ",A.FEntryID " +
                                            ",B.FUnitID " +
                                            ",B.FName FItemID_FName " +
                                            ",B.FNumber FItemID_FNumber " +
                                            ",B.FModel FItemID_FModel " +
                                            ",C.FName FUnitID_FName " +
                                            ",D.FBillNo " +
                                            ",D.FCustID FSupplyID" +
                                            ",E.FName FSupplyID_FName " +
                                            "FROM SEOrderEntry A " +
                                            "JOIN t_ICItem B ON A.FItemID = B.FItemID " +
                                            "JOIN t_MeasureUnit C ON B.FUnitID = C.FItemID " +
                                            "JOIN SEOrder D ON A.FInterID = D.FInterID " +
                                            "JOIN t_Organization E ON E.FItemID = D.FCustID " +
                                            "WHERE FMrpClosed != 1 " +
                                            "AND FQty - FCommitQty > 0 " +
                                            (m_FNumber == "" || m_FNumber == null ? "" : "and E.FNumber = " + m_FNumber);

                                var sqlTxt__Head =
                                            "SELECT A.FInterID " +
                                            ",A.FBillNo " +
                                            ",A.FCustID FSupplyID" +
                                            ",E.FNumber FSupplyID_FNumber " +
                                            ",E.FName FSupplyID_FName " +
                                            "FROM SEOrder A " +
                                            "JOIN t_Organization E ON E.FItemID = A.FCustID " +
                                            "WHERE A.FClosed != 1 ";
                                var t = new Thread(() =>
                                {
                                    var ret = Tools_SQL_Class.getTable(sqlTxt);
                                    var ret_Detail = Tools_SQL_Class.getTable(sqlTxt__Head);
                                    m_datatable = ret != null && ret.Rows.Count > 0 ? ret : null;
                                    m_head_datatable = ret_Detail != null && ret_Detail.Rows.Count > 0 ? ret_Detail : null;

                                    if (m_datatable == null)
                                    {
                                        this.RunOnUiThread(() =>
                                        {
                                            m_search.Text = "系统没有未关闭的采购订单！";
                                        });
                                        return;
                                    }

                                    // m_body_ = m_datatable.AsEnumerable();
                                    m_body_ = DataTableExtensions.AsEnumerable(ret);
                                    //EnumerableRowCollection<DataRow> d
                                    int a1 = m_body_.Count();
                                    //m_head_ = m_head_datatable.AsEnumerable();

                                    m_head_ = DataTableExtensions.AsEnumerable(ret_Detail);
                                    int a2 = m_head_.Count();
                                    //var head = bills.Select(i => new Bill__(i.Field<string>("FBillNo"), i.Field<int>("FInterID"))).ToList<Bill__>();
                                    //var afterfilter = //head.Distinct<Bill__>().ToList();
                                    //var entrys = bills;
                                    Console.WriteLine("last rows " + a1.ToString() + "  :  " + a2.ToString());
                                    LIstOfSourceBill = m_head_
                                    .Select(
                                        i => new Tools_Tables_Adapter_Class.Source_Bill
                                        {
                                            FBillNo = i.Field<string>("FBillNo"),
                                            FInterID = i.Field<int>("FInterID"),
                                            F_Dep_Cust_Sup = i.Field<int>("FSupplyID"),
                                            F_Dep_Cust_Sup_Name = i.Field<string>("FSupplyID_FName"),
                                            F_Dep_Cust_Sup_Number = i.Field<string>("FSupplyID_FNumber"),
                                            m_Entry = m_body_.Where(
                                                i1 => i1.Field<int>("FInterID") == i.Field<int>("FInterID")).Select(
                                                i2 => new Tools_Tables_Adapter_Class.Source_Bill_Body
                                                {

                                                    FItemID = i2.Field<int>("FItemID"),
                                                    FUnitID = i2.Field<int>("FUnitID"),
                                                    FUnitID_FName = i2.Field<string>("FUnitID_FName"),
                                                    FItemID_FNumber = i2.Field<string>("FItemID_FNumber"),
                                                    FItemID_FName = i2.Field<string>("FItemID_FName"),
                                                    FItemID_FModel = i2.Field<string>("FItemID_FModel"),
                                                    FEntryID = i2.Field<int>("FEntryID"),
                                                    FQty = i2.Field<string>("FQty"),
                                                    FCommitQty = i2.Field<string>("FCommitQty")
                                                }).ToList<Tools_Tables_Adapter_Class.Source_Bill_Body>()
                                        })
                                    .ToList<Tools_Tables_Adapter_Class.Source_Bill>();
                                    //Source_Bill 将作为单据的分录
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
            try
            {
                Intent intent = new Intent();
                var clickedItem = LIstOfSourceBill.Where(item => item.m_uuid.ToString() == data).ToList()[0];
                //var finterid = clickedItem.FInterID.ToString()
                intent.PutExtra("FInterID", clickedItem.FInterID.ToString());
                //intent.PutExtra("FEntryID", clickedItem.FEntryID.ToString());
                intent.PutExtra("FBillNo", clickedItem.FBillNo.ToString());
                //intent.PutExtra("FItemID_FNumber", clickedItem.FItemID_FNumber.ToString());
                intent.PutExtra("FName", clickedItem.F_Dep_Cust_Sup_Name);
                intent.PutExtra("FNumber", clickedItem.F_Dep_Cust_Sup_Number);
                intent.PutExtra("FItemID", clickedItem.F_Dep_Cust_Sup);
                SetResult(0, intent);
                this.Finish();
            }
            catch
            {

            }
        }

        Thread ProcessDataingThread = null;
        private void ProcessDataing()
        {
            m_search.Enabled = false;
            if (ProcessDataingThread == null || ProcessDataingThread.ThreadState == ThreadState.Unstarted || ProcessDataingThread.ThreadState == ThreadState.Stopped)
            {
                ProcessDataingThread = new Thread(() =>
                {
                    var keyword = m_search == null ? "" : m_search.Text;
                    if (/*keyword!="" &&*/ m_datatable != null)
                    {
                        var bills = m_datatable.AsEnumerable();
                        var head = bills.Select(i => new Bill__(i.Field<string>("FBillNo")
                                       , i.Field<int>("FInterID"))).ToList<Bill__>();
                        var afterfilter = head.Distinct<Bill__>().ToList();

                        var ret = afterfilter.Where(
                            t =>
                            t.FBillNo.Contains(keyword)
                            )
                        .Select(
                                        i => new Tools_Tables_Adapter_Class.Source_Bill
                                        {
                                            FBillNo = i.FBillNo,
                                            FInterID = i.FInterID,
                                            F_Dep_Cust_Sup = i.F_Dep_Cust_Sup,
                                            F_Dep_Cust_Sup_Name = i.F_Dep_Cust_Sup_Name,
                                            m_Entry = bills.Where(
                                                i1 => i1.Field<int>("FInterID") == i.FInterID).Select(
                                                i2 => new Tools_Tables_Adapter_Class.Source_Bill_Body
                                                {

                                                    FItemID = i2.Field<int>("FItemID"),
                                                    FUnitID = i2.Field<int>("FUnitID"),
                                                    FUnitID_FName = i2.Field<string>("FUnitID_FName"),
                                                    FItemID_FNumber = i2.Field<string>("FItemID_FNumber"),
                                                    FItemID_FName = i2.Field<string>("FItemID_FName"),
                                                    FItemID_FModel = i2.Field<string>("FItemID_FModel"),
                                                    FEntryID = i2.Field<int>("FEntryID"),
                                                    FQty = i2.Field<string>("FQty"),
                                                    FCommitQty = i2.Field<string>("FCommitQty")
                                                }).ToList()
                                        })
                                    .ToList();

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
            (obj, args) =>
            {
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
            //修改 不需要按照物料过滤了 改为选单
            m_ItemType = this.Intent.GetStringExtra("Type");
            //m_currentFitemID = this.Intent.GetStringExtra("FItemID");
            m_FNumber = this.Intent.GetStringExtra("FNumber");

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
