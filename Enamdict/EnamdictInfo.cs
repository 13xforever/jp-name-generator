namespace Enamdict;

public record EnamdictInfo(string Name, string Copyright, DateOnly Date)
{
    public static EnamdictInfo Parse(string line)
    {
        var lineParts = line.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (lineParts is not [_, string name, string copyright, string created])
            throw new FormatException($"Expected ENAMDICT header, but got '{line}'");

        if (created.Split(' ') is not ["Created:", string date])
            throw new FormatException($"Expected ENAMDICT version timestamp, but got '{created}'");

        return new(name, copyright, DateOnly.ParseExact(date, "yyyy-MM-dd"));
    }
}