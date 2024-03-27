using DocumentServer.Models;
using Bogus;

namespace DocumentServer;
public static class DummyData {
    public static List<CaseFile> GenerateCaseFiles(List<string> caseFileNumbers) {
        List<CaseFile> caseFiles = [];
        foreach (var caseFileNumber in caseFileNumbers) {
            caseFiles.Add(GenerateCaseFile(caseFileNumber));
        }

        return caseFiles;
    }

    public static CaseFile GenerateCaseFile(string caseFileNumber) {
        var caseFile = new Faker<CaseFile>()
            .StrictMode(false)
            .RuleFor(c => c.CaseFileNumber, f => caseFileNumber)
            .RuleFor(c => c.Defendant, f => new Faker<Defendant>()
                .RuleFor(d => d.FirstName, f => f.Name.FirstName())
                .RuleFor(d => d.LastName, f => f.Name.LastName())
                .Generate())
            .RuleFor(c => c.CourtFileNumber, f => $"MCCRM-24-{f.Random.Number(1, 100)}")
            .RuleFor(c => c.PreviousHearings, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                new HearingEntry {
                    AppearanceType = f.PickRandom(new string[] { "First Appearance", "Mention", "Trial" }),
                    HearingDate = f.Date.Past(),
                    Notes = f.Lorem.Sentences(f.Random.Number(1, 3))
                }).ToList())
            .RuleFor(c => c.CfelEntries, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                new CaseFileEnquiryLog {
                    EnteredBy = $"PD{f.Random.Number(10000, 99999)}, {f.Name.FullName()}",
                    EntryDate = f.Date.Past(),
                    EntryText = f.Lorem.Sentences(f.Random.Number(1, 3))
                }).ToList())
            .RuleFor(c => c.FactsOfCharge, f => f.Lorem.Paragraphs(f.Random.Number(1, 3)))
            .RuleFor(c => c.Charges, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                Charges[f.Random.Number(0, Charges.Count - 1)]).ToList())
            .RuleFor(c => c.CaseFileDocuments, f => Enumerable.Range(1, 5).Select(i =>
                new CaseFileDocument {
                    FileName = CaseFileDocuments[i].FileName,
                    Title = CaseFileDocuments[i].Title
                }).ToList())
            .RuleFor(c => c.OccurrenceDocuments, f => Enumerable.Range(1, 5).Select(i =>
                new OccurrenceDocument {
                    FileName = OccurrenceDocuments[i].FileName,
                    Title = OccurrenceDocuments[i].Title
                }).ToList());

        return caseFile;

    }

    static List<Charge> Charges = [
        new Charge {
            Date = new Bogus.DataSets.Date().Recent(),
            Name = "Disorderly Behaviour",
            Sequence = 1,
            VictimName = null,
            ChargeWording = $"""
            On the 1st day of June 2023 at ADELAIDE behaved in a disorderly manner.
            Section 7(1) Summary Offences Act 1953.
            This is a Summary Offence.
            """
        },
        new Charge {
            Date = new Bogus.DataSets.Date().Recent(),
            Name = "Assault",
            Sequence = 1,
            VictimName = "Paul Robertson",
            ChargeWording = $"""
            On the 1st day of June 2023 at ADELAIDE assaulted Paul Robertson.
            Section 20(3) of the Criminal Law Consolidation Act 1935.
            This is a Summary Offence.
            """
        },
        new Charge {
            Date = new Bogus.DataSets.Date().Recent(),
            Name = "Theft",
            Sequence = 1,
            VictimName = "OTR Adelaide",
            ChargeWording = $"""
            On the 15th day of December 2023 in the said State, took property namely a screwdriver of a value less than $2500, dishonestly and without the consent of Fred Smith, the owner of the property, or intending to make a serious encroachment on the owner's proprietary rights.
            Section 134 of the Criminal Law Consolidation Act 1935.
            This is a Summary Offence.
            """
        },
        new Charge {
            Date = new Bogus.DataSets.Date().Recent(),
            Name = "Drive disqualified",
            Sequence = 1,
            VictimName = null,
            ChargeWording = $"""
            On the 12th day of March 2023 in the said State, drove a motor vehicle on a road while disqualified from holding or obtaining a driver's licence.
            Section 91(5a) of the Motor Vehicles Act 1959.
            This is a Minor Indictable offence.
            """
        },
        new Charge {
            Date = new Bogus.DataSets.Date().Recent(),
            Name = "Deception",
            Sequence = 1,
            VictimName = "Betty Ingridson",
            ChargeWording = $"""
            On the 22nd day of January 2023 in the said State, deceived Betty Ingridson and obtained property, namely $500 cash.
            Section 139 of the Criminal Law Consolidation Act 1935.
            This is a Summary Offence.
            """
        },
    ];

    static List<CaseFileDocument> CaseFileDocuments = [
        new CaseFileDocument {
        FileName = "Letter to Prosecution.pdf",
        Title = "Letter to Prosecution",
        },
        new CaseFileDocument {
            FileName = "Bail Application.pdf",
            Title = "Bail Application"
        },
        new CaseFileDocument {
            FileName = "Letter to Defence.pdf",
            Title = "Letter to Defence"
        },
        new CaseFileDocument {
            FileName = "Typical Negotiations.pdf",
            Title = "Typical Negotiations"
        },
        new CaseFileDocument {
            FileName = "Application to Revoke Bail.pdf",
            Title = "Application to Revoke Bail"
        },
    ];

    static List<OccurrenceDocument> OccurrenceDocuments = [
        new OccurrenceDocument {
        FileName = "Police Notes.pdf",
        Title = "Police Notes"
        },
        new OccurrenceDocument {
            FileName = "Victim Statement.pdf",
            Title = "Victim Statement"
        },
        new OccurrenceDocument {
            FileName = "Witness Statement.pdf",
            Title = "Witness Statement"
        },
        new OccurrenceDocument {
            FileName = "PD731.pdf",
            Title = "PD731"
        },
        new OccurrenceDocument {
            FileName = "FSSA Statement of Analysis.pdf",
            Title = "FSSA Statement of Analysis"
        },
    ];
}
