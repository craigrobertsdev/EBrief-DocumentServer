namespace DocumentServer.Models;
public class Defendant {
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<CaseFile> CaseFiles { get; set; } = [];
    public List<BailAgreement> BailAgreements { get; set; } = [];
    public List<InterventionOrder> InterventionOrders { get; set; } = []; 
}
