/**************************************************************************************************/
/*!
\file   ExtendIList.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-02-28
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains extension functions for any IList, including Arrays and Lists.
  The code was taken from a personal code library project known as SINGULARITY (Previously called ENTITY).

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ACSim.Extensions
{
  /// <summary>
  /// A series of functions that extend the functionality of an IList. An IList includes things
  /// like arrays or Lists.
  /// </summary>
  public static partial class ExtendIList
  {
    /// <summary>
    /// A check to see if an IList is empty or null.
    /// <typeparam name="T">The type of the elements stored in the IList.</typeparam>
    /// </summary>
    /// <param name="ilist">The IList to check.</param>
    /// <returns>Returns true if the IList is null, or has a Count of 0.</returns>
    public static bool IsEmptyOrNull<T>(this IList<T> ilist)
    {
      return ilist == null || ilist.Count <= 0;
    }

    /// <summary>
    /// A check to see if an IList is empty or null.
    /// </summary>
    /// <param name="ilist">The IList to check.</param>
    /// <returns>Returns true if the IList is null, or has a Count of 0.</returns>
    public static bool IsEmptyOrNullGeneric(this IList ilist)
    {
      return ilist == null || ilist.Count <= 0;
    }

    /// <summary>
    /// A check to see if a given index is valid for an IList.
    /// </summary>
    /// <typeparam name="T">The type of the elements stored in the IList.</typeparam>
    /// <param name="ilist">The IList to check.</param>
    /// <param name="index">The index to check.</param>
    /// <returns>Returns true if the index is valid. Returns false if out of bounds, or the IList is null.</returns>
    public static bool IsValidIndex<T>(this IList<T> ilist, int index)
    {
      // Return if the IList is valid, and if the index is a valid one.
      return ilist != null && index >= 0 && index < ilist.Count;
    }

    /// <summary>
    /// A check to see if a given index is valid for an IList. This is the generic version, without a specified IList type.
    /// </summary>
    /// <param name="ilist">The IList to check.</param>
    /// <param name="index">The index to check.</param>
    /// <returns>Returns true if the index is valid. Returns false if out of bounds, or the IList is null.</returns>
    public static bool IsValidIndexGeneric(this IList ilist, int index)
    {
      // Return if the IList is valid, and if the index is a valid one.
      return ilist != null && index >= 0 && index < ilist.Count;
    }

    /// <summary>
    /// A function that gets the last valid index of a given IList.
    /// </summary>
    /// <typeparam name="T">The type of the elements stored in the IList.</typeparam>
    /// <param name="ilist">The IList to check.</param>
    /// <returns>Returns the last valid index. Returns -1 if the IList is null.</returns>
    public static int LastIndex<T>(this IList<T> ilist)
    {
      // If the IList is not null, return 1 less than the count. Otherwise, return -1.
      return ilist != null ? ilist.Count - 1 : -1;
    }

    /// <summary>
    /// A function that gets the last valid index of a given IList. This is the generic version, without a specified IList type.
    /// </summary>
    /// <param name="ilist">The IList to check.</param>
    /// <returns>Returns the last valid index. Returns -1 if the IList is null.</returns>
    public static int LastIndexGeneric(this IList ilist)
    {
      // If the IList is not null, return 1 less than the count. Otherwise, return -1.
      return ilist != null ? ilist.Count - 1 : -1;
    }

    /// <summary>
    /// A function that gets the last element within an IList.
    /// </summary>
    /// <typeparam name="T">The type of the elements stored in the IList.</typeparam>
    /// <param name="ilist">The IList to access.</param>
    /// <returns>Returns the element at the last valid index. Returns the default element value if the IList is null.</returns>
    public static T LastElement<T>(this IList<T> ilist)
    {
      // If the IList is not null, return the item at the last index. Otherwise, return the default value.
      return !ilist.IsEmptyOrNull() ? ilist[ilist.LastIndex()] : default;
    }

    /// <summary>
    /// A function that gets the last element within an IList. This is the generic version, without a specified IList type.
    /// </summary>
    /// <typeparam name="T">The type of the elements stored in the IList.</typeparam>
    /// <param name="ilist">The IList to access.</param>
    /// <returns>Returns the element at the last valid index. Returns the default element value if the IList is null.</returns>
    public static object LastElementGeneric(this IList ilist)
    {
      // If the IList is not null, return the item at the last index. Otherwise, return the default value.
      return !ilist.IsEmptyOrNullGeneric() ? ilist[ilist.LastIndexGeneric()] : null;
    }

    /// <summary>
    /// A function which will set a value to the last index of an IList.
    /// This WILL result in an error if the list is null.
    /// </summary>
    /// <typeparam name="T">The type of the elements stored in the IList.</typeparam>
    /// <param name="ilist">The IList to access.</param>
    /// <param name="value">The value to set at the last index.</param>
    public static void SetLastElement<T>(this IList<T> ilist, T value)
    {
      ilist[ilist.LastIndex()] = value; // Set the value at the last index.
    }

    /// <summary>
    /// A function which will set a value to the last index of an IList.
    /// This is the generic version, without a specified IList type.
    /// This WILL result in an error if the list is null.
    /// </summary>
    /// <param name="ilist">The IList to access.</param>
    /// <param name="value">The value to set at the last index.</param>
    public static void SetLastElementGeneric(this IList ilist, object value)
    {
      ilist[ilist.LastIndexGeneric()] = value; // Set the value at the last index.
    }

    /// <summary>
    /// A function which creates a string printout of every element in an IList.
    /// </summary>
    /// <param name="ilist">The array or list to print out.</param>
    /// <param name="separator">The separation between each element.</param>
    /// <param name="prefix">The characters to print before each element.</param>
    /// <param name="suffix">The characters to print after each element.</param>
    /// <returns>Returns the string of all elements.</returns>
    public static string CreateElementPrintout(this IList ilist, string separator = ", ", string prefix = "[", string suffix = "]")
    {
      // If the ilist is invalid or has no elements, return an empty string.
      if (ilist.IsEmptyOrNullGeneric())
        return string.Empty;

      StringBuilder message = new StringBuilder(); // Create a new StringBuilder.

      // For each element, wrap up the element in the prefix and suffix, and separate with the separator.
      foreach (object element in ilist)
        message.Append(prefix).Append(element.ToString()).Append(suffix).Append(separator);

      // Remove the final separator.
      message.Remove(message.Length - 1, 1);

      return message.ToString(); // Return the message.
    }
  }
}