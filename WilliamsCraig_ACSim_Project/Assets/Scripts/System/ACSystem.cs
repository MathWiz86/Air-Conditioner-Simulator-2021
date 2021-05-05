/**************************************************************************************************/
/*!
\file   ACSystem.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-02
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the system singleton for the air conditioner itself. This handles the fan,
  checking if the temperature has to change, and similar mechanics.

\par References:
*/
/**************************************************************************************************/

using ACSim.FuzzyLogic;
using ACSim.Kits;
using ACSim.Objects;
using ACSim.Systems;
using ACSim.UI.Screens;
using ACSim.UnityEditor;
using System.Collections;
using UnityEngine;

/// <summary>
/// The singleton system for the A/C unit. This primarily is for handling changing the temperature
/// based on fuzzy logic.
/// </summary>
public class ACSystem : MonoBehaviour
{
  /// <summary>The singleton signature for this system.</summary>
  private static ACSystem _singleton = null;
  /// <summary>The absolute range of time allowed between each temperature check.</summary>
  public static readonly Vector2 AbsoluteTemperatureCheckRange = new Vector2(0.0f, 15.0f);

  /// <summary>The amount of time waited between every check of the world temperature vs the target temperature.</summary>
  public static float TemperatureCheckTimer { get { return _singleton._TemperatureCheckTimer; } set { _singleton._TemperatureCheckTimer = Mathf.Clamp(value, AbsoluteTemperatureCheckRange.x, AbsoluteTemperatureCheckRange.y); } }
  /// <summary>The internal value for <see cref="TemperatureCheckTimer"/>.</summary>
  [SerializeField] [RangeRef("!AbsoluteTemperatureCheckRange.x", "!AbsoluteTemperatureCheckRange.y")]
  private float _TemperatureCheckTimer = 10.0f;

  /// <summary>The antecedent for 'Much Lower'.</summary>
  private readonly TSTriangleMembershipFunction antecedent1 = new TSTriangleMembershipFunction((f) => -6.0f * f + 1);
  /// <summary>The antecedent for 'Lower'.</summary>
  private readonly TSTriangleMembershipFunction antecedent2 = new TSTriangleMembershipFunction((f) => -2.5f * f);
  /// <summary>The antecedent for 'Acceptable'.</summary>
  private readonly TSTriangleMembershipFunction antecedent3 = new TSTriangleMembershipFunction((f) => 0.0f);
  /// <summary>The antecedent for 'Higher'.</summary>
  private readonly TSTriangleMembershipFunction antecedent4 = new TSTriangleMembershipFunction((f) => 2.5f * f);
  /// <summary>The antecedent for 'Much Higher.</summary>
  private readonly TSTriangleMembershipFunction antecedent5 = new TSTriangleMembershipFunction((f) => 6.0f * f + 1);

  /// <summary>The fan object that will spin in tune with the temperature speed.</summary>
  [SerializeField] private ACFan acFan = null;
  /// <summary>The change in temperature the world will have per second while the fan is running.</summary>
  private float temperatureChangeSpeed = 0.0f;
  /// <summary>The <see cref="Coroutine"/> for awaiting the temperature check.</summary>
  private Coroutine coAwaitTemperatureCheck = null;
  /// <summary>The <see cref="Coroutine"/> for changing the temperature.</summary>
  private Coroutine coChangeTemperature = null;

  private void Awake()
  {
    // Initialize the Singleton.
    if (!_singleton)
      _singleton = this;
    else
      Destroy(gameObject);
  }

  /// <summary>
  /// A function to begin the simulation of the system. This should only be called once.
  /// </summary>
  public static void BeginSimulation()
  {
    if (_singleton)
      _singleton.coAwaitTemperatureCheck = _singleton.StartCoroutine(_singleton.AwaitTemperatureCheck());
  }

  /// <summary>
  /// A function to force the temperatures to be rechecked. Typically, this is called when
  /// world values are updated.
  /// </summary>
  public static void CheckTemperatures()
  {
    if (_singleton)
      _singleton.CheckTemperaturesInternal();
  }

