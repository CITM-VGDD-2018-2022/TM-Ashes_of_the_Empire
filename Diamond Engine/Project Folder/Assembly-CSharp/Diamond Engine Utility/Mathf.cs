using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DiamondEngine
{
    public static class Mathf
    {
        public static float Distance(Vector3 pointA, Vector3 pointB)
        {
            if (pointA == null || pointB == null)
            {
                //Add infinity someday
            }

            Vector3 distance = pointB - pointA;
            return distance.magnitude;
        }


        public static float Lerp(float from, float to, float t)
        {
            return (1.0f - t) * from + to * t;
        }

        //given 2 numbers and a number between them, inverse lerp returns a value between 0 and 1
        public static float InvLerp(float from,float to,float value)
        {
            return (value - from) / (to - from);
        }

        //remaps a value v between iMin & iMax to oMin, oMax.
        public static float Remap(float iMin,float iMax,float oMin, float oMax,float v)
        {
            float t = InvLerp(iMin, iMax, v);
            return Lerp(oMin,oMax,t);
        }
        //Same as remap but clamps to the nearest value
        public static float RemapClamp(float iMin, float iMax, float oMin, float oMax, float v)
        {
            float realMin = Math.Min(iMin, iMax);
            float realMax = Math.Max(iMin, iMax);
            float newV = Clamp(v, realMin, realMax);

            float t = InvLerp(iMin, iMax, newV);
            return Lerp(oMin, oMax, t);
        }

        public static float LerpAngle(float from, float to, float t)
        {
            float delta = Repeat((to - from), 360);
            if (delta > 180)
                delta -= 360;
            return from + delta * Clamp01(t);
        }


        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseInSine(float x)
        {
            return Convert.ToSingle(1 - Math.Cos((x * Math.PI) / 2.0f));
        }        

        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseInOutCirc(float x)
        {
            return x < 0.5  ? (float)(1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) * 0.5f  : (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) * 0.5f;
        }

        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseOutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }


        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseInCubic(float x)
        {
            return x * x * x;
        }

        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseOutCubic(float x)
        {
            return Convert.ToSingle(1 - Math.Pow(1 - x, 3));
        }


        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseInQuart(float x)
        {
            return x * x * x * x;
        }


        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseInQuint(float x)
        {
            return x * x * x * x * x;
        }


        //X Must be between 1 and 0 and will return values between 1 and 0
        public static float EaseOutQuint(float x)
        {
            return Convert.ToSingle(1 - Math.Pow(1 - 5, 5));
        }

        public static float Clamp01(float value)
        {
            if (value < 0f)
                return 0f;
            else if (value > 1f)
                return 1f;
            else
                return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static float Repeat(float t, float length)
        {
            return Clamp(t - Mathf.Floor(t / length) * length, 0.0f, length);
        }
        public static float Floor(float f) { return (float)Math.Floor(f); }

        public const float Rad2Deg = 57.29578f;

        public const float Deg2RRad = 0.01745f;

        public static void LookAt(ref Transform goTransform, Vector3 pointToLook, float slerpDegree = 1.0f)
        {
            Vector3 direction = pointToLook - goTransform.globalPosition;
            direction = direction.normalized;
            float angle = (float)Math.Atan2(direction.x, direction.z);

            if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
                return;


            Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

            if (slerpDegree > 1.0f) slerpDegree = 1.0f;
            if (slerpDegree < 0.0f) slerpDegree = 0.1f;
            Quaternion desiredRotation = Quaternion.Slerp(goTransform.localRotation, dir, slerpDegree);

            goTransform.localRotation = desiredRotation;

        }

        public static Vector2 RandomPointAround(Vector2 centerPoint, int range)
        {
            Random randomizer = new Random();

            int randomInt = randomizer.Next(-range, range);

            return new Vector2(centerPoint.x + randomInt, centerPoint.y + randomInt);
        }
    }


}

