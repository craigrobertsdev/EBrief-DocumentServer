using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;

public class OrderCondition {
    [Key]
    public int Id { get; set; }
    public int Number { get; set; }
    public string Condition { get; set; } = string.Empty;
}