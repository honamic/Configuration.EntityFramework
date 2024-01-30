namespace Honamic.Configuration.EntityFramework.Storages;

public class SettingNameValue
{
    public SettingNameValue(string name, string? value)
    {
        SectionName = name;
        JsonValue = value;
    }

    public string SectionName { get; set; } = default!;

    public string? JsonValue { get; set; }
}