using DocumentServer.Models;
using Bogus;

namespace DocumentServer;
public static class DummyData
{
    private static readonly Faker _faker;
    private static readonly List<Defendant> _defendants;
    private static Random _rng = new();

    static DummyData()
    {
        _faker = new();
        _defendants = RandomiseDefendants();
    }

    public static List<Casefile> GenerateCaseFiles(List<string> caseFileNumbers, DateTime courtDate)
    {
        List<Casefile> caseFiles = [];
        var rand = new Random();
        foreach (var caseFileNumber in caseFileNumbers)
        {
            var caseFile = GenerateCaseFile(caseFileNumber);
            var hearingTime = _sittingTimes[rand.Next(_sittingTimes.Length)];
            caseFile.Schedule.Add(new()
            {
                AppearanceType = _appearanceTypes[rand.Next(_appearanceTypes.Length)],
                HearingDate = new(courtDate.Year, courtDate.Month, courtDate.Day, hearingTime.Hour, hearingTime.Minute, 0),
                Notes = _hearingNotes[rand.Next(_hearingNotes.Count)]
            });
            caseFiles.Add(caseFile);

        }

        for (int i = 0; i < caseFiles.Count; i++)
        {
            for (int j = 0; j < caseFiles[i].Charges.Count; j++)
            {
                caseFiles[i].Charges[j] = new Charge
                {
                    Date = caseFiles[i].Charges[j].Date,
                    ChargeWording = caseFiles[i].Charges[j].ChargeWording,
                    Name = caseFiles[i].Charges[j].Name,
                    Sequence = j + 1,
                    VictimName = caseFiles[i].Charges[j].VictimName
                };
            }

            caseFiles[i].Documents.AddRange(Enumerable.Range(0, rand.Next(_caseFileDocuments.Count)).Select(i => _caseFileDocuments[i]));
            caseFiles[i].Documents.AddRange(Enumerable.Range(0, rand.Next(_occurrenceDocuments.Count)).Select(i => _occurrenceDocuments[i]));
            caseFiles[i].Documents.Shuffle();
        }

        return caseFiles;
    }

