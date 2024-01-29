namespace Honamic.Configuration.EntityFramework.Storages;

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