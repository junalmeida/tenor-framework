using System;

namespace Tenor
{
    /// <summary>
    /// Indicates a general tenor internal logic exception.
    /// </summary>
    public class TenorException : Exception
    {
        internal TenorException()
        { }

        internal TenorException(string message) : base(message) { }

        internal TenorException(string message, Exception inner) : base(message, inner) { }
    }
}

namespace Tenor.Data
{
    /// <summary>
    /// Indicates that user-code passed an argument with a type that does not derive from EntityBase.
    /// </summary>
    public class InvalidTypeException : ArgumentException
    {
        Type type;
        public InvalidTypeException(Type type, string paramName)
            : base(null, paramName)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            this.type = type;
        }

        public override string Message
        {
            get
            {
                return string.Format("The type '{0}' must derive directly or indirectly from '{1}'.", type.FullName, typeof(EntityBase).FullName);
            }
        }

    }


    /// <summary>
    /// Occurs when an invalid mapping was done by user-code.
    /// </summary>
    public class InvalidMappingException : TenorException
    {
        private Type type;

        internal InvalidMappingException(Type type)
        {
            this.type = type;
        }

        public override string Message
        {
            get
            {
                return string.Format("The type '{0}' must inherit directly or indirecty from EntityBase.", RelatedClass.FullName);
            }
        }

        public Type RelatedClass
        {
            get
            {
                return this.type;
            }
        }
    }

    /// <summary>
    /// Occurs when TableAttribute is missing.
    /// </summary>
    public class MissingTableMetaDataException : InvalidMappingException
    {
        internal MissingTableMetaDataException(Type type)
            : base(type)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format("The type '{0}' does not have TableAttribute.", RelatedClass.FullName);
            }
        }
    }

    /// <summary>
    /// Occurs when primary key is missing.
    /// </summary>
    public class MissingPrimaryKeyException : InvalidMappingException
    {
        internal MissingPrimaryKeyException(Type type)
            : base(type)
        {
        }

        public override string Message
        {
            get
            {
                return string.Format("The type '{0}' does not implement any primary key property.", RelatedClass.FullName);
            }
        }

    }

    /// <summary>
    /// Occurs when a type does not have any mapped field.
    /// </summary>
    public class MissingFieldsException : InvalidMappingException
    {

        bool justLoadableItems;

        internal MissingFieldsException(Type type)
            : this(type, false)
        {
        }
        internal MissingFieldsException(Type type, bool justLoadableItems)
            : base(type)
        {
            this.justLoadableItems = justLoadableItems;
        }

        public override string Message
        {
            get
            {
                if (justLoadableItems)
                    return "Cannot find any field definition on loading '" + RelatedClass.FullName + "'. Please check your mapping.";
                else
                    return string.Format("The type '{0}' does not implement any member with a FieldAttribute instance.", RelatedClass.FullName);
            }
        }
    }


    /// <summary>
    /// Occurs when a type misses some field mapping.
    /// </summary>
    public class MissingFieldException : InvalidMappingException
    {
        string propName;
        bool anyFieldType;

        internal MissingFieldException(Type type, string propName)
            : this(type, propName, false)
        {
        }

        internal MissingFieldException(Type type, string propName, bool anyFieldType)
            : base(type)
        {
            this.propName = propName;
            this.anyFieldType = anyFieldType;
        }

        public override string Message
        {
            get
            {
                string message;
                if (anyFieldType)
                    message = string.Format("The type '{0}' does not implement '{1}' with a Field or a SpecialField attribute. You must define just one FieldAttribute or SpecialFieldAttribute for each property.", RelatedClass.FullName, propName);
                else
                    message = string.Format("The type '{0}' does not implement '{1}' with a Field attribute. You must define just one FieldAttribute for each property.", RelatedClass.FullName, propName);
                return message;
            }
        }
    }

    /// <summary>
    /// Occurs when when a type is missing some ForeignKeyAttribute.
    /// </summary>
    public class MissingForeignKeyException : InvalidMappingException
    {
        string propName;

        internal MissingForeignKeyException(Type type, string propName)
            : base(type)
        {
            this.propName = propName;
        }

        public override string Message
        {
            get
            {
                return string.Format("The type '{0}' does not implement '{1}' with a ForeignKey attribute.", RelatedClass.FullName, propName);
            }
        }

    }

    /// <summary>
    /// Occurs when a type is missing some SpecialFieldAttribute.
    /// </summary>
    public class MissingSpecialFieldException : InvalidMappingException
    {
        string propName;

        internal MissingSpecialFieldException(Type type, string propName)
            : base(type)
        {
            this.propName = propName;
        }

        public override string Message
        {
            get
            {
                return string.Format("The type '{0}' does not implement '{1}' with a SpecialField attribute. You must define just one SpecialFieldAttribute for each property.", RelatedClass.FullName, propName);
            }
        }

    }

    /// <summary>
    /// Occurs when a type has an invalid many-to-many mapping.
    /// </summary>
    public class InvalidManyToManyMappingException : InvalidMappingException
    {
        string propName;

        internal InvalidManyToManyMappingException(Type type, string propName)
            : base(type)
        {
            this.propName = propName;
        }

        public override string Message
        {
            get
            {
                return string.Format("The type '{0}' has an invalid many-to-many mapping on property '{1}'.", RelatedClass.FullName, propName);
            }
        }
    }

    /// <summary>
    /// Occurs when no record was found on a read operation.
    /// </summary>
    public class RecordNotFoundException : TenorException
    {
        public override string Message
        {
            get
            {
                return "Could not find any record that matches the current filter conditions.";
            }
        }
    }

    /// <summary>
    /// Occurs when more than one record was found on a many to one relation.
    /// </summary>
    public class ManyRecordsFoundException : TenorException
    {
        public override string Message
        {
            get
            {
                return "Found more than one record that matches the current filter conditions.";
            }
        }
    }

    internal enum CollectionProblem
    {
        InvalidLastItem,
        NullCollection,
        InvalidJoin
    }

    /// <summary>
    /// Occurs when condition information is missing or mistaken.
    /// </summary>
    public class InvalidCollectionArgument : TenorException
    {
        CollectionProblem type;
        string item;
        internal InvalidCollectionArgument(CollectionProblem type)
            : this(type, null)
        {
        }

        internal InvalidCollectionArgument(CollectionProblem type, string item)
        {
            this.type = type;
            this.item = item;
        }

        public override string Message
        {
            get
            {
                switch (type)
                {
                    case CollectionProblem.InvalidLastItem:
                        return "Cannot have a collection that ends with an operator.";
                    case CollectionProblem.NullCollection:
                        return "Cannot have null or an empty ConditionCollecion.";
                    case CollectionProblem.InvalidJoin:
                        return string.Format("Cannot find the join alias '{0}'.", item);
                    default:
                        throw new TenorException();
                }
            }
        }
    }

    /// <summary>
    /// Occurs when a field belonging to a collection association is given when paging
    /// </summary>
    public class InvalidSortException : TenorException
    {
        public InvalidSortException()
            : base()
        {
        }

        public InvalidSortException(string message)
            : base(message)
        {
        }
    }


    /// <summary>
    /// Occurs when a projection with an invalid alias or Field was specified.
    /// </summary>
    public class InvalidProjectionException : TenorException
    {
        public Projection Projection
        { get; private set; }


        public InvalidProjectionException(Projection projection)
        {
            this.Projection = projection;
        }

        public override string Message
        {
            get
            {
                return string.Format("The specified projection '{0}' is invalid. Check aliases and field names.", Projection);
            }
        }
    }
}

namespace Tenor.Web
{
    /// <summary>
    /// Occurs when user-code tries to make an operation that requires a web context.
    /// </summary>
    public class InvalidContextException : TenorException
    {
        public override string Message
        {
            get
            {
                return "You must be in a web context to use this methods.";
            }
        }
    }
}