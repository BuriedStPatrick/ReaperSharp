using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReaperAPI.Services;
using ReaperCore;

namespace ReaperAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IReaperParser _reaperParser;
        private readonly IFileOptions _fileOptions;
        private readonly IFileService _fileService;

        public ProjectController(
            ILogger<ProjectController> logger,
            IReaperParser reaperParser,
            IFileOptions fileOptions,
            IFileService fileService
        )
        {
            _logger = logger;
            _reaperParser = reaperParser;
            _fileOptions = fileOptions;
            _fileService = fileService;
        }

        [HttpPost, RequestSizeLimit(1000000)]
        public async Task<IActionResult> Post([FromForm] IFormFile reaperFile)
        {
            var ids = await _fileService.SaveFiles(reaperFile);

            return Ok(ids);
        }

        [HttpPost]
        public async Task<IActionResult> Parse(Guid id)
        {
            var files = await _fileService.GetFiles(id);
            var parsed = (await Task.WhenAll(files.Select(f => _reaperParser.ParseAsync(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    _fileOptions.ProcessingFolder,
                    f.Id.ToString()
                )
            )))).ToList();

            return Ok(parsed);
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var files = await _fileService.GetFiles(id);

            return Ok(files);
        }
    }
}
