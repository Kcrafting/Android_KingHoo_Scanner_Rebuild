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
using System.Data;
using System.Data.SqlClient;
using Android.Graphics;
using Android.Util;
using Android.Support.V7.Widget;
using Java.Util;
using Android.Text.Format;
using System.Threading;
using System.Linq.Expressions;
using Android.Support.V4.View;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Tools_Tables_Adapter_Class
    {
        public static DataTable m_login_userList = null;
        public static bool m_DatabaseLegal = false;
        public static bool m_DatabaseExist = false;
        public static bool m_AccountListed = false;
        public static void ShowMsg(Context context, string title, string content)
        {
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.Show();
        }

        //public static void ShowDialog(Context context, string title, string content, Action ok, Action cancel)
        //{
        //    var dialog = new Android.Support.V7.App.AlertDialog.Builder(context);
        //    dialog.SetTitle(title);
        //    dialog.SetMessage(content);
        //    dialog.SetPositiveButton(Resource.String.CANCEL, new EventHandler<DialogClickEventArgs>((__sender, __event) => {; }));
        //    dialog.SetNegativeButton(Resource.String.QUIT, new EventHandler<DialogClickEventArgs>((__sender, __event) => { Process.KillProcess(Android.OS.Process.MyPid()); }));
        //    dialog.Show();
        //}

        public static void ShowDialog(Context context, string title, string content,string okText = "",string cancelText = "", Action ok = null,Action cancel = null)
        {
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetPositiveButton(cancelText, new EventHandler<DialogClickEventArgs>((__sender, __event) => { cancel(); } )) ;
            dialog.SetNegativeButton(okText, new EventHandler<DialogClickEventArgs>((__sender, __event) => { ok(); /* Process.KillProcess(Android.OS.Process.MyPid());*/ }));
            dialog.Show();
        }

        public class Login_Account : Java.Lang.Object
        {
            public string UserName { get; set; }
            public string UserID { get; set; }
            public string UserPassword { get; }
        }
        public class Login_AccountList : BaseAdapter
        {
            private List<Login_Account> m_list;
            private Context context;
            //private int ResourceID = 0;
            public Login_AccountList(Context pContext, List<Login_Account> pList/*,int resourceId*/)
            {
                context = pContext;
                m_list = pList;
                //ResourceID = resourceId;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                LayoutInflater _LayoutInflater = LayoutInflater.From(context);
                convertView = _LayoutInflater.Inflate(Resource.Layout.activity_login_account_list_layout, null);
                if (convertView != null)
                {
                    TextView _TextView1 = (TextView)convertView.FindViewById<TextView>(Resource.Id.activity_login_account_list_layout_label);
                    _TextView1.Text = m_list.ElementAt<Login_Account>(position).UserName;

                }
                return convertView;
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return m_list.ElementAt(position);
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override int Count { get { return m_list.Count; } }
        }
        public class Account_Detail : Java.Lang.Object
        {
            public string FAcctName { get; set; }
            public string FDBName { get; set; }
        }

        public class KingHoo_Generic_ViewHolder : Java.Lang.Object
        {
            private SparseArray<View> Views;
            View ConvertView;
            private Context context;
            int mPosition;
            private KingHoo_Generic_ViewHolder(Context _context, ViewGroup parent, int itemLayoutId, int position)
            {
                this.mPosition = position;
                Views = new SparseArray<View>();
                ConvertView = LayoutInflater.From(_context).Inflate(itemLayoutId, null);
                ConvertView.Tag = this;
            }
            public static KingHoo_Generic_ViewHolder Get(Context context, View convertView, ViewGroup parent, int itemLayoutId, int position)
            {
                if (convertView == null)
                {
                    return new KingHoo_Generic_ViewHolder(context, parent, itemLayoutId, position);
                }
                else
                {
                    KingHoo_Generic_ViewHolder holder = (KingHoo_Generic_ViewHolder)convertView.Tag;
                    holder.mPosition = position;
                    return holder;
                }
            }
            public T GetView<T>(int viewId) where T : View
            {
                View view = Views.Get(viewId);
                if (view == null)
                {
                    view = ConvertView.FindViewById<T>(viewId);
                    Views.Put(viewId, view);
                }
                return (T)view;
            }
            public View GetConvertView()
            {
                return ConvertView;
            }
            /// <summary>
               /// 给TextView 设置文本
               /// </summary>
               /// <param name="viewId"></param>
               /// <param name="text"></param>
               /// <returns></returns>
            public KingHoo_Generic_ViewHolder SetText(int viewId, string text)
            {
                TextView view = GetView<TextView>(viewId);
                view.Text = text;
                return this;
            }
            public KingHoo_Generic_ViewHolder SetTag(int viewId, string text)
            {
                TextView view = GetView<TextView>(viewId);
                view.Tag = text;
                return this;
            }

            /// <summary>
               /// 给ImageView 设置图片
               /// </summary>
            public KingHoo_Generic_ViewHolder SetImageBitMap(int viewId, Bitmap bm)
            {
                ImageView view = GetView<ImageView>(viewId);
                view.SetImageBitmap(bm);
                return this;
            }
        }

        public abstract class GenericAdapter<T> : BaseAdapter
        {
            Context mContext;
            List<T> mData;
            int mItemLayoutId;
            public GenericAdapter(Context context, List<T> data, int itemLayoutId) : base()
            {
                this.mContext = context;
                mData = data;
                mItemLayoutId = itemLayoutId;
            }
            public override int Count
            {
                get
                {
                    return mData.Count;
                }
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return null;

            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var item = mData[position];
                KingHoo_Generic_ViewHolder viewHolder = KingHoo_Generic_ViewHolder.Get(mContext, convertView, parent, mItemLayoutId, position);
                convert(viewHolder, mData[position]);

                System.Diagnostics.Debug.Write(position);
                return viewHolder.GetConvertView();
            }
            public abstract void convert(KingHoo_Generic_ViewHolder helper, T item);
            public KingHoo_Generic_ViewHolder GetViewHolder(int position, View convertView, ViewGroup parent)
            {
                return KingHoo_Generic_ViewHolder.Get(mContext, convertView, parent, mItemLayoutId, position);
            }
        }
        public class DataBaseAdapter<T> : GenericAdapter<T>
        {
            public DataBaseAdapter(Context context, List<T> data, int resId) : base(context, data, resId)
            {

            }
            public override void convert(KingHoo_Generic_ViewHolder helper, T item)
            {
                Account_Detail model = (Account_Detail)Convert.ChangeType(item, typeof(Account_Detail));
                helper.SetText(Resource.Id.activity_login_account_list_layout_label, model.FAcctName);
                helper.SetTag(Resource.Id.activity_login_account_list_layout_label, model.FDBName);
                //helper.SetText(Resource.Id.tv_news_id, model.FDBName);
            }
        }

        public class Inventroy_Detail_Row : Java.Lang.Object
        {
            public Inventroy_Detail_Row() { }
            public string m_Bill_Stock;
            public string m_StockPlace;
            public string m_MainUint;
            public string m_SecondUint;
            public string m_Batch;
        }

        public abstract class RecyclerViewGenericAdapter<T> : Android.Support.V7.Widget.RecyclerView.Adapter
        {
            Context mContext;
            List<T> mData;
            int mItemLayoutId;
            public RecyclerViewGenericAdapter(Context context, List<T> data, int itemLayoutId) : base()
            {
                this.mContext = context;
                mData = data;
                mItemLayoutId = itemLayoutId;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                throw new NotImplementedException();
            }
            public override int ItemCount
            {
                get
                {
                    return mData.Count;
                }
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                throw new NotImplementedException();
            }
            public abstract void convert(KingHoo_Generic_ViewHolder helper, T item);
            public KingHoo_Generic_ViewHolder GetViewHolder(int position, View convertView, ViewGroup parent)
            {
                return KingHoo_Generic_ViewHolder.Get(mContext, convertView, parent, mItemLayoutId, position);
            }
        }


        public class CheckInventory_Adapter : Android.Support.V7.Widget.RecyclerView.Adapter
        {
            public static CheckInventory_Adapter m_rva = null;
            public static List<Inventroy_Detail_Row> m_list = new List<Inventroy_Detail_Row>();
            private Context m_context;
            bool m_Ifinit = false;
            public CheckInventory_Adapter(Context context, List<Inventroy_Detail_Row> data)
            {
                m_rva = this;
                m_list = data;
                m_context = context;
            }
            public override int ItemCount
            {
                get
                {
                    return m_list.Count;
                }
            }

            public void Clear()
            {
                m_list.Clear();
            }
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                BillViewHolder bh = holder as BillViewHolder;
                bh.m_TextView_Bill_Stock.Text = m_list.ElementAt<Inventroy_Detail_Row>(position).m_Bill_Stock;
                bh.m_TextView_StockPlace.Text = m_list.ElementAt<Inventroy_Detail_Row>(position).m_StockPlace;
                bh.m_TextView_MainUint.Text = m_list.ElementAt<Inventroy_Detail_Row>(position).m_MainUint;
                bh.m_TextView_SecondUint.Text = m_list.ElementAt<Inventroy_Detail_Row>(position).m_SecondUint;
                bh.m_TextView_Batch.Text = m_list.ElementAt<Inventroy_Detail_Row>(position).m_Batch;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View v = LayoutInflater.FromContext(m_context).Inflate(Resource.Layout.activity_main_check_inventory_entry_layout, parent, false);
                return new BillViewHolder(v, m_context);// new ViewHolderM(v); 
            }

            public class BillViewHolder : RecyclerView.ViewHolder
            {
                //public ImageView Image { get; private set; }
                //public TextView Caption { get; private set; }
                public TextView m_TextView_Bill_Stock { get; private set; }
                public TextView m_TextView_StockPlace { get; private set; }
                public TextView m_TextView_MainUint { get; private set; }
                public TextView m_TextView_SecondUint { get; private set; }
                public TextView m_TextView_Batch { get; private set; }


                //public LinearLayout root_main { get; private set; }
                //public Button Button_QD { get; private set; }
                private Android.Content.Context m_context;
                public BillViewHolder(View itemView, Android.Content.Context context) : base(itemView)
                {
                    m_context = context;
                    // Locate and cache view references:
                    //Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
                    //Caption = itemView.FindViewById<TextView>(Resource.Id.textView);
                    m_TextView_Bill_Stock = itemView.FindViewById<TextView>(Resource.Id.entry_bill_or_stock_label);
                    m_TextView_StockPlace = itemView.FindViewById<TextView>(Resource.Id.entry_tockplace_label);
                    m_TextView_MainUint = itemView.FindViewById<TextView>(Resource.Id.entry_mianuint_label);
                    m_TextView_SecondUint = itemView.FindViewById<TextView>(Resource.Id.entry_seconduint_label);
                    m_TextView_Batch = itemView.FindViewById<TextView>(Resource.Id.entry_batch_label);
                }
            }
        }

        //基础资料 adapter
        public class Item : Java.Lang.Object
        {
            public string m_fitemid;
            public string m_fnumber;
            public string m_fname;
            public string m_fextend;
            public bool m_IfDetail;
        }
        //public class ItemViewHolder : ViewHolder
        public class ItemAdapter : BaseAdapter
        {
            public override int Count { get { return m_list.Count; } }
            List<Item> m_list = null; 
            //根本不需要 组分组，组内容应该包含在列表当中
            //List<string> m_groupKey = null; 
            Context m_context = null;
            public ItemAdapter(Context context, List<Item> list/*, List<string> groupkey*/)
            {
                m_context = context;
                m_list = list;
                //m_groupKey = groupkey;
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return m_list[position];
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View groupkey, ViewGroup parent)
            {

                View view = groupkey;
                if (!m_list[position].m_IfDetail)
                {
                    view = LayoutInflater.From(m_context).Inflate(Resource.Layout.activity_itemSelect_entry_tag_layout, null);

                }
                else
                {
                    view = LayoutInflater.From(m_context).Inflate(Resource.Layout.activity_itemSelect_entry_layout, null);
                }
                var tag_fnumber = view.FindViewById<TextView>(Resource.Id.activity_itemSelect_entry_tag_layout_tag_fnumber);
                var tag_fname = view.FindViewById<TextView>(Resource.Id.activity_itemSelect_entry_tag_layout_tag_fname);
                if (tag_fnumber != null)
                {
                    tag_fnumber.Text = ((Item)GetItem(position)).m_fnumber;
                }
                if (tag_fname != null)
                {
                    tag_fname.Text = ((Item)GetItem(position)).m_fname;
                }

                var fnumber = view.FindViewById<TextView>(Resource.Id.activity_itemSelect_entry_layout_fnumber);
                var fname = view.FindViewById<TextView>(Resource.Id.activity_itemSelect_entry_layout_fname);
                var fmodel = view.FindViewById<TextView>(Resource.Id.activity_itemSelect_entry_layout_fmodel);
                if (fnumber != null)
                {
                    fnumber.Text = ((Item)GetItem(position)).m_fnumber;
                }

                if (fname != null)
                {
                    fname.Text = ((Item)GetItem(position)).m_fname;
                }

                if (fmodel != null)
                {
                    fmodel.Text = ((Item)GetItem(position)).m_fextend;
                }

                return view;

            }
        }
        //public enum ItemType
        //{
        //    ICItem = 1, 
        //    ICStock = 2,
        //    ICStockPlace = 3,
        //    User = 4
        //}
        public class ItemType
        {
            public const string ICItem = "1";
            public const string ICStock = "2";
            public const string ICStockPlace = "3";
            public const string User = "4";
            public const string Supply = "5";
            public const string Customer = "6";
            public const string Dep = "7";
            //添加原单、
            public const string SourceBill_POORDER = "8";
            public const string SourceBill_PPBOM = "9";
            public const string SourceBill_SEORDER = "10";
        }

        public class MDFDatePickerDialog : DatePickerDialog
        {
            Android.Icu.Util.Calendar m_calendar = null;
            EditText m_editText = null; Context m_context = null;
            public MDFDatePickerDialog(Context context, EditText et) : base(context)
            {
                m_calendar = Android.Icu.Util.Calendar.GetInstance(new Locale("Prc"));
                m_editText = et;
                m_context = context;
                m_editText.Text = DateUtils.FormatDateTime(context, m_calendar.Time.GetDate(), FormatStyleFlags.Utc);
            }
            public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
            {
                m_calendar.Set(Android.Icu.Util.CalendarField.Year, year);
                m_calendar.Set(Android.Icu.Util.CalendarField.Month, month);
                m_calendar.Set(Android.Icu.Util.CalendarField.DayOfMonth, dayOfMonth);
                m_editText.Text = DateUtils.FormatDateTime(m_context, m_calendar.Time.GetDate(), FormatStyleFlags.Utc);
            }

        }

        public class DatePickerFragment : Android.Support.V4.App.DialogFragment,
                          Android.App.DatePickerDialog.IOnDateSetListener
        {
            // TAG can be any string of your choice.
            public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

            // Initialize this value to prevent NullReferenceExceptions.
            Action<DateTime> _dateSelectedHandler = delegate { };

            public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
            {
                DatePickerFragment frag = new DatePickerFragment();
                frag._dateSelectedHandler = onDateSelected;
                return frag;
            }

            public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime currently = DateTime.Now;
                Android.App.DatePickerDialog dialog = new Android.App.DatePickerDialog(
                                                               Activity,
                                                               this,
                                                               currently.Year,
                                                               currently.Month - 1,
                                                               currently.Day);
                return dialog;
            }

            public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
            {
                // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
                DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
                Log.Debug(TAG, selectedDate.ToLongDateString());
                _dateSelectedHandler(selectedDate);
            }
        }

        public class Stock_Header: Java.Lang.Object
        {
            public int m_FInterID { get; set; } = 0;
            public string m_FSupply { get; set; }
            public string m_FSupplyName { get; set; }
            public string m_FDate { set; get; }
            public string m_FNote { set; get; }
            public string m_FCustomer { set; get; }
            public string m_FCustomerName { set; get; }
            public string m_Fbillno { get; set; }
            public string m_Foperator { get; set; }
            public string m_FoperatorName { get; set; }
            public void Clear()
            {
                m_FInterID = 0;
                m_FSupply = "";
                m_FDate = "";
                m_FNote = "";
                m_FCustomer = "";
                m_Fbillno = "";
                m_Foperator = "";
                m_FoperatorName = "";
            }
        }
        public class Stock_Entry : Java.Lang.Object
        {
            public string m_fnumber_fitemid { get; set; }
            public string m_fnumber { get; set; }
            public string m_fnumber_name { get; set; }
            public string m_fnumber_model { get; set; }
            public string m_fstock_fitemid { get; set; }
            public string m_fstock { get; set; }
            public string m_fstock_name { get; set; }
            public string m_fstockplace_fitemid { get; set; }
            public string m_fstockplace { get; set; }
            public string m_fstockplace_name { get; set; }
            public string m_fuint_fitemid { get; set; }
            public string m_fuint { get; set; }
            public string m_fuint_name { get; set; }
            public string m_fqty { get; set; }
            public string m_fbatchno { get; set; }
            public string m_fnote { get; set; }
            public Guid m_uuid { get; set; }
            public int m_fsource_interid { get; set; } = 0;
            public int m_fsource_entryid { get; set; } = 0;
            public void Clear()
            {
                m_fnumber = "";
                m_fnumber_name = "";
                m_fnumber_model = "";
                m_fstock = "";
                m_fstock_name = "";
                m_fstockplace = "";
                m_fstockplace_name = "";
                m_fuint = "";
                m_fuint_name = "";
                m_fqty = "";
                m_fnote = "";
            }
        }
        public class UIDTag: Java.Lang.Object
        {
            public UIDTag(Guid guid)
            {
                m_UUID = guid;
            }
            public Guid m_UUID;
        }

        public class Entry_Adapter : BaseAdapter
        {

            public override int Count { get { return m_list.Count; } }
            List<Stock_Entry> m_list = null ; Context m_context = null;
            public Entry_Adapter(Context context, List<Stock_Entry> list)
            {
                m_context = context;
                m_list = list;
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return m_list[position];
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View groupkey, ViewGroup parent)
            {

                View view = groupkey;
            
                view = LayoutInflater.From(m_context).Inflate(Resource.Layout.activity_main_instock_entry_layout, null);

                var fnumber = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FNumber);
                var fstock = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FStock);
                var fstockplace = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FStockPlace);
                var fmainuint = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FMainUint);
                var fqty = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FQty);
                var fnote = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FNote);
                var button_save = view.FindViewById<Button>(Resource.Id.activity_main_instock_entry_layout_unknow);
                var button_edit = view.FindViewById<Button>(Resource.Id.activity_main_instock_entry_layout_edit);
                var button_delete = view.FindViewById<Button>(Resource.Id.activity_main_instock_entry_layout_delete);

                var fnumber_name = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FName);
                var fnumber_model = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FModelr);
                var fstock_name = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FStock_Name);
                var fstockplace_name = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FStockPlace_Name);
                var fmainuint_name = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_FMainUint_Name);
                var fbatchNo = view.FindViewById<TextView>(Resource.Id.activity_main_instock_entry_layout_fbatchNo);

                var uuid = Guid.NewGuid();
                m_list[position].m_uuid = uuid;
                button_delete.SetTag(Resource.Id.activity_main_instock_entry_layout_delete, new UIDTag(uuid));

                button_delete.Click += Button_delete_Click;
                if (fmainuint_name != null)
                {
                    fmainuint_name.Text = ((Stock_Entry)GetItem(position)).m_fuint_name;

                }
                if (fstockplace_name != null)
                {
                    fstockplace_name.Text = ((Stock_Entry)GetItem(position)).m_fstockplace_name;
                }
                if (fstock_name != null)
                {
                    fstock_name.Text = ((Stock_Entry)GetItem(position)).m_fstock_name;
                }
                if (fnumber_model != null)
                {
                    fnumber_model.Text = ((Stock_Entry)GetItem(position)).m_fnumber_model;
                }
                if (fnumber_name != null)
                {
                    fnumber_name.Text = ((Stock_Entry)GetItem(position)).m_fnumber_name;

                }
                if (fnumber != null)
                {
                    fnumber.Text = ((Stock_Entry)GetItem(position)).m_fnumber;
                }

                if (fstock != null)
                {
                    fstock.Text = ((Stock_Entry)GetItem(position)).m_fstock;
                }

                if (fstockplace != null)
                {
                    fstockplace.Text = ((Stock_Entry)GetItem(position)).m_fstockplace;
                }
                if (fmainuint != null)
                {
                    fmainuint.Text = ((Stock_Entry)GetItem(position)).m_fuint;
                }
                if (fqty != null)
                {
                    fqty.Text = ((Stock_Entry)GetItem(position)).m_fqty;
                }
                if (fnote != null)
                {
                    fnote.Text = ((Stock_Entry)GetItem(position)).m_fnote;
                }
                if (fbatchNo != null)
                {
                    fbatchNo.Text = ((Stock_Entry)GetItem(position)).m_fbatchno; 
                }
                return view;
            }
            //删除分录
            private void Button_delete_Click(object sender, EventArgs e)
            {
                m_list.RemoveAt(m_list.FindIndex(a => a.m_uuid == ((UIDTag)((Button)sender).GetTag(Resource.Id.activity_main_instock_entry_layout_delete)).m_UUID));
                //((Fragment_InStock)m_context).m_scroller.
                NotifyDataSetChanged();
            }
        }

        public class FBatch_Msg: Java.Lang.Object
        {
            public int m_id { get; set; }
            public string m_FBatchNo { get; set; }
            public string m_FQty { get; set; }
        }

        public class Batch_Adapter : BaseAdapter
        {
            public override int Count { get { return m_list.Count; } }
            List<FBatch_Msg> m_list = null; Context m_context = null;
            public Batch_Adapter(Context context, List<FBatch_Msg> list)
            {
                m_context = context;
                m_list = list;
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return m_list[position];
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;

                view = LayoutInflater.From(m_context).Inflate(Resource.Layout.dialog_entry_add_batch_list, null);
                var batchNo = view.FindViewById<TextView>(Resource.Id.dialog_entry_add_batch_list_batch);
                batchNo.Text = ((FBatch_Msg)GetItem(position)).m_FBatchNo;
                return view;
            }
        }
        //添加分录对话
        public class TypeEntry : Dialog
        {

            private Context m_context = null;
            //Android.Support.V4.App.Fragment m_fragment;
            Android.Support.V4.App.Fragment m_fragment;
            string m_ClassType = "";
            Button m_cancel = null, m_save = null;
            EditText m_fnumber = null, m_fstock = null, m_stockplace = null, m_mainuint = null, m_fqty = null, m_fnote = null, m_fqty_outStock = null,m_stock_fqty = null;
            EditText m_batchno = null,m_uncommitqty;
            bool StockPlace_Enable = false, BatchNo_Enable = false;
            Spinner m_batchSelector = null;
            EditText m_chooseBill = null;
            TextView m_stockplace_label = null, m_batch_label = null;
            //物料
            class _Item{
                public string fitemid { get; set; } = "";
                public string fnumber { get; set; } = "";
                public string fname { get; set; } = "";
                public string fmodel { get; set; } = "";
            }
            //仓库
            class _Stock
            {
                public string fitemid { get; set; } = "";
                public string fnumber { get; set; } = "";
                public string fname { get; set; } = "";
            }
            //仓位
            class _StockPlace
            {
                public string fnumber { get; set; } = "";
                public string fname { get; set; } = "";
                public string fitemid { get; set; } = "";
            }
            //单位
            class _Unit
            {
                public string fitemid { get; set; } = "";
                public string fnumber { get; set; } = "";
                public string fname { get; set; } = "";
            }
            //原单
            class _SourceBill
            {
                public int fsourceinterid { get; set; } = 0;
                public int fsourceentryid { get; set; } = 0;
                public decimal funcommitqty { get; set; } = 0.0M;
            }
            _Unit _m_unit = null;_StockPlace _m_stockplace = null;_Stock _m_stock = null;_Item _m_Item = null;_SourceBill _m_Sourcebill = null;
            MainActivity m_g_mainActivivty = null;//接收扫描头数据
            List<Source_Bill> m_SouBillList = new List<Source_Bill>();
            public TypeEntry(Context context, Android.Support.V4.App.Fragment fragment,string Type,MainActivity main) : base(context,Resource.Style.mdialog)
            {
                m_g_mainActivivty = main;
                m_g_mainActivivty.g_ProcessReciveData += M_g_mainActivivty_g_ProcessReciveData;
                m_context = context;
                m_fragment = fragment;
                m_ClassType = Type;
                if (Type == "IN")
                {
                    ((Fragment_InStock)fragment).inStock_FunRecivieData += Fragment_inStock_FunRecivieData;
                } else if (Type == "OUT")
                {
                    ((Fragment_OutStock)fragment).outStock_FunRecivieData += Fragment_inStock_FunRecivieData;
                } else if (Type == "XOUT") 
                {
                    ((Fragment_OutStockX)fragment).outStock_FunRecivieData += Fragment_inStock_FunRecivieData;
                }    
            }
            //处理物料选择
            private void M_g_mainActivivty_g_ProcessReciveData(string data)
            {
                if(data!=null && data != "")
                {
                    var ret = Tools_SQL_Class.getTable("select 1 from t_ICItem where FNumber='" + data + "'");
                    if(ret!=null && ret.Rows.Count > 0)
                    {
                        m_fnumber.Text = data;
                    }
                    else
                    {
                        ShowMsg(this.Context, "错误", "不存在该物料！");
                    }
                }
            }

            List<FBatch_Msg> m_batchNo_list = new List<FBatch_Msg>();
            //获取选择的物料的批号
            private void getBatchNo(string FItemID,string FStock,string FStockPlace)
            {
                if(FItemID!="" && FStock != "")
                {
                    var T = new System.Threading.Thread(new ThreadStart(() => {
                        if (!BatchNo_Enable /*&& StockPlace_Enable*/)
                        {
                            var _ret = Tools_SQL_Class.getTable(
                            "select cast(FQty as decimal(18,2)) FQty from ICInventory A " +
                            "join t_ICItem B on A.FItemID = B.FItemID " +
                            "join t_Stock C on A.FStockID = C.FItemID " +
                            "join t_StockPlace D on A.FStockPlaceID = D.FSPID " +
                            "where B.FNumber like '" + FItemID + "' and C.FNumber like '" + FStock + "' and D.FNumber like '" + FStockPlace + "' ");
                            if(_ret!=null && _ret.Rows.Count > 0)
                            {
                                m_fragment.Activity.RunOnUiThread(()=> {
                                    m_stock_fqty.Text = _ret.Rows[0]["FQty"].ToString();
                                });
                            }
                            else
                            {
                                m_fragment.Activity.RunOnUiThread(() => {
                                    m_stock_fqty.Text = "";
                                });
                            }
                            return;
                        };
                        //if (!BatchNo_Enable && !StockPlace_Enable)
                        //{
                        //    var _ret = Tools_SQL_Class.getTable(
                        //    "select FQty from ICInventory A " +
                        //    "join t_ICItem B on A.FItemID = B.FItemID " +
                        //    "join t_Stock C on A.FStockID = C.FItemID " +
                        //    "join t_StockPlace D on A.FStockPlaceID = D.FSPID " +
                        //    "where B.FNumber like '" + FItemID + "' and C.FNumber like '" + FStock + "';" );
                        //    return;
                        //};
                        //if (BatchNo_Enable && !StockPlace_Enable)
                        //{
                        //    var _ret = Tools_SQL_Class.getTable(
                        //    "select FBatchNo,FQty from ICInventory A " +
                        //    "join t_ICItem B on A.FItemID = B.FItemID " +
                        //    "join t_Stock C on A.FStockID = C.FItemID " +
                        //    "join t_StockPlace D on A.FStockPlaceID = D.FSPID " +
                        //    "where B.FNumber like '" + FItemID + "' and C.FNumber like '" + FStock + "' and D.FNumber like '" + FStockPlace + "' ");
                        //    return;
                        //};
                        if (BatchNo_Enable)
                        {
                            var ret = Tools_SQL_Class.getTable(
                            "select FBatchNo,cast(FQty as decimal(18,2)) FQty from ICInventory A " +
                            "join t_ICItem B on A.FItemID = B.FItemID " +
                            "join t_Stock C on A.FStockID = C.FItemID " +
                            "join t_StockPlace D on A.FStockPlaceID = D.FSPID " +
                            "where B.FNumber like '" + FItemID + "' and C.FNumber like '" + FStock + "' and D.FNumber like '" + FStockPlace + "' ");
                            if (ret != null && ret.Rows.Count > 0)
                            {
                                m_batchNo_list.Clear();
                                for (int i = 0; i < ret.Rows.Count; i++)
                                {
                                    var bat = new FBatch_Msg();
                                    bat.m_id = i;
                                    bat.m_FBatchNo = ret.Rows[i]["FBatchNo"].ToString();
                                    bat.m_FQty = ret.Rows[i]["FQty"].ToString();
                                    m_batchNo_list.Add(bat);
                                }
                                m_fragment.Activity.RunOnUiThread(()=> {
                                    m_batchSelector.Enabled = true;
                                    m_batchSelector.Adapter = new Batch_Adapter(m_context, m_batchNo_list);
                                });
                               
                            }
                            else
                            {
                                m_fragment.Activity.RunOnUiThread(() => {
                                    m_stock_fqty.Text = "";
                                });
                            }
                        };
                    }));
                    T.IsBackground = true;
                    T.Start();
                }
                
            }
            //获取仓库
            void getStock()
            {
                if (m_ClassType == "OUT")
                {
                    if (_m_Item!=null && _m_stock!=null&& _m_Item.fnumber != "" && _m_stock.fnumber != "")
                    {
                        if (StockPlace_Enable && _m_stockplace != null && _m_stockplace.fnumber != "")
                        {
                            getBatchNo(_m_Item.fnumber, _m_stock.fnumber, _m_stockplace.fnumber);
                        }
                        else
                        {
                            getBatchNo(_m_Item.fnumber, _m_stock.fnumber, "*");
                        }
                    }
                } 
            }
            //获取物料信息回调
            private void Fragment_inStock_FunRecivieData(string Type, string fnumber,string fname,string fextend,string fitemid)
            {
               switch (Type)
                {
                    case ItemType.ICItem:
                        {
                            m_fnumber.Text = fnumber;
                            _m_Item = new _Item();
                            _m_Item.fnumber = fnumber;
                            _m_Item.fname = fname;
                            _m_Item.fmodel = fextend;
                            _m_Item.fitemid = fitemid;
                            var ret = Tools_SQL_Class.getTable("select FNumber,FName,FItemID from t_Item where FItemClassID=7 and FItemID = (select top 1 FUnitID from t_ICItem where FNumber='" + fnumber + "')");
                            if(ret!=null && ret.Rows.Count > 0)
                            {
                                _m_unit = new _Unit();
                                _m_unit.fname = ret.Rows[0]["FName"].ToString();
                                _m_unit.fnumber = ret.Rows[0]["FNumber"].ToString();
                                _m_unit.fitemid = ret.Rows[0]["FItemID"].ToString();
                                m_mainuint.Text = ret.Rows[0]["FName"].ToString();
                            }
                            var _ret = Tools_SQL_Class.getTable("select 1 from t_ICItem where FBatchManager=1 and FNumber='" + fnumber + "'");
                            if (_ret != null && _ret.Rows.Count > 0)
                            {
                                //m_mainuint.Text = ret.Rows[0]["FName"].ToString();
                                m_batch_label.Visibility = ViewStates.Visible;
                                m_batchno.Visibility = ViewStates.Visible;
                                m_batchno.Enabled = true;
                                BatchNo_Enable = true;
                                m_batchSelector.Enabled = false;
                            }
                            else
                            {
                                m_batch_label.Visibility = ViewStates.Gone;
                                m_batchno.Visibility = ViewStates.Gone;
                                m_batchno.Text = "";
                                m_batchno.Enabled = false;
                                BatchNo_Enable = false;
                                m_batchSelector.Enabled = true;
                            }
                            getStock();
                        }
                        break;
                    case ItemType.ICStock:
                        {
                            m_fstock.Text = fname;
                            _m_stock = new _Stock();
                            _m_stock.fnumber = fnumber;
                            _m_stock.fname = fname;
                            _m_stock.fitemid = fitemid;
                            var ret = Tools_SQL_Class.getTable("select 1 from t_Stock where FNumber='" + fnumber + "' and FIsStockMgr=1");
                            if (ret != null && ret.Rows.Count > 0)
                            {
                                //m_stockplace.SetFocusable(ViewFocusability.Focusable);
                                //m_stockplace.RequestFocus();
                                m_stockplace_label.Visibility = ViewStates.Visible;
                                m_stockplace.Visibility = ViewStates.Visible;
                                m_stockplace.Enabled = true;
                                StockPlace_Enable = true;
                            }
                            else
                            {
                                //m_stockplace.SetFocusable(ViewFocusability.NotFocusable);
                                m_stockplace_label.Visibility = ViewStates.Gone;
                                m_stockplace.Visibility = ViewStates.Gone;
                                m_stockplace.Enabled = false;
                                StockPlace_Enable = false;
                               // m_stockplace.RequestFocus();
                            }
                            getStock();
                        }
                        break;
                    case ItemType.ICStockPlace:
                        {
                            _m_stockplace = new _StockPlace();
                            _m_stockplace.fnumber = fnumber;
                            _m_stockplace.fname = fname;
                            _m_stockplace.fitemid = fitemid;

                            m_stockplace.Text = fname;
                            getStock();
                        }
                        break;
                    case ItemType.SourceBill_POORDER:
                        {
                            _m_Sourcebill = new _SourceBill();
                            _m_Sourcebill.fsourceinterid = Convert.ToInt32(fnumber);
                            _m_Sourcebill.fsourceentryid = Convert.ToInt32(fname);
                            m_chooseBill.Text = fextend + " - " + fitemid;
                            var t = new Thread(()=> {
                                var ret = Tools_SQL_Class.getTable("select str(B.FQty-B.FCommitQty,len(B.FQty-B.FCommitQty),C.FQtyDecimal) FQty from POOrder A join POOrderEntry B on A.FInterID=B.FInterID join t_ICItem C on B.FItemID=C.FItemID where A.FInterID=" + fnumber + " and B.FEntryID=" + fname);
                                if(ret!=null && ret.Rows.Count > 0)
                                {
                                    m_fragment.Activity.RunOnUiThread(()=> {
                                        var quantity = ret.Rows[0]["FQty"].ToString();
                                        _m_Sourcebill.funcommitqty = Convert.ToDecimal(quantity);
                                        m_uncommitqty.Text = quantity;
                                    });
                                }
                            }) { IsBackground=true};
                            t.Start();
                        }
                        break;
                    case ItemType.SourceBill_PPBOM:
                        {
                            _m_Sourcebill = new _SourceBill();
                            _m_Sourcebill.fsourceinterid = Convert.ToInt32(fnumber);
                            _m_Sourcebill.fsourceentryid = Convert.ToInt32(fname);
                            m_chooseBill.Text = fextend + " - " + fitemid;
                        }
                        break;
                    case ItemType.SourceBill_SEORDER:
                        {
                            _m_Sourcebill = new _SourceBill();
                            _m_Sourcebill.fsourceinterid = Convert.ToInt32(fnumber);
                            _m_Sourcebill.fsourceentryid = Convert.ToInt32(fname);
                            m_chooseBill.Text = fextend + " - " + fitemid;
                        }
                        break;
                    default:
                        break;
                }
            }

            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                LayoutInflater layoutInflater = LayoutInflater.From(m_context);
                View view = layoutInflater.Inflate(Resource.Layout.dialog_entry_add,null);
                SetContentView(view);
                m_fnumber = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_fnumber);
                m_fstock = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_fstock);
                m_stockplace = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_fstockplace);
                m_batchno = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_fbatchno);
                m_mainuint = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_mainuint);
                m_fqty = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_fqty);
                m_fnote = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_fnote);
                m_cancel = view.FindViewById<Button>(Resource.Id.dialog_entry_add_cancel);
                m_save = view.FindViewById<Button>(Resource.Id.dialog_entry_add_save);
                m_batchSelector = view.FindViewById<Spinner>(Resource.Id.dialog_entry_add_fbatchno_selector);
                m_batchSelector.ItemSelected += M_batchSelector_ItemSelected;
                m_uncommitqty = view.FindViewById<EditText>(Resource.Id.dialog_entry_source_bill_qty);

                m_fqty_outStock = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_fqty_outstock);
                m_fqty_outStock.AfterTextChanged += M_fqty_outStock_AfterTextChanged;
                m_stock_fqty = view.FindViewById<EditText>(Resource.Id.dialog_entry_add_stock_fqty);
                //添加选单
                m_chooseBill = view.FindViewById<EditText>(Resource.Id.dialog_entry_source_bill);
                m_chooseBill.Click += M_chooseBill_Click;

                m_stockplace_label = FindViewById<TextView>(Resource.Id.dialog_entry_add_fstockplace_label);
                m_batch_label = FindViewById<TextView>(Resource.Id.dialog_entry_add_fbatchno_label);

                if (m_ClassType == "IN")
                {
                    m_batchSelector.Visibility = ViewStates.Invisible;
                    m_fqty_outStock.Visibility = ViewStates.Invisible;
                    m_stock_fqty.Visibility = ViewStates.Invisible;
                }
                else if (m_ClassType == "OUT" || m_ClassType == "XOUT")
                {
                    m_batchno.Visibility = ViewStates.Invisible;
                    m_fqty.Visibility = ViewStates.Invisible;
                }
                m_cancel.Click += M_cancel_Click;
                m_save.Click += M_save_Click;
                m_fnumber.Click += M_fnumber_Click;
                m_fstock.Click += M_fstock_Click;
                m_stockplace.Click += M_stockplace_Click;
            }
            //处理原单选择
            private void M_chooseBill_Click(object sender, EventArgs e)
            {
                if (_m_Item == null || _m_Item.fitemid == "")
                {
                    ShowMsg(m_context, "错误", "您还没有选择物料！");
                    return;
                }
                var intent = new Intent(Application.Context, typeof(Activity_BillSelect_Class));
                if (m_ClassType == "IN")//外购入库
                {
                    ((Fragment_InStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.SourceBill_POORDER;
                    intent.PutExtra("Type", Tools_Tables_Adapter_Class.ItemType.SourceBill_POORDER);
                }
                else if (m_ClassType == "OUT")//销售出库
                {
                    ((Fragment_OutStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.SourceBill_SEORDER;
                    intent.PutExtra("Type", Tools_Tables_Adapter_Class.ItemType.SourceBill_SEORDER);
                }
                else if (m_ClassType == "XOUT") //领料单
                {
                    ((Fragment_OutStockX)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.SourceBill_PPBOM;
                    intent.PutExtra("Type", Tools_Tables_Adapter_Class.ItemType.SourceBill_PPBOM);
                }
                intent.PutExtra("FItemID",_m_Item.fitemid);
                m_fragment.StartActivityForResult(intent, 0);

                //Activity_BillSelect_Class.m_main = (MainActivity)m_g_mainActivivty;
                //Activity_BillSelect_Class.m_Type = Tools_Tables_Adapter_Class.SourceBillType.PPOREDER;
                //m_context.StartActivity(new Intent(Application.Context, typeof(Activity_BillSelect_Class)));
            }

            private void M_stockplace_Click(object sender, EventArgs e)
            {
                var intent = new Intent(Application.Context, typeof(Activity_ItemSelect_Class));
                if (m_ClassType == "IN")
                {
                    ((Fragment_InStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICStockPlace;
                }
                else if (m_ClassType == "OUT")
                {
                    ((Fragment_OutStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICStockPlace;
                }
                else if (m_ClassType == "XOUT")
                {
                    ((Fragment_OutStockX)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICStockPlace;
                }
                intent.PutExtra("Type", Tools_Tables_Adapter_Class.ItemType.ICStockPlace);
                m_fragment.StartActivityForResult(intent, 0);
            }

            private void M_fqty_outStock_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
            {
                //throw new NotImplementedException();
                try
                {
                    if(Convert.ToDecimal(m_stock_fqty.Text) < Convert.ToDecimal(((EditText)sender).Text))
                    {
                        ShowMsg(m_fragment.Activity, "错误", "出库数量不能大于库存数量");
                        ((EditText)sender).Text = "";
                    }
                }
                catch
                {

                }
            }

            private void M_batchSelector_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
            {
                try
                {
                    m_stock_fqty.Text = m_batchNo_list[(int)((Spinner)sender).SelectedItemId].m_FQty;
                }catch(Exception ex)
                {
                    ShowMsg(m_context,"错误",ex.Message);
                }
            }

            private void M_fstock_Click(object sender, EventArgs e)
            {
                var intent = new Intent(Application.Context, typeof(Activity_ItemSelect_Class));
                if(m_ClassType == "IN")
                {
                    ((Fragment_InStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICStock;
                }
                else if (m_ClassType == "OUT")
                {
                    ((Fragment_OutStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICStock;
                }
                else if (m_ClassType == "XOUT")
                {
                    ((Fragment_OutStockX)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICStock;
                }
                intent.PutExtra("Type", Tools_Tables_Adapter_Class.ItemType.ICStock);
                m_fragment.StartActivityForResult(intent, 0);
                //如果仓库启用了仓位

            }

            private void M_fnumber_Click(object sender, EventArgs e)
            {
                var intent = new Intent(Application.Context, typeof(Activity_ItemSelect_Class));
                if (m_ClassType == "IN")
                {
                    ((Fragment_InStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICItem;
                }
                else if(m_ClassType == "OUT")
                {
                    ((Fragment_OutStock)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICItem;
                } else if(m_ClassType == "XOUT")
                {
                    ((Fragment_OutStockX)m_fragment).m_currentType = Tools_Tables_Adapter_Class.ItemType.ICItem;
                }
                //m_fragment.m_currentType = Tools_Tables_Adapter_Class.ItemType.ICItem;
                intent.PutExtra("Type", Tools_Tables_Adapter_Class.ItemType.ICItem);
                m_fragment.StartActivityForResult(intent, 0);
            }

            private void M_save_Click(object sender, EventArgs e)
            {
                //throw new NotImplementedException();
                var intent = new Intent(Application.Context, typeof(Activity_ItemSelect_Class));
                if (m_ClassType == "IN")
                {
                    ((Fragment_InStock)m_fragment).m_itemlist.Adapter = null;
                }
                else if (m_ClassType == "OUT")
                {
                    ((Fragment_OutStock)m_fragment).m_itemlist.Adapter = null;
                }
                else if (m_ClassType == "XOUT")
                {
                    ((Fragment_OutStockX)m_fragment).m_itemlist.Adapter = null;
                }
                //m_fragment.m_itemlist.Adapter = null;
                var entry = new Stock_Entry();
                if(_m_Item != null)
                {
                    entry.m_fnumber = _m_Item.fnumber;
                    entry.m_fnumber_name = _m_Item.fname;
                    entry.m_fnumber_model = _m_Item.fmodel;
                    entry.m_fnumber_fitemid = _m_Item.fitemid;
                }
                if (_m_stock != null)
                {
                    entry.m_fstock = _m_stock.fnumber;
                    entry.m_fstock_name = _m_stock.fname;
                    entry.m_fstock_fitemid = _m_stock.fitemid;
                }
                if (_m_stockplace != null)
                {
                    entry.m_fstockplace = _m_stockplace.fnumber;
                    entry.m_fstockplace_name = _m_stockplace.fname;
                    entry.m_fstockplace_fitemid = _m_stockplace.fitemid;
                }
                if (_m_unit != null)
                {
                    entry.m_fuint = _m_unit.fnumber;
                    entry.m_fuint_name = _m_unit.fname;
                    entry.m_fuint_fitemid = _m_unit.fitemid;
                }
                if (_m_Sourcebill != null)
                {
                    entry.m_fsource_interid = _m_Sourcebill.fsourceinterid;
                    entry.m_fsource_entryid = _m_Sourcebill.fsourceentryid;
                }
               
                if(m_ClassType == "IN")
                {
                    entry.m_fqty = m_fqty.Text;
                    entry.m_fbatchno = m_batchno.Text;
                }
                else if (m_ClassType == "OUT" || m_ClassType == "XOUT")
                {
                    entry.m_fqty = m_fqty_outStock.Text;
                    entry.m_fbatchno = m_batchNo_list[((int)m_batchSelector.SelectedItemId)].m_FBatchNo;
                }
                
                entry.m_fnote = m_fnote.Text;
                if(entry.m_fnumber == "")
                {
                    ShowMsg(m_fragment.Activity, "错误", "您的物料编码没有选择！");
                    return;
                }
                if (entry.m_fstock == "")
                {
                    ShowMsg(m_fragment.Activity, "错误", "您的仓库没有选择！");
                    return;
                }
                if (entry.m_fqty == "")
                {
                    ShowMsg(m_fragment.Activity, "错误", "您还没有填写数量！");
                    return;
                }
                
                if(_m_Sourcebill != null && _m_Sourcebill.funcommitqty != 0 && _m_Sourcebill.funcommitqty < Convert.ToDecimal(entry.m_fqty))
                {
                    ShowMsg(m_context, "警告", "入库数量不得大于未提交数量！");
                    return;
                }

                if (StockPlace_Enable)
                {
                    if (entry.m_fstockplace == "")
                    {
                        ShowMsg(m_fragment.Activity, "错误", "您还没有填写仓位！");
                        return;
                    }
                }
                if (BatchNo_Enable)
                {
                    if(entry.m_fbatchno == "")
                    {
                        ShowMsg(m_fragment.Activity, "错误", "您还没有填写批号！");
                        return;
                    }
                }

                if (m_ClassType == "IN")
                {
                    ((Fragment_InStock)m_fragment).m_EntryList_list.Add(entry);
                    var ada = new Entry_Adapter(m_fragment.Activity, ((Fragment_InStock)m_fragment).m_EntryList_list);
                    ((Fragment_InStock)m_fragment).m_itemlist.Adapter = ada;
                    getListViewHeigth(((Fragment_InStock)m_fragment).m_itemlist);
                }
                else if(m_ClassType == "OUT")
                {
                    ((Fragment_OutStock)m_fragment).m_EntryList_list.Add(entry);
                    var ada = new Entry_Adapter(m_fragment.Activity, ((Fragment_OutStock)m_fragment).m_EntryList_list);
                    ((Fragment_OutStock)m_fragment).m_itemlist.Adapter = ada;
                    getListViewHeigth(((Fragment_OutStock)m_fragment).m_itemlist);
                }else if(m_ClassType == "XOUT")
                {
                    ((Fragment_OutStockX)m_fragment).m_EntryList_list.Add(entry);
                    var ada = new Entry_Adapter(m_fragment.Activity, ((Fragment_OutStockX)m_fragment).m_EntryList_list);
                    ((Fragment_OutStockX)m_fragment).m_itemlist.Adapter = ada;
                    getListViewHeigth(((Fragment_OutStockX)m_fragment).m_itemlist);
                }
                //m_fragment.m_EntryList_list.Add(entry);
                //var ada = new Entry_Adapter(m_fragment.Activity, m_fragment.m_EntryList_list);
                //m_fragment.m_itemlist.Adapter = ada;
                //getListViewHeigth(m_fragment.m_itemlist);
                m_g_mainActivivty.g_ProcessReciveData -= M_g_mainActivivty_g_ProcessReciveData;
                this.Dismiss();
            }
            private void getListViewHeigth(ListView listview)
            {
                var adapter = listview.Adapter;
                if (adapter == null) return;
                int totalHeight = 0;
                for(int i = 0; i < adapter.Count; i++)
                {
                    View listItem = adapter.GetView(i, null, listview);
                    listItem.Measure(0, 0);
                    totalHeight += listItem.MeasuredHeight;
                }
                ViewGroup.LayoutParams _params = listview.LayoutParameters;
                _params.Height = totalHeight + (listview.DividerHeight * adapter.Count - 1);
                //listview.SetLayoutParams(_params);
                listview.LayoutParameters = _params;
            }

            private void M_cancel_Click(object sender, EventArgs e)
            {
                m_g_mainActivivty.g_ProcessReciveData -= M_g_mainActivivty_g_ProcessReciveData;
                this.Dismiss();
            }
        }


        public static Bitmap textAsBitmap(System.String text, float textSize, Color textColor)
        {
            Paint paint = new Paint(PaintFlags.AntiAlias);
            paint.TextSize = textSize;
            //paint.SetTextSize(textSize);
            //paint.SetColor(textColor);
            paint.Color = textColor;
            paint.TextAlign = Paint.Align.Left;
            float baseline = -paint.Ascent(); // ascent() is negative
            int width = (int)(paint.MeasureText(text) + 0.0f); // round
            int height = (int)(baseline + paint.Descent() + 0.0f);
            Bitmap image = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(image);
            canvas.DrawText(text, 0, baseline, paint);
            return image;
        }



        public class ShowPrograss: Dialog
        {
            Context m_context = null;
            //VideoView m_videoView = null;
            public ShowPrograss(Context context):base(context, Resource.Style.mdialog)
            {
                m_context = context;
            }
            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                LayoutInflater layoutInflater = LayoutInflater.From(m_context);
                View view = layoutInflater.Inflate(Resource.Layout.dialog_Prograss_layout, null);
                SetContentView(view);
                //m_videoView = view.FindViewById<VideoView>(Resource.Id.dialog_Prograss_layout_videoView);
                //
                //string package = m_context.PackageName;
                //string path = (Element.Source as ResourceVideoSource).Path;
                //string uri = "android.resource://" + package + "/raw/" + Resource.Raw.wait;
                //m_videoView.SetVideoURI(Android.Net.Uri.Parse(uri));
                //m_videoView.Start();
                SetCancelable(false);
            }
        }


        public class Software_Version : Java.Lang.Object
        {
            public int Id { get; set; }
            public string m_Software_Version { get; set; }
            public Software_Version(int id,string label)
            {
                Id = id;
                m_Software_Version = label;
            }
        }
        public class Software_Version_List : BaseAdapter
        {
            private List<Software_Version> m_list;
            private Context context;
            //private int ResourceID = 0;
            public Software_Version_List(Context pContext, List<Software_Version> pList/*,int resourceId*/)
            {
                context = pContext;
                m_list = pList;
                //ResourceID = resourceId;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                LayoutInflater _LayoutInflater = LayoutInflater.From(context);
                convertView = _LayoutInflater.Inflate(Resource.Layout.activity_login_account_list_layout, null);
                if (convertView != null)
                {
                    TextView _TextView1 = (TextView)convertView.FindViewById<TextView>(Resource.Id.activity_login_account_list_layout_label);
                    _TextView1.Text = m_list.ElementAt<Software_Version>(position).m_Software_Version;

                }
                return convertView;
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return m_list.ElementAt(position);
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override int Count { get { return m_list.Count; } }
        }
        public class SourceStockBill : Java.Lang.Object
        {
            public string m_FBillNo = "";
            public int m_FInterID = 0;
            public int m_FEntryID = 0;
            public int m_Sup = 0;
            public int m_Dep = 0;
            public int m_Cust = 0;
            public int m_FitemID = 0;
            public decimal m_Qty = 0;
            public int m_Unit = 0;
            public string m_BatchNo = "";
            public int m_FStock = 0;
            public int m_FStockPlace = 0;
            public string m_FNote = "";
            public decimal m_CommitQty = 0;

        }

        public class Source_Bill : Java.Lang.Object
        {
            public Source_Bill() {
                m_uuid = UUID.RandomUUID();
            }
            public string FBillNo { get; set; }
            public int FInterID { get; set; }
            public int F_Dep_Cust_Sup { get; set; }
            public string F_Dep_Cust_Sup_Name { get; set; }
            public int FItemID { get; set; }
            public int FUnitID { get; set; }
            public string FUnitID_FName { get; set; }
            public string FItemID_FNumber { get; set; }
            public string FItemID_FName { get; set; }
            public string FItemID_FModel { get; set; }
            public int FEntryID { get; set; }
            public decimal FQty { get; set; }
            public decimal FCommitQty { get; set; }
            public UUID m_uuid { get; private set; }
        }

        public class SourceBillViewHolder : RecyclerView.ViewHolder
        {
            public TextView m_FBillNo = null, m_Import = null, m_FName = null, m_FNumber = null, m_FModel = null, m_FQty = null, m_FCommitQty = null;
            public SourceBillViewHolder(View view) : base(view)
            {
                m_FBillNo = view.FindViewById<TextView>(Resource.Id.sourceselect_fbillno);
                m_Import = view.FindViewById<TextView>(Resource.Id.sourceselect_important);
                m_FName = view.FindViewById<TextView>(Resource.Id.sourceselect_fname);
                m_FNumber = view.FindViewById<TextView>(Resource.Id.sourceselect_fnumber);
                m_FModel = view.FindViewById<TextView>(Resource.Id.sourceselect_fmodel);
                m_FQty = view.FindViewById<TextView>(Resource.Id.sourceselect_fqty);
                m_FCommitQty = view.FindViewById<TextView>(Resource.Id.sourceselect_fcommitqty);
            }
        }

        public class SourceBillListAdapter : BaseAdapter
        {
            private List<Source_Bill> m_list;
            private Context m_context = null;
            public delegate void ClickCallBack(string data);
            public event ClickCallBack __ClickCallBack;

            //private int ResourceID = 0;
            public SourceBillListAdapter(Context pContext, List<Source_Bill> pList)
            {
                m_context = pContext;
                m_list = pList;
            }
            //public override int ItemCount { get { return m_list.Count; } }

            //public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            //{
            //    SourceBillViewHolder sbvh = holder as SourceBillViewHolder;
            //    sbvh.m_FBillNo.Text = m_list.ElementAt<Source_Bill>(position).FBillNo;
            //    sbvh.m_FCommitQty.Text = m_list.ElementAt<Source_Bill>(position).FCommitQty.ToString();
            //    sbvh.m_FModel.Text = m_list.ElementAt<Source_Bill>(position).FItemID_FModel;
            //    sbvh.m_FName.Text = m_list.ElementAt<Source_Bill>(position).FItemID_FName;
            //    sbvh.m_FNumber.Text = m_list.ElementAt<Source_Bill>(position).FItemID_FNumber;
            //    sbvh.m_FQty.Text = m_list.ElementAt<Source_Bill>(position).FQty.ToString();
            //    sbvh.m_Import.Text = m_list.ElementAt<Source_Bill>(position).F_Dep_Cust_Sup_Name.ToString();

            //}

            //public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            //{
            //    View v = LayoutInflater.FromContext(m_context).Inflate(Resource.Layout.activity_sourcebillselect_entry, parent, false);
            //    return new SourceBillViewHolder(v);// new ViewHolderM(v); 
            //}


            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                LayoutInflater _LayoutInflater = LayoutInflater.From(m_context);
                convertView = _LayoutInflater.Inflate(Resource.Layout.activity_sourcebillselect_entry, null);
                if (convertView != null)
                {
                    TextView fbillno = (TextView)convertView.FindViewById<TextView>(Resource.Id.sourceselect_fbillno);
                    TextView important = (TextView)convertView.FindViewById<TextView>(Resource.Id.sourceselect_important);
                    TextView fnumber = (TextView)convertView.FindViewById<TextView>(Resource.Id.sourceselect_fnumber);
                    TextView fname = (TextView)convertView.FindViewById<TextView>(Resource.Id.sourceselect_fname);
                    TextView fmodel = (TextView)convertView.FindViewById<TextView>(Resource.Id.sourceselect_fmodel);
                    TextView fqty = (TextView)convertView.FindViewById<TextView>(Resource.Id.sourceselect_fqty);
                    TextView fcommitqty = (TextView)convertView.FindViewById<TextView>(Resource.Id.sourceselect_fcommitqty);
                    Button selectbill = (Button)convertView.FindViewById<Button>(Resource.Id.sourceselect_select);
                    
                    selectbill.Click += Selectbill_Click;
                    //selectbill.Tag = m_list.ElementAt<Source_Bill>(position).m_uuid.ToString() + "|" + m_list.ElementAt<Source_Bill>(position).FEntryID.ToString();
                    selectbill.Tag = m_list.ElementAt<Source_Bill>(position).m_uuid.ToString();
                    fbillno.Text = m_list.ElementAt<Source_Bill>(position).FBillNo;
                    important.Text = m_list.ElementAt<Source_Bill>(position).F_Dep_Cust_Sup_Name;
                    fnumber.Text = m_list.ElementAt<Source_Bill>(position).FItemID_FNumber;
                    fname.Text = m_list.ElementAt<Source_Bill>(position).FItemID_FName;
                    fmodel.Text = m_list.ElementAt<Source_Bill>(position).FItemID_FModel;
                    fqty.Text = m_list.ElementAt<Source_Bill>(position).FQty.ToString();
                    fcommitqty.Text = m_list.ElementAt<Source_Bill>(position).FCommitQty.ToString();
                }
                return convertView;
            }

            private void Selectbill_Click(object sender, EventArgs e)
            {
                if (__ClickCallBack != null)
                {
                    __ClickCallBack(((Button)sender).Tag.ToString());
                }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return m_list.ElementAt(position);
            }
            public override long GetItemId(int position)
            {
                return position;
            }

            //public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            //{
            //    throw new NotImplementedException();
            //}

            //public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            //{
            //    throw new NotImplementedException();
            //}

            public override int Count { get { return m_list.Count; } }

            //public override int ItemCount { get { return m_list.Count; } }
        }
        //原单类型
        public class  SourceBillType{
            public const string PPOREDER = "PPOREDER";         //采购订单
            public const string PPBOM = "PPBOM";               //领料单
            public const string SEORDER = "SEORDER";           //销售订单

        }
        //not end
    }
}