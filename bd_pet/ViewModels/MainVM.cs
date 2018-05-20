using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace bd_pet.ViewModels
{
    public class MainVM : NotificationObject
    {
        #region 买入
        private int commonBuy;
        /// <summary>
        /// 普通买入价
        /// </summary>
        public int CommonBuy
        {
            get { return commonBuy; }
            set
            {
                commonBuy = value;
                this.RaisePropertyChange("CommonBuy");
            }
        }

        private int rareBuy;
        /// <summary>
        /// 稀有买入价
        /// </summary>
        public int RareBuy
        {
            get { return rareBuy; }
            set
            {
                rareBuy = value;
                this.RaisePropertyChange("RareBuy");
            }
        }

        private int excellenceBuy;
        /// <summary>
        /// 卓越买入价
        /// </summary>
        public int ExcellenceBuy
        {
            get { return excellenceBuy; }
            set
            {
                excellenceBuy = value;
                this.RaisePropertyChange("ExcellenceBuy");
            }
        }

        private int epicBuy;
        /// <summary>
        /// 史诗买入价
        /// </summary>
        public int EpicBuy
        {
            get { return epicBuy; }
            set
            {
                epicBuy = value;
                this.RaisePropertyChange("EpicBuy");
            }
        }

        private int mythicalBuy;
        /// <summary>
        /// 神话买入价
        /// </summary>
        public int MythicalBuy
        {
            get { return mythicalBuy; }
            set
            {
                mythicalBuy = value;
                this.RaisePropertyChange("MythicalBuy");
            }
        }

        private int legendBuy;
        /// <summary>
        /// 传说买入价
        /// </summary>
        public int LegendBuy
        {
            get { return legendBuy; }
            set
            {
                legendBuy = value;
                this.RaisePropertyChange("LegendBuy");
            }
        }
        #endregion

        #region 销售
        private int commonSale;
        /// <summary>
        /// 普通销售价
        /// </summary>
        public int CommonSale
        {
            get { return commonSale; }
            set
            {
                commonSale = value;
                this.RaisePropertyChange("CommonSale");
            }
        }

        private int rareSale;
        /// <summary>
        /// 稀有销售价
        /// </summary>
        public int RareSale
        {
            get { return rareSale; }
            set
            {
                rareSale = value;
                this.RaisePropertyChange("RareSale");
            }
        }

        private int excellenceSale;
        /// <summary>
        /// 卓越销售价
        /// </summary>
        public int ExcellenceSale
        {
            get { return excellenceSale; }
            set
            {
                excellenceSale = value;
                this.RaisePropertyChange("ExcellenceSale");
            }
        }

        private int epicSale;
        /// <summary>
        /// 史诗销售价
        /// </summary>
        public int EpicSale
        {
            get { return epicSale; }
            set
            {
                epicSale = value;
                this.RaisePropertyChange("EpicSale");
            }
        }

        private int mythicalSale;
        /// <summary>
        /// 神话销售价
        /// </summary>
        public int MythicalSale
        {
            get { return mythicalSale; }
            set
            {
                mythicalSale = value;
                this.RaisePropertyChange("MythicalSale");
            }
        }

        private int legendSale;
        /// <summary>
        /// 传说销售价
        /// </summary>
        public int LegendSale
        {
            get { return legendSale; }
            set
            {
                legendSale = value;
                this.RaisePropertyChange("LegendSale");
            }
        }
        #endregion

        private string apiUrl;
        /// <summary>
        /// Api链接
        /// </summary>
        public string ApiUrl
        {
            get { return apiUrl; }
            set
            {
                apiUrl = value;
                this.RaisePropertyChange("ApiUrl");
            }
        }

        private string apiKey;
        /// <summary>
        /// ApiKey
        /// </summary>
        public string ApiKey
        {
            get { return apiKey; }
            set
            {
                apiKey = value;
                this.RaisePropertyChange("ApiKey");
            }
        }

        private string password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.RaisePropertyChange("Password");
            }
        }

        private string cookie;
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie
        {
            get { return cookie; }
            set
            {
                cookie = value;
                this.RaisePropertyChange("Cookie");
            }
        }

        private bool isBuy;
        /// <summary>
        /// 自动买入
        /// </summary>
        public bool IsBuy
        {
            get { return isBuy; }
            set
            {
                isBuy = value;
                this.RaisePropertyChange("IsBuy");
            }
        }

        private bool isSale;
        /// <summary>
        /// 自动卖出
        /// </summary>
        public bool IsSale
        {
            get { return isSale; }
            set
            {
                isSale = value;
                this.RaisePropertyChange("IsSale");
            }
        }

        private ObservableCollection<LogMsgItem> logMsg;
        /// <summary>
        /// 日志
        /// </summary>
        public ObservableCollection<LogMsgItem> LogMsg
        {
            get { return logMsg; }
            set
            {
                logMsg = value;
                this.RaisePropertyChange("LogMsg");
            }
        }

        public MainVM()
        {
            logMsg = new ObservableCollection<LogMsgItem>();
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        public void LoadData()
        {
            this.CommonBuy = Convert.ToInt32(IniHelper.Read("Buy", "common_buy"));
            this.RareBuy = Convert.ToInt32(IniHelper.Read("Buy", "rare_buy"));
            this.ExcellenceBuy = Convert.ToInt32(IniHelper.Read("Buy", "excellence_buy"));
            this.EpicBuy = Convert.ToInt32(IniHelper.Read("Buy", "epic_buy"));
            this.MythicalBuy = Convert.ToInt32(IniHelper.Read("Buy", "mythical_buy"));
            this.LegendBuy = Convert.ToInt32(IniHelper.Read("Buy", "legend_buy"));

            this.CommonSale = Convert.ToInt32(IniHelper.Read("Sale", "common_sale"));
            this.RareSale = Convert.ToInt32(IniHelper.Read("Sale", "rare_sale"));
            this.ExcellenceSale = Convert.ToInt32(IniHelper.Read("Sale", "excellence_sale"));
            this.EpicSale = Convert.ToInt32(IniHelper.Read("Sale", "epic_sale"));
            this.MythicalSale = Convert.ToInt32(IniHelper.Read("Sale", "mythical_sale"));
            this.LegendSale = Convert.ToInt32(IniHelper.Read("Sale", "legend_sale"));

            this.ApiUrl = IniHelper.Read("BaiduCodeAPI", "key");
            this.ApiKey = IniHelper.Read("BaiduCodeAPI", "url");
            this.Cookie = IniHelper.Read("BaiduCodeAPI", "cookie");
            this.Password = IniHelper.Read("BaiduCodeAPI", "password");

            this.IsBuy = Convert.ToBoolean(IniHelper.Read("Setting", "isBuy"));
            this.IsSale = Convert.ToBoolean(IniHelper.Read("Setting", "isSale"));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public void SaveData()
        {
            IniHelper.Write("Buy", "common_buy", this.CommonBuy.ToString());
            IniHelper.Write("Buy", "rare_buy", this.RareBuy.ToString());
            IniHelper.Write("Buy", "excellence_buy", this.ExcellenceBuy.ToString());
            IniHelper.Write("Buy", "epic_buy", this.EpicBuy.ToString());
            IniHelper.Write("Buy", "mythical_buy", this.MythicalBuy.ToString());
            IniHelper.Write("Buy", "legend_buy", this.LegendBuy.ToString());

            IniHelper.Write("Sale", "common_sale", this.CommonSale.ToString());
            IniHelper.Write("Sale", "rare_sale", this.RareSale.ToString());
            IniHelper.Write("Sale", "excellence_sale", this.ExcellenceSale.ToString());
            IniHelper.Write("Sale", "epic_sale", this.EpicSale.ToString());
            IniHelper.Write("Sale", "mythical_sale", this.MythicalSale.ToString());
            IniHelper.Write("Sale", "legend_sale", this.LegendSale.ToString());

            IniHelper.Write("BaiduCodeAPI", "key", this.ApiUrl);
            IniHelper.Write("BaiduCodeAPI", "url", this.ApiKey);
            IniHelper.Write("BaiduCodeAPI", "cookie", this.Cookie);
            IniHelper.Write("BaiduCodeAPI", "password", this.Password);
            IniHelper.Write("Setting", "isBuy", this.IsBuy.ToString());
            IniHelper.Write("Setting", "isSale", this.IsSale.ToString());
        }
    }
}
