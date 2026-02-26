using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Data;
using System.Net;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public class GovernmentApiService : IGovernmentApiService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GovernmentApiService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GovernmentApiService(AppDbContext context, 
            ILogger<GovernmentApiService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // Login to the government wrbsite and geting the Session Cookie
        // تسجيل دخول للموقع الحكومي واستخراج Session Cookie
        public async Task<string> LoginAndGetSessionAsync(int userId, string phone, string otp)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.All
            };

            var client = _httpClientFactory.CreateClient();

            //link is virtual, you can change to the realone
            //  الرابط ده افتراضي - يتم تغييره للرابط الحقيقي
            //var loginUrl = "https://gov-site.example.com/login";
            //var loginUrl = "https://localhost:7171/login";

            var baseUrl = _configuration["GovernmentApi:BaseUrl"];
            var endpoint = _configuration["GovernmentApi:LoginEndpoint"];

            var loginUrl = $"{baseUrl}{endpoint}";

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("phone", phone),
                new KeyValuePair<string, string>("otp", otp)
            });
            HttpResponseMessage response;
            try
            {

                response = await client.PostAsync(loginUrl, formData);

                _logger.LogInformation("Logging into government site...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during government login");
                throw new Exception ("Error during government login: " + ex.Message);
            }



            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Government login failed");
                throw new Exception("Government login failed");
            }
            //extracting cookies
            // استخراج الـ Cookie
            var cookies = handler.CookieContainer.GetCookies(new Uri(loginUrl));
            var sessionCookie = cookies["SESSIONID"]?.Value;

            if (string.IsNullOrEmpty(sessionCookie))
                throw new InvalidOperationException("Session cookie not found");
            //throw new Exception("Session cookie not found");
            

            //save session to db
            // حفظ السيشن في الداتابيز
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.GovernmentSessionToken = sessionCookie;//you can encrypt (later) this value if you want
            user.SessionExpireAt = DateTime.UtcNow.AddMinutes(30);

            await _context.SaveChangesAsync();

            return sessionCookie;
        }

        // check the validation
        // التحقق من صلاحية السيشن
        public async Task<bool> IsSessionValidAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null || string.IsNullOrEmpty(user.GovernmentSessionToken))
                return false;

            if (user.SessionExpireAt < DateTime.UtcNow)
                return false;

            return true;
        }

        //getting saved session
        // جلب السيشن المحفوظ
        public async Task<string> GetSavedSessionAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return user?.GovernmentSessionToken;
        }
    }//GovernmentApiService

    /*
    public class GovernmentApiService : IGovernmentApiService
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDataProtector _protector;
    private readonly IConfiguration _configuration;

    public GovernmentApiService(
        AppDbContext context,
        IHttpClientFactory httpClientFactory,
        IDataProtectionProvider provider,
        IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _protector = provider.CreateProtector("GovernmentSessionProtector");
        _configuration = configuration;
    }

    // تسجيل دخول للموقع الحكومي واستخراج Session Cookie
    public async Task<string> LoginAndGetSessionAsync(int userId, string phone, string otp)
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            AutomaticDecompression = DecompressionMethods.All
        };

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //  الرابط ده افتراضي - يتم تغييره للرابط الحقيقي
        var loginUrl = "https://gov-site.example.com/login";

        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("phone", phone),
            new KeyValuePair<string, string>("otp", otp)
        });

        var response = await client.PostAsync(loginUrl, formData);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Government login failed");

        // استخراج الـ Cookie
        var cookies = handler.CookieContainer.GetCookies(new Uri(loginUrl));
        var sessionCookie = cookies["SESSIONID"]?.Value;

        if (string.IsNullOrEmpty(sessionCookie))
            throw new InvalidOperationException("Session cookie not found");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // تشفير السيشن قبل التخزين
        user.GovernmentSessionToken = _protector.Protect(sessionCookie);

        // مدة صلاحية السيشن من appsettings.json (default: 30 دقيقة)
        int expiryMinutes = _configuration.GetValue<int>("GovernmentSession:ExpiryMinutes", 30);
        user.SessionExpireAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        await _context.SaveChangesAsync();

        return sessionCookie;
    }

    // التحقق من صلاحية السيشن
    public async Task<bool> IsSessionValidAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null || string.IsNullOrEmpty(user.GovernmentSessionToken))
            return false;

        if (user.SessionExpireAt < DateTime.UtcNow)
            return false;

        return true;
    }

    // جلب السيشن المحفوظ (مع فك التشفير)
    public async Task<string> GetSavedSessionAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null || string.IsNullOrEmpty(user.GovernmentSessionToken))
            return null;

        return _protector.Unprotect(user.GovernmentSessionToken);
    }
}


2-
    in appsettng.json
    {
  "GovernmentSession": {
    "ExpiryMinutes": 45
  }
}
3- in program.cs add

    builder.Services.AddDataProtection();
    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IGovernmentApiService, GovernmentApiService>();


     */




}//NationalCardBookingSystemWithoutCleanArch.Services
