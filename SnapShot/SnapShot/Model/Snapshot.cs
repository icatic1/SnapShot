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

        #endregion
    }
}
