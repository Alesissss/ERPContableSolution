using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("UMEDIDA")]
    public class Umedida
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string abreviatura { get; set; }
    }
}
