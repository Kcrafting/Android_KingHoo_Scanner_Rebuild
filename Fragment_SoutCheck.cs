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
using Android.Views;
using Android.Widget;
using Java.Sql;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Fragment_SoutCheck : Android.Support.V4.App.Fragment
    {
        private static Fragment_SoutCheck m_instance = null;
        ListView m_list = null;
        EditText m_search = null;
        System.Timers.Timer m_timer = new System.Timers.Timer(2000);
        TextView m_textviewforclick = null; //activity_main_soutcheck_search_label
        public static Fragment_SoutCheck Instance()
        {
            Console.WriteLine("开始初始化");
            m_instance = m_instance == null? new Fragment_SoutCheck(): m_instance;
            Console.WriteLine("初始化结束");
            return m_instance;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        List<Tools_Tables_Adapter_Class.SOUT_Bill> m_adapterList = null;

        Thread m_queryThread = null;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.activity_main_soutcheck, container, false);
            m_search = v.FindViewById<EditText>(Resource.Id.activity_main_soutcheck_search);
            m_list = v.FindViewById<ListView>(Resource.Id.activity_main_soutcheck_billlist);
            m_textviewforclick = v.FindViewById<TextView>(Resource.Id.activity_main_soutcheck_search_label);
            //if(m_queryThread == null)
            //{
            m_textviewforclick.Click += M_textviewforclick_Click;
            m_search.AfterTextChanged += M_search_AfterTextChanged;
            m_queryThread = new Thread(()=> {
                    var ret = Tools_SQL_Class.getTable("select TOP 200 A.FBillNo,A.FInterID,cast(A.FDate as date ) FDate,C.FName  ,ISNULL( A.FNote,'') FNote from ICStockBill A join t_Department C on A.FDeptID = C.FItemID where A.FTranType = 24 and A.FInterID not in (select FInterID from ZZ_KingHoo_StockCheck) ORDER BY FINTERID DESC");
                    try
                    {
                        if (ret != null && ret.Rows.Count > 0)
                        {
                            m_adapterList = ret.AsEnumerable().Select(
                                i => new Tools_Tables_Adapter_Class.SOUT_Bill(
                                    i.Field<string>("FBillNo"),
                                    i.Field<int>("FInterID"),
                                    i.Field<string>("FNote"),
                                    i.Field<string>("FName"),
                                    i.Field<DateTime>("FDate")
                                    )
                                ).ToList<Tools_Tables_Adapter_Class.SOUT_Bill>();
                            var ada = new Tools_Tables_Adapter_Class.SOUTBillAdapter(this.Activity, m_adapterList);
                            ada.__ClickCallBack += Ada___ClickCallBack;
                            this.Activity.RunOnUiThread(()=>{
                                m_list.Adapter = ada;
                            });
                            
                        }
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                });
                m_queryThread.IsBackground = true;
                m_queryThread.Start();
            //}
            //m_queryThread.IsBackground = true;
            //if (m_queryThread.ThreadState == ThreadState.Stopped)
            //{
            //    m_queryThread.Start();
            //}
            
            return v;
        }

        private void M_search_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            m_timer.Stop();
            m_timer.Start();
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler((_sender,_event) => {
                m_timer.Stop();
                var t = new Thread(()=> {
                    m_adapterList = m_adapterList.Where(item => item.m_FBillNo.Contains(((EditText)sender).Text)).ToList();
                    if (m_adapterList.Count < 1)
                    {
                        var ret = Tools_SQL_Class.getTable("select TOP 200 A.FBillNo,A.FInterID,cast(A.FDate as date ) FDate,C.FName  ,ISNULL( A.FNote,'') FNote from ICStockBill A join t_Department C on A.FDeptID = C.FItemID where A.FTranType = 24 and A.FInterID not in (select FInterID from ZZ_KingHoo_StockCheck) and A.FBillNo like '%" + m_search.Text + "%'  ORDER BY FINTERID DESC");
                        if (ret != null && ret.Rows.Count > 0)
                        {
                            m_adapterList = ret.AsEnumerable().Select(
                                i => new Tools_Tables_Adapter_Class.SOUT_Bill(
                                    i.Field<string>("FBillNo"),
                                    i.Field<int>("FInterID"),
                                    i.Field<string>("FNote"),
                                    i.Field<string>("FName"),
                                    i.Field<DateTime>("FDate")
                                    )
                                ).ToList<Tools_Tables_Adapter_Class.SOUT_Bill>();
                            var ada = new Tools_Tables_Adapter_Class.SOUTBillAdapter(this.Activity, m_adapterList);
                            ada.__ClickCallBack += Ada___ClickCallBack;
                            this.Activity.RunOnUiThread(() => {
                                m_list.Adapter = ada;
                            });
                        }
                        else
                        {
                            m_adapterList = new List<Tools_Tables_Adapter_Class.SOUT_Bill>();
                            var ada = new Tools_Tables_Adapter_Class.SOUTBillAdapter(this.Activity, m_adapterList);
                            ada.__ClickCallBack += Ada___ClickCallBack;
                            this.Activity.RunOnUiThread(() => {
                                m_list.Adapter = ada;
                            });
                        }
                    }
                    else
                    {
                        var ada = new Tools_Tables_Adapter_Class.SOUTBillAdapter(this.Activity, m_adapterList);
                        ada.__ClickCallBack += Ada___ClickCallBack;
                        this.Activity.RunOnUiThread(() => {
                            m_list.Adapter = ada;
                        });
                    }
                    this.Activity.RunOnUiThread(() => {
                        m_search.Enabled = true;
                    });
                });
                t.Start();
                m_search.Enabled = false;
            });
        }

        private void M_textviewforclick_Click(object sender, EventArgs e)
        {
            ThreadQuery();
        }
        void ThreadQuery()
        {
            m_queryThread = new Thread(() => {
                var ret = Tools_SQL_Class.getTable("select TOP 200 A.FBillNo,A.FInterID,cast(A.FDate as date ) FDate,C.FName  ,ISNULL( A.FNote,'') FNote from ICStockBill A join t_Department C on A.FDeptID = C.FItemID where A.FTranType = 24 and A.FInterID not in (select FInterID from ZZ_KingHoo_StockCheck) ORDER BY FINTERID DESC");
                try
                {
                    if (ret != null && ret.Rows.Count > 0)
                    {
                        m_adapterList = ret.AsEnumerable().Select(
                            i => new Tools_Tables_Adapter_Class.SOUT_Bill(
                                i.Field<string>("FBillNo"),
                                i.Field<int>("FInterID"),
                                i.Field<string>("FNote"),
                                i.Field<string>("FName"),
                                i.Field<DateTime>("FDate")
                                )
                            ).ToList<Tools_Tables_Adapter_Class.SOUT_Bill>();
                        var ada = new Tools_Tables_Adapter_Class.SOUTBillAdapter(this.Activity, m_adapterList);
                        ada.__ClickCallBack += Ada___ClickCallBack;
                        this.Activity.RunOnUiThread(() => {
                            m_list.Adapter = ada;
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            m_queryThread.IsBackground = true;
            m_queryThread.Start();
        }

        private void Ada___ClickCallBack(string finterid)
        {
            var dialog = new Tools_Tables_Adapter_Class.BillConfirm(MainActivity.ms_thisPointer, finterid);
            dialog.__OnScanFinish += Dialog___OnScanFinish;
            dialog.Show();
        }

        private void Dialog___OnScanFinish(string Txt)
        {
            //刷新列表
            ThreadQuery();
        }
    }
}