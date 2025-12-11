using System.ComponentModel.DataAnnotations;

namespace ERPContable.ViewModels.IngresoSalidaAlm
{
    public class IngresoSalidaAlmViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHora { get; set; }

        [Display(Name = "Nota / Observaciones")]
        [StringLength(500, ErrorMessage = "La nota no debe exceder los 500 caracteres.")]
        public string? Nota { get; set; }

        // Relaciones obligatorias
        [Required(ErrorMessage = "El Requerimiento Interno es obligatorio.")]
        [Display(Name = "Requerimiento Interno")]
        public int? ReqInternoId { get; set; }

        [Required(ErrorMessage = "El Documento es obligatorio.")]
        [Display(Name = "Tipo de Documento")]
        public int DocumentoId { get; set; }

        [Required]
        public int EstadoId { get; set; }

        // Relación opcional
        [Display(Name = "Orden de Compra Asociada")]
        public int? OCompraId { get; set; }

        // Propiedades de Solo Lectura para Display/Index
        public string? ReqInternoNombre { get; set; }
        public string? DocumentoNombre { get; set; }
        public string? EstadoNombre { get; set; }
        // Detalles
        [Display(Name = "Detalles de Productos")]
        public List<DIngresoSalidaAlmViewModel>? Detalles { get; set; }

        // Propiedades calculadas (para Index/Details)
        public decimal TotalArticulos => Detalles?.Sum(d => d.Cantidad) ?? 0;
    }
}
