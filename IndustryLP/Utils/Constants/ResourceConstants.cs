namespace IndustryLP.Utils.Constants
{
    internal static class ResourceConstants
    {
        /// <summary>
        /// The resources path in the assembly
        /// </summary>
        public static string ResourcePath => $"{LibraryConstants.AssemblyName}.Resources";

        /// <summary>
        /// The icons path in the assembly
        /// </summary>
        public static string IconPath => $"{ResourcePath}.Icons";

        /// <summary>
        /// Name of the resource that appears as the background of a button
        /// </summary>
        public static string ButtonNormal => "ButtonNormal";

        /// <summary>
        /// Name of the resource that appears as the background of a button when the cursor is over
        /// </summary>
        public static string ButtonHover => "ButtonHover";

        /// <summary>
        /// Name of the resource that appears as the background of a button when the button is pressed
        /// </summary>
        public static string ButtonPushed => "ButtonPushed";

        /// <summary>
        /// Name of area selection resource
        /// </summary>
        public static string SelectionIcon => "Selection";

        /// <summary>
        /// Name of build resource
        /// </summary>
        public static string BuildIcon => "Build";

        /// <summary>
        /// Name of options resource
        /// </summary>
        public static string OptionsIcon => "Options";

        /// <summary>
        /// Name of up arrow resource
        /// </summary>
        public static string UpArrowIcon => "UpArrow";

        /// <summary>
        /// Name of down arrow resource
        /// </summary>
        public static string DownArrowIcon => "DownArrow";

        /// <summary>
        /// Name of the atlas
        /// </summary>
        public static string AtlasName => $"{LibraryConstants.LibPrefix}_Atlas";
    }
}
