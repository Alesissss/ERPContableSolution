using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("DCOMPRA")]
    public class DCompra
    {
        // Claves compuestas
        [Column("producto_id")]
        public int productoId { get; set; }

        [Column("ocompra_id")]
        public int ocompraId { get; set; }

        // Datos del detalle
        public decimal cantidad { get; set; }
        public decimal precio { get; set; }
        public string? observacion { get; set; }

        // --- PROPIEDADES DE NAVEGACIÓN ---

        // Referencia al producto para obtener su nombre/código
        [ForeignKey("productoId")]
        public Producto Producto { get; set; } = null!; // Asume que la clase Producto existe

        // Referencia a la cabecera OCompra
        [ForeignKey("ocompraId")]
        public OCompra OCompra { get; set; } = null!;
    }
}