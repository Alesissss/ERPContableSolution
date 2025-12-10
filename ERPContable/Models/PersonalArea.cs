using System.ComponentModel.DataAnnotations.Schema;

namespace ERPContable.Models
{
    [Table("personal_area")]
    public class PersonalArea
    {
        [Column("personal_id")]
        public int personalId { get; set; }
        [Column("area_id")]
        public int areaId { get; set; }
    }
}
