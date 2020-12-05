using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManager
{

    public class FileSystemInfoDto
    {
        public string fileName { get; set; }
        public DateTime creationTime { get; set; }
        public string url { get; set; }
        public string size { get; set; }

    }
}
