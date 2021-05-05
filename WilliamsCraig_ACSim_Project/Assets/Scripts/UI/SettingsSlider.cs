/**************************************************************************************************/
/*!
\file   SettingsSlider.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-02
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of a UI slider that shows its own value.

\par References:
*/
/**************************************************************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ACSim.UI
{
  /// <summary>
  /// A partner component to a <see cref="Slider"/> to display its own value in text.
  /// </summary>
  [RequireComponent(typeof(Slider))]
  public class SettingsSlider : MonoBehaviour
  {
    /// <summary>The slider attached to this object.</summary>
    public Slider slider { get; private set; }

    /// <summary>The text to use to display the value.</summary>
    [SerializeField] private TextMeshProUGUI tmpValueDisplay = null;
    /// <summary>The formatting of the text.</summary>
    [SerializeField] private string valueFormat = "F0";

    private void Awake()
    {
      slider = GetComponent<Slider>(); // Get the slider.
      slider.onValueChanged.AddListener(OnValueChanged); // add to the event.
    }

    /// <summary>
    /// A function to update the text display when the <see cref="slider"/>'s value is changed.
    /// </summary>
    /// <param name="value">The new value, passed in by the <see cref="slider"/>.</param>
    private void OnValueChanged(float value)
    {
      // Update the display.
      tmpValueDisplay.text = value.ToString(valueFormat);
    }
  }
}