using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeNormalization
{
	internal class DecompIterator : IEnumerator<UChar>
	{
		IEnumerator<UChar> it;
		Deque<UChar> resBuf = new Deque<UChar>();

		public DecompIterator(IEnumerator<UChar> it)
		{
			this.it = it;
		}

		public UChar Current { get; private set; }

		object System.Collections.IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			int cc;
			if (this.resBuf.Count == 0)
			{
				do
				{
					if (!this.it.MoveNext())
						break;

					var uchar = it.Current;
					cc = uchar.CanonicalClass;
					var inspt = this.resBuf.Count;
					if (cc != 0)
					{
						for (; inspt > 0; --inspt)
						{
							var uchar2 = this.resBuf[inspt - 1];
							var cc2 = uchar2.CanonicalClass;
							if (cc2 <= cc)
							{
								break;
							}
						}
					}
					this.resBuf.Insert(inspt, uchar);
				}
				while (cc != 0);
			}

			if (this.resBuf.Count == 0)
				return false;

			Current = this.resBuf.RemoveFront();
			return true;
		}

		public void Reset()
		{
			it.Reset();
			resBuf = new Deque<UChar>();
		}

		public void Dispose()
		{
			it.Dispose();
			resBuf = null;
		}
	}
}
