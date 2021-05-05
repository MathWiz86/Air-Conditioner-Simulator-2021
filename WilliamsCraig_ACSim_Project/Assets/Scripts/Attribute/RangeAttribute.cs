/**************************************************************************************************/
/*!
\file   RangeAttribute.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-02-28
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the RangeRefAttribute, which is used to create a range between two numbers
  stored in other variables.

\par References:
*/
/**************************************************************************************************/

using UnityEngine;
using UnityEditor;
using ACSim.Kits;
using System.Runtime.CompilerServices;

namespace ACSim.UnityEditor
{
  /// <summary>
  /// A custom attribute which allows for creating a range between two numbers via referencing
  /// other variables on an object.
  /// </summary>
  public class RangeRefAttribute : PropertyAttribute
  {
    /// <summary>The path to the variable with the minimum value.</summary>
    public string min;
    /// <summary>The path to the variable with the maximum. value.</summary>
    public string max;
    /// <summary>A bool determining if the passed-in paths are valid.</summary>
    public bool isValid { get; private set; }

    /// <summary>
    /// The constructor for the <see cref="RangeRefAttribute"/>.
    /// </summary>
    /// <param name="min">The path to the variable with the minimum value.</param>
    /// <param name="max">The path to the variable with the maximum. value.</param>
    public RangeRefAttribute(string min, string max)
    {
      this.min = string.IsNullOrWhiteSpace(min) ? string.Empty : min; // Set the minimum value.
      this.max = string.IsNullOrWhiteSpace(max) ? string.Empty : max; // Set the maximum value.

      // The attribute is considered valid if both the min and max start with the variable call character.
      isValid = min.StartsWith(Kits.Reflection.VariableCall) && max.StartsWith(Kits.Reflection.VariableCall);
    }
  }

#if UNITY_EDITOR
  /// <summary>
  /// The custom drawer for a <see cref="RangeRefAttribute"/>.
  /// </summary>
  [CustomPropertyDrawer(typeof(RangeRefAttribute))]
  public sealed class RangeDrawer : PropertyDrawer
  {
    /// <summary>The minimum value of the range.</summary>
    private float minValue = 0.0f;
    /// <summary>The maximum value of the range.</summary>
    private float maxValue = 100.0f;
    /// <summary>A bool determining if the values were found. Unused due to issues with EditorGUI's Help Box.</summary>
    private bool valuesFound = true;

    /// <summary>
    /// A version of the attribute GUI for float properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyFloat(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      EditorGUI.Slider(position, property, minValue, maxValue, label); // Draw the slider.
      EditorGUI.EndProperty(); // End the property.
    }

