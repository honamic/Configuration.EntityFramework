using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.Configuration.EntityFramework.Storages;

internal class SettingEntityTypeConfiguration : IEntityTypeConfiguration<Setting>
{
    private string TableName;
    private string? Schema = null;

    public SettingEntityTypeConfiguration(string tableName, string? schema)
    {
        TableName = tableName;
        Schema = schema;
    }

    public virtual void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(model => model.Id);

        builder.HasIndex(m => new { m.Application, m.SectionName })
            .IsUnique()
            .HasFilter(null);

        builder.Property(model => model.Application)
            .IsRequired(false)
           .HasMaxLength(100);

        builder.Property(model => model.SectionName)
                .IsRequired()
                .HasMaxLength(450);

        builder.Property(model => model.Value)
            .IsRequired(false);

        builder.Property(model => model.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(model => model.ModifiedBy)
               .IsRequired(false)
               .HasMaxLength(50);

        builder.ToTable(TableName, Schema);
    }
}