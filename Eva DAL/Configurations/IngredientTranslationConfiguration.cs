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
    public class IngredientTranslationConfiguration : IEntityTypeConfiguration<IngredientTranslation>
    {
        public void Configure(EntityTypeBuilder<IngredientTranslation> builder)
        {
            builder.HasKey(it => it.Id);

            builder.Property(it => it.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(it => it.Story)
                   .HasMaxLength(2000);

            builder.Property(it => it.Benfits)
                   .HasMaxLength(2000);

            builder.HasOne(it => it.Ingredient)
                   .WithMany(i => i.Translations)
                   .HasForeignKey(it => it.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(it => it.Language)
                   .WithMany(l => l.IngredientTranslations)
                   .HasForeignKey(it => it.LanguageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
