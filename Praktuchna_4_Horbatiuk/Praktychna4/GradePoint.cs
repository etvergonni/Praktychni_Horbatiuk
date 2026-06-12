using System;

namespace Praktychna4;

public partial class GradePoint
{
    private double _value;

    public double Value
    {
        get => _value;
        set
        {
            if (value < 0 || value > 10)
                throw new ArgumentOutOfRangeException(nameof(value), "Оцінка має бути від 0 до 10.");
            _value = value;
        }
    }

    public GradePoint(double value)
    {
        Value = value;
    }

    // Неявне приведення в обидва боки: GradePoint <-> double
    public static implicit operator double(GradePoint g) => g.Value;
    public static implicit operator GradePoint(double v) => new GradePoint(v);

    public static GradePoint operator +(GradePoint a, GradePoint b)
    {
        double sum = a.Value + b.Value;
        return new GradePoint(sum > 10 ? 10 : sum);
    }

    public static GradePoint operator ++(GradePoint g)
    {
        double v = g.Value + 1;
        return new GradePoint(v > 10 ? 10 : v);
    }

    public static GradePoint operator --(GradePoint g)
    {
        double v = g.Value - 1;
        return new GradePoint(v < 0 ? 0 : v);
    }

    public override string ToString() => Value.ToString("F1");
}
