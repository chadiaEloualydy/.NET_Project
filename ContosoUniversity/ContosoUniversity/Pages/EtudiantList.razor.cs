using Microsoft.AspNetCore.Components;
using test_API.Models;

namespace ContosoUniversity.Pages
{
    public partial class EtudiantList
    {      
        [Inject]
        public IHttpClientFactory _httpClientFactory { get; set; }
        private List<Etudiant> etudiants { get; set; } = new();
        private List<Niveau> niveaux { get; set; } = new();
        private Etudiant selectedEtudiant { get; set; } = new();
        private string errorMessage = "";
        protected override async Task OnInitializedAsync()
        {
            await LoadNiveaux();
            await RefreshEtudiants();
            await base.OnInitializedAsync();
        }

        private async Task RefreshEtudiants()
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<IEnumerable<Etudiant>>("api/Etudiant/etudiants");
            etudiants = response.ToList();
        }

        private async Task LoadNiveaux()
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<IEnumerable<Niveau>>("api/Niveau/niveaux");
            niveaux = response.ToList();
        }

        private async Task EditEtudiant(int EtudiantId)
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<Etudiant>($"api/Etudiant/etudiant/{EtudiantId}");
            selectedEtudiant = response;
        }

        private async Task DeleteEtudiant(int EtudiantId)
        {

            var client = _httpClientFactory.CreateClient("api");
            await client.DeleteAsync($"api/Etudiant/deleteetudiant/{EtudiantId}");
            await RefreshEtudiants();

        }
        private async Task SaveChanges()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("api");
                Niveau newniveau = await client.GetFromJsonAsync<Niveau>($"api/Niveau/niveau/{selectedEtudiant.niveau_Id}");
                selectedEtudiant.niveau = newniveau;

                if (selectedEtudiant != null && selectedEtudiant.ID != 0)
                {
                    // Modification du cours existant
                    var response = await client.PutAsJsonAsync($"api/Etudiant/updateetudiant/{selectedEtudiant.ID}", selectedEtudiant);

                    if (response.IsSuccessStatusCode)
                    {
                        // Recharger la liste des cours après la modification
                        await RefreshEtudiants();
                        selectedEtudiant = new Etudiant(); // Réinitialiser le formulaire après la modification réussie
                    }
                    else
                    {
                        // Gérer l'échec de la modification
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        this.errorMessage = errorMessage; // Stocker le message d'erreur pour l'affichage
                    }
                }
                else
                {
                    // Ajout d'un nouveau cours
                    var response = await client.PostAsJsonAsync("api/Etudiant/addetudiant", selectedEtudiant);

                    if (response.IsSuccessStatusCode)
                    {
                        // Recharger la liste des cours après l'ajout
                        await RefreshEtudiants();
                        selectedEtudiant = new Etudiant(); // Réinitialiser le formulaire après l'ajout réussi
                    }
                    else
                    {
                        // Gérer l'échec de l'ajout
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        this.errorMessage = errorMessage; // Stocker le message d'erreur pour l'affichage
                    }
                }

                // Recharger la liste des cours au cas où l'action a été effectuée avec succès
                await RefreshEtudiants();
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message; // Stocker le message d'erreur pour l'affichage                
            }
        }


    }
}

