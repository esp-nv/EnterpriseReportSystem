using Microsoft.AspNetCore.Mvc;

namespace EnterpriseReportSystem.Controllers;
public class HomeController1 : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
