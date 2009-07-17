using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace Text
	{
		
		/// <summary>
		/// Abstract base class for all SoundEx implementations
		/// Could also be used for other phonetic matching algorithms
		/// such as "Metaphone"/"Metaphon".
		/// </summary>
		public abstract class SoundEx
		{
			
			public abstract string GenerateSoundEx(string s);
			
			public abstract void ValidateAlgorithm();
			
			/// <summary>
			/// Implements the Difference algorithm, as found in SQL Server
			/// </summary>
			/// <returns>0-4 depending on the similarity of the two words</returns>
			public virtual int Difference(string s1, string s2)
			{
				throw (new NotImplementedException());
			}
			
			/// <summary>
			/// Marked as virtual so that concrete ISoundEx implementations can
			/// replace this and add extra characters for encoding.
			/// For example, the Online Dictionary of Computings specifies
			/// several extra characters in the lookup table.
			/// </summary>
			protected virtual string EncodeChar(char c)
			{
				//' C# will re-order this list and produce a look-up list from it
				//' C# will do all the work we would otherwise do by building arrays of values
				switch (char.ToLower(c))
				{
					case 'b':
					case 'f':
					case 'p':
					case 'v':
						return "1";
					case 'c':
					case 'g':
					case 'j':
					case 'k':
					case 'q':
					case 's':
					case 'x':
					case 'z':
						return "2";
					case 'd':
					case 't':
						return "3";
					case 'l':
						return "4";
					case 'm':
					case 'n':
						return "5";
					case 'r':
						return "6";
					default:
						return string.Empty;
				}
			}
		}
	}
	
}
