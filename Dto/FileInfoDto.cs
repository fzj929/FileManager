using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileManager.Dto
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileInfoDto
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 文件内容
        /// </summary>
        public string fileContent { get; set; }
    }


}
