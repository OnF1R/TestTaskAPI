namespace TaskAPI.Models
{
    public class DoctorForUpdate
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public int CabinetId { get; set; }
        public int SpecializationId { get; set; }
        public int? SectionId { get; set; }
    }
}
