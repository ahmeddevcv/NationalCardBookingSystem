using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NationalCardBookingSystemWithoutCleanArch.BackgroundJobs;
using NationalCardBookingSystemWithoutCleanArch.Data;
using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Helpers;
using NationalCardBookingSystemWithoutCleanArch.Hubs;
using NationalCardBookingSystemWithoutCleanArch.Models;
using NationalCardBookingSystemWithoutCleanArch.Repositories;
using NationalCardBookingSystemWithoutCleanArch.Services;
using Serilog;
using System.Text;

namespace NationalCardBookingSystemWithoutCleanArch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =====================
            // Serilog Configuration
            // =====================
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // =====================
            // 1? Add DbContext
            // =====================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // =====================
            // 2? JWT Authentication
            // =====================
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                // ?????? ??? ??? ?? SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/notificationHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            // =====================
            // 3? Hangfire Configuration
            // =====================
            builder.Services.AddHangfire(config =>
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddHangfireServer();

            // =====================
            // 4? SignalR
            // =====================
            //builder.Services.AddSignalR();

            // ????? ???? CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowed(_ => true));
            });


            // for notifications, we can use SignalR to push real-time updates to clients when appointment availability changes or when a booking is confirmed.
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();





            // =====================
            // 5?? Dependency Injection
            // =====================

            // Helpers
            builder.Services.AddScoped<JwtHelper>();

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookingSettingService, BookingSettingService>();
            builder.Services.AddScoped<IGovernmentApiService, GovernmentApiService>();
            builder.Services.AddHttpClient();// For GovernmentApiService
            //builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IBookingService, BookingService>();// For BookingMonitorJob
            builder.Services.AddScoped<BookingMonitorJob>();// For Hangfire
            //builder.Services.AddScoped<INotificationService, NotificationService>();

            // Background Jobs
            builder.Services.AddScoped<BookingMonitorJob>();

            // Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // =====================
            // 6? Build App
            // =====================

            // Add services to the container.

            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            var app = builder.Build();

            #region seed date
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                SeedLargeData.Initialize(context);
            }
            #endregion


            app.UseCors("AllowAll");
            // =====================
            // 7? Middlewares
            // =====================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Hangfire Dashboard
            app.UseHangfireDashboard("/hangfire");

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unhandled Exception");
                    throw;
                }
            });

            // =====================
            // ????? CORS ??? ??? ?? Map
            // =====================

            // =====================
            // Map Endpoints
            // =====================
            app.MapControllers();

            // SignalR Hub
            app.MapHub<NotificationHub>("/notificationHub");

            // =====================
            // Schedule Hangfire Job
            // =====================
            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate<BookingMonitorJob>(
                    "check-appointments-job",
                    job => job.CheckAppointments(),
                    "*/5 * * * *" // every 5 seconds
                );
            }

            app.Run();



            //app.UseHttpsRedirection();
            //app.UseAuthorization();
            //app.MapControllers();
        }
    }
}
