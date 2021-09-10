using ColossalFramework.IO;
using System.IO;

namespace IndustryLP.Utils.Constants
{
    internal static class ClingoConstants
    {
        public static string ClingoPath => Path.Combine(DataLocation.modsPath, "IndustryLP");

        public static string LogicProgramPath => Path.Combine(ClingoPath, "logic_program");
    }
}
