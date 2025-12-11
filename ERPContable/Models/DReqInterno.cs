using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("DREQINTERNO")]
    public class DReqInterno
    {
        [Column("reqinterno_id")]
        public int reqinternoId { get; set; }
        [Column("producto_id")]
        public int productoId { get; set; }
        public decimal cantidad { get; set; }
        public string? observacion { get; set; }
        // ***** PROPIEDAD DE NAVEGACIÓN A PRODUCTO (PARA OBTENER EL NOMBRE) *****
        [ForeignKey("productoId")]
        public Producto Producto { get; set; }
        [ForeignKey("reqinternoId")]
        public ReqInterno ReqInterno { get; set; }
    }
}
