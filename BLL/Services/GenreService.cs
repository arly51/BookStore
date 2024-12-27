// GenreService.cs in BLL.Services
using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BLL.Services
{
    public interface IGenreService : IService<Genre, GenreModel>
    {
    }

    public class GenreService : ServiceBase, IGenreService
    {
        private readonly Db _db;

        public GenreService(Db db) : base(db)
        {
            _db = db;
        }

        public IQueryable<GenreModel> Query()
        {
            return _db.Genres
                .Include(g => g.BookGenres)
                .ThenInclude(bg => bg.Book)
                .OrderBy(g => g.Name)
                .Select(g => new GenreModel
                {
                    Record = g,
                    BooksText = string.Join(", ", g.BookGenres.Select(bg => bg.Book.Name))
                });
        }

        public ServiceBase Create(Genre record)
        {
            try
            {
                if (_db.Genres.Any(g => g.Name.ToUpper() == record.Name.ToUpper().Trim()))
                    return Error("Genre with this name already exists!");

                record.Name = record.Name.Trim();
                _db.Genres.Add(record);
                _db.SaveChanges();

                return Success("Genre created successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Genre creation failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Update(Genre record)
        {
            try
            {
                if (_db.Genres.Any(g => g.Name.ToUpper() == record.Name.ToUpper().Trim()
                    && g.Id != record.Id))
                    return Error("Genre with this name already exists!");

                var genre = _db.Genres.Find(record.Id);
                if (genre == null)
                    return Error("Genre not found!");

                genre.Name = record.Name.Trim();

                _db.Genres.Update(genre);
                _db.SaveChanges();

                return Success("Genre updated successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Genre update failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Delete(int id)
        {
            try
            {
                var genre = _db.Genres
                    .Include(g => g.BookGenres)
                    .SingleOrDefault(g => g.Id == id);

                if (genre == null)
                    return Error("Genre not found!");

                if (genre.BookGenres.Any())
                    return Error("Cannot delete genre because it has associated books!");

                _db.Genres.Remove(genre);
                _db.SaveChanges();

                return Success("Genre deleted successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Genre deletion failed! Error: {ex.Message}");
            }
        }
    }
}