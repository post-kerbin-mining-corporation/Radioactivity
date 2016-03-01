// an interface that all radiation absorbing modules should implement
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radioactivity.Interfaces
{
    public interface IRadiationAbsorber
    {
        // Get the name of the alias of this absorber type in UIs
        string GetAlias();

        // Get the name of the RadioactiveSink on this part to use
        string GetSinkName();

        // Add radiation to a part. Should propagate radiation to kerbals, track it, etc.
        void AddRadiation(float amt);


    }
}
