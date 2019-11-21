using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace SplinePower.Editors
{
    [CustomEditor(typeof(WaypointRider))]
    [CanEditMultipleObjects]
    class WaypointRiderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SplineFormerEditor.DrawHeaderTexture();
            base.OnInspectorGUI();
        }
    }
}
