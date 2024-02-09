using Microsoft.AspNetCore.Components;
using test_API.Models;

namespace ContosoUniversity.Pages
{
    public partial class FormateurList
    {
        [Inject]
        public IHttpClientFactory _httpClientFactory { get; set; }
        private List<Formateur> formateurs { get; set; } = new();
        private Formateur selectedFormateur { get; set; } = new();
        private string errorMessage = "";
        protected override async Task OnInitializedAsync()
        {
            await RefreshFormateurs();
            await base.OnInitializedAsync();
        }

        private async Task RefreshFormateurs()
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<IEnumerable<Formateur>>("api/Formateur/formateurs");
            formateurs = response.ToList();
        }

        private async Task EditFormateur(int FormateurId)
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<Formateur>($"api/Formateur/formateur/{FormateurId}");
            selectedFormateur = response;
        }

        private async Task DeleteFormateur(int FormateurId)
        {

            var client = _httpClientFactory.CreateClient("api");
            await client.DeleteAsync($"api/Formateur/deleteformateur/{FormateurId}");
            await RefreshFormateurs();

        }
        private async Task SaveChanges()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("api");

                if (selectedFormateur != null && selectedFormateur.ID != 0)
                {
                    // Modification du cours existant
                    var response = await client.PutAsJsonAsync($"api/Formateur/updateformateur/{selectedFormateur.ID}", selectedFormateur);

                    if (response.IsSuccessStatusCode)
                    {
                        // Recharger la liste des cours après la modification
                        await RefreshFormateurs();
                        selectedFormateur = new Formateur(); // Réinitialiser le formulaire après la modification réussie
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
                    var response = await client.PostAsJsonAsync("api/Formateur/addformateur", selectedFormateur);

                    if (response.IsSuccessStatusCode)
                    {
                        // Recharger la liste des cours après l'ajout
                        await RefreshFormateurs();
                        selectedFormateur = new Formateur(); // Réinitialiser le formulaire après l'ajout réussi
                    }
                    else
                    {
                        // Gérer l'échec de l'ajout
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        this.errorMessage = errorMessage; // Stocker le message d'erreur pour l'affichage
                    }
                }

                // Recharger la liste des cours au cas où l'action a été effectuée avec succès
                await RefreshFormateurs();
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message; // Stocker le message d'erreur pour l'affichage                
            }
        }


    }
}

