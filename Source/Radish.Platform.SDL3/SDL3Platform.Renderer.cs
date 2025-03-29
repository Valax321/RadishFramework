using System.Diagnostics;
using System.Drawing;
using Radish.Graphics;

namespace Radish.Platform;
using static SDL3.SDL;

public partial class SDL3Platform : IPlatformRenderer
{
    public IPlatformRenderer GetRenderBackend() => this;
    
    public void Initialized()
    {
        var windows = SDL_GetWindows();
        foreach (var window in windows)
        {
            SDL_GetWindowSizeInPixels(window, out var w, out var h);
            GraphicsDeviceManager.Reset(window, new Size(w, h), SDL_GetDisplayForWindow(window));
        }
    }

    private IntPtr _currentCommandBuffer = IntPtr.Zero;
    private IntPtr _currentSwapchainTexture = IntPtr.Zero;
    private Size _currentSwapchainSize;
    private SDL_GPUTextureFormat _currentSwapchainFormat;
    
    public IntPtr InitDeviceWithWindowHandle(IntPtr window, out Size pixelSize, out uint displayIndex)
    {
        if (window == IntPtr.Zero)
            throw new InvalidOperationException("Window cannot be null");
        
        var shaderFormats = SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL |
                            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL |
                            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV;

        var debugContext = Environment.GetEnvironmentVariable("RADISH_GPU_DEBUG") == "1";

        var device = SDL_CreateGPUDevice(shaderFormats, debugContext, null!);
        if (device == IntPtr.Zero)
        {
            throw new PlatformException($"Failed to create GPU device: {SDL_GetError()}");
        }

        var driver = SDL_GetGPUDeviceDriver(device);
        Logger.Info("GPU device driver: {0}", driver);

        if (!SDL_ClaimWindowForGPUDevice(device, window))
            throw new PlatformException($"Failed to claim window for GPU device: {SDL_GetError()}");

        SDL_GetWindowSizeInPixels(window, out var w, out var h);
        pixelSize = new Size(w, h);
        displayIndex = SDL_GetDisplayForWindow(window);
        
        return device;
    }

    public void ReleaseDevice(IntPtr renderer, IntPtr window)
    {
        if (renderer == IntPtr.Zero)
            return;
        
        if (window != IntPtr.Zero)
            SDL_ReleaseWindowFromGPUDevice(renderer, window);
        
        SDL_DestroyGPUDevice(renderer);
    }

    public bool SetVsyncEnabled(IntPtr renderer, IntPtr window, bool enabled)
    {
        return SetNewSwapchainParams(renderer, window, enabled);
    }

    public void BeginFrame(IntPtr renderer, IntPtr window)
    {
        if (renderer == IntPtr.Zero)
            return;

        _currentCommandBuffer = SDL_AcquireGPUCommandBuffer(renderer);
        if (_currentCommandBuffer == IntPtr.Zero)
        {
            throw new PlatformException($"Failed to acquire command buffer: {SDL_GetError()}");
        }

        _currentSwapchainFormat = SDL_GetGPUSwapchainTextureFormat(renderer, window);
        SDL_WaitAndAcquireGPUSwapchainTexture(_currentCommandBuffer, window, out _currentSwapchainTexture, out var width,
            out var height);
        _currentSwapchainSize = new Size((int)width, (int)height);
        if (_currentSwapchainTexture == IntPtr.Zero)
        {
            throw new PlatformException($"Failed to acquire swapchain texture: {SDL_GetError()}");
        }
    }

    public void EndFrame(IntPtr renderer)
    {
        if (renderer == IntPtr.Zero)
            return;
        
        SDL_SubmitGPUCommandBuffer(_currentCommandBuffer);
        _currentCommandBuffer = IntPtr.Zero;
        _currentSwapchainTexture = IntPtr.Zero;
    }

