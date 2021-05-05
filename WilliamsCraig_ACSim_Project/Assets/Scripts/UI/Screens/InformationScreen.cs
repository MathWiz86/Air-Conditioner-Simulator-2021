/**************************************************************************************************/
/*!
\file   InformationScreen.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of the world information menu.

\par References:
*/
/**************************************************************************************************/

using ACSim.Extensions;
using ACSim.FuzzyLogic;
using ACSim.Systems;
using TMPro;
using UnityEngine;

namespace ACSim.UI.Screens
{
  /// <summary>
  /// The menu for displaying information about the game's world and the Takagi-Sugeno System.
  /// </summary>
  public class InformationScreen : PhoneScreen
  {
    /// <summary>
    /// A helper class for a display of a <see cref="TSTriangleMembershipFunction"/>.
    /// </summary>
    [System.Serializable]
    private class TSTriangleInformationDisplay
    {
      /// <summary>The text for the first value.</summary>
      [SerializeField] private TextMeshProUGUI tmpA = null;
      /// <summary>The text for the second value.</summary>
      [SerializeField] private TextMeshProUGUI tmpB = null;
      /// <summary>The text for the third value.</summary>
      [SerializeField] private TextMeshProUGUI tmpC = null;

      /// <summary>
      /// A function to update the text values displayed.
      /// </summary>
      /// <param name="membershipFunction">The membership function to display.</param>
      public void UpdateValues(TSTriangleMembershipFunction membershipFunction)
      {
        // Update teh values.
        tmpA.text = membershipFunction.a.ToString("F2");
        tmpB.text = membershipFunction.b.ToString("F2");
        tmpC.text = membershipFunction.c.ToString("F2");
      }
    }

    /// <summary>
    /// A helper enum for displaying the current state of the A/C. This is only ever used on this
    /// screen, but must be set from other locations.
    /// </summary>
    public enum ACState
    {
      /// <summary>The fan is off.</summary>
      Off,
      /// <summary>The fan is heating up.</summary>
      Heating,
      /// <summary>The fan is cooling down.</summary>
      Cooling,
    }

    /// <summary>The singleton signature for this system.</summary>
    private static InformationScreen _singleton;

    /// <summary>The display for the <see cref="WorldSystem.TargetWorldTemperature"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpTargetTemperature = null;
    /// <summary>The display for the <see cref="WorldSystem.CurrentWorldTemperature"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpWorldTemperature = null;
    /// <summary>The display for the minimum of the <see cref="WorldSystem.WorldTemperatureRange"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpRangeMin = null;
    /// <summary>The display for the maximum of the <see cref="WorldSystem.WorldTemperatureRange"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpRangeMax = null;
    /// <summary>The display for <see cref="WorldSystem.AllowTemperatureFluctuation"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpFluctuationOn = null;
    /// <summary>The display for the state of the <see cref="ACSystem"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpACState = null;
    /// <summary>The display for the result of the Takagi-Sugeno System in the <see cref="ACSystem"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpTSResult = null;
    /// <summary>The display for the first <see cref="TSTriangleMembershipFunction"/>.</summary>
    [SerializeField] private TSTriangleInformationDisplay displayA1 = null;
    /// <summary>The display for the second <see cref="TSTriangleMembershipFunction"/>.</summary>
    [SerializeField] private TSTriangleInformationDisplay displayA2 = null;
    /// <summary>The display for the third <see cref="TSTriangleMembershipFunction"/>.</summary>
    [SerializeField] private TSTriangleInformationDisplay displayA3 = null;
    /// <summary>The display for the fourth <see cref="TSTriangleMembershipFunction"/>.</summary>
    [SerializeField] private TSTriangleInformationDisplay displayA4 = null;
    /// <summary>The display for the fifth <see cref="TSTriangleMembershipFunction"/>.</summary>
    [SerializeField] private TSTriangleInformationDisplay displayA5 = null;

    protected override void Awake()
    {
      base.Awake();

      // Initialize the Singleton.
      if (!_singleton)
        _singleton = this;
      else
        Destroy(gameObject);
    }

    private void Start()
    {
      // Initialize to the beginning values.
      UpdateTargetTemperatureDisplay();
      UpdateWorldTemperatureDisplay();
      UpdateTemperatureRangeDisplay();
      UpdateFluctuationOnDisplay();
      UpdateAirConditionerStateDisplay(ACState.Off);
    }

    /// <summary>
    /// A function to update the displayed target temperature.
    /// </summary>
    public static void UpdateTargetTemperatureDisplay()
    {
      if (_singleton)
        _singleton.tmpTargetTemperature.text = WorldSystem.TargetWorldTemperature.ToString("F2") + "\u00B0F";
    }

    /// <summary>
    /// A function to update the displayed world temperature.
    /// </summary>
    public static void UpdateWorldTemperatureDisplay()
    {
      if (_singleton)
        _singleton.tmpWorldTemperature.text = WorldSystem.CurrentWorldTemperature.ToString("F2") + "\u00B0F";
    }

    /// <summary>
    /// A function to update the displayed world temperature range.
    /// </summary>
    public static void UpdateTemperatureRangeDisplay()
    {
      if (_singleton)
      {
        _singleton.tmpRangeMin.text = WorldSystem.WorldTemperatureRange.x.ToString() + "\u00B0F";
        _singleton.tmpRangeMax.text = WorldSystem.WorldTemperatureRange.y.ToString() + "\u00B0F";
      }
    }

    /// <summary>
    /// A function to update the display of fluctuation being on or not.
    /// </summary>
    public static void UpdateFluctuationOnDisplay()
    {
      if (_singleton)
        _singleton.tmpFluctuationOn.text = WorldSystem.AllowTemperatureFluctuation.ToString();
    }

    /// <summary>
    /// A function to update the displayed state of the A/C system.
    /// </summary>
    public static void UpdateAirConditionerStateDisplay(ACState state)
    {
      if (_singleton)
        _singleton.tmpACState.text = state.ToString();
    }

    /// <summary>
    /// A function to update the displayed output value of the Takagi-Sugeno System.
    /// </summary>
    public static void UpdateTSOutputValueDisplay(float value)
    {
      if (_singleton)
        _singleton.tmpTSResult.text = value.ToString("F3");
    }

    /// <summary>
    /// A function to update the displayed values in the Takagi-Sugeno System.
    /// </summary>
    public static void UpdateMembershipFunctionDisplays(params TSTriangleMembershipFunction[] antecedents)
    {
      if (!antecedents.IsEmptyOrNull() && antecedents.Length == 5 && _singleton)
      {
        _singleton.displayA1.UpdateValues(antecedents[0]);
        _singleton.displayA2.UpdateValues(antecedents[1]);
        _singleton.displayA3.UpdateValues(antecedents[2]);
        _singleton.displayA4.UpdateValues(antecedents[3]);
        _singleton.displayA5.UpdateValues(antecedents[4]);
      }  
    }
  }
}