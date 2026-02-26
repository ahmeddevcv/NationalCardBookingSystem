using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public interface IUserService
    {
        Task AddFamilyMemberAsync(int userId, FamilyMemberDto dto);
        Task<List<FamilyMemberDto>> GetFamilyMembersAsync(int userId);
        Task<bool> UpdateFamilyMemberAsync(int userId, int memberId, UpdateFamilyMemberDto dto);
        Task DeleteFamilyMemberAsync(int userId, int memberId);
        Task<FamilyMemberDto?> GetFamilyMemberByIdAsync(int userId, int memberId);
    }
}
