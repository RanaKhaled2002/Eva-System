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
    public class CategoryTranslationConfiguration : IEntityTypeConfiguration<CategoryTranslation>
    {
        public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
        {
            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(ct => ct.Description)
                   .HasMaxLength(1000);

            builder.HasOne(ct => ct.Category)
                   .WithMany(c => c.Translations)
                   .HasForeignKey(ct => ct.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ct => ct.Language)
                   .WithMany(l => l.CategoryTranslations)
                   .HasForeignKey(ct => ct.LanguageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
