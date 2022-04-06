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

        string terminalName = "";
        bool debugLog = false,
            licenced = false;

        List<Configuration> camera = new List<Configuration>()
        {
            new Configuration(),
            new Configuration(),
            new Configuration()
        };

        #endregion

        #region Properties

        public List<Configuration> Camera { get => camera; set => camera = value; }
        public bool DebugLog { get => debugLog; set => debugLog = value; }
        public string TerminalName { get => terminalName; set => terminalName = value; }
        public bool Licenced { get => licenced; set => licenced = value; }

        #endregion
    }
}
