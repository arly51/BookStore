using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BLL.Services
{
    public interface IBookService : IService<Book, BookModel>
    {
    }

    public class BookService : ServiceBase, IBookService
    {
        private readonly Db _db;

        public BookService(Db db) : base(db)
        {
            _db = db;
        }

        public IQueryable<BookModel> Query()
        {
            return _db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .OrderBy(b => b.Name)
                .Select(b => new BookModel
                {
                    Record = b,
                    AuthorName = b.Author.Name + " " + b.Author.Surname,
                    GenresText = string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name)),
                    GenreIds = b.BookGenres.Select(bg => bg.GenreId).ToList()
                });
        }

        public ServiceBase Create(Book record)
        {
            try
            {
                if (_db.Books.Any(b => b.Name.ToUpper() == record.Name.ToUpper().Trim()))
                    return Error("Book with this name already exists!");

                using var transaction = _db.Database.BeginTransaction();
                try
                {
                    record.Name = record.Name.Trim();
                    _db.Books.Add(record);
                    _db.SaveChanges();

                    transaction.Commit();
                    return Success("Book created successfully.");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Error($"Book creation failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Update(Book record)
        {
            try
            {
                if (_db.Books.Any(b => b.Name.ToUpper() == record.Name.ToUpper().Trim()
                    && b.Id != record.Id))
                    return Error("Book with this name already exists!");

                using var transaction = _db.Database.BeginTransaction();
                try
                {
                    var book = _db.Books
                        .Include(b => b.BookGenres)
                        .SingleOrDefault(b => b.Id == record.Id);

                    if (book == null)
                        return Error("Book not found!");

                    // Update basic properties
                    book.Name = record.Name.Trim();
                    book.NumberOfPages = record.NumberOfPages;
                    book.PublishDate = record.PublishDate;
                    book.Price = record.Price;
                    book.IsTopSeller = record.IsTopSeller;
                    book.AuthorId = record.AuthorId;

                    // Update genres if provided
                    if (record.BookGenres != null)
                    {
                        // Remove existing genres
                        _db.BookGenres.RemoveRange(book.BookGenres);

                        // Add new genres
                        book.BookGenres = record.BookGenres;
                    }

                    _db.Books.Update(book);
                    _db.SaveChanges();

                    transaction.Commit();
                    return Success("Book updated successfully.");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Error($"Book update failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Delete(int id)
        {
            try
            {
                using var transaction = _db.Database.BeginTransaction();
                try
                {
                    var book = _db.Books
                        .Include(b => b.BookGenres)
                        .SingleOrDefault(b => b.Id == id);

                    if (book == null)
                        return Error("Book not found!");

                    // Remove associated genres first
                    _db.BookGenres.RemoveRange(book.BookGenres);

                    // Then remove the book
                    _db.Books.Remove(book);
                    _db.SaveChanges();

                    transaction.Commit();
                    return Success("Book deleted successfully.");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Error($"Book deletion failed! Error: {ex.Message}");
            }
        }
    }
}