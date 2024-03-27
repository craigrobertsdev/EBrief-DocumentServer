using DocumentServer;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapPost("/load-correspondence", (List<string> filePaths) => {
    foreach (var filePath in filePaths) {
        File.Copy(filePath, Path.Combine("wwwroot/correspondence", Path.GetFileName(filePath)));
    }
});

app.MapGet("/correspondence/{fileName}", (string fileName) => {
    var filePath = Path.Combine("wwwroot/correspondence", fileName);
    if (File.Exists(filePath)) {
        var fileStream = new FileStream(filePath, FileMode.Open);
        return Results.File(fileStream, "application/octet-stream");
    }
    return Results.NotFound();
});

app.MapGet("/evidence/{fileName}", (string fileName) => {
    var filePath = Path.Combine("wwwroot/evidence", fileName);
    if (File.Exists(filePath)) {
        var fileStream = new FileStream(filePath, FileMode.Open);
        return Results.File(fileStream, "application/octet-stream");
    }
    return Results.NotFound();
});

app.MapPost("/generate-case-files", (List<string> caseFileNumbers) => {
    var caseFiles = DummyData.GenerateCaseFiles(caseFileNumbers);
    return Results.Ok(caseFiles);
});

app.Run();

