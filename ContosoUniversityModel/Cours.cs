namespace test_API.Models
{
    public class Cours
    {
        public int ID { get; set; }
        public String Nom_Cours { get; set; }
        public String Description {  get; set; }
        public int FormateurID { get; set; }
        public Formateur formateur { get; set; }
    }
}
