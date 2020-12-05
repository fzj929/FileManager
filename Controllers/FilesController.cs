using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileManager.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {

        private readonly ILogger<FilesController> _logger;
        public static readonly string FileRootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

        static FilesController()
        {
            if (!System.IO.Directory.Exists(FileRootPath))
            {
                System.IO.Directory.CreateDirectory(FileRootPath);
            }
        }

        public FilesController(ILogger<FilesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<FileSystemInfoDto> Get()
        {
            var files = System.IO.Directory.GetFiles(FileRootPath);
            List<FileSystemInfoDto> fInfoList = new List<FileSystemInfoDto>();

            foreach (var item in files)
            {
                var fInfo = new FileInfo(item);
                fInfoList.Add(new FileSystemInfoDto
                {
                    creationTime = fInfo.CreationTime,//.ToString("yyyy-MM-dd HH:mm:ss"),
                    fileName = fInfo.Name,
                    url = createUrl(fInfo.Name),
                    size = getSizeStr(fInfo)
                }) ;
                
            }
            return fInfoList.OrderByDescending(o => o.creationTime);
        }

        static string getSizeStr(FileInfo fInfo)
        {
            long size = fInfo.Length;
            string unit = "B";
            double sizeValue = size;
            if (size < 1024)
            {
                unit = "B";
                sizeValue = size;
            }
            else if (size < 1024 * 1024)
            {
                unit = "KB";
                sizeValue = size * 1.0 / 1024;
            }
            else if (size < 1024 * 1024 * 1024)
            {
                unit = "MB";
                sizeValue = size * 1.0 / 1024 / 1024;
            }
            else
            {
                unit = "GB";
                sizeValue = size * 1.0 / 1024 / 1024 / 1024;
            }
            return $"{sizeValue.ToString("0.00")}{unit}";
        }

        [HttpGet("{id}", Name =nameof(Get))]
        public async Task<ActionResult> Get(string id)
        {
            var info = strToDto(id);

            string fileName = fullFileName(id);

            if (!System.IO.File.Exists(fileName))
            {
                return NoContent();
            }

            var fBuffer = System.IO.File.ReadAllBytesAsync(fileName);
            var buffer = await fBuffer;
            info.fileContent = System.Convert.ToBase64String(buffer);

            return Ok(info);
        }


        [HttpPost("UploadFiles")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> UploadFiles()
        {
            var files = Request.Form.Files;

            foreach (var item in files)
            {
                if (System.IO.File.Exists(fullFileName(item.FileName)))
                {
                    return BadRequest(new { msg = "文件已经存在:" + item.FileName, status = 8001, isSuccess = false });
                }
            }

            List<FileInfoDto> rList = new List<FileInfoDto>();
            foreach (var file in files)
            {
                long fileSize = file.Length; //获得文件大小，以字节为单位
                string newFileName = file.FileName; //随机生成新的文件名
                var path = fullFileName(newFileName);

                using (FileStream fs = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(fs);
                    await fs.FlushAsync();
                }
                rList.Add(strToDto(file.FileName));
            }
            rList.ForEach(f=> {
                f.fileContent = createUrl(f.fileName);
            });
            return Ok(rList);
        }

        string createUrl(string f)
        {
            var host = string.Format("{0}://{1}", this.Request.Scheme, this.Request.Host.ToString());
            var url = string.Format("{0}/files/{1}", host, f);// System.Net.WebUtility.UrlEncode(f));
            return url;
        }

        string fullFileName(string fileName)
        {
            var path = System.IO.Path.Combine(FileRootPath, fileName);
            return path;
        }

        FileInfoDto strToDto(string nameStr)
        {
            var rValue = new FileInfoDto
            {
                fileName = nameStr,
            };
            return rValue;
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAsync(string id)
        {
            string fileName = fullFileName(id);
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody]FileInfoDto fileInfo)
        {
            if(fileInfo == null)
            {
                return BadRequest(); ;
            }
            if (string.IsNullOrWhiteSpace(fileInfo.fileContent))
            {
                return BadRequest();
            }

            var buffer = System.Convert.FromBase64String(fileInfo.fileContent);

            var fileFullName = fullFileName(fileInfo.fileName);

            if(System.IO.File.Exists(fileFullName))
            {
                return BadRequest(new { msg = "文件已经存在" + fileInfo.fileName, status = 8001, isSuccess = false });
            }

            var task = System.IO.File.WriteAllBytesAsync(fileFullName, buffer);
            await task;

            fileInfo.fileContent = "";
            return CreatedAtRoute(nameof(Get), new
            {
                id = fileInfo.fileName
            }, fileInfo);
        }
    }
}
