using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using RoomBooking.Infrastructure.Services;
using RoomBooking.Application.Services;
using RoomBooking.Application.Interfaces;
using RoomBooking.Infrastructure.Data;
using RoomBooking.Infrastructure.Models;
using RoomBooking.Infrastructure.Repositories;
using RoomBooking.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtSection = builder.Configuration.GetSection("JWT");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy
            .WithOrigins("http://localhost:5173")  
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
    
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();    

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await DevelopmentSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

public partial class Program { }