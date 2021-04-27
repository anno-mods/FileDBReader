namespace Syroot.BinaryData.Core {

  /// <summary>
  /// Represents utilities for mathematical operations.
  /// </summary>
  public static class MathTools {
    // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

    #region Methods

    /// <summary>
    /// Calculates the delta required to add to <paramref name="position"/> to reach the given
    /// <paramref name="alignment"/>.
    /// </summary>
    /// <param name="position">The initial position.</param>
    /// <param name="alignment">The multiple to align to. If negative, the delta is negative to reach the previous
    /// multiple rather than the next one.</param>
    /// <returns>The delta to add to the position.</returns>
    public static int GetAlignmentDelta(int position, int alignment) {
      return (-position % alignment + alignment) % alignment;
    }

    #endregion Methods
  }
}