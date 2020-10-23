using IndustryLP.Utils.Constants;

namespace IndustryLP.UI
{
    internal class IndustryGroupPanel : GeneratedGroupPanel
    {
        #region Properties
        
        public static string Name => $"{LibraryConstants.ObjectPrefix}.IndustryGroupPanel";

        public override ItemClass.Service service => ItemClass.Service.None;
        public override string serviceName => LibraryConstants.AssemblyName;
        
        #endregion
        
        protected override bool IsServiceValid(PrefabInfo info)
        {
            return true;
        }
    }
}
