using Robust.Shared.Prototypes;

namespace Content.Shared.Portraits;

/// <summary>
/// Defines a character portrait that can be selected during character customization.
/// Portraits are filtered by faction (band) and optionally by specific job.
/// Used as sender icons in PDA notifications.
/// </summary>
[Prototype("characterPortrait")]
public sealed partial class CharacterPortraitPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Display name shown in the portrait selector UI.
    /// </summary>
    [DataField(required: true)]
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description for tooltip in the portrait selector.
    /// </summary>
    [DataField]
    public string? Description { get; private set; }

    /// <summary>
    /// Path to the portrait texture (PNG format).
    /// Example: /Textures/_Stalker_EN/Portraits/freedom_1.png
    /// </summary>
    [DataField(required: true)]
    public string TexturePath { get; private set; } = string.Empty;

    /// <summary>
    /// The role (job) this portrait is tied to.
    /// Players with this job will see the portrait in their customization menu.
    /// </summary>
    [DataField(required: true)]
    public string JobId { get; private set; } = string.Empty;

    /// <summary>
    /// If true, this portrait is used as a fallback when no other portrait is available
    /// for the character's role.
    /// </summary>
    [DataField]
    public bool IsFallback { get; private set; }

    /// <summary>
    /// Whether this portrait should be available by default or locked behind conditions.
    /// </summary>
    [DataField]
    public bool UnlockedByDefault { get; private set; } = true;
}
