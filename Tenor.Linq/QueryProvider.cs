using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Tenor.Linq
{
    /// <summary>
    /// The Tenor Query provider. This class holds all LINQ mapping logic.
    /// </summary>
    internal class TenorQueryProvider : IQueryProvider
    {

        internal TenorQueryProvider()
        {

        }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            Type elementType = typeof(T);
            return (IQueryable<T>)Activator.CreateInstance(typeof(SearchOptions<>).MakeGenericType(elementType), new object[] { this, expression });
        }



        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = expression.Type;

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(SearchOptions<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }



        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)this.Execute(expression);
        }



        private string GetQueryText(Expression expression)
        {
            throw new NotImplementedException();
        }

        Tenor.Data.SearchOptions searchOptions;

        public object Execute(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Call)
                throw new InvalidOperationException();
            MethodCallExpression exp = (MethodCallExpression)expression;



            bool doCount = false;

            Type type = expression.Type;
            if (type == typeof(Int32))
                doCount = true;


            type = exp.Method.GetGenericArguments()[0];
            if (!type.IsSubclassOf(typeof(BLL.BLLBase)))
                throw new InvalidCastException("Expression type is invalid.");


            searchOptions = new Tenor.Data.SearchOptions(type);

            ReadExpressions(expression);

            //Tenor.Data.ISearchOptions linqSo =((MethodCallExpression)
            //if (linqSo != null)
            //{
            //    //this is the base SearchOptions query;
            //    Tenor.Data.ISearchOptions realSo = (Tenor.Data.ISearchOptions)so;

            //    realSo.Distinct = linqSo.Distinct;
            //    realSo.LazyLoading = linqSo.LazyLoading;
            //    realSo.Top = linqSo.Top;
            //}

            try
            {
                if (doCount)
                    return searchOptions.ExecuteCount();
                else
                    return searchOptions.Execute();
            }
            catch
            {
                throw;
            }
            finally
            {
                searchOptions = null;
            }
        }



        private void ReadExpressions(Expression ex)
        {
            switch (ex.NodeType)
            {
                case ExpressionType.Call:
                    {
                        MethodCallExpression mce = (MethodCallExpression)ex;

                        switch (mce.Method.Name)
                        {
                            /* LINQ Methods */
                            case "Where":
                                //the where clause.
                                ReadWhereExpressions(searchOptions.Conditions, mce.Arguments[1], false);
                                break;
                            case "OrderBy":
                                ReadOrderByExpressions(mce.Arguments[1], true);
                                break;
                            case "OrderByDescending":
                                ReadOrderByExpressions(mce.Arguments[1], false);
                                break;
                            case "Take":
                                //the TOP/LIMIT function.
                                int top = (int)((ConstantExpression)mce.Arguments[1]).Value;
                                searchOptions.Top = top;
                                break;
                            case "Distinct":
                                searchOptions.Distinct = true;
                                break;
                            case "Count":
                                //just pass it on
                                break;
                            /* END LINQ */
                            default:
                                throw new NotImplementedException("Linq method call to '" + mce.Method.Name.ToString() + "' is not implemented. Please, send a feature request.");
                        }
                        //continue recursively
                        ReadExpressions(mce.Arguments[0]);
                    }
                    break;
                case ExpressionType.Constant:
                    {
                        //TODO: Check if we need to check constants.
                        ConstantExpression cex = (ConstantExpression)ex;

                    }
                    break;
                default:
                    throw new NotImplementedException("Linq '" + ex.NodeType.ToString() + "' is not implemented. Please, send a feature request.");
                    break;
            }
        }


        private void ReadWhereExpressions(Tenor.Data.ConditionCollection cc, Expression ex, bool not)
        {
            switch (ex.NodeType)
            {
                case ExpressionType.Quote:
                    {
                        UnaryExpression une = ex as UnaryExpression;
                        if (une.IsLifted || une.IsLiftedToNull)
                            throw new NotImplementedException();

                        ReadWhereExpressions(cc, une.Operand, false);

                    }
                    break;
                case ExpressionType.Lambda:
                    {
                        LambdaExpression lex = ex as LambdaExpression;
                        //TODO: Should we check parameters?
                        ReadWhereExpressions(cc, lex.Body, false);
                    }
                    break;
                case ExpressionType.Not:
                    {
                        UnaryExpression une = ex as UnaryExpression;
                        if (une.IsLifted || une.IsLiftedToNull)
                            throw new NotImplementedException();

                        ReadWhereExpressions(cc, une.Operand, true);
                    }
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    {
                        BinaryExpression andBinary = ex as BinaryExpression;

                        Tenor.Data.ConditionCollection newCc = new Tenor.Data.ConditionCollection();

                        ReadWhereExpressions(newCc, andBinary.Left, false);
                        newCc.Add(Tenor.Data.LogicalOperator.And);
                        ReadWhereExpressions(newCc, andBinary.Right, false);

                        cc.Add(newCc);
                    }
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    {
                        BinaryExpression andBinary = ex as BinaryExpression;

                        Tenor.Data.ConditionCollection newCc = new Tenor.Data.ConditionCollection();

                        ReadWhereExpressions(newCc, andBinary.Left, false);
                        newCc.Add(Tenor.Data.LogicalOperator.Or);
                        ReadWhereExpressions(newCc, andBinary.Right, false);

                        cc.Add(newCc);
                    }
                    break;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    {
                        BinaryExpression bex = ex as BinaryExpression;


                        MemberExpression left = bex.Left as MemberExpression;
                        if (left == null)
                            left = bex.Right as MemberExpression;
                        if (left == null)
                            throw new NotImplementedException("Not implemeted lambda expression.");
                        ConstantExpression right = bex.Right as ConstantExpression;
                        if (right == null)
                            right = bex.Left as ConstantExpression;
                        if (right == null)
                            throw new NotImplementedException("Not implemeted lambda expression.");

                        Tenor.Data.CompareOperator op = Tenor.Data.CompareOperator.Equal;
                        if (ex.NodeType == ExpressionType.Equal && !not)
                            op = Tenor.Data.CompareOperator.Equal;
                        else if (ex.NodeType == ExpressionType.Equal && not)
                            op = Tenor.Data.CompareOperator.NotEqual;
                        else if (ex.NodeType == ExpressionType.NotEqual && !not)
                            op = Tenor.Data.CompareOperator.NotEqual;
                        else if (ex.NodeType == ExpressionType.NotEqual && not)
                            op = Tenor.Data.CompareOperator.Equal;

                        cc.Add(left.Member.Name, right.Value, op);
                    }
                    break;
                case ExpressionType.MemberAccess:
                    {
                        MemberExpression mex = ex as MemberExpression;
                        if (mex.Type != typeof(bool) && (!mex.Type.IsGenericType || mex.Type != typeof(bool?)))
                            throw new InvalidOperationException("Invalid lambda expression");

                        Tenor.Data.CompareOperator op = Tenor.Data.CompareOperator.Equal;
                        if (not)
                            op = Tenor.Data.CompareOperator.NotEqual;

                        cc.Add(mex.Member.Name, true, op);
                    }
                    break;

                case ExpressionType.Call:
                    {
                        MethodCallExpression mce = (MethodCallExpression)ex;

                        switch (mce.Method.Name)
                        {
                            case "Contains":
                            case "StartsWith":
                            case "EndsWith":
                                //this will generate a like expression
                                MemberExpression member = mce.Object as MemberExpression;
                                if (mce.Object == null)
                                    throw new InvalidOperationException();
                                string value = (string)((ConstantExpression)mce.Arguments[0]).Value;

                                if (mce.Method.Name == "StartsWith")
                                    value = string.Format("%{0}", value);
                                else if (mce.Method.Name == "EndsWith")
                                    value = string.Format("{0}%", value);
                                else if (mce.Method.Name == "Contains")
                                    value = string.Format("%{0}%", value);

                                Tenor.Data.CompareOperator op = Tenor.Data.CompareOperator.Like;
                                if (not)
                                    op = Tenor.Data.CompareOperator.NotLike;

                                cc.Add(member.Member.Name, value, op);

                                break;
                            default:
                                throw new NotImplementedException("Linq method call to '" + mce.Method.Name + "' is not implemented. Please, send a feature request.");
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException("Linq '" + ex.NodeType.ToString() + "' is not implemented. Please, send a feature request.");
            }
        }


        private void ReadOrderByExpressions(Expression ex, bool ascending)
        {
            switch (ex.NodeType)
            {
                case ExpressionType.Quote:
                    {
                        UnaryExpression exp = (UnaryExpression)ex;
                        if (exp.IsLifted || exp.IsLiftedToNull)
                            throw new NotImplementedException();

                        ReadOrderByExpressions(exp.Operand, ascending);
                    }
                    break;
                case ExpressionType.Lambda:
                    {
                        LambdaExpression exp = (LambdaExpression)ex;
                        //TODO: Should we check parameters?
                        ReadOrderByExpressions(exp.Body, ascending);
                    }
                    break;
                case ExpressionType.MemberAccess:
                    {
                        MemberExpression exp = (MemberExpression)ex;
                        Tenor.Data.SortOrder order = Tenor.Data.SortOrder.Ascending;
                        if (!ascending) order = Tenor.Data.SortOrder.Descending;

                        searchOptions.Sorting.Add(exp.Member.Name, order);
                    }
                    break;
            }

        }


    }
}
