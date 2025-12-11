using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("AREA")]
    public class Area
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        // Empresa
        [Column("empresa_id")]
        public int empresaId { get; set; }
    }
}
