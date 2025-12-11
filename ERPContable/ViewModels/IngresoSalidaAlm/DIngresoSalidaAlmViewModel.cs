using System.ComponentModel.DataAnnotations;

namespace ERPContable.ViewModels.IngresoSalidaAlm
{
    public class DIngresoSalidaAlmViewModel
    {
        // ID del detalle (se usa para identificar la fila, aunque no es PK)
        public int Id { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        public int ProductoId { get; set; }

        // Propiedad para mostrar en la vista
        public string? ProductoNombre { get; set; }
        public string? ProductoCodigo { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0.01, 999999.99, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public decimal Cantidad { get; set; }

        public string? Observacion { get; set; }
    }
}
