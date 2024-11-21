using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using System;
using System.Linq;

namespace BLL.Services
{
    public interface IRolesService : IService<Role, RoleModel>
    {
    }

    public class RolesService : ServiceBase, IRolesService
    {
        private readonly Db _db;

        public RolesService(Db db) : base(db)
        {
            _db = db;
        }

        public IQueryable<RoleModel> Query()
        {
            return _db.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleModel
                {
                    Record = r
                });
        }

        public ServiceBase Create(Role record)
        {
            try
            {
                if (_db.Roles.Any(r => r.Name.ToUpper() == record.Name.ToUpper().Trim()))
                    return Error("Role with this name already exists!");

                record.Name = record.Name.Trim();
                _db.Roles.Add(record);
                _db.SaveChanges();

                return Success("Role created successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Role creation failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Update(Role record)
        {
            try
            {
                if (_db.Roles.Any(r => r.Name.ToUpper() == record.Name.ToUpper().Trim()
                    && r.Id != record.Id))
                    return Error("Role with this name already exists!");

                var role = _db.Roles.Find(record.Id);
                if (role == null)
                    return Error("Role not found!");

                record.Name = record.Name.Trim();
                role.Name = record.Name;

                _db.Roles.Update(role);
                _db.SaveChanges();

                return Success("Role updated successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Role update failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Delete(int id)
        {
            try
            {
                if (_db.Users.Any(u => u.RoleId == id))
                    return Error("Role cannot be deleted because it is assigned to users!");

                var role = _db.Roles.Find(id);
                if (role == null)
                    return Error("Role not found!");

                _db.Roles.Remove(role);
                _db.SaveChanges();

                return Success("Role deleted successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Role deletion failed! Error: {ex.Message}");
            }
        }
    }
}