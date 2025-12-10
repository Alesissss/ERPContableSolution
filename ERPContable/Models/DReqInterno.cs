using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    public class DReqInterno
    {
        [Column("reqinterno_id")]
        public int reqinternoId { get; set; }
        [Column("producto_id")]
        public int productoId { get; set; }

    }
}
