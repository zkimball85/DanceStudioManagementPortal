using EatonvilleDanceStudio.Models;

namespace EatonvilleDanceStudio.ViewModels;

/// <summary>
/// Represents the view model for browsing dance classes in the Eatonville Dance Studio application.
/// </summary>
/// <remarks>
/// This view model is responsible for holding the data necessary for displaying
/// available dance classes and the dancer profiles associated with the user.
/// It acts as a data container between the controller and the view.
/// </remarks>

public class ClassBrowseViewModel
{
    public required List<DanceClass> AvailableClasses { get; set; }
    public required List<Dancer> MyDancers { get; set; }
}