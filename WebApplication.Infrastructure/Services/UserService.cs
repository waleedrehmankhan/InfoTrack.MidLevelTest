using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly InMemoryContext _dbContext;
        private static readonly ILog _log = LogManager.GetLogger(typeof(UserService));

        public UserService(InMemoryContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.Users.Where(user => user.Id == id)
                    .Include(x => x.ContactDetail)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error($"Failed to retrieve User with Id: {id}", ex);
                }

                throw;
            }
        }
        
        public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
        {
            try
            {
                var userSet = _dbContext.Set<User>();
                IQueryable<User> query = userSet;

                if (!string.IsNullOrEmpty(givenNames))
                {
                    query = query.Where(user => user.GivenNames.Contains(givenNames));
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    query = query.Where(user => user.LastName.Contains(lastName));
                }

                if (!string.IsNullOrEmpty(givenNames) && !string.IsNullOrEmpty(lastName))
                {
                    query = query.Where(user => user.GivenNames.Contains(givenNames) && user.LastName.Contains(lastName));
                }

                return await query.Include(a => a.ContactDetail).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error($"Failed to retrieve Users with Name: {givenNames} {lastName}", ex);
                }

                throw;
            }
        }

        public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            var itemsToSkip = (page - 1) * count;
            return await _dbContext.Users.Include(u => u.ContactDetail).Skip(itemsToSkip).Take(count).ToListAsync(cancellationToken);
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.Users.AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return user;
            }
            catch (Exception  ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error($"Failed to create User with Given Name: {user.GivenNames}, Last Name: {user.LastName}", ex);
                }

                throw;
            }
        }

        public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingUser = await _dbContext.Users.Include(u => u.ContactDetail).FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

                if (existingUser == null)
                {
                    return null;
                }

                existingUser.GivenNames = user.GivenNames;
                existingUser.LastName = user.LastName;
                existingUser.ContactDetail = user.ContactDetail;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return existingUser;
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error($"Failed to update User with Id: {user.Id}, Given Name: {user.GivenNames}, Last Name: {user.LastName}", ex);
                }

                throw;
            }
        }

        public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var userToDelete = await _dbContext.Users.Include(x => x.ContactDetail).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
                if (userToDelete == null)
                {
                    return null;
                }

                _dbContext.Users.Remove(userToDelete);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return userToDelete;
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error($"Failed to delete User with Id: {id}", ex);
                }

                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.CountAsync(cancellationToken);
        }
    }
}
