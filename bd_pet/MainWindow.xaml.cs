using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using bd_pet.ViewModels;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace bd_pet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static object lockObj = new object();
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private string cookie = "";
        /// <summary>
        /// 卖出多线程
        /// </summary>
        private Thread threadSale;
        /// <summary>
        /// 买入多线程
        /// </summary>
        private Thread threadBuy;
        /// <summary>
        /// 买入多线程列表
        /// </summary>
        private List<Thread> threadBuys = new List<Thread>();
        /// <summary>
        /// 忽略异常pedId
        /// </summary>
        private List<string> listPetId = new List<string>();
        private MainVM mainVM = new MainVM();
        private RSACryptoService rsa;
        public MainWindow()
        {
            InitializeComponent();
            //headers.Add("Accept", "application/json");
            headers.Add("Accept-Encoding", "gzip, deflate, br");
            headers.Add("Accept-Language", "zh-CN,zh-TW;q=0.9,zh;q=0.8,en-US;q=0.7,en;q=0.6");
            //headers.Add("Connection", "keep-alive");
            //headers.Add("Cookie", "");
            //headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36");
            this.DataContext = mainVM;
            mainVM.LoadData();
            txtPassword.Password = mainVM.Password;
            rsa = new RSACryptoService(null, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA6TnX1giecxk83kHZexmemlJJ7yLtUJYqPPbX4GTCjRKSU/UuWf2B62sD3O4OBpL8qFP78hpMq6FguuMgA8iD8tjska6DyBZLMV80G+XNtOEut0yIHSFIVGxkUKuP6NrrBHyA5hdTYrnun+N/blGXvPSd/FdtjFSYgXyVvxcGKs3kgk0E2Unc/uNWJTW8Em9gZk4zwG5aw+HbTHbTcoserVIZ2MliDvjRIB6dS/qplKVlLbEMGOOqo+XMujJZKddVimPVS2eEYKRmZzEtcRDTmQFuLBi8LkyysjexfHZuMMLcPTNus+suxnaTOyZL/QxGQDgJsfj3Ux7ABAvBovcZkwIDAQAB");
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.ToString() == "启动")
            {
                pnlBuy.IsEnabled = false;
                pnlSale.IsEnabled = false;
                pnlSetting.IsEnabled = false;
                mainVM.SaveData();
                btnStart.Content = "停止";
                cookie = mainVM.Cookie;
                if (mainVM.IsSale)
                {
                    #region 卖出
                    threadSale = new Thread(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                int page = 1;
                                int totalCount = 100000000;
                                while ((page - 1) * 10 < totalCount)
                                {
                                    var jsonStr = GetSaleList(page);
                                    if (String.IsNullOrEmpty(jsonStr))
                                        continue;
                                    var jsonData = JObject.Parse(jsonStr);
                                    if (jsonData["errorNo"].Value<String>() == "00")
                                    {
                                        totalCount = jsonData["data"]["totalCount"].Value<int>();
                                        var userItemVMList = jsonData["data"]["dataList"].Values<JObject>();
                                        foreach (var tmpUserItemVM in userItemVMList)
                                        {
                                            if (tmpUserItemVM["shelfStatus"].Value<int>() == 0)
                                            {
                                                string result = "";
                                                switch (tmpUserItemVM["rareDegree"].Value<int>())
                                                {
                                                    case 0:
                                                        result = Sale(tmpUserItemVM["petId"].Value<string>(), mainVM.CommonSale.ToString());
                                                        break;
                                                    case 1:
                                                        result = Sale(tmpUserItemVM["petId"].Value<string>(), mainVM.RareSale.ToString());
                                                        break;
                                                    case 2:
                                                        result = Sale(tmpUserItemVM["petId"].Value<string>(), mainVM.ExcellenceSale.ToString());
                                                        break;
                                                    case 3:
                                                        result = Sale(tmpUserItemVM["petId"].Value<string>(), mainVM.EpicSale.ToString());
                                                        break;
                                                    case 4:
                                                        result = Sale(tmpUserItemVM["petId"].Value<string>(), mainVM.MythicalSale.ToString());
                                                        break;
                                                    case 5:
                                                        result = Sale(tmpUserItemVM["petId"].Value<string>(), mainVM.LegendSale.ToString());
                                                        break;
                                                }
                                                this.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "卖出:" + result });
                                                }));
                                            }
                                            Thread.Sleep(500);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    page++;
                                    Thread.Sleep(1000);
                                }
                                Thread.Sleep(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "错误:" + ex.ToString() });
                            }));
                        }
                    });
                    threadSale.IsBackground = true;
                    threadSale.Start();
                    #endregion
                }
                if (mainVM.IsBuy)
                {
                    #region 买入
                    threadBuy = new Thread(() =>
                {
                    try
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Thread threadBuyOne = new Thread((obj) =>
                            {
                                int rareDegree = (int)obj;
                                while (true)
                                {
                                    try
                                    {
                                        for (int i2 = 1; i2 <= 10; i2++)
                                        {
                                            lock (lockObj)
                                            {
                                                bool canBuy = false;
                                                string jsonStr = GetBuyList(i2, rareDegree);
                                                if (String.IsNullOrEmpty(jsonStr))
                                                    continue;
                                                var jsonData = JObject.Parse(jsonStr);
                                                if (jsonData["errorNo"].Value<String>() == "00")
                                                {
                                                    var saleList = jsonData["data"]["petsOnSale"].Values<JObject>();
                                                    foreach (var item in saleList)
                                                    {
                                                        var petId = item["petId"].Value<string>();
                                                        if (listPetId.Exists(m => m == petId))
                                                            continue;
                                                        if (listPetId.Count > 1000)
                                                            listPetId.Remove(listPetId.First());
                                                        string result = "";
                                                        switch (item["rareDegree"].Value<int>())
                                                        {
                                                            case 0:
                                                                {
                                                                    if (mainVM.CommonBuy >= item["amount"].Value<double>())
                                                                    {
                                                                        canBuy = true;
                                                                        this.Dispatcher.Invoke(new Action(() =>
                                                                        {
                                                                            mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "尝试买入:" + item["petId"].Value<string>() + ",价格" + item["amount"].Value<string>() + ",稀有度:" + item["rareDegree"].Value<int>() });
                                                                        }));
                                                                        result = Buy(item["petId"].Value<string>(), item["amount"].Value<string>());
                                                                    }
                                                                }
                                                                break;
                                                            case 1:
                                                                {
                                                                    if (mainVM.RareBuy >= item["amount"].Value<double>())
                                                                    {
                                                                        canBuy = true;
                                                                        this.Dispatcher.Invoke(new Action(() =>
                                                                        {
                                                                            mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "尝试买入:" + item["petId"].Value<string>() + ",价格" + item["amount"].Value<string>() + ",稀有度:" + item["rareDegree"].Value<int>() });
                                                                        }));
                                                                        result = Buy(item["petId"].Value<string>(), item["amount"].Value<string>());
                                                                    }
                                                                }
                                                                break;
                                                            case 2:
                                                                {
                                                                    if (mainVM.ExcellenceBuy >= item["amount"].Value<double>())
                                                                    {
                                                                        canBuy = true;
                                                                        this.Dispatcher.Invoke(new Action(() =>
                                                                        {
                                                                            mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "尝试买入:" + item["petId"].Value<string>() + ",价格" + item["amount"].Value<string>() + ",稀有度:" + item["rareDegree"].Value<int>() });
                                                                        }));
                                                                        result = Buy(item["petId"].Value<string>(), item["amount"].Value<string>());
                                                                    }
                                                                }
                                                                break;
                                                            case 3:
                                                                {
                                                                    if (mainVM.EpicBuy >= item["amount"].Value<double>())
                                                                    {
                                                                        canBuy = true;
                                                                        this.Dispatcher.Invoke(new Action(() =>
                                                                        {
                                                                            mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "尝试买入:" + item["petId"].Value<string>() + ",价格" + item["amount"].Value<string>() + ",稀有度:" + item["rareDegree"].Value<int>() });
                                                                        }));
                                                                        result = Buy(item["petId"].Value<string>(), item["amount"].Value<string>());
                                                                    }
                                                                }
                                                                break;
                                                            case 4:
                                                                {
                                                                    if (mainVM.MythicalBuy >= item["amount"].Value<double>())
                                                                    {
                                                                        canBuy = true;
                                                                        if (!String.IsNullOrEmpty(result))
                                                                        {
                                                                            this.Dispatcher.Invoke(new Action(() =>
                                                                            {
                                                                                mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "尝试买入:" + item["petId"].Value<string>() + ",价格" + item["amount"].Value<string>() + ",稀有度:" + item["rareDegree"].Value<int>() });
                                                                            }));
                                                                        }
                                                                        result = Buy(item["petId"].Value<string>(), item["amount"].Value<string>());
                                                                    }
                                                                }
                                                                break;
                                                            case 5:
                                                                {
                                                                    if (mainVM.LegendBuy >= item["amount"].Value<double>())
                                                                    {
                                                                        canBuy = true;
                                                                        if (!String.IsNullOrEmpty(result))
                                                                        {
                                                                            this.Dispatcher.Invoke(new Action(() =>
                                                                            {
                                                                                mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "尝试买入:" + item["petId"].Value<string>() + ",价格" + item["amount"].Value<string>() + ",稀有度:" + item["rareDegree"].Value<int>() });
                                                                            }));
                                                                        }
                                                                        result = Buy(item["petId"].Value<string>(), item["amount"].Value<string>());
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                        if (!String.IsNullOrEmpty(result))
                                                        {
                                                            this.Dispatcher.Invoke(new Action(() =>
                                                            {
                                                                if (mainVM.LogMsg.Count > 1000)
                                                                    mainVM.LogMsg.Clear();
                                                                mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "买入:" + result });
                                                            }));
                                                        }
                                                    }
                                                }
                                                if (canBuy == false)
                                                {
                                                    break;
                                                }
                                                Thread.Sleep(300);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Dispatcher.Invoke(new Action(() =>
                                        {
                                            mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "错误:" + ex.ToString() });
                                        }));
                                    }
                                    Thread.Sleep(200);
                                }
                            });
                            threadBuyOne.IsBackground = true;
                            threadBuyOne.Start(i);
                            threadBuys.Add(threadBuyOne);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            mainVM.LogMsg.Insert(0, new LogMsgItem() { Message = "错误:" + ex.ToString() });
                        }));
                    }
                });
                    threadBuy.IsBackground = true;
                    threadBuy.Start();
                    #endregion
                }
            }
            else
            {
                pnlBuy.IsEnabled = true;
                pnlSale.IsEnabled = true;
                pnlSetting.IsEnabled = true;
                btnStart.Content = "启动";
                StopThread();
            }
        }
        /// <summary>
        /// 结束所有
        /// </summary>
        private void StopThread()
        {
            if (threadSale != null)
            {
                threadSale.Abort();
                threadSale = null;
            }
            if (threadBuy != null)
            {
                threadBuy.Abort();
                threadBuy = null;
                foreach (var threadBuyOne in threadBuys)
                {
                    threadBuyOne.Abort();
                }
                threadBuys.Clear();
            }
        }
        public string PostMoths(Uri url, string param, Dictionary<string, string> headers)
        {
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.Accept = "application/json";
            //request.Connection = "keep-alive";
            request.UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Pixel 2 XL Build/OPD1.170816.004) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Mobile Safari/537.36";
            request.Host = url.Host;
            SetHeaderValue(request.Headers, "Connection", "Keep-Alive");
            request.Headers.Add("Origin", url.OriginalString);
            request.Headers.Add("Cookie", cookie);
            if (headers != null)
                foreach (var i in headers)
                    request.Headers.Add(i.Key, i.Value);
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrData = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            StrData = Reader.ReadLine();
            Console.WriteLine(StrData);
            return StrData;
        }
        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
        /// <summary>
        /// 识别百度验证码
        /// </summary>
        /// <returns></returns>
        public string GetBaiduCode(string imgBase64)
        {
            try
            {
                return PostMoths(new Uri(mainVM.ApiUrl),
                     JsonConvert.SerializeObject(new
                     {
                         key = mainVM.ApiKey,
                         img = imgBase64
                     }), headers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }
        /// <summary>
        /// 查询自己背包列表
        /// </summary>
        /// <returns></returns>
        public string GetSaleList(int pageNo)
        {
            try
            {
                long requestId = DateTime.Now.Ticks / 42000;
                return PostMoths(new Uri("https://pet-chain.baidu.com/data/user/pet/list"),
                     JsonConvert.SerializeObject(new
                     {
                         pageNo = pageNo,
                         pageSize = 10,
                         pageTotal = -1,
                         totalCount = 0,
                         requestId = requestId,
                         appId = 1,
                         tpl = "",
                         timeStamp = (object)null,
                         nounce = (object)null,
                         token = (object)null
                     }), headers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }
        /// <summary>
        /// 上架
        /// </summary>
        /// <param name="petid"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string Sale(string petid, string amount)
        {
            try
            {
                long requestId = DateTime.Now.Ticks / 42000;
                var jsonStr = PostMoths(new Uri("https://pet-chain.baidu.com/data/market/sale/shelf/create"),
                    JsonConvert.SerializeObject(new
                    {
                        petId = petid,
                        amount = amount,
                        requestId = requestId,
                        appId = 1,
                        tpl = "",
                        timeStamp = (object)null,
                        nounce = (object)null,
                        token = (object)null
                    }), headers);
                var jsonObj = JObject.Parse(jsonStr);
                if (jsonObj["errorNo"].Value<String>() == "00")
                {
                    var order_id = jsonObj["data"]["orderId"].Value<String>();
                    var nonce = jsonObj["data"]["nonce"].Value<String>();
                    var s = GetSHA256HashFromString(mainVM.Password) + "|" + order_id + "|" + nonce;
                    s = rsa.Encrypt(s);
                    jsonStr = PostMoths(new Uri("https://pet-chain.baidu.com/data/order/confirm"),
                         JsonConvert.SerializeObject(new
                         {
                             confirmType = 1,
                             s = s,
                             requestId = requestId,
                             appId = 1,
                             tpl = "",
                             timeStamp = (Object)null,
                             nounce = (Object)null,
                             token = (Object)null
                         }), headers);
                }
                return jsonStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }
        /// <summary>
        /// 下架
        /// </summary>
        /// <param name="petid"></param>
        /// <returns></returns>
        public string Unsale(string petid)
        {
            try
            {
                long requestId = DateTime.Now.Ticks / 42000;
                return PostMoths(new Uri("https://pet-chain.baidu.com/data/market/unsalePet"),
                     JsonConvert.SerializeObject(new
                     {
                         petId = petid,
                         requestId = requestId,
                         appId = 1,
                         tpl = "",
                         timeStamp = (object)null,
                         nounce = (object)null,
                         token = (object)null
                     }), headers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }
        /// <summary>
        /// 查询商店列表
        /// </summary>
        /// <returns></returns>
        public string GetBuyList(int pageNo, int rareDegree)
        {
            try
            {
                long requestId = DateTime.Now.Ticks / 42000;
                return PostMoths(new Uri("https://pet-chain.baidu.com/data/market/queryPetsOnSale"),
                     JsonConvert.SerializeObject(new
                     {
                         pageNo = pageNo,
                         pageSize = 10,
                         querySortType = "AMOUNT_ASC",
                         petIds = new string[0],
                         lastAmount = "",
                         lastRareDegree = "",
                         filterCondition = "{\"1\":\"" + rareDegree + "\"}",
                         appId = 1,
                         tpl = "",
                         type = (Object)null,
                         requestId = requestId,
                         timeStamp = (Object)null,
                         nounce = (Object)null,
                         token = (Object)null
                     }), headers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }
        /// <summary>
        /// 买入
        /// </summary>
        /// <returns></returns>
        public string Buy(string petid, string amount)
        {
            try
            {
                long requestId = DateTime.Now.Ticks / 42000;
                //获取验证码
                var jsonStr = PostMoths(new Uri("https://pet-chain.baidu.com/data/captcha/gen"),
                    JsonConvert.SerializeObject(new
                    {
                        requestId = requestId,
                        appId = 1,
                        tpl = "",
                        timeStamp = (Object)null,
                        nounce = (Object)null,
                        token = (Object)null
                    }), headers);
                var jsonObj = JObject.Parse(jsonStr);
                if (jsonObj["errorNo"].Value<String>() == "00")
                {
                    string seed = jsonObj["data"]["seed"].Value<String>();
                    string img = jsonObj["data"]["img"].Value<String>();
                    //识别验证码
                    jsonStr = PostMoths(new Uri(mainVM.ApiUrl + "bd_server/get_code"),
                     JsonConvert.SerializeObject(new
                     {
                         key = mainVM.ApiKey,
                         img = img
                     }), headers);
                    jsonObj = JObject.Parse(jsonStr);
                    string captcha = jsonObj["code"].Value<String>();
                    //买入
                    requestId = DateTime.Now.Ticks / 42000;
                    jsonStr = PostMoths(new Uri("https://pet-chain.baidu.com/data/txn/sale/create"),
                         JsonConvert.SerializeObject(new
                         {
                             petId = petid,
                             amount = amount,
                             seed = seed,
                             captcha = captcha,
                             validCode = "",
                             requestId = requestId,
                             appId = 1,
                             tpl = "",
                             timeStamp = (Object)null,
                             nounce = (Object)null,
                             token = (Object)null
                         }), headers);
                    jsonObj = JObject.Parse(jsonStr);
                    if (jsonObj["errorNo"].Value<String>() == "00")
                    {
                        var order_id = jsonObj["data"]["orderId"].Value<String>();
                        var nonce = jsonObj["data"]["nonce"].Value<String>();
                        var s = GetSHA256HashFromString(mainVM.Password) + "|" + order_id + "|" + nonce;
                        s = rsa.Encrypt(s);
                        jsonStr = PostMoths(new Uri("https://pet-chain.baidu.com/data/order/confirm"),
                             JsonConvert.SerializeObject(new
                             {
                                 confirmType = 2,
                                 s = s,
                                 requestId = requestId,
                                 appId = 1,
                                 tpl = "",
                                 timeStamp = (Object)null,
                                 nounce = (Object)null,
                                 token = (Object)null
                             }), headers);
                        PostMoths(new Uri(mainVM.ApiUrl + "bd_server/save_code"),
                            JsonConvert.SerializeObject(new
                            {
                                img = img,
                                code = captcha
                            }), headers);
                    }
                    else if (jsonObj["errorNo"].Value<String>() == "100")
                    {
                        PostMoths(new Uri(mainVM.ApiUrl + "bd_server/save_code2"),
                            JsonConvert.SerializeObject(new
                            {
                                img = img,
                                code = captcha
                            }), headers);
                    }
                    else
                    {
                        listPetId.Add(petid);
                        PostMoths(new Uri(mainVM.ApiUrl + "bd_server/save_code"),
                            JsonConvert.SerializeObject(new
                            {
                                img = img,
                                code = captcha
                            }), headers);
                    }
                }
                return jsonStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
            txtCookie.Text = loginWindow.Cookie;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            mainVM.SaveData();
        }
        private string GetSHA256HashFromString(string strData)
        {
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(strData);
            try
            {
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] retVal = sha256.ComputeHash(bytValue);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetSHA256HashFromString() fail,error:" + ex.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            StopThread();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            mainVM.Password = txtPassword.Password;
        }
    }
}
