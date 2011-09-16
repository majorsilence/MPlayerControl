/*

Copyright 2011 (C) Peter Gill <peter@majorsilence.com>

This file is part of LibImages

LibImages is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

LibImages is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibImages
{
    public class ImageResize
    {

        /// <summary>
        /// Resize image and keep proportions
        /// </summary>
        /// <param name="file">string - path to file</param>
        /// <param name="width">int</param>
        /// <param name="height">int</param>
        /// <returns>Image</returns>
        public static System.Drawing.Image Resize(string file, int width, int height)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(file);
            return Resize(image, width, height);
        }

        /// <summary>
        /// Resize image and keep proportions
        /// </summary>
        /// <param name="imgToResize">Image</param>
        /// <param name="width">int</param>
        /// <param name="height">int</param>
        /// <returns>Image</returns>
        public static System.Drawing.Image Resize(System.Drawing.Image imgToResize, int width, int height)
        {
            if (imgToResize == null)
            {
                return null;
            }

            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            System.Drawing.Bitmap b = new System.Drawing.Bitmap(destWidth, destHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)b);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (System.Drawing.Image)b;
        }
    }
}
