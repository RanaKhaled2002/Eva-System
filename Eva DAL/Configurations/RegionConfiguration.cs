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
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(r => r.MainImageUrl)
                   .HasMaxLength(500);

            builder.Property(r => r.HistoricalContent)
                   .HasMaxLength(2000);


            builder.Property(r => r.CulturalContent)
                   .HasMaxLength(2000);

            builder.Property(r => r.TranslationKey)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(r => r.TranslationKey)
                   .IsUnique();

            builder.HasMany(r => r.Ingredients)
                   .WithOne(i => i.Region)
                   .HasForeignKey(i => i.RegionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Translations)
                   .WithOne(t => t.Region)
                   .HasForeignKey(t => t.RegionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.RegionCulturalImages)
                   .WithOne(pi => pi.Region)
                   .HasForeignKey(pi => pi.RegionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.RegionHistoricalImages)
                   .WithOne(pi => pi.Region)
                   .HasForeignKey(pi => pi.RegionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
