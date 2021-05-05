/**************************************************************************************************/
/*!
\file   ReadOnlyAttribute.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-02
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the ReadOnlyAttribute, which is used to force a value to be uneditable
  in the Unity Editor.

\par References:
*/
/**************************************************************************************************/

using UnityEngine;
using UnityEditor;

namespace ACSim.UnityEditor
{
  /// <summary>
  /// An attribute for making a value uneditable when in the Unity Editor.
  /// </summary>
  public class ReadOnlyAttribute : PropertyAttribute
  {
    /// <summary>A toggle for allowing the variable to be edited when not playing the game.</summary>
    public bool isOnlyDuringGameplay = false;

    /// <summary>
    /// A constructor for a <see cref="ReadOnlyAttribute"/>.
    /// </summary>
    /// <param name="isOnlyDuringGameplay">A toggle for allowing the variable to be edited when not playing the game.</param>
    public ReadOnlyAttribute(bool isOnlyDuringGameplay)
    {
      this.isOnlyDuringGameplay = isOnlyDuringGameplay;
    }
  }

#if UNITY_EDITOR
  /// <summary>
  /// The custom drawer for a <see cref="ReadOnlyAttribute"/>.
  /// </summary>
  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
  public class ReadOnlyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      bool currentEnabled = GUI.enabled;
      bool isOnlyDuringGameplay = (this.attribute as ReadOnlyAttribute).isOnlyDuringGameplay;
      
      // The GUI should either be disabled, or only disabled if playing the game.
      if (isOnlyDuringGameplay)
        GUI.enabled = !(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused);
      else
        GUI.enabled = false;

      // Draw the property, and reset the GUI.
      EditorGUI.PropertyField(position, property, label, true);
      GUI.enabled = currentEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }
  }
#endif
}