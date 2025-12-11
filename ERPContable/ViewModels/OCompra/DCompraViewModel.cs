using System.ComponentModel.DataAnnotations;

namespace ERPContable.ViewModels.OCompra
{
    public class DCompraViewModel
    {
        // Propiedades de Edición (Mandatorias)
        [Required(ErrorMessage = "El ID del producto es requerido.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(0.01, 99999.99, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El precio es requerido.")]
        [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal Precio { get; set; }

        public string? Observacion { get; set; }

        // Propiedades de SOLO LECTURA (usadas para la vista, deben ser anulables)
        // Eliminamos [Required] y usamos string? para evitar fallos 400.
        public string? ProductoCodigo { get; set; }
        public string? ProductoNombre { get; set; }
    }
}