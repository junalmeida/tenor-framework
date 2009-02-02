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


            public InvalidMappingException()
            {
            }

            public override string Message
            {
                get
                {
                    return "This class must inherit directly or indirecty from BLLBase.";
                }
            }
        }
		
		public class MissingFieldsException : InvalidMappingException
		{
			
			
			public MissingFieldsException()
			{
			}
			
			public override string Message
			{
				get
				{
					return "This class does not implement any member with a FieldAttribute instance.";
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
