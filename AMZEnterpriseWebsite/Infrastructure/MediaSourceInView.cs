using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Models;

namespace AMZEnterpriseWebsite.Infrastructure
{
    public static class MediaSourceInView
    {
        public static string GetSource(Media media)
        {
            string path = "/";
            if (media.MediaFileType == MediaFileType.Img)
            {
                path += FileUploadUtil.PathImg + "/" + media.Name;
            }
            else if (media.MediaFileType == MediaFileType.Sound)
            {
                path += FileUploadUtil.PathSound + "/" + media.Name;
            }
            else if (media.MediaFileType == MediaFileType.Video)
            {
                path += FileUploadUtil.PathVideo + "/" + media.Name;
            }
            else
            {
                path += FileUploadUtil.PathOther + "/" + media.Name;

            }

            return path;
        }
    }
}
