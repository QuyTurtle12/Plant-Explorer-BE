using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.DI;
using Plant_Explorer.Middleware;
using Plant_Explorer.Repositories.Base;
using Plant_Explorer.Services;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Plant_Explorer.Services.Services;
using Plant_Explorer.Contract.Services.Interface;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FirestoreDb>(sp =>
{
    return new FirestoreDbBuilder
    {
        ProjectId = "plant-explorer-95afb",
        Credential = GoogleCredential.FromFile("plant-explorer-95afb-firebase-adminsdk-fbsvc-f5232f5fc8.json")
    }.Build();
});

// Configure configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IQuizAttemptService, QuizAttemptService>();
builder.Services.AddScoped<IQuizService, QuizService>();


// Bind JwtSettings from configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
}).AddEntityFrameworkStores<PlantExplorerDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Configure DbContext with connection string from appsettings.json
builder.Services.AddDbContext<PlantExplorerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Set the minimum level to Debug
builder.Logging.SetMinimumLevel(LogLevel.Debug);


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<PermissionHandlingMiddleware>();
app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.MapControllers();
app.Run();