    public IntPtr AcquireTexture(IntPtr renderer, in TextureCreationOptions options)
    {
        //TODO: needs to handle SRGB modes for textures
        if (options.Kind is not (TextureKind.Tex2D or TextureKind.TexCube))
            Debug.Assert(options.Dimensions.Length >= 3);
        else
            Debug.Assert(options.Dimensions.Length >= 2);
        
        var texInfo = new SDL_GPUTextureCreateInfo
        {
            type = options.Kind switch
            {
                TextureKind.Tex2D => SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                TextureKind.Tex2DArray => SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D_ARRAY,
                TextureKind.TexCube => SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_CUBE,
                TextureKind.TexCubeArray => SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_CUBE_ARRAY,
                TextureKind.Tex3D => SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_3D,
                _ => throw new ArgumentOutOfRangeException(nameof(options.Kind), options.Kind, null)
            },
            format = options.Format switch
            {
                TextureFormat.RGBA8 => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM,
                TextureFormat.DXT1 => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_BC1_RGBA_UNORM,
                TextureFormat.DXT3 => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_BC2_RGBA_UNORM,
                TextureFormat.DXT5 => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_BC3_RGBA_UNORM,
                TextureFormat.DXTnm => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_BC5_RG_UNORM,
                TextureFormat.BC6H => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_BC6H_RGB_FLOAT,
                TextureFormat.BC7 => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_BC7_RGBA_UNORM,
                TextureFormat.RGBAHalf => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R16G16B16A16_FLOAT,
                TextureFormat.RGBAFloat => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R32G32B32A32_FLOAT,
                TextureFormat.BC4 => SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_BC4_R_UNORM,
                _ => throw new ArgumentOutOfRangeException(nameof(options.Format), options.Format, null)
            },
            sample_count = options.MSAALevel switch
            {
                AntialiasingLevel.None => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
                AntialiasingLevel.MSAA2x => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_2,
                AntialiasingLevel.MSAA4x => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_4,
                AntialiasingLevel.MSAA8x => SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_8,
                _ => throw new ArgumentOutOfRangeException(nameof(options.MSAALevel), options.MSAALevel, null)
            },
            num_levels = (uint)options.MipmapLevels,
            width = options.Dimensions[0],
            height = options.Dimensions[1],
        };

        if (options.Usage.HasFlag(TextureUsage.TextureSampler))
            texInfo.usage |= SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_SAMPLER;
        if (options.Usage.HasFlag(TextureUsage.ColorTarget))
            texInfo.usage |= SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_COLOR_TARGET;
        if (options.Usage.HasFlag(TextureUsage.DepthTarget))
            texInfo.usage |= SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET;

        if (options.Kind is not (TextureKind.Tex2D or TextureKind.TexCube))
        {
            texInfo.layer_count_or_depth = options.Dimensions[2];
        }

        if (!SDL_GPUTextureSupportsFormat(renderer, texInfo.format, texInfo.type, texInfo.usage))
        {
            Logger.Error("Unsupported texture format: {0} {1} {2}", texInfo.format, texInfo.type, texInfo.usage);
            return IntPtr.Zero;
        }

        var handle = SDL_CreateGPUTexture(renderer, in texInfo);
        if (handle == IntPtr.Zero)
        {
            throw new PlatformException($"Failed to create texture: {SDL_GetError()}");
        }

        return handle;
    }

    public void ReleaseTexture(IntPtr renderer, IntPtr handle)
    {
        SDL_ReleaseGPUTexture(renderer, handle);
    }

    private bool SetNewSwapchainParams(IntPtr device, IntPtr window, bool wantsVsync)
    {
        var supportsSDRSwapchain = SDL_WindowSupportsGPUSwapchainComposition(device, window,
            SDL_GPUSwapchainComposition.SDL_GPU_SWAPCHAINCOMPOSITION_SDR);
        
        var supportsLinearSDRSwapchain = SDL_WindowSupportsGPUSwapchainComposition(device, window,
            SDL_GPUSwapchainComposition.SDL_GPU_SWAPCHAINCOMPOSITION_SDR_LINEAR);
        var compMode = SDL_GPUSwapchainComposition.SDL_GPU_SWAPCHAINCOMPOSITION_SDR;
        if (supportsSDRSwapchain && !supportsLinearSDRSwapchain)
        {
            Logger.Warn("Window does not support linear colorspace used by RadishFramework");
        }
        else if (!supportsSDRSwapchain && !supportsLinearSDRSwapchain)
        {
            throw new PlatformNotSupportedException("GPU device does not support SDR swapchain");
        }

        if (supportsLinearSDRSwapchain)
            compMode = SDL_GPUSwapchainComposition.SDL_GPU_SWAPCHAINCOMPOSITION_SDR_LINEAR;

        var presentMode = SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_IMMEDIATE;
        if (wantsVsync)
        {
            if (SDL_WindowSupportsGPUPresentMode(device, window, SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_MAILBOX))
                presentMode = SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_MAILBOX;
            else if (SDL_WindowSupportsGPUPresentMode(device, window, SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_VSYNC))
                presentMode = SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_VSYNC;
            else
                Logger.Warn("Window does not support VSync or Mailbox swapchain modes");
        }

        if (!SDL_SetGPUSwapchainParameters(device, window, compMode, presentMode))
        {
            Logger.Error("Failed to set swapchain parameters ({0}, {1}): {2}", compMode, presentMode, SDL_GetError());
            return false;
        }
        else
        {
            Logger.Info("Successfully set swapchain parameters: {0}, {1}", compMode, presentMode);
        }

        return presentMode is SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_VSYNC
            or SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_MAILBOX;
    }

    private void OnGpuDeviceReset(in SDL_Event ev)
    {
        
    }

    private void OnGpuDeviceLost()
    {
        
    }
}