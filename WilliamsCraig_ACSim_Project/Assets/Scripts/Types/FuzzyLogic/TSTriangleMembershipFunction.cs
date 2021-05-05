/**************************************************************************************************/
/*!
\file   TSTriangleMembershipFunction.cs
\author Craig Williams
\par    Unity Version: 2020.2.5
\par    Updated: 2021-03-01
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the implementation of a Takagi-Sugeno Triangular Membership Function.

\par References:
*/
/**************************************************************************************************/

namespace ACSim.FuzzyLogic
{
  /// <summary>
  /// A Triangular Membership Function used in Takagi-Sugeno systems.
  /// </summary>
  public class TSTriangleMembershipFunction : TSMembershipFunction
  {
    /// <summary>The leftmost value of the function.</summary>
    public float a;
    /// <summary>The middle value of the function.</summary>
    public float b;
    /// <summary>The rightmost value of the function.</summary>
    public float c;

    /// <summary>
    /// A constructor for a triangular membership function.
    /// </summary>
    /// <param name="a">The leftmost value.</param>
    /// <param name="b">The middle value.</param>
    /// <param name="c">The rightmost value.</param>
    /// <param name="outputFunction">The output rule function of this membership.</param>
    public TSTriangleMembershipFunction(float a, float b, float c, System.Func<float, float> outputFunction)
    {
      this.a = a;
      this.b = b;
      this.c = c;
      this.outputFunction = outputFunction;
    }

    /// <summary>
    /// A simple constructor for a triangular membership function that only needs the output function.
    /// </summary>
    /// <param name="outputFunction">The output rule function of this membership.</param>
    public TSTriangleMembershipFunction(System.Func<float, float> outputFunction)
    {
      this.outputFunction = outputFunction;
    }

    protected override float GetValueFromFunctionInternal(float crispInput)
    {
      switch (crispInput)
      {
        case float x when a == b && a == x: // Special case. If all values equal each other, return the max value.
          return MaxValue;
        case float x when b == c && b == x: // Special case. If all values equal each other, return the max value.
          return MaxValue;
        case float x when a <= x && x < b: // First branch
          return (x - a) / (b - a);
        case float x when b <= x && x < c: // Second branch
          return (c - x) / (c - b);
        default: // Default return
          return MinValue;
      }
    }

    protected override bool Validate()
    {
      return a <= b && a <= c && b <= c;
    }

    /// <summary>
    /// A helper function to reset the triangular equtaion values.
    /// </summary>
    /// <param name="a">The leftmost value.</param>
    /// <param name="b">The middle value.</param>
    /// <param name="c">The rightmost value.</param>
    public void ResetValues(float a, float b, float c)
    {
      this.a = a;
      this.b = b;
      this.c = c;
    }
  }
}