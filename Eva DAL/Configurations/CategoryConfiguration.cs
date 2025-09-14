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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.TranslationKey)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(c => c.TranslationKey)
                   .IsUnique();

            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.categoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Translations)
                   .WithOne(t => t.Category)
                   .HasForeignKey(t => t.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
