namespace Honamic.Configuration.EntityFramework;

public class SettingNameValue
{
    public SettingNameValue(string name, string? value)
    {
        Name = name;
        JsonValue = value;
    }

    public string Name { get; set; } = default!;

    public string? JsonValue { get; set; }
}