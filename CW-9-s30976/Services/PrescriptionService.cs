using CW_9_s30976.Data;
using CW_9_s30976.DTOs;
using CW_9_s30976.Models;
using Microsoft.EntityFrameworkCore;

namespace CW_9_s30976.Services;

public class PrescriptionService(AppDbContext db) : IPrescriptionService
{
    public async Task AddPrescriptionAsync(PrescriptionCreateDto dto)
    {
        if (dto.Medicaments.Count > 10)
            throw new ArgumentException("Prescription can contain at most 10 medicaments.");

        if (dto.DueDate < dto.Date)
            throw new ArgumentException("DueDate must be after or equal to Date.");

        var doctor = await db.Doctors.FindAsync(dto.Doctor.IdDoctor);
        if (doctor is null)
            throw new ArgumentException($"Doctor with ID {dto.Doctor.IdDoctor} not found.");

        foreach (var med in dto.Medicaments)
        {
            var exists = await db.Medicaments.AnyAsync(m => m.IdMedicament == med.IdMedicament);
            if (!exists)
                throw new ArgumentException($"Medicament with ID {med.IdMedicament} not found.");
        }

        var patient = await db.Patients
            .FirstOrDefaultAsync(p => p.FirstName == dto.Patient.FirstName &&
                                      p.LastName == dto.Patient.LastName &&
                                      p.Birthdate == dto.Patient.Birthdate);

        if (patient is null)
        {
            patient = new Patient
            {
                FirstName = dto.Patient.FirstName,
                LastName = dto.Patient.LastName,
                Birthdate = dto.Patient.Birthdate
            };
            await db.Patients.AddAsync(patient);
            await db.SaveChangesAsync();
        }

        var prescription = new Prescription
        {
            Date = dto.Date,
            DueDate = dto.DueDate,
            IdDoctor = doctor.IdDoctor,
            IdPatient = patient.IdPatient,
            PrescriptionMedicaments = new List<PrescriptionMedicament>()
        };

        await db.Prescriptions.AddAsync(prescription);
        await db.SaveChangesAsync(); 

        var meds = dto.Medicaments.Select(m => new PrescriptionMedicament
        {
            IdPrescription = prescription.IdPrescription,
            Prescription = prescription, 
            IdMedicament = m.IdMedicament,
            Dose = m.Dose,
            Details = m.Details
        }).ToList();

        await db.PrescriptionMedicaments.AddRangeAsync(meds);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ SaveChanges failed: " + ex.ToString());
            throw;
        }
    }

    public async Task<PrescriptionResponseDto?> GetPatientDataAsync(int idPatient)
    {
        var patient = await db.Patients
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.PrescriptionMedicaments)
                    .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
            .FirstOrDefaultAsync(p => p.IdPatient == idPatient);

        if (patient is null) return null;

        return new PrescriptionResponseDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Birthdate = patient.Birthdate,
            Prescriptions = patient.Prescriptions
                .OrderBy(p => p.DueDate)
                .Select(p => new PrescriptionDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new DoctorShortDto
                    {
                        IdDoctor = p.Doctor.IdDoctor,
                        FirstName = p.Doctor.FirstName,
                        LastName = p.Doctor.LastName
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDetailsDto
                    {
                        IdMedicament = pm.IdMedicament,
                        Name = pm.Medicament.Name,
                        Dose = pm.Dose,
                        Description = pm.Medicament.Description
                    }).ToList()
                }).ToList()
        };
    }
}
