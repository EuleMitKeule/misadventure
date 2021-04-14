
/// <summary>
/// Provides access to singleton objects and services.
/// </summary>
public static class Locator
{
    /// <summary>
    /// The player's player component.
    /// </summary>
    public static PlayerComponent PlayerComponent { get; set; }

    /// <summary>
    /// The main camera's camera component.
    /// </summary>
    public static CameraComponent CameraComponent { get; set; }
}
