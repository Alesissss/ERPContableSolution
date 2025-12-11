using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("ESTADO")]
    public class Estado
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string tabla { get; set; }
    }
}
