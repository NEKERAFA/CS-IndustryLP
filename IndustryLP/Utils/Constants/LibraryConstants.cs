namespace IndustryLP.Utils.Constants
{
    internal static class LibraryConstants
    {
        /// <summary>
        /// The name of the assembly
        /// </summary>
        public static string AssemblyName => "IndustryLP";

        /// <summary>
        /// The prefix of IndustryLP Library
        /// </summary>
        public static string LibPrefix => "IndustryLP";

        /// <summary>
        /// The prefix of all IndustryLP Gameobjects
        /// </summary>
        public static string ObjectPrefix => $"{LibPrefix}.GameObject";

        /// <summary>
        /// The prefix of all IndustryLP Unity UI objects
        /// </summary>
        public static string UIPrefix => $"{ObjectPrefix}.UI";

        /// <summary>
        /// The minimun number of rows
        /// </summary>
        public static int MinRows => 3;

        /// <summary>
        /// The minimun number of columns
        /// </summary>
        public static int MinColumns => 3;
    }
}
