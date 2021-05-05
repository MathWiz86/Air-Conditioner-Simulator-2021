/**************************************************************************************************/
/*!
\file   TitleScreen.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of the title screen for the game.

\par References:
*/
/**************************************************************************************************/

using ACSim.Systems;
using System.Collections;
using UnityEngine;

/// <summary>
/// The title screen for the game. This handles some initialization too.
/// </summary>
[RequireComponent(typeof(Animator))]
public class TitleScreen : MonoBehaviour
{
  /// <summary>The <see cref="Animator"/> for the title screen canvas.</summary>
  private Animator titleAnimator = null;

  private void Awake()
  {
    titleAnimator = GetComponent<Animator>(); // Get the animator.
    StartCoroutine(HandleTitleScreen()); // Handle title screen functionality.
  }

  /// <summary>
  /// A routine for handling the functionality of the title screen.
  /// </summary>
  /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
  private IEnumerator HandleTitleScreen()
  {
    yield return AwaitAnimation("DISPLAY"); // Display the title screen.
    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0)); // Wait for a mouse click.
    yield return AwaitAnimation("HIDE"); // Hide the title screen.

    WorldSystem.BeginSimulation(); // Begin the simulation.
    Destroy(this.gameObject); // The title screen is no longer needed.
  }

  /// <summary>
  /// A routine to wait for an animation to complete.
  /// </summary>
  /// <param name="animName">The name of the animation state.</param>
  /// <param name="layer">The layer of the animation state.</param>
  /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
  protected IEnumerator AwaitAnimation(string animName, int layer = 0)
  {
    titleAnimator.Play(animName, layer); // Play the animation.

    // Wait a bit, to make sure the state updates. Then get the time remaining.
    yield return new WaitForSeconds(0.01f);
    float time = titleAnimator.GetCurrentAnimatorStateInfo(0).length - 0.01f;
    // Wait out the remaining time.
    yield return new WaitForSeconds(time);
  }
}
