using ClingoSharp;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using System;
using System.Collections.Generic;

namespace IndustryLP.DomainDefinition
{
    internal static class ClingoControlThread
    {
        internal static Clingo clingo = null;

        public static void LoadClingo()
        {
            try
            {
                LoggerUtils.Log($"Clingo Path: {ClingoConstants.ClingoPath}");
                clingo = new Clingo(ClingoConstants.ClingoPath);
            }
            catch (Exception ex)
            {
                LoggerUtils.Error(ex);
            }
        }

        public static void Generate()
        {
            LoggerUtils.Log("Creating program");

            Control program = new Control();

            LoggerUtils.Log("Loading files");

            program.Load(ClingoConstants.ItemDefinitionFile);
            program.Load(ClingoConstants.IndustryGeneratorFile);

            LoggerUtils.Log("Grounding program");

            var parts = new List<Tuple<string, List<Symbol>>>()
            {
                new Tuple<string, List<Symbol>>("base", new List<Symbol>())
            };
            program.Ground(parts);

            LoggerUtils.Log("Solve program");

            SolveHandle handle = program.Solve(yield: true, async: true);

            foreach (Model model in handle)
            {
                LoggerUtils.Log($"Model: {model}");

                SolveResult result = handle.Get();

                LoggerUtils.Log($"SAT: {result.IsSatisfiable}");

                handle.Cancel();
            }

            LoggerUtils.Log("Done");
        }
    }
}
