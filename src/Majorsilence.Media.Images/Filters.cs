// According to http://www.codeproject.com/Messages/2255862/Re-Wonderful-set-of-Classes-and-functionality-Re-u.aspx it is free for any use.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
namespace Majorsilence.Media.Images
{
    /// <summary>
    /// 
    /// </summary>
    public class ConvMatrix
    {
        /// <summary>
        /// 
        /// </summary>
        public int TopLeft = 0, TopMid = 0, TopRight = 0;

        /// <summary>
        /// 
        /// </summary>
        public int MidLeft = 0, Pixel = 1, MidRight = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;

        /// <summary>
        /// 
        /// </summary>
        public int Factor = 1;

        /// <summary>
        /// 
        /// </summary>
        public int Offset = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nVal"></param>
        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct FloatPoint
    {
        public double X;
        public double Y;
    }

    /// <summary>
    /// 
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// 
        /// </summary>
        public const short EDGE_DETECT_KIRSH = 1;

        /// <summary>
        /// 
        /// </summary>
        public const short EDGE_DETECT_PREWITT = 2;

        /// <summary>
        /// 
        /// </summary>
        public const short EDGE_DETECT_SOBEL = 3;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool GrayScale(Image b)
        {
            b.Mutate(x => x
                  .Grayscale());
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nBrightness"></param>
        /// <returns></returns>
        public static bool Brightness(Image b, int nBrightness)
        {
            if (nBrightness < -255 || nBrightness > 255)
                return false;


            b.Mutate(x => x
               .Brightness(nBrightness));
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nContrast"></param>
        /// <returns></returns>
        public static bool Contrast(Image b, sbyte nContrast)
        {
            if (nContrast < -100) return false;
            if (nContrast > 100) return false;

            float contrast = (100.0f + nContrast) / 100.0f;
            contrast *= contrast;

            b.Mutate(x => x
               .Contrast(contrast));
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nWeight"></param>
        /// <returns></returns>
        public static bool GaussianBlur(Image b, int nWeight /* default to 4*/)
        {
            b.Mutate(x => x
             .GaussianBlur(nWeight));

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nWeight"></param>
        /// <returns></returns>
        public static bool Sharpen(Image b, int nWeight /* default to 11*/ )
        {
            b.Mutate(x => x
           .GaussianSharpen(nWeight));

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nDegree"></param>
        /// <returns></returns>
        public static bool RandomJitter(Image b, short nDegree)
        {
            Point[,] ptRandJitter = new Point[b.Width, b.Height];

            int nWidth = b.Width;
            int nHeight = b.Height;

            int newX, newY;

            short nHalf = (short)Math.Floor(nDegree / 2.0);
            Random rnd = new Random();

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    newX = rnd.Next(nDegree) - nHalf;

                    if (x + newX > 0 && x + newX < nWidth)
                        ptRandJitter[x, y].X = newX;
                    else
                        ptRandJitter[x, y].X = 0;

                    newY = rnd.Next(nDegree) - nHalf;

                    if (y + newY > 0 && y + newY < nWidth)
                        ptRandJitter[x, y].Y = newY;
                    else
                        ptRandJitter[x, y].Y = 0;
                }

           // OffsetFilter(b, ptRandJitter);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="fDegree"></param>
        /// <param name="bSmoothing"></param>
        /// <returns></returns>
        public static bool Swirl(Image b, double fDegree, bool bSmoothing /* default fDegree to .05 */)
        {
            int nWidth = b.Width;
            int nHeight = b.Height;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
            Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = nWidth / 2;
            mid.Y = nHeight / 2;

            double theta, radius;
            double newX = 0, newY = 0;

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - mid.X;
                    int trueY = y - mid.Y;
                    theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    newX = mid.X + (radius * Math.Cos(theta + fDegree * radius));
                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        pt[x, y].X = (int)newX;
                    }
                    else
                        fp[x, y].X = pt[x, y].X = x;

                    newY = mid.Y + (radius * Math.Sin(theta + fDegree * radius));
                    if (newY > 0 && newY < nHeight)
                    {
                        fp[x, y].Y = newY;
                        pt[x, y].Y = (int)newY;
                    }
                    else
                        fp[x, y].Y = pt[x, y].Y = y;
                }

            b.Mutate(x => x
                .Dither(
                    new Rectangle(
                        new Point((int)newX, (int)newY),
                        new Size(nWidth, nHeight)
                    )
            ));

            return true;
        }



        public static bool Sphere(Image b, bool bSmoothing)
        {
            return Sphere(b, bSmoothing, b.Height, b.Width);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="bSmoothing"></param>
        /// <returns></returns>
        public static bool Sphere(Image b, bool bSmoothing, int height, int width)
        {
            int nWidth = height; //b.Width;
            int nHeight = width; // b.Height;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
            Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = nWidth / 2;
            mid.Y = nHeight / 2;

            double theta, radius;
            double newX = 0, newY = 0;

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - mid.X;
                    int trueY = y - mid.Y;
                    theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double newRadius = radius * radius / (Math.Max(mid.X, mid.Y));

                    newX = mid.X + (newRadius * Math.Cos(theta));

                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        pt[x, y].X = pt[x, y].Y = 0;
                    }

                    newY = mid.Y + (newRadius * Math.Sin(theta));

                    if (newY > 0 && newY < nHeight && newX > 0 && newX < nWidth)
                    {
                        fp[x, y].Y = newY;
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        pt[x, y].X = pt[x, y].Y = 0;
                    }
                }

            b.Mutate(x => x
                   .Dither(
                       new Rectangle(
                           new Point((int)newX, (int)newY),
                           new Size(nWidth, nHeight)
                       )
               ));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="factor"></param>
        /// <param name="bSmoothing"></param>
        /// <returns></returns>
        public static bool TimeWarp(Image b, Byte factor, bool bSmoothing)
        {
            int nWidth = b.Width;
            int nHeight = b.Height;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
            Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = nWidth / 2;
            mid.Y = nHeight / 2;

            double theta, radius;
            double newX = 0, newY = 0;

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - mid.X;
                    int trueY = y - mid.Y;
                    theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double newRadius = Math.Sqrt(radius) * factor;

                    newX = mid.X + (newRadius * Math.Cos(theta));
                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        fp[x, y].X = 0.0;
                        pt[x, y].X = 0;
                    }

                    newY = mid.Y + (newRadius * Math.Sin(theta));
                    if (newY > 0 && newY < nHeight)
                    {
                        fp[x, y].Y = newY;
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        fp[x, y].Y = 0.0;
                        pt[x, y].Y = 0;
                    }
                }

            b.Mutate(x => x
              .Dither(
                  new Rectangle(
                      new Point((int)newX, (int)newY),
                      new Size(nWidth, nHeight)
                  )
          ));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="fDegree"></param>
        /// <returns></returns>
        public static bool Moire(Image b, double fDegree)
        {
            int nWidth = b.Width;
            int nHeight = b.Height;




            Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = nWidth / 2;
            mid.Y = nHeight / 2;



            double theta, radius;
            int newX = 0, newY = 0;

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - mid.X;
                    int trueY = y - mid.Y;
                    theta = Math.Atan2((trueX), (trueY));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    newX = (int)(radius * Math.Sin(theta + fDegree * radius));
                    if (newX > 0 && newX < nWidth)
                    {
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        pt[x, y].X = 0;
                    }

                    newY = (int)(radius * Math.Sin(theta + fDegree * radius));
                    if (newY > 0 && newY < nHeight)
                    {
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        pt[x, y].Y = 0;
                    }
                }

            b.Mutate(x => x
                .Dither(
                    new Rectangle(
                        new Point(newX, newY),
                        new Size(nWidth, nHeight)
                    )
            ));


            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nWave"></param>
        /// <param name="bSmoothing"></param>
        /// <returns></returns>
        public static bool Water(Image b, short nWave, bool bSmoothing)
        {
            int nWidth = b.Width;
            int nHeight = b.Height;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
            Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = nWidth / 2;
            mid.Y = nHeight / 2;

            double newX = 0, newY = 0;
            double xo, yo;

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    xo = ((double)nWave * Math.Sin(2.0 * 3.1415 * (float)y / 128.0));
                    yo = ((double)nWave * Math.Cos(2.0 * 3.1415 * (float)x / 128.0));

                    newX = (x + xo);
                    newY = (y + yo);

                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        fp[x, y].X = 0.0;
                        pt[x, y].X = 0;
                    }


                    if (newY > 0 && newY < nHeight)
                    {
                        fp[x, y].Y = newY;
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        fp[x, y].Y = 0.0;
                        pt[x, y].Y = 0;
                    }
                }

            b.Mutate(x => x
                .Dither(
                    new Rectangle(
                        new Point((int)newX, (int)newY),
                        new Size(nWidth, nHeight)
                    )
            ));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="pixel"></param>
        /// <param name="bGrid"></param>
        /// <returns></returns>
        public static bool Pixelate(Image b, short pixel, bool bGrid)
        {
            int nWidth = b.Width;
            int nHeight = b.Height;

            Point[,] pt = new Point[nWidth, nHeight];

            int newX = 0, newY = 0;

            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    newX = pixel - x % pixel;

                    if (bGrid && newX == pixel)
                        pt[x, y].X = -x;
                    else if (x + newX > 0 && x + newX < nWidth)
                        pt[x, y].X = newX;
                    else
                        pt[x, y].X = 0;

                    newY = pixel - y % pixel;

                    if (bGrid && newY == pixel)
                        pt[x, y].Y = -y;
                    else if (y + newY > 0 && y + newY < nHeight)
                        pt[x, y].Y = newY;
                    else
                        pt[x, y].Y = 0;
                }

            b.Mutate(x => x
               .Dither(
                   new Rectangle(
                       new Point(newX, newY),
                       new Size(nWidth, nHeight)
                   )
           ));

            return true;
        }
    }
}