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
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(i => i.Story)
                   .HasMaxLength(2000);

            builder.Property(i => i.Benefits)
                   .HasMaxLength(2000);

            builder.Property(i => i.TranslationKey)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(i => i.TranslationKey)
                   .IsUnique();

            builder.HasOne(i => i.Region)
                   .WithMany(r => r.Ingredients)
                   .HasForeignKey(i => i.RegionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.ProductIngredients)
                   .WithOne(pi => pi.Ingredient)
                   .HasForeignKey(pi => pi.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.Translations)
                   .WithOne(t => t.Ingredient)
                   .HasForeignKey(t => t.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.IngredientImages)
                   .WithOne(pi => pi.Ingredient)
                   .HasForeignKey(pi => pi.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
