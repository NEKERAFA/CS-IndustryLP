using ColossalFramework.IO;
using System.IO;

namespace IndustryLP.Utils.Constants
{
    internal static class ClingoConstants
    {
#if DEBUG
        public static string ClingoPath => Path.Combine(DataLocation.modsPath, "IndustryLP");
#else
        public static string ClingoPath => "";
#endif

        public static string LogicProgramPath => Path.Combine(ClingoPath, "logic_program");

        public static string ItemDefinitionFile => Path.Combine(LogicProgramPath, "item_definition.lp");

        public static string IndustryGeneratorFile => Path.Combine(LogicProgramPath, "generator.lp");
    }
}
