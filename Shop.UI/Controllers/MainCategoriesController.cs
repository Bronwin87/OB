using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Products;

namespace Shop.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MainCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MainCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MainCategory>>> GetMainCategories()
        {
            return await _context.MainCategories.ToListAsync();
        }

        // GET: api/MainCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MainCategory>> GetMainCategory(string id)
        {
            var mainCategory = await _context.MainCategories.FindAsync(id);

            if (mainCategory == null)
            {
                return NotFound();
            }

            return mainCategory;
        }

        // PUT: api/MainCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMainCategory(string id, MainCategory mainCategory)
        {
            if (id != mainCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(mainCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MainCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MainCategories
        [HttpPost]
        public async Task<ActionResult<MainCategory>> PostMainCategory(MainCategory mainCategory)
        {
            _context.MainCategories.Add(mainCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMainCategory", new { id = mainCategory.Id }, mainCategory);
        }

        // DELETE: api/MainCategories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MainCategory>> DeleteMainCategory(string id)
        {
            var mainCategory = await _context.MainCategories.FindAsync(id);
            if (mainCategory == null)
            {
                return NotFound();
            }

            _context.MainCategories.Remove(mainCategory);
            await _context.SaveChangesAsync();

            return mainCategory;
        }

        private bool MainCategoryExists(string id)
        {
            return _context.MainCategories.Any(e => e.Id == id);
        }
    }
}
