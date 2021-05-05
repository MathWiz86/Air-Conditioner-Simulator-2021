/**************************************************************************************************/
/*!
\file   SettingsScreen.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of the world settings menu.

\par References:
*/
/**************************************************************************************************/

using ACSim.Systems;
using Min_Max_Slider;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ACSim.UI.Screens
{
  /// <summary>
  /// The menu for the world's settings. Various values used in the simulation can be changed here.
  /// </summary>
  public class SettingsScreen : PhoneScreen
  {
    /// <summary>The slider for the <see cref="WorldSystem.CurrentWorldTemperature"/>.</summary>
    [SerializeField] private Slider slWorldTemperature = null;
    /// <summary>The slider for the <see cref="WorldSystem.WorldTemperatureRange"/>.</summary>
    [SerializeField] private MinMaxSlider slTemperatureRange = null;
    /// <summary>The toggle for the <see cref="WorldSystem.AllowTemperatureFluctuation"/>.</summary>
    [SerializeField] private Toggle toAllowFluctuation = null;
    /// <summary>The slider for the <see cref="WorldSystem.FluctuationWaitRange"/>.</summary>
    [SerializeField] private MinMaxSlider slTimeBeforeFluctuation = null;
    /// <summary>The slider for the <see cref="WorldSystem.FluctuationLengthRange"/>.</summary>
    [SerializeField] private MinMaxSlider slFluctuationDuration = null;
    /// <summary>The slider for the <see cref="WorldSystem.FluctuationTemperatureRange"/>.</summary>
    [SerializeField] private MinMaxSlider slFluctuationRange = null;
    /// <summary>The slider for the <see cref="WorldSystem.CurrentAcceptanceRange"/>.</summary>
    [SerializeField] private MinMaxSlider slTemperatureMargin = null;
    /// <summary>The slider for the <see cref="ACSystem.TemperatureCheckTimer"/>.</summary>
    [SerializeField] private Slider slACCheckTime = null;

    /// <summary>The last value of the <see cref="toAllowFluctuation"/> before the screen opened.</summary>
    private bool lastFluctuationToggle = false;

    private void Start()
    {
      // Set up the limits with absolute values from each system.
      slTemperatureRange.SetLimits(WorldSystem.AbsoluteTempRange.x, WorldSystem.AbsoluteTempRange.y);
      slTimeBeforeFluctuation.SetLimits(WorldSystem.AbsoluteFluctuationWaitRange.x, WorldSystem.AbsoluteFluctuationWaitRange.y);
      slFluctuationDuration.SetLimits(WorldSystem.AbsoluteFluctuationLengthRange.x, WorldSystem.AbsoluteFluctuationLengthRange.y);
      slFluctuationRange.SetLimits(WorldSystem.AbsoluteFluctuationTemperatureRange.x, WorldSystem.AbsoluteFluctuationTemperatureRange.y);
      slTemperatureMargin.SetLimits(WorldSystem.AbsoluteAcceptanceRange.x, WorldSystem.AbsoluteAcceptanceRange.y);
      slACCheckTime.minValue = ACSystem.AbsoluteTemperatureCheckRange.x;
      slACCheckTime.maxValue = ACSystem.AbsoluteTemperatureCheckRange.y;
    }

    public override void InitializeScreen()
    {
      // Update the values to the current world.
      slTemperatureRange.SetValues(WorldSystem.WorldTemperatureRange.x, WorldSystem.WorldTemperatureRange.y);
      slWorldTemperature.value = WorldSystem.CurrentWorldTemperature;

      toAllowFluctuation.isOn = WorldSystem.AllowTemperatureFluctuation;
      lastFluctuationToggle = WorldSystem.AllowTemperatureFluctuation;
      WorldSystem.ToggleTemperatureFluctuation(false);

      slTimeBeforeFluctuation.SetValues(WorldSystem.FluctuationWaitRange.x, WorldSystem.FluctuationWaitRange.y);
      slFluctuationDuration.SetValues(WorldSystem.FluctuationLengthRange.x, WorldSystem.FluctuationLengthRange.y);
      slFluctuationRange.SetValues(WorldSystem.FluctuationTemperatureRange.x, WorldSystem.FluctuationTemperatureRange.y);

      slTemperatureMargin.SetValues(WorldSystem.CurrentAcceptanceRange.x, WorldSystem.CurrentAcceptanceRange.y);

      slACCheckTime.value = ACSystem.TemperatureCheckTimer;
    }

    public override IEnumerator HideScreen()
    {
      WorldSystem.ToggleTemperatureFluctuation(lastFluctuationToggle);
      yield return base.HideScreen();
    }

    /// <summary>
    /// A callback function when updating the <see cref="slTemperatureRange"/>. This affects what
    /// the value of the <see cref="WorldSystem.CurrentWorldTemperature"/> can be.
    /// </summary>
    public void UpdateWorldTemperatureSliderBounds()
    {
      slWorldTemperature.minValue = slTemperatureRange.Values.minValue;
      slWorldTemperature.maxValue = slTemperatureRange.Values.maxValue;
    }

    /// <summary>
    /// A function called to submit all changes to the systems again, once the user is done.
    /// </summary>
    public void SubmitChanges()
    {
      WorldSystem.WorldTemperatureRange = new Vector2(slTemperatureRange.Values.minValue, slTemperatureRange.Values.maxValue);
      WorldSystem.TargetWorldTemperature = WorldSystem.TargetWorldTemperature;
      WorldSystem.SetWorldTemperature(slWorldTemperature.value);
      WorldSystem.ToggleTemperatureFluctuation(toAllowFluctuation.isOn);
      lastFluctuationToggle = toAllowFluctuation.isOn;

      WorldSystem.FluctuationWaitRange = new Vector2(slTimeBeforeFluctuation.Values.minValue, slTimeBeforeFluctuation.Values.maxValue);
      WorldSystem.FluctuationLengthRange = new Vector2(slFluctuationDuration.Values.minValue, slFluctuationDuration.Values.maxValue);
      WorldSystem.FluctuationTemperatureRange = new Vector2(slFluctuationRange.Values.minValue, slFluctuationRange.Values.maxValue);

      WorldSystem.CurrentAcceptanceRange = new Vector2(slTemperatureMargin.Values.minValue, slTemperatureMargin.Values.maxValue);
      ACSystem.TemperatureCheckTimer = slACCheckTime.value;

      ACSystem.CheckTemperatures();
    }
  }
}