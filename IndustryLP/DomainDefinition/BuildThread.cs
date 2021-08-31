using ClingoSharp;
using IndustryLP.Entities;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IndustryLP.DomainDefinition
{
    /// <summary>
    /// This class represents the thread that calls clingo in order to create new solutions
    /// </summary>
    internal class BuildThread : MonoBehaviour
    {
        #region Attributtes

        private static Clingo clingo = null;

        private int m_maxSolutions;
        private int m_rows;
        private int m_cols;
        private Control m_program;
        private SolveHandle m_solver;
        private ModelEnumerator m_modelHandle;
        private List<Region> m_results;

        #endregion

        #region Class Properties

        /// <summary>
        /// The name of the button
        /// </summary>
        public static string ObjectName => $"{LibraryConstants.ObjectPrefix}_Generator";

        #endregion

        #region Instance Properties

        /// <summary>
        /// The current number of solutions
        /// </summary>
        public int Count => m_results.Count;

        /// <summary>
        /// <c>true</c> if the generator is running, <c>false</c> otherwise.
        /// </summary>
        public bool IsAlive { get; private set; }

        /// <summary>
        /// <c>true</c> if the generator finish the search of solutions.
        /// </summary>
        public bool IsFinished { get; private set; }

        #endregion

        #region Instance Methods

        #region Private Methods

        private void LoadClingo()
        {
            try
            {
                //LoggerUtils.Log($"Loading clingo from {ClingoConstants.ClingoPath}");
                clingo = new Clingo(ClingoConstants.ClingoPath);
            }
            catch (Exception ex)
            {
                LoggerUtils.Error(ex, "cannot load clingo: ");
            }
        }

        private void LoadProgram()
        {
            try
            {
                m_program = new Control(args: new List<string>() { m_maxSolutions.ToString(), "--const", $"rows={m_rows}", "--const", $"columns={m_cols}" });

                LoggerUtils.Log("Loading definition files");

                var files = Directory.GetFiles(ClingoConstants.LogicProgramPath, "*.lp");

                foreach (var file in files)
                {
                    m_program.Load(file);
                }
            }
            catch (Exception ex)
            {
                LoggerUtils.Error(ex, "cannot load logic program: ");
            }
        }

        private void SolveProgram()
        {
            try
            {
                LoggerUtils.Log("Grounding program");

                var parts = new List<Tuple<string, List<Symbol>>>()
                {
                    new Tuple<string, List<Symbol>>("base", new List<Symbol>())
                };
                m_program.Ground(parts);

                LoggerUtils.Log("Solving program");
                m_solver = m_program.Solve(yield: true);
            }
            catch(Exception ex)
            {
                LoggerUtils.Error(ex, "cannot solve logic program: ");
            }
        }

        private Model NewSolution()
        {
            try
            {
                if (m_modelHandle.MoveNext())
                {
                    return m_modelHandle.Current;
                }

                return null;
            }
            catch (Exception ex)
            {
                LoggerUtils.Error(ex, "cannot get new solution: ");
                return null;
            }
        }

        private Region GetSolution(Model model)
        {
            if (model == null) return null;

            var region = new Region()
            {
                Rows = m_program.GetConst("rows").Number,
                Columns = m_program.GetConst("columns").Number,
            };

            region.Parcels = new string[region.Rows, region.Columns];

            var atoms = model.GetSymbols(shown: true);

            LoggerUtils.Log($"Atoms count: {atoms.Count}");

            foreach (var symbol in atoms)
            {
                if (symbol.Name.Equals("parcel"))
                {
                    var arguments = symbol.Arguments;
                    if (arguments.Count == 3)
                    {
                        var row = arguments[0].Number;
                        var column = arguments[1].Number;
                        var parcel = arguments[2].String;
                        LoggerUtils.Log($"{symbol} - ({row}, {column}, {parcel})");
                        region.Parcels[row, column] = parcel;
                    }
                    else
                    {
                        LoggerUtils.Error("parcel must be 3 argumen");
                    }
                }
            }

            return region;
        }

        private void LoadStringParcels()
        {
            var parcels = new StringBuilder();

            for (var i = 0u; i < PrefabCollection<BuildingInfo>.PrefabCount(); i++)
            {
                var prefab = PrefabCollection<BuildingInfo>.GetPrefab(i);

                if (prefab != null && prefab.m_class.m_subService == ItemClass.SubService.IndustrialGeneric)
                {
                    parcels.AppendLine($"str_parcel(\"{prefab.name}\").");
                }
            }

            LoggerUtils.Log("Parcels", parcels.ToString().Replace("\n", "\\n"));

            m_program.Add("base", new List<string>(), parcels.ToString());
        }

        private void LoadPreferences(List<BuildingAtom> preferences)
        {
            var strPreferences = new StringBuilder();

            foreach (var preference in preferences)
            {
                strPreferences.AppendLine($"parcel({preference.Row}, {preference.Column}, \"{preference.Name}\").");
            }

            m_program.Add("base", new List<string>(), strPreferences.ToString());
        }

        private void LoadRestrictions(List<BuildingAtom> restrictions)
        {
            var strRestrictions = new StringBuilder();

            foreach (var restriction in restrictions)
            {
                strRestrictions.AppendLine($":- parcel({restriction.Row}, {restriction.Column}, \"{restriction.Name}\").");
            }

            m_program.Add("base", new List<string>(), strRestrictions.ToString());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new clingo program and starts the searching
        /// </summary>
        /// <param name="maxSolutions">The number of solutions that clingo will find. <c>0</c> for no limit</param>
        /// <param name="rows">The number of rows in the parcel</param>
        /// <param name="cols">The number of cols in the parcel</param>
        public void StartProgram(int maxSolutions, int rows, int cols, List<BuildingAtom> preferences, List<BuildingAtom> restrictions)
        {
            try
            {
                if (clingo == null)
                    LoadClingo();

                m_maxSolutions = maxSolutions;
                m_rows = rows;
                m_cols = cols;

                LoadProgram();

                LoadStringParcels();
                
                if (preferences.Any()) LoadPreferences(preferences);
                if (restrictions.Any()) LoadRestrictions(restrictions);

                SolveProgram();

                m_modelHandle = m_solver.GetEnumerator() as ModelEnumerator;

                m_results = new List<Region>();

                IsAlive = true;
                IsFinished = false;
            }
            catch (Exception ex)
            {
                LoggerUtils.Error(ex);
            }
        }

        /// <summary>
        /// Gets a solution in <paramref name="position"/>
        /// </summary>
        /// <param name="position">The position of the solution</param>
        /// <returns>The solution</returns>
        public Region GetSolution(int position)
        {
            return m_results[position];
        }

        /// <summary>
        /// Stop searching new solutions
        /// </summary>
        public void Stop()
        {
            IsAlive = false;
        }

        #endregion

        #endregion

        #region Unity Behaviour

        public void Awake()
        {
            name = ObjectName;
        }

        public void Update()
        {
            if (IsAlive && !IsFinished)
            {
                //LoggerUtils.Log("Getting new model");

                Model model = NewSolution();

                if (model != null)
                {
                    LoggerUtils.Log($"Saving model: {model}");
                    m_results.Add(GetSolution(model));
                }
                else
                {
                    //LoggerUtils.Log("Finished!");
                    IsFinished = true;
                }
            }
            else if (!IsAlive)
            {
                m_program.Dispose();
            }
        }

        #endregion
    }
}
