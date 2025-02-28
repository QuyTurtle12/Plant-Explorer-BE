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

var builder = WebApplication.CreateBuilder(args);


// Configure configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
}).AddEntityFrameworkStores<PlantExplorerDbContext>()
.AddDefaultTokenProviders();

// Configure DbContext with connection string from appsettings.json
builder.Services.AddDbContext<PlantExplorerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PermissionHandlingMiddleware>();
app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.MapControllers();
app.Run();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
