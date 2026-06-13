using System;

namespace Praktychna5;

// ВАРІАНТ 2: дробові числа з автоматичним скороченням (наприклад, 2/4 стає 1/2).
public class Fraction
{
    public int Numerator { get; private set; }
    public int Denominator { get; private set; }

    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
            throw new ArgumentException("Знаменник не може бути нулем.");

        // Знак тримаємо у чисельнику
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        int g = Gcd(Math.Abs(numerator), Math.Abs(denominator));
        Numerator = numerator / g;
        Denominator = denominator / g;
    }

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            int t = b;
            b = a % b;
            a = t;
        }
        return a == 0 ? 1 : a;
    }

    public double ToDouble() => (double)Numerator / Denominator;

    public static Fraction operator +(Fraction a, Fraction b)
        => new Fraction(a.Numerator * b.Denominator + b.Numerator * a.Denominator,
                        a.Denominator * b.Denominator);

    public static Fraction operator -(Fraction a, Fraction b)
        => new Fraction(a.Numerator * b.Denominator - b.Numerator * a.Denominator,
                        a.Denominator * b.Denominator);

    public static Fraction operator *(Fraction a, Fraction b)
        => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

    public static bool operator >(Fraction a, Fraction b) => a.ToDouble() > b.ToDouble();
    public static bool operator <(Fraction a, Fraction b) => a.ToDouble() < b.ToDouble();

    public static bool operator ==(Fraction? a, Fraction? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Numerator == b.Numerator && a.Denominator == b.Denominator;
    }

    public static bool operator !=(Fraction? a, Fraction? b) => !(a == b);

    public override bool Equals(object? obj) => obj is Fraction f && this == f;
    public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

    public override string ToString() => $"{Numerator}/{Denominator}";
}
