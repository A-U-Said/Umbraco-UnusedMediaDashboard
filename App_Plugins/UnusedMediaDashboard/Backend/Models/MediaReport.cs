using System.Collections.Generic;

namespace UnusedMediaDashboard.Models
{
    public class MediaReport
    {
        public bool IsProcessingMedia { get; set; }
        public IEnumerable<UnusedMedia> Data { get; set; }
        public int TotalAmountOfMedia { get; set; }
        public int TotalUnusedMedia { get; set; }
    }
}