using System;
using UnityEngine;

namespace Radioactivity.UI
{
    public class OverlayRenderer
    {
        public bool Drawn { get { return drawn; } }

        protected bool drawn = false;
        protected GameObject root;

        public OverlayRenderer()
        {
        }
    }
}
