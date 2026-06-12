namespace Praktychna4;

public partial class GradePoint
{
    public static bool operator >(GradePoint a, GradePoint b) => a.Value > b.Value;
    public static bool operator <(GradePoint a, GradePoint b) => a.Value < b.Value;
    public static bool operator >=(GradePoint a, GradePoint b) => a.Value >= b.Value;
    public static bool operator <=(GradePoint a, GradePoint b) => a.Value <= b.Value;

    // true якщо оцінка достатня (>= 8), false якщо ні
    public static bool operator true(GradePoint g) => g.Value >= 8;
    public static bool operator false(GradePoint g) => g.Value < 8;
}
