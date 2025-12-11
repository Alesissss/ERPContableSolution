using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("DOCUMENTO")]
    public class Documento
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
    }
}
