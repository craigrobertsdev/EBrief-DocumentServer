using System.Text.Json;
using System.Text.Json.Serialization;
using DocumentServer;
using DocumentServer.Models;
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapPost("/load-correspondence", (List<string> filePaths) =>
{
    foreach (var filePath in filePaths)
    {
        File.Copy(filePath, Path.Combine("wwwroot/correspondence", Path.GetFileName(filePath)));
    }
});

app.MapGet("/correspondence", (string fileName) =>
{
    var filePath = Path.Combine("wwwroot/correspondence", fileName);
    if (File.Exists(filePath))
    {
        var fileStream = new FileStream(filePath, FileMode.Open);
        return Results.File(fileStream, "application/octet-stream");
    }
    return Results.NotFound();
});

app.MapGet("/evidence", (string fileName) =>
{
    var filePath = Path.Combine("wwwroot/evidence", fileName);
    if (File.Exists(filePath))
    {
        var fileStream = new FileStream(filePath, FileMode.Open);
        return Results.File(fileStream, "application/octet-stream");
    }
    return Results.NotFound();
});

app.MapPost("/generate-case-files", (CourtListDto courtList) =>
{
    var caseFiles = DummyData.GenerateCaseFiles(courtList.CaseFileNumbers, courtList.CourtDate);

    File.WriteAllText("casefiles.json", JsonSerializer.Serialize(caseFiles, new JsonSerializerOptions
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    }));

    return Results.Ok(caseFiles);
});

app.MapPost("/add-custody", (CourtListDto courtList) =>
{
    // for custody testing purposes
    if (courtList.CaseFileNumbers.Count == 1)
    {
        var caseFile = DummyData.GenerateCaseFileWithNewDefendant(courtList.CaseFileNumbers.First());
        var courtDate = courtList.CourtDate;
        caseFile.Schedule.Add(new()
        {
            AppearanceType = "Hearing",
            HearingDate = new(courtDate.Year, courtDate.Month, courtDate.Day, 15, 0, 0),
            Notes = "First appearance"
        });

        caseFile.CaseFileNumber = caseFile.CaseFileNumber.ToUpper();

        return Results.Ok(new List<Casefile>() { caseFile });
    }
    else
    {
        var caseFiles = DummyData.GenerateCaseFiles(courtList.CaseFileNumbers, courtList.CourtDate);
        return Results.Ok(caseFiles);
    }
});

app.MapPost("/update-cfels", (IEnumerable<string> casefileNumbers, string text) =>
{
    return Results.Ok();
});

app.MapPost("/refresh", (CasefileUpdateContent content) =>
{
    var savedCaseFiles = JsonSerializer.Deserialize<List<Casefile>>(File.ReadAllText("casefiles.json"));
    var casefiles = savedCaseFiles.Where(cf => content.CasefileNumbers.Contains(cf.CaseFileNumber)).ToList();
    return Results.Ok(casefiles);
});

app.Run();

record CourtListDto(List<string> CaseFileNumbers, DateTime CourtDate);
record CasefileUpdateContent(IEnumerable<string> CasefileNumbers);