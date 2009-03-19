using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Collections.Specialized;
using System.Reflection;

namespace Tenor.Data.Dialects
{
 
    /// <summary>
    /// Represents the common code between all dialects.
    /// </summary>
    public abstract class GeneralDialect : IDialect
    {
        public abstract DbProviderFactory Factory
        {
            get;
        }

        protected abstract DbCommandBuilder CommandBuilder
        {
            get;
        }

        public abstract string ProviderInvariantName
        {
            get;
        }

        protected abstract string ParameterIdentifier
        {
            get;
        }


        public abstract string IdentityBeforeQuery
        {
            get;
        }

        public abstract string IdentityAfterQuery
        {
            get;
        }

        public abstract bool GetIdentityOnSameCommand
        {
            get;
        }


        protected virtual string GetOperator(LogicalOperator logicalOperator)
        {
            switch (logicalOperator)
            {
                case Tenor.Data.LogicalOperator.And:
                    return (" AND ");
                case Tenor.Data.LogicalOperator.Or:
                    return (" OR ");
                default:
                    throw new ArgumentOutOfRangeException("logicalOperator");
            }

        }

        protected virtual string CreateClassAlias(Type classType)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");
            return classType.FullName.Replace(".", "").ToLower();
        }



        protected virtual string GetDbType(Type systemType)
        {
            return Helper.GetDbTypeName(systemType, Factory);
        }

        private string GetSpecialFieldExpression(string classAlias, SpecialFieldInfo spFieldInfo)
        {
            string expression = spFieldInfo.Expression;
            return GetSpecialFieldExpression(classAlias, expression);
        }

        protected virtual string GetSpecialFieldExpression(string classAlias, string expression)
        {
            if (expression.Contains("{0}"))
                expression = string.Format(expression, classAlias);

            return "(" + expression + ")";
        }

        protected virtual string CreateCompareSql(Type classType, string classAlias, string propertyName, object value, CompareOperator compareOperator, Type castType, out string parameterName)
        {
            StringBuilder str = new StringBuilder();
            PropertyInfo propInfo = classType.GetProperty(propertyName);
            FieldInfo fieldInfo = FieldInfo.Create(propInfo);
            SpecialFieldInfo spFieldInfo = SpecialFieldInfo.Create(propInfo);

            //Left side

            if (fieldInfo != null)
            {

                if (castType != null)
                {
                    str.Append("CAST(");
                }

                if (!string.IsNullOrEmpty(classAlias))
                {
                    str.Append(classAlias);
                    str.Append(".");
                }
                str.Append(CommandBuilder.QuoteIdentifier(fieldInfo.DataFieldName));

                if (castType != null)
                {
                    str.Append(" AS " + GetDbType(castType) + ")");
                }

            }
            else if (spFieldInfo != null)
            {
                str.Append(GetSpecialFieldExpression(classAlias, spFieldInfo));
            }
            else
            {
                throw new MissingFieldException(classType, propertyName, true);
            }

            //Right side
            if ((value == null) || value == DBNull.Value)
            {
                parameterName = null;
                if (compareOperator == CompareOperator.Equal)
                {
                    str.Append(" IS NULL ");
                }
                else if (compareOperator == CompareOperator.NotEqual)
                {
                    str.Append(" IS NOT NULL ");
                }
                else
                {
                    throw (new InvalidOperationException("Cannot use this operator with null expressions."));
                }
            }
            else
            {
                //TenorParameter p = new Parameter(ParameterName, Value);
                parameterName = null;

                if (fieldInfo != null)
                {
                    parameterName = fieldInfo.DataFieldName.ToLower().Trim().Replace(" ", "_");
                }
                else if (spFieldInfo != null)
                {
                    parameterName = spFieldInfo.Alias.ToLower().Trim().Replace(" ", "_");
                }


                if (parameterName.Length > 30)
                {
                    throw (new InvalidOperationException("Cannot generate parameter name. Identifier \'" + parameterName + "\' is too long."));
                }

                parameterName += Guid.NewGuid().ToString().Replace("-", "");
                if (parameterName.Length > 29)
                {
                    parameterName = parameterName.Substring(0, 29);
                }
                parameterName = this.ParameterIdentifier + parameterName;


                switch (compareOperator)
                {
                    case Data.CompareOperator.Equal:
                        str.Append(" = " + parameterName);
                        break;
                    case Data.CompareOperator.NotEqual:
                        str.Append(" <>  " + parameterName);
                        break;
                    case Data.CompareOperator.LessThan:
                        str.Append(" <  " + parameterName);
                        break;
                    case Data.CompareOperator.LessThanOrEqual:
                        str.Append(" <=  " + parameterName);
                        break;
                    case Data.CompareOperator.GreaterThan:
                        str.Append(" >  " + parameterName);
                        break;
                    case Data.CompareOperator.GreaterThanOrEqual:
                        str.Append(" >=  " + parameterName);
                        break;
                    case Data.CompareOperator.Like:
                        str.Append(" LIKE  " + parameterName);
                        break;
                    case Data.CompareOperator.NotLike:
                        str.Append(" NOT LIKE  " + parameterName);
                        break;
                    case Data.CompareOperator.ContainsInFlags:
                        //if (Connection.ProviderName != "System.Data.SqlClient")
                        //{
                        //    throw (new ConfigurationErrorsException("ContainsInFlags can only be used with SqlClient"));
                        //}
                        str = new System.Text.StringBuilder(GetContainsInFlagsExpression(str.ToString(), parameterName));
                        break;
                    default:
                        throw (new ArgumentOutOfRangeException("compareOperator", "Specified argument was out of the range of valid values."));
                }
            }

            return str.ToString();
        }

