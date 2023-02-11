namespace Majorsilence.Media.Videos;

/// <summary>
///     The seek type that is used when seeking a new position in the video stream.
/// </summary>
public enum Seek
{
    Relative = 0,
    Percentage = 1,
    Absolute = 2
}