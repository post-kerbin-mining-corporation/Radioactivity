using System;
namespace Radioactivity.UI
{
    /// <summary>
    /// The editor window allows the player to vary exposures for ambient radiation bits
    /// in the editor only
    /// </summary>
    public class UIEditorWindow: UIWindow
    {
        
        public UIEditorWindow(Random randomizer, RadioactivityUI uiHost): base(randomizer, uiHost)
        {
            Utils.Log("[UIEditorWindow]: Initialized");
        }

        /// <summary>
        /// Does window drawing
        /// </summary>
        public void Draw()
        {
            
        }

        /// <summary>
        /// Does any per-frame updates
        /// </summary>
        public void Update()
        {
            
        }
    }
}
