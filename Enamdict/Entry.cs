namespace Enamdict;

public record Entry(string? Kanji, string Kana, NameCategory Categories, string Description)
{
    public static List<Entry> Parse(string line)
    {
        var parts = line.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            throw new FormatException($"Expected at least two parts in entry, but got '{line}");

        var result = new List<Entry>(parts.Length-1);
        var (kanji, kana) = ParseKanjiReading(parts[0]);
        var prevCat = NameCategory.Unknown;
        foreach (var l in parts[1..])
        {
            var (categories, description) = ParseCategories(l);
            if (categories is NameCategory.Unknown)
                categories = prevCat;
            else
                prevCat = categories;
            result.Add(new(kanji, kana, categories, description));
        }
        return result;
    }

    private static (string? kanji, string kana) ParseKanjiReading(string kanjiPart)
        => kanjiPart.Split(' ', StringSplitOptions.TrimEntries) switch
        {
            [{ Length: > 0 } kana] => (null, kana),
            [{ Length: > 0 } kanji, ['[', .., ']'] kana] => (kanji, kana.Trim('[', ']')),
            _ => throw new FormatException($"Expected kanji with reading part, but got '{kanjiPart}'"),
        };

    private static (NameCategory categories, string description) ParseCategories(string line)
    {
        if (!line.StartsWith('(')
            || line.StartsWith("(Gulf of")
            || line.StartsWith("(formerly"))
            return (NameCategory.Unknown, line);
        
        if (line.Split(' ', 2, StringSplitOptions.TrimEntries) is not [['(', .., ')'] categoryPart, { Length: >0 } description])
            throw new FormatException($"Expected first description part with general information, but got '{line}'");

        if (categoryPart.Trim('(', ')').Split(',', StringSplitOptions.TrimEntries) is not { Length: > 0 } categoryList)
            throw new FormatException($"Expected category list, but got '{categoryPart}'");

        var categories = NameCategory.Unknown;
        foreach (var cat in categoryList)
        {
            categories |= cat switch
            {
                "s" => NameCategory.Surname,
                "p" => NameCategory.Place,
                "u" => NameCategory.PersonUnclassified,
                "g" => NameCategory.GivenUnclassified,
                "f" => NameCategory.GivenFemale,
                "m" => NameCategory.GivenMale,
                "h" => NameCategory.PersonFull,
                "pr" => NameCategory.Product,
                "c" => NameCategory.Company,
                "o" => NameCategory.Organization,
                "st" => NameCategory.Station,
                "wk" => NameCategory.Work,

                "group" => NameCategory.Group,
                "obj" => NameCategory.Object,
                "serv" => NameCategory.Service,
                "ship" => NameCategory.Ship,
                
                "ch" or "leg" or "fic" or "myth" or
                "cr" or "dei" or
                "ev" or "document" => NameCategory.Unknown,
                _ => throw new FormatException($"Unknown name category '{cat}'")
            };
        }
        return (categories, description);
    }
}