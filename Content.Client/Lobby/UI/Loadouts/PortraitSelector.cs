using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.Portraits;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby.UI.Loadouts;

/// <summary>
/// Simple grid-based portrait selector for the LoadoutWindow Portrait tab.
/// </summary>
public sealed class PortraitSelector : BoxContainer
{
    private readonly IPrototypeManager _protoMan;
    private readonly IResourceCache _resCache;
    private readonly GridContainer _grid;
    private readonly TextureRect _previewRect;
    private readonly Label _previewName;
    private readonly Label _previewDesc;
    private readonly Dictionary<string, Button> _buttons = new();

    public event Action<string>? OnPortraitSelected;

    public PortraitSelector()
    {
        _protoMan = IoCManager.Resolve<IPrototypeManager>();
        _resCache = IoCManager.Resolve<IResourceCache>();

        Orientation = LayoutOrientation.Horizontal;
        HorizontalExpand = true;
        VerticalExpand = true;

        var scroll = new ScrollContainer { HorizontalExpand = true, VerticalExpand = true };
        _grid = new GridContainer { Columns = 4, HorizontalExpand = true };
        scroll.AddChild(_grid);
        AddChild(scroll);

        var preview = new BoxContainer
        {
            Orientation = LayoutOrientation.Vertical,
            MinSize = new Vector2(150, 0),
        };
        preview.AddChild(new Label { Text = "Preview", StyleClasses = { "labelHeading" }, HorizontalAlignment = HAlignment.Center });
        preview.AddChild(new Control { MinSize = new Vector2(0, 5) });

        var texPanel = new PanelContainer { HorizontalAlignment = HAlignment.Center };
        texPanel.PanelOverride = new StyleBoxFlat { BackgroundColor = Color.FromHex("#1B1B1E") };
        _previewRect = new TextureRect { Stretch = TextureRect.StretchMode.KeepCentered, MinSize = new Vector2(128, 128) };
        texPanel.AddChild(_previewRect);
        preview.AddChild(texPanel);

        preview.AddChild(new Control { MinSize = new Vector2(0, 5) });
        _previewName = new Label { HorizontalAlignment = HAlignment.Center };
        preview.AddChild(_previewName);
        _previewDesc = new Label { HorizontalAlignment = HAlignment.Center, FontColorOverride = Color.FromHex("#888888") };
        preview.AddChild(_previewDesc);

        AddChild(preview);
    }

    public void Setup(List<CharacterPortraitPrototype> portraits, string? selectedId)
    {
        _grid.RemoveAllChildren();
        _buttons.Clear();

        // Fallback: auto-select fallback portrait if none selected
        if ((string.IsNullOrEmpty(selectedId) || !portraits.Any(p => p.ID == selectedId))
            && portraits.Count > 0)
        {
            selectedId = portraits.FirstOrDefault(p => p.IsFallback)?.ID ?? portraits[0].ID;
        }

        foreach (var portrait in portraits)
        {
            var btn = CreateButton(portrait);
            btn.Pressed = portrait.ID == selectedId;
            if (portrait.ID == selectedId)
                UpdatePreview(portrait);

            _buttons[portrait.ID] = btn;
            _grid.AddChild(btn);
        }

        if (string.IsNullOrEmpty(selectedId))
        {
            _previewName.Text = "[ No portrait selected ]";
            _previewDesc.Text = "Choose a portrait from the list";
            _previewRect.Texture = null;
        }
    }

    private Button CreateButton(CharacterPortraitPrototype portrait)
    {
        var btn = new Button { MinSize = new Vector2(64, 64), ToggleMode = true };

        var texRect = new TextureRect { Stretch = TextureRect.StretchMode.KeepCentered, HorizontalExpand = true, VerticalExpand = true };
        btn.AddChild(texRect);

        if (!string.IsNullOrEmpty(portrait.TexturePath))
        {
            if (_resCache.TryGetResource<TextureResource>(portrait.TexturePath, out var tex))
                texRect.Texture = tex;
        }

        btn.OnToggled += _ =>
        {
            if (btn.Pressed)
            {
                foreach (var kvp in _buttons)
                    kvp.Value.Pressed = false;
                btn.Pressed = true;
                UpdatePreview(portrait);
                OnPortraitSelected?.Invoke(portrait.ID);
            }
        };

        return btn;
    }

    private void UpdatePreview(CharacterPortraitPrototype portrait)
    {
        _previewName.Text = portrait.Name;
        _previewDesc.Text = portrait.Description ?? string.Empty;

        if (!string.IsNullOrEmpty(portrait.TexturePath) &&
            _resCache.TryGetResource<TextureResource>(portrait.TexturePath, out var tex))
        {
            _previewRect.Texture = tex;
        }
        else
        {
            _previewRect.Texture = null;
        }
    }
}
