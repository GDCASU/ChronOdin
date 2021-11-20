using UnityEngine;
using UnityEditor;

/// <summary>
/// This script creates the UI in the inspector for
/// a 2D Boolean Array.
/// NOTE: this script needs to be in folder called Editor that's
/// in the Asset's folder for it to work.
/// Resources: https://youtu.be/uoHc-Lz9Lsc
/// https://docs.unity3d.com/Manual/editor-PropertyDrawers.html
/// Author: Alben Trang
/// </summary>
[CustomPropertyDrawer(typeof(BooleanArray2D))]
public class Array2DDrawer : PropertyDrawer
{
    int rows;
    int columns;
    float standardSpacing = 25.0f;
    float standardTextWidth = 100.0f;
    float standardTextHeight = 20.0f;
    float standardInputWidth = 60.0f;
    float standardInputHeight = 20.0f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);

        Rect newPos = position;
        newPos.y += standardSpacing; // Offset for elements to go under "Script" text

        SerializedProperty info = property.FindPropertyRelative("info");
        rows = info.FindPropertyRelative("rows").intValue;
        columns = info.FindPropertyRelative("columns").intValue;

        SerializedProperty booleanArrays = property.FindPropertyRelative("booleanArrays");

        // Text elements
        var textRect = new Rect(newPos.x, newPos.y, standardTextWidth, standardTextHeight);
        EditorGUI.LabelField(textRect, "Rows");
        textRect.y += standardSpacing;
        EditorGUI.LabelField(textRect, "Columns");
        textRect.y += standardSpacing;
        EditorGUI.LabelField(textRect, "2D Boolean Grid");

        // Rows and columns input elements
        var intRect = new Rect(newPos.x * 6, newPos.y, standardInputWidth, standardInputHeight); // Placement for row and column boxes
        EditorGUI.PropertyField(intRect, info.FindPropertyRelative("rows"), GUIContent.none);
        intRect.y += standardSpacing;
        EditorGUI.PropertyField(intRect, info.FindPropertyRelative("columns"), GUIContent.none);

        // Boolean Array 2D elements
        if (booleanArrays.arraySize != rows)
            booleanArrays.arraySize = rows;
        newPos.y = standardSpacing * 6;
        newPos.width = standardSpacing;
        newPos.height = standardSpacing;
        for (int i = 0; i < rows; i++) 
        {
            SerializedProperty boolArray = booleanArrays.GetArrayElementAtIndex(i).FindPropertyRelative("boolArray");
            if (boolArray.arraySize != columns)
                boolArray.arraySize = columns;

            for (int j = 0; j < columns; j++)
            {
                EditorGUI.PropertyField(newPos, boolArray.GetArrayElementAtIndex(j), GUIContent.none);
                newPos.x += newPos.width;
            }

            newPos.x = position.x;
            newPos.y += standardSpacing;
        }
    }

    /// <summary>
    /// Makes sure anything that's supposed to be under the elements doesn't overlap them
    /// under a certain number of pixels
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (standardSpacing * 5) + (standardSpacing * rows);
    }
}
