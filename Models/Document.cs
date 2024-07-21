using static DocumentServer.DummyData;

namespace DocumentServer.Models;

public class Document
{
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
}