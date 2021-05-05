/**************************************************************************************************/
/*!
\file   WorldTemperatureDisplay.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-02
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the code for displaying the current world temperature on the A/C unit.

\par References:
*/
/**************************************************************************************************/

using ACSim.Systems;
using TMPro;
using UnityEngine;

namespace ACSim.UI
{
  /// <summary>
  /// The display on the A/C unit for the current world temperature.
  /// </summary>
  public class WorldTemperatureDisplay : MonoBehaviour
  {
    /// <summary>The Unicode symbol for a degree.</summary>
    private static readonly string DegreeSymbol = "\u00B0";
    /// <summary>The text ui for the world temperature display.</summary>
    [SerializeField] private TextMeshProUGUI tmpTemperature = null;
    /// <summary>A toggle for showing a degree symbol or hiding it.</summary>
    [SerializeField] private bool showDegreeSymbol = true;

    private void Update()
    {
      // Update the world temperature display every frame.
      tmpTemperature.text = ((int)WorldSystem.CurrentWorldTemperature).ToString();
      
      if (showDegreeSymbol)
        tmpTemperature.text += DegreeSymbol;
    }
  }
}