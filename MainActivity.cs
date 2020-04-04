﻿using System;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Device;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Views;
using Android.Widget;

namespace Android_KingHoo_Scanner_Rebuild
{

    [Activity(Label = "@string/company_main_title", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        #region override
        //    <FrameLayout
        //  android:id="@+id/main_fragment_layout"
        //  android:layout_width="match_parent"
        //  android:layout_height="match_parent">
        //<fragment
        //  android:name="Android_Scanner.StockOut_Fragment"
        //  android:id="@+id/main_fragment"
        //  android:layout_width="match_parent"
        //  android:layout_height="match_parent"/>
        //ScanDevice m_sd = new ScanDevice();
        public static string u_title = "";
        public static string u_message = "";
        public delegate void g_ReciveData(string data);
        public event g_ReciveData g_ProcessReciveData;

        public static MainActivity ms_thisPointer = null;
        private FloatingActionButton m_fab = null;
        public static string m_CurrentUserID = "";

        private Android.Support.V4.App.Fragment m_current_fragment = null;
        Scanner_Receiver m_sr = new Scanner_Receiver();
        private const String SCAN_ACTION = "scan.rcv.message";
        [BroadcastReceiver(Enabled = true)]
        [IntentFilter(new[] { SCAN_ACTION })]
        public class Scanner_Receiver : BroadcastReceiver
        {
            public delegate void ReciveData(string data);
            public event ReciveData ProcessReciveData;

            public override void OnReceive(Context context, Intent intent)
            {
                byte[] barocode = intent.GetByteArrayExtra("barocode");
                if (ProcessReciveData != null)
                {
                    ProcessReciveData(System.Text.Encoding.Default.GetString(barocode));
                }
            }
        }

        int m_selectedItem = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            m_fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            m_fab.Click += FabOnClick;
            m_fab.Visibility = ViewStates.Invisible;
            //m_fab.SetImageBitmap(Tools_Tables_Adapter_Class.textAsBitmap("OK",40,Color.Aqua));

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            var sn = getsn();

            //checkinventory

            ms_thisPointer = this;
            if(u_title!="" && u_message != "")
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, u_title, u_message);
            }
            var t = new Thread(async () =>
            {
                await Tools_Github.greetingMsg(this);
            });
            t.IsBackground = true;
            t.Start();
        }
        public string getsn() {
            TelephonyManager telephonyMgr = this.GetSystemService(Context.TelephonyService) as TelephonyManager;
            string deviceId = telephonyMgr.DeviceId == null ? "UNAVAILABLE" : telephonyMgr.DeviceId;
            return deviceId;
        }
        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                Tools_Tables_Adapter_Class.ShowDialog(this, "注意", "您是否要退出程序？",
                    ()=> { Process.KillProcess(Android.OS.Process.MyPid()); },
                    ()=> { });
                //base.OnBackPressed();
            }
        }

        IMenuItem m_save_menu = null;
        IMenuItem m_check_menu = null;
        IMenuItem m_delete_menu = null;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            //var save = FindViewById(Resource.Id.action_menu_save);

            m_save_menu = menu.FindItem(Resource.Id.action_menu_save);
            m_check_menu = menu.FindItem(Resource.Id.action_menu_check);
            m_delete_menu = menu.FindItem(Resource.Id.action_menu_delete);


            Tools_Extend_Storage tes = new Tools_Extend_Storage(this);
            var _name = FindViewById<TextView>(Resource.Id.menu_userprofile_name);
            _name.Text = tes.getValueString(Tools_Extend_Storage.ValueType.userProfile_Name);
            var _description = FindViewById<TextView>(Resource.Id.menu_userprofile_description);
            _description.Text = tes.getValueString(Tools_Extend_Storage.ValueType.userProfile_Description);

            m_save_menu.SetVisible(false);
            m_check_menu.SetVisible(false);
            m_delete_menu.SetVisible(false);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            switch (m_selectedItem)
            {
                case Resource.Id.menu_side_bar_inStock:
                    {
                        ProcessMenuItemClick_instock(id);
                    }
                    break;
                case Resource.Id.menu_side_bar_outStock:
                    {
                        ProcessMenuItemClick_outstock(id);
                    }
                    break;
                case Resource.Id.menu_side_bar_llStockout:
                    {
                        ProcessMenuItemClick_outstockx(id);
                    }
                    break;
                default:
                    break;
            }


            return base.OnOptionsItemSelected(item);
        }

        void ProcessMenuItemClick_outstockx(int id)
        {
            if (id == Resource.Id.action_menu_save)
            {
                var progrss = new Tools_Tables_Adapter_Class.ShowPrograss(this);
                progrss.Show();
                if (Fragment_OutStockX.Instance().m_EntryList_list.Count > 0)
                {
                    if (Fragment_OutStockX.Instance().m_Stock_Header.m_FDate != "" &&
                        Fragment_OutStockX.Instance().m_Stock_Header.m_FCustomer != "" &&
                        Fragment_OutStockX.Instance().m_Stock_Header.m_FInterID != 0 &&
                        Fragment_OutStockX.Instance().m_Stock_Header.m_Foperator != "" &&
                         Fragment_OutStockX.Instance().m_Stock_Header.m_Fbillno != "")
                    {
                        var T = new Thread(new ThreadStart(() =>
                        {
                            string[] sql_list = new string[Fragment_OutStockX.Instance().m_EntryList_list.Count + 1];
                            for (int i = 0; i < Fragment_OutStockX.Instance().m_EntryList_list.Count; i++)
                            {
                                var item = Fragment_OutStockX.Instance().m_EntryList_list[i];
                                var sql__ = "SELECT 1 FROM dbo.ICInventory A JOIN dbo.t_ICItem B ON A.FItemID=B.FItemID " +
                                    "JOIN dbo.t_Stock C ON A.FStockID = C.FItemID" +
                                    "JOIN dbo.t_StockPlace D ON A.FStockPlaceID = D.FSPID WHERE A.FBatchNo = " + (item.m_fbatchno == "" ? "A.FBatchNo" : "'" + item.m_fbatchno + "'") + " AND B.FNumber = '" + item.m_fnumber + "' AND D.FNumber = " + (item.m_fbatchno == "" ? "D.FNumber" : "'" + item.m_fbatchno + "'") + " AND C.FNumber = '" + item.m_fstock + "'";
                                var ret = Tools_SQL_Class.getTable(sql__);
                                if (ret != null && ret.Rows.Count > 0)
                                {
                                    progrss.Dismiss();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "库存不足，无法出库");
                                    return;
                                }
                                sql_list[i] = "INSERT INTO dbo.ICStockBillEntry( FBrNo ,FInterID ,FEntryID ,FItemID ,FQtyMust ,FQty ,FPrice ,FBatchNo ,FAmount ,FNote ,FUnitID ,FAuxPrice ,FAuxQty ,FAuxQtyMust ,FSCStockID ,FDCStockID ,FPlanMode ,FChkPassItem,FDCSPID)" + "VALUES('0'," + Fragment_OutStockX.Instance().m_Stock_Header.m_FInterID + "," + i.ToString() + "," + "(select FItemID from t_ICItem where FNumber = '" + item.m_fnumber + "')," + item.m_fqty.ToString() + "," + item.m_fqty.ToString() + ", 0,'" + item.m_fbatchno + "', 0,'" + item.m_fnote + "', (select FUnitID from t_ICItem where FNumber = '" + item.m_fnumber + "'), 0, " + item.m_fqty.ToString() + ", " + item.m_fqty.ToString() + ", " + "0, (select FItemID from t_Stock where FNumber = '" + item.m_fstock + "' ), 14036, 1058, (select FSPID from t_StockPlace where FSPID = " + (item.m_fstockplace_fitemid == "" || item.m_fstockplace == null ? "0" : item.m_fstockplace_fitemid) + "))";
                            }
                            var ___sql = "INSERT INTO dbo.ICStockBill( FBrNo ,FInterID ,FTranType ,FDate ,FBillNo ,FNote ,FDCStockID ,FSCStockID ,FDeptID ,FSupplyID,FEmpID ,FFManagerID ,FSManagerID ,FBillerID ,FROB ,FUpStockWhenSave ,FUUID , FMarketingStyle ,FSourceType ,FPOStyle) VALUES ('0'," + Fragment_OutStockX.Instance().m_Stock_Header.m_FInterID + ", 24, GETDATE(),'" + Fragment_OutStockX.Instance().m_Stock_Header.m_Fbillno + "', '" + Fragment_OutStockX.Instance().m_Stock_Header.m_FNote + "', 0, 0, (select FItemID from t_Organization where FNumber = '" + Fragment_OutStockX.Instance().m_Stock_Header.m_FCustomer + "'),0, (select FItemID from t_Emp where FNumber = '" + Fragment_OutStockX.Instance().m_Stock_Header.m_Foperator + "'),(select FItemID from t_Emp where FNumber = '" + Fragment_OutStockX.Instance().m_Stock_Header.m_Foperator + "'), (select FItemID from t_Emp where FNumber = '" + Fragment_OutStockX.Instance().m_Stock_Header.m_Foperator + "'), (select FUserID from t_user where FUserID = " + m_CurrentUserID + "), 1, 1,NEWID(),12530,37521,251);";
                            sql_list[Fragment_OutStockX.Instance().m_EntryList_list.Count] = ___sql;

                            var __ret = Tools_SQL_Class.TransationAutoCommit(sql_list);
                            if (__ret != "")
                            {
                                RunOnUiThread(() =>
                                {
                                    progrss.Dismiss();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", __ret);
                                });

                                return;
                            }
                            else
                            {
                                Tools_SQL_Class.ExecutProcedure("dbo.CheckInventory", System.Data.CommandType.StoredProcedure, null);
                                RunOnUiThread(() =>
                                {
                                    progrss.Dismiss();
                                    Fragment_OutStockX.Instance().clear();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "提示", "单据保存成功！");

                                    return;
                                });

                            }
                        }));
                        T.IsBackground = true;
                        T.Start();

                    }
                    else
                    {
                        progrss.Dismiss();
                        Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "单据头没有填写完整！");
                        return;
                    }

                }
                else
                {
                    progrss.Dismiss();
                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "您还没有插入任何分录！");
                    return;
                }
            }
            else
            if (id == Resource.Id.action_menu_check)
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "还不支持审核操作！");
            }
            else
            if (id == Resource.Id.action_menu_delete)
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "还不支持删除操作！");
            }
        }

        void ProcessMenuItemClick_outstock(int id)
        {
            if (id == Resource.Id.action_menu_save)
            {
                var progrss = new Tools_Tables_Adapter_Class.ShowPrograss(this);
                progrss.Show();
                if (Fragment_OutStock.Instance().m_EntryList_list.Count > 0)
                {
                    if (Fragment_OutStock.Instance().m_Stock_Header.m_FDate != "" &&
                        Fragment_OutStock.Instance().m_Stock_Header.m_FCustomer != "" &&
                        Fragment_OutStock.Instance().m_Stock_Header.m_FInterID != 0 &&
                        Fragment_OutStock.Instance().m_Stock_Header.m_Foperator != "" &&
                         Fragment_OutStock.Instance().m_Stock_Header.m_Fbillno != "")
                    {
                        var T = new Thread(new ThreadStart(() =>
                        {
                            string[] sql_list = new string[Fragment_OutStock.Instance().m_EntryList_list.Count + 1];
                            for (int i = 0; i < Fragment_OutStock.Instance().m_EntryList_list.Count; i++)
                            {
                                var item = Fragment_OutStock.Instance().m_EntryList_list[i];
                                var sql__ = "SELECT 1 FROM dbo.ICInventory A JOIN dbo.t_ICItem B ON A.FItemID=B.FItemID " +
                                    "JOIN dbo.t_Stock C ON A.FStockID = C.FItemID" +
                                    "JOIN dbo.t_StockPlace D ON A.FStockPlaceID = D.FSPID WHERE A.FBatchNo = " + (item.m_fbatchno == "" ? "A.FBatchNo" : "'" + item.m_fbatchno + "'") + " AND B.FNumber = '" + item.m_fnumber + "' AND D.FNumber = " + (item.m_fbatchno == "" ? "D.FNumber" : "'" + item.m_fbatchno + "'") + " AND C.FNumber = '" + item.m_fstock + "'";
                                var ret = Tools_SQL_Class.getTable(sql__);
                                if (ret != null && ret.Rows.Count > 0)
                                {
                                    progrss.Dismiss();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "库存不足，无法出库");
                                    return;
                                }
                                sql_list[i] = "INSERT INTO dbo.ICStockBillEntry( FBrNo ,FInterID ,FEntryID ,FItemID ,FQtyMust ,FQty ,FPrice ,FBatchNo ,FAmount ,FNote ,FUnitID ,FAuxPrice ,FAuxQty ,FAuxQtyMust ,FSCStockID ,FDCStockID ,FPlanMode ,FChkPassItem,FDCSPID)" + "VALUES('0'," + Fragment_OutStock.Instance().m_Stock_Header.m_FInterID + "," + i.ToString() + "," + "(select FItemID from t_ICItem where FNumber = '" + item.m_fnumber + "')," + item.m_fqty.ToString() + "," + item.m_fqty.ToString() + ", 0,'" + item.m_fbatchno + "', 0,'" + item.m_fnote + "', (select FUnitID from t_ICItem where FNumber = '" + item.m_fnumber + "'), 0, " + item.m_fqty.ToString() + ", " + item.m_fqty.ToString() + ", " + "0, (select FItemID from t_Stock where FNumber = '" + item.m_fstock + "' ), 14036, 1058, (select FSPID from t_StockPlace where FSPID = " + (item.m_fstockplace_fitemid == "" || item.m_fstockplace == null ? "0" : item.m_fstockplace_fitemid) + "))";
                            }
                            var ___sql = "INSERT INTO dbo.ICStockBill( FBrNo ,FInterID ,FTranType ,FDate ,FBillNo ,FNote ,FDCStockID ,FSCStockID ,FDeptID ,FSupplyID,FEmpID ,FFManagerID ,FSManagerID ,FBillerID ,FROB ,FUpStockWhenSave ,FUUID , FMarketingStyle ,FSourceType ,FPOStyle) VALUES ('0'," + Fragment_OutStock.Instance().m_Stock_Header.m_FInterID + ", 21, GETDATE(),'" + Fragment_OutStock.Instance().m_Stock_Header.m_Fbillno + "', '" + Fragment_OutStock.Instance().m_Stock_Header.m_FNote + "', 0, 0, 0,(select FItemID from t_Organization where FNumber = '" + Fragment_OutStock.Instance().m_Stock_Header.m_FCustomer + "'), (select FItemID from t_Emp where FNumber = '" + Fragment_OutStock.Instance().m_Stock_Header.m_Foperator + "'),(select FItemID from t_Emp where FNumber = '" + Fragment_OutStock.Instance().m_Stock_Header.m_Foperator + "'), (select FItemID from t_Emp where FNumber = '" + Fragment_OutStock.Instance().m_Stock_Header.m_Foperator + "'), (select FUserID from t_user where FUserID = " + m_CurrentUserID + "), 1, 1,NEWID(),12530,37521,251);";
                            sql_list[Fragment_OutStock.Instance().m_EntryList_list.Count] = ___sql;

                            var __ret = Tools_SQL_Class.TransationAutoCommit(sql_list);
                            if (__ret != "")
                            {
                                RunOnUiThread(() =>
                                {
                                    progrss.Dismiss();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", __ret);
                                });

                                return;
                            }
                            else
                            {
                                Tools_SQL_Class.ExecutProcedure("dbo.CheckInventory", System.Data.CommandType.StoredProcedure, null);
                                RunOnUiThread(() =>
                                {
                                    progrss.Dismiss();
                                    Fragment_OutStock.Instance().clear();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "提示", "单据保存成功！");

                                    return;
                                });

                            }
                        }));
                        T.IsBackground = true;
                        T.Start();

                    }
                    else
                    {
                        progrss.Dismiss();
                        Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "单据头没有填写完整！");
                        return;
                    }

                }
                else
                {
                    progrss.Dismiss();
                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "您还没有插入任何分录！");
                    return;
                }
            }
            else
            if (id == Resource.Id.action_menu_check)
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "还不支持审核操作！");
            }
            else
            if (id == Resource.Id.action_menu_delete)
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "还不支持删除操作！");
            }
        }
        void ProcessMenuItemClick_instock(int id)
        {
            if (id == Resource.Id.action_menu_save)
            {
                var progrss = new Tools_Tables_Adapter_Class.ShowPrograss(this);
                progrss.Show();
                if (Fragment_InStock.Instance().m_EntryList_list.Count > 0)
                {
                    if (Fragment_InStock.Instance().m_Stock_Header.m_FDate != "" &&
                        Fragment_InStock.Instance().m_Stock_Header.m_FSupply != "" &&
                        Fragment_InStock.Instance().m_Stock_Header.m_FInterID != 0 &&
                         Fragment_InStock.Instance().m_Stock_Header.m_Foperator != "" &&
                        Fragment_OutStock.Instance().m_Stock_Header.m_Fbillno != "")
                    {
                        var T = new Thread(new ThreadStart(() =>
                        {
                            string[] sql_list = new string[Fragment_InStock.Instance().m_EntryList_list.Count + 1];
                            //校验库存
                            for (int i = 0; i < Fragment_InStock.Instance().m_EntryList_list.Count; i++)
                            {
                                var item = Fragment_InStock.Instance().m_EntryList_list[i];

                                sql_list[i] = "INSERT INTO dbo.ICStockBillEntry( FBrNo ,FInterID ,FEntryID ,FItemID ,FQtyMust ,FQty ,FPrice ,FBatchNo ,FAmount ,FNote ,FUnitID ,FAuxPrice ,FAuxQty ,FAuxQtyMust ,FSCStockID ,FDCStockID ,FPlanMode ,FChkPassItem,FDCSPID)" + "VALUES('0'," + Fragment_InStock.Instance().m_Stock_Header.m_FInterID + "," + i.ToString() + "," + "(select FItemID from t_ICItem where FNumber = '" + item.m_fnumber + "')," + item.m_fqty.ToString() + "," + item.m_fqty.ToString() + ", 0,'" + item.m_fbatchno + "', 0,'" + item.m_fnote + "', (select FUnitID from t_ICItem where FNumber = '" + item.m_fnumber + "'), 0, " + item.m_fqty.ToString() + ", " + item.m_fqty.ToString() + ", " + "0, (select FItemID from t_Stock where FNumber = '" + item.m_fstock + "' ), 14036, 1058, (select FSPID from t_StockPlace where FSPID = " + (item.m_fstockplace == "" || item.m_fstockplace == null ? "0" : item.m_fstockplace) + "))";
                            }
                            var ___sql = "INSERT INTO dbo.ICStockBill( FBrNo ,FInterID ,FTranType ,FDate ,FBillNo ,FNote ,FDCStockID ,FSCStockID ,FDeptID ,FSupplyID,FEmpID ,FFManagerID ,FSManagerID ,FBillerID ,FROB ,FUpStockWhenSave ,FUUID , FMarketingStyle ,FSourceType ,FPOStyle) VALUES ('0'," + Fragment_InStock.Instance().m_Stock_Header.m_FInterID + ", 1, GETDATE(),'" + Fragment_InStock.Instance().m_Stock_Header.m_Fbillno + "', '" + Fragment_InStock.Instance().m_Stock_Header.m_FNote + "', 0, 0, 0,(select FItemID from t_Supplier where FNumber = '" + Fragment_InStock.Instance().m_Stock_Header.m_FSupply + "'), (select FItemID from t_Emp where FNumber = '" + Fragment_InStock.Instance().m_Stock_Header.m_Foperator + "'),(select FItemID from t_Emp where FNumber = '" + Fragment_InStock.Instance().m_Stock_Header.m_Foperator + "'), (select FItemID from t_Emp where FNumber = '" + Fragment_InStock.Instance().m_Stock_Header.m_Foperator + "'), (select FUserID from t_user where FUserID = " + m_CurrentUserID + "), 1, 1,NEWID(),12530,37521,251);";
                            sql_list[Fragment_InStock.Instance().m_EntryList_list.Count] = ___sql;

                            var __ret = Tools_SQL_Class.TransationAutoCommit(sql_list);
                            if (__ret != "")
                            {
                                RunOnUiThread(() =>
                                {
                                    progrss.Dismiss();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", __ret);
                                });

                                return;
                            }
                            else
                            {
                                Tools_SQL_Class.ExecutProcedure("dbo.CheckInventory", System.Data.CommandType.StoredProcedure, null);
                                RunOnUiThread(() =>
                                {
                                    progrss.Dismiss();
                                    Fragment_InStock.Instance().clear();
                                    Tools_Tables_Adapter_Class.ShowMsg(this, "提示", "单据保存成功！");
                                    return;
                                });
                            }
                        }));
                        T.IsBackground = true;
                        T.Start();
                    }
                    else
                    {
                        progrss.Dismiss();
                        Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "单据头没有填写完整！");
                        return;
                    }


                }
                else
                {
                    progrss.Dismiss();
                    Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "您还没有插入任何分录！");
                    return;
                }
            }
            else
            if (id == Resource.Id.action_menu_check)
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "还不支持审核操作！");
            }
            else
            if (id == Resource.Id.action_menu_delete)
            {
                Tools_Tables_Adapter_Class.ShowMsg(this, "错误", "还不支持审核操作！");
            }
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            if (m_current_fragment != null)
            {
                if (m_current_fragment == Fragment_InStock.Instance())
                {
                    var Dialog = new Tools_Tables_Adapter_Class.TypeEntry(this, (Fragment_InStock)m_current_fragment, "IN", this);
                    Dialog.Show();
                }
                else if (m_current_fragment == Fragment_OutStock.Instance())
                {
                    var Dialog = new Tools_Tables_Adapter_Class.TypeEntry(this, (Fragment_OutStock)m_current_fragment, "OUT", this);
                    Dialog.Show();
                }
                else if (m_current_fragment == Fragment_OutStockX.Instance())
                {
                    var Dialog = new Tools_Tables_Adapter_Class.TypeEntry(this, (Fragment_OutStockX)m_current_fragment, "XOUT", this);
                    Dialog.Show();

                }
            }
        }

        long m_lastSelectedFragment = 0;
        bool m_welcomUIShow = true;
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (m_welcomUIShow)
            {
                Fragment_Welcome.Instance().View.Visibility = ViewStates.Invisible;
                m_welcomUIShow = false;
            }

            if (id == Resource.Id.menu_side_bar_checkInventory && m_lastSelectedFragment != Resource.Id.menu_side_bar_checkInventory)
            {
                m_save_menu.SetVisible(false);
                m_check_menu.SetVisible(false);
                m_delete_menu.SetVisible(false);
                this.Title = "扫码盘点";
                m_fab.Visibility = ViewStates.Invisible;
                m_selectedItem = Resource.Id.menu_side_bar_checkInventory;
                Android.Support.V4.App.FragmentTransaction fragmentTx = SupportFragmentManager.BeginTransaction();
                m_current_fragment = Fragment_CheckInventory.Instance();
                fragmentTx.Replace(Resource.Id.main_fragment_layout, m_current_fragment);
                fragmentTx.Commit();
                if (Fragment_CheckInventory.Instance().View != null)
                {
                    Fragment_CheckInventory.Instance().View.Visibility = ViewStates.Invisible;
                }
                m_lastSelectedFragment = Resource.Id.menu_side_bar_checkInventory;
                // Handle the camera action
            }
            else if (id == Resource.Id.menu_side_bar_inStock && m_lastSelectedFragment != Resource.Id.menu_side_bar_inStock)
            {
                m_save_menu.SetVisible(true);
                m_check_menu.SetVisible(true);
                m_delete_menu.SetVisible(true);
                this.Title = "外购入库";
                m_fab.Visibility = ViewStates.Visible;
                m_selectedItem = Resource.Id.menu_side_bar_inStock;
                //m_selectedItem = Resource.Id.menu_side_bar_checkInventory;
                Android.Support.V4.App.FragmentTransaction fragmentTx = SupportFragmentManager.BeginTransaction();
                m_current_fragment = Fragment_InStock.Instance();
                fragmentTx.Replace(Resource.Id.main_fragment_layout, m_current_fragment);
                fragmentTx.Commit();
                if (Fragment_InStock.Instance().View != null)
                {
                    Fragment_InStock.Instance().View.Visibility = ViewStates.Invisible;
                }
                m_lastSelectedFragment = Resource.Id.menu_side_bar_inStock;
            }
            else if (id == Resource.Id.menu_side_bar_outStock && m_lastSelectedFragment != Resource.Id.menu_side_bar_outStock)
            {
                m_save_menu.SetVisible(true);
                m_check_menu.SetVisible(true);
                m_delete_menu.SetVisible(true);
                this.Title = "销售出库";
                m_fab.Visibility = ViewStates.Visible;
                m_selectedItem = Resource.Id.menu_side_bar_outStock;
                //m_selectedItem = Resource.Id.menu_side_bar_checkInventory;
                Android.Support.V4.App.FragmentTransaction fragmentTx = SupportFragmentManager.BeginTransaction();
                m_current_fragment = Fragment_OutStock.Instance();
                fragmentTx.Replace(Resource.Id.main_fragment_layout, m_current_fragment);
                fragmentTx.Commit();
                if (Fragment_OutStock.Instance().View != null)
                {
                    Fragment_OutStock.Instance().View.Visibility = ViewStates.Invisible;
                }
                m_lastSelectedFragment = Resource.Id.menu_side_bar_outStock;
            }
            else if (id == Resource.Id.menu_side_bar_setting && m_lastSelectedFragment != Resource.Id.menu_side_bar_setting)
            {
                m_save_menu.SetVisible(false);
                m_check_menu.SetVisible(false);
                m_delete_menu.SetVisible(false);
                this.Title = "系统设置";
                m_fab.Visibility = ViewStates.Invisible;
                m_selectedItem = Resource.Id.menu_side_bar_setting;
                Android.Support.V4.App.FragmentTransaction fragmentTx = SupportFragmentManager.BeginTransaction();
                m_current_fragment = Fragment_Setting_InApp.Instance();
                fragmentTx.Replace(Resource.Id.main_fragment_layout, m_current_fragment);
                fragmentTx.Commit();
                if (Fragment_Setting_InApp.Instance().View != null)
                {
                    Fragment_Setting_InApp.Instance().View.Visibility = ViewStates.Invisible;
                }
                m_lastSelectedFragment = Resource.Id.menu_side_bar_setting;
            }
            else if (id == Resource.Id.menu_side_bar_llStockout && m_lastSelectedFragment != Resource.Id.menu_side_bar_llStockout)
            {
                m_save_menu.SetVisible(true);
                m_check_menu.SetVisible(true);
                m_delete_menu.SetVisible(true);
                this.Title = "领料出库";
                m_fab.Visibility = ViewStates.Visible;
                m_selectedItem = Resource.Id.menu_side_bar_llStockout;
                //m_selectedItem = Resource.Id.menu_side_bar_checkInventory;
                Android.Support.V4.App.FragmentTransaction fragmentTx = SupportFragmentManager.BeginTransaction();
                m_current_fragment = Fragment_OutStockX.Instance();
                fragmentTx.Replace(Resource.Id.main_fragment_layout, m_current_fragment);
                fragmentTx.Commit();
                if (Fragment_OutStockX.Instance().View != null)
                {
                    Fragment_OutStockX.Instance().View.Visibility = ViewStates.Invisible;
                }
                m_lastSelectedFragment = Resource.Id.menu_side_bar_llStockout;
            }
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            
            m_sr.ProcessReciveData += M_sr_ProcessReciveData;
            RegisterReceiver(m_sr, new IntentFilter(SCAN_ACTION));
            // Code omitted for clarity
        }

        //处理扫描头的信息
        private void M_sr_ProcessReciveData(string data)
        {
            if (g_ProcessReciveData != null)
            {
                g_ProcessReciveData(data);
            }

            if (m_selectedItem == Resource.Id.menu_side_bar_checkInventory)
            {

            }
            else if (m_selectedItem == Resource.Id.menu_side_bar_inStock)
            {

            }
            else if (m_selectedItem == Resource.Id.menu_side_bar_outStock)
            {

            }
            else if (m_selectedItem == Resource.Id.menu_side_bar_setting)
            {

            }
            else if (m_selectedItem == Resource.Id.menu_side_bar_llStockout)
            {

            }
        }

        protected override void OnPause()
        {
            m_sr.ProcessReciveData -= M_sr_ProcessReciveData;
            UnregisterReceiver(m_sr);
            // Code omitted for clarity
            base.OnPause();
        }
        #endregion


    }
}


