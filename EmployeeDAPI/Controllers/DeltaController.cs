using Microsoft.AspNetCore.Mvc;

namespace EmployeeDAPI.Controllers
{
    public class DeltaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Display()
        {
            return Ok("Successfull!");
        }
    }
}
