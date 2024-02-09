using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_API.Models;

namespace test_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        public CoursController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("courses")]
        public async Task<ActionResult<IEnumerable<Cours>>> GetCourses()
        {
            if (_dbContext.Courses == null)
            {
                return NotFound();
            }
            return await _dbContext.Courses.ToListAsync();
        }


        [HttpGet("cours/{id}")]
        public async Task<ActionResult<Cours>> GetCours(int id)
        {
            if (_dbContext.Courses == null)
            {
                return NotFound();
            }
            var cours = await _dbContext.Courses.FindAsync(id);
            if(cours == null)
            {
                return NotFound(nameof(cours));
            }
            return cours;
        }
        [HttpPost("addcourse")]
        public async Task<ActionResult<Cours>> PostCours(Cours cours)
        {
            var existingFormateur = await _dbContext.Formateurs.FindAsync(cours.FormateurID);
            if (existingFormateur == null)
            {
                // Gérer le cas où le formateur n'existe pas
                return NotFound($"Le formateur avec l'ID {cours.FormateurID} n'a pas été trouvé.");
            }

            cours.formateur = existingFormateur;
            _dbContext.Courses.Add(cours);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCours), new { id = cours.ID }, cours);
        }


        [HttpPut("updatecourse/{id}")]
        public async Task<IActionResult> PutCours(int id, Cours cours)
        {
            
            if (id != cours.ID)
            {
                return BadRequest();
            }
            _dbContext.Entry(cours).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoursAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }
        private Boolean CoursAvailable(int id)
        {
            return (_dbContext.Courses?.Any(x => x.ID == id)).GetValueOrDefault();
        }

        [HttpDelete("deletecourse/{id}")]
        public async Task<IActionResult> DeleteCours(int id)
        {
            if (_dbContext.Courses == null)
            {
                return NotFound();
            }
            var cours = await _dbContext.Courses.FindAsync(id);
            if (cours == null)
            {
                return NotFound();
            }
            _dbContext.Courses.Remove(cours);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
