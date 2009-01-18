using System.Diagnostics;
using System;
using System.Collections;
using Microsoft.VisualBasic;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace Data
	{
		internal sealed class DummyDataSource : System.Collections.ICollection, System.Collections.IEnumerable
		{
			
			// Methods
			internal DummyDataSource(int dataItemCount)
			{
				this.dataItemCount = dataItemCount;
			}
			
			public void CopyTo(Array array, int index)
			{
				IEnumerator enumerator = this.GetEnumerator();
				while (enumerator.MoveNext())
				{
					array.SetValue(enumerator.Current, index);
					index++;
				}
			}
			
			public IEnumerator GetEnumerator()
			{
				return new DummyDataSourceEnumerator(this.dataItemCount);
			}
			
			
			// Properties
			public int Count
			{
				get
				{
					return this.dataItemCount;
				}
			}
			
			public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
			
			public object SyncRoot
			{
				get
				{
					return this;
				}
			}
			
			
			// Fields
			private int dataItemCount;
			
			private class DummyDataSourceEnumerator : System.Collections.IEnumerator
			{
				
				
				// Methods
				public DummyDataSourceEnumerator(int count)
				{
					this.count = count;
					this.index = - 1;
				}
				
				public bool MoveNext()
				{
					this.index++;
					return (this.index < this.count);
				}
				
				public void Reset()
				{
					this.index = - 1;
				}
				
				
				// Properties
				public object Current
				{
					get
					{
						return null;
					}
				}
				
				
				// Fields
				private int count;
				private int index;
				
				
				
			}
		}
	}
	
}
