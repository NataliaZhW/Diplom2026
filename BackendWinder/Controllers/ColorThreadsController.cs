using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Data;
using BackendWinder.Models;

namespace BackendWinder.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ColorThreadsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ColorThreadsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ColorThread>>> GetAll()
    {
        return await _context.ColorThreads.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ColorThread>> GetById(int id)
    {
        var item = await _context.ColorThreads.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<ColorThread>> Create(ColorThread item)
    {
        _context.ColorThreads.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ColorThread item)
    {
        if (id != item.Id) return BadRequest();
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.ColorThreads.FindAsync(id);
        if (item == null) return NotFound();
        _context.ColorThreads.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}