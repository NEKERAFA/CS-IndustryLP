using ColossalFramework.Plugins;
using IndustryLP.Constants;
using System.Text;

namespace IndustryLP.Common
{
    internal static class DebugLogger
    {
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
