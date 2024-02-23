using System.Runtime.CompilerServices;

namespace BlueNoah.Math.FixedPoint {

    /// <summary>
    /// Contains common math operations.
    /// </summary>
    public sealed class FixedPointMath {

        /// <summary>
        /// PI constant.
        /// </summary>
        public static FixedPoint64 Pi = FixedPoint64.Pi;

        /**
        *  @brief PI over 2 constant.
        **/
        public static FixedPoint64 PiOver2 = FixedPoint64.PiOver2;

        /// <summary>
        /// A small value often used to decide if numeric 
        /// results are zero.
        /// </summary>
		public static FixedPoint64 Epsilon = FixedPoint64.Epsilon;

        /**
        *  @brief Degree to radians constant.
        **/
        public static FixedPoint64 Deg2Rad = FixedPoint64.Deg2Rad;

        /**
        *  @brief Radians to degree constant.
        **/
        public static FixedPoint64 Rad2Deg = FixedPoint64.Rad2Deg;

        /// <summary>
        /// Gets the square root.
        /// </summary>
        /// <param name="number">The number to get the square root from.</param>
        /// <returns></returns>
        #region public static FP Sqrt(FP number)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Sqrt(FixedPoint64 number) {
            return FixedPoint64.Sqrt(number);
        }
        #endregion

        /// <summary>
        /// Gets the maximum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the largest value.</returns>
        #region public static FP Max(FP val1, FP val2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Max(FixedPoint64 val1, FixedPoint64 val2) {
            return (val1 > val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        /// Gets the minimum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the smallest value.</returns>
        #region public static FP Min(FP val1, FP val2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Min(FixedPoint64 val1, FixedPoint64 val2) {
            return (val1 < val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        /// Gets the maximum number of three values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <param name="val3">The third value.</param>
        /// <returns>Returns the largest value.</returns>
        #region public static FP Max(FP val1, FP val2,FP val3)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Max(FixedPoint64 val1, FixedPoint64 val2, FixedPoint64 val3) {
            FixedPoint64 max12 = (val1 > val2) ? val1 : val2;
            return (max12 > val3) ? max12 : val3;
        }
        #endregion

        /// <summary>
        /// Returns a number which is within [min,max]
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        #region public static FP Clamp(FP value, FP min, FP max)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Clamp(FixedPoint64 value, FixedPoint64 min, FixedPoint64 max) {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }
        #endregion

        /// <summary>
        /// Changes every sign of the matrix entry to '+'
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="result">The absolute matrix.</param>
        #region public static void Absolute(ref JMatrix matrix,out JMatrix result)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Absolute(ref FixedPointMatrix matrix, out FixedPointMatrix result) {
            result.M11 = FixedPoint64.Abs(matrix.M11);
            result.M12 = FixedPoint64.Abs(matrix.M12);
            result.M13 = FixedPoint64.Abs(matrix.M13);
            result.M21 = FixedPoint64.Abs(matrix.M21);
            result.M22 = FixedPoint64.Abs(matrix.M22);
            result.M23 = FixedPoint64.Abs(matrix.M23);
            result.M31 = FixedPoint64.Abs(matrix.M31);
            result.M32 = FixedPoint64.Abs(matrix.M32);
            result.M33 = FixedPoint64.Abs(matrix.M33);
        }
        #endregion

        /// <summary>
        /// Returns the sine of value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Sin(FixedPoint64 value) {
            return FixedPoint64.Sin(value);
        }

        /// <summary>
        /// Returns the cosine of value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Cos(FixedPoint64 value) {
            return FixedPoint64.Cos(value);
        }

        /// <summary>
        /// Returns the tan of value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Tan(FixedPoint64 value) {
            return FixedPoint64.Tan(value);
        }

        /// <summary>
        /// Returns the arc sine of value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Asin(FixedPoint64 value) {
            return FixedPoint64.Asin(value);
        }

        /// <summary>
        /// Returns the arc cosine of value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Acos(FixedPoint64 value) {
            return FixedPoint64.Acos(value);
        }

        /// <summary>
        /// Returns the arc tan of value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Atan(FixedPoint64 value) {
            return FixedPoint64.Atan(value);
        }

        /// <summary>
        /// Returns the arc tan of coordinates x-y.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Atan2(FixedPoint64 y, FixedPoint64 x) {
            return FixedPoint64.Atan2(y, x);
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Floor(FixedPoint64 value) {
            return FixedPoint64.Floor(value);
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Ceiling(FixedPoint64 value) {
            return value;
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Round(FixedPoint64 value) {
            return FixedPoint64.Round(value);
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(FixedPoint64 value) {
            return FixedPoint64.Sign(value);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Abs(FixedPoint64 value) {
            return FixedPoint64.Abs(value);                
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Barycentric(FixedPoint64 value1, FixedPoint64 value2, FixedPoint64 value3, FixedPoint64 amount1, FixedPoint64 amount2) {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 CatmullRom(FixedPoint64 value1, FixedPoint64 value2, FixedPoint64 value3, FixedPoint64 value4, FixedPoint64 amount) {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using FPs not to lose precission
            FixedPoint64 amountSquared = amount * amount;
            FixedPoint64 amountCubed = amountSquared * amount;
            return (FixedPoint64)(0.5 * (2.0 * value2 +
                                 (value3 - value1) * amount +
                                 (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                                 (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Distance(FixedPoint64 value1, FixedPoint64 value2) {
            return FixedPoint64.Abs(value1 - value2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Hermite(FixedPoint64 value1, FixedPoint64 tangent1, FixedPoint64 value2, FixedPoint64 tangent2, FixedPoint64 amount) {
            // All transformed to FP not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            FixedPoint64 v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            FixedPoint64 sCubed = s * s * s;
            FixedPoint64 sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                         (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                         t1 * s +
                         v1;
            return (FixedPoint64)result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Lerp(FixedPoint64 value1, FixedPoint64 value2, FixedPoint64 amount) {
            return value1 + (value2 - value1) * amount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 InverseLerp(FixedPoint64 value1, FixedPoint64 value2, FixedPoint64 amount)
        {
            return (amount - value1) / (value2 - value1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 SmoothStep(FixedPoint64 value1, FixedPoint64 value2, FixedPoint64 amount) {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
            FixedPoint64 result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }
    }
}
