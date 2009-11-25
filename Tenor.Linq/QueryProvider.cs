using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Tenor.Linq
{
    public class QueryProvider : IQueryProvider
    {

        public QueryProvider()
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
            return null;
        }

        public object Execute(Expression expression)
        {
            Type type = expression.Type;
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
                if (!type.IsSubclassOf(typeof(BLL.BLLBase)))
                    throw new InvalidCastException("Expression type is invalid.");
            }
            else
                throw new InvalidCastException("Expression type is invalid.");

            Tenor.Data.SearchOptions so = new Tenor.Data.SearchOptions(type);
            
            ReadExpressions(so.Conditions, expression, false);

            //Tenor.Data.ISearchOptions linqSo =((MethodCallExpression)
            //if (linqSo != null)
            //{
            //    //this is the base SearchOptions query;
            //    Tenor.Data.ISearchOptions realSo = (Tenor.Data.ISearchOptions)so;

            //    realSo.Distinct = linqSo.Distinct;
            //    realSo.LazyLoading = linqSo.LazyLoading;
            //    realSo.Top = linqSo.Top;
            //}

            return so.Execute();
        }
        
        
        private void ReadExpressions(Tenor.Data.ConditionCollection cc, Expression ex, bool not) 
        {
        	switch (ex.NodeType) {
                case ExpressionType.Quote:
                    {
                        UnaryExpression une = ex as UnaryExpression;
                        if (une.IsLifted || une.IsLiftedToNull)
                            throw new NotImplementedException();
                       
                        ReadExpressions(cc, une.Operand, false);

                    }
                    break;
                case ExpressionType.Lambda:
                    {
                        LambdaExpression lex = ex as LambdaExpression;
                        //if (lex.Parameters.Count != 1 || lex.Parameters[0].Type != so.Class)
                        //    throw new InvalidOperationException("Expressions can support only one parameter of '" + so.Class.FullName + "'.");

                        ReadExpressions(cc, lex.Body, false);
                    }
                    break;
                case ExpressionType.Not:
                    {
                        UnaryExpression une = ex as UnaryExpression;
                        if (une.IsLifted || une.IsLiftedToNull)
                            throw new NotImplementedException();

                        ReadExpressions(cc, une.Operand, true);
                    }
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    {
                        BinaryExpression andBinary = ex as BinaryExpression;

                        Tenor.Data.ConditionCollection newCc = new Tenor.Data.ConditionCollection();

                        ReadExpressions(newCc, andBinary.Left, false);
                        newCc.Add(Tenor.Data.LogicalOperator.And);
                        ReadExpressions(newCc, andBinary.Right, false);

                        cc.Add(newCc);
                    }
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    {
                        BinaryExpression andBinary = ex as BinaryExpression;

                        Tenor.Data.ConditionCollection newCc = new Tenor.Data.ConditionCollection();

                        ReadExpressions(newCc, andBinary.Left, false);
                        newCc.Add(Tenor.Data.LogicalOperator.Or);
                        ReadExpressions(newCc, andBinary.Right, false);

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
                            throw new InvalidOperationException ("Invalid lambda expression");

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
                            case "Where":
                                //the where clause.
                                foreach (Expression e in mce.Arguments)
                                {
                                    ReadExpressions(cc, e, false);
                                }
                                break;

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
                                throw new NotImplementedException("Linq method call to '" + mce.Method.Name.ToString() + "' is not implemented. Please, send a feature request.");
                        }


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


    }
}