        protected abstract string GetContainsInFlagsExpression(string field, string parameterName);

        protected virtual string CreateOrderBy(Type classType, string classAlias, string propertyName, SortOrder sortOrder, Type castType)
        {
            if (string.IsNullOrEmpty(classAlias))
                classAlias = CreateClassAlias(classType);

            StringBuilder str = new StringBuilder();
            PropertyInfo propInfo = classType.GetProperty(propertyName);
            FieldInfo fieldInfo = FieldInfo.Create(propInfo);
            SpecialFieldInfo spFieldInfo = SpecialFieldInfo.Create(propInfo);

            if (fieldInfo != null)
            {
                if (castType != null)
                {
                    str.Append("CAST(");
                }

                str.Append(classAlias);

                str.Append(".");
                str.Append(CommandBuilder.QuoteIdentifier(fieldInfo.DataFieldName));

                if (castType != null)
                {
                    str.Append(" AS " + GetDbType(castType) + ")");
                }
            }
            else if (spFieldInfo != null)
            {
                str.Append(spFieldInfo.Alias);
            }
            else
            {
                throw (new ArgumentException("Invalid property \'" + propertyName + "\' on class '" + classType.Name + "'. You must define a Field or a SpecialField property item.", "propertyName", null));
            }

            switch (sortOrder)
            {
                case SortOrder.Descending:
                    str.Append(" DESC");
                    break;
                case SortOrder.Ascending:
                    str.Append(" ASC");
                    break;
            }

            return str.ToString();

        }

        #region IDialect Members


        string IDialect.CreateWhereSql(ConditionCollection conditions, Type baseClass, Join[] joins, out TenorParameter[] parameters)
        {
            return CreateWhereSql(conditions, baseClass, joins, out parameters, true);
        }

        private string CreateWhereSql(ConditionCollection conditions, Type baseClass, Join[] joins, out TenorParameter[] parameters, bool generateAliases)
        {


            string baseAlias = string.Empty;
            if (generateAliases)
                baseAlias = CreateClassAlias(baseClass);
            StringBuilder sqlWhere = new StringBuilder();


            List<TenorParameter> parametersList = new List<TenorParameter>();

            for (int i = 0; i <= conditions.Count - 1; i++)
            {
                object obj = conditions[i];

                if (obj.GetType() == typeof(SearchCondition))
                {
                    SearchCondition sc = (SearchCondition)obj;

                    Type table = baseClass;
                    string alias = baseAlias;
                    if (!string.IsNullOrEmpty(sc.JoinAlias))
                    {
                        int pos = Array.IndexOf(joins, new Join(sc.JoinAlias));
                        if (pos == -1)
                            throw new InvalidOperationException("Cannot find the join alias '" + sc.JoinAlias + "'");
                        Join join = joins[pos];
                        table = join.ForeignKey.ElementType;
                        alias = join.JoinAlias;
                    }
                    string parameter = null;
                    sqlWhere.Append(CreateCompareSql(table, alias, sc.PropertyName, sc.Value, sc.CompareOperator, sc.CastType, out parameter));

                    if (sc.Value != null)
                    {
                        parametersList.Add(new TenorParameter(parameter, sc.Value));
                    }
                }
                else if (obj.GetType() == typeof(LogicalOperator))
                {
                    LogicalOperator lOp = (LogicalOperator)obj;

                    sqlWhere.AppendLine(GetOperator(lOp));

                    if (i == conditions.Count - 1)
                    {
                        throw (new InvalidCollectionArgument(CollectionProblem.InvalidLastItem));
                    }
                }
                else if (obj.GetType() == typeof(ConditionCollection))
                {
                    TenorParameter[] groupParameters = null;
                    string group = ((IDialect)this).CreateWhereSql(((ConditionCollection)obj), baseClass, joins, out groupParameters);
                    if (string.IsNullOrEmpty(group))
                    {
                        throw new InvalidCollectionArgument(CollectionProblem.NullCollection);
                    }
                    sqlWhere.AppendLine(" ( " + group + " ) ");
                    parametersList.AddRange(groupParameters);
                }

            }

            parameters = parametersList.ToArray();

            return sqlWhere.ToString();

        }


