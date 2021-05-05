/**************************************************************************************************/
/*!
\file   MainScreen.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of the main menu.

\par References:
*/
/**************************************************************************************************/

using System.Collections;

namespace ACSim.UI.Screens
{
  /// <summary>
  /// The main menu that leads to all other <see cref="PhoneScreen"/>s.
  /// </summary>
  public class MainScreen : PhoneScreen
  {
    /// <summary>
    /// A function to play the boot-up animation upon the start of the game.
    /// </summary>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    public IEnumerator BootUpScreen()
    {
      yield return AwaitAnimation("BOOTUP"); // Play the animation.
    }
  }
}