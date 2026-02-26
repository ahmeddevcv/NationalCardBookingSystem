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

        public async Task<List<FamilyMemberDto>> GetFamilyMembersAsync(int userId)
        {
            return await _context.FamilyMembers
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .Select(x => new FamilyMemberDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    NationalId = x.NationalId,
                    BirthDate = x.BirthDate,
                    TransactionType = x.TransactionType
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateFamilyMemberAsync(int userId, int memberId, UpdateFamilyMemberDto dto)
        {
            var member = await _context.FamilyMembers
                .FirstOrDefaultAsync(x => x.Id == memberId && x.UserId == userId);

            if (member == null)
                throw new KeyNotFoundException("Family member not found");
            bool isModified = false;


            if (dto.FullName != null && member.FullName != dto.FullName)
            {
                member.FullName = dto.FullName;
                isModified = true;
            }

            if (dto.NationalId != null && member.NationalId != dto.NationalId)
            {
                member.NationalId = dto.NationalId;
                isModified = true;
            }

            if (dto.BirthDate.HasValue && member.BirthDate != dto.BirthDate.Value)
            {
                member.BirthDate = dto.BirthDate.Value;
                isModified = true;
            }

            if (dto.TransactionType != null && member.TransactionType != dto.TransactionType)
            {
                member.TransactionType = dto.TransactionType;
                isModified = true;
            }

            if (isModified)
                await _context.SaveChangesAsync();

             return isModified;
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

        public async Task<FamilyMemberDto?> GetFamilyMemberByIdAsync(int userId, int memberId)
        {
            return await _context.FamilyMembers
                .AsNoTracking()
                .Where(x => x.Id == memberId && x.UserId == userId)
                .Select(x => new FamilyMemberDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    NationalId = x.NationalId,
                    BirthDate = x.BirthDate,
                    TransactionType = x.TransactionType
                })
                .FirstOrDefaultAsync();
        }



    }

}
