using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
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
        public string estadoId { get; set; }
    }
}
