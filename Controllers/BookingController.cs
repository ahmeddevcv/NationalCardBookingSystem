using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Data;
using NationalCardBookingSystemWithoutCleanArch.Hubs;
using NationalCardBookingSystemWithoutCleanArch.Services;
using System.Security.Claims;

namespace NationalCardBookingSystemWithoutCleanArch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IRecurringJobManager _jobManager;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly AppDbContext _context;

        public BookingController(IBookingService bookingService,
                                 IRecurringJobManager jobManager,
                                 IHubContext<NotificationHub> hub,
                                 AppDbContext context)
        {
            _bookingService = bookingService;
            _jobManager = jobManager;
            _hub = hub;// for testing notifications
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing User ID claim");

            return userId;
        }

        // for Test Notification
        [HttpPost("test-notification")]
        public async Task<IActionResult> TestNotification()
        {
            var userId = GetUserId();

            var notifications = new List<string>
    {
        "📢 إشعار 1: مرحباً بك!",
        "📢 إشعار 2: لديك رسالة جديدة",
        "📢 إشعار 3: تم تحديث بياناتك",
        "📢 إشعار 4: موعد الاجتماع غداً الساعة 10 صباحاً"
    };

            foreach (var note in notifications)
            {
                await _hub.Clients.User(userId.ToString())
                    .SendAsync("ReceiveNotification", note);
            }

            return Ok("Notifications sent");
        }





        // 1️ Manual booking execution
        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteBooking()
        {
            var userId = GetUserId();
            try
            {
                await _bookingService.ExecuteBookingAsync(userId);

                return Ok("Booking executed successfully");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }



        // 2️ Start monitoring (Admin only)
        [HttpPost("start-monitor")]
        //[Authorize(Roles = "Admin")] // Role-based Authorization
        public IActionResult StartMonitoring()
        {
            try
            {
                var userId = GetUserId();
                var jobName = $"check-appointments-job-{userId}";


                _jobManager.AddOrUpdate<BackgroundJobs.BookingMonitorJob>(
                "check-appointments-job",
                job => job.CheckAppointments(),
                "*/5 * * * * *" // every 5 seconds
                );

                return Ok(new { status = "success", message = "Monitoring started" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        // 3️ Stop monitoring (Admin only)
        [HttpPost("stop-monitor")]
        //[Authorize(Roles = "Admin")] // Role-based Authorization
        public IActionResult StopMonitoring()
        {
            try
            {
                var userId = GetUserId();
                var jobName = $"check-appointments-job-{userId}";


                _jobManager.RemoveIfExists("check-appointments-job");
                return Ok(new { status = "success", message = "Monitoring stopped" });
            }
            catch (Exception ex) 
            { return BadRequest(new { status = "error", message = ex.Message }); }
        }



        [HttpPost("book/{appointmentId}")]
public async Task<IActionResult> BookAppointment(int appointmentId)
{
    var userId = GetUserId();

    var appointment = await _context.Appointments
        .FirstOrDefaultAsync(a => a.Id == appointmentId && !a.IsBooked);

    if (appointment == null)
        return BadRequest(new { status = "error", message = "Appointment not available" });

    appointment.IsBooked = true;
    appointment.UserId = userId;

    try
    {
        await _context.SaveChangesAsync();
        return Ok(new { status = "success", message = "Appointment booked successfully", appointment });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { status = "error", message = ex.Message });
    }
}




    }


}
