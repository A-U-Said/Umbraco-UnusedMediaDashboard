using UnusedMediaDashboard.Models;
using System.Collections.Generic;

namespace UnusedMediaDashboard.Models
{
    public class ExemptFoldersFile
    {
        public List<ExemptFolder> ManuallyDefinedFolders = new List<ExemptFolder>();
        public List<ExemptFolder> AutomaticFolders = new List<ExemptFolder>();
    }
}
