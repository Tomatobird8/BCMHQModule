using System;
using System.Collections.Generic;
using System.Text;

namespace BCMHQModule
{
    /// <summary>
    /// Used for matching the game version, BCM version and required patches.
    /// </summary>
    /// <param name="version">Version of Lethal Company</param>
    /// <param name="bcmVersion">Version of Brutal Company</param>
    /// <param name="types">Array of types of each required patch class</param>
    internal class VersionDefinition(BCMHQModule.Versions version, string bcmVersion, Type[] types)
    {
        public BCMHQModule.Versions version = version;
        public string bcmVersion = bcmVersion;
        public Type[] types = types;
    }
}
