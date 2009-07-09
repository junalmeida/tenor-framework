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
        /// <summary>
        /// Defines the max length of a parameter name.
        /// </summary>
        public const int ParameterMaxLength = 30;

        /// <summary>
        /// Gets the system default factory for the current provider. 
        /// </summary>
        public abstract DbProviderFactory Factory
        {
            get;
        }

        /// <summary>
        /// Gets the command builder of the current factory.
        /// </summary>
        protected virtual DbCommandBuilder CommandBuilder
        {
            get
            {
                if (Factory != null)
                    return Factory.CreateCommandBuilder();
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the invariant provider name represented by this dialect.
        /// </summary>
        public abstract string ProviderInvariantName
        {
            get;
        }

        /// <summary>
        /// Get the character used to define a parameter or a variable on SQL code. For examplan an '@' for T-SQL.
        /// </summary>
        protected abstract string ParameterIdentifier
        {
            get;
        }

        /// <summary>
        /// Gets the characher used to define the end of a statement. Returns null when line ending is not necessary.
        /// </summary>
        public abstract string LineEnding
        {
            get;
        }

        /// <summary>
        /// Gets the statement used to get an identity value before the query. Returns null when this is not supported.
        /// </summary>
        public abstract string IdentityBeforeQuery
        {
            get;
        }

        /// <summary>
        /// Gets the statement used to get an identity value during the query. Returns null when this is not supported.
        /// </summary>
        public abstract string IdentityDuringQuery
        {
            get;
        }

        /// <summary>
        /// Gets the statement used to get an identity value after the query. Returns null when this is not supported.
        /// </summary>
        public abstract string IdentityAfterQuery
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this provider can supply the identity value on the same round trip.
        /// </summary>
        public abstract bool GetIdentityOnSameCommand
        {
            get;
        }

        /// <summary>
        /// Gets a LimitType value that defines where the limit statement will be written.
        /// </summary>
        public abstract LimitType LimitAt
        {
            get;
        }

        /// <summary>
        /// Creates the limit command using the valued passed as parameter.
        /// </summary>
        /// <param name="limitValue">A limit value used to limit the number of rows returned.</param>
        /// <returns>A string with the limit query part.</returns>
        public abstract string CreateLimit(int limitValue);

        /// <summary>
        /// Creates the SQL operator.
        /// </summary>
        /// <param name="logicalOperator">The logical operator used on conditions.</param>
        /// <returns>A string with the SQL representation of the operator.</returns>
        protected virtual string GetOperator(LogicalOperator logicalOperator)
        {
            //TODO: Remove spaces around the operator.
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

        /// <summary>
        /// Creates an string alias based on the class type. 
        /// </summary>
        /// <param name="classType">A Type that implements a BLLBase</param>
        /// <returns></returns>
        protected virtual string CreateClassAlias(Type classType)
        {
            if (classType == null)
                throw new ArgumentNullException("classType");

            if (!classType.IsSubclassOf(typeof(BLL.BLLBase)))
                throw new ArgumentException(string.Format("The class '{0}' must derive directly or indirectly from '{1}'.", classType.FullName, typeof(BLL.BLLBase).FullName), "classType");

            return classType.FullName.Replace(".", "").ToLower();
        }


        /// <summary>
        /// Converts a system type name into a database type.
        /// </summary>
        /// <param name="systemType">A System.Type.</param>
        /// <returns></returns>
        protected virtual string GetDbType(Type systemType)
        {
            return Helper.GetDbTypeName(systemType, Factory);
        }

        /// <summary>
        /// Creates the query part equivalent to a SpecialFieldAttribute.
        /// </summary>
        private string GetSpecialFieldExpression(string classAlias, SpecialFieldInfo spFieldInfo)
        {
            string expression = spFieldInfo.Expression;
            return GetSpecialFieldExpression(classAlias, expression);
        }

        /// <summary>
        /// Creates the query part equivalent to a SpecialFieldAttribute.
        /// </summary>
        protected virtual string GetSpecialFieldExpression(string classAlias, string expression)
        {
            if (expression.Contains("{0}"))
                expression = string.Format(expression, classAlias);

            return "(" + expression + ")";
        }

        /// <summary>
        /// Creates the SQL that can make a logical comparison between two parts.
        /// </summary>
        /// <param name="classType">The type that contains the property to be compared.</param>
        /// <param name="classAlias">The classAlias used.</param>
        /// <param name="propertyName">The property name to be compared.</param>
        /// <param name="value">The value to be compared against the property.</param>
        /// <param name="compareOperator">A logical operator.</param>
        /// <param name="castType">Specifies that a cast is needed on the table field.</param>
        /// <param name="parameterName">Ouputs the parameter generated by this method.</param>
        /// <returns>Returns the query part that compares a field with a valued parameter.</returns>
        protected virtual string CreateCompareSql(Type classType, string classAlias, string propertyName, object value, CompareOperator compareOperator, Type castType, out string parameterName)
        {
            StringBuilder str = new StringBuilder();
            PropertyInfo propInfo = classType.GetProperty(propertyName);
            FieldInfo fieldInfo = FieldInfo.Create(propInfo);
            SpecialFieldInfo spFieldInfo = SpecialFieldInfo.Create(propInfo);

            //Left side

            if (fieldInfo != null)
            {
                //TODO: Check if cast can be used on all dialects... 
                //I Guess not ¬¬
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
                //This property is not mapped. duh!
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
                    throw (new TenorException("Cannot use this operator with null expressions."));
                }
            }
            else
            {
                //TenorParameter p = new Parameter(ParameterName, Value);
                parameterName = null;

                if (fieldInfo != null)
                {
                    //Remove spaces
                    parameterName = fieldInfo.DataFieldName;
                }
                else if (spFieldInfo != null)
                {
                    parameterName = spFieldInfo.Alias;
                }
                //Remove spaces and changes the case to lower.
                parameterName = parameterName.ToLower().Trim().Replace(" ", "_");

                if (parameterName.Length > ParameterMaxLength)
                {
                    throw (new TenorException(string.Format("Cannot generate parameter name. Identifier \'{0}\' is too long.", parameterName)));
                }

                parameterName += Guid.NewGuid().ToString().Replace("-", "");
                if (parameterName.Length > ParameterMaxLength - ParameterIdentifier.Length)
                {
                    parameterName = parameterName.Substring(0, ParameterMaxLength - ParameterIdentifier.Length);
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
                        str = new System.Text.StringBuilder(GetContainsInFlagsExpression(str.ToString(), parameterName));
                        break;
                    default:
                        throw (new ArgumentOutOfRangeException("compareOperator", "Specified argument was out of the range of valid values."));
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Creates an expression that can bitwise check if the value is in a flags field.
        /// </summary>
        /// <param name="field">The table field name.</param>
        /// <param name="parameterName">The name of the parameter value to check.</param>
        /// <returns>Returns the query part that checks if the parameter is in the field value.</returns>
        protected abstract string GetContainsInFlagsExpression(string field, string parameterName);

        /// <summary>
        /// Creates the SQL that can order the results of the query.
        /// </summary>
        /// <param name="classType">The type that contains the property to be compared.</param>
        /// <param name="classAlias">The classAlias used.</param>
        /// <param name="propertyName">The property name to be compared.</param>
        /// <param name="sortOrder">A SortOrder value that defines how items are sorted.</param>
        /// <param name="castType">Specifies that a cast is needed on the table field.</param>
        /// <returns>Returns the query part that sorts the results.</returns>
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
                //TODO: Check if cast can be used on all dialects... 
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
                throw (new ArgumentException("Invalid property \'" + propertyName + "\' on class '" + classType.Name + "'. You must define a Field or a SpecialField property item.", "propertyName"));
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

        /// <summary>
        /// Creates the SQL that can filter the results. 
        /// </summary>
        /// <param name="conditions">The ConditionCollection used by this query.</param>
        /// <param name="baseClass">The base class that will be returned by the query.</param>
        /// <param name="joins">An array of Join.</param>
        /// <param name="parameters">Outputs an array of TenorParameters.</param>
        /// <param name="generateAliases">Defines whether to create aliases for each class on this query. If false, direct access to fields will be written.</param>
        /// <returns></returns>
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
                    string group = CreateWhereSql(((ConditionCollection)obj), baseClass, joins, out groupParameters, generateAliases);
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
                //TODO :Check if we can consider all fields from the parameter.
                //if (f.PrimaryKey || !f.LazyLoading)
                //{
                    
                    fieldsSql.Append(", " + alias + "." + CommandBuilder.QuoteIdentifier(f.DataFieldName));
                //
            }
            if (fieldsSql.Length == 0)
            {
                throw new MissingFieldsException(baseClass, true);
            }

            if (spFields != null)
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
                string tableName = GetPrefixAndTable(join.ForeignTableInfo.Prefix, join.ForeignTableInfo.TableName) + " " + join.JoinAlias;
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
                    fks.Append(join.ParentAlias + "." + CommandBuilder.QuoteIdentifier(join.ForeignKey.LocalFields[i].DataFieldName));
                    fks.Append(" =  ");
                    fks.Append(join.JoinAlias + "." + CommandBuilder.QuoteIdentifier(join.ForeignKey.ForeignFields[i].DataFieldName));
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
                if (limit > 0 && LimitAt == LimitType.Start)
                {
                    sql.Append(CreateLimit(limit) + " ");
                }
                sql.AppendLine(fieldsPart);
            }
            else
            {
                sql.Append(" COUNT(" + baseAlias + ".*) ");
            }


            string froms = GetPrefixAndTable(table.Prefix, table.TableName);

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

            froms += " " + baseAlias;
            sql.AppendLine(" FROM " + froms);
            sql.AppendLine(joinsPart);

            if (!string.IsNullOrEmpty(wherePart))
            {
                sql.Append(" WHERE ");
                sql.AppendLine(wherePart);
            }
            if (limit > 0 && LimitAt == LimitType.WhereClause)
            {
                if (string.IsNullOrEmpty(wherePart))
                {
                    sql.Append(" WHERE ");
                }
                else
                {
                    sql.Append(" AND ");
                }
                sql.AppendLine(" " + CreateLimit(limit) + " ");
            }


            if (!string.IsNullOrEmpty(sortPart))
            {
                sql.Append(" ORDER BY ");
                sql.AppendLine(sortPart.ToString());
            }

            if (limit > 0 && LimitAt == LimitType.End)
            {
                sql.Append(" " + CreateLimit(limit));
            }

            //sql.Append(LineEnding);

            return sql.ToString();

        }

        string IDialect.CreateSaveSql(Type baseClass, Dictionary<FieldInfo, object> data, NameValueCollection specialValues, ConditionCollection conditions, out TenorParameter[] parameters)
        {
            List<TenorParameter> parameterList = new List<TenorParameter>();
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            StringBuilder fieldAndValues = new StringBuilder();
            string alias = CreateClassAlias(baseClass);
            bool update = conditions != null && conditions.Count > 0;

            foreach (FieldInfo field in data.Keys)
            {
                string paramName = field.DataFieldName;
                
                // does nothing in case it's autonumber and doesn't get identity during query
                if (field.AutoNumber && string.IsNullOrEmpty(this.IdentityDuringQuery))
                    continue;

                TenorParameter param = new TenorParameter(this.ParameterIdentifier + paramName, data[field]);

                if ((field.AutoNumber && !this.GetIdentityOnSameCommand) || !field.AutoNumber)
                    parameterList.Add(param);

                fields.Append(", ");
                fields.Append(CommandBuilder.QuoteIdentifier(field.DataFieldName));

                values.Append(", ");

                if (specialValues == null || string.IsNullOrEmpty(specialValues[paramName]))
                {
                    if (field.AutoNumber && !update)
                    {
                        // adds the identity setter
                        values.Append(string.Format(this.IdentityDuringQuery, field.InsertSQL));
                    }
                    else
                    {
                        // The parameter as is.
                        values.Append(param.ParameterName);
                    }
                }
                else
                {
                    //Replaced by a SQL statement
                    values.Append(specialValues[paramName]);
                }

                if (!field.AutoNumber)
                {
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
            }

            if (fieldAndValues.Length > 0)
                fieldAndValues = fieldAndValues.Remove(0, 2);
            if (fields.Length > 0)
                fields = fields.Remove(0, 2);
            if (values.Length > 0)
                values = values.Remove(0, 2);


            TableInfo table = TableInfo.CreateTableInfo(baseClass);

            string query = null;
            if (update)
            {
                TenorParameter[] whereParameters = null;
                string clause = CreateWhereSql(conditions, baseClass, null, out whereParameters, false);
                parameterList.AddRange(whereParameters);

                query = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    GetPrefixAndTable(table.Prefix, table.TableName),
                    fieldAndValues,
                    clause);
            }
            else
            {
                query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                    GetPrefixAndTable(table.Prefix, table.TableName),
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


            string clause = CreateWhereSql(conditions, baseClass, joins, out parameters, false);

            return "DELETE FROM " + GetPrefixAndTable(table.Prefix, table.TableName) + " WHERE " + clause;
        }

        public virtual string GetPrefixAndTable(string prefix, string tableName)
        {
            string res = CommandBuilder.QuoteIdentifier(tableName);
            if (!string.IsNullOrEmpty(prefix))
            {
                res = prefix + "." + res;
            }

            return res;
        }

        #endregion
    }


    /// <summary>
    /// Specifies where the limit part of a query will be written.
    /// </summary>
    public enum LimitType
    {
        /// <summary>
        /// Writes the limit after the SELECT statement.
        /// </summary>
        Start,
        /// <summary>
        /// Writes the limit at the where clause. 
        /// </summary>
        WhereClause, 
        /// <summary>
        /// Writes the limit at the end of the query.
        /// </summary>
        End
    }
}
