namespace TaskAPI.Models
{
    public class PatientWithoutForeignKeys
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public int Section { get; set; }
    }
}
