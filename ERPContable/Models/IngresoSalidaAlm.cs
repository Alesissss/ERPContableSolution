using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    public class IngresoSalidaAlm
    {
        public int id { get; set; }
        [Column("fecha_hora")]
        public DateTime fechaHora { get; set; }
        public string nota { get; set; }
        [Column("reqinterno_id")]
        public int reqinternoId { get; set; }
        [Column("documento_id")]
        public int documentoId { get; set; }
        [Column("estado_id")]
        public int estadoId { get; set; }
        [Column("ocompra_id")]
        public int ocompraId { get; set; }
    }
}
