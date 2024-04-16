namespace DocumentServer.Models;
public class HearingEntry
{
    public DateTime HearingDate { get; set; }
    public string AppearanceType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
