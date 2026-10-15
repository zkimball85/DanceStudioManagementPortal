using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EatonvilleDanceStudio.Models;


namespace EatonvilleDanceStudio.Data;

    // Inheriting from IdentityDbContext gives us all the built-in auth tables automatically
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        // Maps the ParentProfile class to the ParentProfiles SQL table
        public DbSet<ParentProfile> ParentProfiles { get; set; }

        // Maps the Dancer class to the Dancers SQL table
        public DbSet<Dancer> Dancers { get; set; }

    }

