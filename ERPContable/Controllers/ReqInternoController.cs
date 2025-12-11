using ERPContable.Data;
using ERPContable.Models;
using ERPContable.ViewModels;
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
            // La carga de DropDownLists (ViewBag) es independiente de las propiedades de navegación.
            // Se usa SELECT para crear el valor y el texto de la lista.

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

        // GET: /ReqInterno/Index
        public async Task<IActionResult> Index()
        {
            // Usamos LINQ Join para obtener datos de tablas relacionadas
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

        // Método de ayuda para Details/Delete (usa Joins)
        private async Task<ReqInternoViewModel> GetReqInternoViewModelForDisplay(int id)
        {
            var vm = await (
                from req in _context.ReqInternos
                where req.id == id
                join p in _context.Personales on req.personalId equals p.id
                join a in _context.Areas on req.areaId equals a.id
                join doc in _context.Documentos on req.documentoId equals doc.id
                join est in _context.Estados on req.estadoId equals est.id
                select new ReqInternoViewModel
                {
                    Id = req.id,
                    FechaHora = req.fechaHora,
                    Nota = req.nota,
                    PersonalNombreCompleto = $"{p.apellidoPaterno} {p.apellidoMaterno}, {p.nombres}",
                    AreaNombre = a.nombre,
                    DocumentoCodigo = doc.codigo,
                    EstadoNombre = est.nombre,
                    // Cargar detalles usando la propiedad de navegación (si la agregaste)
                    Detalles = req.DReqInterno.Select(d => new DReqInternoViewModel
                    {
                        ProductoId = d.productoId,
                        Cantidad = d.cantidad,
                        Observacion = d.observacion,
                        ProductoNombre = d.Producto.nombre // Necesita navegación en DReqInterno
                    }).ToList()
                }
            ).FirstOrDefaultAsync();

            return vm;
        }

        // GET: /ReqInterno/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var vm = await GetReqInternoViewModelForDisplay(id.Value);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // GET: /ReqInterno/Create (No cambia)
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            var vm = new ReqInternoViewModel { FechaHora = DateTime.Now, EstadoId = 1 };
            return View(vm);
        }
        // GET: /ReqInterno/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Error si falta ID
            }

            // Asegúrate de que esta llamada traiga datos y no sea nula.
            var vm = await GetReqInternoViewModelForDisplay(id.Value);

            if (vm == null)
            {
                return NotFound(); // Error si el ID no existe en la BD
            }

            // Asegúrate de que este método exista y esté bien escrito.
            await PopulateDropDowns(vm);

            return View(vm); // Busca Views/ReqInterno/Edit.cshtml
        }

        // POST: /ReqInterno/Create (No cambia, ya que solo interactúa con los modelos principales)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaHora,Nota,PersonalId,AreaId,DocumentoId,EstadoId,Detalles")] ReqInternoViewModel vm)
        {
            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                ModelState.AddModelError("Detalles", "Debe agregar al menos un producto al requerimiento.");
            }

            if (ModelState.IsValid)
            {
                var reqInterno = new ReqInterno
                {
                    fechaHora = vm.FechaHora,
                    nota = vm.Nota,
                    personalId = vm.PersonalId,
                    areaId = vm.AreaId,
                    documentoId = vm.DocumentoId,
                    estadoId = 1
                };

                _context.ReqInternos.Add(reqInterno);
                await _context.SaveChangesAsync();

                // 2. Mapear los Detalles (Hijo)
                var detalles = vm.Detalles.Select(d => new DReqInterno
                {
                    reqinternoId = reqInterno.id,
                    productoId = d.ProductoId,
                    cantidad = d.Cantidad,
                    observacion = d.Observacion
                }).ToList();

                _context.DReqInternos.AddRange(detalles);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDowns(vm);
            return View(vm);
        }

        // GET: /ReqInterno/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vm = await GetReqInternoViewModelForDisplay(id.Value);
            if (vm == null) return NotFound();

            return View(vm);
        }

        // POST: /ReqInterno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reqInterno = await _context.ReqInternos.FindAsync(id);

            if (reqInterno != null)
            {
                // Ahora, dado que agregaste ICollection<DReqInterno> al modelo, 
                // podemos cargar el detalle para eliminarlo si la DB no está en cascada
                var detalles = await _context.DReqInternos
                                             .Where(d => d.reqinternoId == id)
                                             .ToListAsync();
                _context.DReqInternos.RemoveRange(detalles);

                _context.ReqInternos.Remove(reqInterno);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaHora,Nota,PersonalId,AreaId,DocumentoId,EstadoId,Detalles")] ReqInternoViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            // Validación de que existe al menos un detalle
            if (vm.Detalles == null || !vm.Detalles.Any())
            {
                ModelState.AddModelError("Detalles", "Debe agregar al menos un producto al requerimiento.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Obtener el encabezado existente
                    var reqInternoToUpdate = await _context.ReqInternos.FindAsync(id);

                    if (reqInternoToUpdate == null) return NotFound();

                    // 2. Actualizar las propiedades del encabezado (Maestro)
                    reqInternoToUpdate.fechaHora = vm.FechaHora;
                    reqInternoToUpdate.nota = vm.Nota;
                    reqInternoToUpdate.personalId = vm.PersonalId;
                    reqInternoToUpdate.areaId = vm.AreaId;
                    reqInternoToUpdate.documentoId = vm.DocumentoId;
                    reqInternoToUpdate.estadoId = vm.EstadoId; // Permitir que el estado se cambie si es parte del flujo

                    // 3. Gestionar los Detalles (Hijo)

                    // a) Eliminar los detalles antiguos asociados a este requerimiento
                    var oldDetalles = await _context.DReqInternos
                                                    .Where(d => d.reqinternoId == id)
                                                    .ToListAsync();
                    _context.DReqInternos.RemoveRange(oldDetalles);

                    // b) Crear e insertar los nuevos detalles (los que vinieron del formulario)
                    var newDetalles = vm.Detalles.Select(d => new DReqInterno
                    {
                        reqinternoId = id,
                        productoId = d.ProductoId,
                        cantidad = d.Cantidad,
                        observacion = d.Observacion
                    }).ToList();

                    _context.DReqInternos.AddRange(newDetalles);

                    // 4. Guardar todos los cambios (Maestro, Eliminación de viejos detalles, Creación de nuevos detalles)
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _context.ReqInternos.FindAsync(id) == null)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDowns(vm);
            return View(vm);
        }
    }
}