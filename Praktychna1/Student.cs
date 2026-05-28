using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Praktychna1; //meow


public class Student
{

    private string _fullName = string.Empty;
    private string _recordBookNumber = string.Empty;
    private string? _personalEmail;
    private double _averageGrade;


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

    public void UpdateAverageGrade(double newGrade)
    {
        AverageGrade = newGrade; 
    }

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

    public void ShowDetailedInfo()
    {
        Console.WriteLine();
        Console.WriteLine($"ПІБ: {FullName}");
        Console.WriteLine($"Заліковка: {RecordBookNumber}");
        Console.WriteLine($"Дата народження: {DateOfBirth:dd.MM.yyyy} (вік: {Age})");
        Console.WriteLine($"Зарахування: {EnrollmentDate:dd.MM.yyyy}");
        Console.WriteLine($"До випуску: {GetYearsToGraduation()} р.");
        Console.WriteLine($"Статус: {Status}");
        Console.WriteLine($"Email: {PersonalEmail ?? "не вказано"}");
        Console.WriteLine($"Середній бал: {AverageGrade:F2}");
        Console.WriteLine($"Оцінки: {Journal.GetGradesSummary()}");
        if (!string.IsNullOrEmpty(Notes))
            Console.WriteLine($"Нотатки: {Notes}");
    }
}
