using System.Text.RegularExpressions;

namespace PruebaTecnicaBack.Application.Common.Validators;

public static partial class RegexPatterns
{
    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]

    public static partial Regex EmailRegex();
}