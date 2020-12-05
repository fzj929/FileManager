using System;

namespace FileManager
{
    public class CmdHelper
    {

        /// <summary>
        /// 获取命令行参数
        /// </summary>
        /// <param name="pName">参数名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string QueryParameter(string pName, string defaultValue=null)
        {
            var args = Environment.GetCommandLineArgs();
            bool findValue = false;

            foreach (var item in args)
            {
                //Console.WriteLine(item);
                if (findValue)
                {
                    defaultValue = item;
                    break;
                }
                if(pName.ToUpper() == item.ToUpper())
                {
                    findValue = true;
                }
            }
            return defaultValue;
        }
    }
}
