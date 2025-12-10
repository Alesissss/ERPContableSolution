using ERPContable.Data;
using Microsoft.AspNetCore.Mvc;

namespace ERPContable.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmpresaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult GestionarEmpresas()
        {
            return View();
        }
    }
}
