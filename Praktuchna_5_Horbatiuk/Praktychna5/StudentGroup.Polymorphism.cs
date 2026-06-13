using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Praktychna5;

public partial class StudentGroup
{
    public void AddMember(UniversityMember member)
    {
        if (member == null) throw new ArgumentNullException(nameof(member));

        // Якщо це студент, перевіряємо унікальність заліковки.
        if (member is Student s &&
            _members.OfType<Student>().Any(x => x.RecordBookNumber == s.RecordBookNumber))
            throw new InvalidOperationException("Студент з таким номером заліковки вже існує.");

        _members.Add(member);
    }

    // Generic: повертає всіх членів заданого типу.
    public List<T> GetMembersByType<T>() where T : UniversityMember
        => _members.OfType<T>().ToList();

    // Поліморфізм: сумує стипендію всіх членів через спільний метод.
    public decimal GetTotalScholarship()
        => _members.Sum(m => m.CalculateScholarship());

    // Поліморфізм: GetInfo викликається для кожного члена за його реальним типом.
    public string GetMembersInfo()
    {
        if (_members.Count == 0) return "У групі немає членів.";

        var sb = new StringBuilder();
        sb.AppendLine($"Усього членів університету: {_members.Count}");
        int i = 1;
        foreach (var m in _members)
            sb.AppendLine($"{i++}. {m.GetInfo()}");
        return sb.ToString();
    }
}
