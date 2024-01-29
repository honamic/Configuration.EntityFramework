using Honamic.Configuration.EntityFramework.Parser;
using Honamic.Extensions.Configuration.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework;

internal sealed class EntityFrameworkConfigurationSource : IConfigurationSource
{
    public readonly Action<DbContextOptionsBuilder> OptionsAction;
    public IJosnConfigurationParser Parser { get; set; }


    public EntityFrameworkConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
    {
        OptionsAction = optionsAction;
        Parser = new JsonConfigurationParser();

        EntityFrameworkConfiguration.Current = new EntityFrameworkConfigurationProvider(this);
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return EntityFrameworkConfiguration.Current;
    }
}
