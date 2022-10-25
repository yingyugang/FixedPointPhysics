using System;

namespace BlueNoah
{
	/// <summary>
	/// Xorshiftのアルゴリズムで乱数
	/// </summary>
	public class Xorshift
	{
		// 内部メモリ
		private UInt32 x;
		private UInt32 y;
		private UInt32 z;
		private UInt32 w;

		public static Action<uint> onRandom;

		static Xorshift instance;
		/// <summary>
		/// 初期化
		/// </summary>
		public static void SetSeed(UInt32 seed)
		{
			instance = new Xorshift(seed);
		}
		/// <summary>
		/// 乱数生成
		/// </summary>
		public static uint Random1000()
		{
			var random = instance.Next() % 1000;
			onRandom?.Invoke(random);
			return random;
		}

		public static int Random(int min,int max)
        {
            if (min == max)
            {
				return min;
            }
			var random = instance.Next() % (max - min) + min;
			onRandom?.Invoke((uint)random);
			return (int)random ;
		}

		Xorshift() : this((UInt32)DateTime.Now.Ticks) { }

		Xorshift(UInt32 seed)
		{
			x = 123456789U;
			y = 362436069U;
			z = 521288629U;
			w = seed;
		}

		UInt32 Next()
		{
			UInt32 t = x ^ (x << 11);
			x = y;
			y = z;
			z = w;
			w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
			return w;
		}
	}
}