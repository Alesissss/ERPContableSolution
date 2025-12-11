using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("DINGRESOSALIDAALM")]
    public class DIngresoSalidaAlm
    {
        // Claves Foráneas (Asumo clave compuesta: IngresoSalidaAlmId + ProductoId)
        [Column("ingresosalidaalm_id")] // Nombre sugerido para FK
        public int ingresoSalidaAlmId { get; set; }

        [Column("producto_id")]
        public int productoId { get; set; }

        public decimal cantidad { get; set; }
        public string? observacion { get; set; }

        // 🛑 PROPIEDADES DE NAVEGACIÓN 🛑
        public virtual IngresoSalidaAlm? IngresoSalidaAlm { get; set; }
        public virtual Producto? Producto { get; set; }
    }
}