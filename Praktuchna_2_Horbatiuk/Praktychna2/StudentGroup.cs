using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Praktychna2;

// Реалізовано в гілці feature/persistence
// Методи SaveToFile та LoadFromFile з JSON

public class StudentGroup
{
    public string GroupName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int Course { get; set; }

    private List<Student> _students = new();

    public int GroupSize => _students.Count;

    public double AverageGroupGrade =>
        _students.Count == 0 ? 0 : Math.Round(_students.Average(s => s.AverageGrade), 2);

  
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

    public List<Student> FindStudent(string namePart, bool byName)
    {
        if (!byName) return new List<Student>();
        return _students
            .Where(s => s.FullName.Contains(namePart, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Student> GetExcellentStudents() => _students.Where(s => s.IsExcellent()).ToList();
    public List<Student> GetFailingStudents() => _students.Where(s => s.IsFailing()).ToList();
    public List<Student> GetStudentsByStatus(StudentStatus status) =>
        _students.Where(s => s.Status == status).ToList();
    public List<Student> GetAllStudents() => _students.ToList();

    public double ExcellentPercent =>
        GroupSize == 0 ? 0 : Math.Round(GetExcellentStudents().Count * 100.0 / GroupSize, 1);


    public void AssignStudentToPort(Student s, int row, int col)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (row < 0 || row >= PortMatrix.Rows || col < 0 || col >= PortMatrix.Cols)
            throw new PortMatrixIndexException(row, col);


        if (_students.Any(other => other != s && other.PortRow == row && other.PortCol == col))
            throw new InvalidOperationException(
                $"Порт ({row}, {col}) вже зайнято іншим студентом.");

        s.PortRow = row;
        s.PortCol = col;
    }


    public void UnassignStudentFromPort(Student s)
    {
        s.PortRow = -1;
        s.PortCol = -1;
    }


    public List<Student> GetStudentsByPortStatus(bool isOpen, PortMatrix matrix)
    {
        return _students
            .Where(s => s.IsAssignedToPort)
            .Where(s =>
            {
                try { return matrix.GetPort(s.PortRow, s.PortCol).IsOpen == isOpen; }
                catch { return false; }
            })
            .ToList();
    }

 
    public void SimulateLab(Student s, int labNumber, byte grade, PortMatrix matrix, PortLogger logger)
    {
        if (!s.IsAssignedToPort)
            throw new InvalidOperationException("Студент не прив'язаний до порту.");


        s.AddLabGrade(labNumber, grade);


        var port = matrix.GetPort(s.PortRow, s.PortCol);
        bool wasClosed = !port.IsOpen;
        if (wasClosed) port.Open();


        var dataToWrite = new byte[]
        {
            (byte)labNumber,
            grade,
            (byte)(DateTime.Now.Day),
            (byte)(DateTime.Now.Month),
            (byte)(DateTime.Now.Year % 100)
        };
        port.Write(dataToWrite);


        logger.LogOperation("LAB_DONE", port.PortNumber,
            $"студент={s.FullName}, лаб#{labNumber}, оцінка={grade}");
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
                PortRow = item.TryGetProperty("PortRow", out var pr) ? pr.GetInt32() : -1,
                PortCol = item.TryGetProperty("PortCol", out var pc) ? pc.GetInt32() : -1
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
