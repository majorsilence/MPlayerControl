namespace Majorsilence.Media.Videos;

/// <summary>
///     The video output backend that mplayer is using.
/// </summary>
public enum MplayerBackends
{
    /// <summary>
    ///     This may be the recommened backend on Mac OSX.
    /// </summary>
    OpenGL = 1,

    /// <summary>
    ///     Simple Version
    /// </summary>
    GL = 2,

    /// <summary>
    ///     Simple Version.  Variant of the OpenGL  video  output  driver.
    ///     Supports  videos larger  than  the maximum texture size but lacks
    ///     many of the ad‐vanced features and optimizations of the gl driver
    ///     and  is  un‐likely to be extended further.
    /// </summary>
    GL2 = 3,

    /// <summary>
    ///     Windows. This is the recommened backend on windows.
    /// </summary>
    Direct3D = 4,

    /// <summary>
    ///     Windows
    /// </summary>
    DirectX = 5,

    /// <summary>
    ///     Linux
    /// </summary>
    X11 = 6,
    VESA = 7,

    /// <summary>
    ///     Mac OS X
    /// </summary>
    Quartz = 8,

    /// <summary>
    ///     Mac OS X
    /// </summary>
    CoreVideo = 9,

    /// <summary>
    ///     Cross Platform
    /// </summary>
    SDL = 10,

    /// <summary>
    ///     Linux
    /// </summary>
    Vdpau = 11,

    /// <summary>
    ///     ASCII art video output driver that works on a text console.
    /// </summary>
    ASCII = 12,

    /// <summary>
    ///     Color  ASCII  art  video output driver that works on a text console.
    /// </summary>
    ColorASCII = 13,

    /// <summary>
    ///     Linux.  Play video using the DirectFB library.
    /// </summary>
    Directfb = 14,

    /// <summary>
    ///     Linux.  Nintendo Wii/GameCube specific video output driver.
    /// </summary>
    Wii = 15,

    /// <summary>
    /// Linux.   requires Linux 2.6.22+ kernel,  Video output driver for 
    // V4L2 compliant cards with built-in hardware MPEG decoder.
    /// </summary>
    V4l2 = 16,

    /// <summary>
    ///     Linux
    /// </summary>
    XV = 17
}