using IndustryLP.Constants;
using System;
using System.Text;
using UnityEngine;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class represents a wrapper between <see cref="Debug"/> class
    /// </summary>
    internal static class LoggerUtils
    {
        /// <summary>
        /// Convert a list of objects into string
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string GetParamsAsString(object[] values)
        {
            StringBuilder msg = new StringBuilder("");
            for (int i = 0; i < values.Length; i++)
            {
                msg.Append(values[i].ToString());

                if (i < values.Length - 1)
                {
                    msg.Append(", ");
                }
            }

            return msg.ToString();
        }

        /// <summary>
        /// Prints a debug message onto the output file
        /// </summary>
        public static void Log(params object[] values)
        {
            Debug.Log($"{LibraryConstants.AssemblyName}: {GetParamsAsString(values)}");
        }

        /// <summary>
        /// Prints a warning message onto the output file
        /// </summary>
        public static void Warning(params object[] values)
        {
            Debug.LogWarning($"{LibraryConstants.AssemblyName}: {GetParamsAsString(values)}");
        }

        /// <summary>
        /// Prints an error message onto the output file
        /// </summary>
        public static void Error(params object[] values)
        {
            Debug.LogError($"{LibraryConstants.AssemblyName}: {GetParamsAsString(values)}");
        }

        /// <summary>
        /// Prints an error message onto the output file
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> object to print message errors</param>
        public static void Error(Exception ex, params object[] values)
        {
            StringBuilder msg = new StringBuilder("");
            if (values.Length > 0) msg.Append(GetParamsAsString(values));
            msg.AppendLine($", {ex.Message}");
            msg.AppendLine(ex.StackTrace);

            Debug.LogError($"{LibraryConstants.AssemblyName}: {msg.ToString()}");
        }
    }
}
