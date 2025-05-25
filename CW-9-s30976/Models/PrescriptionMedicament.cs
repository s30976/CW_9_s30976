using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CW_9_s30976.Models;

public class PrescriptionMedicament {
    public int IdMedicament { get; set; }
    public Medicament Medicament { get; set; }

    public int IdPrescription { get; set; }
    public Prescription Prescription { get; set; }

    public int Dose { get; set; }
    public string Details { get; set; }
}
