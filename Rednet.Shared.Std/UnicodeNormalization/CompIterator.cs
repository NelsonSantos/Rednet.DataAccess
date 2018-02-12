using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeNormalization
{
	internal class CompIterator : IEnumerator<UChar>
	{
		IEnumerator<UChar> it;
		Deque<UChar> procBuf = new Deque<UChar>();
		Deque<UChar> resBuf = new Deque<UChar>();
		int lastClass;

		public CompIterator(IEnumerator<UChar> it)
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
			while (this.resBuf.Count == 0)
			{
				if (!this.it.MoveNext())
				{
					this.resBuf = this.procBuf;
					this.procBuf = new Deque<UChar>();
					break;
				}
				var uchar = this.it.Current;
				if (this.procBuf.Count == 0)
				{
					this.lastClass = uchar.CanonicalClass;
					this.procBuf.AddBack(uchar);
				}
				else
				{
					var starter = this.procBuf.First();
					var composite = starter.GetComposite(uchar);
					var cc = uchar.CanonicalClass;
					if (composite != null && (this.lastClass < cc || this.lastClass == 0))
					{
						this.procBuf[0] = composite;
					}
					else
					{
						if (cc == 0)
						{
							this.resBuf = this.procBuf;
							this.procBuf = new Deque<UChar>();
						}
						this.lastClass = cc;
						this.procBuf.AddBack(uchar);
					}
				}
			}

			if (this.resBuf.Count == 0)
				return false;

			Current = this.resBuf.RemoveFront();
			return true;
		}

		public void Reset()
		{
			it.Reset();
			procBuf = new Deque<UChar>();
			resBuf = new Deque<UChar>();
		}

		public void Dispose()
		{
			it.Dispose();
			procBuf = null;
			resBuf = null;
		}
	}
}
