using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SafeToNet.Prototype.Core.Interfaces;

namespace SafeToNet.Prototype.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ISearchBusiness _business;
        private readonly ILogger _logger;

        public SearchController(ILogger<SearchController> logger, ISearchBusiness searchBusiness)
        {
            _business = searchBusiness ?? throw new ArgumentNullException(nameof(searchBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogDebug("SearchController .ctor");
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Search()
        {
            var searchBytes = await this.GetFileBytesFromMultipartRequest("audio");

            var result = await _business.SearchWithSpeech(searchBytes);

            return Ok(result);
        }

        [HttpGet("")]
        public async Task<IActionResult> SearchByText([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest();

            var result = await _business.SearchWithText(query);

            return Ok(result);
        }

        private async Task<byte[]> GetFileBytesFromMultipartRequest(string fileType)
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.Files.Count(f => f.ContentType.Contains(fileType)) == 1)
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