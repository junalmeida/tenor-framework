using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Tenor.Data
{
    internal sealed class DummyDataSource : System.Collections.ICollection, System.Collections.IEnumerable
    {

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


        private int dataItemCount;

        private class DummyDataSourceEnumerator : System.Collections.IEnumerator
        {


            public DummyDataSourceEnumerator(int count)
            {
                this.count = count;
                this.index = -1;
            }

            public bool MoveNext()
            {
                this.index++;
                return (this.index < this.count);
            }

            public void Reset()
            {
                this.index = -1;
            }


            public object Current
            {
                get
                {
                    return null;
                }
            }
            private int count;
            private int index;
        }
    }
}