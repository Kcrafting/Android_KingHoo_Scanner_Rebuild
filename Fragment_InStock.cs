﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Util;
using Java.Util;
using Android.App;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Android.Text;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Fragment_InStock : Android.Support.V4.App.Fragment
    {
        private static Fragment_InStock m_instance = null;
        TextView m_date_picker = null,m_supply_select = null;
        EditText m_date_picker_edittext = null, m_billno = null, m_supply_select_edit = null,m_fnote = null,m_operator=null,m_orgbill = null ;
        public static string m_currentType = "";
        public ListView m_itemlist = null;
        public Scroller m_scroller = null;
        //private int _m_saveFInterID = 0;
        //adapter 
        public List<Tools_Tables_Adapter_Class.Stock_Entry> m_EntryList_list = new List<Tools_Tables_Adapter_Class.Stock_Entry>();
        public Tools_Tables_Adapter_Class.Stock_Header m_Stock_Header = new Tools_Tables_Adapter_Class.Stock_Header();

        public delegate void onActivityRecive_instock_(string Type,string fnumber,string fname,string fextend,string fitemid);
        public event onActivityRecive_instock_ inStock_FunRecivieData;
        static public string m_EntryUID = "";//编辑分录的标志

        public static string m_uuid_click = "";
        public static Fragment_InStock Instance()
        {
            if (m_instance == null)
            {
                m_instance = new Fragment_InStock();
            }
            return m_instance;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        /*
         * PDA入库的物料需要注意一下几点
         * 1.不要启用辅助属性管理（未实现）
         * 2.不要启用双计量单位（未实现）
         * 3.不要启用用sn管理 (未实现)
         * 4.不要启用保质期管理 (未实现)
         * 5.不要启用辅助计量（浮动换算）管理(未实现)
         */
         public Fragment_InStock()
        {
            m_EntryList_list.Clear();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.activity_main_instock_layout, container, false);

            m_date_picker = v.FindViewById<TextView>(Resource.Id.activity_main_instock_layout_billdate);
            m_date_picker_edittext = v.FindViewById<EditText>(Resource.Id.activity_main_instock_layout_billdate_edittext);
            //m_date_picker.Click += M_date_picker_Click;

            m_supply_select_edit = v.FindViewById<EditText>(Resource.Id.activity_main_instock_layout_supply_edittext);
            m_supply_select_edit.Click += M_supply_select_Click;

            var m_date_picker_edit = v.FindViewById<EditText>(Resource.Id.activity_main_instock_layout_billdate_edittext);
            m_date_picker_edit.Click += M_date_picker_Click;

            m_supply_select = v.FindViewById<TextView>(Resource.Id.activity_main_instock_layout_supply);
            //m_supply_select.Click += M_supply_select_Click;

            m_billno = v.FindViewById<EditText>(Resource.Id.activity_main_instock_layout_fbillno);
            m_operator = v.FindViewById<EditText>(Resource.Id.activity_main_instock_layout_operator);
            m_operator.Click += M_operator_Click;

            m_itemlist = v.FindViewById<ListView>(Resource.Id.activity_main_instock_layout_entry);
            m_fnote = v.FindViewById<EditText>(Resource.Id.activity_main_instock_layout_note);
            m_fnote.AfterTextChanged += M_fnote_AfterTextChanged;
            var T = new Thread(new ThreadStart(()=> {
                //var ret = Tools_SQL_Class.getTable(
                //    " EXEC ZZ_KingHoo_GetBillNo");
                var para = new SqlParameter[] {
                new SqlParameter("@IsSave","1"),
                new SqlParameter("@FBillType","1"),
                new SqlParameter("@BillID",SqlDbType.NVarChar,500)
                };
                para[2].Direction = ParameterDirection.Output;
                var ret = Tools_SQL_Class.ExecutProcedure("dbo.GetICBillNo ", CommandType.StoredProcedure, para);
                var ret_value = para[2].Value == null?"":para[2].Value.ToString();
                if (ret_value != "")
                {
                    m_Stock_Header.m_Fbillno = ret_value;
                    Activity.RunOnUiThread(() => {
                        m_billno.Text = ret_value;
                    });
                }
                //if(ret!=null && ret.Rows.Count > 0)
                //{
                //    string billno = ret.Rows[0]["Ret"].ToString();
                //    Activity.RunOnUiThread(()=> {
                //        m_billno.Text = billno;
                //    });
                //}
                var _para = new SqlParameter[] {
                new SqlParameter("@TableName","ICStockBill"),
                new SqlParameter("@FInterID",SqlDbType.NVarChar,50),
                new SqlParameter("@Increment","1"),
                new SqlParameter("@UserID","16394")
                };
                _para[1].Direction = ParameterDirection.Output;
                var _ret = Tools_SQL_Class.ExecutProcedure("dbo.GetICMaxNum ", CommandType.StoredProcedure, _para);

                m_Stock_Header.m_FInterID = _para[1].Value == null?0: Convert.ToInt32(_para[1].Value.ToString());

                /*选单操作
                 1.先选择供应商 也可以不选择
                 2.如果选择供应商，则按照供应商过滤
                 3.选单，
                 4.选单后，将所有的单据的物料列出，带出数量
                 5.如果数量与订单不一致，修改数量
                 */

            }));
            T.IsBackground = true;
            T.Start();

            m_orgbill = v.FindViewById<EditText>(Resource.Id.activity_main_instock_layout_orgbill_edittext);
            m_orgbill.Click += M_orgbill_Click;

            return v;
            
        }

        private void M_orgbill_Click(object sender, EventArgs e)
        {
            //双击选择原单
            var intent = new Intent(Application.Context, typeof(Activity_BillSelect_Class));
            Fragment_InStock.m_currentType = Tools_Tables_Adapter_Class.ItemType.SourceBill_POORDER;
            intent.PutExtra("Type", Tools_Tables_Adapter_Class.ItemType.SourceBill_POORDER);
            if (m_supply_select_edit.Text != "")
            {
                //供应商 编码传入
                intent.PutExtra("FNumber", m_Stock_Header.m_FSupply);
            }
            this.StartActivityForResult(intent, 0);
        }

        private void M_operator_Click(object sender, EventArgs e)
        {
            m_currentType = Tools_Tables_Adapter_Class.ItemType.User;
            var intent = new Intent(Application.Context, typeof(Activity_ItemSelect_Class));
            intent.PutExtra("Type", m_currentType);
            StartActivityForResult(intent, 0);
        }

        private void M_fnote_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            m_Stock_Header.m_FNote = ((EditText)(sender)).Text;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (data == null) return;
            //
            var ret_FInterID = data.GetStringExtra("FInterID");
            var ret_FEntryID = data.GetStringExtra("FEntryID");
            var ret_FBillNo = data.GetStringExtra("FBillNo");
            var ret_FItemID_FNumber = data.GetStringExtra("FItemID_FNumber");
            
            var ret_FNumber = ret_FInterID != null && ret_FInterID != ""?ret_FInterID: data.GetStringExtra("FNumber");
            var ret_FName = ret_FEntryID != null && ret_FEntryID != ""? ret_FEntryID:data.GetStringExtra("FName");
            var ret_FExtend = ret_FBillNo != null && ret_FBillNo!=""? ret_FBillNo: data.GetStringExtra("FExtend");
            var ret_FItemID = ret_FItemID_FNumber != null && ret_FItemID_FNumber!=""? ret_FItemID_FNumber: data.GetStringExtra("FItemID");
            inStock_FunRecivieData?.Invoke(m_currentType, ret_FNumber, ret_FName, ret_FExtend, ret_FItemID);
            if (requestCode == 0)
            {
                if (resultCode == 0)
                {
                    if (data != null)
                    {
                        switch (m_currentType)
                        {
                            case Tools_Tables_Adapter_Class.ItemType.ICItem:
                                {
                                    //var ret = data.GetStringExtra("FNumber");

                                }
                                break;
                            case Tools_Tables_Adapter_Class.ItemType.ICStock:
                                {
                                    //var ret = data.GetStringExtra("FNumber");
                                    m_EntryList_list.Find(item => item.m_uuid.ToString() == m_uuid_click).m_fstock_fitemid = ret_FItemID;
                                    m_EntryList_list.Find(item => item.m_uuid.ToString() == m_uuid_click).m_fstock_name = ret_FName;
                                    m_EntryList_list.Find(item => item.m_uuid.ToString() == m_uuid_click).m_fstock = ret_FName;
                                }
                                break;
                            case Tools_Tables_Adapter_Class.ItemType.ICStockPlace:
                                {
                                    //var ret = data.GetStringExtra("FNumber");

                                }
                                
                                break;
                            case Tools_Tables_Adapter_Class.ItemType.User:
                                {
                                    m_operator.Text = ret_FName;
                                    m_Stock_Header.m_Foperator = ret_FNumber;
                                    m_Stock_Header.m_FoperatorName = ret_FName;
                                    //var ret = data.GetStringExtra("FNumber");
                                }
                                
                                break;
                                case Tools_Tables_Adapter_Class.ItemType.Supply:
                                {
                                    //var ret = data.GetStringExtra("FNumber");
                                    m_supply_select_edit.Text = ret_FName;
                                    m_Stock_Header.m_FSupply = ret_FNumber;
                                    m_Stock_Header.m_FSupplyName = ret_FName;
                                }
                                break;
                                case Tools_Tables_Adapter_Class.ItemType.Customer:
                                {
                                    //var ret = data.GetStringExtra("FNumber");
                                }
                                break;
                            case Tools_Tables_Adapter_Class.ItemType.SourceBill_POORDER:
                                {
                                    //ret_FBillNo
                                    //ret_FInterID

                                    m_orgbill.Text = ret_FBillNo;
                                    m_Stock_Header.m_FSourceBill = ret_FBillNo;
                                    m_supply_select_edit.Text = ret_FName;
                                    m_Stock_Header.m_FSupplyName = ret_FName;
                                    m_Stock_Header.m_FSupply = ret_FItemID;
                                    var t = new Thread(() =>
                                    {
                                        var ret = Tools_SQL_Class.getTable(
                                            "SELECT " +
                                            "  replace(str(A.FQty, len(A.FQty), B.FQtyDecimal), ' ', '') FTotleQty " +
                                            ", replace(str(A.FCommitQty, len(A.FCommitQty), B.FQtyDecimal), ' ', '') FCommitQty " +
                                            ", replace(str(A.FQty - A.FCommitQty, len(A.FQty - A.FCommitQty), B.FQtyDecimal), ' ', '') FQty" +
                                            ", A.FItemID " +
                                            ", A.FInterID " +
                                            ", A.FEntryID " +
                                            ", B.FUnitID " +
                                            ", B.FName FItemID_FName " +
                                            ", B.FNumber FItemID_FNumber " +
                                            ", B.FModel FItemID_FModel " +
                                            ", C.FName FUnitID_FName " +
                                            ", D.FBillNo " +
                                            ", D.FSupplyID " +
                                            ", B.FBatchManager" + 
                                            ", E.FName FSupplyID_FName " +
                                            "FROM POOrderEntry A " +
                                            "JOIN t_ICItem B ON A.FItemID = B.FItemID " +
                                            "JOIN t_MeasureUnit C ON B.FUnitID = C.FItemID " +
                                            "JOIN POOrder D ON A.FInterID = D.FInterID " +
                                            "JOIN t_Supplier E ON E.FItemID = D.FSupplyID " +
                                            "WHERE FMrpClosed != 1 " +
                                            "AND FQty - FCommitQty > 0 " +
                                            "AND A.FInterID = " + ret_FInterID.ToString());
                                        var em = ret.AsEnumerable();

                                        var entryList = em.Select(item => new Tools_Tables_Adapter_Class.Stock_Entry
                                        {
                                            m_fnumber_fitemid = item.Field<int>("FItemID").ToString(),
                                            m_fnumber = item.Field<string>("FItemID_FNumber"),
                                            m_fnumber_name = item.Field<string>("FItemID_FName"),
                                            m_fnumber_model = item.Field<string>("FItemID_FModel"),
                                            m_fcommitqty = item.Field<string>("FCommitQty"),
                                            m_ftotleqty = item.Field<string>("FTotleQty"),
                                            m_fqty = item.Field<string>("FQty"),
                                            m_funit_name = item.Field<string>("FUnitID_FName"),
                                            m_funit = item.Field<int>("FUnitID").ToString(),
                                            m_fromsourcebill = true,
                                            m_batchmanagment = item.Field<bool>("FBatchManager"),
                                            m_fsup = item.Field<int>("FSupplyID"),
                                            m_fsupname = item.Field<string>("FSupplyID_FName"),
                                            m_fsource_interid = item.Field<int>("FInterID"),
                                            m_fsource_entryid = item.Field<int>("FEntryID")

                                        }).ToList();

                                        this.m_EntryList_list = entryList;
                                        var ada = new Tools_Tables_Adapter_Class.Entry_Adapter(this.Activity, ((Fragment_InStock)this).m_EntryList_list);
                                        //ada.setFragment(this);
                                        this.Activity.RunOnUiThread(()=> {
                                            this.m_itemlist.Adapter = ada;
                                            Tools_Tables_Adapter_Class.TypeEntry.getListViewHeigth(this.m_itemlist);
                                            
                                        });
                                        
                                        //getListViewHeigth(((Fragment_InStock)m_fragment).m_itemlist);

                                    })
                                    { IsBackground=true};
                                    t.Start();

                                }
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
            m_currentType = "";
        }

        public void clear()
        {
            m_date_picker_edittext.Text = "";
            m_supply_select_edit.Text = "";
            m_fnote.Text = "";
            m_operator.Text = "";
            m_orgbill.Text = "";
            m_Stock_Header.Clear();
            new Thread(new ThreadStart(() =>
            {
                var _para = new SqlParameter[] {
                new SqlParameter("@TableName","ICStockBill"),
                new SqlParameter("@FInterID",SqlDbType.NVarChar,50),
                new SqlParameter("@Increment","1"),
                new SqlParameter("@UserID","16394")
                };
                _para[1].Direction = ParameterDirection.Output;
                var _ret = Tools_SQL_Class.ExecutProcedure("dbo.GetICMaxNum ", CommandType.StoredProcedure, _para);
                Activity.RunOnUiThread(()=> {
                    m_Stock_Header.m_FInterID = _para[1].Value == null ? 0 : Convert.ToInt32(_para[1].Value.ToString());
                });

                var para = new SqlParameter[] {
                new SqlParameter("@IsSave","1"),
                new SqlParameter("@FBillType","1"),
                new SqlParameter("@BillID",SqlDbType.NVarChar,500)
                };
                para[2].Direction = ParameterDirection.Output;
                var ret = Tools_SQL_Class.ExecutProcedure("dbo.GetICBillNo ", CommandType.StoredProcedure, para);
                var ret_value = para[2].Value == null ? "" : para[2].Value.ToString();
                if (ret_value != "")
                {
                    m_Stock_Header.m_Fbillno = ret_value;
                    Activity.RunOnUiThread(() => {
                        m_billno.Text = ret_value;
                    });
                }

            })).Start();
            //EditText m_date_picker_edittext = null, m_billno = null, m_supply_select_edit = null, m_fnote = null;
            m_itemlist.Adapter = null;
            m_EntryList_list.Clear();
        }
        private void M_supply_select_Click(object sender, EventArgs e)
        {
            m_currentType = Tools_Tables_Adapter_Class.ItemType.Supply;
            var intent = new Intent(Application.Context, typeof(Activity_ItemSelect_Class));
            intent.PutExtra("Type", m_currentType);
            StartActivityForResult(intent, 0);
        }

        private void M_date_picker_Click(object sender, EventArgs e)
        {
            //var dialog = new Tools_Tables_Adapter_Class.MDFDatePickerDialog(this.Activity, m_date_picker_edittext);
            //var dialog = new Android.App.DatePickerDialog(this.Context);
            ////MaterialDateTimePicker
            //dialog.Show();

            Tools_Tables_Adapter_Class.DatePickerFragment frag = Tools_Tables_Adapter_Class.DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                m_date_picker_edittext.Text = time.ToString("yyyy/MM/dd");
                m_Stock_Header.m_FDate = time.ToString("yyyy/MM/dd"); 
            });
            frag.Show(this.FragmentManager, Tools_Tables_Adapter_Class.DatePickerFragment.TAG);
        }


        public override void OnResume()
        {
            base.OnResume();
            //this.View.FindViewById
            if (m_Stock_Header.m_FDate != "")
            {
                if (m_date_picker_edittext != null)
                {
                    m_date_picker_edittext.Text = m_Stock_Header.m_FDate;
                }  
            }
            if (m_Stock_Header.m_FSupplyName != "")
            {
                if (m_supply_select_edit != null)
                {
                    m_supply_select_edit.Text = m_Stock_Header.m_FSupplyName;
                }
            }
            if (m_Stock_Header.m_Fbillno != "")
            {
                if (m_billno != null)
                {
                    m_billno.Text = m_Stock_Header.m_Fbillno;
                }
            }
            if (m_Stock_Header.m_FNote != "")
            {
                if (m_fnote != null)
                {
                    m_fnote.Text = m_Stock_Header.m_FNote;
                }
            }
            if (m_Stock_Header.m_FoperatorName != "")
            {
                if (m_operator != null)
                {
                    m_operator.Text = m_Stock_Header.m_FoperatorName;
                }
            }
            if (m_EntryList_list != null && m_EntryList_list.Count > 0)
            {
                if (m_itemlist != null)
                {
                    var adapter = new Tools_Tables_Adapter_Class.Entry_Adapter(Activity, m_EntryList_list);
                    m_itemlist.Adapter = adapter;
                }
            }
        }
        //
    }
}