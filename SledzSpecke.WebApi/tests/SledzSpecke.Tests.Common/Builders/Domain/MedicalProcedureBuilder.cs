using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using System;
using System.Linq;

namespace SledzSpecke.Tests.Common.Builders.Domain;

public class MedicalProcedureBuilder : TestDataBuilder<Procedure>
{
    private int _id = 1;
    private int _internshipId = 1;
    private DateTime _date = DateTime.Today;
    private string _name;
    private string _category;
    private string _icdCode;
    private string _description;
    private bool _supervised = true;
    private int? _patientAge;
    private string _supervisorName;
    private string _supervisorPwz;
    
    // Polish medical procedures with real ICD-10 codes
    private readonly (string Name, string Category, string IcdCode, string Description)[] _procedures = new[]
    {
        // Cardiology procedures
        ("Echokardiografia przezklatkowa", "Kardiologia", "88.72", "Badanie echokardiograficzne serca"),
        ("Koronarografia", "Kardiologia", "88.56", "Cewnikowanie serca z koronarografią"),
        ("Test wysiłkowy EKG", "Kardiologia", "89.41", "Elektrokardiograficzna próba wysiłkowa"),
        ("Holter EKG", "Kardiologia", "89.50", "24-godzinne monitorowanie EKG"),
        ("Angioplastyka wieńcowa", "Kardiologia", "00.66", "Przezskórna angioplastyka naczyń wieńcowych"),
        
        // Internal Medicine procedures
        ("Gastroskopia", "Choroby wewnętrzne", "45.13", "Ezofagogastroduodenoskopia"),
        ("Kolonoskopia", "Choroby wewnętrzne", "45.23", "Kolonoskopia z lub bez biopsji"),
        ("Biopsja wątroby", "Choroby wewnętrzne", "50.11", "Biopsja wątroby igłowa"),
        ("Punkcja lędźwiowa", "Neurologia", "03.31", "Nakłucie lędźwiowe"),
        ("Bronchoskopia", "Pulmonologia", "33.23", "Bronchoskopia światłowodowa"),
        
        // Anesthesiology procedures
        ("Intubacja dotchawicza", "Anestezjologia", "96.04", "Wprowadzenie rurki dotchawiczej"),
        ("Kaniulacja żyły centralnej", "Anestezjologia", "38.93", "Cewnikowanie żyły"),
        ("Znieczulenie podpajęczynówkowe", "Anestezjologia", "03.91", "Iniekcja środka znieczulającego do przestrzeni podpajęczynówkowej"),
        ("Blokada splotu ramiennego", "Anestezjologia", "04.81", "Wstrzyknięcie środka znieczulającego do nerwu obwodowego"),
        
        // Emergency procedures
        ("Resuscytacja krążeniowo-oddechowa", "Medycyna ratunkowa", "99.60", "Resuscytacja krążeniowo-oddechowa"),
        ("Defibrylacja", "Medycyna ratunkowa", "99.62", "Defibrylacja serca"),
        ("Intubacja w trybie pilnym", "Medycyna ratunkowa", "96.04", "Nagła intubacja dotchawicza")
    };
    
    public MedicalProcedureBuilder()
    {
        var procedure = Faker.PickRandom(_procedures);
        _name = procedure.Name;
        _category = procedure.Category;
        _icdCode = procedure.IcdCode;
        _description = procedure.Description;
        _supervisorName = GeneratePolishDoctorName();
        _supervisorPwz = GeneratePwz();
    }
    
    public MedicalProcedureBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public MedicalProcedureBuilder ForInternship(int internshipId)
    {
        _internshipId = internshipId;
        return this;
    }
    
    public MedicalProcedureBuilder OnDate(DateTime date)
    {
        _date = date;
        return this;
    }
    
    public MedicalProcedureBuilder AsUnsupervised()
    {
        _supervised = false;
        _supervisorName = null;
        _supervisorPwz = null;
        return this;
    }
    
    public MedicalProcedureBuilder WithSpecificProcedure(string category)
    {
        var procedure = _procedures.FirstOrDefault(p => p.Category == category);
        if (procedure != default)
        {
            _name = procedure.Name;
            _category = procedure.Category;
            _icdCode = procedure.IcdCode;
            _description = procedure.Description;
        }
        return this;
    }
    
    public MedicalProcedureBuilder ForPatientAge(int age)
    {
        _patientAge = age;
        return this;
    }
    
    private string GeneratePolishDoctorName()
    {
        var titles = new[] { "dr", "dr hab.", "prof. dr hab." };
        var firstNames = new[] 
        { 
            "Anna", "Maria", "Katarzyna", "Małgorzata", "Agnieszka",
            "Piotr", "Krzysztof", "Andrzej", "Tomasz", "Paweł"
        };
        var lastNames = new[] 
        { 
            "Nowak", "Kowalski", "Wiśniewski", "Wójcik", "Kowalczyk",
            "Kamiński", "Lewandowski", "Zieliński", "Szymański", "Woźniak"
        };
        
        return $"{Faker.PickRandom(titles)} {Faker.PickRandom(firstNames)} {Faker.PickRandom(lastNames)}";
    }
    
    private string GeneratePwz()
    {
        return Faker.Random.Replace("#######");
    }
    
    public override Procedure Build()
    {
        return Procedure.Create(
            id: new ProcedureId(_id),
            moduleId: new ModuleId(1), // Default module ID
            internshipId: new InternshipId(_internshipId),
            date: _date,
            code: _icdCode,
            name: _name,
            location: "Szpital Uniwersytecki", // Default location
            executionType: _supervised ? ProcedureExecutionType.CodeA : ProcedureExecutionType.CodeB,
            supervisorName: _supervisorName,
            year: _date.Year,
            smkVersion: SmkVersion.New // Default to new SMK version
        );
    }
}