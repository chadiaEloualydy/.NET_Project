using Microsoft.AspNetCore.Components;
using test_API.Models;

namespace ContosoUniversity.Pages
{
    public partial class CoursList
    {
        [Inject]
        public IHttpClientFactory _httpClientFactory { get; set; }
        private List<Cours> courses { get; set; } = new();
        private Cours selectedCourse { get; set; } = new();
        private Cours newCourse {  get; set; } = new();

        private List<Formateur> listeFormateurs { get; set; } = new();
        private string errorMessage = "";


        protected override async Task OnInitializedAsync()
        {
            await LoadFormateurs();
            await RefreshCourses();
            await base.OnInitializedAsync();
        }

        private async Task LoadFormateurs()
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<IEnumerable<Formateur>>("api/Formateur/formateurs");
            listeFormateurs = response.ToList();
        }

        private async Task RefreshCourses()
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<IEnumerable<Cours>>("api/Cours/courses");
            courses = response.ToList();
        }
       
        private async Task EditCours(int courseId)
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetFromJsonAsync<Cours>($"api/Cours/cours/{courseId}");
            selectedCourse = response;
        }

        private async Task DeleteCourse(int courseId)
        {
           
            var client = _httpClientFactory.CreateClient("api");
            await client.DeleteAsync($"api/Cours/deletecourse/{courseId}");
            await RefreshCourses();
            
        }
        private async Task SaveChanges()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("api");
                Formateur newformateur = await client.GetFromJsonAsync<Formateur>($"api/Formateur/formateur/{selectedCourse.FormateurID}");
                selectedCourse.formateur = newformateur;

                if (selectedCourse != null && selectedCourse.ID != 0)
                {
                    // Modification du cours existant
                    var response = await client.PutAsJsonAsync($"api/Cours/updatecourse/{selectedCourse.ID}", selectedCourse);

                    if (response.IsSuccessStatusCode)
                    {
                        // Recharger la liste des cours après la modification
                        await RefreshCourses();
                        selectedCourse = new Cours(); // Réinitialiser le formulaire après la modification réussie
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
                    var response = await client.PostAsJsonAsync("api/Cours/addcourse", selectedCourse);

                    if (response.IsSuccessStatusCode)
                    {
                        // Recharger la liste des cours après l'ajout
                        await RefreshCourses();
                        selectedCourse = new Cours(); // Réinitialiser le formulaire après l'ajout réussi
                    }
                    else
                    {
                        // Gérer l'échec de l'ajout
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        this.errorMessage = errorMessage; // Stocker le message d'erreur pour l'affichage
                    }
                }

                // Recharger la liste des cours au cas où l'action a été effectuée avec succès
                await RefreshCourses();
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message; // Stocker le message d'erreur pour l'affichage                
            }
        }


    }
}
