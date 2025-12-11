using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("PROVEEDOR")]
    public class Proveedor
    {
        public int id { get; set; }
        public string nombre { get; set; }
        [Column("razon_social")]
        public string razonSocial { get; set; }
        public string ruc { get; set; }
    }
}
