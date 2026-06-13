using System;

namespace Praktychna5;

public class WorkingStudent : Student
{
    public string Company { get; set; }
    public int WorkHoursPerWeek { get; set; }

    public WorkingStudent(string fullName, DateTime dateOfBirth, string recordBookNumber, string company, int workHours)
        : base(fullName, dateOfBirth, recordBookNumber)
    {
        Company = company;
        WorkHoursPerWeek = workHours;
    }

    public override decimal CalculateScholarship()
    {
        // Працюючим студентам нараховується половина стипендії.
        return base.CalculateScholarship() / 2;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + $" [працює: {Company}, {WorkHoursPerWeek} год/тиждень]";
    }
}
