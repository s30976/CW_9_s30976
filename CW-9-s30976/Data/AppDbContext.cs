using System;
using CW_9_s30976.Models;
using Microsoft.EntityFrameworkCore;

namespace CW_9_s30976.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasKey(pm => new { pm.IdPrescription, pm.IdMedicament });

          
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdPrescription);

           
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdMedicament);

            
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(pa => pa.Prescriptions)
                .HasForeignKey(p => p.IdPatient)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.IdDoctor)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Doctor>().HasData(new Doctor
            {
                IdDoctor = 1,
                FirstName = "Adam",
                LastName = "Nowak",
                Email = "adam.nowak@example.com"
            });

            modelBuilder.Entity<Patient>().HasData(new Patient
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Birthdate = new DateTime(1990, 1, 1)
            });

            modelBuilder.Entity<Medicament>().HasData(
                new Medicament
                {
                    IdMedicament = 1,
                    Name = "Paracetamol",
                    Description = "Painkiller",
                    Type = "Tablet"
                },
                new Medicament
                {
                    IdMedicament = 2,
                    Name = "Ibuprofen",
                    Description = "Anti-inflammatory",
                    Type = "Tablet"
                }
            );
        }
    }
}
