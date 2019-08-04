using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace BoxStorage.Globals
{
    public static class FromConfig
    {
        // data from configuration-file
        public static int CriticalMinBoxes { get; private set; }
        public static int MaxBoxesSameSize { get; private set; }
        public static TimeSpan UnusingBoxMaxTime { get; private set; } // when and which boxes move to trash
        public static TimeSpan CheckingUnusedBox { get; private set; } // for timer - when run checking and deleting boxes

        // for avoid double-loading
        internal static bool Loaded { get; private set; }

        public static void LoadConfigData()
        {
            if (!Loaded)
            {
                // default values:
                int minBox = 5;
                int maxBox = 50;
                TimeSpan unusingMax = new TimeSpan(24, 0, 0);
                TimeSpan checking = new TimeSpan(24, 0, 0);

                // try load from configuration or place default
                string temp;
                int result;

                temp = ConfigurationManager.AppSettings["MaxBoxesSameSize"];
                if (int.TryParse(temp, out result) && result > 0)
                    MaxBoxesSameSize = result;
                else
                    MaxBoxesSameSize = maxBox;

                temp = ConfigurationManager.AppSettings["CriticalMinBoxes"];
                if (int.TryParse(temp, out result) && result >= 0 && result < MaxBoxesSameSize)
                    CriticalMinBoxes = result;
                else
                    CriticalMinBoxes = minBox;

                TimeSpan resultT;
                temp = ConfigurationManager.AppSettings["UnusingBoxMaxTime"];
                if (TimeSpan.TryParse(temp, out resultT))
                    UnusingBoxMaxTime = resultT;
                else
                    UnusingBoxMaxTime = unusingMax;

                temp = ConfigurationManager.AppSettings["CheckingUnusedBox"];
                if (TimeSpan.TryParse(temp, out resultT))
                    CheckingUnusedBox = resultT;
                else
                    CheckingUnusedBox = checking;
            }
        }
    }
}
