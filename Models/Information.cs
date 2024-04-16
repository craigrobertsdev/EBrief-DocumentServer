using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;

public class Information
{
    [Key]
    public int Id { get; set; }
    public List<InformationEntry> Charges { get; set; } = [];
}

public record InformationEntry(int Sequence, string Text);