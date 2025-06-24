using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using RoomBookingC.Data;
using RoomBookingC.Models;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddDbContext<RoomBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomBookContext>();
    db.Database.EnsureCreated();

    if (!db.Rooms.Any())
    {
        db.Rooms.AddRange(
            new Room { Name = "Besprechungsraum 1" },
            new Room { Name = "Besprechungsraum 2" },
            new Room { Name = "Besprechungsraum 3" }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();