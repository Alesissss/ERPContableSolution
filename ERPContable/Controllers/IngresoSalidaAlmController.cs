using ERPContable.Data;
using ERPContable.Models;
using ERPContable.ViewModels.IngresoSalidaAlm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ERPContable.Controllers
{
    public class IngresoSalidaAlmController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngresoSalidaAlmController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------- MÉTODOS AUXILIARES CORREGIDOS -------------------------

        private void PopulateTipoMovimientoList()
        {
            var tipos = new List<SelectListItem>
        {
            // true (1) = Ingreso
            new SelectListItem { Value = "True", Text = "Ingreso (Entrada a Almacén)" }, 
            // false (0) = Salida
            new SelectListItem { Value = "False", Text = "Salida (Salida de Almacén)" }
        };
            // Asignar a ViewBag.tipoMovimiento para que la vista lo use
            ViewBag.tipoMovimiento = tipos;
        }
        private async Task PopulateDropDowns(IngresoSalidaAlmViewModel vm = null)
        {
            // Requerimientos Internos
            ViewData["ReqInternoId"] = new SelectList(
                await _context.ReqInternos
                    .Select(r => new { r.id, Display = $"Req. N° {r.id} ({r.fechaHora:dd/MM/yyyy})" })
                    .ToListAsync(),
                "id",
                "Display",
                vm?.ReqInternoId
            );

            // Órdenes de Compra (Opcional)
            ViewData["OCompraId"] = new SelectList(
                await _context.OCompras
                    .Select(o => new { o.id, Display = $"OC N° {o.id} ({o.fechaHora:dd/MM/yyyy})" })
                    .ToListAsync(),
                "id",
                "Display",
                vm?.OCompraId
            );

            // 🛑 CORRECCIÓN: Documentos. Asumiendo que hay un campo 'tipo' o 'codigo' para filtrar documentos de ALMACÉN.
            // Si no hay columna 'tabla', usar otro criterio (ej. Código D01 para Ingresos, S01 para Salidas).
            // Usaré un filtro genérico por 'tipo' para seguir la lógica si existiera, o cargo todos:
            ViewData["DocumentoId"] = new SelectList(
                await _context.Documentos.ToListAsync(), // Cargo todos si no hay columna de filtro
                "id",
                "nombre",
                vm?.DocumentoId
            );

            // Productos
            ViewData["ProductoId"] = new SelectList(await _context.Productos.ToListAsync(), "id", "nombre", null);

            // 🛑 CORRECCIÓN: Estados. Usar el filtro por 'tabla' sí es correcto si existe en la tabla ESTADOS.
            ViewData["EstadoId"] = new SelectList(
                await _context.Estados.Where(e => e.tabla == "INGRESOSALIDAALM").ToListAsync(),
                "id",
                "nombre",
                vm?.EstadoId
            );
        }

        // Método de ayuda para obtener el ViewModel con todos los includes
        private async Task<IngresoSalidaAlmViewModel> GetIngresoSalidaViewModelForDisplay(int id)
        {
            var ingresoSalida = await _context.IngresoSalidaAlms
                // 🛑 CORRECCIÓN: Nombres de propiedades de navegación corregidos 🛑
                .Include(i => i.DIngresoSalidaAlms)
                    .ThenInclude(d => d.Producto)
                .Include(i => i.ReqInterno)
                .Include(i => i.Documento)
                .Include(i => i.Estado)
                .Include(i => i.OCompra)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.id == id);

            if (ingresoSalida == null) return null;

            // Mapeo a ViewModel
            var vm = new IngresoSalidaAlmViewModel
            {
                Id = ingresoSalida.id,
                TipoMovimiento = ingresoSalida.tipoMovimiento,
                FechaHora = ingresoSalida.fechaHora,
                Nota = ingresoSalida.nota,
                ReqInternoId = ingresoSalida.reqinternoId,
                DocumentoId = ingresoSalida.documentoId,
                EstadoId = ingresoSalida.estadoId,
                OCompraId = ingresoSalida.ocompraId,

                // Propiedades de solo lectura
                ReqInternoNombre = ingresoSalida.ReqInterno != null ? $"Req. N° {ingresoSalida.ReqInterno.id}" : "N/A",
                DocumentoNombre = ingresoSalida.Documento?.nombre,
                EstadoNombre = ingresoSalida.Estado?.nombre,

                // Mapeo de Detalles
                Detalles = ingresoSalida.DIngresoSalidaAlms.Select(d => new DIngresoSalidaAlmViewModel
                {
                    ProductoId = d.productoId,
                    Cantidad = d.cantidad,
                    Observacion = d.observacion,
                    ProductoNombre = d.Producto?.nombre,
                    ProductoCodigo = d.Producto?.codigo
                }).ToList()
            };

            return vm;
        }


        // ----------------------------- 1. INDEX -----------------------------

        // GET: IngresoSalidaAlm
        public async Task<IActionResult> Index()
        {
            var ingresosSalidas = await (
                from isa in _context.IngresoSalidaAlms
                    // 🛑 CORRECCIÓN: Uso de Propiedades de Navegación si se han agregado a ApplicationDbContext 🛑
                join r in _context.ReqInternos on isa.reqinternoId equals r.id
                join d in _context.Documentos on isa.documentoId equals d.id
                join e in _context.Estados on isa.estadoId equals e.id
                orderby isa.fechaHora descending
                select new IngresoSalidaAlmViewModel
                {
                    Id = isa.id,
                    TipoMovimiento = isa.tipoMovimiento,
                    FechaHora = isa.fechaHora,
                    ReqInternoNombre = $"Req. N° {r.id}",
                    DocumentoNombre = d.nombre,
                    EstadoNombre = e.nombre,
                    // Asegurar que la suma funciona si DIngresoSalidaAlms está en el DbContext
                    //TotalArticulos = isa.DIngresoSalidaAlms.Sum(det => (decimal?)det.cantidad) ?? 0M
                }
            ).ToListAsync();

            return View(ingresosSalidas);
        }

        // ----------------------------- 2. DETAILS -----------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var vm = await GetIngresoSalidaViewModelForDisplay(id.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // ----------------------------- 3. CREATE -----------------------------

        // GET: IngresoSalidaAlm/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            PopulateTipoMovimientoList();

            var estadoInicial = await _context.Estados.FirstOrDefaultAsync(e => e.tabla == "INGRESOSALIDAALM" && e.nombre.ToLower() == "pendiente");
            // 🛑 CORRECCIÓN: Buscar un documento por código, si 'tabla' no existe 🛑
            var documentoDefecto = await _context.Documentos.FirstOrDefaultAsync(d => d.codigo == "GRI"); // Ejemplo: GRI = Guía de Recepción de Inventario

            var vm = new IngresoSalidaAlmViewModel
            {
                FechaHora = DateTime.Now,
                TipoMovimiento = true, // Por defecto Ingreso
                EstadoId = estadoInicial?.id ?? 1,
                DocumentoId = documentoDefecto?.id ?? 1,
                Detalles = new List<DIngresoSalidaAlmViewModel>()
            };

            return View(vm);
        }

        // POST: IngresoSalidaAlm/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaHora,TipoMovimiento,Nota,ReqInternoId,DocumentoId,EstadoId,OCompraId,Detalles")] IngresoSalidaAlmViewModel vm)
        {
            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                ModelState.AddModelError("Detalles", "El Ingreso/Salida debe tener al menos un producto.");
            }

            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // ... (Retorno de errores JSON)
                }
                await PopulateDropDowns(vm);
                PopulateTipoMovimientoList();
                return View(vm);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ingresoSalida = new IngresoSalidaAlm
                    {
                        fechaHora = vm.FechaHora,
                        tipoMovimiento = vm.TipoMovimiento,
                        nota = vm.Nota,
                        reqinternoId = vm.ReqInternoId.Value,
                        documentoId = vm.DocumentoId,
                        estadoId = vm.EstadoId,
                        ocompraId = vm.OCompraId
                    };

                    _context.IngresoSalidaAlms.Add(ingresoSalida);
                    await _context.SaveChangesAsync();

                    var detalles = vm.Detalles.Select(d => new DIngresoSalidaAlm
                    {
                        ingresoSalidaAlmId = ingresoSalida.id,
                        productoId = d.ProductoId,
                        cantidad = d.Cantidad,
                        observacion = d.Observacion
                    }).ToList();

                    _context.DIngresoSalidaAlms.AddRange(detalles);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Error interno al guardar el Ingreso/Salida de Almacén.", debug = ex.Message });
                    }

                    ModelState.AddModelError("", "Ocurrió un error al guardar los datos.");
                    PopulateTipoMovimientoList();
                    await PopulateDropDowns(vm);
                    return View(vm);
                }
            }
        }

        // ----------------------------- 4. EDIT -----------------------------

        // GET: IngresoSalidaAlm/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var vm = await GetIngresoSalidaViewModelForDisplay(id.Value);
            if (vm == null) return NotFound();
            PopulateTipoMovimientoList();
            await PopulateDropDowns(vm);
            return View(vm);
        }

        // POST: IngresoSalidaAlm/Edit/5 (La lógica de transacciones y AJAX se mantiene)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaHora,TipoMovimiento,Nota,ReqInternoId,DocumentoId,EstadoId,OCompraId,Detalles")] IngresoSalidaAlmViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                ModelState.AddModelError("Detalles", "El Ingreso/Salida debe tener al menos un producto.");
            }

            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // ... (Retorno de errores JSON)
                }
                await PopulateDropDowns(vm);
                PopulateTipoMovimientoList();
                return View(vm);
            }

            // ... (Lógica de transacción y persistencia para Edit) ...
            var strategy = _context.Database.CreateExecutionStrategy();
            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var ingresoSalidaToUpdate = await _context.IngresoSalidaAlms.FindAsync(id);
                            if (ingresoSalidaToUpdate == null) return;

                            // Actualizar Cabecera
                            ingresoSalidaToUpdate.fechaHora = vm.FechaHora;
                            ingresoSalidaToUpdate.tipoMovimiento = vm.TipoMovimiento;
                            ingresoSalidaToUpdate.nota = vm.Nota;
                            ingresoSalidaToUpdate.reqinternoId = vm.ReqInternoId.Value;
                            ingresoSalidaToUpdate.documentoId = vm.DocumentoId;
                            ingresoSalidaToUpdate.estadoId = vm.EstadoId;
                            ingresoSalidaToUpdate.ocompraId = vm.OCompraId;

                            _context.Update(ingresoSalidaToUpdate);

                            // Gestión de Detalles (Delete and Re-insert)
                            var oldDetalles = await _context.DIngresoSalidaAlms
                                .Where(d => d.ingresoSalidaAlmId == id)
                                .ToListAsync();
                            _context.DIngresoSalidaAlms.RemoveRange(oldDetalles);

                            var newDetalles = vm.Detalles.Select(d => new DIngresoSalidaAlm
                            {
                                ingresoSalidaAlmId = id,
                                productoId = d.ProductoId,
                                cantidad = d.Cantidad,
                                observacion = d.Observacion
                            }).ToList();

                            _context.DIngresoSalidaAlms.AddRange(newDetalles);

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });

                return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
            }
            catch (Exception ex)
            {
                if (await _context.IngresoSalidaAlms.FindAsync(id) == null) return NotFound();
                return Json(new
                {
                    success = false,
                    message = "Error interno al guardar los datos.",
                    debug = ex.Message
                });
            }
        }


        // ----------------------------- 5. DELETE -----------------------------

        // GET: IngresoSalidaAlm/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var vm = await GetIngresoSalidaViewModelForDisplay(id.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // POST: IngresoSalidaAlm/Delete/5 (La lógica de transacciones se mantiene)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ingresoSalida = await _context.IngresoSalidaAlms.FindAsync(id);

                    if (ingresoSalida != null)
                    {
                        var detalles = await _context.DIngresoSalidaAlms
                                                    .Where(d => d.ingresoSalidaAlmId == id)
                                                    .ToListAsync();
                        _context.DIngresoSalidaAlms.RemoveRange(detalles);
                        _context.IngresoSalidaAlms.Remove(ingresoSalida);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}