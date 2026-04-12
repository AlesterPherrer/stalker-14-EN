using Robust.Shared.GameStates;

namespace Content.Shared._Stalker.Portraits;

/// <summary>
/// Stores the selected character portrait for a mob entity.
/// Set during character spawning from the player's profile.
/// Used for PDA notification sender icons.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CharacterPortraitComponent : Component
{
    /// <summary>
    /// The texture path of the selected portrait.
    /// Empty string means no portrait selected.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public string PortraitTexturePath = string.Empty;
}
