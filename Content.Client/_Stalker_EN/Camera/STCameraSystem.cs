using System.IO;
using Content.Client.DoAfter;
using Content.Client.Viewport;
using Content.Shared._Stalker_EN.Camera;
using Robust.Client.State;
using Robust.Client.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Content.Client._Stalker_EN.Camera;

/// <summary>
/// Client-side camera system: captures viewport, resizes, JPEG-encodes, sends to server.
/// </summary>
public sealed class STCameraSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;

    private const int PhotoWidth = 480;
    private const int PhotoHeight = 360;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<STCaptureViewportRequestEvent>(OnCaptureViewportRequestEvent);
    }

    private void OnCaptureViewportRequestEvent(STCaptureViewportRequestEvent ev)
    {
        if (_stateManager.CurrentState is not IMainViewportState state)
            return;

        var token = ev.Token;

        // Temporarily hide the DoAfter overlay so the progress bar doesn't appear in the photo.
        _overlayManager.TryGetOverlay<DoAfterOverlay>(out var doAfterOverlay);
        if (doAfterOverlay != null)
            _overlayManager.RemoveOverlay(doAfterOverlay);

        state.Viewport.Viewport.Screenshot(screenshot =>
        {
            if (doAfterOverlay != null)
                _overlayManager.AddOverlay(doAfterOverlay);

            ProcessScreenshot(screenshot, token);
        });
    }

    private void ProcessScreenshot(Image<Rgba32> screenshot, Guid token)
    {
        // Clone immediately — the original image is shared with other screenshot callbacks.
        // Do NOT mutate or dispose the original.
        using var resized = screenshot.Clone(ctx => ctx.Resize(PhotoWidth, PhotoHeight));

        using var stream = new MemoryStream();
        resized.SaveAsJpeg(stream);
        var imageData = stream.ToArray();

        RaiseNetworkEvent(new STCaptureViewportResponseEvent
        {
            Token = token,
            ImageData = imageData,
        });
    }
}
