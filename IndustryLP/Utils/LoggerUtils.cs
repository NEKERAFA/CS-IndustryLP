using IndustryLP.Utils.Constants;
using System;
using System.Diagnostics;
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
            var msg = new StringBuilder("");

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var val = values[i];
                    msg.Append(val == null ? "null" : val.ToString());

                    if (i < values.Length - 1)
                    {
                        msg.Append(", ");
                    }
                }
            }

            return msg.ToString();
        }

        private static string GetHeader()
        {
            var msg = new StringBuilder("");
            msg.Append(DateTime.Now.ToString("r"));
            msg.Append(" - ");
            msg.Append(LibraryConstants.AssemblyName);

            var lastFrame = new StackTrace().GetFrame(2);

            if (lastFrame != null) {
                msg.Append(" - ");
                msg.Append(lastFrame.GetMethod().DeclaringType.FullName);
                msg.Append(": ");
                msg.Append(lastFrame.GetMethod().Name);
                msg.Append(": ");
            }

            return msg.ToString();
        }

        /// <summary>
        /// Prints a debug message onto the output file
        /// </summary>
        public static void Log(params object[] values)
        {
            UnityEngine.Debug.Log($"{GetHeader()}: {GetParamsAsString(values)}");
        }

        /// <summary>
        /// Prints a warning message onto the output file
        /// </summary>
        public static void Warning(params object[] values)
        {
            UnityEngine.Debug.LogWarning($"{GetHeader()}: {GetParamsAsString(values)}");
        }

        /// <summary>
        /// Prints an error message onto the output file
        /// </summary>
        public static void Error(params object[] values)
        {
            UnityEngine.Debug.LogError($"{GetHeader()}: {GetParamsAsString(values)}");
        }

        /// <summary>
        /// Prints an error message onto the output file
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> object to print message errors</param>
        public static void Error(Exception ex, params object[] values)
        {
            StringBuilder msg = new StringBuilder();
            if (values != null && values.Length > 0)
            {
                msg.Append(GetParamsAsString(values));
                msg.Append(", ");
            }
            msg.AppendLine($"{ex.Message}");
            msg.AppendLine(ex.StackTrace);

            UnityEngine.Debug.LogError($"{GetHeader()}: {ex.GetType().FullName} : {msg}");
        }
    }
}
