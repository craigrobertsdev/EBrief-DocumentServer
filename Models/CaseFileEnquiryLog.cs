namespace DocumentServer.Models;
public class CasefileEnquiryLog
{
    public string EntryText { get; set; } = string.Empty;
    public string EnteredBy { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; }
}
