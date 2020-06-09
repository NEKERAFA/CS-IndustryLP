using ClingoSharp;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using System;
using System.Collections.Generic;

namespace IndustryLP.DomainDefinition
{
    internal static class ClingoControlThread
    {
        internal static Clingo clingo;

        public static void LoadClingo()
        {
            try
            {
                clingo = new Clingo(ClingoConstants.ClingoPath);
            }
            catch (Exception ex)
            {
                LoggerUtils.Error(ex);
            }
        }

        public static void Generate()
        {
            try
            {
                Control program = new Control();

                program.Load(ClingoConstants.ItemDefinitionFile);
                program.Load(ClingoConstants.IndustryGeneratorFile);

                var parts = new List<Tuple<string, List<Symbol>>>()
                {
                    new Tuple<string, List<Symbol>>("base", new List<Symbol>())
                };
                program.Ground(parts);

                SolveHandle handle = program.Solve(onModel: m =>
                {
                    LoggerUtils.Log(m.ToString());
                    return false;
                }, async: true);

                while (!handle.Wait(0)) continue;
            }
            catch(Exception ex)
            {
                LoggerUtils.Error(ex);
            }
        }
    }
}
