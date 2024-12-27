using BLL.DAL;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class GenreModel
    {
        public GenreModel()
        {
            Record = new Genre();
        }

        public Genre Record { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        [Display(Name = "Genre Name")]
        public string Name
        {
            get => Record.Name;
            set => Record.Name = value;
        }

        // Property to display related books
        public string BooksText { get; set; }
    }
}