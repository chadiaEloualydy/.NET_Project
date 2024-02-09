using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_API.Models;

namespace test_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public EtudiantController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("etudiants")]
        public async Task<ActionResult<IEnumerable<Etudiant>>> GetEtudiants()
        {
            if (_dbContext.Etudiants == null)
            {
                return NotFound();
            }
            return await _dbContext.Etudiants.ToListAsync();
        }


        [HttpGet("etudiant/{id}")]
        public async Task<ActionResult<Etudiant>> GetEtudiant(int id)
        {
            if (_dbContext.Etudiants == null)
            {
                return NotFound();
            }
            var etudiant = await _dbContext.Etudiants.FindAsync(id);
            if (etudiant == null)
            {
                return NotFound();
            }
            return etudiant;
        }

        [HttpPost("addetudiant")]
        public async Task<ActionResult<Etudiant>> PostEtudiant(Etudiant etudiant)
        {
            var existingNiveau = await _dbContext.Niveaux.FindAsync(etudiant.niveau_Id);
            if (existingNiveau == null)
            {
                // Gérer le cas où le niveau n'existe pas
                return NotFound($"Le niveau avec l'ID {etudiant.niveau_Id} n'a pas été trouvé.");
            }

            etudiant.niveau = existingNiveau;
            _dbContext.Etudiants.Add(etudiant);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEtudiant), new { id = etudiant.ID }, etudiant);
        }

        [HttpPut("updateetudiant/{id}")]
        public async Task<IActionResult> PutEtudiant(int id, Etudiant etudiant)
        {
            if (id != etudiant.ID)
            {
                return BadRequest();
            }
            _dbContext.Entry(etudiant).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EtudiantAvailable(id))
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
        private Boolean EtudiantAvailable(int id)
        {
            return (_dbContext.Etudiants?.Any(x => x.ID == id)).GetValueOrDefault();
        }

        [HttpDelete("deleteetudiant/{id}")]
        public async Task<IActionResult> DeleteEtudiant(int id)
        {
            if (_dbContext.Etudiants == null)
            {
                return NotFound();
            }
            var etudiant = await _dbContext.Etudiants.FindAsync(id);
            if (etudiant == null)
            {
                return NotFound();
            }
            _dbContext.Etudiants.Remove(etudiant);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
