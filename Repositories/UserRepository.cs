using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Data;
using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByPhoneAsync(string phone)
        {
            return await _context.Users
                .Include(u => u.FamilyMembers)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.FamilyMembers)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
