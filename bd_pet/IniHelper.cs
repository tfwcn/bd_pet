using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace bd_pet
{
    /// <summary>
    /// 操作INI文件
    /// </summary>
    public class IniHelper
    {
        #region "声明变量"

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        private static string strFilePath = AppDomain.CurrentDomain.BaseDirectory + "config.ini";//获取INI文件路径

        public static bool Write(string section, string key,string value)
        {
            try
            {
                //根据INI文件名设置要写入INI文件的节点名称
                WritePrivateProfileString(section, key, value, strFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string Read(string section,string key)
        {
            return ContentValue(section, key);
        }

        /// <summary>
        /// 自定义读取INI文件中的内容方法
        /// </summary>
        /// <param name="Section">节点</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        private static string ContentValue(string Section, string key)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, key, "", temp, 1024, strFilePath);
            return temp.ToString();
        }

        #endregion
    }
}
