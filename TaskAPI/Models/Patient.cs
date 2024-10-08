﻿namespace TaskAPI.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname {  get; set; } 
        public string Patronymic { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public Section Section { get; set; }
    }

    public enum PatientSort
    {
        Id,
        Name,
        Surname,
        Patronymic,
        Address,
        Gender,
        BirthDate,
        Section,
    }
}
