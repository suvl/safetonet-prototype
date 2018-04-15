using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SafeToNet.Prototype.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Search()
        {
            var searchBytes = await this.GetFileBytesFromMultipartRequest("audio");



            return Ok();
        }

        private async Task<byte[]> GetFileBytesFromMultipartRequest(string fileType)
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.Files.Where(f => f.ContentType.Contains(fileType)).Count() == 1)
                {
                    var file = form.Files.FirstOrDefault(f => f.ContentType.Contains(fileType));
                    using (var memStream = new MemoryStream())
                    {
                        file.CopyTo(memStream);
                        return memStream.ToArray();
                    }
                }
                else
                {
                    throw new Exception("Too many files were provided");
                }
            }
            throw new Exception($"Wrong content type. Should be multipart/form-data, was {Request.ContentType}");
        }
    }
}