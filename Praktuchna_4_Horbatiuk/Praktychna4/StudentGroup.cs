using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Praktychna4;

public class StudentGroup
{
    public string GroupName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int Course { get; set; }

    private List<Student> _students = new();

    public int GroupSize => _students.Count;

    public double AverageGroupGrade =>
        _students.Count == 0 ? 0 : Math.Round(_students.Average(s => s.AverageGrade), 2);

    // Індексатор: швидкий пошук студента за номером заліковки.
    public Student? this[string recordBookNumber]
        => _students.FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

    public void AddStudent(Student s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (_students.Any(x => x.RecordBookNumber == s.RecordBookNumber))
            throw new InvalidOperationException("Студент з таким номером заліковки вже існує.");
        _students.Add(s);
    }

    public bool RemoveStudent(string recordBookNumber)
        => _students.RemoveAll(s => s.RecordBookNumber == recordBookNumber) > 0;

    public Student? FindStudent(string recordBookNumber)
        => _students.FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

    public List<Student> GetExcellentStudents() => _students.Where(s => s.IsExcellent()).ToList();
    public List<Student> GetFailingStudents() => _students.Where(s => s.IsFailing()).ToList();
    public List<Student> GetStudentsByStatus(StudentStatus status) =>
        _students.Where(s => s.Status == status).ToList();
    public List<Student> GetAllStudents() => _students.ToList();

    public double ExcellentPercent =>
        GroupSize == 0 ? 0 : Math.Round(GetExcellentStudents().Count * 100.0 / GroupSize, 1);

    // Оператор + : об'єднання двох груп (дублікати за заліковкою пропускаються).
    public static StudentGroup operator +(StudentGroup a, StudentGroup b)
    {
        var merged = new StudentGroup
        {
            GroupName = $"{a.GroupName}+{b.GroupName}",
            Specialization = a.Specialization,
            Course = a.Course
        };

        foreach (var s in a._students)
            merged._students.Add(s);
        foreach (var s in b._students)
            if (!merged._students.Any(x => x.RecordBookNumber == s.RecordBookNumber))
                merged._students.Add(s);

        return merged;
    }

    public StudentGroup MergeGroups(StudentGroup other) => this + other;

    // Найкращий студент за перевантаженим оператором порівняння.
    public Student? BestStudent()
    {
        if (_students.Count == 0) return null;
        Student best = _students[0];
        foreach (var s in _students)
            if (s > best) best = s;
        return best;
    }

    public string SearchByNameFragment(string fragment)
    {
        if (string.IsNullOrWhiteSpace(fragment))
            return "Порожній запит для пошуку.";

        string lower = fragment.Trim().ToLower();
        var found = _students
            .Where(s => s.FullName.ToLower().Contains(lower))
            .ToList();

        if (found.Count == 0)
            return $"За фрагментом \"{fragment}\" нікого не знайдено.";

        var sb = new StringBuilder();
        sb.AppendLine($"Знайдено {found.Count} студент(ів) за фрагментом \"{fragment}\":");
        int i = 1;
        foreach (var s in found)
            sb.AppendLine($"  {i++}. {s.FullName} (заліковка: {s.RecordBookNumber})");
        return sb.ToString();
    }

    public string ExportToCsv()
    {
        var sb = new StringBuilder();
        sb.AppendLine("ПІБ;Заліковка;ДатаНародження;Статус;СереднійБал;Прогрес;Email");

        foreach (var s in _students)
        {
            sb.Append(s.FullName).Append(';');
            sb.Append(s.RecordBookNumber).Append(';');
            sb.Append(s.DateOfBirth.ToString("dd.MM.yyyy")).Append(';');
            sb.Append(s.Status).Append(';');
            sb.Append(s.AverageGrade.ToString("F2")).Append(';');
            sb.Append(s.CourseProgress).Append(';');
            sb.AppendLine(s.PersonalEmail ?? "");
        }
        return sb.ToString();
    }

    // Кожен рядок: ПІБ;Заліковка;ДатаНародження (дата необов'язкова).
    public void ImportStudentsFromText(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            throw new ArgumentException("Текст для імпорту порожній.");

        string[] lines = rawText.Split(
            new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string[] parts = line.Split(';');
            if (parts.Length < 2) continue;

            string fullName = parts[0].Trim();
            string recordBook = parts[1].Trim();

            DateTime dob = new DateTime(2005, 1, 1);
            if (parts.Length >= 3 && DateTime.TryParse(parts[2].Trim(), out var parsed))
                dob = parsed;

            try
            {
                var student = new Student
                {
                    FullName = fullName,
                    RecordBookNumber = recordBook,
                    DateOfBirth = dob
                };
                AddStudent(student);
            }
            catch
            {
                continue;
            }
        }
    }

    public void SaveToFile(string filename)
    {
        var data = new
        {
            GroupName,
            Specialization,
            Course,
            Students = _students
        };
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };
        File.WriteAllText(filename, JsonSerializer.Serialize(data, options));
    }

    public void LoadFromFile(string filename)
    {
        if (!File.Exists(filename))
            throw new FileNotFoundException("Файл не знайдено.", filename);

        var json = File.ReadAllText(filename);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        GroupName = root.GetProperty("GroupName").GetString() ?? string.Empty;
        Specialization = root.GetProperty("Specialization").GetString() ?? string.Empty;
        Course = root.GetProperty("Course").GetInt32();

        _students.Clear();
        foreach (var item in root.GetProperty("Students").EnumerateArray())
        {
            var student = new Student
            {
                RecordBookNumber = item.GetProperty("RecordBookNumber").GetString()!,
                FullName = item.GetProperty("FullName").GetString()!,
                DateOfBirth = item.GetProperty("DateOfBirth").GetDateTime(),
                EnrollmentDate = item.GetProperty("EnrollmentDate").GetDateTime(),
                Status = Enum.Parse<StudentStatus>(item.GetProperty("Status").GetString()!),
                PersonalEmail = item.TryGetProperty("PersonalEmail", out var em) ? em.GetString() : null,
                Notes = item.TryGetProperty("Notes", out var nt) ? nt.GetString() : null,
                CourseProgress = item.TryGetProperty("CourseProgress", out var cp) ? cp.GetInt32() : 0
            };

            if (item.TryGetProperty("LabGrades", out var labArr) &&
                labArr.ValueKind == JsonValueKind.Array)
            {
                int i = 0;
                foreach (var g in labArr.EnumerateArray())
                {
                    if (i < 10) student.LabGrades[i++] = g.GetByte();
                }
            }

            if (item.TryGetProperty("Journal", out var journal) &&
                journal.TryGetProperty("SubjectGrades", out var grades))
            {
                foreach (var g in grades.EnumerateObject())
                    student.Journal.SetGrade(g.Name, g.Value.GetDouble());
            }
            student.RecalculateFromJournal();
            _students.Add(student);
        }
    }
}
