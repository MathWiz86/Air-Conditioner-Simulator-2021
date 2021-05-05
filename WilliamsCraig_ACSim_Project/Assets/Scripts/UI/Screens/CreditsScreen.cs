/**************************************************************************************************/
/*!
\file   CreditsScreen.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-04
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of the credits menu.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;

namespace ACSim.UI.Screens
{
  /// <summary>
  /// The menu for displaying the simulation's credits.
  /// </summary>
  public class CreditsScreen : PhoneScreen
  {
    /// <summary>The scroll area for the credits.</summary>
    [SerializeField] private RectTransform scrollZone = null;

    public override IEnumerator DisplayScreen()
    {
      scrollZone.anchoredPosition = Vector2.zero; // Reset the position of the scroll area.
      yield return base.DisplayScreen();
    }
  }
}