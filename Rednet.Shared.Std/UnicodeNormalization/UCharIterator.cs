using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeNormalization
{
	internal class UCharIterator : IEnumerator<UChar>
	{
		int cursor = 0;
		string Str;

		public UCharIterator(string str)
		{
			this.Str = str;
		}

		public UChar Current { get; private set; }

		object System.Collections.IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			if (this.Str != null && this.cursor < this.Str.Length)
			{
				int cp = (int)this.Str[this.cursor++];
				int d;
				if (UChar.IsHighSurrogate(cp) && this.cursor < this.Str.Length && UChar.IsLowSurrogate(d = (int)this.Str[this.cursor]))
				{
					cp = (cp - 0xD800) * 0x400 + (d - 0xDC00) + 0x10000;
					++this.cursor;
				}
				Current = UChar.FromCharCode(cp);
				return true;
			}
			else
			{
				this.Str = null;
				return false;
			}
		}

		public void Reset()
		{
			cursor = 0;
		}

		public void Dispose()
		{
			this.Str = null;
		}
	}
}
