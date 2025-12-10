using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    public class Personal
    {
        public int id { get; set; }
        [Column("apellido_paterno")]
        public string apellidoPaterno { get; set; }
        [Column("apellido_materno")]
        public string apellidoMaterno { get; set; }
        public string nombres { get; set; }
        public string dni { get; set; }
        public bool sexo { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
