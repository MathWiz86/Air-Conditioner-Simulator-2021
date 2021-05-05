/**************************************************************************************************/
/*!
\file   TSMembershipFunction.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-01
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of a Takagi-Sugeno Membership Function.

\par References:
*/
/**************************************************************************************************/

namespace ACSim.FuzzyLogic
{
  /// <summary>
  /// An abstract class for any membership functions used in a Takagi-Sugeno system.
  /// </summary>
  public abstract class TSMembershipFunction : MembershipFunction
  {
    /// <summary>The output function for this membership.</summary>
    protected System.Func<float, float> outputFunction;

    /// <summary>
    /// A function used to get the output value of this membership function, based on a firing strength.
    /// </summary>
    /// <param name="fireStrength">The firing strength of this membership function.</param>
    /// <returns>Returns the output value.</returns>
    public float GetOutputFunctionValue(float fireStrength)
    {
      return outputFunction.Invoke(fireStrength);
    }
  }
}