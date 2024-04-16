using System.ComponentModel.DataAnnotations;

namespace DocumentServer.Models;

public class InterventionOrder
{
    [Key]
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Person ProtectedPerson { get; set; } = default!;
    public DateTime DateIssued { get; set; }
    public List<OrderCondition> Conditions { get; set; } = [];
}
