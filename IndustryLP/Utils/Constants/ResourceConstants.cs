namespace IndustryLP.Utils.Constants
{
    internal static class ResourceConstants
    {
        /// <summary>
        /// The resources path in the assembly
        /// </summary>
        public static string ResourcesPath => $"{LibraryConstants.AssemblyName}.Resources";

        /// <summary>
        /// The icons path in the assembly
        /// </summary>
        public static string IconsPath => $"{ResourcesPath}.Icons";

        /// <summary>
        /// The distribution button path in the assembly
        /// </summary>
        public static string DistributionsPath => $"{ResourcesPath}.Distributions";

        /// <summary>
        /// The main toolbar icon
        /// </summary>
        public static string ToolbarNormal => "ToolbarIconNormal";

        /// <summary>
        /// The main toolbar icon when the mouse is hover
        /// </summary>
        public static string ToolbarHovered => "ToolbarIconHovered";

        /// <summary>
        /// The main toolbar icon when is focused
        /// </summary>
        public static string ToolbarFocused => "ToolbarIconFocused";

        /// <summary>
        /// The main toolbar icon when is pressed
        /// </summary>
        public static string ToolbarPressed => "ToolbarIconPressed";

        /// <summary>
        /// The main toolbar icon when is disabled
        /// </summary>
        public static string ToolbarDisabled => "ToolbarIconDisabled";

        /// <summary>
        /// The background of the main toolbar icon
        /// </summary>
        public static string ToolbarFgNormal => "ToolbarIconGroup1Normal";

        /// <summary>
        /// The background of the main toolbar icon when the mouse is hover
        /// </summary>
        public static string ToolbarFgHovered => "ToolbarIconGroup1Hovered";

        /// <summary>
        /// The background of the main toolbar icon when is focused
        /// </summary>
        public static string ToolbarFgFocused => "ToolbarIconGroup1Focused";

        /// <summary>
        /// The background of the main toolbar icon when is pressed
        /// </summary>
        public static string ToolbarFgPressed => "ToolbarIconGroup1Pressed";

        /// <summary>
        /// The background of the main toolbar icon when is disabled
        /// </summary>
        public static string ToolbarFgDisabled => "ToolbarIconGroup1Disabled";

        /// <summary>
        /// The arrow to change down the solution
        /// </summary>
        public static string SolutionDown => "SolutionOptionDown";

        /// <summary>
        /// The arrow to change up the solution
        /// </summary>
        public static string SolutionUp => "SolutionOptionUp";

        /// <summary>
        /// The icon to build the solution
        /// </summary>
        public static string OptionBuild => "ZoningOptionBuild";

        /// <summary>
        /// The icon to move the zoning
        /// </summary>
        public static string OptionMove => "ZoningOptionMove";

        /// <summary>
        /// The icon to create the zoning
        /// </summary>
        public static string OptionZoning => "ZoningOptionMarquee";

        /// <summary>
        /// The background of option button
        /// </summary>
        public static string OptionFgNormal => "OptionBase";

        /// <summary>
        /// The background of option button when the mouse is hover
        /// </summary>
        public static string OptionFgHovered => "OptionBaseHovered";

        /// <summary>
        /// The background of option button when is focused
        /// </summary>
        public static string OptionFgFocused => "OptionBaseFocused";

        /// <summary>
        /// The background of option button when is pressed
        /// </summary>
        public static string OptionFgPressed => "OptionBasePressed";

        /// <summary>
        /// The background of option button when is disabled
        /// </summary>
        public static string OptionFgDisabled => "OptionBaseDisabled";

        /// <summary>
        /// The grid distribution
        /// </summary>
        public static string GridDistributionNormal => "GridDistribution";

        /// <summary>
        /// The grid distribution when is hovered
        /// </summary>
        public static string GridDistributionHovered => "GridDistributionHovered";

        /// <summary>
        /// The grid distribution when is pressed
        /// </summary>
        public static string GridDistributionPressed => "GridDistributionPressed";

        /// <summary>
        /// The grid distribution when is focused
        /// </summary>
        public static string GridDistributionFocused => "GridDistributionFocused";

        /// <summary>
        /// The line distribution
        /// </summary>
        public static string LineDistributionNormal => "LineDistribution";

        /// <summary>
        /// The line distribution when is hovered
        /// </summary>
        public static string LineDistributionHovered => "LineDistributionHovered";

        /// <summary>
        /// The line distribution when is pressed
        /// </summary>
        public static string LineDistributionPressed => "LineDistributionPressed";

        /// <summary>
        /// The grid distribution when is focused
        /// </summary>
        public static string LineDistributionFocused => "LineDistributionFocused";

        /// <summary>
        /// The mine distribution
        /// </summary>
        public static string MineDistributionNormal => "MineDistribution";

        /// <summary>
        /// The mine distribution when is hovered
        /// </summary>
        public static string MineDistributionHovered => "MineDistributionHovered";

        /// <summary>
        /// The grid distribution when is pressed
        /// </summary>
        public static string MineDistributionPressed => "MineDistributionPressed";

        /// <summary>
        /// The grid distribution when is focused
        /// </summary>
        public static string MineDistributionFocused => "MineDistributionFocused";

        /// <summary>
        /// The disabled distribution
        /// </summary>
        public static string DistributionDisabled => "DistributionDisabled";
        
        /// <summary>
        /// Name of the atlas
        /// </summary>
        public static string AtlasName => $"{LibraryConstants.LibPrefix}.Atlas";

        /// <summary>
        /// Name of the icons atlas
        /// </summary>
        public static string IconAtlasName => $"{AtlasName}.Icons";

        /// <summary>
        /// Name of the distribution thumbnails atlas
        /// </summary>
        public static string DistributionAtlasName => $"{AtlasName}.DistributionThumbnails";
    }
}
