/**************************************************************************************************/
/*!
\file   ThermostatScreen.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of the thermostat menu.

\par References:
*/
/**************************************************************************************************/

using ACSim.Systems;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ACSim.UI.Screens
{
  /// <summary>
  /// The menu for changing the target temperature to reach in the world.
  /// </summary>
  public class ThermostatScreen : PhoneScreen
  {
    /// <summary>The button for incrementing the target temperature.</summary>
    [SerializeField] private MenuButton buIncrement = null;
    /// <summary>The button for decrementing the target temperature.</summary>
    [SerializeField] private MenuButton buDecrement = null;
    /// <summary>The text for displaying the current the target temperature.</summary>
    [SerializeField] private TextMeshProUGUI tmpTargetTemperature = null;
    /// <summary>The fill image for the current temperature based on the range allowed.</summary>
    [SerializeField] private Image imgTemperatureGauge = null;
    /// <summary>The amount of time to wait before registering a button is being held.</summary>
    [SerializeField] private float timerHoldWait = 0.7f;
    /// <summary>The amount of time to wait before updating the temperature as the button is held.</summary>
    [SerializeField] private float timerHoldChange = 0.03f;
    /// <summary>The max scale for the button when pressed.</summary>
    [SerializeField] private Vector3 maxButtonScale = new Vector3(1.3f, 1.3f, 1.0f);
    /// <summary>The amount of time to grow the button up and down as its held.</summary>
    [SerializeField] private float timerButtonGrowth = 0.015f;

    /// <summary>A bool for determining if a button is being held down.</summary>
    private bool isHoldingButton = false;
    /// <summary>A routine for holding down the button and updating the value.</summary>
    private Coroutine coButtonHold = null;
    /// <summary>The current target temperature.</summary>
    private float currentTarget = 0.0f;

    /// <summary>
    /// A function to change the temperature by a certain amount.
    /// </summary>
    /// <param name="change">The amount to change the temperature by.</param>
    /// <param name="button">The button that caused the change.</param>
    public void ChangeTargetTemperature(float change, MenuButton button)
    {
      DiscontinueButtonHold(); // Stop the previous button hold.
      coButtonHold = StartCoroutine(HoldChangeButton(change, button)); // Start holding the button.
    }

    /// <summary>
    /// A function to stop holding the last button.
    /// </summary>
    public void DiscontinueButtonHold()
    {
      // Stop the coroutine and reset the bool toggle.
      if (coButtonHold != null)
        StopCoroutine(coButtonHold);

      isHoldingButton = false;
    }

    /// <summary>
    /// A function to update current target value.
    /// </summary>
    /// <param name="change">The amount to change the temperature by.</param>
    private void UpdateTargetWorldTemperature(float change)
    {
      // Update the current target, while keeping on the world range.
      Vector2 range = WorldSystem.WorldTemperatureRange;
      currentTarget = Mathf.Clamp(currentTarget + change, range.x,range.y);

      // Update the text and fill amount.
      tmpTargetTemperature.text = currentTarget.ToString();
      imgTemperatureGauge.fillAmount = (currentTarget - range.x) / (range.y - range.x);

      // Update the buttons to only be enabled or disabled when not at the extremities of the range.
      buDecrement.IsEnabled = currentTarget > range.x;
      buIncrement.IsEnabled = currentTarget < range.y;
    }

    /// <summary>
    /// A routine for changing the temperature and holding the button.
    /// </summary>
    /// <param name="change">The amount to change the temperature by.</param>
    /// <param name="button">The button that caused the change.</param>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    private IEnumerator HoldChangeButton(float change, MenuButton button)
    {
      StartCoroutine(ChangeButtonScale(button)); // Change the button scale once.
      UpdateTargetWorldTemperature(change); // Update the temperature once.

      // Wait a bit of time. If this function is still active by that point, we are holding the button.
      yield return new WaitForSeconds(timerHoldWait);
      isHoldingButton = true;

      // Start changing the button scale again.
      StartCoroutine(ChangeButtonScale(button));

      // Until the button is no longer held, update the target temperature every increment.
      while (true)
      {
        UpdateTargetWorldTemperature(change);
        yield return new WaitForSeconds(timerHoldChange);
      }
    }

    /// <summary>
    /// A function to animate the button growing and shrinking each click.
    /// </summary>
    /// <param name="button">The button that caused the change.</param>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    private IEnumerator ChangeButtonScale(MenuButton button)
    {
      // Get information on the rect transform and the start and target scales.
      RectTransform buRTR = button.GetComponent<RectTransform>();
      Vector3 start = buRTR.localScale;
      Vector3 target = new Vector3(start.x * maxButtonScale.x, start.y * maxButtonScale.y, start.z * maxButtonScale.z);

      do
      {
        float lerpTimer = 0.0f;
        // While lerping, update the scale to grow up.
        while (lerpTimer < timerButtonGrowth)
        {
          lerpTimer += Time.deltaTime;
          buRTR.localScale = Vector3.Lerp(start, target, lerpTimer / timerButtonGrowth);
          yield return null;
        }

        lerpTimer = 0.0f;
        // While lerping, update the scale to grow down.
        while (lerpTimer < timerButtonGrowth)
        {
          lerpTimer += Time.deltaTime;
          buRTR.localScale = Vector3.Lerp(target, start, lerpTimer / timerButtonGrowth);
          yield return null;
        }
      }
      while (isHoldingButton); // Do this while holding the button.
    }

    /// <summary>
    /// A function to submit the changes to the <see cref="WorldSystem"/>.
    /// </summary>
    public void SubmitChanges()
    {
      WorldSystem.TargetWorldTemperature = currentTarget; // Set the target.
      ACSystem.CheckTemperatures(); // Force a temperature check.
    }

    public override void InitializeScreen()
    {
      currentTarget = WorldSystem.TargetWorldTemperature;
      UpdateTargetWorldTemperature(0.0f);
    }
  }
}