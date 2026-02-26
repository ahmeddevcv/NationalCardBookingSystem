using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Services;
using System.Security.Claims;

namespace NationalCardBookingSystemWithoutCleanArch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] //  JWT
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId)) 
                throw new UnauthorizedAccessException("Invalid or missing User ID claim");

            return userId;
        }
        //POST /api/users/family-members
        // 1️ Add Family Member
        [HttpPost("family-members")]
        public async Task<IActionResult> AddFamilyMember([FromBody] FamilyMemberDto dto)
        {
            var userId = GetUserId();
            await _userService.AddFamilyMemberAsync(userId, dto);
            return Ok("Family member added successfully");
        }

        // 2️ Get Family Members
        [HttpGet("family-members")]
        public async Task<IActionResult> GetFamilyMembers()
        {
            var userId = GetUserId();
            var members = await _userService.GetFamilyMembersAsync(userId);
            return Ok(members);
        }

        // 3️ Update Family Member
        [HttpPut("family-members/{id}")]
        public async Task<IActionResult> UpdateFamilyMember(int id, [FromBody] FamilyMemberDto dto)
        {
            var userId = GetUserId();
            try
            {
                await _userService.UpdateFamilyMemberAsync(userId, id, dto);
                return Ok("Family member updated");
            }
            catch (KeyNotFoundException) 
            { return NotFound("Family member not found"); }
        }

        // 4️ Delete Family Member
        [HttpDelete("family-members/{id}")]
        public async Task<IActionResult> DeleteFamilyMember(int id)
        {
            var userId = GetUserId();
            try
            {
                await _userService.DeleteFamilyMemberAsync(userId, id);
                return Ok("Family member deleted");
            }
            catch (KeyNotFoundException) 
            { return NotFound("Family member not found"); }
        }
    }
}
