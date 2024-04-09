using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;

public class BailAgreement {
    [Key]
    public int Id { get; set; }
    public DateTime DateEnteredInto { get; set; }
    public List<OrderCondition> Conditions { get; set; } = [];
}
