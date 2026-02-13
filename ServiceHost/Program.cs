using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using FluentValidation.AspNetCore;
using MediatR;
using Restaurant.Core.ApplicationService.Extensions;


var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddSingleton(typeof(ILogRepository), typeof(LogRepository));


Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                   .Enrich.FromLogContext()
                   .CreateLogger();

//builder.Services.AddSingleton(typeof(ILogRepository), typeof(LogRepository));


builder.Services.AddFluentValidation();
//builder.Services.AddScoped<IValidator<UserPostDTO>, UserValidation>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddPayamakService();
builder.Services.AddHttpClient();

//.Configure<AppSettingsModel>(Configuration.GetSection("ApplicationSettings"));
//builder.Services.AddPersistence(builder.Configuration);

//New Added By Me 2025-12-07
//builder.Services.AddScoped<HelpRepository>();

builder.Services.AddRepository();

//builder.Services.AddApplicationSevice();
//builder.Services.AddCalculationServices();

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.Configure<AppSettingsModel>(Configuration.GetSection("ApplicationSettings"));

//builder.Services.AddHostedService<EnterService>();
//builder.Services.AddHostedService<UnitEnterService>();
//builder.Services.AddHostedService<DefectiveTrafficService>();
//builder.Services.AddHostedService<SMSJobService>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//options.Cookie.Name = "SamanIdentityCookies";
//options.Cookie.HttpOnly = true;
//options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//options.LoginPath = "/";
//    // ReturnUrlParameter requires 
//    //using Microsoft.AspNetCore.Authentication.Cookies;
//    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
//    options.SlidingExpiration = true;
//});
builder.Services.Configure<FormOptions>(options =>
{
    // Set the value as needed, e.g., 4096 (4 KB) instead of 2048 (2 KB)
    options.ValueCountLimit = 4096;
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");


//builder.Services.AddTransient<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(k =>
{
    k.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    k.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(p =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]);
    p.SaveToken = true;
    p.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Secret"],
        ValidAudience = builder.Configuration["JwtSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});



//// Background service
//builder.Services.AddHostedService<AccessLogsPollingService>();
/////
///// ??? HttpClient
//builder.Services.AddHttpClient("AccessLogsClient", client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["AccessLogs:BaseUrl"]);
//    client.Timeout = TimeSpan.FromSeconds(60);
//   
//    // client.DefaultRequestHeaders.Add("Accept", "application/json");
//})
//.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
//{
//    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
//    {
//     
//        if (builder.Environment.IsDevelopment())
//            return true;

//        return errors == System.Net.Security.SslPolicyErrors.None;
//    }
//});



// ??? HttpClient
//builder.Services.AddHttpClient("AccessLogsClient", client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["AccessLogs:BaseUrl"]);
//    client.Timeout = TimeSpan.FromSeconds(60);
//})
//.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
//{
//    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//});

//// ??? Handler
//builder.Services.AddTransient<SyncAccessLogsCommandHandler>();

//// ??? Mediator
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SyncAccessLogsCommandHandler>());

//// ?????? Hangfire ?? MemoryStorage
//builder.Services.AddHangfire(config =>
//{
//    config.UseMemoryStorage();
//});
//builder.Services.AddHangfireServer();




var app = builder.Build();

//var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
//recurringJobManager.AddOrUpdate<AccessLogsJobs>(
//    "sync-access-logs",
//    job => job.ExecuteSyncAccessLogs(),
//    "*/30 * * * * *"
//);


var config = app.Configuration;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();



var serviceAccountKeyFilePath = builder.Configuration["Firebase:ServiceAccountKeyFilePath"];
var credential = GoogleCredential.FromFile(serviceAccountKeyFilePath);
FirebaseApp.Create(new AppOptions
{
    Credential = credential,
});



app.UseCors("AllowSpecificOrigin");
//app.UseWindsorContainer();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{

endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
endpoints.MapRazorPages();
//endpoints.MapHub<AnnouncementHub>("/hubs/announcement");


    endpoints.MapGet("/service", async context =>
    {
        await context.Response.WriteAsync("Service Host23333");
    });
});


app.Run();


// -------------------------
// Job
// -------------------------
public class AccessLogsJobs
{
    private readonly IMediator _mediator;
    private readonly ILogger<AccessLogsJobs> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public AccessLogsJobs(IMediator mediator, ILogger<AccessLogsJobs> logger, IServiceScopeFactory scopeFactory)
    {
        _mediator = mediator;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task ExecuteSyncAccessLogs()
    {
        using var scope = _scopeFactory.CreateScope();
        try
        {
            //await _mediator.Send(new SyncAccessLogsCommand());
            _logger.LogInformation("SyncAccessLogs executed successfully at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SyncAccessLogs job");
        }
    }
}




