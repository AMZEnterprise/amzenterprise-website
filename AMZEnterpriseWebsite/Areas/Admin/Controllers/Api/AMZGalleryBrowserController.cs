using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMZEnterpriseWebsite.Data;
using AMZEnterpriseWebsite.Infrastructure;
using AMZEnterpriseWebsite.Models;
using AMZEnterpriseWebsite.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMZEnterpriseWebsite.Areas.Admin.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AMZGalleryBrowserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AMZGalleryBrowserController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Return Gallery Modal Data
        public async Task<JsonResult> GetGallery(string fileType, int? pageNumber, string searchString, string currentFilter, int pageSize = 8)
        {
            MediaFileType mediaFileType;
            if (fileType == "img")
            {
                mediaFileType = MediaFileType.Img;
            }
            else if (fileType == "sound")
            {
                mediaFileType = MediaFileType.Sound;
            }
            else if (fileType == "video")
            {
                mediaFileType = MediaFileType.Video;
            }
            else
            {
                mediaFileType = MediaFileType.Other;
            }


            var medias = _context.Medias.Where(m => m.MediaFileType == mediaFileType);


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            if (!(String.IsNullOrEmpty(searchString)))
            {
                medias = medias.Where(m => m.Name.Contains(searchString));
            }

            medias = medias.OrderByDescending(m => m.DateTime);

            var mediasResult = await PaginatedList<Media>.CreateAsync(medias.AsNoTracking(), pageNumber ?? 1, pageSize);

            bool hasNextPage = mediasResult.HasNextPage;
            bool hasPrevPage = mediasResult.HasPreviousPage;

            int nextPage = mediasResult.PageIndex + 1;
            int prevPage = mediasResult.PageIndex - 1;

            return new JsonResult(new
            {
                data = mediasResult,
                mediaType = fileType,
                hasNext = hasNextPage,
                hasPrev = hasPrevPage,
                nextPageIndex = nextPage,
                prevPageIndex = prevPage,
                currentSearchString = searchString
            });
        }



        public JsonResult GetGalleryItemDetails(int? id)
        {
            if (id == null)
            {
                return new JsonResult(null);
            }

            var media = _context.Medias.FirstOrDefault(m => m.Id == id);

            if (media != null)
            {
                string mediaLink = "/" + FileUploadUtil.GetFilePath(media.MediaFileType) + "/" + media.Name;

                string mediaFileType = string.Empty;

                if (media.MediaFileType == MediaFileType.Img)
                {
                    mediaFileType = "img";
                }
                else if (media.MediaFileType == MediaFileType.Sound)
                {
                    mediaFileType = "sound";
                }
                else if (media.MediaFileType == MediaFileType.Video)
                {
                    mediaFileType = "video";
                }
                else
                {
                    mediaFileType = "other";
                }

                return new JsonResult(new
                {
                    name = media.Name,
                    link = mediaLink,
                    filetype = mediaFileType
                });
            }


            return new JsonResult(null);
        }


        public JsonResult GetAProjectMedias(int? id)
        {
            if (id == null)
            {
                return new JsonResult(null);
            }

            var projectMedias = _context.ProjectAndMedias
                .Include(pm => pm.Media)
                .Where(pm => pm.ProjectId == id);

            var medias = new List<ProjectMediasDTO>();
            foreach (var media in projectMedias)
            {
                string mediaLink = "/" + FileUploadUtil.GetFilePath(media.Media.MediaFileType) + "/" + media.Media.Name;


                string mediaFileType = string.Empty;

                if (media.Media.MediaFileType == MediaFileType.Img)
                {
                    mediaFileType = "img";
                }
                else if (media.Media.MediaFileType == MediaFileType.Sound)
                {
                    mediaFileType = "sound";
                }
                else if (media.Media.MediaFileType == MediaFileType.Video)
                {
                    mediaFileType = "video";
                }
                else
                {
                    mediaFileType = "other";
                }

                medias.Add(new ProjectMediasDTO
                {
                    id = media.MediaId,
                    name = media.Media.Name,
                    link = mediaLink,
                    filetype = mediaFileType,
                });
            }

            return new JsonResult(medias);
        }

    }
}