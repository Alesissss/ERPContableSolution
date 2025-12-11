using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("REQINTERNO")]
    public class ReqInterno
    {
        public int id { get; set; }
        [Column("fecha_hora")]
        public DateTime fechaHora { get; set; }
        public string nota { get; set; }
        [Column("personal_id")]
        public int personalId { get; set; }
        [Column("area_id")]
        public int areaId { get; set; }
        [Column("documento_id")]
        public int documentoId { get; set; }
        [Column("estado_id")]
        public int estadoId { get; set; }
        // ***** PROPIEDAD DE NAVEGACIÓN A DETALLE (IMPRESCINDIBLE PARA DETAILS/DELETE EFICIENTE) *****
        public ICollection<DReqInterno> DReqInterno { get; set; } = new List<DReqInterno>();
    }
}
