using ColossalFramework.UI;

namespace IndustryLP.UI.Panels.Items
{
    internal interface IItemData
    {
        string Name { get; set; }
        string Tooltip { get; set; }
        UIScrollablePanel Panel { get; set; }
    }
}
