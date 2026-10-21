using System.Security.Claims;
using EatonvilleDanceStudio.Controllers;
using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.Models;
using EatonvilleDanceStudio.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EatonvilleDanceStudio.Tests;

public class ParentControllerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _context;

    public ParentControllerTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private static (ParentController Controller, FakeUserManager UserManager) CreateController(
        ApplicationDbContext context,
        string? userId = "test-user-id")
    {
        var fakeUserManager = new FakeUserManager(userId);
        var controller = new ParentController(context, fakeUserManager);

        var claims = new List<Claim>();
        if (userId != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        return (controller, fakeUserManager);
    }

    private async Task<ApplicationUser> CreateUserAsync(string userId)
    {
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = $"user_{userId}@test.com",
            Email = $"user_{userId}@test.com"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task Dashboard_WhenUserIdIsNull_ReturnsUnauthorized()
    {
        // Arrange
        var (controller, _) = CreateController(_context, userId: null);

        // Act
        var result = await controller.Dashboard();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Dashboard_WhenParentProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var (controller, _) = CreateController(_context, userId: "unknown-user");

        // Act
        var result = await controller.Dashboard();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Parent profile not found", notFoundResult.Value);
    }

    [Fact]
    public async Task Dashboard_WhenParentProfileExists_PopulatesDashboardViewModel()
    {
        // Arrange
        var userId = "parent-user-1";
        await CreateUserAsync(userId);

        var parentProfile = new ParentProfile
        {
            UserId = userId,
            User = null,
            FirstName = "Jane",
            LastName = "Doe",
            PrimaryPhone = "555-1234",
            EmergencyContactName = "John Doe",
            EmergencyContactPhone = "555-5678",
            AccountStatus = "Approved"
        };
        _context.ParentProfiles.Add(parentProfile);
        await _context.SaveChangesAsync();

        var dancer1 = new Dancer
        {
            FirstName = "Alice",
            LastName = "Doe",
            DateOfBirth = new DateTime(2015, 5, 20),
            SkillLevel = "Beginner",
            ParentProfileId = parentProfile.Id,
            PhotoApprovalStatus = "Approved"
        };
        var dancer2 = new Dancer
        {
            FirstName = "Bob",
            LastName = "Doe",
            DateOfBirth = new DateTime(2012, 8, 15),
            SkillLevel = "Intermediate",
            ParentProfileId = parentProfile.Id,
            PhotoApprovalStatus = "Approved"
        };
        _context.Dancers.AddRange(dancer1, dancer2);

        var danceClass = new DanceClass
        {
            ClassName = "Ballet 101",
            MinimumAge = 6,
            MaximumAge = 10,
            RequiredSkillLevel = "Beginner",
            DayOfWeek = "Monday",
            StartTime = new TimeSpan(16, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            Capacity = 15
        };
        _context.DanceClasses.Add(danceClass);
        await _context.SaveChangesAsync();

        var enrollment = new Enrollment
        {
            DancerId = dancer1.Id,
            DanceClassId = danceClass.Id,
            Status = "Enrolled",
            EnrollmentDate = DateTime.UtcNow
        };
        _context.Enrollments.Add(enrollment);

        var charge = new LedgerEntry
        {
            ParentId = userId,
            Amount = 60.00m,
            Description = "Monthly Tuition",
            TransactionDate = DateTime.UtcNow.AddDays(-5),
            Category = "Tuition"
        };
        var payment = new LedgerEntry
        {
            ParentId = userId,
            Amount = -60.00m,
            Description = "Credit Card Payment",
            TransactionDate = DateTime.UtcNow.AddDays(-2),
            Category = "Payment"
        };
        _context.LedgerEntries.AddRange(charge, payment);
        await _context.SaveChangesAsync();

        var (controller, _) = CreateController(_context, userId: userId);

        // Act
        var result = await controller.Dashboard();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsType<ParentDashboardViewModel>(viewResult.Model);

        Assert.NotNull(viewModel.Profile);
        Assert.Equal("Jane", viewModel.Profile.FirstName);
        Assert.Equal(2, viewModel.Dancers.Count);
        Assert.Single(viewModel.EnrolledClasses);
        Assert.Equal("Ballet 101", viewModel.EnrolledClasses[0].DanceClass?.ClassName);
        Assert.Equal("Alice", viewModel.EnrolledClasses[0].Dancer?.FirstName);
        Assert.Equal(0.00m, viewModel.CurrentBalance);
        Assert.Single(viewModel.RecentCharges);
        Assert.Equal(60.00m, viewModel.RecentCharges[0].Amount);
        Assert.Single(viewModel.RecentPayments);
        Assert.Equal(-60.00m, viewModel.RecentPayments[0].Amount);
    }

    [Fact]
    public void AddDancer_GET_ReturnsViewWithEmptyModel()
    {
        // Arrange
        var (controller, _) = CreateController(_context, userId: "test-user");

        // Act
        var result = controller.AddDancer();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AddDancerViewModel>(viewResult.Model);
        Assert.Equal(string.Empty, model.FirstName);
        Assert.Equal(string.Empty, model.LastName);
    }

    [Fact]
    public async Task AddDancer_POST_WhenParentProfileNotFound_ReturnsNotFound()
    {
        // Arrange
        var (controller, _) = CreateController(_context, userId: "non-existent-user");
        var input = new AddDancerViewModel
        {
            FirstName = "Charlie",
            LastName = "Doe",
            DateOfBirth = new DateTime(2016, 1, 1),
            SkillLevel = "Beginner"
        };

        // Act
        var result = await controller.AddDancer(input);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddDancer_POST_WhenModelStateInvalid_ReturnsViewWithModel()
    {
        // Arrange
        var userId = "parent-user-2";
        await CreateUserAsync(userId);

        _context.ParentProfiles.Add(new ParentProfile
        {
            UserId = userId,
            User = null,
            FirstName = "Jane",
            LastName = "Doe",
            PrimaryPhone = "555-1111",
            EmergencyContactName = "John Doe",
            EmergencyContactPhone = "555-2222",
            AccountStatus = "Approved"
        });
        await _context.SaveChangesAsync();

        var (controller, _) = CreateController(_context, userId: userId);
        controller.ModelState.AddModelError("FirstName", "First name is required");

        var input = new AddDancerViewModel
        {
            FirstName = "",
            LastName = "Doe",
            DateOfBirth = new DateTime(2016, 1, 1),
            SkillLevel = "Beginner"
        };

        // Act
        var result = await controller.AddDancer(input);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(input, viewResult.Model);
        Assert.Empty(_context.Dancers);
    }

    [Fact]
    public async Task AddDancer_POST_WhenValid_CreatesDancerAndRedirectsToDashboard()
    {
        // Arrange
        var userId = "parent-user-3";
        await CreateUserAsync(userId);

        var parentProfile = new ParentProfile
        {
            UserId = userId,
            User = null,
            FirstName = "Jane",
            LastName = "Doe",
            PrimaryPhone = "555-1111",
            EmergencyContactName = "John Doe",
            EmergencyContactPhone = "555-2222",
            AccountStatus = "Approved"
        };
        _context.ParentProfiles.Add(parentProfile);
        await _context.SaveChangesAsync();

        var (controller, _) = CreateController(_context, userId: userId);

        var input = new AddDancerViewModel
        {
            FirstName = "Charlie",
            LastName = "Doe",
            DateOfBirth = new DateTime(2016, 3, 10),
            SkillLevel = "Beginner"
        };

        // Act
        var result = await controller.AddDancer(input);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ParentController.Dashboard), redirectResult.ActionName);

        var savedDancer = await _context.Dancers.FirstOrDefaultAsync(d => d.FirstName == "Charlie");
        Assert.NotNull(savedDancer);
        Assert.Equal("Doe", savedDancer.LastName);
        Assert.Equal(parentProfile.Id, savedDancer.ParentProfileId);
        Assert.Equal("None", savedDancer.PhotoApprovalStatus);
        Assert.Null(savedDancer.ProfilePhotoPath);
    }
}

public class FakeUserManager : UserManager<ApplicationUser>
{
    private readonly string? _userId;

    public FakeUserManager(string? userId = null)
        : base(new FakeUserStore(), null!, null!, null!, null!, null!, null!, null!, null!)
    {
        _userId = userId;
    }

    public override string? GetUserId(ClaimsPrincipal principal) => _userId;
}

public class FakeUserStore : IUserStore<ApplicationUser>
{
    public void Dispose() { }
    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);
    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser?>(null);
    public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser?>(null);
}
