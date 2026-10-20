using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EatonvilleDanceStudio.Models;


namespace EatonvilleDanceStudio.Data;

/// <summary>
/// Represents the database context for the Eatonville Dance Studio application, extending the
/// IdentityDbContext to include identity management and database sets specific to the application's needs.
/// </summary>
/// <remarks>
/// The ApplicationDbContext serves as the bridge between the application and the database. It defines
/// the entity sets for the core models in the system, allowing for interaction with the database.
/// </remarks>

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        // Maps the ParentProfile class to the ParentProfiles SQL table
        public DbSet<ParentProfile> ParentProfiles { get; set; }

        // Maps the Dancer class to the Dancers SQL table
        public DbSet<Dancer> Dancers { get; set; }
        public DbSet<DanceClass> DanceClasses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

    }

