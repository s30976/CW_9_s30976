namespace CW_9_s30976.DTOs;

public class PrescriptionResponseDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }

    public List<PrescriptionDto> Prescriptions { get; set; }
}

public class PrescriptionDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }

    public DoctorShortDto Doctor { get; set; }
    public List<MedicamentDetailsDto> Medicaments { get; set; }
}

public class DoctorShortDto
{
    public int IdDoctor { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class MedicamentDetailsDto
{
    public int IdMedicament { get; set; }
    public string Name { get; set; }
    public int Dose { get; set; }
    public string Description { get; set; }
}