using System;

namespace Praktychna5;

public class Vector
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vector(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    public static Vector operator +(Vector a, Vector b)
        => new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector operator -(Vector a, Vector b)
        => new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    // Скалярне множення (вектор на число)
    public static Vector operator *(Vector v, double scalar)
        => new Vector(v.X * scalar, v.Y * scalar, v.Z * scalar);

    // Збільшує/зменшує всі координати на 1
    public static Vector operator ++(Vector v)
        => new Vector(v.X + 1, v.Y + 1, v.Z + 1);

    public static Vector operator --(Vector v)
        => new Vector(v.X - 1, v.Y - 1, v.Z - 1);

    // Порівняння за довжиною вектора
    public static bool operator >(Vector a, Vector b) => a.Length > b.Length;
    public static bool operator <(Vector a, Vector b) => a.Length < b.Length;

    public static bool operator ==(Vector? a, Vector? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(Vector? a, Vector? b) => !(a == b);

    // Приведення до double повертає довжину вектора
    public static explicit operator double(Vector v) => v.Length;

    public override bool Equals(object? obj) => obj is Vector v && this == v;
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public override string ToString() => $"({X}, {Y}, {Z})";
}
