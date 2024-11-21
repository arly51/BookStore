using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.DAL
{
    public class BookGenre
    {
        [Key]
        public int Id { get; set; }

        public int BookId { get; set; }

        public int GenreId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }
    }
}