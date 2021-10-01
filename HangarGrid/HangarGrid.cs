using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;
using ToolbarControl_NS;

using static HangarGrid.RegisterToolbar;

namespace HangarGrid
{
    /// <summary>
    /// A grid plugin for Kerbal Space Program's SPH and VAB 
    /// 
    /// Copyright (C) 2016 Ser
    ///This program is free software; you can redistribute it and/or
    ///modify it under the terms of the GNU General Public License
    ///as published by the Free Software Foundation; either version 2
    ///of the License, or (at your option) any later version.
    ///
    ///This program is distributed in the hope that it will be useful,
    ///but WITHOUT ANY WARRANTY; without even the implied warranty of
    ///MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ///GNU General Public License for more details.
    /// 
    ///You should have received a copy of the GNU General Public License
    ///along with this program.  If not, see <http://www.gnu.org/licenses/>.
    /// </summary>

    //TODO Put the shader being used in the mod's assets

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class HangarGrid : MonoBehaviour
    {
        internal static HangarGrid Instance;
        private bool modEnabled = false;
        private bool symmetryMode = true;
        private Part gridOriginPart = null;
        //private ApplicationLauncherButton launcherButton;
        static ToolbarControl toolbarControl;
        private Configuration conf;

        private Part prevSelectedPart = null;
        //private Vector3 prevPosition = Vector3.zero;

        internal static GridManager gridManager = new GridManager();
        DirectionGuidesManager guidesManager = new DirectionGuidesManager();

        public void Awake()
        {
            conf = Configuration.Instance;
            addMenuButton();
            Instance = this;
        }

        internal const string MODID = "HangarGrid";
        internal const string MODNAME = "Hangar Grid";

        private void addMenuButton()
        {
            if (toolbarControl != null)
            {
                return;
            }
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(null, null,
                    KSP.UI.Screens.ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB,
                    MODID,
                    "HangerGridButton",
                    "HangarGrid/textures/toolbarbutton",
                    "HangarGrid/textures/toolbarbutton",
                    MODNAME
                );
            toolbarControl.AddLeftRightClickCallbacks(OnButtonToggle, ToggleSettingsWindow);

        }

        private void OnButtonToggle()
        {
            if (modEnabled)
            {
                gridManager.hideGrid();
                guidesManager.hideGuides();
                modEnabled = false;
            }
            else
            {
                gridManager.showGrid(EditorLogic.fetch.editorBounds);
                guidesManager.showGuides();
                modEnabled = true;
            }
        }

        internal void ToggleSettingsWindow()
        {
            SettingsWindow.Instance.isVisible = !SettingsWindow.Instance.isVisible;
            if (SettingsWindow.Instance.isVisible)
                SettingsWindow.Instance.CaptureData();
        }


#if false
        private void onButtonTrue()
        {
            gridManager.showGrid(EditorLogic.fetch.editorBounds);
            guidesManager.showGuides();
            modEnabled = true;
        }

        private void onButtonFalse()
        {
            gridManager.hideGrid();
            guidesManager.hideGuides();
            modEnabled = false;
        }

        public void OnDestroy()
        {
        }
#endif
        /*public void FixedUpdate() {
			if ((prevSelectedPart != null) && (prevSelectedPart == EditorLogic.SelectedPart) && Input.GetKey(KeyCode.P)) {
				Vector3 translation = Vector3.Project(EditorLogic.SelectedPart.transform.position - prevPosition, gridOriginPart.transform.up);
				EditorLogic.SelectedPart.transform.Translate(translation, Space.World);
			}
			prevSelectedPart = EditorLogic.SelectedPart;
			if (prevSelectedPart != null) {
				prevPosition = prevSelectedPart.transform.position;
			}
		}*/

        internal void ToggleSymmetryMode()
        {
            symmetryMode = !symmetryMode;
            guidesManager.setSymmetryMode(symmetryMode);
        }
        internal void ToggleMasterToggle()
        {
            if (!modEnabled)
            {
                //onButtonTrue();
                toolbarControl.SetTrue(true);
            }
            else
            {
                //onButtonFalse();
                toolbarControl.SetFalse(true);
            }
        }
        internal void AlignUpAxis()
        {
            alignSelectedPartToGrid(Vector3.up);
            UpdateEditorGizmo();
        }
        internal void AlignForwardAxis()
        {
            alignSelectedPartToGrid(Vector3.forward);
        }
        internal void AlignRightAxis()
        {
            alignSelectedPartToGrid(Vector3.right);
        }
        internal void AlignToGrid()
        {
            Part part;
            Vector3 localDirection;
            guidesManager.findClosestDirectionOnScreen(Input.mousePosition, conf.data.guideSelectionTolerance, out part, out localDirection);
            if (part != null)
            {
                alignPartToGrid(part, localDirection);
            }
        }
        internal void BindGridToPart()
        {
            gridOriginPart = GetPartUnderCursor();
        }

        internal void FinalCheck()
        {
            if (gridOriginPart == null)
            {
                gridOriginPart = EditorLogic.RootPart;
            }
            if (gridOriginPart != null)
            {
                if (modEnabled)
                {
                    gridManager.showGrid(EditorLogic.fetch.editorBounds);
                }
                gridManager.updateGrid(gridOriginPart);
            }
            else
            {
                gridManager.hideGrid();
            }

            guidesManager.updateGuides(prevSelectedPart, Color.red, 5f);
        }
        public void Update()
        {
            //EditorLogic editor = EditorLogic.fetch;
            //Bounds bounds = editor.editorBounds;
            if (EditorLogic.SelectedPart != null)
            {
                prevSelectedPart = EditorLogic.SelectedPart;
            }
            else return;

            if (Input.GetKeyDown(conf.data.masterToggle))
            {
                //Debug.Log("[HANGAR GRID] " + GUI.GetNameOfFocusedControl());
                ToggleMasterToggle();
            }

            if (Input.GetKeyDown(conf.data.toggleSymmetryGuides))
            {
                ToggleSymmetryMode();
            }

            if (Input.GetKeyDown(conf.data.alignUpAxis))
            {
                AlignUpAxis();
            }

            if (Input.GetKeyDown(conf.data.alignForwardAxis))
            {
                AlignForwardAxis();
            }

            if (Input.GetKeyDown(conf.data.alignRightAxis))
            {
                AlignRightAxis();
            }

            if (Input.GetKeyDown(conf.data.alignToGrid))
            {
                AlignToGrid();
            }

            if (Input.GetKeyDown(conf.data.bindGridToPart))
            {
                BindGridToPart();
            }

            FinalCheck();


        }

        private void UpdateEditorGizmo()
        {
            Vector3 position = EditorLogic.SelectedPart.transform.position;
            Quaternion rotation = EditorLogic.SelectedPart.transform.rotation;

            var gizmoOffset = HighLogic.FindObjectOfType<EditorGizmos.GizmoOffset>();
            if (gizmoOffset != null)
            {
                gizmoOffset.transform.position = position;
                if (gizmoOffset.CoordSpace == Space.Self)
                {
                    gizmoOffset.transform.rotation = rotation;
                }
            }

            var gizmoRotate = HighLogic.FindObjectOfType<EditorGizmos.GizmoRotate>();
            if (gizmoRotate != null)
            {
                gizmoRotate.transform.position = position;
                if (gizmoRotate.CoordSpace == Space.Self)
                {
                    gizmoRotate.transform.rotation = rotation;
                }
            }
        }

        private void alignSelectedPartToGrid(Vector3 localDirection)
        {
            Part part = EditorLogic.SelectedPart;
            if (part == null)
            {
                part = GetPartUnderCursor();
            }
            if (part == null)
            {
                return;
            }
            alignPartToGrid(part, localDirection);
        }

        private void alignPartToGrid(Part part, Vector3 localDirection)
        {
            if (gridOriginPart == null)
            {
                return;
            }
            SymmetryMethod symMethod = part.symMethod;
            Vector3 originalDirection = part.transform.TransformDirection(localDirection);
            Vector3 targetAxis = Utils.closestAxis(originalDirection, gridOriginPart.transform);
            Vector3 rotationAxis = Vector3.Cross(targetAxis, originalDirection);
            float angle = Utils.directionIndependentAngle(Utils.SignedAngleBetween(originalDirection, targetAxis, rotationAxis));
            part.transform.Rotate(rotationAxis, angle, Space.World);
            foreach (Part symPart in part.symmetryCounterparts)
            {
                Vector3 symmetryRotationAxis;
                if (symMethod == SymmetryMethod.Mirror)
                {
                    //In the Mirror symmetry mode the rotation axis and angle should be mirrored in relation the the plane that is orthogonal to the line between parts
                    symmetryRotationAxis = Vector3.ProjectOnPlane(rotationAxis, symPart.transform.position - part.transform.position) * 2 - rotationAxis;
                }
                else
                {
                    //In the Radial symmetry mode all the rotation axis' should be the same in the local space of every part
                    Vector3 rotationAxisLocal = part.transform.InverseTransformDirection(rotationAxis);
                    symmetryRotationAxis = symPart.transform.TransformDirection(rotationAxisLocal);
                }
                symPart.transform.Rotate(symmetryRotationAxis, (symMethod == SymmetryMethod.Radial ? 1 : -1) * angle, Space.World);
            }
        }

        //Thanks MachXXV
        public static Part GetPartUnderCursor()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                return new List<Part>(EditorLogic.FindObjectsOfType<Part>()).Find(p => p.gameObject == hit.transform.gameObject);
            }
            return null;
        }

        /*void OnPostRender() {
			if (part != null) {
				lineMat = new Material (Shader.Find("Particles/Additive"));
				lineMat.SetColor("color", Color.green);
				GL.Begin(GL.LINES);
				for (int i=-10; i<10; i++) {
					lineMat.SetPass(0);
					Vector3 lineStart = part.transform.TransformPoint(part.transform.right * i) - part.transform.up * 10;
					Vector3 lineEnd = part.transform.TransformPoint(part.transform.right * i) + part.transform.up * 10;
					GL.Color(Color.green);
					GL.Vertex3(lineStart.x, lineStart.y, lineStart.z);
					GL.Vertex3(lineEnd.x, lineEnd.y, lineEnd.z);
					}
				GL.End();
			}
		}*/

    }
}