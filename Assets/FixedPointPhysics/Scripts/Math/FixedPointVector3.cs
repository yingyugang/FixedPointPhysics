using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BlueNoah.Math.FixedPoint
{
    /// <summary>
    /// A vector structure.
    /// </summary>
    [Serializable]
    public struct FixedPointVector3
    {

        private static FixedPoint64 ZeroEpsilonSq = FixedPointMath.Epsilon;
        internal static FixedPointVector3 InternalZero;
        internal static FixedPointVector3 Arbitrary;

        /// <summary>The X component of the vector.</summary>
        public FixedPoint64 x;
        /// <summary>The Y component of the vector.</summary>
        public FixedPoint64 y;
        /// <summary>The Z component of the vector.</summary>
        public FixedPoint64 z;

        #region Static readonly variables
        /// <summary>
        /// A vector with components (0,0,0);
        /// </summary>
        public static readonly FixedPointVector3 zero;
        /// <summary>
        /// A vector with components (-1,0,0);
        /// </summary>
        public static readonly FixedPointVector3 left;
        /// <summary>
        /// A vector with components (1,0,0);
        /// </summary>
        public static readonly FixedPointVector3 right;
        /// <summary>
        /// A vector with components (0,1,0);
        /// </summary>
        public static readonly FixedPointVector3 up;
        /// <summary>
        /// A vector with components (0,-1,0);
        /// </summary>
        public static readonly FixedPointVector3 down;
        /// <summary>
        /// A vector with components (0,0,-1);
        /// </summary>
        public static readonly FixedPointVector3 back;
        /// <summary>
        /// A vector with components (0,0,1);
        /// </summary>
        public static readonly FixedPointVector3 forward;
        /// <summary>
        /// A vector with components (1,1,1);
        /// </summary>
        public static readonly FixedPointVector3 one;
        /// <summary>
        /// A vector with components 
        /// (FP.MinValue,FP.MinValue,FP.MinValue);
        /// </summary>
        public static readonly FixedPointVector3 MinValue;
        /// <summary>
        /// A vector with components 
        /// (FP.MaxValue,FP.MaxValue,FP.MaxValue);
        /// </summary>
        public static readonly FixedPointVector3 MaxValue;
        #endregion

        #region Private static constructor
        static FixedPointVector3()
        {
            one = new FixedPointVector3(1, 1, 1);
            zero = new FixedPointVector3(0, 0, 0);
            left = new FixedPointVector3(-1, 0, 0);
            right = new FixedPointVector3(1, 0, 0);
            up = new FixedPointVector3(0, 1, 0);
            down = new FixedPointVector3(0, -1, 0);
            back = new FixedPointVector3(0, 0, -1);
            forward = new FixedPointVector3(0, 0, 1);
            MinValue = new FixedPointVector3(FixedPoint64.MinValue);
            MaxValue = new FixedPointVector3(FixedPoint64.MaxValue);
            Arbitrary = new FixedPointVector3(1, 1, 1);
            InternalZero = zero;
        }
        #endregion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Abs(FixedPointVector3 other)
        {
            return new FixedPointVector3(FixedPoint64.Abs(other.x), FixedPoint64.Abs(other.y), FixedPoint64.Abs(other.z));
        }
        /// <summary>
        /// Projects a vector onto another vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Project(FixedPointVector3 vector, FixedPointVector3 onNormal)
        {
            var sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < FixedPointMath.Epsilon)
                return zero;
            else
            {
                var dot = Dot(vector, onNormal) / sqrMag;
                return new FixedPointVector3(onNormal.x * dot,
                    onNormal.y * dot,
                    onNormal.z * dot);
            }
        }
        /// <summary>
        /// Gets the squared length of the vector.
        /// </summary>
        /// <returns>Returns the squared length of the vector.</returns>
        public FixedPoint64 sqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }

        public FixedPoint64 sqrMagnitudeXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ((this.x * this.x) + (this.z * this.z));
        }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        /// <returns>Returns the length of the vector.</returns>
        public FixedPoint64 magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var num = ((this.x * this.x) + (this.y * this.y)) + (this.z * this.z);
                return FixedPoint64.Sqrt(num);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClampMagnitude(FixedPointVector3 vector, FixedPoint64 maxLength)
        {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        /// Gets a normalized version of the vector.
        /// </summary>
        /// <returns>Returns a normalized version of the vector.</returns>
        public FixedPointVector3 normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var result = new FixedPointVector3(this.x, this.y, this.z);
                result.Normalize();
                return result;
            }
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointVector3(Vector3Int vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointVector3(FixedPoint64 x, FixedPoint64 y, FixedPoint64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(FixedPointVector3 other)
        {
            this.x = x * other.x;
            this.y = y * other.y;
            this.z = z * other.z;
        }

        /// <summary>
        /// Sets all vector component to specific values.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(FixedPoint64 x, FixedPoint64 y, FixedPoint64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="xyz">All components of the vector are set to xyz</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointVector3(FixedPoint64 xyz)
        {
            this.x = xyz;
            this.y = xyz;
            this.z = xyz;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Lerp(FixedPointVector3 from, FixedPointVector3 to, FixedPoint64 percent)
        {
            return from + (to - from) * percent;
        }

        /// <summary>
        /// Builds a string from the JVector.
        /// </summary>
        /// <returns>A string containing all three components.</returns>
        #region public override string ToString()
        public override string ToString()
        {
            return $"({x.AsFloat():f1}, {y.AsFloat():f1}, {z.AsFloat():f1})";
        }
        #endregion

        /// <summary>
        /// Tests if an object is equal to this vector.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>Returns true if they are equal, otherwise false.</returns>
        #region public override bool Equals(object obj)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is FixedPointVector3)) return false;
            var other = (FixedPointVector3)obj;

            return (((x == other.x) && (y == other.y)) && (z == other.z));
        }
        #endregion

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Scale(FixedPointVector3 vecA, FixedPointVector3 vecB)
        {
            FixedPointVector3 result;
            result.x = vecA.x * vecB.x;
            result.y = vecA.y * vecB.y;
            result.z = vecA.z * vecB.z;

            return result;
        }

        /// <summary>
        /// Tests if two JVector are equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns true if both values are equal, otherwise false.</returns>
        #region public static bool operator ==(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            return (((value1.x == value2.x) && (value1.y == value2.y)) && (value1.z == value2.z));
        }
        #endregion

        /// <summary>
        /// Tests if two JVector are not equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns false if both values are equal, otherwise true.</returns>
        #region public static bool operator !=(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            if ((value1.x == value2.x) && (value1.y == value2.y))
            {
                return (value1.z != value2.z);
            }
            return true;
        }
        #endregion

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the minimum x,y and z values of both vectors.</returns>
        #region public static JVector Min(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Min(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Min(ref value1, ref value2, out var result);
            return result;
        }

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the minimum x,y and z values of both vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref FixedPointVector3 value1, ref FixedPointVector3 value2, out FixedPointVector3 result)
        {
            result.x = (value1.x < value2.x) ? value1.x : value2.x;
            result.y = (value1.y < value2.y) ? value1.y : value2.y;
            result.z = (value1.z < value2.z) ? value1.z : value2.z;
        }
        #endregion

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the maximum x,y and z values of both vectors.</returns>
        #region public static JVector Max(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Max(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Max(ref value1, ref value2, out var result);
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Distance(FixedPointVector3 v1, FixedPointVector3 v2)
        {
            return FixedPoint64.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 DistanceXZ(FixedPointVector3 v1, FixedPointVector3 v2)
        {
            return FixedPoint64.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
        }

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the maximum x,y and z values of both vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref FixedPointVector3 value1, ref FixedPointVector3 value2, out FixedPointVector3 result)
        {
            result.x = (value1.x > value2.x) ? value1.x : value2.x;
            result.y = (value1.y > value2.y) ? value1.y : value2.y;
            result.z = (value1.z > value2.z) ? value1.z : value2.z;
        }
        #endregion

        /// <summary>
        /// Sets the length of the vector to zero.
        /// </summary>
        #region public void MakeZero()
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MakeZero()
        {
            x = FixedPoint64.Zero;
            y = FixedPoint64.Zero;
            z = FixedPoint64.Zero;
        }
        #endregion

        /// <summary>
        /// Checks if the length of the vector is zero.
        /// </summary>
        /// <returns>Returns true if the vector is zero, otherwise false.</returns>
        #region public bool IsZero()
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero()
        {
            return (this.sqrMagnitude == FixedPoint64.Zero);
        }

        /// <summary>
        /// Checks if the length of the vector is nearly zero.
        /// </summary>
        /// <returns>Returns true if the vector is nearly zero, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNearlyZero()
        {
            return (this.sqrMagnitude < ZeroEpsilonSq);
        }
        #endregion

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The transformed vector.</returns>
        #region public static JVector Transform(JVector position, JMatrix matrix)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Transform(FixedPointVector3 position, FixedPointMatrix matrix)
        {
            FixedPointVector3.Transform(ref position, ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref FixedPointVector3 position, ref FixedPointMatrix matrix, out FixedPointVector3 result)
        {
            var num0 = ((position.x * matrix.M11) + (position.y * matrix.M21)) + (position.z * matrix.M31);
            var num1 = ((position.x * matrix.M12) + (position.y * matrix.M22)) + (position.z * matrix.M32);
            var num2 = ((position.x * matrix.M13) + (position.y * matrix.M23)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// Transforms a vector by the transposed of the given Matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransposedTransform(ref FixedPointVector3 position, ref FixedPointMatrix matrix, out FixedPointVector3 result)
        {
            var num0 = ((position.x * matrix.M11) + (position.y * matrix.M12)) + (position.z * matrix.M13);
            var num1 = ((position.x * matrix.M21) + (position.y * matrix.M22)) + (position.z * matrix.M23);
            var num2 = ((position.x * matrix.M31) + (position.y * matrix.M32)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        #region public static FP Dot(JVector vector1, JVector vector2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Dot(FixedPointVector3 vector1, FixedPointVector3 vector2)
        {
            return FixedPointVector3.Dot(ref vector1, ref vector2);
        }
        /// <summary>
        /// Calculates the dot product of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Dot(ref FixedPointVector3 vector1, ref FixedPointVector3 vector2)
        {
            return ((vector1.x * vector2.x) + (vector1.y * vector2.y)) + (vector1.z * vector2.z);
        }
        #endregion

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        #region public static void Add(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Add(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Add(ref value1, ref value2, out var result);
            return result;
        }

        /// <summary>
        /// Adds to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The sum of both vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref FixedPointVector3 value1, ref FixedPointVector3 value2, out FixedPointVector3 result)
        {
            FixedPoint64 num0 = value1.x + value2.x;
            FixedPoint64 num1 = value1.y + value2.y;
            FixedPoint64 num2 = value1.z + value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Divide(FixedPointVector3 value1, FixedPoint64 scaleFactor)
        {
            FixedPointVector3.Divide(ref value1, scaleFactor, out var result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the scaled vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(ref FixedPointVector3 value1, FixedPoint64 scaleFactor, out FixedPointVector3 result)
        {
            result.x = value1.x / scaleFactor;
            result.y = value1.y / scaleFactor;
            result.z = value1.z / scaleFactor;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        #region public static JVector Subtract(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Subtract(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Subtract(ref value1, ref value2, out var result);
            return result;
        }

        /// <summary>
        /// Subtracts to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The difference of both vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref FixedPointVector3 value1, ref FixedPointVector3 value2, out FixedPointVector3 result)
        {
            FixedPoint64 num0 = value1.x - value2.x;
            FixedPoint64 num1 = value1.y - value2.y;
            FixedPoint64 num2 = value1.z - value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product of both vectors.</returns>
        #region public static JVector Cross(JVector vector1, JVector vector2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Cross(FixedPointVector3 vector1, FixedPointVector3 vector2)
        {
            FixedPointVector3.Cross(ref vector1, ref vector2, out var result);
            return result;
        }

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="result">The cross product of both vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cross(ref FixedPointVector3 vector1, ref FixedPointVector3 vector2, out FixedPointVector3 result)
        {
            FixedPoint64 num3 = (vector1.y * vector2.z) - (vector1.z * vector2.y);
            FixedPoint64 num2 = (vector1.z * vector2.x) - (vector1.x * vector2.z);
            FixedPoint64 num = (vector1.x * vector2.y) - (vector1.y * vector2.x);
            result.x = num3;
            result.y = num2;
            result.z = num;
        }
        #endregion

        /// <summary>
        /// Gets the hashcode of the vector.
        /// </summary>
        /// <returns>Returns the hashcode of the vector.</returns>
        #region public override int GetHashCode()
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Inverses the direction of the vector.
        /// </summary>
        #region public static JVector Negate(JVector value)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <returns>The negated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Negate(FixedPointVector3 value)
        {
            FixedPointVector3.Negate(ref value, out var result);
            return result;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <param name="result">The negated vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negate(ref FixedPointVector3 value, out FixedPointVector3 result)
        {
            var num0 = -value.x;
            var num1 = -value.y;
            var num2 = -value.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <returns>A normalized vector.</returns>
        #region public static JVector Normalize(JVector value)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Normalize(FixedPointVector3 value)
        {
            FixedPointVector3.Normalize(ref value, out var result);
            return result;
        }

        /// <summary>
        /// Normalizes this vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            var num2 = ((this.x * this.x) + (this.y * this.y)) + (this.z * this.z);
            var num = FixedPoint64.One / FixedPoint64.Sqrt(num2);
            this.x *= num;
            this.y *= num;
            this.z *= num;
        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <param name="result">A normalized vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(ref FixedPointVector3 value, out FixedPointVector3 result)
        {
            var num2 = ((value.x * value.x) + (value.y * value.y)) + (value.z * value.z);
            var num = FixedPoint64.One / FixedPoint64.Sqrt(num2);
            result.x = value.x * num;
            result.y = value.y * num;
            result.z = value.z * num;
        }
        #endregion

        #region public static void Swap(ref JVector vector1, ref JVector vector2)

        /// <summary>
        /// Swaps the components of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector to swap with the second.</param>
        /// <param name="vector2">The second vector to swap with the first.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref FixedPointVector3 vector1, ref FixedPointVector3 vector2)
        {
            var temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;
        }
        #endregion

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the multiplied vector.</returns>
        #region public static JVector Multiply(JVector value1, FP scaleFactor)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Multiply(FixedPointVector3 value1, FixedPoint64 scaleFactor)
        {
            Multiply(ref value1, scaleFactor, out var result);
            return result;
        }
        
        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the multiplied vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 Multiply(Vector3Int value1, FixedPoint64 scaleFactor)
        {
            var fixedPoint = new FixedPointVector3(value1);
            Multiply(ref fixedPoint, scaleFactor, out var result);
            return result;
        }

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the multiplied vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref FixedPointVector3 value1, FixedPoint64 scaleFactor, out FixedPointVector3 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
        }
        #endregion

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the cross product of both.</returns>
        #region public static JVector operator %(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 operator %(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Cross(ref value1, ref value2, out var result);
            return result;
        }
        #endregion

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the dot product of both.</returns>
        #region public static FP operator *(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 operator *(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            return FixedPointVector3.Dot(ref value1, ref value2);
        }
        #endregion

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value1">The vector to scale.</param>
        /// <param name="value2">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        #region public static JVector operator *(JVector value1, FP value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 operator *(FixedPointVector3 value1, FixedPoint64 value2)
        {
            FixedPointVector3.Multiply(ref value1, value2, out var result);
            return result;
        }
        #endregion

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value2">The vector to scale.</param>
        /// <param name="value1">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        #region public static JVector operator *(FP value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 operator *(FixedPoint64 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Multiply(ref value2, value1, out var result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        #region public static JVector operator -(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 operator -(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Subtract(ref value1, ref value2, out var result);
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 operator -(FixedPointVector3 value)
        {
            return new FixedPointVector3(- value.x, - value.y, - value.z);
        }
        #endregion

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        #region public static JVector operator +(JVector value1, JVector value2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 operator +(FixedPointVector3 value1, FixedPointVector3 value2)
        {
            FixedPointVector3.Add(ref value1, ref value2, out var result);
            return result;
        }
        #endregion

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="value2">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 operator /(FixedPointVector3 value1, FixedPoint64 value2)
        {
            Divide(ref value1, value2, out var result);
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 Angle(FixedPointVector3 a, FixedPointVector3 b)
        {
            return FixedPoint64.Acos(a.normalized * b.normalized) * FixedPoint64.Rad2Deg;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointVector2 ToTSVector2()
        {
            return new FixedPointVector2(x, y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 ToVector3()
        {
            return new Vector3(x.AsFloat(), y.AsFloat(), z.AsFloat());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(x.AsInt(), y.AsInt(), z.AsInt());
        }
    }
}