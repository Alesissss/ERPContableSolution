using ERPContable.Data;
using ERPContable.Models;
using ERPContable.ViewModels; // Necesario para el LoginViewModel
using ERPContable.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para .FirstOrDefaultAsync()
using System.Diagnostics;

namespace ERPContable.Controllers
{
    // Usaremos un filtro para obligar a iniciar sesión en todos los controladores, 
    // excepto en el Login. Esto se puede hacer con una clase base, pero por simplicidad,
    // usaremos la Sesión y redirección aquí.

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constantes para el manejo de Sesiones
        private const string SessionKeyUserId = "_UserId";
        private const string SessionKeyUserName = "_UserName";
        private const string SessionKeyUserRole = "_UserRole"; // Simularemos un rol

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // MÉTODOS DE AUTENTICACIÓN
        // ==========================================================

        // [GET] /Home/Login
        public IActionResult Login()
        {
            // Si el usuario ya está autenticado, redirigir al Dashboard
            if (HttpContext.Session.GetInt32(SessionKeyUserId).HasValue)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // [POST] /Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscamos el usuario por Username y Password
                // IMPORTANTE: En producción, el password debe estar HASHED
                var personal = await _context.Personales
                    .FirstOrDefaultAsync(p => p.username == model.Username && p.password == model.Password);

                if (personal != null)
                {
                    // 1. Establecer Sesiones de Autenticación
                    HttpContext.Session.SetInt32(SessionKeyUserId, personal.id);
                    HttpContext.Session.SetString(SessionKeyUserName, $"{personal.nombres} {personal.apellidoPaterno}");
                    // Simulación de un rol para ejemplo de barra lateral
                    HttpContext.Session.SetString(SessionKeyUserRole, "Administrador de Almacén");

                    // 2. Redirigir al Dashboard
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Credenciales inválidas. Por favor, inténtelo de nuevo.");
            }

            // Si hay errores, volvemos a mostrar el formulario de login
            return View(model);
        }

        // [GET] /Home/Logout
        public IActionResult Logout()
        {
            // Limpiar la Sesión
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==========================================================
        // MÉTODOS PROTEGIDOS (Requieren Sesión)
        // ==========================================================

        // Filtro manual de autenticación
        private IActionResult CheckAuthentication()
        {
            if (!HttpContext.Session.GetInt32(SessionKeyUserId).HasValue)
            {
                // Si no está logueado, redirigir a Login
                return RedirectToAction("Login");
            }
            return null; // El usuario está autenticado
        }

        public IActionResult Index()
        {
            var authCheck = CheckAuthentication();
            if (authCheck != null) return authCheck; // Redirigir si no está logueado

            return View();
        }

        public IActionResult Privacy()
        {
            var authCheck = CheckAuthentication();
            if (authCheck != null) return authCheck;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Nota: Podrías necesitar un manejo especial si el error ocurre en Login
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}