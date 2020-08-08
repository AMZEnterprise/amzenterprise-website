using System;
using System.IO;
using AMZEnterpriseWebsite.Models;

namespace AMZEnterpriseWebsite.Infrastructure
{
    public static class FileUploadUtil
    {
        public const string PathImg = @"media/img";
        public const string PathVideo = @"media/video";
        public const string PathSound = @"media/sound";
        public const string PathOther = @"media/other";
        public const string PathUserProfiles = @"media/profiles";


        private static string[] ImgTypes = new[] { ".jpg", ".png", ".jpeg" };
        private static string[] VideoTypes = new[] { ".mp4", ".avi", ".mvk", ".mpeg" };
        private static string[] SoundTypes = new[] { ".mp3", ".wav", ".ogg" };


        private static bool IsImgType(string fileExtension)
        {
            foreach (var item in ImgTypes)
            {
                if (fileExtension == item)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsVideoType(string fileExtension)
        {
            foreach (var item in VideoTypes)
            {
                if (fileExtension == item)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSoundType(string fileExtension)
        {
            foreach (var item in SoundTypes)
            {
                if (fileExtension == item)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFilePath(string fileName)
        {
            var fileType = Path.GetExtension(fileName);

            if (IsImgType(fileType))
            {
                return PathImg;
            }
            else if (IsSoundType(fileType))
            {
                return PathSound;
            }
            else if (IsVideoType(fileType))
            {
                return PathVideo;
            }
            else
            {
                return PathOther;
            }
        }

        public static string GetFilePath(MediaFileType fileType)
        {
            switch (fileType)
            {
                case MediaFileType.Img: return PathImg;
                case MediaFileType.Sound: return PathSound;
                case MediaFileType.Video: return PathVideo;
                default: return PathOther;
            }
        }


        public static MediaFileType GetFileType(string fileName)
        {
            var fileType = Path.GetExtension(fileName);

            if (IsImgType(fileType))
            {
                return MediaFileType.Img;
            }
            else if (IsSoundType(fileType))
            {
                return MediaFileType.Sound;
            }
            else if (IsVideoType(fileType))
            {
                return MediaFileType.Video;
            }
            else
            {
                return MediaFileType.Other;
            }
        }




        public static string ByteToMegabyteConvert(double bytesSize)
        {
            double size = bytesSize /Math.Pow(10, 6);
            return Math.Round(size, 2).ToString();
        }
    }


}
