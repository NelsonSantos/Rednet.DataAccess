using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeNormalization
{
	public enum NormalizationForm
	{
		FormC,
		FormD,
		FormKC,
		FormKD,
	}

	public static class UNorm
	{

		private static IEnumerator<UChar> CreateIterator(NormalizationForm mode, string str)
		{
			switch (mode)
			{
				case NormalizationForm.FormD:
					return new DecompIterator(new RecursDecompIterator(new UCharIterator(str), true));
				case NormalizationForm.FormKD:
					return new DecompIterator(new RecursDecompIterator(new UCharIterator(str), false));
				case NormalizationForm.FormC:
					return new CompIterator(new DecompIterator(new RecursDecompIterator(new UCharIterator(str), true)));
				case NormalizationForm.FormKC:
					return new CompIterator(new DecompIterator(new RecursDecompIterator(new UCharIterator(str), false)));
			}
			throw new ArgumentException("Invalid form");
		}

		public static string Normalize(this string str, NormalizationForm mode = NormalizationForm.FormC)
		{
			var it = CreateIterator(mode, str);
			StringBuilder ret = new StringBuilder();
			while (it.MoveNext())
			{
				ret.Append(it.Current);
			}
			return ret.ToString();
		}
	}
}
