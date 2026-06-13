using System;
using System.Text.RegularExpressions;

namespace Praktychna5;

public abstract class Person : UniversityMember
{
    private string _fullName = string.Empty;
    private string? _personalEmail;

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ПІБ не може бути порожнім.");

            string[] parts = value.Trim().Split(
                new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
                throw new ArgumentException(
                    "ПІБ має містити щонайменше три слова: прізвище, ім'я, по батькові.");

            _fullName = string.Join(" ", parts);
        }
    }

    public DateTime DateOfBirth { get; set; }

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

    public int Age => CalculateAge();

    // Конструктор базового класу: похідні викликають його через base(...).
    protected Person(string fullName, DateTime dateOfBirth)
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
    }

    public int CalculateAge()
    {
        var today = DateTime.Today;
        int age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return Math.Max(0, age);
    }

    public override string GetInfo()
    {
        return $"{FullName}, вік: {Age}";
    }
}
