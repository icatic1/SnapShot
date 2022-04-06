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

        List<Configuration> camera = new List<Configuration>()
        {
            new Configuration(),
            new Configuration(),
            new Configuration()
        };

        bool debugLog = false;
        string terminalName = "";

        #endregion

        #region Properties

        public List<Configuration> Camera { get => camera; set => camera = value; }
        public bool DebugLog { get => debugLog; set => debugLog = value; }
        public string TerminalName { get => terminalName; set => terminalName = value; }

        #endregion
    }
}
