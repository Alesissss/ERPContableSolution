using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("EMPRESA")]
    public class Empresa
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string razon_social { get; set; }
        public string ruc { get; set; }
    }
}
