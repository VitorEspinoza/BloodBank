namespace BloodBank.Infrastructure.Services.Reports.Models;

public class DonationReportData
{
    public DateTime DonationDate { get; set; }
    public int QuantityMl { get; set; }
    public string DonorName { get; set; }
    public string BloodType { get; set; }
    public string DonorCity { get; set; }
    public string DonorState { get; set; }

}