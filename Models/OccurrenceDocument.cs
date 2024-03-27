using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;

public class OccurrenceDocument {
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}