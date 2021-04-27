using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Syroot.BinaryData.Core {

  /// <summary>
  /// Represents utilities for working with <see cref="Enum"/> instances.
  /// </summary>
  public static class EnumTools {
    // ---- FIELDS -------------------------------------------------------------------------------------------------

    #region Methods

    /// <summary>
    /// Validates the given <paramref name="value"/> to be defined in the enum of the given type, allowing combined
    /// flags for enums decorated with the <see cref="FlagsAttribute"/>.
    /// </summary>
    /// <param name="enumType">The type of the <see cref="Enum"/> to validate against.</param>
    /// <param name="value">The value to validate.</param>
    /// <returns><see langword="true"/> when the value is defined; otherwise, <see langword="false"/>.</returns>
    public static bool Validate(Type enumType, object value) {
      // Check if a simple value is defined in the enum.
      bool valid = Enum.IsDefined(enumType, value);
      if (!valid) {
        // For enums decorated with the FlagsAttribute, allow sets of flags.
        if (!_flagEnums.TryGetValue(enumType, out bool isFlag)) {
          isFlag = enumType.GetCustomAttributes(typeof(FlagsAttribute), false)?.Any() == true;
          _flagEnums.TryAdd(enumType, isFlag);
        }
        if (isFlag) {
          long mask = 0;
          foreach (object definedValue in Enum.GetValues(enumType))
            mask |= Convert.ToInt64(definedValue);
          long longValue = Convert.ToInt64(value);
          valid = (mask & longValue) == longValue;
        }
      }
      return valid;
    }

    #endregion Methods

    #region Fields

    private static readonly ConcurrentDictionary<Type, bool> _flagEnums = new ConcurrentDictionary<Type, bool>();

    #endregion Fields

    // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------
  }
}