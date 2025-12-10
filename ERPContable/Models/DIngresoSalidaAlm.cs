namespace ERPContable.Models
{
    public class DIngresoSalidaAlm
    {
        public int ingresoSalidaAlmId { get; set; }
        public int productoId { get; set; }
        public decimal cantidad { get; set; }
        public string observacion { get; set; }
    }
}
