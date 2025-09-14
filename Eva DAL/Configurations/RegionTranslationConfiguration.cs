using Eva_DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Configurations
{
    public class RegionTranslationConfiguration : IEntityTypeConfiguration<RegionTranslation>
    {
        public void Configure(EntityTypeBuilder<RegionTranslation> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(rt => rt.HistoricalContent)
                   .HasMaxLength(2000);

            builder.Property(rt => rt.CulturalContent)
                   .HasMaxLength(2000);

            builder.HasOne(rt => rt.Region)
                   .WithMany(r => r.Translations)
                   .HasForeignKey(rt => rt.RegionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rt => rt.Language)
                   .WithMany(l => l.RegionTranslations)
                   .HasForeignKey(rt => rt.LanguageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
