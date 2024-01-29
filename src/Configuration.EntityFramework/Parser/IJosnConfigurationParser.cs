namespace Honamic.Configuration.EntityFramework.Parser;

public interface IJosnConfigurationParser
{
    IDictionary<string, string> Parse(Stream stream);
}
