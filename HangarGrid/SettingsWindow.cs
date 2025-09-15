using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ClickThroughFix;
using SpaceTuxUtility;
using static HangarGrid.RegisterToolbar;

namespace HangarGrid
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    internal class SettingsWindow : MonoBehaviour
    {
        internal static SettingsWindow Instance;

        bool helpMode = false;
        internal bool isVisible = false;
        int winId;
        ModuleDeployableAntenna
        Configuration.Data localData;
        void Start()
        {
            winId = SpaceTuxUtility.WindowHelper.NextWindowId("HangerGridSettings");
            Instance = this;
            localData = new Configuration.Data(Configuration.Instance.data);
        }

        internal void CaptureData()
        {
            localData = new Configuration.Data(Configuration.Instance.data);
        }

        Rect winPos = new Rect((Screen.width - 200) / 2, (Screen.height - 200) / 2, 200, 200);
        void OnGUI()
        {
            if (isVisible)
            {
                winPos = ClickThruBlocker.GUILayoutWindow(winId, winPos, Window, "Hangar Grid Settings");
            }
        }

        void DoButton(string text, Configuration.Keys key, KeyCode current)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(text))
            {
#if false
                if (helpMode)
                {
                    switch (currentKey)
                    {
                        case Configuration.Keys.masterToggle:
                            HangarGrid.Instance.ToggleMasterToggle();
                            break;

                        case Configuration.Keys.alignUpAxis:
                            HangarGrid.Instance.AlignUpAxis();
                            break;

                        case Configuration.Keys.alignForwardAxis:
                            HangarGrid.Instance.AlignForwardAxis();
                            break;

                        case Configuration.Keys.alignRightAxis:
                            HangarGrid.Instance.AlignRightAxis();
                            break;

                        case Configuration.Keys.alignToGrid:
                            HangarGrid.Instance.AlignToGrid();
                            break;

                        case Configuration.Keys.toggleSymmetryGuides:
                            HangarGrid.Instance.ToggleSymmetryMode();
                            break;

                        case Configuration.Keys.bindGridToPart:
                            HangarGrid.Instance.BindGridToPart();
                            break;
                    }
                    HangarGrid.Instance.FinalCheck();
                }
                else
#endif
                {
                    currentKey = key;
                }

            }
            GUILayout.FlexibleSpace();
            if (currentKey != key)
                GUILayout.Label(current.ToString());
            GUILayout.EndHorizontal();

        }

        Configuration.Keys currentKey = Configuration.Keys.none;
        void Window(int id)
        {
            //GUILayout.BeginHorizontal();
            DoButton("Master Toggle", Configuration.Keys.masterToggle, localData.masterToggle);
            DoButton("Align Up Axis", Configuration.Keys.alignUpAxis, localData.alignUpAxis);
            DoButton("Align Forward Axis", Configuration.Keys.alignForwardAxis, localData.alignForwardAxis);
            DoButton("Align Right Axis", Configuration.Keys.alignRightAxis, localData.alignRightAxis);
            DoButton("Align To Grid", Configuration.Keys.alignToGrid, localData.alignToGrid);
            DoButton("Toggle Symmetry Guides", Configuration.Keys.toggleSymmetryGuides, localData.toggleSymmetryGuides);
            DoButton("Bind Grid To Part", Configuration.Keys.bindGridToPart, localData.bindGridToPart);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Guide Selection Tolerance:");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", GUILayout.Width(25)))
                if (localData.guideSelectionTolerance > 1)
                    localData.guideSelectionTolerance--;
            GUILayout.Label(localData.guideSelectionTolerance.ToString());
            if (GUILayout.Button(">", GUILayout.Width(25)))
                localData.guideSelectionTolerance++;
            GUILayout.EndHorizontal();

            float oldstep = localData.step;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Step:");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<<", GUILayout.Width(25)))
                if (localData.step > 10f)
                    localData.step -= 10f;
            if (GUILayout.Button("<", GUILayout.Width(25)))
                if (localData.step > 1f)
                    localData.step -= 1f;
            GUILayout.Label((localData.step / 10).ToString());
            if (GUILayout.Button(">", GUILayout.Width(25)))
                localData.step += 1f;
            if (GUILayout.Button(">>", GUILayout.Width(25)))
                localData.step += 10f;

            if (GridManager.gridEnabled && oldstep != localData.step)
            {
                HangarGrid.gridManager.showGrid(EditorLogic.fetch.editorBounds, true, localData.step);
                HangarGrid.Instance.FinalCheck();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save & Close"))
            {
                Configuration.Instance.data = localData;
                Configuration.Instance.SaveConfiguration();

                isVisible = false;
            }
            if (GUILayout.Button("Cancel"))
            {
                if (GridManager.gridEnabled)
                {
                    HangarGrid.gridManager.showGrid(EditorLogic.fetch.editorBounds, true, Configuration.Instance.data.step);
                    HangarGrid.Instance.FinalCheck();
                }
                isVisible = false;
            }
            if (GUILayout.Button("Reset"))
            {
                localData.ResetToDefault();
                if (GridManager.gridEnabled)
                {
                    HangarGrid.gridManager.showGrid(EditorLogic.fetch.editorBounds, true, Configuration.Instance.data.step);
                    HangarGrid.Instance.FinalCheck();
                }
            }
            if (GUILayout.Button("Revert"))
            {
                localData = Configuration.Instance.data;
                if (GridManager.gridEnabled)
                {
                    HangarGrid.gridManager.showGrid(EditorLogic.fetch.editorBounds, true, Configuration.Instance.data.step);
                }
            }
            //if (GUILayout.Button(helpMode ? "Help on" : "Help off"))
            //    helpMode = !helpMode;

            GUILayout.EndHorizontal();
            if (currentKey != Configuration.Keys.none)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("<Press any key>");
                GUILayout.FlexibleSpace();
                if (Event.current.isKey)
                {
                    switch (currentKey)
                    {
                        case Configuration.Keys.masterToggle:
                            localData.masterToggle = Event.current.keyCode;
                            break;
                        case Configuration.Keys.alignUpAxis:
                            localData.alignUpAxis = Event.current.keyCode;
                            break;

                        case Configuration.Keys.alignForwardAxis:
                            localData.alignForwardAxis = Event.current.keyCode;
                            break;

                        case Configuration.Keys.alignRightAxis:
                            localData.alignRightAxis = Event.current.keyCode;
                            break;

                        case Configuration.Keys.alignToGrid:
                            localData.alignToGrid = Event.current.keyCode;
                            break;

                        case Configuration.Keys.toggleSymmetryGuides:
                            localData.toggleSymmetryGuides = Event.current.keyCode;
                            break;

                        case Configuration.Keys.bindGridToPart:
                            localData.bindGridToPart = Event.current.keyCode;
                            break;
                    }
                    currentKey = Configuration.Keys.none;
                }

                GUILayout.EndHorizontal();

            }
            GUI.DragWindow();
        }
    }
}
