using System.Collections.Concurrent;

namespace UnusedMediaDashboard.Models
{
    public class MediaRemoveContext
    {
        private static MediaRemoveContext instance;
        public MediaRemoveContext()
        {
            UnusedMedia = new ConcurrentBag<MediaItemWrapper>();
            IsProcessingMedia = false;
            instance = this;
        }

        public static MediaRemoveContext Current => instance ?? new MediaRemoveContext();
        public ConcurrentBag<MediaItemWrapper> UnusedMedia { get; set; }
        public bool IsProcessingMedia { get; set; }
        public int TotalAmountOfMedia { get; set; }
    }
}
