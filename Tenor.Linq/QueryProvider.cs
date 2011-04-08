using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        Type elementType;
        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            elementType = typeof(T);
            return (IQueryable<T>)Activator.CreateInstance(typeof(SearchOptions<>).MakeGenericType(elementType), new object[] { this, expression });
        }



        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            elementType = expression.Type;

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

        bool doCount = false;
        public object Execute(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Call)
                throw new InvalidOperationException();
            MethodCallExpression exp = (MethodCallExpression)expression;

            aliasList = new Dictionary<MemberInfo, string>();

            ReadExpressions(expression);

            try
            {
                if (doCount)
                    return searchOptions.ExecuteCount();
                else
                {
                    IList toReturn;
                    if (searchOptions.Projections.Count == 0)
                    {
                        toReturn = searchOptions.Execute();
                    }
                    else
                    {
                        object[][] items = searchOptions.ExecuteMatrix();
                        Type listof = typeof(List<>).MakeGenericType(elementType);
                        IList list = (IList)Activator.CreateInstance(listof);

                        foreach (object[] row in items)
                        {
                            object typedRow = null;
                            if (projectionBindings == null)
                            {
                                if (searchOptions.Projections.Count == 1)
                                    typedRow = row[0];
                                else
                                    typedRow = projectionCtor.Invoke(row);
                            }
                            else
                            {
                                typedRow = projectionCtor.Invoke(null);
                                for (int i = 0; i < projectionBindings.Count; i++)
                                {
                                    FieldInfo field = projectionBindings[i].Member as FieldInfo;
                                    PropertyInfo prop = projectionBindings[i].Member as PropertyInfo;
                                    if (field != null)
                                        field.SetValue(typedRow, row[i]);
                                    else if (prop != null)
                                        prop.SetValue(typedRow, row[i], null);
                                    else
                                        throw new NotImplementedException(string.Format("The member access of '{0}' is not implemented.", projectionBindings[i].Member.Name));
                                }
                            }
                            list.Add(typedRow);
                        }

                        toReturn = list;
                    }



                    switch (exp.Method.Name)
                    {
                        case "Single":
                        case "First":
                            if (toReturn.Count > 1)
                                throw new InvalidOperationException("The input sequence contains more than one element.");
                            else if (toReturn.Count == 0)
                                throw new ArgumentNullException("The input sequence is empty.");
                            return toReturn[0];

                        case "SingleOrDefault":
                        case "FirstOrDefault":
                            if (toReturn.Count > 1)
                                throw new InvalidOperationException("The input sequence contains more than one element.");
                            else if (toReturn.Count == 0)
                                return null;
                            else
                                return toReturn[0];
                        default:
                            return toReturn;
                    }


                }
            }
            catch
            {
                throw;
            }
            finally
            {
                searchOptions = null;
                aliasList = null;
            }
        }



        private void ReadExpressions(Expression ex)
        {

            switch (ex.NodeType)
            {
                case ExpressionType.Call:
                    {
                        MethodCallExpression mce = (MethodCallExpression)ex;
                        //continue recursively
                        ReadExpressions(mce.Arguments[0]);

                        switch (mce.Method.Name)
                        {
                            /* LINQ Methods */
                            case "Where":
                                //the where clause.
                                ReadWhereExpressions(searchOptions.Conditions, mce.Arguments[1], false, null);
                                break;
                            case "ThenBy":
                            case "OrderBy":
                                ReadOrderByExpressions(mce.Arguments[1], true);
                                break;
                            case "ThenByDescending":
                            case "OrderByDescending":
                                ReadOrderByExpressions(mce.Arguments[1], false);
                                break;
                            case "Take":
                                //the TOP/LIMIT function.
                                int top = (int)((ConstantExpression)mce.Arguments[1]).Value;
                                searchOptions.Top = top;
                                break;
                            case "SingleOrDefault":
                            case "Single":
                                //nothing to do here
                                break;
                            case "FirstOrDefault":
                            case "First":
                                searchOptions.Top = 1;
                                break;
                            case "Distinct":
                                searchOptions.Distinct = true;
                                break;
                            case "Count":
                                doCount = true;
                                break;
                            /* END LINQ */
                            /* TENOR LINQ Methods */
                            case "LoadAlso":
                                ReadEager(mce.Arguments[1], null);
                                break;
                            /* END LINQ Methods */
                            case "Select":
                                ReadSelect(mce.Arguments[1]);
                                break;
                            default:
                                throw new NotImplementedException("Linq method call to '" + mce.Method.Name.ToString() + "' is not implemented. Please, send a feature request.");
                        }
                    }
                    break;
                case ExpressionType.Constant:
                    {
                        ConstantExpression cex = (ConstantExpression)ex;
                        IQueryable item = cex.Value as IQueryable;
                        if (item != null)
                        {
                            Type[] t = item.GetType().GetGenericArguments();
                            searchOptions = new Tenor.Data.SearchOptions(t[0]);
                            //searchOptions.LazyLoading = item.LazyLoading;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException("Linq '" + ex.NodeType.ToString() + "' is not implemented. Please, send a feature request.");
            }
        }

        ConstructorInfo projectionCtor;
        ReadOnlyCollection<MemberBinding> projectionBindings;

        private void ReadSelect(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Quote:
                    {
                        UnaryExpression exp = (UnaryExpression)expression;
                        ReadSelect(exp.Operand);
                    }
                    break;
                case ExpressionType.Lambda:
                    {
                        LambdaExpression exp = (LambdaExpression)expression;
                        ReadSelect(exp.Body);
                    }
                    break;
                case ExpressionType.New:
                    {
                        //Anonymous types
                        NewExpression exp = (NewExpression)expression;
                        projectionCtor = exp.Constructor;
                        foreach (MemberExpression member in exp.Arguments)
                        {
                            AddProjection(member);
                        }

                    }
                    break;
                case ExpressionType.MemberAccess:
                    {
                        MemberExpression exp = (MemberExpression)expression;
                        AddProjection(exp);
                    }
                    break;
                case ExpressionType.MemberInit:
                    {
                        //Typed types
                        MemberInitExpression exp = (MemberInitExpression)expression;
                        projectionCtor = exp.NewExpression.Constructor;
                        projectionBindings = exp.Bindings;

                        foreach (MemberAssignment member in exp.Bindings)
                        {
                            MemberExpression propExp = member.Expression as MemberExpression;
                            AddProjection(propExp);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void AddProjection(MemberExpression member)
        {
            string alias = GetAliasForMemberExpression(member.Expression, null);
            searchOptions.Projections.Add(member.Member.Name, alias);
        }

        private string GetAliasForMemberExpression(Expression ex, string baseAlias)
        {
            MemberExpression memberCall = ex as MemberExpression;
            if (memberCall != null)
            {
                baseAlias = GetAliasForMemberExpression(memberCall.Expression, baseAlias);
                string alias = GenerateAliasForMember(baseAlias, memberCall);
                if (!string.IsNullOrEmpty(alias))
                    searchOptions.LoadAlso(memberCall.Member.Name);
                return alias;
            }
            else
                return null;
        }



        private void ReadEager(Expression expression, string alias)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Quote:
                    {
                        UnaryExpression exp = (UnaryExpression)expression;
                        ReadEager(exp.Operand, alias);
                    }
                    break;
                case ExpressionType.Lambda:
                    {
                        LambdaExpression exp = (LambdaExpression)expression;
                        ReadEager(exp.Body, alias);
                    }
                    break;
                case ExpressionType.MemberAccess:
                    {
                        MemberExpression exp = (MemberExpression)expression;
                        searchOptions.LoadAlso(exp.Member.Name);
                    }
                    break;
                case ExpressionType.Constant:
                    {
                        ConstantExpression exp = (ConstantExpression)expression;
                        searchOptions.LoadAlso((string)exp.Value);
                    }
                    break;
                default:
                    throw new NotImplementedException("Linq '" + expression.NodeType.ToString() + "' is not implemented. Please, send a feature request.");
            }

        }
        Dictionary<MemberInfo, string> aliasList = null;

        private void ReadWhereExpressions(Tenor.Data.ConditionCollection cc, Expression ex, bool not, string alias)
        {
            switch (ex.NodeType)
            {
                case ExpressionType.Quote:
                    {
                        UnaryExpression une = ex as UnaryExpression;
                        if (une.IsLifted || une.IsLiftedToNull)
                            throw new NotImplementedException();

                        ReadWhereExpressions(cc, une.Operand, not, alias);

                    }
                    break;
                case ExpressionType.Lambda:
                    {
                        LambdaExpression lex = ex as LambdaExpression;
                        //TODO: Should we check parameters?
                        ReadWhereExpressions(cc, lex.Body, not, alias);
                    }
                    break;
                case ExpressionType.Not:
                    {
                        UnaryExpression une = ex as UnaryExpression;
                        if (une.IsLifted || une.IsLiftedToNull)
                            throw new NotImplementedException();

                        ReadWhereExpressions(cc, une.Operand, !not, alias);
                    }
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    {
                        BinaryExpression andBinary = ex as BinaryExpression;

                        Tenor.Data.ConditionCollection newCc = new Tenor.Data.ConditionCollection();

                        ReadWhereExpressions(newCc, andBinary.Left, not, alias);
                        newCc.Add(Tenor.Data.LogicalOperator.And);
                        ReadWhereExpressions(newCc, andBinary.Right, not, alias);

                        if (cc.Count > 0 && cc[cc.Count - 1].GetType() != typeof(Tenor.Data.LogicalOperator))
                            cc.Add(Tenor.Data.LogicalOperator.And);
                        cc.Add(newCc);
                    }
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    {
                        BinaryExpression andBinary = ex as BinaryExpression;

                        Tenor.Data.ConditionCollection newCc = new Tenor.Data.ConditionCollection();

                        ReadWhereExpressions(newCc, andBinary.Left, not, alias);
                        newCc.Add(Tenor.Data.LogicalOperator.Or);
                        ReadWhereExpressions(newCc, andBinary.Right, not, alias);

                        if (cc.Count > 0 && cc[cc.Count - 1].GetType() != typeof(Tenor.Data.LogicalOperator))
                            cc.Add(Tenor.Data.LogicalOperator.And);
                        cc.Add(newCc);
                    }
                    break;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    {
                        BinaryExpression bex = ex as BinaryExpression;


                        MemberExpression left = ReadOperand(bex, false) as MemberExpression;




                        bool invertOperator = false;
                        Expression right = ReadOperand(bex, true, ref invertOperator);

                        if (ex.NodeType != ExpressionType.Equal && ex.NodeType != ExpressionType.NotEqual && invertOperator)
                            not = !not;


                        //check if we need another alias
                        if (left.Expression != null && left.Expression is MemberExpression)
                        {
                            MemberExpression lExp = (MemberExpression)left.Expression;
                            if (lExp.Member.MemberType == MemberTypes.Property)
                            {

                                alias = IncludeAlias(cc, lExp, alias);


                            }
                        }

                        Tenor.Data.CompareOperator op = Tenor.Data.CompareOperator.Equal;
                        if (ex.NodeType == ExpressionType.Equal && !not)
                            op = Tenor.Data.CompareOperator.Equal;
                        else if (ex.NodeType == ExpressionType.Equal && not)
                            op = Tenor.Data.CompareOperator.NotEqual;
                        else if (ex.NodeType == ExpressionType.NotEqual && !not)
                            op = Tenor.Data.CompareOperator.NotEqual;
                        else if (ex.NodeType == ExpressionType.NotEqual && not)
                            op = Tenor.Data.CompareOperator.Equal;
                        else if (ex.NodeType == ExpressionType.GreaterThan && !not)
                            op = Tenor.Data.CompareOperator.GreaterThan;
                        else if (ex.NodeType == ExpressionType.GreaterThan && not)
                            op = Tenor.Data.CompareOperator.LessThan;
                        else if (ex.NodeType == ExpressionType.LessThan && !not)
                            op = Tenor.Data.CompareOperator.LessThan;
                        else if (ex.NodeType == ExpressionType.LessThan && not)
                            op = Tenor.Data.CompareOperator.GreaterThan;
                        else if (ex.NodeType == ExpressionType.GreaterThanOrEqual && !not)
                            op = Tenor.Data.CompareOperator.GreaterThanOrEqual;
                        else if (ex.NodeType == ExpressionType.GreaterThanOrEqual && not)
                            op = Tenor.Data.CompareOperator.LessThanOrEqual;
                        else if (ex.NodeType == ExpressionType.LessThanOrEqual && !not)
                            op = Tenor.Data.CompareOperator.LessThanOrEqual;
                        else if (ex.NodeType == ExpressionType.LessThanOrEqual && not)
                            op = Tenor.Data.CompareOperator.GreaterThanOrEqual;


                        cc.Add(left.Member.Name, FindValue(right), op, alias);
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

                        if (cc.Count > 0 && cc[cc.Count - 1].GetType() != typeof(Tenor.Data.LogicalOperator))
                            cc.Add(Tenor.Data.LogicalOperator.And);
                        cc.Add(mex.Member.Name, true, op, alias);
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
                                {
                                    //this will generate a like expression
                                    MemberExpression member = mce.Object as MemberExpression;
                                    if (mce.Object == null)
                                    {
                                        if (mce.Method.Name == "Contains" && mce.Arguments[0] is MemberExpression)
                                        {
                                            member = mce.Arguments[1] as MemberExpression;

                                            // In
                                            Tenor.Data.ConditionCollection inValues = new Data.ConditionCollection();
                                            IList values = FindValue(mce.Arguments[0]) as IList;
                                            foreach (object value in values)
                                            {
                                                if (inValues.Count > 0)
                                                    inValues.Add(Data.LogicalOperator.Or);
                                                inValues.Add(member.Member.Name, value, Data.CompareOperator.Equal, alias);
                                            }

                                            cc.Add(inValues);
                                        }
                                        else
                                            throw new InvalidOperationException();
                                    }
                                    else if (member.Type != typeof(string))
                                    {
                                        throw new NotImplementedException();
                                    }
                                    else
                                    {

                                        string value = FindValue(mce.Arguments[0]) as string;

                                        if (mce.Method.Name == "StartsWith")
                                            value = string.Format("{0}%", value);
                                        else if (mce.Method.Name == "EndsWith")
                                            value = string.Format("%{0}", value);
                                        else if (mce.Method.Name == "Contains")
                                            value = string.Format("%{0}%", value);

                                        Tenor.Data.CompareOperator op = Tenor.Data.CompareOperator.Like;
                                        if (not)
                                            op = Tenor.Data.CompareOperator.NotLike;

                                        cc.Add(member.Member.Name, value, op);
                                    }
                                }
                                break;
                            case "Any":
                                {
                                    //here we will join.

                                    MemberExpression member = mce.Arguments[0] as MemberExpression;
                                    string newAlias = GenerateAliasForMember(alias, member);

                                    Tenor.Data.ConditionCollection newCc = new Tenor.Data.ConditionCollection();
                                    ReadWhereExpressions(newCc, mce.Arguments[1], not, newAlias);
                                    if (cc.Count > 0 && cc[cc.Count - 1].GetType() != typeof(Tenor.Data.LogicalOperator))
                                        cc.Add(Tenor.Data.LogicalOperator.And);
                                    cc.Add(newCc);

                                }
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

        private string IncludeAlias(Data.ConditionCollection cc, MemberExpression lExp, string alias)
        {
            var innerExp = lExp.Expression as MemberExpression;
            if (innerExp != null)
                alias = IncludeAlias(cc, innerExp, alias);

            string newAlias = lExp.Member.Name.ToLower() + DateTime.Now.Millisecond.ToString();

            cc.Include(alias, lExp.Member.Name, newAlias, Data.JoinMode.LeftJoin);

            return newAlias;
        }

        private static object FindValue(Expression expression)
        {
            if (expression is MemberExpression)
            {
                MemberExpression exp = (MemberExpression)expression;

                PropertyInfo prop = exp.Member as PropertyInfo;
                FieldInfo field = exp.Member as FieldInfo;
                if (prop != null)
                    return prop.GetValue((exp.Expression as ConstantExpression).Value, null);
                else if (field != null)
                    return field.GetValue((exp.Expression as ConstantExpression).Value);
                else
                    throw new NotImplementedException("Access to " + expression.ToString() + " not implemented.");
            }
            else if (expression is MethodCallExpression)
            {
                MethodCallExpression exp = (MethodCallExpression)expression;
                List<object> parameters = new List<object>();
                foreach (Expression ex in exp.Arguments)
                    parameters.Add(FindValue(ex));
                return exp.Method.Invoke(FindValue(exp.Object), parameters.ToArray());
            }
            else if (expression is ConstantExpression)
                return ((ConstantExpression)expression).Value;
            else
                throw new NotImplementedException("Access to " + expression.NodeType.ToString() + " not implemented.");
        }

        private string GenerateAliasForMember(string currentAlias, MemberExpression member)
        {
            string newAlias = null;
            if (aliasList.ContainsKey(member.Member))
                newAlias = aliasList[member.Member];
            else
            {
                newAlias = string.Concat(currentAlias, member.Member.Name);
                aliasList.Add(member.Member, newAlias);
                searchOptions.Conditions.Include(currentAlias, member.Member.Name, newAlias, Tenor.Data.JoinMode.LeftJoin);
            }
            return newAlias;
        }

        private static Expression ReadOperand(BinaryExpression bex, bool justConstant)
        {
            bool weDontNeed = false;
            return ReadOperand(bex, justConstant, ref weDontNeed);
        }

        private static Expression ReadOperand(BinaryExpression bex, bool justConstant, ref bool not)
        {
            Expression right = ReadOperandSide(bex.Right, justConstant);

            if (right == null)
            {
                right = ReadOperandSide(bex.Left, justConstant);
                not = !not;
            }

            if (right == null)
                throw new NotImplementedException("Not implemeted lambda expression.");
            return right;
        }

        private static Expression ReadOperandSide(Expression parameter, bool justConstant)
        {
            if (parameter.NodeType == ExpressionType.Convert)
            {
                return ReadOperandSide(((UnaryExpression)parameter).Operand, justConstant);
            }
            else if (parameter.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression operand = (MemberExpression)parameter;
                if (justConstant && operand.Expression is ConstantExpression)
                    return operand;
                else if (justConstant && !(operand.Expression is ConstantExpression))
                    return null;
                else if (!justConstant && operand.Expression is ConstantExpression)
                    return null;
                else
                    return operand;
            }
            else if (justConstant && parameter.NodeType == ExpressionType.Constant)
                return parameter;
            else
                return null;
        }

        //private static MemberExpression ReadEntityAccess(BinaryExpression bex)
        //{
        //    MemberExpression left = null;
        //    if (bex.Left.NodeType == ExpressionType.MemberAccess)
        //        left = bex.Left as MemberExpression;
        //    //    Convert Type - guess that this was wrong, so commented
        //    //    left = ((UnaryExpression)bex.Left).Operand as MemberExpression;

        //    if (left == null)
        //    {
        //        if (bex.Right.NodeType == ExpressionType.MemberAccess)
        //            left = bex.Right as MemberExpression;
        //        //    Convert Type
        //        // left = ((UnaryExpression)bex.Right).Operand as MemberExpression;
        //    }

        //    if (left == null)
        //        throw new NotImplementedException("Not implemeted lambda expression.");
        //    return left;
        //}


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
