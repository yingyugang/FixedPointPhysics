using BlueNoah.PhysicsEngine;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BlueNoah.Math.FixedPoint
{

    /// <summary>
    /// A Quaternion representing an orientation.
    /// </summary>
    [Serializable]
    public struct FixedPointQuaternion
    {

        /// <summary>The X component of the quaternion.</summary>
        public FixedPoint64 x;
        /// <summary>The Y component of the quaternion.</summary>
        public FixedPoint64 y;
        /// <summary>The Z component of the quaternion.</summary>
        public FixedPoint64 z;
        /// <summary>The W component of the quaternion.</summary>
        public FixedPoint64 w;

        public static readonly FixedPointQuaternion identity;

        static FixedPointQuaternion() {
            identity = new FixedPointQuaternion(0, 0, 0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the JQuaternion structure.
        /// </summary>
        /// <param name="x">The X component of the quaternion.</param>
        /// <param name="y">The Y component of the quaternion.</param>
        /// <param name="z">The Z component of the quaternion.</param>
        /// <param name="w">The W component of the quaternion.</param>
        public FixedPointQuaternion(FixedPoint64 x, FixedPoint64 y, FixedPoint64 z, FixedPoint64 w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public FixedPointQuaternion(QuaternionInt quaternionInt)
        {
            x = quaternionInt.x;
            y = quaternionInt.y;
            z = quaternionInt.z;
            w = quaternionInt.w;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }
        public void Set(FixedPoint64 new_x, FixedPoint64 new_y, FixedPoint64 new_z, FixedPoint64 new_w) {
            x = new_x;
            y = new_y;
            z = new_z;
            w = new_w;
        }

        public void SetFromToRotation(FixedPointVector3 fromDirection, FixedPointVector3 toDirection) {
            FixedPointQuaternion targetRotation = FixedPointQuaternion.FromToRotation(fromDirection, toDirection);
            this.Set(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);
        }

        public FixedPointVector3 eulerAngles {
            get {
                var result = new FixedPointVector3();

                var y_sqr = y * y;
                var t0 = -2.0f * (y_sqr + z * z) + 1.0f;
                var t1 = +2.0f * (x * y - w * z);
                var t2 = -2.0f * (x * z + w * y);
                var t3 = +2.0f * (y * z - w * x);
                var t4 = -2.0f * (x * x + y_sqr) + 1.0f;

                t2 = t2 > 1.0f ? 1.0f : t2;
                t2 = t2 < -1.0f ? -1.0f : t2;

                result.x = FixedPoint64.Atan2(t3, t4) * FixedPoint64.Rad2Deg;
                result.y = FixedPoint64.Asin(t2) * FixedPoint64.Rad2Deg;
                result.z = FixedPoint64.Atan2(t1, t0) * FixedPoint64.Rad2Deg;

                return result * -1;
            }
        }

        public static FixedPoint64 Angle(FixedPointQuaternion a, FixedPointQuaternion b) {
            FixedPointQuaternion aInv = FixedPointQuaternion.Inverse(a);
            FixedPointQuaternion f = b * aInv;

            FixedPoint64 angle = FixedPoint64.Acos(f.w) * 2 * FixedPoint64.Rad2Deg;

            if (angle > 180) {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)
        public static FixedPointQuaternion Add(FixedPointQuaternion quaternion1, FixedPointQuaternion quaternion2)
        {
            Add(ref quaternion1, ref quaternion2, out var result);
            return result;
        }

        public static FixedPointQuaternion LookRotation(FixedPointVector3 forward) {
            return CreateFromMatrix(FixedPointMatrix.LookAt(forward, FixedPointVector3.up));
        }

        public static FixedPointQuaternion LookRotation(FixedPointVector3 forward, FixedPointVector3 upwards) {
            return CreateFromMatrix(FixedPointMatrix.LookAt(forward, upwards));
        }

        public static FixedPointQuaternion Slerp(FixedPointQuaternion from, FixedPointQuaternion to, FixedPoint64 t) {
            t = FixedPointMath.Clamp(t, 0, 1);

            FixedPoint64 dot = Dot(from, to);

            if (dot < 0.0f) {
                to = Multiply(to, -1);
                dot = -dot;
            }

            FixedPoint64 halfTheta = FixedPoint64.Acos(dot);

            return Multiply(Multiply(from, FixedPoint64.Sin((1 - t) * halfTheta)) + Multiply(to, FixedPoint64.Sin(t * halfTheta)), 1 / FixedPoint64.Sin(halfTheta));
        }

        public static FixedPointQuaternion RotateTowards(FixedPointQuaternion from, FixedPointQuaternion to, FixedPoint64 maxDegreesDelta) {
            FixedPoint64 dot = Dot(from, to);

            if (dot < 0.0f) {
                to = Multiply(to, -1);
                dot = -dot;
            }

            FixedPoint64 halfTheta = FixedPoint64.Acos(dot);
            FixedPoint64 theta = halfTheta * 2;

            maxDegreesDelta *= FixedPoint64.Deg2Rad;

            if (maxDegreesDelta >= theta) {
                return to;
            }

            maxDegreesDelta /= theta;

            return Multiply(Multiply(from, FixedPoint64.Sin((1 - maxDegreesDelta) * halfTheta)) + Multiply(to, FixedPoint64.Sin(maxDegreesDelta * halfTheta)), 1 / FixedPoint64.Sin(halfTheta));
        }

        public static FixedPointQuaternion Euler(FixedPoint64 x, FixedPoint64 y, FixedPoint64 z) {
            x *= FixedPoint64.Deg2Rad;
            y *= FixedPoint64.Deg2Rad;
            z *= FixedPoint64.Deg2Rad;

            CreateFromYawPitchRoll(y, x, z, out var rotation);

            return rotation;
        }

        public static FixedPointQuaternion Euler(FixedPointVector3 eulerAngles) {
            return Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

        public static FixedPointQuaternion AngleAxis(FixedPoint64 angle, FixedPointVector3 axis) {
            axis = axis * FixedPoint64.Deg2Rad;
            axis.Normalize();

            FixedPoint64 halfAngle = angle * FixedPoint64.Deg2Rad * FixedPoint64.Half;

            FixedPointQuaternion rotation;
            FixedPoint64 sin = FixedPoint64.Sin(halfAngle);

            rotation.x = axis.x * sin;
            rotation.y = axis.y * sin;
            rotation.z = axis.z * sin;
            rotation.w = FixedPoint64.Cos(halfAngle);

            return rotation;
        }

        public static void CreateFromYawPitchRoll(FixedPoint64 yaw, FixedPoint64 pitch, FixedPoint64 roll, out FixedPointQuaternion result)
        {
            FixedPoint64 num9 = roll * FixedPoint64.Half;
            FixedPoint64 num6 = FixedPoint64.Sin(num9);
            FixedPoint64 num5 = FixedPoint64.Cos(num9);
            FixedPoint64 num8 = pitch * FixedPoint64.Half;
            FixedPoint64 num4 = FixedPoint64.Sin(num8);
            FixedPoint64 num3 = FixedPoint64.Cos(num8);
            FixedPoint64 num7 = yaw * FixedPoint64.Half;
            FixedPoint64 num2 = FixedPoint64.Sin(num7);
            FixedPoint64 num = FixedPoint64.Cos(num7);
            result.x = ((num * num4) * num5) + ((num2 * num3) * num6);
            result.y = ((num2 * num3) * num5) - ((num * num4) * num6);
            result.z = ((num * num3) * num6) - ((num2 * num4) * num5);
            result.w = ((num * num3) * num5) + ((num2 * num4) * num6);
        }

        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The sum of both quaternions.</param>
        public static void Add(ref FixedPointQuaternion quaternion1, ref FixedPointQuaternion quaternion2, out FixedPointQuaternion result)
        {
            result.x = quaternion1.x + quaternion2.x;
            result.y = quaternion1.y + quaternion2.y;
            result.z = quaternion1.z + quaternion2.z;
            result.w = quaternion1.w + quaternion2.w;
        }
        #endregion

        public static FixedPointQuaternion Conjugate(FixedPointQuaternion value)
        {
            FixedPointQuaternion quaternion;
            quaternion.x = -value.x;
            quaternion.y = -value.y;
            quaternion.z = -value.z;
            quaternion.w = value.w;
            return quaternion;
        }

        public static FixedPoint64 Dot(FixedPointQuaternion a, FixedPointQuaternion b) {
            return a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static FixedPointQuaternion Inverse(FixedPointQuaternion rotation) {
            FixedPoint64 invNorm = FixedPoint64.One / ((rotation.x * rotation.x) + (rotation.y * rotation.y) + (rotation.z * rotation.z) + (rotation.w * rotation.w));
            return FixedPointQuaternion.Multiply(FixedPointQuaternion.Conjugate(rotation), invNorm);
        }

        public static FixedPointQuaternion FromToRotation(FixedPointVector3 fromVector, FixedPointVector3 toVector) {
            FixedPointVector3 w = FixedPointVector3.Cross(fromVector, toVector);
            FixedPointQuaternion q = new FixedPointQuaternion(w.x, w.y, w.z, FixedPointVector3.Dot(fromVector, toVector));
            q.w += FixedPoint64.Sqrt(fromVector.sqrMagnitude * toVector.sqrMagnitude);
            q.Normalize();

            return q;
        }

        public static FixedPointQuaternion Lerp(FixedPointQuaternion a, FixedPointQuaternion b, FixedPoint64 t) {
            t = FixedPointMath.Clamp(t, FixedPoint64.Zero, FixedPoint64.One);

            return LerpUnclamped(a, b, t);
        }

        public static FixedPointQuaternion LerpUnclamped(FixedPointQuaternion a, FixedPointQuaternion b, FixedPoint64 t) {
            FixedPointQuaternion result = FixedPointQuaternion.Multiply(a, (1 - t)) + FixedPointQuaternion.Multiply(b, t);
            result.Normalize();

            return result;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)
        public static FixedPointQuaternion Subtract(FixedPointQuaternion quaternion1, FixedPointQuaternion quaternion2)
        {
            Subtract(ref quaternion1, ref quaternion2, out var result);
            return result;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The difference of both quaternions.</param>
        public static void Subtract(ref FixedPointQuaternion quaternion1, ref FixedPointQuaternion quaternion2, out FixedPointQuaternion result)
        {
            result.x = quaternion1.x - quaternion2.x;
            result.y = quaternion1.y - quaternion2.y;
            result.z = quaternion1.z - quaternion2.z;
            result.w = quaternion1.w - quaternion2.w;
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)
        public static FixedPointQuaternion Multiply(FixedPointQuaternion quaternion1, FixedPointQuaternion quaternion2)
        {
            Multiply(ref quaternion1, ref quaternion2, out var result);
            return result;
        }

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The product of both quaternions.</param>
        public static void Multiply(ref FixedPointQuaternion quaternion1, ref FixedPointQuaternion quaternion2, out FixedPointQuaternion result)
        {
            FixedPoint64 x = quaternion1.x;
            FixedPoint64 y = quaternion1.y;
            FixedPoint64 z = quaternion1.z;
            FixedPoint64 w = quaternion1.w;
            FixedPoint64 num4 = quaternion2.x;
            FixedPoint64 num3 = quaternion2.y;
            FixedPoint64 num2 = quaternion2.z;
            FixedPoint64 num = quaternion2.w;
            FixedPoint64 num12 = (y * num2) - (z * num3);
            FixedPoint64 num11 = (z * num4) - (x * num2);
            FixedPoint64 num10 = (x * num3) - (y * num4);
            FixedPoint64 num9 = ((x * num4) + (y * num3)) + (z * num2);
            result.x = ((x * num) + (num4 * w)) + num12;
            result.y = ((y * num) + (num3 * w)) + num11;
            result.z = ((z * num) + (num2 * w)) + num10;
            result.w = (w * num) - num9;
        }
        #endregion

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>The scaled quaternion.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, FP scaleFactor)
        public static FixedPointQuaternion Multiply(FixedPointQuaternion quaternion1, FixedPoint64 scaleFactor)
        {
            Multiply(ref quaternion1, scaleFactor, out var result);
            return result;
        }

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <param name="result">The scaled quaternion.</param>
        public static void Multiply(ref FixedPointQuaternion quaternion1, FixedPoint64 scaleFactor, out FixedPointQuaternion result)
        {
            result.x = quaternion1.x * scaleFactor;
            result.y = quaternion1.y * scaleFactor;
            result.z = quaternion1.z * scaleFactor;
            result.w = quaternion1.w * scaleFactor;
        }
        #endregion

        /// <summary>
        /// Sets the length of the quaternion to one.
        /// </summary>
        #region public void Normalize()
        public void Normalize()
        {
            FixedPoint64 num2 = (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z)) + (this.w * this.w);
            FixedPoint64 num = 1 / (FixedPoint64.Sqrt(num2));
            this.x *= num;
            this.y *= num;
            this.z *= num;
            this.w *= num;
        }
        #endregion

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <returns>JQuaternion representing an orientation.</returns>
        #region public static JQuaternion CreateFromMatrix(JMatrix matrix)
        public static FixedPointQuaternion CreateFromMatrix(FixedPointMatrix matrix)
        {
            CreateFromMatrix(ref matrix, out var result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <param name="result">JQuaternion representing an orientation.</param>
        public static void CreateFromMatrix(ref FixedPointMatrix matrix, out FixedPointQuaternion result)
        {
            FixedPoint64 num8 = (matrix.M11 + matrix.M22) + matrix.M33;
            if (num8 > FixedPoint64.Zero)
            {
                FixedPoint64 num = FixedPoint64.Sqrt((num8 + FixedPoint64.One));
                result.w = num * FixedPoint64.Half;
                num = FixedPoint64.Half / num;
                result.x = (matrix.M23 - matrix.M32) * num;
                result.y = (matrix.M31 - matrix.M13) * num;
                result.z = (matrix.M12 - matrix.M21) * num;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                FixedPoint64 num7 = FixedPoint64.Sqrt((((FixedPoint64.One + matrix.M11) - matrix.M22) - matrix.M33));
                FixedPoint64 num4 = FixedPoint64.Half / num7;
                result.x = FixedPoint64.Half * num7;
                result.y = (matrix.M12 + matrix.M21) * num4;
                result.z = (matrix.M13 + matrix.M31) * num4;
                result.w = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                FixedPoint64 num6 = FixedPoint64.Sqrt((((FixedPoint64.One + matrix.M22) - matrix.M11) - matrix.M33));
                FixedPoint64 num3 = FixedPoint64.Half / num6;
                result.x = (matrix.M21 + matrix.M12) * num3;
                result.y = FixedPoint64.Half * num6;
                result.z = (matrix.M32 + matrix.M23) * num3;
                result.w = (matrix.M31 - matrix.M13) * num3;
            }
            else
            {
                FixedPoint64 num5 = FixedPoint64.Sqrt((((FixedPoint64.One + matrix.M33) - matrix.M11) - matrix.M22));
                FixedPoint64 num2 = FixedPoint64.Half / num5;
                result.x = (matrix.M31 + matrix.M13) * num2;
                result.y = (matrix.M32 + matrix.M23) * num2;
                result.z = FixedPoint64.Half * num5;
                result.w = (matrix.M12 - matrix.M21) * num2;
            }
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static FP operator *(JQuaternion value1, JQuaternion value2)
        public static FixedPointQuaternion operator *(FixedPointQuaternion value1, FixedPointQuaternion value2)
        {
            Multiply(ref value1, ref value2,out var result);
            return result;
        }
        #endregion

        /// <summary>
        /// Add two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static FP operator +(JQuaternion value1, JQuaternion value2)
        public static FixedPointQuaternion operator +(FixedPointQuaternion value1, FixedPointQuaternion value2)
        {
            Add(ref value1, ref value2, out var result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtract two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static FP operator -(JQuaternion value1, JQuaternion value2)
        public static FixedPointQuaternion operator -(FixedPointQuaternion value1, FixedPointQuaternion value2)
        {
            Subtract(ref value1, ref value2, out var result);
            return result;
        }
        #endregion

        public static FixedPointVector3 operator *(FixedPointQuaternion quat, FixedPointVector3 vec) {
            var num = quat.x * 2f;
            var num2 = quat.y * 2f;
            var num3 = quat.z * 2f;
            var num4 = quat.x * num;
            var num5 = quat.y * num2;
            var num6 = quat.z * num3;
            var num7 = quat.x * num2;
            var num8 = quat.x * num3;
            var num9 = quat.y * num3;
            var num10 = quat.w * num;
            var num11 = quat.w * num2;
            var num12 = quat.w * num3;

            FixedPointVector3 result;
            result.x = (1f - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
            result.y = (num7 + num12) * vec.x + (1f - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
            result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (1f - (num4 + num5)) * vec.z;

            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointQuaternion operator /(FixedPointQuaternion value1, FixedPoint64 value2)
        {
            return new FixedPointQuaternion(value1.x / value2, value1.y / value2, value1.z / value2, value1.w / value2);
        }

        public override string ToString() {
            return $"({x.AsFloat():f1}, {y.AsFloat():f1}, {z.AsFloat():f1}, {w.AsFloat():f1})";
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
        }
    }
}
