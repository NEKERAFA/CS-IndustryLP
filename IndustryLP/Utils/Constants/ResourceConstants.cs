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
        /// 
        /// </summary>
        public static string GeneratorIcon => "Build";

        /// <summary>
        /// Name of the atlas
        /// </summary>
        public static string AtlasName => $"{LibraryConstants.LibPrefix}_Atlas";
    }
}
