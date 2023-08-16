using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace Todo.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class FileUploadController : Controller
    {
        private IWebHostEnvironment _iWebHostEnvironment;
        public FileUploadController(IWebHostEnvironment iWebHostEnvironment) 
        {
            _iWebHostEnvironment = iWebHostEnvironment;
        }

        [HttpPost]
        public IActionResult filesUpload(List<IFormFile> files, [FromForm] Guid id)
        {
            string savePath = _iWebHostEnvironment.ContentRootPath + $@"\wwwroot\UploadFiles\{id}\";

            if (!Directory.Exists(savePath)) 
            {
                Directory.CreateDirectory(savePath);
            }

            foreach (IFormFile file in files) 
            {
                using (var stream = System.IO.File.Create(savePath + file.FileName)) 
                {
                    file.CopyTo(stream);
                }

                //存入上傳檔名與路徑等資料
                var fileObject = new
                {
                    fileName = file.FileName,
                    src = $@"\UploadFiles\{id}\{file.FileName}",
                    todoId = id
                }; 
            }
            return Ok("以上傳檔案");
        }
    }
}
