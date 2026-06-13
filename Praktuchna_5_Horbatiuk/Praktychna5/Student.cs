using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Praktychna5;

public class Student : Person, ICloneable
{
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private int _courseProgress;

    public byte[] LabGrades { get; private set; } = new byte[10];
    public List<GradePoint> GradePoints { get; } = new();
    public GradeJournal Journal { get; } = new GradeJournal();
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    public int CourseProgress
    {
        get => _courseProgress;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Прогрес має бути від 0 до 100.");
            _courseProgress = value;
        }
    }

    public string RecordBookNumber
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

    // Конструктор викликає конструктор базового класу Person через base.
    public Student(string fullName, DateTime dateOfBirth, string recordBookNumber)
        : base(fullName, dateOfBirth)
    {
        RecordBookNumber = recordBookNumber;
    }

    public void UpdateAverageGrade(double newGrade) => AverageGrade = newGrade;

    public void RecalculateFromJournal()
    {
        _averageGrade = Math.Round(Journal.CalculateAverage(), 2);
    }

    public bool IsExcellent() => AverageGrade >= 90;
    public bool IsFailing() => AverageGrade < 60;

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

    // Перевизначення віртуального методу базового класу.
    public override string GetInfo()
    {
        return $"Студент {FullName}, заліковка {RecordBookNumber}, середній бал {AverageGrade:F2}";
    }

    // Реалізація абстрактного методу з UniversityMember.
    public override decimal CalculateScholarship()
    {
        if (AverageGrade >= 90) return 2000m;
        if (AverageGrade >= 75) return 1300m;
        return 0m;
    }

    // Перевизначення віртуального Enroll.
    public override void Enroll()
    {
        Status = StudentStatus.Active;
        Console.WriteLine($"Студента {FullName} зараховано до групи.");
    }

    public string GetFormattedInfo(bool detailed = false)
    {
        var sb = new StringBuilder();

        if (!detailed)
        {
            sb.Append(FullName);
            sb.Append(" (заліковка: ").Append(RecordBookNumber).Append(")");
            sb.Append(", середній бал: ").Append(AverageGrade.ToString("F2"));
            sb.Append(", прогрес: ").Append(CourseProgress).Append("%");
            sb.Append(", статус: ").Append(Status);
            return sb.ToString();
        }

        sb.AppendLine($"ПІБ: {FullName}");
        sb.AppendLine($"Заліковка: {RecordBookNumber}");
        sb.AppendLine($"Вік: {Age}, статус: {Status}");
        sb.AppendLine($"Середній бал: {AverageGrade:F2}");
        sb.AppendLine($"Прогрес навчання: {CourseProgress}%");
        sb.AppendLine($"Виконано лабораторних: {CompletedLabsCount}/10");
        sb.AppendLine($"Оцінок (GradePoint): {GradePoints.Count}");
        sb.AppendLine($"Email: {PersonalEmail ?? "не вказано"}");
        sb.AppendLine($"Нотатки: {(string.IsNullOrEmpty(Notes) ? "немає" : Notes)}");
        return sb.ToString();
    }

    public bool ContainsKeyword(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return false;
        string key = keyword.Trim().ToLower();

        if (FullName.ToLower().Contains(key)) return true;
        if (!string.IsNullOrEmpty(Notes) && Notes.ToLower().Contains(key)) return true;
        if (!string.IsNullOrEmpty(PersonalEmail) && PersonalEmail.ToLower().Contains(key)) return true;

        return false;
    }

    public static bool operator >(Student a, Student b) => a.AverageGrade > b.AverageGrade;
    public static bool operator <(Student a, Student b) => a.AverageGrade < b.AverageGrade;
    public static bool operator >=(Student a, Student b) => a.AverageGrade >= b.AverageGrade;
    public static bool operator <=(Student a, Student b) => a.AverageGrade <= b.AverageGrade;

    public static bool operator ==(Student? a, Student? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.AverageGrade == b.AverageGrade && a.CourseProgress == b.CourseProgress;
    }

    public static bool operator !=(Student? a, Student? b) => !(a == b);

    public override bool Equals(object? obj) => obj is Student s && this == s;
    public override int GetHashCode() => HashCode.Combine(AverageGrade, CourseProgress);

    public static Student operator +(Student a, Student b)
    {
        string surnameA = a.FullName.Split(' ')[0];
        string surnameB = b.FullName.Split(' ')[0];

        var team = new Student($"Команда {surnameA} та {surnameB}", a.DateOfBirth, "00000000")
        {
            CourseProgress = (a.CourseProgress + b.CourseProgress) / 2
        };
        team.UpdateAverageGrade(Math.Round((a.AverageGrade + b.AverageGrade) / 2, 2));

        foreach (var gp in a.GradePoints) team.GradePoints.Add(gp);
        foreach (var gp in b.GradePoints) team.GradePoints.Add(gp);

        return team;
    }

    public object Clone()
    {
        var copy = new Student(FullName, DateOfBirth, RecordBookNumber)
        {
            EnrollmentDate = this.EnrollmentDate,
            Status = this.Status,
            PersonalEmail = this.PersonalEmail,
            Notes = this.Notes,
            CourseProgress = this.CourseProgress
        };

        copy.LabGrades = (byte[])this.LabGrades.Clone();
        foreach (var gp in this.GradePoints) copy.GradePoints.Add(new GradePoint(gp.Value));
        foreach (var kvp in this.Journal.SubjectGrades) copy.Journal.SetGrade(kvp.Key, kvp.Value);

        copy._averageGrade = this._averageGrade;
        return copy;
    }
}