    /// <summary>
    /// A version of the attribute GUI for int properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyInt(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      EditorGUI.IntSlider(position, property, (int)minValue, (int)maxValue, label); // Draw the slider.
      EditorGUI.EndProperty(); // End the property.
    }

    /// <summary>
    /// A version of the attribute GUI for Vector2 properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyVector2(Rect position, SerializedProperty property, GUIContent label)
    {
      // Note: Unity EditorGUI is very buggy and boilerplate. There existed a bug that caused the label
      // to change text every time an EditorGUI function was called, even though label never got passed.
      // Please bear with the poorly created code in an attempt to make the MinMaxSlider actually have numbers.

      Rect fullPos = position;
      string text = label.text;
      string tooltip = label.tooltip;
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      Vector2 value = property.vector2Value; // Make a copy of the vector value.

      EditorGUI.indentLevel += 3;
      position.y += 20.0f;
      position.height = 20.0f;
      EditorGUI.PrefixLabel(position, new GUIContent("Min"));
      
      position.x += 30.0f;
      position.width = 100.0f;
      EditorGUI.BeginChangeCheck();
      value.x = EditorGUI.FloatField(position, value.x);

      position.x += 80.0f;
      position.width = fullPos.width;
      EditorGUI.PrefixLabel(position, new GUIContent("Max"));
      
      position.x += 30.0f;
      position.width = 100.0f;
      value.y = EditorGUI.FloatField(position, value.y);

      // Finish clamping values.
      if (EditorGUI.EndChangeCheck())
      {
        value.x = Mathf.Clamp(value.x, minValue, maxValue);
        value.y = Mathf.Clamp(value.y, minValue, maxValue);
        value.x = Mathf.Clamp(value.x, minValue, value.y);
        value.y = Mathf.Clamp(value.y, value.x, maxValue);
      }

      position = fullPos;
      EditorGUI.indentLevel -= 3;
      EditorGUI.MinMaxSlider(fullPos, new GUIContent(text, tooltip), ref value.x, ref value.y, minValue, maxValue); // Draw the slider.
      property.vector2Value = value; // Set the value.
      EditorGUI.EndProperty(); // End the property.
    }

    /// <summary>
    /// A version of the attribute GUI for Vector2Int properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyVector2Int(Rect position, SerializedProperty property, GUIContent label)
    {
      // Note: Unity EditorGUI is very buggy and boilerplate. There existed a bug that caused the label
      // to change text every time an EditorGUI function was called, even though label never got passed.
      // Please bear with the poorly created code in an attempt to make the MinMaxSlider actually have numbers.

      Rect fullPos = position;
      string text = label.text;
      string tooltip = label.tooltip;
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      Vector2 value = property.vector2IntValue; // Make a copy of the vector value.

      EditorGUI.indentLevel += 3;
      position.y += 20.0f;
      position.height = 20.0f;
      EditorGUI.PrefixLabel(position, new GUIContent("Min"));

      position.x += 30.0f;
      position.width = 100.0f;
      EditorGUI.BeginChangeCheck();
      value.x = EditorGUI.FloatField(position, value.x);

      position.x += 80.0f;
      position.width = fullPos.width;
      EditorGUI.PrefixLabel(position, new GUIContent("Max"));

      position.x += 30.0f;
      position.width = 100.0f;
      value.y = EditorGUI.FloatField(position, value.y);

      // Finish clamping values.
      if (EditorGUI.EndChangeCheck())
      {
        value.x = Mathf.Clamp((int)value.x, minValue, maxValue);
        value.y = Mathf.Clamp((int)value.y, minValue, maxValue);
        value.x = Mathf.Clamp((int)value.x, minValue, value.y);
        value.y = Mathf.Clamp((int)value.y, value.x, maxValue);
      }

      position = fullPos;
      EditorGUI.indentLevel -= 3;
      EditorGUI.MinMaxSlider(fullPos, new GUIContent(text, tooltip), ref value.x, ref value.y, minValue, maxValue); // Draw the slider.
      property.vector2IntValue = new Vector2Int((int)value.x, (int)value.y); // Set the value.
      EditorGUI.EndProperty(); // End the property.
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      RangeRefAttribute attribute = this.attribute as RangeRefAttribute;
      valuesFound = attribute.isValid;

      // If this attribute is valid, meaning it targets variables, check their dynamic field values.
      if (attribute.isValid)
      {
        valuesFound = valuesFound & Reflection.CheckDynamicField(property.serializedObject.targetObject, attribute.min, minValue, out minValue);
        valuesFound = valuesFound & Reflection.CheckDynamicField(property.serializedObject.targetObject, attribute.max, maxValue, out maxValue);
      }

      // Perform a different draw function depending on the property type.
      switch (property.propertyType)
      {
        case SerializedPropertyType.Integer:
          OnPropertyInt(position, property, label);
          break;
        case SerializedPropertyType.Float:
          OnPropertyFloat(position, property, label);
          break;
        case SerializedPropertyType.Vector2:
          OnPropertyVector2(position, property, label);
          break;
        case SerializedPropertyType.Vector2Int:
          OnPropertyVector2Int(position, property, label);
          break;
        default:
          EditorGUI.PropertyField(position, property, label);
          break;
      }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      if (property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector2Int)
        return base.GetPropertyHeight(property, label) + 25.0f;

      return base.GetPropertyHeight(property, label);
    }
  }
#endif
}