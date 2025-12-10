using ERPContable.Data;
using Microsoft.AspNetCore.Mvc;

namespace ERPContable.Controllers
{
    public class ReqInternoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReqInternoController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult GestionarReqInternos()
        {
            return View();
        }
    }
}
