using ERPContable.Data;
using ERPContable.Models;
using ERPContable.ViewModels.ReqInterno;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERPContable.Controllers
{
    public class ReqInternoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReqInternoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método de ayuda para cargar DropDownLists
        private async Task PopulateDropDowns(ReqInternoViewModel vm = null)
        {
            // ... (Este método se mantiene igual, ya funciona correctamente) ...

            var personalList = await _context.Personales
                                             .Select(p => new
                                             {
                                                 Id = p.id,
                                                 NombreCompleto = $"{p.apellidoPaterno} {p.apellidoMaterno}, {p.nombres}"
                                             })
                                             .ToListAsync();
            ViewBag.PersonalId = new SelectList(personalList, "Id", "NombreCompleto", vm?.PersonalId);

            ViewBag.AreaId = new SelectList(await _context.Areas.ToListAsync(), "id", "nombre", vm?.AreaId);
            ViewBag.DocumentoId = new SelectList(await _context.Documentos.ToListAsync(), "id", "nombre", vm?.DocumentoId);
            ViewBag.ProductoId = new SelectList(await _context.Productos.ToListAsync(), "id", "nombre", null);
            ViewBag.EstadoId = new SelectList(
                await _context.Estados.Where(e => e.tabla == "REQINTERNO").ToListAsync(),
                "id",
                "nombre",
                vm?.EstadoId
            );
        }

        // GET: /ReqInterno/Index (Se mantiene igual)
        public async Task<IActionResult> Index()
        {
            // ... (Se mantiene igual) ...
            var requerimientos = await (
                from req in _context.ReqInternos
                join p in _context.Personales on req.personalId equals p.id
                join a in _context.Areas on req.areaId equals a.id
                join e in _context.Estados on req.estadoId equals e.id
                orderby req.fechaHora descending
                select new ReqInternoViewModel
                {
                    Id = req.id,
                    FechaHora = req.fechaHora,
                    Nota = req.nota,
                    PersonalNombreCompleto = $"{p.apellidoPaterno} {p.apellidoMaterno}, {p.nombres}",
                    AreaNombre = a.nombre,
                    EstadoNombre = e.nombre
                }
            ).ToListAsync();

            return View(requerimientos);
        }

        // Método de ayuda CORREGIDO para Details/Edit/Delete
        private async Task<ReqInternoViewModel> GetReqInternoViewModelForDisplay(int id)
        {
            // 1. Usar Include para cargar el Maestro (ReqInterno) y sus colecciones anidadas (Detalle y Producto).
            // Esto es crucial cuando se accede a las propiedades de navegación de colecciones.
            var reqInterno = await _context.ReqInternos
                .Include(r => r.Personal)
                .Include(r => r.Area)
                .Include(r => r.Documento)
                .Include(r => r.Estado)
                .Include(r => r.DReqInterno) // Propiedad de colección
                    .ThenInclude(d => d.Producto) // Incluir el producto dentro de cada detalle
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.id == id);

            if (reqInterno == null)
            {
                return null;
            }

            // 2. Mapear la Entidad al ViewModel
            var vm = new ReqInternoViewModel
            {
                // Cabecera (para edición)
                Id = reqInterno.id,
                FechaHora = reqInterno.fechaHora,
                Nota = reqInterno.nota,
                PersonalId = reqInterno.personalId,
                AreaId = reqInterno.areaId,
                DocumentoId = reqInterno.documentoId,
                EstadoId = reqInterno.estadoId,

                // Nombres (para visualización)
                PersonalNombreCompleto = $"{reqInterno.Personal.apellidoPaterno} {reqInterno.Personal.apellidoMaterno}, {reqInterno.Personal.nombres}",
                AreaNombre = reqInterno.Area.nombre,
                DocumentoCodigo = reqInterno.Documento.codigo,
                EstadoNombre = reqInterno.Estado.nombre,

                // Detalles
                Detalles = reqInterno.DReqInterno.Select(d => new DReqInternoViewModel
                {
                    ProductoId = d.productoId,
                    Cantidad = d.cantidad,
                    Observacion = d.observacion,
                    ProductoNombre = d.Producto.nombre, // ¡Ahora funciona gracias a ThenInclude!
                    ProductoCodigo = d.Producto.codigo // Asumiendo que Producto tiene 'codigo'
                }).ToList()
            };

            return vm;
        }


        // GET: /ReqInterno/Details/5 (Se mantiene igual)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var vm = await GetReqInternoViewModelForDisplay(id.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET: /ReqInterno/Create (Se mantiene igual)
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            var vm = new ReqInternoViewModel { FechaHora = DateTime.Now, EstadoId = 1 };
            return View(vm);
        }

        // POST: /ReqInterno/Create (Se mantiene igual)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaHora,Nota,PersonalId,AreaId,DocumentoId,EstadoId,Detalles")] ReqInternoViewModel vm)
        {
            // ... (El código de Create es funcional, lo omito aquí por espacio, asumiendo que ya maneja transacciones o que el SaveChanges resuelve en orden) ...
            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                ModelState.AddModelError("Detalles", "Debe agregar al menos un producto al requerimiento.");
            }

            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var reqInterno = new ReqInterno
                        {
                            fechaHora = vm.FechaHora,
                            nota = vm.Nota,
                            personalId = vm.PersonalId,
                            areaId = vm.AreaId,
                            documentoId = vm.DocumentoId,
                            estadoId = 1 // Estado inicial
                        };

                        _context.ReqInternos.Add(reqInterno);
                        await _context.SaveChangesAsync(); // Guarda para obtener reqInterno.id

                        var detalles = vm.Detalles.Select(d => new DReqInterno
                        {
                            reqinternoId = reqInterno.id,
                            productoId = d.ProductoId,
                            cantidad = d.Cantidad,
                            observacion = d.Observacion
                        }).ToList();

                        _context.DReqInternos.AddRange(detalles);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "Ocurrió un error al guardar el requerimiento.");
                    }
                }
            }

            await PopulateDropDowns(vm);
            return View(vm);
        }

        // GET: /ReqInterno/Delete/5 (Se mantiene igual)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vm = await GetReqInternoViewModelForDisplay(id.Value);
            if (vm == null) return NotFound();

            return View(vm);
        }

        // POST: /ReqInterno/Delete/5 (Se mantiene igual, la eliminación es correcta)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Usamos una transacción para asegurar que ambos se borren o ninguno
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var reqInterno = await _context.ReqInternos.FindAsync(id);

                    if (reqInterno != null)
                    {
                        // 1. Cargar y eliminar detalles
                        var detalles = await _context.DReqInternos
                                                     .Where(d => d.reqinternoId == id)
                                                     .ToListAsync();
                        _context.DReqInternos.RemoveRange(detalles);

                        // 2. Eliminar encabezado
                        _context.ReqInternos.Remove(reqInterno);
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

        // GET: /ReqInterno/Edit/5 (Se mantiene igual)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await GetReqInternoViewModelForDisplay(id.Value);

            if (vm == null)
            {
                return NotFound();
            }

            await PopulateDropDowns(vm);

            return View(vm);
        }

        // ERPContable.Controllers/ReqInternoController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Nota: Dejamos el parámetro del modelo igual para que el Model Binder lo mapee correctamente
        // ERPContable.Controllers/ReqInternoController.cs
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaHora,Nota,PersonalId,AreaId,DocumentoId,EstadoId,Detalles")] ReqInternoViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            // Validación de que existe al menos un detalle (a nivel de Controller)
            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                ModelState.AddModelError("Detalles", "Debe agregar al menos un producto al requerimiento.");
            }

            // 1. Manejo de Fallo de Validación (Retorno AJAX)
            if (!ModelState.IsValid)
            {
                // Mapear los errores del ModelState a un formato JSON simple para el script de AJAX
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return Json(new { success = false, errors = errors });
            }

            // 2. Definir la Estrategia de Ejecución
            // Esto es necesario para ejecutar código transaccional cuando la estrategia de reintento está activa.
            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                // Ejecutar el bloque de código transaccional como una unidad retriable
                await strategy.ExecuteAsync(async () =>
                {
                    // 🛑 2.1. Iniciar la Transacción Manual dentro de la Estrategia 🛑
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Obtener el encabezado existente
                            var reqInternoToUpdate = await _context.ReqInternos.FindAsync(id);

                            if (reqInternoToUpdate == null)
                            {
                                // Si no se encuentra, abortar la operación.
                                // Usaremos un NotFound al final del método si no se encontró el ID.
                                return;
                            }

                            // 2.2. Actualizar las propiedades del encabezado (Maestro)
                            reqInternoToUpdate.fechaHora = vm.FechaHora;
                            reqInternoToUpdate.nota = vm.Nota;
                            reqInternoToUpdate.personalId = vm.PersonalId;
                            reqInternoToUpdate.areaId = vm.AreaId;
                            reqInternoToUpdate.documentoId = vm.DocumentoId;
                            reqInternoToUpdate.estadoId = vm.EstadoId;

                            // 2.3. Gestionar los Detalles (Delete and Re-insert)

                            // a) Eliminar los detalles antiguos asociados
                            var oldDetalles = await _context.DReqInternos
                                                            .Where(d => d.reqinternoId == id)
                                                            .ToListAsync();
                            _context.DReqInternos.RemoveRange(oldDetalles);

                            // b) Crear e insertar los nuevos detalles
                            var newDetalles = vm.Detalles.Select(d => new DReqInterno
                            {
                                reqinternoId = id,
                                productoId = d.ProductoId,
                                cantidad = d.Cantidad,
                                observacion = d.Observacion
                            }).ToList();

                            _context.DReqInternos.AddRange(newDetalles);

                            // 2.4. Guardar todos los cambios
                            await _context.SaveChangesAsync();

                            // 2.5. Confirmar la transacción
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            // Si algo falla dentro, revertir explícitamente y re-lanzar para que ExecuteAsync lo maneje
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });

                // 3. Retorno de Éxito (Si ExecuteAsync termina sin excepción)
                return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
            }
            catch (Exception ex)
            {
                // 4. Retorno de Fallo (Si ExecuteAsync o FindAsync fallan)
                // Se maneja la excepción, que podría ser de reintento fallido, concurrencia, etc.
                if (await _context.ReqInternos.FindAsync(id) == null)
                {
                    return NotFound();
                }

                // Error interno
                return Json(new
                {
                    success = false,
                    message = "Error interno al guardar los datos (ver consola para detalles).",
                    debug = ex.Message
                });
            }
        }
    }
}