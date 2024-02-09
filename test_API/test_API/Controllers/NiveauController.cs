using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test_API.Models;

namespace test_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NiveauController : ControllerBase
    {
            private readonly AppDbContext _dbContext;

            public NiveauController(AppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            [HttpGet("niveaux")]
            public async Task<ActionResult<IEnumerable<Niveau>>> GetNiveaux()
            {
                if (_dbContext.Niveaux == null)
                {
                    return NotFound();
                }
                return await _dbContext.Niveaux.ToListAsync();
            }


            [HttpGet("niveau/{id}")]
            public async Task<ActionResult<Niveau>> GetNiveau(string id)
            {
                if (_dbContext.Niveaux == null)
                {
                    return NotFound();
                }
                var niveau = await _dbContext.Niveaux.FindAsync(id);
                if (niveau == null)
                {
                    return NotFound();
                }
                return niveau;
            }

            [HttpPost("addniveau")]
            public async Task<ActionResult<Niveau>> PostFormateur(Niveau niveau)
            {
                _dbContext.Niveaux.Add(niveau);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetNiveau), new { id = niveau.ID }, niveau);
            }

            [HttpPut("updateniveau/{id}")]
            public async Task<IActionResult> PutNiveau(String id, Niveau niveau)
            {
                if (id != niveau.ID)
                {
                    return BadRequest();
                }
                _dbContext.Entry(niveau).State = EntityState.Modified;
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NiveauAvailable(id))
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
            private Boolean NiveauAvailable(String id)
            {
                return (_dbContext.Niveaux?.Any(x => x.ID == id)).GetValueOrDefault();
            }

        [HttpDelete("deleteniveau/{id}")]
        public async Task<IActionResult> DeleteNiveau(string id)
        {
            // Récupérer le niveau à supprimer
            var niveauToDelete = await _dbContext.Niveaux.FindAsync(id);

            if (niveauToDelete == null)
            {
                return NotFound(); // Niveau non trouvé
            }

            try
            {
                // Récupérer le niveau "aureat" s'il existe, sinon le créer
                var aureatNiveau = await _dbContext.Niveaux.SingleOrDefaultAsync(n => n.Nom == "aureat");
                if (aureatNiveau == null)
                {
                    aureatNiveau = new Niveau { ID = "aureat", Nom = "aureat" };
                    _dbContext.Niveaux.Add(aureatNiveau);
                    await _dbContext.SaveChangesAsync();
                }

                // Réaffecter les étudiants du niveau à supprimer vers le niveau "aureat"
                var studentsToReassign = await _dbContext.Etudiants.Where(e => e.niveau_Id == id).ToListAsync();
                foreach (var student in studentsToReassign)
                {
                    student.niveau_Id = aureatNiveau.ID; // Réaffecter à "aureat"
                    student.niveau = aureatNiveau;
                }

                // Enregistrer les modifications des étudiants dans la base de données
                await _dbContext.SaveChangesAsync();

                // Supprimer le niveau une fois les étudiants réaffectés
                _dbContext.Niveaux.Remove(niveauToDelete);
                await _dbContext.SaveChangesAsync();

                return Ok(); // Suppression réussie
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression du niveau : {ex.Message}");
            }
        }




    }
}

