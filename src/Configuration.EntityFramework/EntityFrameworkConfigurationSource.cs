using Honamic.Extensions.Configuration.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework;

public class EntityFrameworkConfigurationSource : IConfigurationSource
{
    private readonly Action<DbContextOptionsBuilder> _optionsAction;

    public EntityFrameworkConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
    {
        _optionsAction = optionsAction;
        EntityFrameworkConfiguration.Current = new EntityFrameworkConfigurationProvider(_optionsAction);
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return EntityFrameworkConfiguration.Current;
    }
}
