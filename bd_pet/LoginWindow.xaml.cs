using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using CefSharp;
using CefSharp.Wpf;
using System.Security.Cryptography.X509Certificates;

namespace bd_pet
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string Cookie = "";
        private ChromiumWebBrowser chromiumWebBrowser;
        private MyRequestHandler myRequest = new MyRequestHandler();
        public LoginWindow()
        {
            InitializeComponent();
            var setting = new CefSharp.CefSettings();
            setting.Locale = "zh-CN";
            //缓存路径
            //setting.CachePath = "/BrowserCache";
            //浏览器引擎的语言
            setting.AcceptLanguageList = "zh-CN,zh-TW;q=0.9,zh;q=0.8,en-US;q=0.7,en;q=0.6";
            //setting.LocalesDirPath = "/localeDir";
            //日志文件
            //setting.LogFile = "/LogData";
            setting.PersistSessionCookies = true;
            setting.UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Pixel 2 XL Build/OPD1.170816.004) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Mobile Safari/537.36";
            //setting.UserDataPath = "/userData";
            CefSharp.Cef.Initialize(setting,true,new MyBrowserProcessHandler());
            chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Address = "https://pet-chain.baidu.com/";
            chromiumWebBrowser.FrameLoadEnd += ChromiumWebBrowser_FrameLoadEnd;
            //chromiumWebBrowser.RequestHandler = myRequest;
            this.pnlWB.Children.Add(chromiumWebBrowser);
        }

        private void ChromiumWebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            if (e.HttpStatusCode == 200 && e.Url.IndexOf("baidu.com") >0)
            {
                Cookie = "";
                //注册获取cookie回调事件
                CookieVisitor visitor = new CookieVisitor();
                visitor.SendCookie += Visitor_SendCookie;
                var cookieManager = CefSharp.Cef.GetGlobalCookieManager();
                cookieManager.VisitAllCookies(visitor);
            }
        }

        private void Visitor_SendCookie(Cookie obj)
        {
            Cookie += String.Format("{0}={1}; ", obj.Name, obj.Value);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        public class CookieVisitor : CefSharp.ICookieVisitor
        {
            public event Action<CefSharp.Cookie> SendCookie;
            public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
            {
                deleteCookie = false;
                if (SendCookie != null)
                {
                    SendCookie(cookie);
                }

                return true;
            }
            public void Dispose()
            {

            }
        }
        public class MyBrowserProcessHandler : IBrowserProcessHandler
        {
            public void Dispose()
            {

            }

            public void OnContextInitialized()
            {

            }

            public void OnScheduleMessagePumpWork(long delay)
            {

            }
        }
        public class MyRequestHandler : IRequestHandler
        {
            public string Cookie = "";
            public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
            {
                //throw new NotImplementedException();  
                Cookie = request.Headers["Cookie"];//<span style="font-family: Arial, Helvetica, sans-serif;">这里弹出response</span>  
                return CefReturnValue.Continue;
            }
            public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
            {
                throw new NotImplementedException();
            }

            public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
            {
                return false;
                //throw new NotImplementedException();  
            }

            //public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
            //{
            //    //throw new NotImplementedException();  
            //    MessageBox.Show(request.Headers["User-Agent"]);//<span style="font-family: Arial, Helvetica, sans-serif;">这里弹出response</span>  
            //    return CefReturnValue.Continue;
            //}

            public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
            {
                throw new NotImplementedException();
            }

            public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
            {
                throw new NotImplementedException();
            }

            public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
            {
                throw new NotImplementedException();
            }

            public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
            {
                throw new NotImplementedException();
            }

            public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
            {
                throw new NotImplementedException();
            }

            public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
            {
                throw new NotImplementedException();
            }

            public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
            {
                //throw new NotImplementedException();  
            }

            public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
            {
                //throw new NotImplementedException();  
            }

            public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl)
            {
                throw new NotImplementedException();
            }

            public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                //throw new NotImplementedException();  
                //MessageBox.Show(response.ResponseHeaders["Set-Cookie"]);//这里弹出response  
                return false;
            }
            public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                return null;
            }
            public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
            {
                //throw new NotImplementedException();
            }
            public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
            {
                throw new NotImplementedException();
            }
        }
    }
}
