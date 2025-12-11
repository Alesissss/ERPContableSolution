using System.ComponentModel.DataAnnotations;

namespace ERPContable.ViewModels.OCompra
{
    public class OCompraViewModel
    {
        // Propiedades del Encabezado (Maestro)
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha y hora son requeridas.")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        public string? Nota { get; set; } // Anulable en DB

        // IDs de las Entidades Relacionadas
        [Required(ErrorMessage = "El Proveedor es requerido.")]
        public int? ProveedorId { get; set; } // Lo hacemos anulable si el dropdown puede enviar ""

        // IDs que vienen de la BD (pueden ser opcionales o fijos en la edición)
        // Asumo que estos ID ya vienen cargados y son obligatorios
        public int? ReqInternoId { get; set; }
        public int DocumentoId { get; set; }
        public int EstadoId { get; set; }

        // Lista de Detalles
        [MinLength(1, ErrorMessage = "La Orden de Compra debe tener al menos un producto.")]
        public List<DCompraViewModel> Detalles { get; set; } = new List<DCompraViewModel>();

        // Propiedades de SOLO LECTURA (anulables para evitar el error 400)
        public string? ProveedorNombre { get; set; }
        public string? RequerimientoCodigo { get; set; } // Si hay un campo código
        public string? DocumentoCodigo { get; set; }
        public string? EstadoNombre { get; set; }
    }
}