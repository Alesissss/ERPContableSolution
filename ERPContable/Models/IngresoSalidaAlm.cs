using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("INGRESOSALIDAALM")]
    public class IngresoSalidaAlm
    {
        public int id { get; set; }

        [Column("fecha_hora")]
        public DateTime fechaHora { get; set; }

        public string? nota { get; set; }

        // Claves Foráneas
        [Column("reqinterno_id")]
        public int reqinternoId { get; set; }

        [Column("documento_id")]
        public int documentoId { get; set; }

        [Column("estado_id")]
        public int estadoId { get; set; }

        [Column("ocompra_id")]
        public int? ocompraId { get; set; } // Anulable

        // 🛑 PROPIEDADES DE NAVEGACIÓN (MAESTROS) 🛑
        public virtual ReqInterno? ReqInterno { get; set; }
        public virtual Documento? Documento { get; set; }
        public virtual Estado? Estado { get; set; }
        public virtual OCompra? OCompra { get; set; }

        // 🛑 PROPIEDAD DE NAVEGACIÓN (DETALLES) 🛑
        public virtual ICollection<DIngresoSalidaAlm> DIngresoSalidaAlms { get; set; } = new List<DIngresoSalidaAlm>();
    }
}