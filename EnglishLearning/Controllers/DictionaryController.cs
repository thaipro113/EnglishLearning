using Microsoft.AspNetCore.Mvc;

namespace EnglishLearning.Controllers
{
    public class DictionaryController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}