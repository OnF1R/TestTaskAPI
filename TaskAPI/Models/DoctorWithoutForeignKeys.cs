namespace TaskAPI.Models
{
    public class DoctorWithoutForeignKeys
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public int Cabinet { get; set; }
        public string Specialization { get; set; }
        public int? Section { get; set; }
    }
}
