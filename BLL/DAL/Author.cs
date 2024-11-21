using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DAL
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}