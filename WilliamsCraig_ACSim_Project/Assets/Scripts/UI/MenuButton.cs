/**************************************************************************************************/
/*!
\file   MenuButton.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-02
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of a UI button with extra functionality. It works directly
  with the UltEvents plugin.

\par References:
*/
/**************************************************************************************************/

using ACSim.UnityEditor;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UltEvents;

namespace ACSim.UI
{
  /// <summary>
  /// A UI button that works directly with the <see cref="UltEvent"/> plugin.
  /// </summary>
  public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
  {
    /// <summary>The <see cref="RectTransform"/> to move around for movement effects.</summary>
    [SerializeField] private RectTransform moveRTR = null;
    /// <summary>The text for the button.</summary>
    [SerializeField] private TextMeshProUGUI tmpTitle = null;
    /// <summary>The image for the button. This is changed in image effects.</summary>
    [SerializeField] private Image imgButton = null;

    /// <summary>A bool determining if the button is enabled.</summary>
    public bool IsEnabled { get { return _IsEnabled; } set { ToggleEnable(value); } }
    /// <summary>The internal value for <see cref="IsEnabled"/>.</summary>
    [SerializeField] [ReadOnly(true)] private bool _IsEnabled = true;

    /// <summary>An event called when the button is enabled. This is via setting <see cref="IsEnabled"/>.</summary>
    public UltEvent onEnable = null;
    /// <summary>An event called when the button is disabled. This is via setting <see cref="IsEnabled"/>.</summary>
    public UltEvent onDisable = null;
    /// <summary>An event called when the mouse hovers over the button.</summary>
    public UltEvent onPointerEnter = null;
    /// <summary>An event called when the mouse stops hovering over the button.</summary>
    public UltEvent onPointerExit = null;
    /// <summary>An event called when the mouse holds down on the button.</summary>
    public UltEvent onPointerDown = null;
    /// <summary>An event called when the mouse releases the button.</summary>
    public UltEvent onPointerUp = null;
    /// <summary>An event called when the mouse clicks hte button when it is enabled.</summary>
    public UltEvent onClickedWhenEnabled = null;
    /// <summary>An event called when the mouse clicks hte button when it is disabled.</summary>
    public UltEvent onClickedWhenDisabled = null;

    /// <summary>The default position that <see cref="moveRTR"/> is in.</summary>
    private Vector2 defaultPosition = Vector2.zero;
    /// <summary>The routine for moving the button during movement effects.</summary>
    private Coroutine coMoveRTR = null;

    private void Awake()
    {
      // Set the default position.
      if (moveRTR)
        defaultPosition = moveRTR.anchoredPosition;
    }

    private void Start()
    {
      // Disable the button if necessary.
      if (!_IsEnabled)
        onDisable?.Invoke();
    }

    /// <summary>
    /// A function for toggling the button being enabled or disabled.
    /// </summary>
    /// <param name="enabled">A bool for enabling or disabling the button.</param>
    private void ToggleEnable(bool enabled)
    {
      _IsEnabled = enabled; // Set the value.

      // Either invoke the enable or disable event.
      if (_IsEnabled)
        onEnable?.Invoke();
      else
        onDisable?.Invoke();
    }

    /// <summary>
    /// An effect for changing the text in the title.
    /// </summary>
    /// <param name="title">The new text for the title.</param>
    public void ChangeTitleText(string title)
    {
      // Update the text, if the title exists.
      if (tmpTitle && title != null)
        tmpTitle.text = title;
    }

    /// <summary>
    /// An effect for changing the color of the text.
    /// </summary>
    /// <param name="color">The new color.</param>
    public void ChangeTitleColor(Color color)
    {
      // Update the color, if the title exists.
      if (tmpTitle)
        tmpTitle.color = color;
    }

    /// <summary>
    /// An effect for changing the color of the button image.
    /// </summary>
    /// <param name="color">The new color.</param>
    public void ChangeImageColor(Color color)
    {
      // Update the color, if the image exists.
      if (imgButton)
        imgButton.color = color;
    }

    /// <summary>
    /// An effect for moving the button by a set distance.
    /// </summary>
    /// <param name="distance">The distance to move the button.</param>
    /// <param name="time">The amount of time to take to move the button.</param>
    public void MoveButtonPositionBy(Vector2 distance, float time = 0.4f)
    {
      // If we can move, move to the new position.
      if (moveRTR)
        MoveButtonPositionTo(moveRTR.anchoredPosition + distance, time);
    }

    /// <summary>
    /// An effect for moving the button to a specific position.
    /// </summary>
    /// <param name="position">The distance to move the button.</param>
    /// <param name="time">The amount of time to take to move the button.</param>
    public void MoveButtonPositionTo(Vector2 position, float time = 0.4f)
    {
      // If we can move, reset the coroutine and move again.
      if (moveRTR)
      {
        if (coMoveRTR != null)
          StopCoroutine(coMoveRTR);

        coMoveRTR = StartCoroutine(HandlePositionChange(position, time));
      }
    }

    /// <summary>
    /// An effect to set the button at a specific position.
    /// </summary>
    /// <param name="position">The distance to move the button.</param>
    public void SetButtonPosition(Vector2 position)
    {
      // If we can move, set the position.
      if (moveRTR)
        moveRTR.anchoredPosition = position;
    }

    /// <summary>
    /// An effect to set the button back to its default position.
    /// </summary>
    public void SetToDefaultButtonPosition()
    {
      // If we can move, stop any movement routine. set the position.
      if (moveRTR)
      {
        if (coMoveRTR != null)
          StopCoroutine(coMoveRTR);

        moveRTR.anchoredPosition = defaultPosition;
      }
    }

    /// <summary>
    /// An effect to gradually move a button to its default position.
    /// </summary>
    /// <param name="time">The amount of time to take to move the button.</param>
    public void ReturnToDefaultButtonPosition(float time = 0.4f)
    {
      MoveButtonPositionTo(defaultPosition, time);
    }

    /// <summary>
    /// A routine for moving the button to a new position.
    /// </summary>
    /// <param name="position">The distance to move the button.</param>
    /// <param name="time">The amount of time to take to move the button.</param>
    /// <returns>Returns an <see cref="IEnumerator"/> to use in <see cref="Coroutine"/>s.</returns>
    private IEnumerator HandlePositionChange(Vector2 position, float time)
    {
      Vector2 start = moveRTR.anchoredPosition; // Get a start position.
      float lerpTimer = 0.0f; // Make a timer.

      // While moving, update the position with a lerp.
      while (lerpTimer < time)
      {
        lerpTimer += Time.deltaTime;
        moveRTR.anchoredPosition = Vector2.Lerp(start, position, lerpTimer / time);
        yield return null;
      }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (_IsEnabled)
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (_IsEnabled)
        onPointerExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (_IsEnabled)
        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (_IsEnabled)
        onPointerUp?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (_IsEnabled)
        onClickedWhenEnabled?.Invoke();
      else
        onClickedWhenDisabled?.Invoke();
    }
  }
}