using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CW_9_s30976.Models;

public class Patient {
    [Key]
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }

    public ICollection<Prescription> Prescriptions { get; set; }
}