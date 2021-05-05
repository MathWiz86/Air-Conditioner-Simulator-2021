/**************************************************************************************************/
/*!
\file   Television.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-02
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the Television object, which will change its image over and over.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;

namespace ACSim.Objects
{
  /// <summary>
  /// The television in the middle of the room. This changes the image displayed over time.
  /// </summary>
  public class Television : MonoBehaviour
  {
    /// <summary>The <see cref="MeshRenderer"/> of the tv.</summary>
    private MeshRenderer mrenderer;

    /// <summary>The range of time to wait before swapping the image.</summary>
    [SerializeField] private Vector2 swapTimeRange = new Vector2(5.0f, 10.0f);
    /// <summary>The textures that will be displayed on the tv, in order.</summary>
    [SerializeField] private Texture2D[] tvFrames = null;

    private void Awake()
    {
      // Get the mesh renderer and start the image swapping.
      mrenderer = GetComponent<MeshRenderer>();
      StartCoroutine(SwapTVImage());
    }

    /// <summary>
    /// A routine for swapping the displayed image on the tv.
    /// </summary>
    /// <returns>Returns a <see cref="IEnumerator"/>, which can be used in a <see cref="Coroutine"/>.</returns>
    private IEnumerator SwapTVImage()
    {
      int index = 0; // A current index for the array.

      // Constantly update the image.
      while (true)
      {
        // Get the second material (as set on the tv model) and set the texture.
        Material[] mats = mrenderer.materials;
        mats[1].mainTexture = tvFrames[index];
        mrenderer.materials = mats;

        // Increment the index. If the index is more than the number of frames, wrap to 0.
        index++;
        if (index >= tvFrames.Length)
          index = 0;

        // Wait a random amount of time before the next swap.
        yield return new WaitForSeconds(Random.Range(swapTimeRange.x, swapTimeRange.y));
      }
    }
  }
}