        string IDialect.CreateSelectSql(Type baseClass, FieldInfo[] fields, SpecialFieldInfo[] spFields)
        {
            string alias = CreateClassAlias(baseClass);
            StringBuilder fieldsSql = new StringBuilder();

            foreach (FieldInfo f in fields)
            {
                if (f.PrimaryKey || !f.LazyLoading)
                {
                    //determina o nome do campo
                    fieldsSql.Append(", " + alias + "." + CommandBuilder.QuoteIdentifier(f.DataFieldName));
                }
            }
            if (fieldsSql.Length == 0)
            {
                throw new MissingFieldsException(baseClass, true);
            }

            foreach (SpecialFieldInfo f in spFields)
            {
                fieldsSql.Append(", ");
                fieldsSql.Append(GetSpecialFieldExpression(alias, f));
                fieldsSql.Append(" AS ").Append(f.Alias);
            }
            fieldsSql = fieldsSql.Remove(0, 2);
            return fieldsSql.ToString();
        }

        string IDialect.CreateSortSql(SortingCollection sortCollection, Type baseClass, Join[] joins, bool isDistinct, out string appendToSelect)
        {
            //Sorting
            StringBuilder sqlSort = new StringBuilder();
            appendToSelect = string.Empty;
            foreach (SortingCriteria sort in sortCollection)
            {
                Type table = baseClass;
                string alias = CreateClassAlias(baseClass);
                if (!string.IsNullOrEmpty(sort.JoinAlias))
                {
                    int pos = Array.IndexOf(joins, new Join(sort.JoinAlias));
                    if (pos == -1)
                        throw new InvalidCollectionArgument(CollectionProblem.InvalidJoin, sort.JoinAlias);
                    Join join = joins[pos];
                    table = join.ForeignKey.ElementType;
                    alias = join.JoinAlias;
                }

                PropertyInfo property = table.GetProperty(sort.PropertyName);
                if (property == null)
                    throw new MissingFieldException(table, sort.PropertyName);
                FieldInfo fieldInfo = FieldInfo.Create(property);
                SpecialFieldInfo spInfo = SpecialFieldInfo.Create(property);
                if (fieldInfo == null && spInfo == null)
                    throw new MissingFieldException(table, property.Name, true);


                sqlSort.Append(", ");
                sqlSort.Append(CreateOrderBy(table, alias, sort.PropertyName, sort.SortOrder, sort.CastType));
                //fields that must come into sort part
                if (isDistinct && table != baseClass)
                {
                    StringBuilder fieldExpression = new StringBuilder();

                    if (fieldInfo != null)
                    {
                        fieldExpression.Append(alias + "." + CommandBuilder.QuoteIdentifier(fieldInfo.DataFieldName));
                    }
                    else if (spInfo != null)
                    {
                        fieldExpression.Append(GetSpecialFieldExpression(alias, spInfo));
                        fieldExpression.Append(" AS " + spInfo.Alias);
                    }

                    string field = ", " + fieldExpression;
                    if (!appendToSelect.Contains(field))
                    {
                        appendToSelect += (field);
                    }
                }
            }
            if (sqlSort.Length > 0)
                sqlSort = sqlSort.Remove(0, 2);
            return sqlSort.ToString();
        }

