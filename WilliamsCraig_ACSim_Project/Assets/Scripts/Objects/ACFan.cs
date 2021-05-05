/**************************************************************************************************/
/*!
\file   ACFan.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-02
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains code for the air conditioner fan object, which handles the fan animation.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;

namespace ACSim.Objects
{
  /// <summary>
  /// An air conditioner object. This handles the speed of the fan based on how fast the
  /// world temperature is changing.
  /// </summary>
  public class ACFan : MonoBehaviour
  {
    /// <summary>The <see cref="Transform"/> of the fan. This will be spun.</summary>
    [SerializeField] private Transform fanTR = null;

    /// <summary>The current speed the fan is spinning.</summary>
    private float currentSpeed = 0.0f;
    /// <summary>The target speed the fan will reach.</summary>
    private float targetSpeed = 0.0f;
    /// <summary>The time it takes for the fan to spin up to max speed.</summary>
    private float spinUpTime = 4.1f;
    /// <summary>The time it takes for the fan to spin down to a standstill.</summary>
    private float spinDownTime = 4.2f;

    /// <summary>The range of speed that the fan can spin.</summary>
    private Vector2 spinSpeedRange = new Vector2(2.0f, 3.5f);

    /// <summary>The coroutine for spinning up to the <see cref="targetSpeed"/>.</summary>
    private Coroutine coSpinToTarget;
    /// <summary>The coroutine for spinning the fan constantly.</summary>
    private Coroutine coSpinConstantly;

    /// <summary>
    /// A function to update the current rotation of the fan, based on the speed.
    /// </summary>
    private void UpdateRotation()
    {
      Vector3 angles = fanTR.localEulerAngles; // Get the current rotation.
      angles.z += (currentSpeed * 400 * Time.deltaTime); // Add the next rotation.
      fanTR.localEulerAngles = angles; // Set the new rotation.
    }

    /// <summary>
    /// A function called to start spinning the fan.
    /// </summary>
    /// <param name="target">The target speed to spin up to.</param>
    /// <returns>Returns a Coroutine that matches <see cref="coSpinToTarget"/>.</returns>
    public Coroutine StartSpin(float target)
    {
      // Stop the coroutines that are currently happening.
      if (coSpinToTarget != null)
        StopCoroutine(coSpinToTarget);
      if (coSpinConstantly != null)
        StopCoroutine(coSpinConstantly);

      // Calculate the target speed. It should be within range, and in the direction of heating or cooling.
      targetSpeed = Mathf.Clamp(Mathf.Abs(target), spinSpeedRange.x, spinSpeedRange.y);
      targetSpeed *= Mathf.Sign(target);

      // Start spinning to the target speed. Then, spin constantly.
      coSpinToTarget = StartCoroutine(SpinToTargetSpeed(targetSpeed, spinUpTime, () => coSpinConstantly = StartCoroutine(SpinFan())));
      return coSpinToTarget;
    }

    /// <summary>
    /// A function to end the current fan spin.
    /// </summary>
    /// <returns>Returns a Coroutine that matches <see cref="coSpinToTarget"/>.</returns>
    public Coroutine EndSpin()
    {
      // Stop the coroutines that are currently happening.
      if (coSpinToTarget != null)
        StopCoroutine(coSpinToTarget);

      if (coSpinConstantly != null)
        StopCoroutine(coSpinConstantly);

      // Start spinning down to 0 speed.
      coSpinToTarget = StartCoroutine(SpinToTargetSpeed(0.0f, spinDownTime));
      return coSpinToTarget;
    }

    /// <summary>
    /// A routine for spinning the fan up to a target speed.
    /// </summary>
    /// <param name="target">The target speed to reach.</param>
    /// <param name="time">The amount of time to take to reach the <paramref name="target"/>.</param>
    /// <param name="endTask">An ending task to call once the fan is up to speed.</param>
    /// <returns>Returns a <see cref="IEnumerator"/>, which can be used in a <see cref="Coroutine"/>.</returns>
    public IEnumerator SpinToTargetSpeed(float target, float time, System.Action endTask = null)
    {
      float lerpTimer = 0.0f;
      float startSpeed = currentSpeed;

      // While lerping, update the current speed and rotate the fan.
      while (lerpTimer < time)
      {
        lerpTimer += Time.deltaTime;

        currentSpeed = Mathf.Lerp(startSpeed, target, lerpTimer / time);
        UpdateRotation();
        yield return null;
      }

      endTask?.Invoke(); // Invoke the final task.
    }

    /// <summary>
    /// A function which constantly spins the fan at the current speed.
    /// </summary>
    /// <returns>Returns a <see cref="IEnumerator"/>, which can be used in a <see cref="Coroutine"/>.</returns>
    private IEnumerator SpinFan()
    {
      // Until manually stopped, update the rotation.
      while (true)
      {
        UpdateRotation();
        yield return null;
      }
    }
  }
}