using Microsoft.AspNetCore.Mvc;

namespace Rabe_Celina_HW6.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
