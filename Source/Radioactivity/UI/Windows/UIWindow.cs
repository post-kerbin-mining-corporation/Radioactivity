using System;
namespace Radioactivity.UI
{
    public class UIWindow
    {
        public bool Drawn 
        {
            get { return drawn; }
            set { drawn = value; }
        }
        protected int windowID = 0;
        protected RadioactivityUI host;
        protected bool drawn = false;

        public UIWindow(System.Random randomizer, RadioactivityUI uiHost)
        {
            host = uiHost;
            windowID = randomizer.Next();
        }


    }
}
