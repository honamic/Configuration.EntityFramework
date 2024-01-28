namespace Honamic.Configuration.EntityFramework;

public class Appseting
{
    public int Id { get; set; }

    public string? Application { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string? Value { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTimeOffset? CreatedOn { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }
}