using ColossalFramework.UI;
using IndustryLP.Utils;

namespace IndustryLP.UI.Panels
{
    internal class UIGeneratorOptionPanel
    {
        public static void SetupInstance(UIScrollablePanel oldPanel)
        {
            var grid = oldPanel.parent.AddUIComponent<UIDistributionButton>();
            grid.Initialize("Grid");
            grid.transform.parent = oldPanel.transform;

            var line = oldPanel.parent.AddUIComponent<UIDistributionButton>();
            line.Initialize("Line");
            line.transform.parent = oldPanel.transform;
            line.Disable();

            var mine = oldPanel.parent.AddUIComponent<UIDistributionButton>();
            mine.Initialize("Mine");
            mine.transform.parent = oldPanel.transform;
            mine.Disable();

            grid.eventClicked += (c, p) =>
            {
                line.Pressed = false;
                mine.Pressed = false;
                if (!grid.Pressed)
                {
                    IndustryTool.instance.SetGridDistribution();
                }
            };

            line.eventClicked += (c, p) =>
            {
                grid.Pressed = false;
                mine.Pressed = false;
                if (!line.Pressed)
                {
                    IndustryTool.instance.SetLineDistribution();
                }
            };

            mine.eventClicked += (c, p) =>
            {
                grid.Pressed = false;
                line.Pressed = false;
                if (!mine.Pressed)
                {
                    IndustryTool.instance.SetMineDistribution();
                }
            };
        }
    }
}
