using System.Collections.Concurrent;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using UnusedMediaDashboard.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using File = System.IO.File;
using Newtonsoft.Json;
using System.IO;
using static Umbraco.Cms.Core.Constants.HttpContext;

namespace UnusedMediaDashboard.Services
{
    public class UnusedMediaService
    {
        private readonly IMediaService _mediaService;
        private readonly IRelationService _relationService;
        private readonly MediaRemoveContext _mediaRemoveContext;
        //private readonly List<ExemptFolder> _exemptFolders;
        private readonly string _exemptFoldersFilePath;
        private ExemptFoldersFile _exemptFoldersFile;

        public UnusedMediaService(IMediaService mediaService, IRelationService relationService, IConfiguration configuration)
        {
            _mediaService = mediaService;
            _relationService = relationService;
            _mediaRemoveContext = MediaRemoveContext.Current;
            //_exemptFolders = configuration.GetSection("UnusedMediaReporting:ExemptFolders").Get<List<ExemptFolder>>();
            _exemptFoldersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "exemptFolders.json");
            using (StreamReader r = new StreamReader(_exemptFoldersFilePath))
            {
                try
                {
                    var existingFile = JsonConvert.DeserializeObject<ExemptFoldersFile>(r.ReadToEnd());
                    _exemptFoldersFile = existingFile ?? new ExemptFoldersFile();
                }
                catch
                {
                    _exemptFoldersFile = new ExemptFoldersFile();
                }
            }
        }

        private void GetUnusedMediaItems(MediaItemWrapper root)
        {
            var children = _mediaService.GetPagedChildren(root.Media.Id, 0, 10000, out long total);
            bool found = false;
            _mediaRemoveContext.TotalAmountOfMedia += children.Count();
            foreach (var mediaItem in children)
            {
                found = true;
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem, root));
            }
            if (found)
            {
                return;
            }

            if (_exemptFoldersFile.AutomaticFolders.SingleOrDefault(folder => folder.FolderId == root.Media.ParentId) == null 
                && _exemptFoldersFile.ManuallyDefinedFolders.SingleOrDefault(folder => folder.FolderId == root.Media.ParentId) == null)
            {
                IEnumerable<IRelation> mediaRelations = _relationService.GetByChildId(root.Media.Id);
                var itemParent = _mediaService.GetById(root.Media.ParentId);
                var skipThis = false;
                if (itemParent.ContentType.Alias == "Folder" && _relationService.GetByChildId(itemParent.Id).Any())
                {
                    _exemptFoldersFile.AutomaticFolders.Add(
                        new ExemptFolder
                        {
                            FolderId = itemParent.Id,
                            FolderDescription = itemParent.Name ?? "Exempt folder"
                        }
                    );
                    skipThis = true;
                }
                if (!skipThis && !mediaRelations.Any() && root.Media.ContentType.Alias != "Folder")
                {
                    _mediaRemoveContext.UnusedMedia.Add(root);
                }
            }
        }

        public void FindUnusedMedia()
        {
            _mediaRemoveContext.UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
            _mediaRemoveContext.IsProcessingMedia = true;
            _mediaRemoveContext.TotalAmountOfMedia = 0;
            var mediaItems = _mediaService.GetRootMedia();
            _mediaRemoveContext.TotalAmountOfMedia += mediaItems.Count();
            foreach (var mediaItem in mediaItems)
            {
                GetUnusedMediaItems(new MediaItemWrapper(mediaItem));
            }

            using (StreamWriter sw = File.CreateText(_exemptFoldersFilePath))
            {
                sw.Write(JsonConvert.SerializeObject( _exemptFoldersFile, Formatting.Indented));
            }

            _mediaRemoveContext.IsProcessingMedia = false;
        }
    }
}
