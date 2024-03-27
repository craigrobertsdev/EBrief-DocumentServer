using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;
public class HearingEntry {
    [Key]
    public int Id { get; set; }
    public DateTime HearingDate { get; set; }
    public string AppearanceType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