  /// <summary>
  /// The function that handles checking if the A/C unit needs to be turned on and change
  /// the world temperature.
  /// </summary>
  private void CheckTemperaturesInternal()
  {
    UpdateAntecedents(); // Update the antecedent values.

    // Get the difference in temperature between the target and current.
    float tempDifference = WorldSystem.TargetWorldTemperature - WorldSystem.CurrentWorldTemperature;
    // Get the amount of change in the speed.
    float tempChange = FuzzyKit.CalculateTakagiSugenoSingle(tempDifference, antecedent1, antecedent2, antecedent3, antecedent4, antecedent5);
    InformationScreen.UpdateTSOutputValueDisplay(tempChange);

    // If the change is not 0, we need to turn on the fan.
    if (tempChange != 0.0f)
    {
      StopCoroutine(coAwaitTemperatureCheck); // Stop the check timer.
      temperatureChangeSpeed = tempChange; // Set the change speed.

      if (coChangeTemperature != null)
        StopCoroutine(coChangeTemperature);

      coChangeTemperature = StartCoroutine(HandleACTemperatureChange()); // Start changing the temperature.
    }
  }

  /// <summary>
  /// A function which updates the antecedent values depending on the current world temperature settings.
  /// </summary>
  private void UpdateAntecedents()
  {
    // Get the max range between the world temperature values.
    float maxDifference = WorldSystem.WorldTemperatureRange.y - WorldSystem.WorldTemperatureRange.x;
    
    // Update the values based on the current difference and current acceptance range.
    antecedent1.ResetValues(-maxDifference, -maxDifference, -maxDifference * 0.3f);
    antecedent2.ResetValues(-maxDifference * 0.5f, -maxDifference * 0.3f, WorldSystem.CurrentAcceptanceRange.x);
    antecedent3.ResetValues(WorldSystem.CurrentAcceptanceRange.x, 0.0f, WorldSystem.CurrentAcceptanceRange.y);
    antecedent4.ResetValues(WorldSystem.CurrentAcceptanceRange.y, maxDifference * 0.3f, maxDifference * 0.5f);
    antecedent5.ResetValues(maxDifference * 0.3f, maxDifference, maxDifference);

    InformationScreen.UpdateMembershipFunctionDisplays(antecedent1, antecedent2, antecedent3, antecedent4, antecedent5);
  }

  /// <summary>
  /// A simple timer function to wait before checking the temperature.
  /// </summary>
  /// <returns>Returns an IEnumerator to use in <see cref="Coroutine"/>s.</returns>
  private IEnumerator AwaitTemperatureCheck()
  {
    while (true)
    {
      // Until we can change temperature, wait the alotted time before checking the temperatures again.
      yield return new WaitForSeconds(_TemperatureCheckTimer);
      CheckTemperaturesInternal();
    }
  }

  /// <summary>
  /// An IEnumerator for handling the A/C, and changing the world temperature based on the fuzzy system results.
  /// </summary>
  /// <returns>Returns an IEnumerator to use in <see cref="Coroutine"/>s.</returns>
  private IEnumerator HandleACTemperatureChange()
  {
    WorldSystem.ToggleActiveTemperatureFluctuation(false); // The world should not fluctuate during this portion.
    yield return acFan.StartSpin(temperatureChangeSpeed); // Spin up the fan.

    // Get the sign of what direction we are going: Heating or Cooling.
    int sign = System.Math.Sign(WorldSystem.CurrentWorldTemperature - WorldSystem.TargetWorldTemperature);
    InformationScreen.UpdateAirConditionerStateDisplay(sign < 0 ? InformationScreen.ACState.Heating : InformationScreen.ACState.Cooling);
    // While we are still changing, update the temperature every second.
    while ((sign * WorldSystem.CurrentWorldTemperature) > (sign * WorldSystem.TargetWorldTemperature))
    {
      WorldSystem.ChangeWorldTemperature(temperatureChangeSpeed * Time.deltaTime);
      yield return null;
    }

    yield return acFan.EndSpin(); // Stop the fan.
    InformationScreen.UpdateAirConditionerStateDisplay(InformationScreen.ACState.Off);
    WorldSystem.ToggleActiveTemperatureFluctuation(true); // The world can fluctuate again.
    coAwaitTemperatureCheck = StartCoroutine(AwaitTemperatureCheck()); // Start waiting for a change again.
    
    yield return null;
  }
}