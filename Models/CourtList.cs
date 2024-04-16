using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;
public class CourtList
{
    [Key]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public List<Defendant> Defendants { get; set; } = [];

    public void GenerateInformations()
    {
        foreach (var defendant in Defendants)
        {
            foreach (var caseFile in defendant.CaseFiles)
            {
                caseFile.GenerateInformationFromCharges();
            }
        }
    }
}
