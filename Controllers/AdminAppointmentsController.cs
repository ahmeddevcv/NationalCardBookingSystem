using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Data;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Controllers
{
    [ApiController]
    [Route("api/admin/appointments")]
    [Authorize] // لاحقًا ممكن تضيف Role = Admin
    public class AdminAppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminAppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        //  Get All Appointments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _context.Appointments
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return Ok(appointments);
        }

        //  Add Appointment
        [HttpPost]
        public async Task<IActionResult> Create(Appointment model)
        {
            await _context.Appointments.AddAsync(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        //  Update Appointment
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Appointment model)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound();

            appointment.Governorate = model.Governorate;
            appointment.Office = model.Office;
            appointment.AppointmentDate = model.AppointmentDate;

            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        //  Delete Appointment
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}
