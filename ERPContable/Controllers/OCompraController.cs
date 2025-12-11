using ERPContable.Data;
using ERPContable.Models;
using ERPContable.ViewModels.OCompra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERPContable.Controllers
{
    public class OCompraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OCompraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------- MÉTODOS AUXILIARES -------------------------

        // Método de ayuda para cargar DropDownLists
        private async Task PopulateDropDowns(OCompraViewModel vm = null)
        {
            // Proveedores
            ViewData["ProveedorId"] = new SelectList(
                await _context.Proveedores.ToListAsync(),
                "id",
                "nombre",
                vm?.ProveedorId
            );

            // Requerimientos Internos (Solo aquellos que aún no están totalmente cerrados o asociados a una OC)
            ViewData["ReqInternoId"] = new SelectList(
                await _context.ReqInternos
                    .Where(r => r.estadoId != 99) // Asumiendo que 99 es un estado 'Cerrado/Completado'
                    .Select(r => new { r.id, Display = $"Req. N° {r.id} ({r.fechaHora:dd/MM/yyyy})" })
                    .ToListAsync(),
                "id",
                "Display",
                vm?.ReqInternoId
            );

            // Productos (para la grilla de detalles)
            ViewData["ProductoId"] = new SelectList(await _context.Productos.ToListAsync(), "id", "nombre", null);

            // Estado y Documento (Normalmente no se seleccionan en Create/Edit, pero se cargan para el maestro)
            ViewData["DocumentoId"] = new SelectList(await _context.Documentos.ToListAsync(), "id", "nombre", vm?.DocumentoId);
            ViewData["EstadoId"] = new SelectList(
                await _context.Estados.Where(e => e.tabla == "OCOMPRA").ToListAsync(),
                "id",
                "nombre",
                vm?.EstadoId
            );
        }

        // Método de ayuda para obtener el ViewModel con todos los includes
        private async Task<OCompraViewModel> GetOCompraViewModelForDisplay(int id)
        {
            var oCompra = await _context.OCompras
                .Include(o => o.DCompras)
                    .ThenInclude(d => d.Producto)
                .Include(o => o.Proveedor)
                .Include(o => o.Estado)
                .Include(o => o.Documento)
                // Incluir Requerimiento si existe (opcional)
                .Include(o => o.ReqInterno)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.id == id);

            if (oCompra == null) return null;

            // Mapeo a ViewModel
            var vm = new OCompraViewModel
            {
                Id = oCompra.id,
                FechaHora = oCompra.fechaHora,
                Nota = oCompra.nota,
                ProveedorId = oCompra.proveedorId,
                ReqInternoId = oCompra.reqinternoId, // Puede ser int?
                DocumentoId = oCompra.documentoId,
                EstadoId = oCompra.estadoId,

                // Propiedades de solo lectura
                ProveedorNombre = oCompra.Proveedor.nombre,
                DocumentoCodigo = oCompra.Documento.codigo,
                EstadoNombre = oCompra.Estado.nombre,

                // Mapeo de Detalles y cálculo del Total (necesario para Index/Details)
                Detalles = oCompra.DCompras.Select(d => new DCompraViewModel
                {
                    ProductoId = d.productoId,
                    Cantidad = d.cantidad,
                    Precio = d.precio,
                    Observacion = d.observacion,
                    ProductoNombre = d.Producto?.nombre,
                    ProductoCodigo = d.Producto?.codigo
                }).ToList(),
                // Calcular Total sumando los totales de línea
                //Total = oCompra.DCompras.Sum(d => d.cantidad * d.precio)
            };

            return vm;
        }


        // ----------------------------- 1. INDEX -----------------------------

        // GET: OCompra
        public async Task<IActionResult> Index()
        {
            // Cargar solo los datos necesarios para la tabla (Mejorar rendimiento con Projection)
            var oCompras = await (
                from oc in _context.OCompras
                join p in _context.Proveedores on oc.proveedorId equals p.id
                join e in _context.Estados on oc.estadoId equals e.id
                orderby oc.fechaHora descending
                select new OCompraViewModel
                {
                    Id = oc.id,
                    FechaHora = oc.fechaHora,
                    ProveedorNombre = p.nombre,
                    EstadoNombre = e.nombre,
                    ReqInternoId = oc.reqinternoId,
                    // Cargar el total de la suma de los detalles (esto puede ser costoso, idealmente se calcula y guarda)
                    // Para evitar múltiples consultas: usa un cálculo en la misma consulta
                    //Total = oc.DCompras.Sum(d => (decimal?)d.cantidad * d.precio) ?? 0M
                }
            ).ToListAsync();

            return View(oCompras);
        }

        // ----------------------------- 2. DETAILS -----------------------------

        // GET: OCompra/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vm = await GetOCompraViewModelForDisplay(id.Value);

            if (vm == null) return NotFound();

            return View(vm);
        }

        // ----------------------------- 3. CREATE -----------------------------

        // GET: OCompra/Create
        public async Task<IActionResult> Create()
        {
            // Cargar DropDowns
            await PopulateDropDowns();

            // Estado inicial: Pendiente (asumiendo que EstadoId = 1 es Pendiente de Aprobación/Ejecución)
            // Y buscar el Documento de OC (asumiendo que DocumentoId = 2 es OC)
            var estadoInicial = await _context.Estados.FirstOrDefaultAsync(e => e.tabla == "OCOMPRA" && e.nombre.ToLower() == "pendiente");
            var documentoOC = await _context.Documentos.FirstOrDefaultAsync(d => d.codigo.ToLower() == "oc");

            var vm = new OCompraViewModel
            {
                FechaHora = DateTime.Now,
                EstadoId = estadoInicial?.id ?? 1, // Asignar el ID encontrado o 1 por defecto
                DocumentoId = documentoOC?.id ?? 2, // Asignar el ID encontrado o 2 por defecto
                Detalles = new List<DCompraViewModel>() // Inicializar la lista de detalles
            };

            return View(vm);
        }

        // POST: OCompra/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaHora,Nota,ProveedorId,ReqInternoId,DocumentoId,EstadoId,Detalles")] OCompraViewModel vm)
        {
            // 1. Validación de detalles
            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                ModelState.AddModelError("Detalles", "La Orden de Compra debe tener al menos un producto.");
            }

            // 2. Manejo de Fallo de Validación (Si el JS/AJAX lo maneja, retornar JSON)
            if (!ModelState.IsValid)
            {
                // Si el request es AJAX, retornamos errores como JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());
                    return Json(new { success = false, errors = errors });
                }

                // Si el request es normal, volvemos a cargar las listas y la vista
                await PopulateDropDowns(vm);
                return View(vm);
            }

            // 3. Persistencia con Transacción
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Crear Cabecera
                    var oCompra = new OCompra
                    {
                        fechaHora = vm.FechaHora,
                        nota = vm.Nota,
                        proveedorId = vm.ProveedorId.GetValueOrDefault(),
                        reqinternoId = vm.ReqInternoId, // int? mapea correctamente
                        documentoId = vm.DocumentoId,
                        estadoId = vm.EstadoId
                    };

                    _context.OCompras.Add(oCompra);
                    await _context.SaveChangesAsync(); // Guarda para obtener oCompra.id

                    // Crear Detalles
                    var detalles = vm.Detalles.Select(d => new DCompra
                    {
                        ocompraId = oCompra.id,
                        productoId = d.ProductoId,
                        cantidad = d.Cantidad,
                        precio = d.Precio,
                        observacion = d.Observacion
                    }).ToList();

                    _context.DCompras.AddRange(detalles);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 4. Retorno de Éxito (JSON para AJAX)
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    // Retorno de Fallo (JSON para AJAX)
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Error interno al guardar la Orden de Compra.", debug = ex.Message });
                    }

                    // Retorno de Fallo (Vista normal)
                    ModelState.AddModelError("", "Ocurrió un error al guardar la Orden de Compra.");
                    await PopulateDropDowns(vm);
                    return View(vm);
                }
            }
        }


        // -------------------------- 4. EDIT (Ya proporcionado) --------------------------

        // GET: OCompra/Edit/5 (Mantengo el código que proporcionaste, solo aseguro el uso de GetOCompraViewModelForDisplay)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vm = await GetOCompraViewModelForDisplay(id.Value);

            if (vm == null) return NotFound();

            // ViewData para DropDowns
            await PopulateDropDowns(vm);

            return View(vm);
        }

        // POST: OCompra/Edit/5 (Mantenemos tu lógica de transacciones)
        // ... (Tu implementación de POST Edit aquí, ya es correcta) ...


        // ----------------------------- 5. DELETE -----------------------------

        // GET: OCompra/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vm = await GetOCompraViewModelForDisplay(id.Value);

            if (vm == null) return NotFound();

            return View(vm);
        }

        // POST: OCompra/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var oCompra = await _context.OCompras.FindAsync(id);

                    if (oCompra != null)
                    {
                        // 1. Cargar y eliminar detalles
                        var detalles = await _context.DCompras
                                                    .Where(d => d.ocompraId == id)
                                                    .ToListAsync();
                        _context.DCompras.RemoveRange(detalles);

                        // 2. Eliminar encabezado
                        _context.OCompras.Remove(oCompra);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    // Opcional: Manejar errores específicos, pero re-lanzar un error genérico es común
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}