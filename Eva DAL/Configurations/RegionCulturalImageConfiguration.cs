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
    public class RegionCulturalImageConfiguration : IEntityTypeConfiguration<RegionCulturalImage>
    {
        public void Configure(EntityTypeBuilder<RegionCulturalImage> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ImageUrl)
                   .HasMaxLength(500);
        }
    }
}
