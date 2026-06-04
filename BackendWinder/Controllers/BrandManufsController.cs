using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Data;
using BackendWinder.Models;

namespace BackendWinder.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BrandManufsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BrandManufsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandManuf>>> GetAll()
    {
        return await _context.BrandManufs.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrandManuf>> GetById(int id)
    {
        var item = await _context.BrandManufs.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<BrandManuf>> Create(BrandManuf item)
    {
        _context.BrandManufs.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BrandManuf item)
    {
        if (id != item.Id) return BadRequest();
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.BrandManufs.FindAsync(id);
        if (item == null) return NotFound();
        _context.BrandManufs.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}