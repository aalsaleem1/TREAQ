using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public string? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }

        public string? EditId { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
