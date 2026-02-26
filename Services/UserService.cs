using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Data;
using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        //adding user  
        // تسجيل مستخدم جديد ببيانات كاملة



        //

        public async Task AddFamilyMemberAsync(int userId, FamilyMemberDto dto)
        {
            var member = new FamilyMember
            {
                FullName = dto.FullName,
                NationalId = dto.NationalId,
                BirthDate = dto.BirthDate,
                TransactionType = dto.TransactionType,
                UserId = userId
            };

            await _context.FamilyMembers.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FamilyMember>> GetFamilyMembersAsync(int userId)
        {
            return await _context.FamilyMembers
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateFamilyMemberAsync(int userId, int memberId, FamilyMemberDto dto)
        {
            var member = await _context.FamilyMembers
                .FirstOrDefaultAsync(x => x.Id == memberId && x.UserId == userId);

            if (member == null)
                throw new KeyNotFoundException("Family member not found");
            member.FullName = dto.FullName;
            member.NationalId = dto.NationalId;
            member.BirthDate = dto.BirthDate;
            member.TransactionType = dto.TransactionType;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteFamilyMemberAsync(int userId, int memberId)
        {
            var member = await _context.FamilyMembers
                .FirstOrDefaultAsync(x => x.Id == memberId && x.UserId == userId);

            if (member == null)
                throw new KeyNotFoundException("Family member not found");

            _context.FamilyMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }

}
