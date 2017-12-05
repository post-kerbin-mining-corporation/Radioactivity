using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using KSP.IO;
namespace Radioactivity
{
    /// <summary>
    /// Settings are items that are set by the player in the KSP settings screen
    /// </summary>
    public class RadioactivitySimulationSettings: GameParameters.CustomParameterNode

    {
        public override string DisplaySection
        {
            get
            {
                return Section;
            }
        }

        public override string Section
        {
            get
            {
                return "Radioactivity";
            }
        }

        public override string Title
        {
            get
            {
                return "Simulation Parameters";
            }
        }

        public override int SectionOrder
        {
            get
            {
                return 1;
            }
        }

        public override GameParameters.GameMode GameMode
        {
            get
            {
                return GameParameters.GameMode.ANY;
            }
        }

        public override bool HasPresets
        {
            get
            {
                return false;
            }
        }

        public static bool SimulateBeltRadiation
        {
            get
            {
                RadioactivitySimulationSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivitySimulationSettings>();
                return settings.simulateBeltRadiation;
            }
        }
        public static bool SimulateLocalRadiation
        {
            get
            {
                RadioactivitySimulationSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivitySimulationSettings>();
                return settings.simulateLocalRadiation;
            }
        }
        public static bool SimulateCosmicRadiation
        {
            get
            {
                RadioactivitySimulationSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivitySimulationSettings>();
                return settings.simulateCosmicRadiation;
            }
        }
        public static bool SimulatePointRadiation
        {
            get
            {
                RadioactivitySimulationSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivitySimulationSettings>();
                return settings.simulatePointRadiation;
            }
        }
        public static bool SimulateSolarRadiation
        {
            get
            {
                RadioactivitySimulationSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivitySimulationSettings>();
                return settings.simulateSolarRadiation;
            }
        }
      

        [GameParameters.CustomParameterUI("Simulate Point Radiation", toolTip = "If enabled, you must protect your crew from radiation sources on your ship", autoPersistance = true)]
        public bool simulatePointRadiation = true;

        [GameParameters.CustomParameterUI("Simulate Planetary Radiation", toolTip = "If enabled, you must protect your crew from radiation emitted from planets and particular biomes", autoPersistance = true)]
        public bool simulateLocalRadiation = false;

        [GameParameters.CustomParameterUI("Simulate Cosmic Radiation", toolTip = "If enabled, you must protect your crew from a slow, constant accumulation of cosmic radiation", autoPersistance = true)]
        public bool simulateCosmicRadiation = false;

        [GameParameters.CustomParameterUI("Simulate Solar Radiation", toolTip = "If enabled, you must risk exposure to radioactive emissions from the sun", autoPersistance = true)]
        public bool simulateSolarRadiation = false;

        [GameParameters.CustomParameterUI("Simulate Belt Radiation", toolTip = "If enabled, you must protect your crews from radiation emitted from particle belts around planets", autoPersistance = true)]
        public bool simulateBeltRadiation = false;
    }

    public class RadioactivityEffectSettings : GameParameters.CustomParameterNode

    {
        public override string DisplaySection
        {
            get
            {
                return Section;
            }
        }

        public override string Section
        {
            get
            {
                return "Radioactivity";
            }
        }

        public override string Title
        {
            get
            {
                return "Radiation Effects";
            }
        }

        public override int SectionOrder
        {
            get
            {
                return 2;
            }
        }

        public override GameParameters.GameMode GameMode
        {
            get
            {
                return GameParameters.GameMode.ANY;
            }
        }

        public override bool HasPresets
        {
            get
            {
                return false;
            }
        }
        public static bool EnableKerbalSickness
        {
            get
            {
                RadioactivityEffectSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivityEffectSettings>();
                return settings.kerbalSickness;
            }
        }
        public static bool EnableKerbalDeath
        {
            get
            {
                RadioactivityEffectSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivityEffectSettings>();
                return settings.kerbalDeath;
            }
        }
        public static bool EnableInstrumentDegradation
        {
            get
            {
                RadioactivityEffectSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivityEffectSettings>();
                return settings.instrumentDegradation;
            }
        }
        public static bool EnableControlDegradation
        {
            get
            {
                RadioactivityEffectSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<RadioactivityEffectSettings>();
                return settings.controlDegradation;
            }
        }

        [GameParameters.CustomParameterUI("Enable Kerbal Sickness", toolTip = "If enabled, Kerbals will get radiation sickness.", autoPersistance = true)]
        public bool kerbalSickness = true;

        [GameParameters.CustomParameterUI("Enable Kerbal Death", toolTip = "If enabled, Kerbals will die.", autoPersistance = true)]
        public bool kerbalDeath = false;

        [GameParameters.CustomParameterUI("Enable Instrument Degradation", toolTip = "If enabled, science instruments will suffer from degredation in high radiation environments", autoPersistance = true)]
        public bool instrumentDegradation = false;

        [GameParameters.CustomParameterUI("Enable Control Degredation", toolTip = "If enabled, probe cores will suffer from degredation in high radiation environments", autoPersistance = true)]
        public bool controlDegradation = false;

    }
}
