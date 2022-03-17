using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skinet.Identity.Domain.Entities;
using Skinet.Identity.Domain.ValueObjects;

namespace Skinet.Identity.Infrastructure.EntityConfigurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> addressBuilder)
        {
            addressBuilder.HasKey(address => address.Id);

            addressBuilder.Property(address => address.Id);

            addressBuilder.OwnsOne(address => address.UserName, userNameBuilder =>
            {
                userNameBuilder.Property(userName => userName.FirstName)
                    .HasColumnName(nameof(UserName.FirstName))
                    .IsRequired();
                
                userNameBuilder.Property(userName => userName.LastName)
                    .HasColumnName(nameof(UserName.LastName))
                    .IsRequired();
            });

            addressBuilder.OwnsOne(address => address.DeliveryDetails, deliveryDetailsBuilder =>
            {
                deliveryDetailsBuilder.Property(deliveryDetails => deliveryDetails.Street)
                    .HasColumnName(nameof(DeliveryDetails.Street))
                    .IsRequired();
                
                deliveryDetailsBuilder.Property(deliveryDetails => deliveryDetails.City)
                    .HasColumnName(nameof(DeliveryDetails.City))
                    .IsRequired();
                
                deliveryDetailsBuilder.Property(deliveryDetails => deliveryDetails.State)
                    .HasColumnName(nameof(DeliveryDetails.State))
                    .IsRequired();
                
                deliveryDetailsBuilder.Property(deliveryDetails => deliveryDetails.ZipCode)
                    .HasColumnName(nameof(DeliveryDetails.ZipCode))
                    .IsRequired();
            });

            addressBuilder.HasOne(address => address.AppUser)
                .WithMany()
                .IsRequired();
        }
    }
}