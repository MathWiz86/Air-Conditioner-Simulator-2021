/**************************************************************************************************/
/*!
\file   WorldSystem.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the system singleton for the world of Air Conditioner Simulator 2021. This is primarily
  for handling information about temperatures.

\par References:
*/
/**************************************************************************************************/

using ACSim.UI.Screens;
using ACSim.UnityEditor;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ACSim.Systems
{
  /// <summary>
  /// The singleton system for the game's world. This primarily is for handling temperature information.
  /// </summary>
  public class WorldSystem : MonoBehaviour
  {
    /// <summary>The singleton signature for this system.</summary>
    private static WorldSystem _singleton;

    /// <summary>The absolute max and min values for the world's temperature.</summary>
    public static readonly Vector2 AbsoluteTempRange = new Vector2(40.0f, 100.0f);

    /// <summary>The absolute max and min values for the fluctuation wait time.</summary>
    public static readonly Vector2 AbsoluteFluctuationWaitRange = new Vector2(3.0f, 10.0f);

    /// <summary>The absolute max and min values for the length of fluctuation.</summary>
    public static readonly Vector2 AbsoluteFluctuationLengthRange = new Vector2(2.0f, 5.0f);

    /// <summary>The absolute max and min values for amount of temperature fluctuation per second.</summary>
    public static readonly Vector2 AbsoluteFluctuationTemperatureRange = new Vector2(1.0f, 5.0f);

    /// <summary>The absolute max and min values for the acceptable temperature difference range.</summary>
    public static readonly Vector2 AbsoluteAcceptanceRange = new Vector2(-10.0f, 10.0f);

    /// <summary>The range of temperatures allowed to be reached in the game's world.</summary>
    public static Vector2 WorldTemperatureRange { get { return _singleton._WorldTemperatureRange; } set { SetWorldTemperatureRange(value); } }
    /// <summary>The internal value for <see cref="WorldTemperatureRange"/>.</summary>
    [SerializeField] [RangeRef("!AbsoluteTempRange.x", "!AbsoluteTempRange.y")]
    private Vector2 _WorldTemperatureRange;

    /// <summary>The target temperature for the A/C to reach.</summary>
    public static float TargetWorldTemperature { get { return _singleton._TargetWorldTemperature; } set { SetTargetTemperature(value); } }
    /// <summary>The internal value for <see cref="TargetWorldTemperature"/>.</summary>
    [SerializeField] [RangeRef("!_WorldTemperatureRange.x", "!_WorldTemperatureRange.y")]
    private float _TargetWorldTemperature;

    /// <summary>The range of difference allowed between the current and target temperatures before the fan turns on.</summary>
    public static Vector2 CurrentAcceptanceRange { get { return _singleton._CurrentAcceptanceRange; } set { SetTemperatureAcceptanceRange(value); } }
    /// <summary>The internal value for <see cref="CurrentAcceptanceRange"/>.</summary>
    [SerializeField] [RangeRef("!AbsoluteAcceptanceRange.x", "!AbsoluteAcceptanceRange.y")]
    private Vector2 _CurrentAcceptanceRange;

    /// <summary>The range of time to wait before each fluctuation in temperature.</summary>
    public static Vector2 FluctuationWaitRange { get { return _singleton ? _singleton._FluctuationWaitRange : Vector2.zero; } set { SetFluctuationWaitRange(value); } }
    /// <summary>The internal value for <see cref="FluctuationWaitRange"/>.</summary>
    [SerializeField] [RangeRef("!AbsoluteFluctuationWaitRange.x", "!AbsoluteFluctuationWaitRange.y")]
    private Vector2 _FluctuationWaitRange;

    /// <summary>The range of time to fluctuate the temperature.</summary>
    public static Vector2 FluctuationLengthRange { get { return _singleton ? _singleton._FluctuationLengthRange : Vector2.zero; } set { SetFluctuationLengthRange(value); } }
    /// <summary>The internal value for <see cref="FluctuationLengthRange"/>.</summary>
    [SerializeField] [RangeRef("!AbsoluteFluctuationLengthRange.x", "!AbsoluteFluctuationLengthRange.y")]
    private Vector2 _FluctuationLengthRange;

    /// <summary>The range of degrees to fluctuate per second of fluctuation.</summary>
    public static Vector2 FluctuationTemperatureRange { get { return _singleton ? _singleton._FluctuationTemperatureRange : Vector2.zero; } set { SetFluctuationTemperatureRange(value); } }
    /// <summary>The internal value for <see cref="FluctuationTemperatureRange"/>.</summary>
    [SerializeField] [RangeRef("!AbsoluteFluctuationTemperatureRange.x", "!AbsoluteFluctuationTemperatureRange.y")]
    private Vector2 _FluctuationTemperatureRange;

    /// <summary>The current world temperature. This fluctuates during gameplay.</summary>
    public static float CurrentWorldTemperature { get; private set; }

    /// <summary>A toggle for allowing the world temperature to change at random.</summary>
    public static bool AllowTemperatureFluctuation { get; private set; }

    /// <summary>A check for if fluctuation in the world is currently occuring or not.</summary>
    private bool isFluctuationActive = true;

    /// <summary>The <see cref="Coroutine"/> for temperature fluctuation.</summary>
    private Coroutine coFluctuation = null;

#if UNITY_EDITOR
    /// <summary>A viewer value for the current world temperature in the Unity Editor.</summary>
    [SerializeField] [ValueViewer("!CurrentWorldTemperature")] private float ViewCurrentWorldTemperature;
    /// <summary>A viewer value for allowing temperature fluctuation in the Unity Editor.</summary>
    [SerializeField] [ValueViewer("!AllowTemperatureFluctuation")] private bool ViewAllowTemperatureFluctuation;
#endif

    private void Awake()
    {
      // Initialize the Singleton.
      if (!_singleton)
        _singleton = this;
      else
        Destroy(gameObject);
    }

    private void Start()
    {
      SetWorldTemperature(60.0f);
    }

    /// <summary>
    /// A function to begin the simulation of the system. This should only be called once.
    /// </summary>
    public static void BeginSimulation()
    {
      // Activate Fluctuation.
      AllowTemperatureFluctuation = true;
      ToggleActiveTemperatureFluctuation(true);
      ACSystem.BeginSimulation(); // Begin the A/C System.
      PhoneSystem.BeginSimulation(); // Begin the Phone System.
    }

    /// <summary>
    /// A toggle function for if temperature fluctuation is allowed at all. This is used in world settings.
    /// </summary>
    /// <param name="isAllowed">The bool for if fluctuation is allowed.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleTemperatureFluctuation(bool isAllowed)
    {
      AllowTemperatureFluctuation = isAllowed; // Toggle fluctuation.
      InformationScreen.UpdateFluctuationOnDisplay();

      // Check if Fluctuation is currently active.
      if (_singleton)
        _singleton.ToggleActiveTemperatureFluctuationInternal(_singleton.isFluctuationActive);
    }

    /// <summary>
    /// A toggle function for if temperature fluctuation is active. This is used when the A/C is turning on or off.
    /// </summary>
    /// <param name="isOn">A bool for determining if the flucutation is currently active.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleActiveTemperatureFluctuation(bool isOn)
    {
      if (_singleton)
        _singleton.ToggleActiveTemperatureFluctuationInternal(isOn);
    }

    /// <summary>
    /// The internal toggle function for if temperature fluctuation is active.
    /// </summary>
    /// <param name="isOn"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ToggleActiveTemperatureFluctuationInternal(bool isOn)
    {
      isFluctuationActive = isOn;

      if (AllowTemperatureFluctuation)
      {
        // If fluctuation is active, and we're allowing fluctuation, turn on fluctuation.
        if (isFluctuationActive && coFluctuation == null)
          coFluctuation = StartCoroutine(FluctuateTemperature());
        // Otherwise, if currently running, stop fluctuation.
        else if (!isFluctuationActive && coFluctuation != null)
        {
          StopCoroutine(coFluctuation);
          coFluctuation = null;
        }
      }
      else if (coFluctuation != null)
      {
        StopCoroutine(coFluctuation);
        coFluctuation = null;
      }
    }

    /// <summary>
    /// A function for changing the world temperature by an increment or decrement.
    /// </summary>
    /// <param name="change">The amount of change to the world temperature.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ChangeWorldTemperature(float change)
    {
      SetWorldTemperature(CurrentWorldTemperature + change);
    }

    /// <summary>
    /// A function for outright setting the current world temperature.
    /// </summary>
    /// <param name="temp">The new world temperature.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetWorldTemperature(float temp)
    {
      CurrentWorldTemperature = Mathf.Clamp(temp, WorldTemperatureRange.x, WorldTemperatureRange.y);
      InformationScreen.UpdateWorldTemperatureDisplay();
    }

    /// <summary>
    /// An IEnumerator for flucutating the world temperature at random.
    /// </summary>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    private IEnumerator FluctuateTemperature()
    {
      while (true)
      {

        // Get a wait timer based on the random range, and wait that many seconds.
        float waitTimer = Random.Range(_FluctuationWaitRange.x, _FluctuationWaitRange.y);
        yield return new WaitForSeconds(waitTimer);

        float direction = Random.Range(0, 2) == 1 ? 1 : -1; // Get if we are increasing or decreasing temperature.

        // Get the fluctuation per second, based on the random range.
        float changePerSecond = direction * Random.Range(_FluctuationTemperatureRange.x, _FluctuationTemperatureRange.y);
        // Get the amount of time to fluctuate, based on the random range.
        float changeTimer = Random.Range(_FluctuationLengthRange.x, _FluctuationLengthRange.y);

        // While still changing, update the world temperature.
        while (changeTimer > 0.0f)
        {
          changeTimer -= Time.deltaTime;
          ChangeWorldTemperature(Time.deltaTime * changePerSecond);
          yield return null;
        }
      }
    }

    /// <summary>
    /// A function to set the range of acceptable temperature before activating the fan.
    /// </summary>
    /// <param name="range">The +/- range of temperature difference to allow.</param>
    private static void SetTemperatureAcceptanceRange(Vector2 range)
    {
      if (_singleton)
      {
        _singleton._CurrentAcceptanceRange.x = Mathf.Clamp(range.x, AbsoluteAcceptanceRange.x, AbsoluteAcceptanceRange.y);
        _singleton._CurrentAcceptanceRange.y = Mathf.Clamp(range.y, AbsoluteAcceptanceRange.x, AbsoluteAcceptanceRange.y);
      }
    }

    /// <summary>
    /// A function to set the range that the world's temperature can reach.
    /// </summary>
    /// <param name="range">The range of world temperature to allow.</param>
    private static void SetWorldTemperatureRange(Vector2 range)
    {
      if (_singleton)
      {
        _singleton._WorldTemperatureRange.x = Mathf.Clamp(range.x, AbsoluteTempRange.x, AbsoluteTempRange.y);
        _singleton._WorldTemperatureRange.y = Mathf.Clamp(range.y, AbsoluteTempRange.x, AbsoluteTempRange.y);
      }
    }

    /// <summary>
    /// A setter function for the <see cref="_FluctuationWaitRange"/>.
    /// </summary>
    /// <param name="range">The new range.</param>
    private static void SetFluctuationWaitRange(Vector2 range)
    {
      if (_singleton)
      {
        _singleton._FluctuationWaitRange.x = Mathf.Clamp(range.x, AbsoluteFluctuationWaitRange.x, AbsoluteFluctuationWaitRange.y);
        _singleton._FluctuationWaitRange.y = Mathf.Clamp(range.y, AbsoluteFluctuationWaitRange.x, AbsoluteFluctuationWaitRange.y);
      }
    }

    /// <summary>
    /// A setter function for the <see cref="_FluctuationLengthRange"/>.
    /// </summary>
    /// <param name="range">The new range.</param>
    private static void SetFluctuationLengthRange(Vector2 range)
    {
      if (_singleton)
      {
        _singleton._FluctuationLengthRange.x = Mathf.Clamp(range.x, AbsoluteFluctuationLengthRange.x, AbsoluteFluctuationLengthRange.y);
        _singleton._FluctuationLengthRange.y = Mathf.Clamp(range.y, AbsoluteFluctuationLengthRange.x, AbsoluteFluctuationLengthRange.y);
      }
    }

    /// <summary>
    /// A setter function for the <see cref="_FluctuationWaitRange"/>.
    /// </summary>
    /// <param name="range">The new range.</param>
    private static void SetFluctuationTemperatureRange(Vector2 range)
    {
      if (_singleton)
      {
        _singleton._FluctuationTemperatureRange.x = Mathf.Clamp(range.x, AbsoluteFluctuationTemperatureRange.x, AbsoluteFluctuationTemperatureRange.y);
        _singleton._FluctuationTemperatureRange.y = Mathf.Clamp(range.y, AbsoluteFluctuationTemperatureRange.x, AbsoluteFluctuationTemperatureRange.y);
      }
    }

    /// <summary>
    /// A function to set the target temperature to reach in the world.
    /// </summary>
    /// <param name="temperature">The new target temperature.</param>
    private static void SetTargetTemperature(float temperature)
    {
      if (_singleton)
        _singleton._TargetWorldTemperature = Mathf.Clamp(temperature, WorldTemperatureRange.x, WorldTemperatureRange.y);

      InformationScreen.UpdateTargetTemperatureDisplay();
    }
  }
}