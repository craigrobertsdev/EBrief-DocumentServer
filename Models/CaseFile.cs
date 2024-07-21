using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;
public class CaseFile
{
    public string CaseFileNumber { get; set; } = string.Empty;
    public Defendant Defendant { get; set; } = default!;
    public string? CourtFileNumber { get; set; } = string.Empty;
    public List<HearingEntry> Schedule { get; set; } = [];
    public List<CaseFileEnquiryLog> CfelEntries { get; set; } = [];
    public string FactsOfCharge { get; set; } = default!;
    public Information Information { get; set; } = default!;
    public TimeSpan? TimeInCustody { get; set; }
    public List<Charge> Charges { get; set; } = [];
    public List<Document> Documents { get; set; } = [];
    public string Notes { get; set; } = string.Empty;

    public void GenerateInformationFromCharges()
    {
        List<InformationEntry> charges = [];
        foreach (var charge in Charges)
        {
            charges.Add(new InformationEntry(charge.Sequence, charge.ChargeWording));
        }

        Information = new Information
        {
            Charges = charges
        };
    }
}
