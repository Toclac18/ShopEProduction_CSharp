using Microsoft.AspNetCore.Mvc;

namespace ShopEProduction.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
