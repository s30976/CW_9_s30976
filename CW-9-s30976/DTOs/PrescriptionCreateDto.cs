namespace CW_9_s30976.DTOs;

public class PrescriptionCreateDto
{
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }

    public DoctorDto Doctor { get; set; }
    public PatientDto Patient { get; set; }

    public List<MedicamentPrescriptionDto> Medicaments { get; set; }
}

public class DoctorDto
{
    public int IdDoctor { get; set; } 
}

public class PatientDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
}

public class MedicamentPrescriptionDto
{
    public int IdMedicament { get; set; }
    public int Dose { get; set; }
    public string Details { get; set; }
}