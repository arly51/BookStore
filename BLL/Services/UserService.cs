using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using System;
using System.Linq;

namespace BLL.Services
{
    public interface IUserService : IService<User, UserModel>
    {
        ServiceBase Login(string userName, string password);
        ServiceBase ChangePassword(int userId, string oldPassword, string newPassword);
    }

    public class UserService : ServiceBase, IUserService
    {
        private readonly Db _db;

        public UserService(Db db) : base(db)
        {
            _db = db;
        }

        public IQueryable<UserModel> Query()
        {
            // Debug: Print all users in the database
            var allUsers = _db.Users.ToList();
            foreach (var user in allUsers)
            {
                Console.WriteLine($"Found user in DB: ID={user.Id}, Username={user.UserName}, Password={user.Password}, RoleId={user.RoleId}, IsActive={user.IsActive}");
            }

            return _db.Users
                .OrderBy(u => u.UserName)
                .Select(u => new UserModel
                {
                    Record = u,
                    RoleName = u.Role.Name
                });
        }

        public ServiceBase Create(User record)
        {
            try
            {
                if (_db.Users.Any(u => u.UserName.ToUpper() == record.UserName.ToUpper().Trim()))
                    return Error("User with this username already exists!");

                record.UserName = record.UserName.Trim();
                record.IsActive = true;  // Set default value

                _db.Users.Add(record);
                _db.SaveChanges();

                return Success("User created successfully.");
            }
            catch (Exception ex)
            {
                return Error($"User creation failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Update(User record)
        {
            try
            {
                if (_db.Users.Any(u => u.UserName.ToUpper() == record.UserName.ToUpper().Trim()
                    && u.Id != record.Id))
                    return Error("User with this username already exists!");

                var user = _db.Users.Find(record.Id);
                if (user == null)
                    return Error("User not found!");

                user.UserName = record.UserName.Trim();
                user.RoleId = record.RoleId;
                user.IsActive = record.IsActive;

                // Only update password if a new one is provided
                if (!string.IsNullOrWhiteSpace(record.Password))
                {
                    user.Password = record.Password;
                }

                _db.Users.Update(user);
                _db.SaveChanges();

                return Success("User updated successfully.");
            }
            catch (Exception ex)
            {
                return Error($"User update failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Delete(int id)
        {
            try
            {
                var user = _db.Users.Find(id);
                if (user == null)
                    return Error("User not found!");

                user.IsActive = false;
                _db.Users.Update(user);
                _db.SaveChanges();

                return Success("User deactivated successfully.");
            }
            catch (Exception ex)
            {
                return Error($"User deactivation failed! Error: {ex.Message}");
            }
        }

        public ServiceBase Login(string userName, string password)
        {
            try
            {
                Console.WriteLine($"Login attempt - Raw input: Username='{userName}', Password='{password}'");

                // Use case-insensitive comparison
                var user = _db.Users.FirstOrDefault(u =>
                    u.UserName.ToUpper() == userName.ToUpper().Trim());

                if (user != null)
                {
                    Console.WriteLine($"Found user: ID={user.Id}, Username='{user.UserName}', StoredPassword='{user.Password}'");

                    if (user.Password == password)
                    {
                        if (!user.IsActive)
                        {
                            return Error("This account has been deactivated!");
                        }
                        return Success("Login successful.");
                    }
                    Console.WriteLine("Password mismatch");
                }
                else
                {
                    Console.WriteLine("Username not found");
                }

                return Error("Invalid username or password!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex}");
                return Error($"Login failed! Error: {ex.Message}");
            }
        }

        public ServiceBase ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = _db.Users.Find(userId);
                if (user == null)
                    return Error("User not found!");

                if (user.Password != oldPassword)
                    return Error("Current password is incorrect!");

                user.Password = newPassword;
                _db.Users.Update(user);
                _db.SaveChanges();

                return Success("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return Error($"Password change failed! Error: {ex.Message}");
            }
        }
    }
}