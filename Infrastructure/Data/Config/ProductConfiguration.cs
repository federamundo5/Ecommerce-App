using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.Property(p => p.Id).IsRequired();
            builder.Property(P => P.Name).IsRequired();
            builder.Property(P => P.Description).IsRequired();
            builder.Property(P => P.Price).HasColumnType("decimal(18,2)");
            builder.Property(P => P.PictureUrl).IsRequired();
            builder.HasOne(b => b.ProductBrand).WithMany()
                                .HasForeignKey(p => p.ProductBrandId);
            builder.HasOne(b => b.ProductType).WithMany()
                                .HasForeignKey(p => p.ProductTypeId);
        }
    }
}