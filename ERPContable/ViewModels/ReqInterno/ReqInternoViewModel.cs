namespace ERPContable.ViewModels.ReqInterno
{
    public class ReqInternoViewModel
    {
        // Propiedades del Encabezado (Maestro)
        public int Id { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public string Nota { get; set; }

        // IDs de las Entidades Relacionadas (Usados para DropDownLists)
        public int PersonalId { get; set; }
        public int AreaId { get; set; }
        public int DocumentoId { get; set; }
        // El Estado podría ser asignado automáticamente al crear,
        // pero se mantiene para edición o vista si es necesario
        public int EstadoId { get; set; }

        // Lista de Detalles (Hijo/Colección)
        public List<DReqInternoViewModel> Detalles { get; set; } = new List<DReqInternoViewModel>();

        // Propiedades adicionales para mostrar en la vista
        public string PersonalNombreCompleto { get; set; }
        public string AreaNombre { get; set; }
        public string DocumentoCodigo { get; set; }
        public string EstadoNombre { get; set; }
    }
}
