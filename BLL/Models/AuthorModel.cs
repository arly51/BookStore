// AuthorModel.cs
using BLL.DAL;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BLL.Models
{
    public class AuthorModel
    {
        public AuthorModel()
        {
            Record = new Author();
        }

        public Author Record { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        [Display(Name = "Author Name")]
        public string Name
        {
            get => Record.Name;
            set => Record.Name = value;
        }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(100)]
        public string Surname
        {
            get => Record.Surname;
            set => Record.Surname = value;
        }

        // Property to display all books by this author
        public string BooksText { get; set; }
    }
}