using CW_9_s30976.DTOs;
namespace CW_9_s30976.Services;

public interface IPrescriptionService
{
    Task AddPrescriptionAsync(PrescriptionCreateDto dto);
    Task<PrescriptionResponseDto?> GetPatientDataAsync(int idPatient);
}