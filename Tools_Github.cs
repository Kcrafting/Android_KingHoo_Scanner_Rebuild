using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Nancy.Json;
using Newtonsoft.Json;

namespace Android_KingHoo_Scanner_Rebuild
{
    class Tools_Github
    {
        private static int m_loopTimes = 0;
        public class links
        {
            [JsonProperty("self")]
            public string self { get; set; }
            [JsonProperty("git")]
            public string git { get; set; }
            [JsonProperty("html")]
            public string html { get; set; }
        }
        public class ProjectDetail
        {
            [JsonProperty("name")]
            public string name { get; set; }
            [JsonProperty("path")]
            public string path { get; set; }
            [JsonProperty("sha")]
            public string sha { get; set; }
            [JsonProperty("size")]
            public int size { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
            [JsonProperty("html_url")]
            public string html_url { get; set; }
            [JsonProperty("git_url")]
            public string git_url { get; set; }
            [JsonProperty("download_url")]
            public string download_url { get; set; }
            [JsonProperty("type")]
            public string type { get; set; }
            [JsonProperty("_links")]
            public links _links { get; set; }
        }

        public class Notice
        {
            [JsonProperty("title")]
            public string title { get; set; }
            [JsonProperty("message")]
            public string message { get; set; }
        }
        public class Description
        {
            [JsonProperty("project")]
            public string project { get; set; }
            [JsonProperty("date")]
            public string date { get; set; }
            [JsonProperty("update")]
            public string update { get; set; }
        }
        public class CertifiedComputer
        {
            [JsonProperty("uuid")]
            public string uuid { get; set; }
            [JsonProperty("periodofservice")]
            public string periodofservice { get; set; }
            [JsonProperty("periodofuse")]
            public string periodofuse { get; set; }
            [JsonProperty("imei")]
            public string imei { get; set; }
        }
        static HttpClient client = null;
        public static async Task GetServerMsg()
        {
            while (true)
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client = new HttpClient();
                    //client.BaseAddress = new Uri("https://api.github.com");
                    client.DefaultRequestHeaders.Add("User-Agent", "authproject");
                    client.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var url = "https://api.github.com/repos/Kcrafting/Projects/contents/" + "cangzhouruijiezhixiangjixieyouxiangongsi";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var resp = await response.Content.ReadAsStringAsync();

                    string cert = "";
                    string desc = "";
                    string noti = "";
                    var Pro = JsonConvert.DeserializeObject<List<ProjectDetail>>(resp);

                    foreach(var i in Pro)
                    {
                        if (i.name == "CertifiedComputer.json")
                        {
                            HttpResponseMessage response_cer = await client.GetAsync(i.download_url);
                            response_cer.EnsureSuccessStatusCode();
                            cert = await response_cer.Content.ReadAsStringAsync();
                        }
                        else if(i.name == "Description.json")
                        {
                            HttpResponseMessage response_des = await client.GetAsync(i.download_url);
                            response_des.EnsureSuccessStatusCode();
                            desc = await response_des.Content.ReadAsStringAsync();
                        }
                        else if(i.name == "Notice.json")
                        {
                            HttpResponseMessage response_not = await client.GetAsync(i.download_url);
                            response_not.EnsureSuccessStatusCode();
                            noti = await response_not.Content.ReadAsStringAsync();
                        }
                    }
                    List < CertifiedComputer> t_com = null;
                    Description t_des = null;
                    Notice t_noc = null;
                    if (cert!="")
                    {
                        t_com = JsonConvert.DeserializeObject<List<CertifiedComputer>>(cert);
                    }
                    if (desc != "")
                    {
                        t_des = JsonConvert.DeserializeObject<Description>(desc);
                    }
                    if (noti != "")
                    {
                        t_noc = JsonConvert.DeserializeObject<Notice>(noti);
                    }

                    MainActivity.u_title = t_noc.title==""?"": t_noc.title;
                    MainActivity.u_message = t_noc.message == "" ? "" : t_noc.message;
                   
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //Thread.Sleep(3000);
                    return;
                    m_loopTimes++;
                    if (m_loopTimes >= 10)
                    {
                        break;
                    }
                }
            }
        }
        public static async Task AuthDevice(string projectName,string sn, AppCompatActivity act, TextView tv, Tools_Tables_Adapter_Class.ShowPrograss pro)
        {
            //while (true)
            {
                try
                {
                    var tes = new Tools_Extend_Storage(act);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client = new HttpClient();
                    //client.BaseAddress = new Uri("https://api.github.com");
                    client.DefaultRequestHeaders.Add("User-Agent", "authproject");
                    client.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    if (tes.getValueString(Tools_Extend_Storage.ValueType.CertifiedAuthPath) == "")
                    {
                        var urlTxt = "https://api.github.com/repos/Kcrafting/Projects/contents/" + projectName;
                        var url = urlTxt;
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        var ProjectDirMsg = await response.Content.ReadAsStringAsync();
                        var projectDetails = JsonConvert.DeserializeObject<List<ProjectDetail>>(ProjectDirMsg);
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedAuthPath, projectDetails.Find(a => a.name == "CertifiedComputer.json").download_url);
                        //var authUrl = projectDetails.Find(a => a.name == "CertifiedComputer.json").download_url;
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedNoticePath, projectDetails.Find(a => a.name == "Notice.json").download_url);
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedDescriptPath, projectDetails.Find(a => a.name == "Description.json").download_url);
                    }
                    var AuthPath = tes.getValueString(Tools_Extend_Storage.ValueType.CertifiedAuthPath);
                    if (AuthPath == "")
                    {
                        return;
                    }
                    HttpResponseMessage authresp = await client.GetAsync(AuthPath);
                    authresp.EnsureSuccessStatusCode();
                    var authTxt = await authresp.Content.ReadAsStringAsync();
                    var authDevicList = JsonConvert.DeserializeObject<List<CertifiedComputer>>(authTxt);
                    if (authDevicList.Exists(item => item.imei == sn))
                    {
                       
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedUUID, authDevicList.Find(item => item.imei == sn).uuid);
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedDevice, true);
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedServiceDate, authDevicList.Find(item => item.imei == sn).periodofservice);
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedUseDate, authDevicList.Find(item => item.imei == sn).periodofuse);
                        tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedFinish, true);
                        act.RunOnUiThread(()=> {
                            pro.Dismiss();
                            tv.Text = "激活成功！";
                            Tools_Tables_Adapter_Class.ShowMsg(act, "完成！", "已经成功激活！");
                        });
                       
                    }
                    else
                    {
                        act.RunOnUiThread(() =>
                        {
                           
                            Tools_Tables_Adapter_Class.ShowMsg(act, "错误！", "没有找到授权许可!");
                            act.RunOnUiThread(() => {
                                tv.Text = "没有找到授权许可！";
                                pro.Dismiss();
                            });
                        });
                        return;
                    }
                }
                catch(Exception ex)
                {
                    act.RunOnUiThread(()=> {
                        Tools_Tables_Adapter_Class.ShowMsg(act, "错误！", "激活失败！" + ex.Message);
                        act.RunOnUiThread(() => {
                            pro.Dismiss();
                            tv.Text = "激活失败！";
                        });
                    });
                    return;
                    //Thread.Sleep(3000);
                }
            }
        }


        public static async Task getStartingMsg(string projectName, string sn, Context intet)
        {
            //while (true)
            //{
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client = new HttpClient();
                    //client.BaseAddress = new Uri("https://api.github.com");
                    client.DefaultRequestHeaders.Add("User-Agent", "authproject");
                    client.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var url = "https://api.github.com/repos/Kcrafting/Projects/contents/" + projectName;
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var ProjectDirMsg = await response.Content.ReadAsStringAsync();
                    var projectDetails = JsonConvert.DeserializeObject<List<ProjectDetail>>(ProjectDirMsg);

                    var noticeUrl = projectDetails.Find(a => a.name == "CertifiedComputer.json").download_url;
                    HttpResponseMessage noticeresp = await client.GetAsync(noticeUrl);
                    noticeresp.EnsureSuccessStatusCode();
                    var noticeTxt = await noticeresp.Content.ReadAsStringAsync();
                    var notice = JsonConvert.DeserializeObject<Notice>(noticeTxt);
                    MainActivity.u_title = notice.title == "" ? "" : notice.title;
                    MainActivity.u_message = notice.message == "" ? "" : notice.message;

                    var desc = projectDetails.Find(a => a.name == "Description.json").download_url;
                    HttpResponseMessage descresp = await client.GetAsync(desc);
                    descresp.EnsureSuccessStatusCode();
                    var descTxt = await descresp.Content.ReadAsStringAsync();
                    var des = JsonConvert.DeserializeObject<Description>(descTxt);
                    if (des.update == "yes")
                    {
                        var authUrl = projectDetails.Find(a => a.name == "CertifiedComputer.json").download_url;
                        HttpResponseMessage authresp = await client.GetAsync(url);
                        authresp.EnsureSuccessStatusCode();
                        var authTxt = await authresp.Content.ReadAsStringAsync();
                        var authDevicList = JsonConvert.DeserializeObject<List<CertifiedComputer>>(authTxt);
                        if (authDevicList.Exists(item => item.imei == sn))
                        {
                            var tes = new Tools_Extend_Storage(intet);
                            tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedUUID, authDevicList.Find(item => item.imei == sn).uuid);
                            tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedDevice, true);
                            tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedServiceDate, authDevicList.Find(item => item.imei == sn).periodofservice);
                            tes.saveValue(Tools_Extend_Storage.ValueType.CertifiedUseDate, authDevicList.Find(item => item.imei == sn).periodofuse);

                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    m_loopTimes++;
                    if (m_loopTimes >= 10)
                    {
                        return;
                    }
                }
            //}
        }

        public static async Task greetingMsg(AppCompatActivity Act)
        {
            //while (true)
            //{
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client = new HttpClient();
                //client.BaseAddress = new Uri("https://api.github.com");
                client.DefaultRequestHeaders.Add("User-Agent", "authproject");
                client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var tes = new Tools_Extend_Storage(Act);

                var url = tes.getValueString(Tools_Extend_Storage.ValueType.CertifiedNoticePath);

                if (url != "")
                {
                    HttpResponseMessage rpe = await client.GetAsync(url);
                    rpe.EnsureSuccessStatusCode();
                    var _start_Msg = await rpe.Content.ReadAsStringAsync();
                    var smObj = JsonConvert.DeserializeObject<Notice>(_start_Msg);
                    if (smObj != null)
                    {
                        Act.RunOnUiThread(() => {
                            Tools_Tables_Adapter_Class.ShowMsg(Act, smObj.title, smObj.message);
                    });
                    }
                    
                }            
            }
            catch 
            {

            }
            //}
        }
    }
}