        string IDialect.CreateJoinsSql(Join[] joins)
        {
            StringBuilder sql = new StringBuilder();
            foreach (Join join in joins)
            {
                string tableName = join.ForeignTableInfo.Prefix + "." + CommandBuilder.QuoteIdentifier(join.ForeignTableInfo.TableName) + " " + join.JoinAlias;
                switch (join.JoinMode)
                {
                    case JoinMode.InnerJoin:
                        sql.Append(" INNER JOIN " + tableName + " ON ");
                        break;
                    case JoinMode.LeftJoin:
                        sql.Append(" LEFT OUTER JOIN " + tableName + " ON ");
                        break;
                    case JoinMode.RightJoin:
                        sql.Append(" RIGHT OUTER JOIN " + tableName + " ON ");
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(join.ParentAlias))
                {
                    join.ParentAlias = CreateClassAlias(join.LocalTableInfo.RelatedTable);
                }
                StringBuilder fks = new StringBuilder();
                for (int i = 0; i < join.ForeignKey.ForeignFields.Length; i++)
                {
                    fks.Append(" AND ");
                    fks.Append(join.ParentAlias + "." + join.ForeignKey.LocalFields[i].DataFieldName);
                    fks.Append(" =  ");
                    fks.Append(join.JoinAlias + "." + join.ForeignKey.ForeignFields[i].DataFieldName);
                }
                sql.Append(fks.Remove(0, 4));
                sql.AppendLine();
            }
            return sql.ToString();
        }

        string IDialect.CreateFullSql(Type baseClass, bool isDistinct, bool justCount, int limit, string fieldsPart, string joinsPart, string sortPart, string wherePart)
        {
            TableInfo table = TableInfo.CreateTableInfo(baseClass);
            string baseAlias = CreateClassAlias(baseClass);
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT ");
            if (!justCount)
            {
                if (isDistinct)
                {
                    sql.Append("DISTINCT ");
                }
                if (limit > 0)
                {
                    sql.Append(" TOP " + limit.ToString() + " ");
                }
                sql.AppendLine(fieldsPart);
            }
            else
            {
                sql.Append(" COUNT(" + baseAlias + ".*) ");
            }


            string froms = table.GetSchemaAndTable();

            //TODO: Implement localizable searchs


            ////' Tradução automática com a view correspondente
            //if (instance.Localizable && CultureInfo.CurrentCulture.IetfLanguageTag != Configuration.Localization.DefaultCulture)
            //{
            //    froms = GetSchemaAndView(instance);

            //    if (sqlWHERE.Length > 0)
            //    {
            //        sqlWHERE += " AND ";
            //    }
            //    sqlWHERE += "IetfLanguageTag = \'" + CultureInfo.CurrentCulture.IetfLanguageTag + "\'";
            //}

            froms += " AS " + baseAlias;
            sql.AppendLine(" FROM " + froms);
            sql.AppendLine(joinsPart);

            if (wherePart.Length > 0)
            {
                sql.Append(" WHERE ");
                sql.AppendLine(wherePart);
            }

            if (sortPart.Length > 0)
            {
                sql.Append(" ORDER BY ");
                sql.AppendLine(sortPart.ToString());
            }

            return sql.ToString();

        }

