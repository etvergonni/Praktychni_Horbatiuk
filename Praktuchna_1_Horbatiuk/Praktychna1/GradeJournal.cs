using System;
using System.Collections.Generic;
using System.Linq;

namespace Praktychna1;


public class GradeJournal
{

    private Dictionary<string, double> _subjectGrades = new();


    public IReadOnlyDictionary<string, double> SubjectGrades => _subjectGrades;


    public void SetGrade(string subject, double grade)
    {
        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Назва предмета не може бути порожньою.");
        if (grade < 0 || grade > 100)
            throw new ArgumentOutOfRangeException(nameof(grade), "Оцінка має бути від 0 до 100.");
        _subjectGrades[subject.Trim()] = Math.Round(grade, 2);
    }


    public bool RemoveSubject(string subject) => _subjectGrades.Remove(subject);


    public double CalculateAverage()
    {
        if (_subjectGrades.Count == 0) return 0;
        return Math.Round(_subjectGrades.Values.Average(), 2);
    }


    public int SubjectCount => _subjectGrades.Count;


    public string GetGradesSummary()
    {
        if (_subjectGrades.Count == 0) return "Оцінок ще немає.";
        return string.Join(", ", _subjectGrades.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }


    public void Clear() => _subjectGrades.Clear();
}
