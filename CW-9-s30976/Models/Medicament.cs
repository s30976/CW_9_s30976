using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CW_9_s30976.Models;

public class Medicament {
    [Key]
    public int IdMedicament { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }

    public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
}
