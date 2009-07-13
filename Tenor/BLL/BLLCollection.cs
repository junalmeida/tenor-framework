using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
    namespace BLL
    {
        /// <summary>
        /// Represents a collection of entities.
        /// </summary>
        /// <typeparam name="T">A Type that inherits directly or indirectly from BLLBase.</typeparam>
        [Serializable()]
        public class BLLCollection<T> : ICollection<T>, IList<T>, ICollection, IList where T : BLLBase
        {
            public int Count
            {
                get { return list.Count; }
            }

            int ICollection.Count
            {
                get
                {
                    return this.Count;
                }
            }


            public Type ElementType
            {
                get
                {
                    return typeof(T);
                }
            }

            [NonSerialized()]
            private BLLBase _Parent;
            [NonSerialized()]
            private FieldInfo[] ForeignFields;
            [NonSerialized()]
            private FieldInfo[] LocalFields;

            internal BLLCollection(BLLBase Parent, string RelatedPropertyName)
            {
                Init(Parent, RelatedPropertyName, null);
            }

            internal BLLCollection(BLLBase Parent, string RelatedPropertyName, Type RelatedPropertyType)
            {
                Init(Parent, RelatedPropertyName, RelatedPropertyType);
            }

            protected void Init(BLLBase Parent, string RelatedPropertyName, Type RelatedPropertyType)
            {
                _Parent = Parent;

                System.Reflection.PropertyInfo prop;
                if (RelatedPropertyType != null)
                {
                    prop = Parent.GetType().GetProperty(RelatedPropertyName, RelatedPropertyType);
                }
                else
                {
                    prop = Parent.GetType().GetProperty(RelatedPropertyName);
                }
                ForeignKeyInfo fkInfo = ForeignKeyInfo.Create(prop);
                if (fkInfo == null)
                    throw new MissingForeignKeyException(prop.DeclaringType, prop.Name);

                ForeignFields = fkInfo.ForeignFields;
                LocalFields = fkInfo.LocalFields;

                list = new List<T>();
            }

            List<T> list;

            public void Add(T item)
            {
                list.Add(item);

                for (int i = 0; i <= ForeignFields.Length - 1; i++)
                {
                    ForeignFields[i].SetPropertyValue(item, LocalFields[i].PropertyValue(_Parent));
                }
            }

            public void AddRange(IEnumerable<T> items)
            {
                foreach (T i in items)
                {
                    Add(i);
                }
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(T item)
            {
                return list.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }



            //			public int Count
            //			{
            //				get
            //				{
            //					return list.Count;
            //				}
            //			}

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public bool Remove(T item)
            {
                try
                {
                    list.Remove(item);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public System.Collections.Generic.IEnumerator<T> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public int IndexOf(T item)
            {
                return list.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                list.Insert(index, item);
            }

            public T this[int index]
            {
                get
                {
                    return list[index];
                }
                set
                {
                    list[index] = value;
                }
            }

            public void RemoveAt(int index)
            {
                list.RemoveAt(index);
            }

            //			public System.Collections.IEnumerator GetEnumerator()
            //			{
            //				return this.GetEnumerator1();
            //			}

            IEnumerator IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public T[] ToArray()
            {
                return list.ToArray();
            }


            public void CopyTo(System.Array array, int index)
            {
                list.CopyTo(((T[])array), index);
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
                    return null;
                }
            }

            #region " Find Functions "

            private void SetFindDefinitions(string PropertyExpression, object Value, Tenor.Data.CompareOperator @Operator)
            {
                SetFindDefinitions(new string[] { PropertyExpression }, new object[] { Value }, new Tenor.Data.CompareOperator[] { @Operator }, new Tenor.Data.LogicalOperator[] { });
            }

            private void SetFindDefinitions(string[] PropertyExpression, object[] Value, Tenor.Data.CompareOperator[] @Operator, Tenor.Data.LogicalOperator[] Logical)
            {
                _findProp = PropertyExpression;
                _findValue = Value;
                _findOperator = @Operator;
                _findLogical = Logical;

            }


            /// <summary>
            /// Searchs for an especific entity on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">A value to be compared.</param>
            /// <returns>The first entity found.</returns>
            public T Find(string propertyExpression, object value)
            {
                return Find(propertyExpression, value, Tenor.Data.CompareOperator.Equal);
            }


            /// <summary>
            /// Searchs for an especific entity on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">A value to be compared.</param>
            /// <param name="compareOperator">The compare operator to use against the property.</param>
            /// <returns>The first entity found.</returns>
            public T Find(string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
            {
                SetFindDefinitions(propertyExpression, value, compareOperator);
                return list.Find(new Predicate<T>(FindDelegate));
            }

            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">A value to be compared.</param>
            /// <returns>An array of entities found.</returns>
            public T[] FindAll(string propertyExpression, object value)
            {
                return FindAll(propertyExpression, value, Tenor.Data.CompareOperator.Equal);
            }

            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">A value to be compared.</param>
            /// <param name="compareOperator">The compare operator to use against the property.</param>
            /// <returns>An array of entities found.</returns>
            public T[] FindAll(string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
            {
                SetFindDefinitions(propertyExpression, value, compareOperator);
                return list.FindAll(new Predicate<T>(FindDelegate)).ToArray();
            }


            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpressions">An array of property names used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="values">An array of values to be compared.</param>
            /// <param name="compareOperators">An Array of compare operators to use against the properties.</param>
            /// <param name="logicalOperators">An array of logical operators used to join the comparisons. Notice that this array must have one item less than the other arrays.</param>
            /// <returns>An array of entities found.</returns>
            public T[] FindAll(string[] propertyExpressions, object[] values, Tenor.Data.CompareOperator[] compareOperators, Tenor.Data.LogicalOperator[] logicalOperators)
            {
                SetFindDefinitions(propertyExpressions, values, compareOperators, logicalOperators);
                return list.FindAll(new Predicate<T>(FindDelegate)).ToArray();
            }



            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">The value desired.</param>
            /// <returns>The index of the first found entity.</returns>
            public int FindIndex(string propertyExpression, object value)
            {
                return FindIndex(propertyExpression, value, Tenor.Data.CompareOperator.Equal);
            }

            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">The value desired.</param>
            /// <param name="compareOperator">The compare operator to use against the property.</param>
            /// <returns>The index of the first found entity.</returns>
            public int FindIndex(string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
            {
                SetFindDefinitions(propertyExpression, value, compareOperator);
                return list.FindIndex(new Predicate<T>(FindDelegate));
            }

            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">The value desired.</param>
            /// <returns>The last found entity.</returns>
            public T FindLast(string propertyExpression, object value)
            {
                return FindLast(propertyExpression, value, Tenor.Data.CompareOperator.Equal);
            }

            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">The value desired.</param>
            /// <param name="compareOperator">The compare operator to use against the property.</param>
            /// <returns>The last found entity.</returns>
            public T FindLast(string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
            {
                SetFindDefinitions(propertyExpression, value, compareOperator);
                return list.FindLast(new Predicate<T>(FindDelegate));
            }



            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">The value desired.</param>
            /// <returns>The index of the last found entity.</returns>
            public int FindLastIndex(string propertyExpression, object value)
            {
                return FindLastIndex(propertyExpression, value, Tenor.Data.CompareOperator.Equal);
            }



            /// <summary>
            /// Searchs for entities on this collection.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <param name="value">The value desired.</param>
            /// <param name="compareOperator">The compare operator to use against the property.</param>
            /// <returns>The index of the last found entity.</returns>
            public int FindLastIndex(string propertyExpression, object value, Tenor.Data.CompareOperator compareOperator)
            {
                SetFindDefinitions(propertyExpression, value, compareOperator);
                return list.FindLastIndex(new Predicate<T>(FindDelegate));
            }


            internal static string[] _findProp;
            internal static object[] _findValue;
            internal static Tenor.Data.CompareOperator[] _findOperator;
            internal static Tenor.Data.LogicalOperator[] _findLogical;


            private static bool TestValue(T obj, string _findProp, object _findValue, Tenor.Data.CompareOperator _findOperator)
            {
                object valor = Util.GetValue(_findProp, obj);
                bool returnValue = false;

                switch (_findOperator)
                {
                    case Tenor.Data.CompareOperator.Equal:
                    case Tenor.Data.CompareOperator.NotEqual:

                        if (valor == null && _findValue == null)
                        {
                            returnValue = true;
                        }
                        else if (valor == null || _findValue == null)
                        {
                            returnValue = false;
                        }
                        else
                        {
                            if (valor.GetType().IsValueType)
                            {
                                returnValue = object.Equals(valor, _findValue);
                            }
                            else if (valor.GetType() == typeof(string))
                            {
                                returnValue = string.Equals(valor, _findValue);
                            }
                            else
                            {
                                returnValue = valor == _findValue;
                            }
                        }

                        if (_findOperator == Tenor.Data.CompareOperator.NotEqual)
                        {
                            returnValue = !returnValue;
                        }
                        break;

                    default:
                        throw (new ArgumentException());
                }
                return returnValue;
            }

            internal static bool FindDelegate(T obj)
            {
                bool? res = null;

                if (_findProp.Length == 0 || _findProp.Length != _findValue.Length || _findOperator.Length != _findProp.Length || _findLogical.Length < _findProp.Length - 1)
                {
                    throw (new ArgumentException("Invalid conditions. Check arrays of Properties, values and operators"));
                }
                else
                {

                    for (int i = 0; i <= _findProp.Length - 1; i++)
                    {
                        bool test = TestValue(obj, _findProp[i], _findValue[i], _findOperator[i]);
                        if (!res.HasValue)
                        {
                            res = test;
                        }
                        else
                        {
                            if (_findLogical[i - 1] == Tenor.Data.LogicalOperator.And)
                            {
                                res = res.Value && test;
                            }
                            else if (_findLogical[i - 1] == Tenor.Data.LogicalOperator.Or)
                            {
                                res = res.Value || test;
                            }
                        }
                    }
                }


                return res.Value;
            }
            #endregion

            #region " Sort Functions "

            /// <summary>
            /// Sorts entities on this list.
            /// </summary>
            /// <param name="propertyExpression">The property name used to match entities. You can use dot (.) separated properties to go down on the class.</param>
            /// <remarks></remarks>
            public void Sort(string propertyExpression)
            {
                list.Sort(new BLLBaseComparer(propertyExpression));
            }

            private class BLLBaseComparer : IComparer<T>
            {

                private string _PropertyExpression;

                public BLLBaseComparer(string PropertyExpression)
                {
                    //SetCompareDefinitions(PropertyExpression)
                    _PropertyExpression = PropertyExpression;
                }

                public int Compare(T x, T y)
                {
                    return Util.Compare(x, y, _PropertyExpression);
                }
            }

            #endregion

            #region IList Members

            int IList.Add(object value)
            {
                this.Add((T)value);
                return this.IndexOf((T)value);
            }

            void IList.Clear()
            {
                this.Clear();
            }

            bool IList.Contains(object value)
            {
                return this.Contains((T)value);
            }

            int IList.IndexOf(object value)
            {
                return this.IndexOf((T)value);
            }

            void IList.Insert(int index, object value)
            {
                this.Insert(index, (T)value);
            }

            bool IList.IsFixedSize
            {
                get { return false; }
            }

            bool IList.IsReadOnly
            {
                get { return this.IsReadOnly; }
            }

            void IList.Remove(object value)
            {
                throw new NotImplementedException();
            }

            void IList.RemoveAt(int index)
            {
                this.RemoveAt(index);
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    this[index] = (T)value;
                }
            }

            #endregion

            #region ICollection Members

            void ICollection.CopyTo(Array array, int index)
            {
                this.CopyTo(array, index);
            }

            bool ICollection.IsSynchronized
            {
                get { return this.IsSynchronized; }
            }

            object ICollection.SyncRoot
            {
                get { return this.SyncRoot; }
            }

            #endregion
        }


    }

}
