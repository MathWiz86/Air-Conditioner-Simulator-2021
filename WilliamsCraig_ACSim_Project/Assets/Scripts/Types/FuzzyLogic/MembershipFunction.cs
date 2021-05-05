/**************************************************************************************************/
/*!
\file   MembershipFunction.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-01
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of a Membership Function default class.

\par References:
*/
/**************************************************************************************************/

namespace ACSim.FuzzyLogic
{
  /// <summary>
  /// An abstract base class for any type of membership function.
  /// </summary>
  public abstract class MembershipFunction
  {
    /// <summary>A constant value returned when the membership function is not valid.</summary>
    public static readonly float InvalidFunctionValue = -999.9f;

    /// <summary>The min value to return in a membership function. </summary>
    public static readonly float MinValue = 0.0f;

    /// <summary>The max value to return in a membership function. </summary>
    public static readonly float MaxValue = 1.0f;

    /// <summary>A check to make sure the inputted variables for this function are valid.</summary>
    public bool isValidFunction { get { return Validate(); } }

    /// <summary>
    /// A function that gets a value, based on the <paramref name="crispInput"/>, from the membership function.
    /// </summary>
    /// <param name="crispInput">The input for the function.</param>
    /// <returns>Returns the membership function's value, based on the <paramref name="crispInput"/>.
    /// If the function is not valid, returns <see cref="InvalidFunctionValue"/>.</returns>
    public float GetValueFromFunction(float crispInput)
    {
      return isValidFunction ? GetValueFromFunctionInternal(crispInput) : InvalidFunctionValue;
    }

    /// <summary>
    /// A function that gets a value, based on the <paramref name="crispInput"/>, from the membership function.
    /// </summary>
    /// <param name="crispInput">The input for the function.</param>
    /// <param name="foundValue">The outputted value.
    /// If the function is not valid, returns <see cref="InvalidFunctionValue"/>.</param>
    /// <returns>Returns if the function is valid, and thus returned a proper output.</returns>
    public bool GetValueFromFunction(float crispInput, out float foundValue)
    {
      foundValue = isValidFunction ? GetValueFromFunctionInternal(crispInput) : InvalidFunctionValue;
      return isValidFunction;
    }

    /// <summary>
    /// The internal function that gets a value, based on the <paramref name="crispInput"/>, from the membership function.
    /// </summary>
    /// <param name="crispInput">The input for the function.</param>
    /// <returns>Returns the membership function's value, based on the <paramref name="crispInput"/>.</returns>
    protected abstract float GetValueFromFunctionInternal(float crispInput);

    /// <summary>
    /// A helper function to validate the inputted variables of the membership function.
    /// When implementing, this should directly set <see cref="isValidFunction"/>.
    /// </summary>
    protected abstract bool Validate();
  }
}