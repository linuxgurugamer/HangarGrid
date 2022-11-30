using System;
using UnityEngine;
using SpaceTuxUtility;

namespace HangarGrid
{
    /// <summary>
    /// Used to load and hold configurable properties
    /// </summary>
    public class Configuration
    {

        public class Data
        {
            internal const KeyCode defaultAlignUpAxis = KeyCode.J;
            internal const KeyCode defaultAlignForwardAxis = KeyCode.N;
            internal const KeyCode defaultAlignRightAxis = KeyCode.M;
            internal const KeyCode defaultAlignToGrid = KeyCode.L;
            internal const KeyCode defaultToggleSymmetryGuides = KeyCode.K;
            internal const KeyCode defaultBindGridToPart = KeyCode.G;
            internal const KeyCode defaultMasterToggle = KeyCode.Period;
            internal const int defaultGuideSelectionTolerance = 20;
            internal const float defaultStep = 10f;

            public KeyCode alignUpAxis;
            public KeyCode alignForwardAxis;
            public KeyCode alignRightAxis;
            public KeyCode alignToGrid;
            public KeyCode toggleSymmetryGuides;
            public KeyCode bindGridToPart;
            public KeyCode masterToggle = KeyCode.Period;
            public int guideSelectionTolerance = defaultGuideSelectionTolerance;
            public float step = defaultStep;

            public void ResetToDefault()
            {
                alignUpAxis = defaultAlignUpAxis;
                alignForwardAxis = defaultAlignForwardAxis;
                alignRightAxis = defaultAlignRightAxis;
                alignToGrid = defaultAlignToGrid;
                toggleSymmetryGuides = defaultToggleSymmetryGuides;
                bindGridToPart = defaultBindGridToPart;
                masterToggle = defaultMasterToggle;
                guideSelectionTolerance = defaultGuideSelectionTolerance;
                step = defaultStep;
            }

            public Data(Data d)
            {
                alignUpAxis = d.alignUpAxis;
                alignForwardAxis = d.alignForwardAxis;
                alignRightAxis = d.alignRightAxis;
                alignToGrid = d.alignToGrid;
                toggleSymmetryGuides = d.toggleSymmetryGuides;
                bindGridToPart = d.bindGridToPart;
                masterToggle = d.masterToggle;
                guideSelectionTolerance = d.guideSelectionTolerance;
                step = d.step;
            }
            public Data()
            { }
        }
        public enum Keys
        {
            none,
            alignUpAxis,
            alignForwardAxis,
            alignRightAxis,
            alignToGrid,
            toggleSymmetryGuides,
            bindGridToPart,
            masterToggle
        }

        internal Data data;
        private static Configuration instance = null;

        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }
                return instance;
            }
        }

        private Configuration()
        {
            data = new Data();
            LoadConfiguration("HangarGrid");
        }

        public string DATADIR { get { return KSPUtil.ApplicationRootPath + "GameData/HangarGrid/PluginData/"; } }
        public const string SETTINGS_FILE = "HangarGrid.cfg";
        public const string NODE = "HangarGrid";
        public const string HOTKEY_NODE = "HotKeys";


        private void LoadConfiguration(string root)
        {
            ConfigNode settingsFile = ConfigNode.Load(DATADIR + SETTINGS_FILE);
            if (settingsFile != null)
            {
                ConfigNode settings = null;
                if (settingsFile.TryGetNode(NODE, ref settings))
                {
                    if (settings != null)
                    {
                        data.guideSelectionTolerance = ConfigNodeUtils.SafeLoad(settings, "guideSelectionTolerance", Data.defaultGuideSelectionTolerance);
                        data.step = ConfigNodeUtils.SafeLoad(settings, "step", Data.defaultStep);
                        ConfigNode hotKeys = null;
                        if (settings.TryGetNode(HOTKEY_NODE, ref hotKeys))
                        {
                            data.alignUpAxis = ConfigNodeUtils.SafeLoad(hotKeys, "alignUpAxis", Data.defaultAlignUpAxis);
                            data.alignForwardAxis = ConfigNodeUtils.SafeLoad(hotKeys, "alignForwardAxis", Data.defaultAlignForwardAxis);
                            data.alignRightAxis = ConfigNodeUtils.SafeLoad(hotKeys, "alignRightAxis", Data.defaultAlignRightAxis);
                            data.alignToGrid = ConfigNodeUtils.SafeLoad(hotKeys, "alignToGrid", Data.defaultAlignToGrid);
                            data.toggleSymmetryGuides = ConfigNodeUtils.SafeLoad(hotKeys, "toggleSymmetryGuides", Data.defaultToggleSymmetryGuides);
                            data.bindGridToPart = ConfigNodeUtils.SafeLoad(hotKeys, "bindGridToPart", Data.defaultBindGridToPart);
                            data.masterToggle = ConfigNodeUtils.SafeLoad(hotKeys, "masterToggle", Data.defaultMasterToggle);
                        }
                    }
                }
            }
        }

        internal void SaveConfiguration()
        {
            ConfigNode settings = new ConfigNode(NODE);
            ConfigNode node = new ConfigNode(HOTKEY_NODE);
            node.AddValue("alignUpAxis", data.alignUpAxis);
            node.AddValue("alignForwardAxis", data.alignForwardAxis);
            node.AddValue("alignRightAxis", data.alignRightAxis);
            node.AddValue("alignToGrid", data.alignToGrid);
            node.AddValue("toggleSymmetryGuides", data.toggleSymmetryGuides);
            node.AddValue("bindGridToPart", data.bindGridToPart);
            node.AddValue("masterToggle", data.masterToggle);

            settings.AddNode(node);
            settings.AddValue("guideSelectionTolerance", data.guideSelectionTolerance);
            settings.AddValue("step", data.step);
            ConfigNode file = new ConfigNode();
            file.AddNode(settings);
            file.Save(DATADIR + SETTINGS_FILE);
        }
    }
}
