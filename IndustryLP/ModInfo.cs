using ICities;

namespace IndustryLP
{
    /// <summary>
    /// Information about the new mod extension
    /// </summary>
    public class ModInfo : IUserMod
    {
        #region Mod Properties

        /// <summary>
        /// Current version of the mod
        /// </summary>
        private const string Version = "1.0.0";

        /// <summary>
        /// Current git branch
        /// </summary>
#if DEBUG
        private const string Branch = "us1";
#else
        private const string Branch = null;
#endif

        public static readonly string ModName = string.IsNullOrEmpty(Branch) ? $"IndustryLP {Version}" : $"IndustryLP {Version}-{Branch}";

        #endregion

        #region User Mod

        /// <summary>
        /// Name of the mod extension
        /// </summary>
        public string Name => ModName;
        
        /// <summary>
        /// Description of the mod extension
        /// </summary>
        public string Description => "Industrial estate generator that uses Logic Programming";

        #endregion
    }
}
