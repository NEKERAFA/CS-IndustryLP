using ColossalFramework.UI;
using IndustryLP.UI.Buttons;
using IndustryLP.Utils;
using IndustryLP.Utils.Constants;
using UnityEngine;

namespace IndustryLP.UI
{
    /// <summary>
    /// Defines the panel with the generator information
    /// </summary>
    internal class GeneratorPanel : UIPanel
    {
        #region Attributes

        private UILabel m_solutionLbl = null;
        private UILabel m_solutionsLbl = null;
        private UISprite m_spinner = null;
        private UpArrowButton m_upButton = null;
        private DownArrowButton m_downButton = null;
        private BuildSolutionButton m_buildSolutionButton = null;
        private bool m_isLoading = true;

        #endregion

        #region Properties

        public static string ObjectName => LibraryConstants.UIPrefix + "_GeneratorTool";

        public int Solutions { get; private set; }

        public int Solution { get; private set; }

        public ToolButton.OnButtonClickedDelegate OnUpButtonClick { get; set; }

        public ToolButton.OnButtonClickedDelegate OnDownButtonClick { get; set; }

        #endregion

        #region Unity Behaviour

        public override void Start()
        {
            base.Start();

            // Sets the panel
            name = ObjectName;
            backgroundSprite = "SubcategoriesPanel";
            size = new Vector2(200, 32);

            // Sets the widgets
            m_solutionLbl = GUIUtils.CreateLabel(this, "Solution");
            m_solutionLbl.relativePosition = new Vector2(5, 16f - (m_solutionLbl.height / 2f));
            m_solutionsLbl = GUIUtils.CreateLabel(this, "0/0");
            m_solutionsLbl.relativePosition = new Vector2(166 - m_solutionsLbl.width, 16f - (m_solutionLbl.height / 2f));
            m_spinner = SetupSpinner();
            m_upButton = SetupUpButton();
            m_upButton.Disable();
            m_upButton.OnButtonClicked = OnUpClick;
            m_downButton = SetupDownButton();
            m_downButton.OnButtonClicked = OnDownClick;
            m_downButton.Disable();
            m_buildSolutionButton = SetupBuildSolutionButton();
            m_buildSolutionButton.Disable();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (m_solutionLbl != null)
            {
                Destroy(m_solutionLbl.gameObject);
                m_solutionLbl = null;
            }

            if (m_solutionsLbl != null)
            {
                Destroy(m_solutionsLbl.gameObject);
                m_solutionsLbl = null;
            }

            if (m_spinner != null)
            {
                Destroy(m_spinner.gameObject);
                m_spinner = null;
            }

            if (m_upButton != null)
            {
                Destroy(m_upButton.gameObject);
                m_upButton = null;
            }

            if (m_downButton != null)
            {
                Destroy(m_downButton.gameObject);
                m_downButton = null;
            }
        }

        public override void Update()
        {
            if (m_isLoading)
            {
                float rotation = Time.deltaTime * 360f / 2f;
                m_spinner.transform.Rotate(-m_spinner.transform.forward, rotation);
            }
        }

        #endregion

        #region Panel Behavior

        #region Panel widget setup

        private UISprite SetupSpinner()
        {
            var spinner = AddUIComponent<UISprite>();
            spinner.transform.parent = transform;
            spinner.transform.localPosition = Vector2.zero;
            spinner.size = new Vector2(24, 24);
            spinner.relativePosition = new Vector2(171, 4);
            spinner.atlas = ResourceLoader.GetAtlas("InGame");
            spinner.spriteName = "loading_icon";
            spinner.pivot = UIPivotPoint.MiddleCenter;
            return spinner;
        }

        public UpArrowButton SetupUpButton()
        {
            var upArrowButton = AddUIComponent<UpArrowButton>();
            upArrowButton.transform.parent = transform;
            upArrowButton.transform.localPosition = Vector2.zero;
            upArrowButton.relativePosition = new Vector2(208, 0);
            return upArrowButton;
        }

        public DownArrowButton SetupDownButton()
        {
            var downArrowButton = AddUIComponent<DownArrowButton>();
            downArrowButton.transform.parent = transform;
            downArrowButton.transform.localPosition = Vector2.zero;
            downArrowButton.relativePosition = new Vector2(248, 0);
            return downArrowButton;
        }

        public BuildSolutionButton SetupBuildSolutionButton()
        {
            var buildSolutionButton = AddUIComponent<BuildSolutionButton>();
            buildSolutionButton.transform.parent = transform;
            buildSolutionButton.transform.localPosition = Vector2.zero;
            buildSolutionButton.relativePosition = new Vector2(288, 0);
            return buildSolutionButton;
        }

        #endregion

        #region Update state

        private void UpdateLabel()
        {
            m_solutionsLbl.text = $"{Solution}/{Solutions}";
            var offset = m_isLoading ? 29f : 0f;
            m_solutionsLbl.relativePosition = new Vector2(195 - offset - m_solutionsLbl.width, 16f - (m_solutionLbl.height / 2f));
        }

        private void SetSolution(int currentSolution)
        {
            Solution = currentSolution;
            UpdateLabel();
        }

        /// <summary>
        /// Set the number of total solutions
        /// </summary>
        /// <param name="solutions">The number of total solutions</param>
        public void SetSolutions(int solutions)
        {
            if (Solutions == 0 && solutions > 0)
            {
                m_upButton.Enable();
                m_buildSolutionButton.Enable();
                Solution = 1;
            }

            Solutions = solutions;

            UpdateLabel();
        }

        /// <summary>
        /// Hide the loading spinner
        /// </summary>
        public void StopLoading()
        {
            m_isLoading = false;
            m_spinner.Hide();
            UpdateLabel();
        }

        private void OnUpClick(bool isChecked)
        {
            if (Solution < Solutions)
            {
                if (Solution == 1)
                {
                    m_downButton.Enable();
                }

                OnUpButtonClick?.Invoke(isChecked);
                SetSolution(Solution + 1);

                if (Solution == Solutions)
                {
                    m_upButton.Disable();
                }
            }
        }

        private void OnDownClick(bool isChecked)
        {
            if (Solution > 1)
            {
                if (Solution == Solutions)
                {
                    m_upButton.Enable();
                }

                OnDownButtonClick?.Invoke(isChecked);
                SetSolution(Solution - 1);

                if (Solution == 1)
                {
                    m_downButton.Disable();
                }
            }
        }

        #endregion

        #endregion
    }
}
