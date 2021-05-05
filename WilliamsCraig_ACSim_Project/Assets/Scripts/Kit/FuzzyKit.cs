/**************************************************************************************************/
/*!
\file   FuzzyKit.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-01
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains a small kit of functions used for any Fuzzy Logic.

\par References:
*/
/**************************************************************************************************/

using ACSim.Extensions;
using ACSim.FuzzyLogic;

namespace ACSim.Kits
{
  /// <summary>
  /// A collection of handy functions to use for fuzzy logic calculations.
  /// </summary>
  public static class FuzzyKit
  {
    /// <summary>
    /// A function which calculates the output of a Takagi-Sugeno fuzzy system, given that there is only one input.
    /// </summary>
    /// <param name="crispInput">The crisp input for the membership functions.</param>
    /// <param name="inputFunctions">All membership functions that make up this system.</param>
    /// <returns>Returns the final output value of the system.</returns>
    public static float CalculateTakagiSugenoSingle(float crispInput, params TSMembershipFunction[] inputFunctions)
    {
      // Return an invalid value if there are no input functions.
      if (inputFunctions.IsEmptyOrNull())
        return MembershipFunction.InvalidFunctionValue;

      float topSum = 0.0f; // The top of the final equation.
      float bottomSum = 0.0f; // The bottom of the final equation.

      int count = inputFunctions.Length;

      // Iterate through all input functions.
      for (int i = 0; i < count; i++)
      {
        float fireStrength = inputFunctions[i].GetValueFromFunction(crispInput); // Get the firing strength.
        float ruleOutput = inputFunctions[i].GetOutputFunctionValue(fireStrength); // Get the output of the rule.

        topSum += (fireStrength * ruleOutput); // The top is the sum of all firing strengths multiplied by the rule output.
        bottomSum += fireStrength; // The bottom is the sum of all firing strengths.
      }

      return topSum / bottomSum; // Return the final value.
    }
  }
}