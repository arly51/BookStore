using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DAL
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<BookGenre> BookGenres { get; set; }
    }
}