namespace IndustryLP.Constants
{
    internal static class ResourceConstants
    {
        /// <summary>
        /// The resources path in the assembly
        /// </summary>
        public static readonly string ResourcePath = $"{LibraryConstants.AssemblyName}.Resources";

        /// <summary>
        /// The icons path in the assembly
        /// </summary>
        public static readonly string IconPath = $"{ResourcePath}.Icons";

        /// <summary>
        /// Name of the resource that appears as the background of a button
        /// </summary>
        public static readonly string ButtonNormal = "ButtonNormal";

        /// <summary>
        /// Name of the resource that appears as the background of a button when the cursor is over
        /// </summary>
        public static readonly string ButtonHover = "ButtonHover";

        /// <summary>
        /// Name of area selection resource
        /// </summary>
        public static readonly string AreaSelectionIcon = "AreaSelection";
    }
}
