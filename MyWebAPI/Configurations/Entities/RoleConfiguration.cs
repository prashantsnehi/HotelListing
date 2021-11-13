using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWebAPI.Helpers;

namespace MyWebAPI.Configurations.Entities
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole { Name = Role.User.ToString(), NormalizedName = Role.User.ToString().ToUpper() },
                new IdentityRole { Name = Role.Admin.ToString(), NormalizedName = Role.Admin.ToString().ToUpper() },
                new IdentityRole { Name = Role.SuperAdmin.ToString(), NormalizedName = Role.SuperAdmin.ToString().ToUpper() }
            );
        }
    }
}
