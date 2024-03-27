namespace DocumentServer.Models;
public class CaseFileEnquiryLog {
    public string EntryText { get; set; } = string.Empty;
    public string EnteredBy { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; }
}
