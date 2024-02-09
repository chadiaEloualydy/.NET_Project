using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace test_API.Models
{
    public class Etudiant
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public String Nom { get; set; }
        public String Prenom { get; set; }
        public String niveau_Id { get; set; }
        public Niveau niveau { get; set; }

    }
}
