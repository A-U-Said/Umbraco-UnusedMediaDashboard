using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using UnusedMediaDashboard.Models;
using UnusedMediaDashboard.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Linq;

namespace UnusedMediaDashboard.Controllers
{
    [PluginController("UMR")]
    public class UnusedMediaController : UmbracoAuthorizedJsonController
    {
        private readonly IMediaService _mediaService;
        private readonly MediaRemoveContext _mediaRemoveContext;
        private readonly IRelationService _relationService;
        private readonly IConfiguration _configuration;

        public UnusedMediaController(IMediaService mediaService, IRelationService relationService, IConfiguration configuration)
        {
            _mediaService = mediaService;
            _mediaRemoveContext = MediaRemoveContext.Current;
            _relationService = relationService;
            _configuration = configuration;
        }


        [HttpGet]
        public IActionResult StartUnusedMediaReport()
        {
            if (!_mediaRemoveContext.IsProcessingMedia)
            {
                var UnusedMediaService = new UnusedMediaService(_mediaService, _relationService, _configuration);
                Thread backgroundGetMedia = new Thread(UnusedMediaService.FindUnusedMedia);
                backgroundGetMedia.IsBackground = true;
                backgroundGetMedia.Name = "UnusedMedia GetUnusedMedia";
                backgroundGetMedia.Start();

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult GetUnusedMediaReportStatus()
        {
            var mediaReport = new MediaReport
            {
                IsProcessingMedia = _mediaRemoveContext.IsProcessingMedia,
                Data = _mediaRemoveContext.UnusedMedia.Select(x => x.Model),
                TotalAmountOfMedia = _mediaRemoveContext.TotalAmountOfMedia,
                TotalUnusedMedia = _mediaRemoveContext.UnusedMedia.Count
            };
            return Ok(mediaReport);
        }

        [HttpGet]
        public IActionResult GetUnusedMediaReportAsCsv()
        {
            if (!_mediaRemoveContext.IsProcessingMedia)
            {
                var mediaReport = new MediaReport
                {
                    IsProcessingMedia = _mediaRemoveContext.IsProcessingMedia,
                    Data = _mediaRemoveContext.UnusedMedia.Select(x => x.Model),
                    TotalAmountOfMedia = _mediaRemoveContext.TotalAmountOfMedia,
                    TotalUnusedMedia = _mediaRemoveContext.UnusedMedia.Count
                };

                return Ok(FileConversionService.Serialize<UnusedMedia>(mediaReport.Data));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult MoveItemToRecycling(int mediaId)
        {
            var mediaToDelete = _mediaService.GetById(mediaId);
            if (mediaToDelete == null)
            {
                return NotFound();
            }
            _mediaService.MoveToRecycleBin(mediaToDelete);
            return Ok();
        }

    }
}
