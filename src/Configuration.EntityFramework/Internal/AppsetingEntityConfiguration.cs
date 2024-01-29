using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Honamic.Configuration.EntityFramework.Internal;

internal class AppsetingEntityConfiguration : IEntityTypeConfiguration<Setting>
{
    public virtual void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(model => model.Id);

        builder.Property(model => model.Application)
            .IsRequired(false)
           .HasMaxLength(100);

        builder.Property(model => model.Name)
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

        builder.ToTable("AppSettings");
    }
}