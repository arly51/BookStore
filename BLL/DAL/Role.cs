using System.ComponentModel.DataAnnotations;

namespace BLL.DAL
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}