/**************************************************************************************************/
/*!
\file   PhoneSystem.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the system singleton for the phone user interface. This is primarily
  for handling information about temperatures.

\par References:
*/
/**************************************************************************************************/

using ACSim.UI.Screens;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ACSim.Systems
{
  /// <summary>
  /// The singleton system for the game's user interface.
  /// </summary>
  public class PhoneSystem : MonoBehaviour
  {
    /// <summary>The singleton signature for this system.</summary>
    private static PhoneSystem _singleton;

    /// <summary>A blockade image to prevent any mouse clicks.</summary>
    [SerializeField] private Image raycastBlocker = null;
    /// <summary>The main menu on the phone. Used for initialization.</summary>
    [SerializeField] private MainScreen mainPhoneScreen = null;
    /// <summary>The settings menu on the phone. Used to fix a bug with <see cref="Min_Max_Slider.MinMaxSlider"/>.</summary>
    [SerializeField] private SettingsScreen settingsScreen = null;

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
      // Fix the settings screen bug.
      StartCoroutine(FixSettingsScreen());
    }

    /// <summary>
    /// There exists a bug with the <see cref="Min_Max_Slider.MinMaxSlider"/> that causes the
    /// sliders to become unusable if the canvas they are attached to is not enabled when the
    /// scene is launched. This routine merely keeps it enabled for a moment, before disabling
    /// it while the title screen is being viewed.
    /// </summary>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    private IEnumerator FixSettingsScreen()
    {
      settingsScreen.screenCanvas.enabled = enabled;
      yield return new WaitForSeconds(1.1f);
      settingsScreen.screenCanvas.enabled = false;
    }

    /// <summary>
    /// A function to begin the simulation for the phone. This should only be called once.
    /// </summary>
    public static void BeginSimulation()
    {
      if (_singleton)
        _singleton.StartCoroutine(_singleton.BeginSimulationInternal());
    }

    /// <summary>
    /// An IEnumerator to begin the simulation for the phone. This should only be called once.
    /// </summary>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    private IEnumerator BeginSimulationInternal()
    {
      raycastBlocker.raycastTarget = true;
      yield return mainPhoneScreen.BootUpScreen();
      raycastBlocker.raycastTarget = false;
    }

    /// <summary>
    /// A function to update the screen being displayed on the phone.
    /// </summary>
    /// <param name="hideScreen">The previously displayed menu.</param>
    /// <param name="displayScreen">The menu to display.</param>
    public static void ChangeScreens(PhoneScreen hideScreen, PhoneScreen displayScreen)
    {
      if (_singleton)
        _singleton.StartCoroutine(_singleton.HandleChangeScreens(hideScreen, displayScreen));
    }

    /// <summary>
    /// The internal routine for updating the screen being displayed on the phone.
    /// </summary>
    /// <param name="hideScreen">The previously displayed menu.</param>
    /// <param name="displayScreen">The menu to display.</param>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    private IEnumerator HandleChangeScreens(PhoneScreen hideScreen, PhoneScreen displayScreen)
    {
      raycastBlocker.raycastTarget = true; // Block any mouse clicks.

      // Play the hide animation for the previous screen.
      if (hideScreen != null)
      {
        yield return StartCoroutine(hideScreen.HideScreen());
        hideScreen.screenCanvas.enabled = false;
      }

      // Play the display animation for the current screen.
      if (displayScreen != null)
      {
        displayScreen.InitializeScreen();
        displayScreen.screenCanvas.enabled = true;
        yield return StartCoroutine(displayScreen.DisplayScreen());
      }

      raycastBlocker.raycastTarget = false; // Allow mouse clicks.
    }
  }
}