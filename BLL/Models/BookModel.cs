using BLL.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class BookModel
    {
        public BookModel()
        {
            Record = new Book();
        }

        public Book Record { get; set; }

        // Display properties
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255)]
        public string Name
        {
            get => Record.Name;
            set => Record.Name = value;
        }

        [Display(Name = "Number of Pages")]
        public short? NumberOfPages
        {
            get => Record.NumberOfPages;
            set => Record.NumberOfPages = value;
        }

        [Display(Name = "Publish Date")]
        [DataType(DataType.Date)]
        public DateTime? PublishDate
        {
            get => Record.PublishDate;
            set => Record.PublishDate = value;
        }

        public decimal Price
        {
            get => Record.Price;
            set => Record.Price = value;
        }

        [Display(Name = "Top Seller")]
        public bool IsTopSeller
        {
            get => Record.IsTopSeller;
            set => Record.IsTopSeller = value;
        }

        [Display(Name = "Author")]
        public int AuthorId
        {
            get => Record.AuthorId;
            set => Record.AuthorId = value;
        }

        // Properties for Many-to-Many relationship with Genre
        public string GenresText { get; set; } // For displaying genres as string
        public List<int> GenreIds { get; set; } // For managing selected genres
        public string AuthorName { get; set; } // For displaying author name
    }
}