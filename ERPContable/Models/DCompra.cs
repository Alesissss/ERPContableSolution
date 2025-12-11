using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("DCOMPRA")]
    public class DCompra
    {
        [Column("producto_id")]
        public int productoId { get; set; }
        [Column("ocompra_id")]
        public int ocompraId { get; set; }
        public decimal cantidad { get; set; }
        public decimal precio { get; set; }
        public string? observacion { get; set; }
    }
}
