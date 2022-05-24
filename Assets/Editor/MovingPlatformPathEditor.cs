
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;

[CustomEditor(typeof(MovingPlatformPath))]
public class MovingPlatformPathEditor : Editor
{
    Tool lastTool = Tool.None;
    bool hidden = false;

    void OnEnable()
    {
        var scriptComponent = target as MovingPlatformPath;
        if (scriptComponent.points.Length > 0)
        {
            HideTransform(true);
        }
    }

    void OnDisable() => HideTransform(false);
    private void OnSceneGUI()
    {
        var scriptComponent = target as MovingPlatformPath;
        for (int i = 0; i < scriptComponent.points.Length; i++)
        {
            var transform = scriptComponent.transform;

            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                var newPosition = transform.InverseTransformPoint(

                Handles.PositionHandle(
                    transform.TransformPoint(scriptComponent.points[i]),
                    transform.rotation));

                if (cc.changed)
                {
                    Undo.RecordObject(scriptComponent, "Point Changed");
                    scriptComponent.points[i] = newPosition;
                }
            }
        }
        Handles.BeginGUI();
        var rectMin = Camera.current.WorldToScreenPoint(
            scriptComponent.transform.position);
        var rect = new Rect();
        rect.xMin = rectMin.x;
        rect.yMin = SceneView.currentDrawingSceneView.position.height -
            rectMin.y;
        rect.width = 148;
        rect.height = 18;
        GUILayout.BeginArea(rect);
        if (GUILayout.Button("Toggle Transform Gizmo")) HideTransform(!hidden);
        GUILayout.EndArea();
        Handles.EndGUI();

        if (scriptComponent.points == null)
        {
            return;
        }
        else
        {
            Vector3[] points = new Vector3[scriptComponent.points.Length + (scriptComponent.loop ? 1 : 0)];
            for (int i = 0; i < scriptComponent.points.Length; i++)
            {
                points[i] = scriptComponent.transform.position + scriptComponent.points[i];
            }
            if (scriptComponent.loop) points[points.Length - 1] = points[0];
            Handles.color = Color.red;
            Handles.DrawPolyLine(points);
        }

    }
    void HideTransform(bool hide)
    {
        if (hide)
        {
            lastTool = Tools.current;
            Tools.current = Tool.None;
            hidden = true;
        }
        else
        {
            hidden = false;
            Tools.current = lastTool;
        }

    }
}
