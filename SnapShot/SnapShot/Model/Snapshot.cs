using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapShot
{
    public class Snapshot
    {
        #region Attributes

        string terminalName = "",
               JSONImportRoute = "api/JSONConfiguration/JSONImport",
               JSONExportRoute = "api/JSONConfiguration/JSONExport",
               mediaExportRoute = "api/FileUpload/UploadLargeFile",
               listenerRoute = "api/FileUpload/GetStreamState";

        bool debugLog = false,
            licenced = false,
            connected = false;

        Configuration configuration = new Configuration();

        #endregion

        #region Properties

        public Configuration Configuration { get => configuration; set => configuration = value; }
        
        public bool DebugLog { get => debugLog; set => debugLog = value; }
        
        public string TerminalName { get => terminalName; set => terminalName = value; }
        
        public bool Licenced { get => licenced; set => licenced = value; }
        
        public bool Connected { get => connected; set => connected = value; }

        public string JSONImport { get => JSONImportRoute; set => JSONImportRoute = value; }

        public string JSONExport { get => JSONExportRoute; set => JSONExportRoute = value; }

        public string MediaExport { get => mediaExportRoute; set => mediaExportRoute = value; }

        public string Listener { get => listenerRoute; set => listenerRoute = value; }

        #endregion
    }
}
