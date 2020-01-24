using ColossalFramework.Plugins;
using IndustryLP.Constants;
using System.Text;

namespace IndustryLP.Utils
{
    /// <summary>
    /// This class represents a wrapper between <see cref="DebugOutputPanel"/> class
    /// </summary>
    static class LoggerUtils
    {
        /// <summary>
        /// Creates a message onto the output file
        /// </summary>
        /// <param name="values"></param>
        public static void Debug(params object[] values)
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
            
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, $"{LibraryConstants.LibPrefix}: {msg}");
        }
    }
}
