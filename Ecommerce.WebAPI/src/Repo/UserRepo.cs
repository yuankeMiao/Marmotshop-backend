
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;
        private readonly DbSet<User> _users;
        public UserRepo(AppDbContext context)
        {
            _context = context;
            _users = _context.Users;
        }

        public async Task<QueryResult<User>> GetAllUsersAsync(UserQueryOptions? options)
        {

            var query = _users.AsQueryable();

            if (options is not null)
            {
                if (options.SearchRole.HasValue)
                {
                    query = query.Where(u => u.Role == options.SearchRole.Value);
                }

                if (!string.IsNullOrEmpty(options.SearchName))
                {
                    query = query.Where(u => u.Firstname.Contains(options.SearchName) || u.Lastname.Contains(options.SearchName));
                }

                // Execute the query to get total count before applying pagination
                var totalCount = await query.CountAsync();

                if (options.Offset >= 0 && options.Limit > 0)
                {
                    query = query.Skip(options.Offset).Take(options.Limit);
                }

                var users = await query.ToListAsync();
                return new QueryResult<User> { Data = users, TotalCount = totalCount };
            }
            else
            {
                // If no query options provided, return all users without pagination
                var users = await query.ToListAsync();
                return new QueryResult<User> { Data = users, TotalCount = users.Count };
            }
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var foundUser = await _users.FindAsync(userId) ?? throw AppException.NotFound("User not found");
            return foundUser;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var foundUser = await _users.FirstOrDefaultAsync(u => u.Email == email) ?? throw AppException.NotFound("User not found");
            return foundUser;
        }

        public async Task<User> CreateUserAsync(User newUser)
        {
            var duplicatedUser = await _users.FirstOrDefaultAsync(user => user.Email == newUser.Email);
            if (duplicatedUser is not null) throw AppException.Duplicate("User email");

            await _users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<User> UpdateUserByIdAsync(User updatedUser)
        {
            // check if user exist
            var foundUser = await _users.FirstOrDefaultAsync(user => user.Id == updatedUser.Id) ?? throw AppException.NotFound("User not found");

            // check if the new email is duplicated
            var duplicatedUser = await _users.FirstOrDefaultAsync(u => u.Email == updatedUser.Email && u.Id != updatedUser.Id);
            if (duplicatedUser is not null) throw AppException.Duplicate("User email");

            _users.Update(updatedUser);
            await _context.SaveChangesAsync();
            return updatedUser;
        }

        public async Task<bool> DeleteUserByIdAsync(Guid userId)
        {
            var user = await _users.FindAsync(userId) ?? throw AppException.NotFound("user not found");

            _users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByCredentialsAsync(UserCredential userCredential)
        {

            var foundUser = await _users.FirstOrDefaultAsync(user => user.Email == userCredential.Email) ?? throw AppException.InvalidLoginCredentials();
            return foundUser;
        }
    }
}