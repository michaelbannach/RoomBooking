using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingC.Data;
using RoomBookingC.Models;

namespace RoomBookingC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly RoomBookContext _context;

    public EmployeeController(RoomBookContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<Employee>> GetAll() => await _context.Employees.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Employee>> Create(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return Ok(employee);
    }
}
