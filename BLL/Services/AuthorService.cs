// AuthorService.cs
using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BLL.Services
{
    public interface IAuthorService : IService<Author, AuthorModel>
    {
    }

    public class AuthorService : ServiceBase, IAuthorService
    {
        private readonly Db _db;

        public AuthorService(Db db) : base(db)
        {
            _db = db;
        }

        public IQueryable<AuthorModel> Query()
        {
            return _db.Authors
                .Include(a => a.Books)
                .OrderBy(a => a.Surname)
                .ThenBy(a => a.Name)
                .Select(a => new AuthorModel
                {
                    Record = a,
                    BooksText = string.Join(", ", a.Books.Select(b => b.Name))
                });
        }

        public ServiceBase Create(Author record)
        {
            try
            {
                if (_db.Authors.Any(a => a.Name.ToUpper() == record.Name.ToUpper().Trim()
                    && a.Surname.ToUpper() == record.Surname.ToUpper().Trim()))
                    return Error("Author with this name and surname already exists!");

                record.Name = record.Name.Trim();
                record.Surname = record.Surname.Trim();

                _db.Authors.Add(record);
                _db.SaveChanges();

                return Success("Author created successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Author creation failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Update(Author record)
        {
            try
            {
                if (_db.Authors.Any(a => a.Name.ToUpper() == record.Name.ToUpper().Trim()
                    && a.Surname.ToUpper() == record.Surname.ToUpper().Trim()
                    && a.Id != record.Id))
                    return Error("Author with this name and surname already exists!");

                var author = _db.Authors.Find(record.Id);
                if (author == null)
                    return Error("Author not found!");

                author.Name = record.Name.Trim();
                author.Surname = record.Surname.Trim();

                _db.Authors.Update(author);
                _db.SaveChanges();

                return Success("Author updated successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Author update failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Delete(int id)
        {
            try
            {
                var author = _db.Authors
                    .Include(a => a.Books)
                    .SingleOrDefault(a => a.Id == id);

                if (author == null)
                    return Error("Author not found!");

                if (author.Books.Any())
                    return Error("Cannot delete author because they have associated books!");

                _db.Authors.Remove(author);
                _db.SaveChanges();

                return Success("Author deleted successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Author deletion failed! Error: {ex.Message}");
            }
        }
    }
}