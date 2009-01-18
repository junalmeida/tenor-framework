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
		/// <summary>
		/// Operadores disponíveis para a comparação de dados
		/// </summary>
		/// <remarks></remarks>
		public enum CompareOperator
		{
			/// <summary>
			/// Se é igual
			/// </summary>
			/// <remarks></remarks>
			Equal,
			/// <summary>
			/// Se não é igual
			/// </summary>
			/// <remarks></remarks>
			NotEqual,
			/// <summary>
			/// Menor que
			/// </summary>
			/// <remarks></remarks>
			LessThan,
			/// <summary>
			/// Maior que
			/// </summary>
			/// <remarks></remarks>
			GreaterThan,
			/// <summary>
			/// Menor ou igual
			/// </summary>
			/// <remarks></remarks>
			LessThanOrEqual,
			/// <summary>
			/// Maior ou igual
			/// </summary>
			/// <remarks></remarks>
			GreaterThanOrEqual,
			/// <summary>
			/// Operador Like para strings. Utilize % como caracter curinga.
			/// </summary>
			/// <remarks></remarks>
			@Like,
			/// <summary>
			/// Se não é Like. Utilize % como caracter curinga.
			/// </summary>
			/// <remarks></remarks>
			NotLike,
			/// <summary>
			/// Comparador para verificar se o valor contém na flag.
			/// </summary>
			/// <remarks></remarks>
			ContainsInFlags
		}
		
		
		/// <summary>
		/// Operadores disponíveis para portas lógicas.
		/// Não haverão outros operadores além de AND e OR.
		/// </summary>
		/// <remarks></remarks>
		public enum LogicalOperator
		{
			/// <summary>
			/// Operador lógico AND
			/// </summary>
			/// <remarks></remarks>
			@And,
			/// <summary>
			/// Operador lógico OU
			/// </summary>
			/// <remarks></remarks>
			@Or
		}
		
		/// <summary>
		/// Direção da ordenação, ascendente ou descendente
		/// </summary>
		/// <remarks></remarks>
		public enum SortOrder
		{
			/// <summary>
			/// Ordenar de forma ascendente
			/// </summary>
			/// <remarks></remarks>
			Ascending,
			/// <summary>
			/// Ordenar de forma descendente
			/// </summary>
			/// <remarks></remarks>
			Descending,
			/// <summary>
			/// Não ordenar
			/// </summary>
			/// <remarks></remarks>
			None
		}
		
	}
}
