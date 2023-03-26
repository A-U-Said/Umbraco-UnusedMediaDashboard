using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace UnusedMediaDashboard.Models
{
    public class MediaItemWrapper
    {
        public IMedia Media { get; set; }
        public UnusedMedia Model { get; set; }

        public MediaItemWrapper(IMedia media, MediaItemWrapper previous)
        {
            Media = media;
            Model = new UnusedMedia
            {
                Name = media.Name,
                Path = $"{previous.Model.Path}/{media.Name}",
                Id = media.Id,
                Source = media.ContentType.Alias != "Image" 
                    ? media.GetValue<string>("umbracoFile") 
                    : JsonConvert.DeserializeObject<ImageCropperValue>(media.GetValue<string>("umbracoFile")).Src,
                ParentId = media.ParentId
            };
        }

        public MediaItemWrapper(IMedia media)
        {
            Media = media;
            Model = new UnusedMedia
            {
                Name = media.Name,
                Path = $"{media.Name}",
                Id = media.Id,
                Source = media.HasProperty("umbracoFile") ? media.GetValue<string>("umbracoFile") : null,
                ParentId = media.ParentId
            };
        }
    }
}
