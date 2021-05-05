/**************************************************************************************************/
/*!
\file   ValueViewerAttribute.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-01
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the ValueViewerAttribute, which is used to view the current value of any
  named field or property variable. This can be extremely useful for debugging.

\par References:
*/
/**************************************************************************************************/

using ACSim.Kits;
using UnityEditor;
using UnityEngine;

namespace ACSim.UnityEditor
{
  /// <summary>
  /// An attribute used to view the current value of a named field or property variable.
  /// This should only be used on variables under a '\#if UNITY_EDITOR' directive.
  /// Note that structs and classes that are not <see cref="UnityEngine.Object"/>s cannot
  /// be viewed due to EditorGUI limitations.
  /// </summary>
  public class ValueViewerAttribute : PropertyAttribute
  {
    /// <summary>The name of the variable to watch.</summary>
    public string valueName;

    /// <summary>
    /// The constructor for the attribute.
    /// </summary>
    /// <param name="valueName">The name of the variable to watch.</param>
    public ValueViewerAttribute(string valueName)
    {
      this.valueName = valueName;
    }
  }

#if UNITY_EDITOR
  /// <summary>
  /// The custom drawer for a <see cref="ValueViewerAttribute"/>.
  /// </summary>
  [CustomPropertyDrawer(typeof(ValueViewerAttribute))]
  public sealed class ValueViewerDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      ValueViewerAttribute attribute = this.attribute as ValueViewerAttribute;

      // Attempt to find the value wanted.
      bool found = Reflection.CheckDynamicField(property.serializedObject.targetObject, attribute.valueName, null, out object value);

      // If found, we have to switch on the type to display certain fields.
      if (found)
      {
        bool isEnabled = GUI.enabled;
        GUI.enabled = false; // Make the variable read only.
        GUIContent newLabel = new GUIContent(attribute.valueName.Substring(1)); // Make a new label based on the name.
        EditorGUI.BeginProperty(position, label, property); // Start the property.

        // Display a different field depending on the variable's type.
        switch (value)
        {
          case int i:
            EditorGUI.IntField(position, newLabel, i);
            break;
          case double d:
            EditorGUI.DoubleField(position, newLabel, d);
            break;
          case float f:
            EditorGUI.FloatField(position, newLabel, f);
            break;
          case long l:
            EditorGUI.LongField(position, newLabel, l);
            break;
          case string s:
            EditorGUI.TextField(position, newLabel, s);
            break;
          case bool b:
            EditorGUI.Toggle(position, newLabel, b);
            break;
          case Vector2 v2:
            EditorGUI.Vector2Field(position, newLabel, v2);
            break;
          case Vector2Int v2i:
            EditorGUI.Vector2IntField(position, newLabel, v2i);
            break;
          case Vector3 v3:
            EditorGUI.Vector3Field(position, newLabel, v3);
            break;
          case Vector3Int v3i:
            EditorGUI.Vector3IntField(position, newLabel, v3i);
            break;
          case Vector4 v4:
            EditorGUI.Vector4Field(position, newLabel, v4);
            break;
          case UnityEngine.Object o:
            EditorGUI.ObjectField(position, label, o, o.GetType(), true);
            break;
          default:
            // If the value cannot be displayed, simply put a default label.
            EditorGUI.LabelField(position, new GUIContent("VALUE " + newLabel.text + " CANNOT BE DISPLAYED"));
            break;
        }
        EditorGUI.EndProperty();
        GUI.enabled = isEnabled;
      }
      else
      {
        EditorGUI.BeginProperty(position, label, property); // Start the property.
        // If the value cannot be displayed, simply put a default label.
        EditorGUI.LabelField(position, new GUIContent("VALUE " + attribute.valueName.Substring(1) + " NOT FOUND"));
        EditorGUI.EndProperty();
      }
    }
  }
#endif
}