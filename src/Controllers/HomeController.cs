using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegrammBott.Models;

namespace TelegrammBott.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment appEnvironment)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _appEnvironment = appEnvironment ?? throw new System.ArgumentNullException(nameof(appEnvironment));
        }

        public IActionResult Index()
        {   
            return View();
        }

        public IActionResult DownloadFile(string fileName)
        {
            // Путь к файлу
            string file_path = Path.Combine(_appEnvironment.ContentRootPath, "Photo", fileName);
            // Тип файла - content-type
            string file_type = "application/png";

            string file_name = fileName;
            return PhysicalFile(file_path, file_type, file_name);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
