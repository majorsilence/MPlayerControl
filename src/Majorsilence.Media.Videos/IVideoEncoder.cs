/*

Copyright 2012 (C) Peter Gill <peter@majorsilence.com>

This file is part of Majorsilence.Media.Videos.

Majorsilence.Media.Videos is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

Majorsilence.Media.Videos is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System.Threading.Tasks;

namespace Majorsilence.Media.Videos
{
    public interface IVideoEncoder
    {
        void Convert(string cmd, string workingDirectory = "");
        void Convert(Mencoder.VideoType vidType, Mencoder.AudioType audType, string videoToConvertFilePath, string outputFilePath);
        void Convert2DvdMpeg(Mencoder.RegionType regType, string videoToConvertFilePath, string outputFilePath);
        Task Convert2DvdMpegAsync(Mencoder.RegionType regType, string videoToConvertFilePath, string outputFilePath);
        void Convert2WebM(string videoToConvertFilePath, string outputFilePath);
        Task Convert2WebMAsync(string videoToConvertFilePath, string outputFilePath);
        void Convert2X264(string videoToConvertFilePath, string outputFilePath);
        Task Convert2X264Async(string videoToConvertFilePath, string outputFilePath);
        Task ConvertAsync(string cmd);
        Task ConvertAsync(Mencoder.VideoType vidType, Mencoder.AudioType audType, string videoToConvertFilePath, string outputFilePath);
    }
}