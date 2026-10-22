using System.Security.Claims;
using System.Threading.Tasks;
using EatonvilleDanceStudio.Controllers;
using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace EatonvilleDanceStudio.Tests;

public class AccountControllerTests
{
    private readonly FakeAccountUserManager _userManager;
    private readonly FakeSignInManager _signInManager;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _userManager = new FakeAccountUserManager();
        _signInManager = new FakeSignInManager(_userManager);
        _controller = new AccountController(_userManager, _signInManager);
    }

    [Fact]
    public void Register_GET_ReturnsViewResult()
    {
        // Act
        var result = _controller.Register();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Register_POST_WhenModelStateInvalid_ReturnsViewWithModelWithoutCreatingUser()
    {
        // Arrange
        _controller.ModelState.AddModelError("Email", "Email is required.");
        var model = new RegisterViewModel
        {
            Email = "",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            HasAgreedToTerms = true
        };

        // Act
        var result = await _controller.Register(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.Null(_userManager.CreatedUser);
        Assert.False(_signInManager.SignInCalled);
    }

    [Fact]
    public async Task Register_POST_WhenCreateSucceeds_SignsInUserAndRedirectsToHomeIndex()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Email = "director@dance.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            HasAgreedToTerms = true
        };

        // Act
        var result = await _controller.Register(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        Assert.NotNull(_userManager.CreatedUser);
        Assert.Equal("director@dance.com", _userManager.CreatedUser.UserName);
        Assert.Equal("director@dance.com", _userManager.CreatedUser.Email);
        Assert.Equal("Password123!", _userManager.CreatedPassword);

        Assert.True(_signInManager.SignInCalled);
        Assert.Same(_userManager.CreatedUser, _signInManager.SignedInUser);
        Assert.False(_signInManager.IsPersistentUsed);
    }

    [Fact]
    public async Task Register_POST_WhenCreateFails_AddsErrorsToModelStateAndReturnsViewWithModel()
    {
        // Arrange
        var error = new IdentityError
        {
            Code = "DuplicateUserName",
            Description = "User name 'director@dance.com' is already taken."
        };
        _userManager.CreateResult = IdentityResult.Failed(error);

        var model = new RegisterViewModel
        {
            Email = "director@dance.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            HasAgreedToTerms = true
        };

        // Act
        var result = await _controller.Register(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.False(_signInManager.SignInCalled);

        Assert.True(_controller.ModelState.ContainsKey(string.Empty));
        var modelError = Assert.Single(_controller.ModelState[string.Empty]!.Errors);
        Assert.Equal("User name 'director@dance.com' is already taken.", modelError.ErrorMessage);
    }

    private class FakeAccountUserManager : UserManager<ApplicationUser>
    {
        public IdentityResult CreateResult { get; set; } = IdentityResult.Success;
        public ApplicationUser? CreatedUser { get; private set; }
        public string? CreatedPassword { get; private set; }

        public FakeAccountUserManager()
            : base(new FakeUserStore(), null!, null!, null!, null!, null!, null!, null!, null!)
        {
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            CreatedUser = user;
            CreatedPassword = password;
            return Task.FromResult(CreateResult);
        }
    }

    private class FakeSignInManager : SignInManager<ApplicationUser>
    {
        public bool SignInCalled { get; private set; }
        public ApplicationUser? SignedInUser { get; private set; }
        public bool IsPersistentUsed { get; private set; }

        public FakeSignInManager(UserManager<ApplicationUser> userManager)
            : base(
                userManager,
                new HttpContextAccessor { HttpContext = new DefaultHttpContext() },
                new MockClaimsPrincipalFactory(),
                new OptionsWrapper<IdentityOptions>(new IdentityOptions()),
                new FakeLogger<SignInManager<ApplicationUser>>(),
                null!,
                null!)
        {
        }

        public override Task SignInAsync(ApplicationUser user, bool isPersistent, string? authenticationMethod = null)
        {
            SignInCalled = true;
            SignedInUser = user;
            IsPersistentUsed = isPersistent;
            return Task.CompletedTask;
        }
    }

    private class MockClaimsPrincipalFactory : IUserClaimsPrincipalFactory<ApplicationUser>
    {
        public Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            return Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    private class FakeLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }
}
