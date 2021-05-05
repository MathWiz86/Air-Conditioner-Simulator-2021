/**************************************************************************************************/
/*!
\file   PhoneScreen.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains a base class for all menus displayed in the phone user interface.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;

namespace ACSim.UI.Screens
{
  /// <summary>
  /// A base class for all menus that are displayed on the phone UI.
  /// </summary>
  [RequireComponent(typeof(Animator))] [RequireComponent(typeof(Canvas))]
  public abstract class PhoneScreen : MonoBehaviour
  {
    /// <summary>The <see cref="Animator"/> of the screen.</summary>
    protected Animator screenAnimator = null;
    /// <summary>The <see cref="Canvas  "/> of the screen.</summary>
    public Canvas screenCanvas { get; protected set; }

    protected virtual void Awake()
    {
      // Get the components.
      screenAnimator = GetComponent<Animator>();
      screenCanvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// A function called right before the screen is displayed.
    /// </summary>
    public virtual void InitializeScreen() { }

    /// <summary>
    /// A routine called when the screen is being displayed. Be sure to add a base call
    /// back to this routine.
    /// </summary>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    public virtual IEnumerator DisplayScreen()
    {
      yield return StartCoroutine(AwaitAnimation("DISPLAY")); // Wait for the display animation.
    }

    /// <summary>
    /// A routine called when the sreen is being hidden. Be sure to add a base call back to this
    /// routine.
    /// </summary>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    public virtual IEnumerator HideScreen()
    {
      yield return StartCoroutine(AwaitAnimation("HIDE")); // Wait for the hide animation.
    }

    /// <summary>
    /// A routine to wait for an animation to complete.
    /// </summary>
    /// <param name="animName">The name of the animation state.</param>
    /// <param name="layer">The layer of the animation state.</param>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    protected IEnumerator AwaitAnimation(string animName, int layer = 0)
    {
      screenAnimator.Play(animName, layer); // Play the animation.

      // Wait a bit, to make sure the state updates. Then get the time remaining.
      yield return new WaitForSeconds(0.01f);
      float time = screenAnimator.GetCurrentAnimatorStateInfo(0).length - 0.01f;
      // Wait out the remaining time.
      yield return new WaitForSeconds(time);
    }
  }
}