using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity.Interfaces
{
    public interface IRadiationEmitter
    {
        // Get the name of the alias of this emitter type in UIs
        string GetAlias();

        // Get the name of the RadioactiveSource on this part to use
        string GetSourceName();

        // Return true if the source is emitting
        bool IsEmitting();

        // Get the current radiation emission from this module
        float GetEmission(); 
    }
}
