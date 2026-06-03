using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Praktychna2;

// 
// Реалізовано в гілці feature/student-lab-grades
// Додано одновимірний масив byte[] LabGrades[10]
// 
public class Student : ICloneable
{

    private string _fullName = string.Empty;
    private string _recordBookNumber = string.Empty;
    private string? _personalEmail;
    private double _averageGrade;


    public byte[] LabGrades { get; private set; } = new byte[10];


    public int PortRow { get; set; } = -1;
    public int PortCol { get; set; } = -1;
    public bool IsAssignedToPort => PortRow >= 0 && PortCol >= 0;


    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 5)
                throw new ArgumentException("ПІБ має містити мінімум 5 символів.");
            _fullName = value.Trim();
        }
    }

    public DateTime DateOfBirth { get; init; }
    public int Age => CalculateAge();

    public required string RecordBookNumber
    {
        get => _recordBookNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 8 || !value.All(char.IsDigit))
                throw new ArgumentException("Номер залікової книжки має містити рівно 8 цифр.");
            _recordBookNumber = value;
        }
    }

    public double AverageGrade
    {
        get => _averageGrade;
        private set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Середній бал має бути від 0 до 100.");
            _averageGrade = Math.Round(value, 2);
        }
    }

    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public DateTime EnrollmentDate { get; init; } = DateTime.Now;

    public string? PersonalEmail
    {
        get => _personalEmail;
        set
        {
            if (!string.IsNullOrEmpty(value) &&
                !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Невірний формат електронної пошти.");
            _personalEmail = value;
        }
    }

    public string? Notes { get; set; }
    public GradeJournal Journal { get; init; } = new GradeJournal();


    public int CalculateAge()
    {
        var today = DateTime.Today;
        int age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return Math.Max(0, age);
    }

    public void UpdateAverageGrade(double newGrade) => AverageGrade = newGrade;

    public void RecalculateFromJournal()
    {
        _averageGrade = Math.Round(Journal.CalculateAverage(), 2);
    }

    public bool IsExcellent() => AverageGrade >= 90;
    public bool IsFailing() => AverageGrade < 60;

    public int GetYearsToGraduation()
    {
        int years = 4 - (DateTime.Now.Year - EnrollmentDate.Year);
        return Math.Max(0, years);
    }


    public void AddLabGrade(int labNumber, byte grade)
    {
        if (labNumber < 1 || labNumber > 10)
            throw new IndexOutOfRangeException($"Номер лабораторної має бути від 1 до 10 (отримано: {labNumber}).");
        if (grade > 100)
            throw new ArgumentOutOfRangeException(nameof(grade), "Оцінка має бути від 0 до 100.");

        LabGrades[labNumber - 1] = grade;
    }


    public double GetAverageLabGrade()
    {
        var marks = LabGrades.Where(g => g > 0).ToArray();
        if (marks.Length == 0) return 0;
        return Math.Round(marks.Average(g => (double)g), 2);
    }

  
    public int CompletedLabsCount => LabGrades.Count(g => g > 0);

    
    public byte[] GetSortedLabGrades()
    {
        var copy = (byte[])LabGrades.Clone();
        Array.Sort(copy);
        return copy;
    }

    
    public string GetDetailedInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ПІБ: {FullName}");
        sb.AppendLine($"Заліковка: {RecordBookNumber}");
        sb.AppendLine($"Дата народження: {DateOfBirth:dd.MM.yyyy} (вік: {Age})");
        sb.AppendLine($"Зарахування: {EnrollmentDate:dd.MM.yyyy}");
        sb.AppendLine($"До випуску: {GetYearsToGraduation()} р.");
        sb.AppendLine($"Статус: {Status}");
        sb.AppendLine($"Email: {PersonalEmail ?? "не вказано"}");
        sb.AppendLine($"Середній бал: {AverageGrade:F2}");
        sb.AppendLine($"Оцінки: {Journal.GetGradesSummary()}");

  
        sb.Append("Лабораторні (");
        sb.Append(CompletedLabsCount);
        sb.AppendLine("/10):");
        sb.Append("  [");
        for (int i = 0; i < LabGrades.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(LabGrades[i] == 0 ? "—" : LabGrades[i].ToString());
        }
        sb.AppendLine("]");
        if (CompletedLabsCount > 0)
            sb.AppendLine($"  Середня лабораторна: {GetAverageLabGrade():F2}");


        if (IsAssignedToPort)
            sb.AppendLine($"Прив'язано до порту: ({PortRow}, {PortCol})");
        else
            sb.AppendLine("Прив'язано до порту: ні");

        if (!string.IsNullOrEmpty(Notes))
            sb.AppendLine($"Нотатки: {Notes}");

        return sb.ToString();
    }

    
    public void ShowDetailedInfo()
    {
        Console.Write(GetDetailedInfo());
    }

    public object Clone()
    {
        var copy = new Student
        {
            FullName = this.FullName,
            RecordBookNumber = this.RecordBookNumber,
            DateOfBirth = this.DateOfBirth,
            EnrollmentDate = this.EnrollmentDate,
            Status = this.Status,
            PersonalEmail = this.PersonalEmail,
            Notes = this.Notes,
            PortRow = this.PortRow,
            PortCol = this.PortCol
        };

        copy.LabGrades = (byte[])this.LabGrades.Clone();

   
        foreach (var kvp in this.Journal.SubjectGrades)
            copy.Journal.SetGrade(kvp.Key, kvp.Value);

        copy._averageGrade = this._averageGrade;
        return copy;
    }
}
