using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("PROVEEDOR")]
    public class Proveedor
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string razon_social { get; set; }
        public string ruc { get; set; }
    }
}
