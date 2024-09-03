namespace TaskAPI.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public Cabinet Cabinet { get; set; }
        public Specialization Specialization { get; set; }
        public Section? Section { get; set; }
    }
}
