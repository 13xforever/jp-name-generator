using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Enamdict;

namespace JpNameGenerator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private int generatorType = 0;
    [ObservableProperty] private string generatedName = "";
    [ObservableProperty] private string generatedNameKanji = "";

    private static readonly ConcurrentDictionary<NameCategory, List<Entry>> NameListCache = new();
    private static readonly Random Rng = new();
    private static readonly Entry EmptyEntry = new("", "", NameCategory.Unknown, "");

    partial void OnGeneratorTypeChanged(int value) => GenerateNameCommand.Execute(null);

    [RelayCommand]
    private async Task GenerateName()
    {
        var entry = GeneratorType switch
        {
            0 => await GeneratePersonAsync(),
            1 => await GenerateMaleAsync(),
            2 => await GenerateFemaleAsync(),
            _ => EmptyEntry,
        };

        GeneratedName = entry.Description;
        GeneratedNameKanji = entry.Kanji ?? entry.Kana;
    }

    private static async Task<List<Entry>> GetList(NameCategory key)
    {
        if (!NameListCache.TryGetValue(key, out var result))
        {
            var (_, list) = await Provider.ParseAsync().ConfigureAwait(false);
            NameListCache[key] = result = list.Where(
                e => (e.Categories & key) != NameCategory.Unknown
            ).ToList();
        }
        return result;
    }
    
    private static async Task<Entry> GeneratePersonAsync()
    {
        var nameList = await GetList(NameCategory.PersonFull);
        return nameList[Rng.Next(nameList.Count)];
    }

    private static async Task<Entry> GenerateMaleAsync()
    {
        var nameList = await GetList(NameCategory.GivenMale).ConfigureAwait(false);
        var result = nameList[Rng.Next(nameList.Count)];
        if (result.Categories.HasFlag(NameCategory.PersonFull))
            return result;

        var familyNameList = await GetList(NameCategory.Surname).ConfigureAwait(false);
        var surname = familyNameList[Rng.Next(familyNameList.Count)];
        return new(
            $"{surname.Kanji ?? surname.Kana} {result.Kanji ?? result.Kana}",
            $"{surname.Kana}{result.Kana}",
            surname.Categories | result.Categories,
            $"{surname.Description} {result.Description}"
        );
    }
    
    private static async Task<Entry> GenerateFemaleAsync()
    {
        var nameList = await GetList(NameCategory.GivenFemale).ConfigureAwait(false);
        var result = nameList[Rng.Next(nameList.Count)];
        if (result.Categories.HasFlag(NameCategory.PersonFull))
            return result;

        var familyNameList = await GetList(NameCategory.Surname).ConfigureAwait(false);
        var surname = familyNameList[Rng.Next(familyNameList.Count)];
        return new(
            $"{surname.Kanji ?? surname.Kana} {result.Kanji ?? result.Kana}",
            $"{surname.Kana}{result.Kana}",
            surname.Categories | result.Categories,
            $"{surname.Description} {result.Description}"
        );
    }
}