using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    public class Producto
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal? stock { get; set; }
        [Column("umedida_id")]
        public int umedidaId { get; set; }
    }
}
