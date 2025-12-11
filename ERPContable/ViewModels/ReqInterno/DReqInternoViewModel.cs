namespace ERPContable.ViewModels.ReqInterno
{
    public class DReqInternoViewModel
    {
        // Propiedades de la tabla de detalle
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public string? Observacion { get; set; }

        // Propiedades adicionales para la vista (ej: Nombre del Producto)
        public string? ProductoNombre { get; set; }
        public string? ProductoCodigo { get; set; }
    }
}