        string IDialect.CreateSaveSql(Type baseClass, Dictionary<FieldInfo, object> data, NameValueCollection specialValues, ConditionCollection conditions, out TenorParameter[] parameters)
        {
            List<TenorParameter> parameterList = new List<TenorParameter>();
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder fieldAndValues = new StringBuilder();
            string alias = CreateClassAlias(baseClass);

            foreach (FieldInfo field in data.Keys)
            {
                string paramName = field.DataFieldName;
                if (field.AutoNumber && string.IsNullOrEmpty(this.IdentityBeforeQuery))
                    continue;
                else
                    paramName = "identity";

                TenorParameter param = new TenorParameter(this.ParameterIdentifier + paramName, data[field]);

                if ((field.AutoNumber && !this.GetIdentityOnSameCommand) ||
                    !field.AutoNumber
                    )
                    parameterList.Add(param);

                fields.Append(", ");
                fields.Append(CommandBuilder.QuoteIdentifier(field.DataFieldName));


                values.Append(", ");

                if (specialValues == null || string.IsNullOrEmpty(specialValues[paramName]))
                {
                    // The parameter as is.
                    values.Append(param.ParameterName);
                }
                else
                {
                    //Replaced by a SQL statement
                    values.Append(specialValues[paramName]);
                }


                fieldAndValues.Append(", ");
                fieldAndValues.Append(CommandBuilder.QuoteIdentifier(field.DataFieldName) + " = ");
                if (specialValues == null || string.IsNullOrEmpty(specialValues[paramName]))
                {
                    // The parameter as is.
                    fieldAndValues.Append(param.ParameterName);
                }
                else
                {
                    //Replaced by a SQL statement
                    fieldAndValues.Append(specialValues[paramName]);
                }


            }
            if (fieldAndValues.Length > 0)
                fieldAndValues = fieldAndValues.Remove(0, 2);
            if (fields.Length > 0)
                fields = fields.Remove(0, 2);
            if (values.Length > 0)
                values = values.Remove(0, 2);


            TableInfo table = TableInfo.CreateTableInfo(baseClass);

            string query = null;
            if (conditions != null && conditions.Count > 0)
            {
                TenorParameter[] whereParameters = null;
                string clause = CreateWhereSql(conditions, baseClass, null, out whereParameters, false);
                parameterList.AddRange(whereParameters);

                query = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    table.GetSchemaAndTable(),
                    fieldAndValues,
                    clause);
            }
            else
            {
                query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                    table.GetSchemaAndTable(),
                    fields,
                    values);
            }
            parameters = parameterList.ToArray();

            return query;
        }

        string IDialect.CreateConditionalSaveSql(string insertQuery, string updateQuery, string[] conditionalProperties, FieldInfo[] fieldsPrimary)
        {
            throw new NotImplementedException();
            /*
            StringBuilder query = new StringBuilder();
            StringBuilder queryDeclares = new StringBuilder();

            StringBuilder querySelect = new StringBuilder();
            StringBuilder queryIsNull = new StringBuilder();
            StringBuilder queryWhere = new StringBuilder();

            foreach (FieldInfo f in fieldsPrimary)
            {
                TenorParameter p = new TenorParameter(f.RelatedProperty.Name + "__test", f.PropertyValue(this));
                querySelect.Append(",");
                querySelect.Append(paramPrefix + table.GetTableAlias() + f.RelatedProperty.Name);
                querySelect.Append(" = ");
                querySelect.Append(f.DataFieldName);
            }


            foreach (FieldInfo f in fields)
            {
                TenorParameter p = new TenorParameter(f.RelatedProperty.Name + "__test", f.PropertyValue(this));
                queryDeclares.AppendLine("DECLARE " + paramPrefix + f.RelatedProperty.Name + "_ " + Helper.GetDbTypeName(p.Value.GetType(), factory));
                querySelect.Append(",");
                querySelect.Append(paramPrefix + f.RelatedProperty.Name + "_");
                querySelect.Append(" = ");
                querySelect.Append(f.DataFieldName);

                queryWhere.Append(" AND ");
                queryWhere.Append(f.DataFieldName);
                queryWhere.Append(" = ");
                queryWhere.Append(paramPrefix + table.GetTableAlias() + f.RelatedProperty.Name);

                queryIsNull.Append(" AND ");
                queryIsNull.Append(paramPrefix + f.RelatedProperty.Name + "_ IS NULL");

            }
            query.Append(queryDeclares);
            query.AppendLine();
            query.Append("SELECT ");

            querySelect.Remove(0, 1);
            query.Append(querySelect);
            query.Append(" FROM ");
            query.Append(table.GetSchemaAndTable());
            query.Append(" WHERE ");
            queryWhere.Remove(0, 4);
            query.AppendLine(queryWhere.ToString());

            queryIsNull.Remove(0, 4);
            query.Append("IF ");
            query.Append(queryIsNull);
            query.AppendLine(" BEGIN");
            query.AppendLine("      " + insertQuery);
            query.AppendLine("      SELECT SCOPE_IDENTITY()");
            query.AppendLine(" END ELSE BEGIN ");
            if (isUpdate)
            {
                query.AppendLine("      " + updateQuery);
            }
            query.AppendLine("      SELECT -1 ");
            query.AppendLine(" END ");
       
             */
        }

        string IDialect.CreateDeleteSql(Type baseClass, ConditionCollection conditions, Join[] joins, out TenorParameter[] parameters)
        {
            string alias = CreateClassAlias(baseClass);

            TableInfo table = TableInfo.CreateTableInfo(baseClass);


            string clause = ((IDialect)this).CreateWhereSql(conditions, baseClass, joins, out parameters);

            return "DELETE FROM " + table.GetSchemaAndTable() + " AS " + alias + " WHERE " + clause;
        }

        #endregion
    }
}
