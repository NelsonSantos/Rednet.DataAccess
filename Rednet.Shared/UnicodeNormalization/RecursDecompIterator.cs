using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeNormalization
{
	internal class RecursDecompIterator : IEnumerator<UChar>
	{
		IEnumerator<UChar> it;
		bool canonical;
		Queue<UChar> resBuf = new Queue<UChar>();

		public RecursDecompIterator(IEnumerator<UChar> it, bool canonical)
		{
			this.it = it;
			this.canonical = canonical;
		}

		public UChar Current { get; private set; }

		object System.Collections.IEnumerator.Current
		{
			get { return Current; }
		}

		Queue<UChar> RecursiveDecomp(bool cano, UChar uchar)
		{
			var decomp = uchar.Decomp;
			var ret = new Queue<UChar>();
			if (decomp != null && !(cano && uchar.IsCompatibility))
			{
				for (var i = 0; i < decomp.Length; ++i)
				{
					var a = RecursiveDecomp(cano, UChar.FromCharCode(decomp[i]));
					foreach (var item in a)
					{
						ret.Enqueue(item);
					}
				}
				return ret;
			}
			else
			{
				ret.Enqueue(uchar);
				return ret;
			}
		}

		public bool MoveNext()
		{
			if (this.resBuf.Count == 0)
			{
				if (!it.MoveNext())
					return false;

				this.resBuf = RecursiveDecomp(this.canonical, it.Current);
			}
			Current = this.resBuf.Dequeue();
			return true;
		}

		public void Reset()
		{
			it.Reset();
			resBuf = new Queue<UChar>();
		}

		public void Dispose()
		{
			it.Dispose();
			resBuf = null;
		}
	}
}
