using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using JpNameGenerator.Converters;
using JpNameGenerator.Utils.WinRTInterop;

namespace JpNameGenerator.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    public ViewModelBase() => OnAccentColorChanged(accentColor);

    [ObservableProperty] private static string tintColor = "#ffffff";
    [ObservableProperty] private static double tintOpacity = 1.0;
    [ObservableProperty] private static double materialOpacity = 0.69;
    [ObservableProperty] private static double luminosityOpacity = 1.0;
    [ObservableProperty] private static string accentColor = "#000000";
    [ObservableProperty] private static string systemAccentColor = "#bc002d";

    [ObservableProperty] private static bool hasSystemAccentColor = Application.Current?.PlatformSettings is not null;
    [ObservableProperty] private static bool micaEnabled = true;
    [ObservableProperty] private static bool acrylicEnabled = false;

    [ObservableProperty] private static FontFamily symbolFontFamily = new("avares://ps3-disc-dumper/Assets/Fonts#Font Awesome 6 Free");
    [ObservableProperty] private static FontFamily largeFontFamily = FontManager.Current.DefaultFontFamily;
    [ObservableProperty] private static FontFamily smallFontFamily = FontManager.Current.DefaultFontFamily;

    partial void OnAccentColorChanged(string value)
    {
        if (Application.Current is not {ApplicationLifetime: IClassicDesktopStyleApplicationLifetime
            {
                MainWindow.ActualThemeVariant: {} t
            }} app)
            return;

        AccentColorInfo accentInfo;
        if (OperatingSystem.IsWindowsVersionAtLeast(10))
        {
            accentInfo = CustomPlatformSettings.GetColorValues();
        }
        else
        {
            var accent = ColorConverter.Parse(value);
            var light1 = ChangeColorLuminosity(accent, 0.3);
            var light2 = ChangeColorLuminosity(accent, 0.5);
            var light3 = ChangeColorLuminosity(accent, 0.7);
            var dark1 = ChangeColorLuminosity(accent, -0.3);
            var dark2 = ChangeColorLuminosity(accent, -0.5);
            var dark3 = ChangeColorLuminosity(accent, -0.7);
            accentInfo = new(accent, light1, light2, light3, dark1, dark2, dark3);
        }
        app.Resources["SystemAccentColor"] = accentInfo.Accent;
        app.Resources["SystemAccentColorDark1"] = accentInfo.Dark1;
        app.Resources["SystemAccentColorDark2"] = accentInfo.Dark2;
        app.Resources["SystemAccentColorDark3"] = accentInfo.Dark3;
        app.Resources["SystemAccentColorLight1"] = accentInfo.Light1;
        app.Resources["SystemAccentColorLight2"] = accentInfo.Light2;
        app.Resources["SystemAccentColorLight3"] = accentInfo.Light3;
    }

    private static Color ChangeColorLuminosity(Color color, double luminosityFactor)
    {
        var red = (double)color.R;
        var green = (double)color.G;
        var blue = (double)color.B;

        if (luminosityFactor < 0)
        {
            luminosityFactor = 1 + luminosityFactor;
            red *= luminosityFactor;
            green *= luminosityFactor;
            blue *= luminosityFactor;
        }
        else if (luminosityFactor >= 0)
        {
            red = (255 - red) * luminosityFactor + red;
            green = (255 - green) * luminosityFactor + green;
            blue = (255 - blue) * luminosityFactor + blue;
        }

        return new(color.A, (byte)red, (byte)green, (byte)blue);
    }}