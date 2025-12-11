using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("DINGRESOSALIDAALM")]
    public class DIngresoSalidaAlm
    {
        public int ingresoSalidaAlmId { get; set; }
        public int productoId { get; set; }
        public decimal cantidad { get; set; }
        public string observacion { get; set; }
    }
}
