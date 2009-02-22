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
		//Public Class MissingTableAttributeException
		//    Inherits Exception
		
		//    Public Sub New()
		//        MyBase.New()
		//    End Sub
		
		//    Public Overrides ReadOnly Property Message() As String
		//        Get
		//            Return "This class does not implement a TableAttribute instance."
		//        End Get
		//    End Property
		//End Class

        public class InvalidMappingException : Exception
        {
            Type type;

            internal InvalidMappingException(Type type)
            {
                this.type = type;
            }

            public override string Message
            {
                get
                {
                    return string.Format("The type '{0}' must inherit directly or indirecty from BLLBase.", type.FullName);
                }
            }
        }
		
		public class MissingFieldsException : InvalidMappingException
		{
            Type type;
            string propName;

            internal MissingFieldsException(Type type, string propName) : base (type)
            {
                this.type = type;
                this.propName = propName;
            }
			
			public override string Message
			{
				get
				{
                    if (string.IsNullOrEmpty(propName))
                    {
                        return string.Format("The type '{0}' does not implement any member with a FieldAttribute instance.", type.FullName);
                    }
                    else
                    {
                        return string.Format("The type '{0}' does not implement '{1}' with a FieldAttribute instance.", type.FullName, propName);
                    }
				}
			}
		}
		
		public class RecordNotFoundException : Exception
		{
			
			
			public override string Message
			{
				get
				{
					return "Could not find any record that matches the current filter conditions.";
				}
			}
		}
        public class ManyRecordsFoundException : Exception
        {


            public override string Message
            {
                get
                {
                    return "Found more than one record that matches the current filter conditions.";
                }
            }
        }
		
	}
	
}
