using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Praktychna5;

public partial class StudentGroup
{
    public string GroupName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int Course { get; set; }

    private List<UniversityMember> _members = new();

    public int GroupSize => _members.OfType<Student>().Count();
    public int MembersCount => _members.Count;

    public double AverageGroupGrade
    {
        get
        {
            var students = _members.OfType<Student>().ToList();
            return students.Count == 0 ? 0 : Math.Round(students.Average(s => s.AverageGrade), 2);
        }
    }

    public Student? this[string recordBookNumber]
        => _members.OfType<Student>().FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

    public void AddStudent(Student s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (_members.OfType<Student>().Any(x => x.RecordBookNumber == s.RecordBookNumber))
            throw new InvalidOperationException("Студент з таким номером заліковки вже існує.");
        _members.Add(s);
    }

    public bool RemoveStudent(string recordBookNumber)
        => _members.RemoveAll(m => m is Student s && s.RecordBookNumber == recordBookNumber) > 0;

    public Student? FindStudent(string recordBookNumber)
        => _members.OfType<Student>().FirstOrDefault(s => s.RecordBookNumber == recordBookNumber);

    public List<Student> GetExcellentStudents() => _members.OfType<Student>().Where(s => s.IsExcellent()).ToList();
    public List<Student> GetFailingStudents() => _members.OfType<Student>().Where(s => s.IsFailing()).ToList();
    public List<Student> GetStudentsByStatus(StudentStatus status) =>
        _members.OfType<Student>().Where(s => s.Status == status).ToList();
    public List<Student> GetAllStudents() => _members.OfType<Student>().ToList();
    public List<UniversityMember> GetAllMembers() => _members.ToList();

    public double ExcellentPercent
    {
        get
        {
            int total = GroupSize;
            return total == 0 ? 0 : Math.Round(GetExcellentStudents().Count * 100.0 / total, 1);
        }
    }

    // Об'єднання двох груп (дублікати студентів за заліковкою пропускаються).
    public static StudentGroup operator +(StudentGroup a, StudentGroup b)
    {
        var merged = new StudentGroup
        {
            GroupName = $"{a.GroupName}+{b.GroupName}",
            Specialization = a.Specialization,
            Course = a.Course
        };

        foreach (var m in a._members)
            merged._members.Add(m);

        foreach (var m in b._members)
        {
            if (m is Student s)
            {
                if (!merged._members.OfType<Student>().Any(x => x.RecordBookNumber == s.RecordBookNumber))
                    merged._members.Add(m);
            }
            else
            {
                merged._members.Add(m);
            }
        }

        return merged;
    }

    public StudentGroup MergeGroups(StudentGroup other) => this + other;

    public Student? BestStudent()
    {
        var students = _members.OfType<Student>().ToList();
        if (students.Count == 0) return null;
        Student best = students[0];
        foreach (var s in students)
            if (s > best) best = s;
        return best;
    }

    public string SearchByNameFragment(string fragment)
    {
        if (string.IsNullOrWhiteSpace(fragment))
            return "Порожній запит для пошуку.";

        string lower = fragment.Trim().ToLower();
        var found = _members.OfType<Student>()
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

        foreach (var s in _members.OfType<Student>())
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
                AddStudent(new Student(fullName, dob, recordBook));
            }
            catch
            {
                continue;
            }
        }
    }

    // Зберігаються лише студенти (базові дані); похідні типи зберігаються як студенти.
    public void SaveToFile(string filename)
    {
        var students = _members.OfType<Student>()
            .Select(s => new
            {
                s.FullName,
                s.RecordBookNumber,
                s.DateOfBirth,
                s.EnrollmentDate,
                Status = s.Status.ToString(),
                s.PersonalEmail,
                s.Notes,
                s.CourseProgress,
                Subjects = s.Journal.SubjectGrades
            })
            .ToList();

        var data = new { GroupName, Specialization, Course, Students = students };
        var options = new JsonSerializerOptions { WriteIndented = true };
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

        _members.Clear();
        foreach (var item in root.GetProperty("Students").EnumerateArray())
        {
            var student = new Student(
                item.GetProperty("FullName").GetString()!,
                item.GetProperty("DateOfBirth").GetDateTime(),
                item.GetProperty("RecordBookNumber").GetString()!)
            {
                EnrollmentDate = item.GetProperty("EnrollmentDate").GetDateTime(),
                Status = Enum.Parse<StudentStatus>(item.GetProperty("Status").GetString()!),
                PersonalEmail = item.TryGetProperty("PersonalEmail", out var em) ? em.GetString() : null,
                Notes = item.TryGetProperty("Notes", out var nt) ? nt.GetString() : null,
                CourseProgress = item.TryGetProperty("CourseProgress", out var cp) ? cp.GetInt32() : 0
            };

            if (item.TryGetProperty("Subjects", out var grades) &&
                grades.ValueKind == JsonValueKind.Object)
            {
                foreach (var g in grades.EnumerateObject())
                    student.Journal.SetGrade(g.Name, g.Value.GetDouble());
            }
            student.RecalculateFromJournal();
            _members.Add(student);
        }
    }
}