    public static Casefile GenerateCaseFile(string caseFileNumber)
    {
        var caseFile = new Faker<Casefile>()
            .StrictMode(false)
            .RuleFor(c => c.CaseFileNumber, f => caseFileNumber)
            .RuleFor(c => c.Defendant, f => _defendants[f.Random.Number(0, _defendants.Count - 1)])
            .RuleFor(c => c.CourtFileNumber, f => $"MCCRM-24-{f.Random.Number(1, 10000)}")
            .RuleFor(c => c.Schedule, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                new HearingEntry
                {
                    AppearanceType = f.PickRandom(_appearanceTypes),
                    HearingDate = f.Date.Past(),
                    Notes = _hearingNotes[f.Random.Number(0, _hearingNotes.Count - 1)]
                }).ToList())
            .RuleFor(c => c.CfelEntries, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                new CasefileEnquiryLog
                {
                    EnteredBy = $"ID {f.Random.Number(10000, 99999)}, {f.Name.FullName()}",
                    EntryDate = f.Date.Past(),
                    EntryText = _cfelEntries[f.Random.Number(0, _cfelEntries.Count - 1)]
                }).ToList())
            .RuleFor(c => c.FactsOfCharge, f => _factsOfCharge[f.Random.Number(0, _factsOfCharge.Count - 1)])
            .RuleFor(c => c.Charges, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                Charges[f.Random.Number(0, Charges.Count - 1)]).ToList())
            .RuleFor(c => c.TimeInCustody, f => new TimeSpan(f.Random.Number(0, 30), f.Random.Number(0, 59), 0));

        return caseFile;
    }

    public static Casefile GenerateCaseFileWithNewDefendant(string caseFileNumber)
    {
        var caseFile = new Faker<Casefile>()
            .StrictMode(false)
            .RuleFor(c => c.CaseFileNumber, f => caseFileNumber)
            .RuleFor(c => c.Defendant, f => CustodyDefendant())
            .RuleFor(c => c.CourtFileNumber, f => $"MCCRM-24-{f.Random.Number(1, 10000)}")
            .RuleFor(c => c.Schedule, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                new HearingEntry
                {
                    AppearanceType = f.PickRandom(_appearanceTypes),
                    HearingDate = f.Date.Past(),
                    Notes = _hearingNotes[f.Random.Number(0, _hearingNotes.Count - 1)]
                }).ToList())
            .RuleFor(c => c.CfelEntries, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                new CasefileEnquiryLog
                {
                    EnteredBy = $"ID {f.Random.Number(10000, 99999)}, {f.Name.FullName()}",
                    EntryDate = f.Date.Past(),
                    EntryText = _cfelEntries[f.Random.Number(0, _cfelEntries.Count - 1)]
                }).ToList())
            .RuleFor(c => c.FactsOfCharge, f => _factsOfCharge[f.Random.Number(0, _factsOfCharge.Count - 1)])
            .RuleFor(c => c.Charges, f => Enumerable.Range(0, f.Random.Number(1, 5)).Select(i =>
                Charges[f.Random.Number(0, Charges.Count - 1)]).ToList())
            .RuleFor(c => c.Documents, f => Enumerable.Range(0, _caseFileDocuments.Count).Select(i =>
                new Document
                {
                    FileName = _caseFileDocuments[i].FileName,
                    Title = _caseFileDocuments[i].Title,
                    DocumentType = DocumentType.CaseFile
                }).ToList().RandomiseOrder())
            .RuleFor(c => c.Documents, f => Enumerable.Range(0, _occurrenceDocuments.Count).Select(i =>
                new Document
                {
                    FileName = _occurrenceDocuments[i].FileName,
                    Title = _occurrenceDocuments[i].Title,
                    DocumentType = DocumentType.Occurrence
                }).ToList().RandomiseOrder());

        return caseFile;
    }

    static List<Charge> Charges = [
        new Charge
        {
            Date = new Bogus.DataSets.Date().Past(),
            Name = "Disorderly Behaviour",
            Sequence = 1,
            VictimName = null,
            ChargeWording = $"""
            On the 1st day of June 2023 at ADELAIDE behaved in a disorderly manner.
            Section 7(1) Summary Offences Act 1953.
            This is a Summary Offence.
            """
        },
        new Charge
        {
            Date = new Bogus.DataSets.Date().Past(),
            Name = "Assault",
            Sequence = 1,
            VictimName = "Paul Robertson",
            ChargeWording = $"""
            On the 1st day of June 2023 at ADELAIDE assaulted Paul Robertson.
            Section 20(3) of the Criminal Law Consolidation Act 1935.
            This is a Summary Offence.
            """
        },
        new Charge
        {
            Date = new Bogus.DataSets.Date().Past(),
            Name = "Theft",
            Sequence = 1,
            VictimName = "OTR Adelaide",
            ChargeWording = $"""
            On the 15th day of December 2023 in the said State, took property namely a screwdriver of a value less than $2500, dishonestly and without the consent of Fred Smith, the owner of the property, or intending to make a serious encroachment on the owner's proprietary rights.
            Section 134 of the Criminal Law Consolidation Act 1935.
            This is a Summary Offence.
            """
        },
        new Charge
        {
            Date = new Bogus.DataSets.Date().Past(),
            Name = "Drive disqualified",
            Sequence = 1,
            VictimName = null,
            ChargeWording = $"""
            On the 12th day of March 2023 in the said State, drove a motor vehicle on a road while disqualified from holding or obtaining a driver's licence.
            Section 91(5a) of the Motor Vehicles Act 1959.
            This is a Minor Indictable offence.
            """
        },
        new Charge
        {
            Date = new Bogus.DataSets.Date().Past(),
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

    static readonly List<Document> _caseFileDocuments = [
        new Document
        {
            FileName = "Letter from Prosecution.pdf",
            Title = "Letter to Prosecution",
            DocumentType = DocumentType.CaseFile

        },
        new Document
        {
            FileName = "Bail Application.pdf",
            Title = "Bail Application",
            DocumentType = DocumentType.CaseFile

        },
        new Document
        {
            FileName = "Letter to Defence.pdf",
            Title = "Letter to Defence",
            DocumentType = DocumentType.CaseFile

        },
        new Document
        {
            FileName = "Letter of Negotiation.pdf",
            Title = "Letter of Negotiation",
            DocumentType = DocumentType.CaseFile

        },
        new Document
        {
            FileName = "Application to Revoke Bail.pdf",
            Title = "Application to Revoke Bail",
            DocumentType = DocumentType.CaseFile

        },
    ];

    static readonly List<Document> _occurrenceDocuments = [
        new Document
        {
            FileName = "Police Notes.pdf",
            Title = "Police Notes",
            DocumentType = DocumentType.Occurrence
        },
        new Document
        {
            FileName = "Victim Statement.pdf",
            Title = "Victim Statement",
            DocumentType = DocumentType.Occurrence
        },
        new Document
        {
            FileName = "Witness Statement.pdf",
            Title = "Witness Statement",
            DocumentType = DocumentType.Occurrence
        },
        new Document
        {
            FileName = "Licence Disqualification.pdf",
            Title = "Licence Disqualification",
            DocumentType = DocumentType.Occurrence
        },
        new Document
        {
            FileName = "Forensic Statement of Analysis.pdf",
            Title = "Forensic Statement of Analysis",
            DocumentType = DocumentType.Occurrence
        },
        new Document
        {
            FileName = "Test Statement.pdf",
            Title = "Test Statement",
            DocumentType = DocumentType.Occurrence
        },
    ];

    static List<Defendant> RandomiseDefendants()
    {
        var defendants = Enumerable.Range(1, 40)
         .Select(i => new Defendant
         {
             Id = i,
             FirstName = _faker.Name.FirstName(),
             LastName = _faker.Name.LastName(),
             DateOfBirth = _faker.Person.DateOfBirth,
             Address = _faker.Address.FullAddress(),
             Phone = i % 3 == 0 ? "" : _faker.Phone.PhoneNumber(),
             Email = i % 3 == 0 ? "" : _faker.Person.Email,
             BailAgreements = Enumerable.Range(0, new Random().Next(0, 5))
                 .Select(i => new BailAgreement
                 {
                     DateEnteredInto = _faker.Date.Past(),
                     Conditions = Enumerable.Range(0, new Random().Next(1, _bailConditions.Count - 1))
                         .Select(i => new OrderCondition
                         {
                             Number = i,
                             Condition = _bailConditions[i]
                         }).ToList(),
                 }).ToList(),
             InterventionOrders = Enumerable.Range(0, new Random().Next(0, 5))
                 .Select(i => new InterventionOrder
                 {
                     ProtectedPerson = new Models.Person
                     {
                         FirstName = _faker.Name.FirstName(),
                         LastName = _faker.Name.LastName(),
                         DateOfBirth = _faker.Date.Past()
                     },
                     DateIssued = _faker.Date.Past(),
                     Conditions = Enumerable.Range(0, new Random().Next(1, _bailConditions.Count - 1))
                         .Select(i => new OrderCondition
                         {
                             Number = i,
                             Condition = _interventionOrderConditions[i]
                         }).ToList(),
                 }).ToList(),
         }).ToList();

        return defendants;
    }

    static Defendant CustodyDefendant()
    {
        var rand = new Random();
        return new Defendant
        {
            Id = rand.Next(100, 1000),
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            BailAgreements = Enumerable.Range(0, new Random().Next(0, 5))
                .Select(i => new BailAgreement
                {
                    DateEnteredInto = _faker.Date.Past(),
                    Conditions = Enumerable.Range(0, new Random().Next(1, _bailConditions.Count - 1))
                        .Select(i => new OrderCondition
                        {
                            Number = i,
                            Condition = _bailConditions[i]
                        }).ToList(),
                }).ToList(),
            InterventionOrders = Enumerable.Range(0, new Random().Next(0, 5))
                .Select(i => new InterventionOrder
                {
                    ProtectedPerson = new Models.Person
                    {
                        FirstName = _faker.Name.FirstName(),
                        LastName = _faker.Name.LastName(),
                        DateOfBirth = _faker.Date.Past()
                    },
                    DateIssued = _faker.Date.Past(),
                    Conditions = Enumerable.Range(0, new Random().Next(1, _bailConditions.Count - 1))
                        .Select(i => new OrderCondition
                        {
                            Number = i,
                            Condition = _interventionOrderConditions[i]
                        }).ToList(),
                }).ToList(),
        };
    }
    static List<T> RandomiseOrder<T>(this List<T> list)
    {
        var random = new Random();
        return [.. list.OrderBy(x => random.Next())];
    }

    private static readonly List<string> _factsOfCharge = [
        "BRIEF OVERVIEW:\n\nOn the 20th day of May 2024 in Adelaide, the accused Sarah Johnson engaged in disorderly behavior by shouting obscenities and causing a disturbance in a public park. \n\nPOLICE: Law enforcement officers responded to multiple reports of a disturbance in a public park in Adelaide on the afternoon of May 20th, 2024. Upon arrival, they found Sarah Johnson, visibly intoxicated, shouting obscenities and behaving aggressively towards park visitors. Despite repeated warnings to cease her disorderly behavior, Johnson continued to disrupt the peace and pose a threat to public safety. As a result, she was promptly arrested and escorted to the local police station for further processing. \n\nAt the police station, Johnson's unruly behavior persisted, necessitating additional measures to restrain her. After sobering up, Johnson was charged with disorderly conduct and released on bail pending her court appearance. \n\nACCUSED: Sarah Johnson, a 25-year-old resident of Adelaide, acknowledged her behavior but attributed it to alcohol consumption and personal stressors. She expressed remorse for her actions and pledged to seek counseling to address underlying issues contributing to her disorderly conduct.",
        "BRIEF OVERVIEW:\n\nOn the 14th day of March 2024 in Adelaide, the accused Fred Smith assaulted the victim James Peters by punching him in the face. \n\nVICTIM:\n\nJames Peters, a 30-year-old resident of Adelaide, was walking home from work on the evening of March 12th, 2024. As he passed by a local bar, he encountered Fred Smith, whom he recognized as an acquaintance from high school. Without any prior altercation or provocation, Smith suddenly approached Peters and punched him in the face, causing him to fall to the ground and sustain injuries to his nose and jaw. \n\nPOLICE:\n\nUpon receiving a distress call from a bystander who witnessed the assault, law enforcement officers rushed to the scene. Upon arrival, they found James Peters lying on the ground, clutching his face in pain. Peters provided a detailed statement recounting the events leading up to the assault, identifying Fred Smith as the assailant. Based on the victim's statement and corroborating witness testimonies, the police promptly located and apprehended Fred Smith nearby.\n\nFollowing standard procedure, Smith was transported to the City Watch House for processing and charging. During the arrest, Smith remained uncooperative and belligerent, repeatedly denying his involvement in the assault despite overwhelming evidence against him. Despite his protests, Smith was formally charged with assault and booked into custody pending further legal proceedings. \n\nACCUSED:\n\nFred Smith, a 32-year-old resident of Adelaide, adamantly denied any involvement in the assault upon his arrest. Claiming innocence, Smith asserted that he had not been present at the location of the incident and accused the authorities of mistaken identity. Despite Smith's protests, law enforcement officials remained steadfast in their assertion of his culpability, citing compelling eyewitness testimonies and forensic evidence linking him to the assault.",
        "BRIEF OVERVIEW:\n\nOn the 4th day of April 2024 in Adelaide, the accused James Peters stole a screwdriver from the victim Fred Smith's toolbox.\n\nVICTIM:\n\nFred Smith, a 35-year-old mechanic residing in Adelaide, had been working on his car in his garage on the afternoon of April 4th, 2024. He momentarily stepped away to retrieve a tool from his shed, leaving his toolbox unattended. Upon returning, Smith discovered that his screwdriver was missing from the toolbox.\n\nPOLICE:\n\nFred Smith promptly reported the theft to the local authorities, providing details of the stolen item and a description of a suspicious individual he had noticed in the vicinity earlier. Law enforcement officers from the Adelaide Police Department launched an investigation into the theft, canvassing the neighborhood for potential witnesses and gathering evidence. Utilizing CCTV footage from nearby establishments, the police identified James Peters as the prime suspect.\n\nFollowing diligent efforts, the police located Peters and apprehended him for questioning. Peters eventually confessed to stealing the screwdriver and led the authorities to its location. He was subsequently arrested and charged with theft.\n\nACCUSED:\n\nJames Peters, a 28-year-old resident of Adelaide, initially denied any involvement in the theft when confronted by law enforcement. However, upon further interrogation and the presentation of incriminating evidence, Peters admitted to stealing the screwdriver from Fred Smith's toolbox. Despite his acknowledgment of the crime, Peters claimed it was a spur-of-the-moment decision driven by financial desperation.",
        "BRIEF OVERVIEW:\n\nOn the 12th day of July 2024 in Adelaide, the accused John Doe drove a motor vehicle on a public road while disqualified from holding or obtaining a driver's license. \n\nPOLICE:\n\nLaw enforcement officers conducting routine traffic patrols in Adelaide on the evening of July 12th, 2024, observed a vehicle being driven erratically on a public road. Upon signaling the driver to pull over, they discovered that the driver, John Doe, was disqualified from holding or obtaining a driver's license due to prior traffic offenses. Doe was immediately arrested and charged with driving while disqualified, a serious traffic violation. \n\nACCUSED:\n\nJohn Doe, a 30-year-old resident of Adelaide, acknowledged his disqualification from driving but claimed ignorance of the legal consequences. He expressed regret for his actions and accepted responsibility for the offense, pledging to comply with the law in the future.",
        "BRIEF OVERVIEW:\n\nOn the 8th day of June 2024 in Adelaide, the accused Betty Ingridson deceived a local business owner, Fred Smith, by posing as a customer and stealing $500 cash from the register. \n\nVICTIM:\n\nFred Smith, a 40-year-old small business owner in Adelaide, was operating his convenience store on the morning of June 8th, 2024. Betty Ingridson entered the store and engaged Smith in conversation, distracting him while she surreptitiously pocketed $500 from the cash register. \n\nPOLICE:\n\nFred Smith discovered the missing cash during a routine register check and reviewed the store's security footage, which captured Ingridson's theft. He promptly reported the incident to the Adelaide Police Department, providing a description of the suspect and the stolen amount. Law enforcement officers launched an investigation into the theft, identifying Ingridson as the perpetrator based on witness testimonies and video evidence. \n\nIngridson was located and apprehended by the police, who recovered the stolen cash from her possession. She was arrested and charged with deception, with the stolen money returned to Smith. \n\nACCUSED: \n\nBetty Ingridson, a 35-year-old resident of Adelaide, initially denied any involvement in the theft when questioned by the authorities. However, when presented with irrefutable evidence of her actions, Ingridson confessed to the crime and expressed remorse for her behavior. She cited financial difficulties and personal struggles as contributing factors to her deceptive actions. BRIEF OVERVIEW:\n\nOn the 8th day of June 2024 in Adelaide, the accused Betty Ingridson deceived a local business owner, Fred Smith, by posing as a customer and stealing $500 cash from the register. \n\nVICTIM:\n\nFred Smith, a 40-year-old small business owner in Adelaide, was operating his convenience store on the morning of June 8th, 2024. Betty Ingridson entered the store and engaged Smith in conversation, distracting him while she surreptitiously pocketed $500 from the cash register. \n\nPOLICE:\n\nFred Smith discovered the missing cash during a routine register check and reviewed the store's security footage, which captured Ingridson's theft. He promptly reported the incident to the Adelaide Police Department, providing a description of the suspect and the stolen amount. Law enforcement officers launched an investigation into the theft, identifying Ingridson as the perpetrator based on witness testimonies and video evidence. \n\nIngridson was located and apprehended by the police, who recovered the stolen cash from her possession. She was arrested and charged with deception, with the stolen money returned to Smith. \n\nACCUSED: \n\nBetty Ingridson, a 35-year-old resident of Adelaide, initially denied any involvement in the theft when questioned by the authorities. However, when presented with irrefutable evidence of her actions, Ingridson confessed to the crime and expressed remorse for her behavior. She cited financial difficulties and personal struggles as contributing factors to her deceptive actions.",
    ];

    private static readonly List<string> _bailConditions = [
        "Must not leave the state",
        "Must not contact the victim",
        "Must not consume alcohol",
        "Must not consume drugs",
        "Must not drive a motor vehicle",
        "Must not enter the Adelaide CBD",
        "Be of good behaviour",
        "Report to the police station daily",
        "Remain at home between the hours of 8pm and 6am",
    ];

    private static readonly List<string> _interventionOrderConditions = [
        "Must not assault, threaten, harass or intimidate the protected person(s)",
        "Must not damage or threaten to damage the protected person(s) property",
        "Must not encourage or assist anyone else to do any of the above",
        "Must not contact the protected person(s) by any means",
        "Must not approach the protected person(s) within 100 metres",
        "Must not go to the protected person(s) home, work or any other place they frequent",
        "Must not go to the protected person(s) children's school or any other place they frequent",
    ];

    private static readonly List<string> _hearingNotes = [
        "",
        "Part heard for sentencing",
        "List for trial on the next occasion",
        "No further adjournments for legal advice",
    ];

    private static readonly List<string> _cfelEntries = [
        "Reviewed defense letter asserting difficulties in proving identity. Noted potential challenges and forwarded the matter to the investigating officer for further evidence gathering.\n\nRequested PD90 form from the investigating officer to obtain additional material relevant to the case.",
        "Called victim to update on case progress and discuss potential court dates. Advised victim of upcoming court appearance and provided details for attendance.\n\nNoted victim's concerns regarding safety and arranged for security measures to be in place during court proceedings.",
        "Received forensic analysis report from FSSA. Reviewed findings and identified potential discrepancies in the evidence. Requested further clarification from the forensic analyst to address inconsistencies.\n\nNoted discrepancies in witness statements and arranged for additional interviews to resolve conflicting accounts.",
        "Conducted case review with investigating officer to assess evidence and identify potential gaps in the prosecution's case. Discussed strategies for strengthening the case and addressing weaknesses in the evidence.\n\nReviewed legal submissions from defense counsel and prepared responses to counter arguments raised in the defense letter.",
        "File adjudicated, sufficient to lay."
    ];
    private static readonly string[] _appearanceTypes = ["First Appearance", "Mention", "Pretrial Conference", "Trial"];

    private static readonly TimeOnly[] _sittingTimes = [new(9, 30), new(10, 0), new(11, 30), new(14, 15)];

    public enum DocumentType
    {
        CaseFile,
        Occurrence
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            var val = list[k];
            list[k] = list[n];
            list[n] = val;
        }
    }
}
