using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bd_pet.ViewModels
{
    public class LogMsgItem : NotificationObject
    {
        private string _message;
        /// <summary>
        /// 信息
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                this.RaisePropertyChange("Message");
            }
        }
    }
}
