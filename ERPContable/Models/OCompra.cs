using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Asegúrate de incluir esto

namespace ERPContable.Models
{
    [Table("OCOMPRA")]
    public class OCompra
    {
        public int id { get; set; }

        [Column("fecha_hora")]
        public DateTime fechaHora { get; set; }

        public string? nota { get; set; }

        [Column("reqinterno_id")]
        public int? reqinternoId { get; set; }

        [Column("documento_id")]
        public int documentoId { get; set; }

        [Column("estado_id")]
        public int estadoId { get; set; }

        [Column("proveedor_id")]
        public int proveedorId { get; set; }

        // --- PROPIEDADES DE NAVEGACIÓN ---

        // Referencia a Proveedor
        [ForeignKey("proveedorId")]
        public Proveedor Proveedor { get; set; } = null!; // Asume que la clase Proveedor existe

        // Referencia a Estado
        [ForeignKey("estadoId")]
        public Estado Estado { get; set; } = null!; // Asume que la clase Estado existe

        // Referencia a Documento
        [ForeignKey("documentoId")]
        public Documento Documento { get; set; } = null!; // Asume que la clase Documento existe

        // Referencia al Requerimiento Interno
        [ForeignKey("reqinternoId")]
        public ReqInterno ReqInterno { get; set; } = null!; // Asume que la clase ReqInterno existe

        // Colección de Detalles (DCompra)
        public ICollection<DCompra> DCompras { get; set; } = new List<DCompra>();
    }
}