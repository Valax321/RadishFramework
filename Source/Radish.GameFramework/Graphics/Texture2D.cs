using System.Drawing;
using BCnEncoder.Shared.ImageFiles;
using JetBrains.Annotations;

namespace Radish.Graphics;

[PublicAPI]
public sealed class Texture2D : Texture
{
    public Size Size { get; }

    // public static Texture2D DecodeFromKTXStream(IGraphicsDevice device, Stream stream)
    // {
    //     var f = KtxFile.Load(stream);
    //     var tex = new Texture2D(device, new Size((int)f.header.PixelWidth, (int)f.header.PixelHeight),
    //         f.header. switch
    //         {
    //
    //         }, false, f.header.NumberOfMipmapLevels > 1);
    // }

    public static Texture2D DecodeFromDDSStream(IGraphicsDevice device, Stream stream)
    {
        var f = DdsFile.Load(stream);
        var tex = new Texture2D(device, new Size((int)f.header.dwWidth, (int)f.header.dwHeight),
            f.header.ddsPixelFormat.DxgiFormat switch
            {
                DxgiFormat.DxgiFormatR32G32B32A32Float => TextureFormat.RGBAFloat,
                DxgiFormat.DxgiFormatR16G16B16A16Float => TextureFormat.RGBAHalf,
                DxgiFormat.DxgiFormatR8G8B8G8Unorm => TextureFormat.RGBA8,
                DxgiFormat.DxgiFormatBc1Unorm => TextureFormat.DXT1,
                DxgiFormat.DxgiFormatBc2Unorm => TextureFormat.DXT3,
                DxgiFormat.DxgiFormatBc3Unorm => TextureFormat.DXT5,
                DxgiFormat.DxgiFormatBc4Unorm => TextureFormat.BC4,
                DxgiFormat.DxgiFormatBc5Unorm => TextureFormat.DXTnm,
                DxgiFormat.DxgiFormatB8G8R8A8UnormSrgb => TextureFormat.BGRA8,
                DxgiFormat.DxgiFormatBc6HSf16 => TextureFormat.BC6H,
                DxgiFormat.DxgiFormatBc7Unorm => TextureFormat.BC7,
                _ => throw new ArgumentOutOfRangeException(nameof(f.header.ddsPixelFormat.DxgiFormat), f.header.ddsPixelFormat.DxgiFormat, null)
            }, false, f.header.dwMipMapCount > 1);
        
        //TODO: upload texture data
        return tex;
    }
    
    public Texture2D(IGraphicsDevice device, Size size, TextureFormat format, bool isLinear, bool hasMips) 
        : base(device.ReleaseTexture)
    {
        Size = size;
        Format = format;
        IsLinear = isLinear;
        HasMipmaps = hasMips;
        ResourceHandle = device.AcquireTexture2D(format, size, isLinear, hasMips);
    }
}