/*

Copyright 2011 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibImages.

LibImages is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 2 of the License, or
(at your option) any later version.

LibImages is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with LibImages.  If not, see <http://www.gnu.org/licenses/>.

*/

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Majorsilence.Media.Images;

public class ImageResize
{
    /// <summary>
    ///     Resize image and keep proportions
    /// </summary>
    /// <param name="file">string - path to file</param>
    /// <param name="width">int</param>
    /// <param name="height">int</param>
    /// <returns>Image</returns>
    public static Image Resize(string file, int width, int height)
    {
        var image = Image.Load(file);

        image.Mutate(x => x
            .Resize(width, height));
        return image;
    }

    /// <summary>
    ///     Resize image and keep proportions
    /// </summary>
    /// <param name="imgToResize">Image</param>
    /// <param name="width">int</param>
    /// <param name="height">int</param>
    /// <returns>Image</returns>
    public static Image Resize(Image imgToResize, int width, int height)
    {
        imgToResize.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Stretch
            }));
        return imgToResize;
    }


    public static Image ResizeBlackBar(string file, int width, int height)
    {
        var image = Image.Load(file);
        return ResizeBlackBar(image, width, height);
    }

    /// <summary>
    ///     Resize image and keep proportions
    /// </summary>
    /// <param name="imgToResize">Image</param>
    /// <param name="width">int</param>
    /// <param name="height">int</param>
    /// <returns>Image</returns>
    public static Image ResizeBlackBar(Image imgToResize, int width, int height)
    {
        if (imgToResize == null) return null;

        imgToResize.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Pad
            }));
        return imgToResize;
    }
}