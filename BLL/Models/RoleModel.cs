using BLL.DAL;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class RoleModel
    {
        public RoleModel()
        {
            Record = new Role();
        }

        public Role Record { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50)]
        [Display(Name = "Role Name")]
        public string Name
        {
            get => Record.Name;
            set => Record.Name = value;
        }
    }
}