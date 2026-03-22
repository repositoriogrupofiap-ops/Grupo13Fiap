using System.Text.RegularExpressions;

namespace GAVTech.Shared.Extensions;

public static partial class RegexExtensions
{
    [GeneratedRegex("[0-9]")]
    public static partial Regex ApenasNumeros();

    [GeneratedRegex("[A-Z]")]
    public static partial Regex TemLetraMaiuscula();


    [GeneratedRegex(@"^(\d{11})|([A-Z0-9]{12}\d{2})$")]
    public static partial Regex IsCpfCnpj();


    [GeneratedRegex("[^a-zA-Z0-9]")]
    public static partial Regex PossuiDigitoEspecial();
}