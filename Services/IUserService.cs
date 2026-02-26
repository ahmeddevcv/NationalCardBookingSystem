using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public interface IUserService
    {
        Task AddFamilyMemberAsync(int userId, FamilyMemberDto dto);
        Task<List<FamilyMember>> GetFamilyMembersAsync(int userId);
        Task UpdateFamilyMemberAsync(int userId, int memberId, FamilyMemberDto dto);
        Task DeleteFamilyMemberAsync(int userId, int memberId);
    }
}
