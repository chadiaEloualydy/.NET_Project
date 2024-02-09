using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_API.Models;

namespace test_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormateurController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public FormateurController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("formateurs")]
        public async Task<ActionResult<IEnumerable<Formateur>>> GetFormateurs()
        {
            if (_dbContext.Formateurs == null)
            {
                return NotFound();
            }
            return await _dbContext.Formateurs.ToListAsync();
        }


        [HttpGet("formateur/{id}")]
        public async Task<ActionResult<Formateur>> GetFormateur(int id)
        {
            if (_dbContext.Formateurs == null)
            {
                return NotFound();
            }
            var formateur = await _dbContext.Formateurs.FindAsync(id);
            if (formateur == null)
            {
                return NotFound();
            }
            return formateur;
        }

        [HttpPost("addformateur")]
        public async Task<ActionResult<Formateur>> PostFormateur(Formateur formateur)
        {
            _dbContext.Formateurs.Add(formateur);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFormateur), new { id = formateur.ID }, formateur);
        }

        [HttpPut("updateformateur/{id}")]
        public async Task<IActionResult> PutFormateur(int id, Formateur formateur)
        {
            if(id != formateur.ID)
            {
                return BadRequest();
            }
            _dbContext.Entry(formateur).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!FormateurAvailable(id))
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
        private Boolean FormateurAvailable(int id)
        {
            return (_dbContext.Formateurs?.Any(x => x.ID == id)).GetValueOrDefault();
        }

        [HttpDelete("deleteformateur/{id}")]
        public async Task<IActionResult> DeleteFormateur(int id)
        {
            // Récupérer le formateur à supprimer
            var formateurToDelete = await _dbContext.Formateurs.FindAsync(id);

            if (formateurToDelete == null)
            {
                return NotFound(); // formateur non trouvé
            }

            try
            {
                // Récupérer le formateur "autre" s'il existe, sinon le créer
                var autreFormateur = await _dbContext.Formateurs.SingleOrDefaultAsync(n => n.Nom == "autre");
                if (autreFormateur == null)
                {
                    autreFormateur = new Formateur {  Nom = "autre", Prenom="" };
                    _dbContext.Formateurs.Add(autreFormateur);
                    await _dbContext.SaveChangesAsync();
                }

                // Réaffecter les courses du formateur à supprimer vers le formateur "autre"
                var coursesToReassign = await _dbContext.Courses.Where(c => c.FormateurID == id).ToListAsync();
                foreach (var cours in coursesToReassign)
                {
                    cours.FormateurID = autreFormateur.ID; // Réaffecter à "autre"
                    cours.formateur = autreFormateur;
                }

                // Enregistrer les modifications des courses dans la base de données
                await _dbContext.SaveChangesAsync();

                // Supprimer le formateur une fois les courses réaffectés
                _dbContext.Formateurs.Remove(formateurToDelete);
                await _dbContext.SaveChangesAsync();

                return Ok(); // Suppression réussie
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression de formateur : {ex.Message}");
            }
        }
    }

  
}
