using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CW_9_s30976.Data;
using CW_9_s30976.Services;
using CW_9_s30976.DTOs;
using CW_9_s30976.Models;

namespace CW_9_s30976.Tests
{
    public class PrescriptionServiceTests
    {
        private AppDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new AppDbContext(options);

            
            db.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "A", LastName = "B", Email = "a@b" });
            db.Medicaments.AddRange(
                new Medicament { IdMedicament = 1, Name = "X", Description = "D", Type = "T" },
                new Medicament { IdMedicament = 2, Name = "Y", Description = "E", Type = "T" }
            );
            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task AddPrescriptionAsync_CreatesPatientAndPrescription()
        {
            
            var db = GetInMemoryDb();
            var service = new PrescriptionService(db);
            var dto = new PrescriptionCreateDto
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                Doctor = new DoctorDto { IdDoctor = 1 },
                Patient = new PatientDto { FirstName = "P", LastName = "Q", Birthdate = new DateTime(1980, 1, 1) },
                Medicaments = new List<MedicamentPrescriptionDto>
                {
                    new MedicamentPrescriptionDto { IdMedicament = 1, Dose = 5, Details = "ok" }
                }
            };

            
            await service.AddPrescriptionAsync(dto);

            
            var patient = db.Patients.Single(p => p.FirstName == "P");
            Assert.NotNull(patient);
            var prescription = db.Prescriptions
                .Include(p => p.PrescriptionMedicaments)
                .Single(p => p.IdPatient == patient.IdPatient);
            Assert.Equal(dto.Medicaments.Count, prescription.PrescriptionMedicaments.Count);
        }

        [Fact]
        public async Task AddPrescriptionAsync_TwoMedicaments_Succeeds()
        {
            
            var db = GetInMemoryDb();
            var service = new PrescriptionService(db);
            var dto = new PrescriptionCreateDto
            {
                Date = new DateTime(2025, 5, 25, 10, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 6, 1, 10, 0, 0, DateTimeKind.Utc),
                Doctor = new DoctorDto { IdDoctor = 1 },
                Patient = new PatientDto
                {
                    FirstName = "Maria",
                    LastName = "Nowak",
                    Birthdate = new DateTime(1985, 7, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                Medicaments = new List<MedicamentPrescriptionDto>
                {
                    new MedicamentPrescriptionDto { IdMedicament = 1, Dose = 2, Details = "Po posiłku, 2× dziennie" },
                    new MedicamentPrescriptionDto { IdMedicament = 2, Dose = 1, Details = "Wieczorem przed snem" }
                }
            };

            
            await service.AddPrescriptionAsync(dto);

            
            var patient = db.Patients.Single(p => p.FirstName == "Maria");
            var prescription = db.Prescriptions
                .Include(p => p.PrescriptionMedicaments)
                .Single(p => p.IdPatient == patient.IdPatient);
            Assert.Equal(2, prescription.PrescriptionMedicaments.Count);
        }

        [Fact]
        public async Task AddPrescriptionAsync_TooManyMedicaments_ThrowsArgumentException()
        {
            
            var db = GetInMemoryDb();
            var service = new PrescriptionService(db);
            var dto = new PrescriptionCreateDto
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today,
                Doctor = new DoctorDto { IdDoctor = 1 },
                Patient = new PatientDto { FirstName = "A", LastName = "B", Birthdate = DateTime.Today },
                Medicaments = Enumerable.Range(1, 11)
                    .Select(i => new MedicamentPrescriptionDto { IdMedicament = 1, Dose = 1, Details = string.Empty })
                    .ToList()
            };

            
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.AddPrescriptionAsync(dto));
        }

        [Fact]
        public async Task AddPrescriptionAsync_InvalidDoctor_ThrowsArgumentException()
        {
            
            var db = GetInMemoryDb();
            var service = new PrescriptionService(db);
            var dto = new PrescriptionCreateDto
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today,
                Doctor = new DoctorDto { IdDoctor = 999 },
                Patient = new PatientDto { FirstName = "A", LastName = "B", Birthdate = DateTime.Today },
                Medicaments = new List<MedicamentPrescriptionDto>
                {
                    new MedicamentPrescriptionDto { IdMedicament = 1, Dose = 1, Details = string.Empty }
                }
            };

            
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => service.AddPrescriptionAsync(dto)
            );
            Assert.Contains("Doctor with ID 999 not found", ex.Message);
        }

        [Fact]
        public async Task GetPatientDataAsync_ReturnsSortedPrescriptionsAndDetails()
        {
            
            var db = GetInMemoryDb();
            var patient = new Patient { FirstName = "Z", LastName = "X", Birthdate = new DateTime(1970, 1, 1) };
            db.Patients.Add(patient);
            db.SaveChanges();

            db.Prescriptions.AddRange(
                new Prescription { Date = DateTime.Today, DueDate = DateTime.Today.AddDays(2), IdDoctor = 1, IdPatient = patient.IdPatient },
                new Prescription { Date = DateTime.Today, DueDate = DateTime.Today.AddDays(1), IdDoctor = 1, IdPatient = patient.IdPatient }
            );
            db.SaveChanges();

            var rxs = db.Prescriptions.ToList();
            db.PrescriptionMedicaments.AddRange(
                new PrescriptionMedicament { IdPrescription = rxs[0].IdPrescription, IdMedicament = 1, Dose = 2, Details = "A" },
                new PrescriptionMedicament { IdPrescription = rxs[1].IdPrescription, IdMedicament = 2, Dose = 3, Details = "B" }
            );
            db.SaveChanges();

            var service = new PrescriptionService(db);

            
            var result = await service.GetPatientDataAsync(patient.IdPatient);

            
            Assert.NotNull(result);
            Assert.Equal(
                result.Prescriptions.Select(r => r.DueDate),
                result.Prescriptions.OrderBy(r => r.DueDate).Select(r => r.DueDate)
            );
            Assert.All(result.Prescriptions, p => Assert.NotNull(p.Doctor));
            Assert.All(result.Prescriptions.SelectMany(r => r.Medicaments), m => Assert.InRange(m.IdMedicament, 1, 2));
        }
    }
}
