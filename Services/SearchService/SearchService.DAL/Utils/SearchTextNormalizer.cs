using System.Text.RegularExpressions;

namespace SearchService.DAL.Utils;

public static class SearchTextNormalizer
{
    public static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        string value = text.Trim().ToLowerInvariant();
        value = value.Replace('ё', 'е');

        value = ReplaceToken(value, @"\bc#\b", "c# csharp dotnet asp.net aspnet");
        value = ReplaceToken(value, @"\basp\.net core\b", "asp.net core aspnet core asp.net aspnet dotnet csharp");
        value = ReplaceToken(value, @"\basp\.net\b", "asp.net aspnet dotnet csharp");
        value = ReplaceToken(value, @"(?<!asp)\.net\b", ".net dotnet csharp");
        value = ReplaceToken(value, @"\bстажер\b", "стажер стажировка junior internship trainee");
        value = ReplaceToken(value, @"\bстажировка\b", "стажировка стажер junior internship trainee");
        value = ReplaceToken(value, @"\bjunior\b", "junior стажер стажировка trainee");

        value = Regex.Replace(value, @"\s+", " ").Trim();
        return value;
    }

    private static string ReplaceToken(string source, string pattern, string replacement)
    {
        return Regex.Replace(
            source,
            pattern,
            replacement,
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
