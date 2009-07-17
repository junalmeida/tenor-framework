using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Tenor
{
	namespace Text
	{
		/// <summary>
		/// This SoundEx is described in Knuth TAOCP Vol3 Edition 2, pg 394-395
		/// </summary>
		public class KnuthEd2SoundEx : SoundEx
		{
			
			
			
			public override string GenerateSoundEx(string s)
			{
				StringBuilder output = new StringBuilder();
				
				if (s.Length > 0)
				{
					
					output.Append(char.ToUpper(s[0]));
					
					//' Stop at a maximum of 4 characters
					for (int i = 1; i <= s.Length - 1; i++)
					{
						if (output.Length >= 4)
						{
							break;
						}
						string c = EncodeChar(s[i]);
						
						// We either append or ignore, determined by the preceding char
						if ((((((char.ToLower(s[i - 1])) == 'a') || ((char.ToLower(s[i - 1])) == 'e')) || ((char.ToLower(s[i - 1])) == 'i')) || ((char.ToLower(s[i - 1])) == 'o')) || ((char.ToLower(s[i - 1])) == 'u'))
						{
							//' Chars separated by a vowel - OK to encode
							output.Append(c);
							
							/*
                            case 'h':
							case 'w':
                             */
						}
						else
						{
							//' Ignore duplicated phonetic sounds
							if (output.Length == 1)
							{
								//' We only have the first character, which is never
								//' encoded. However, we need to check whether it is
								//' the same phonetically as the next char
								if (EncodeChar(output[output.Length - 1]) != c)
								{
									output.Append(c);
								}
							}
							else
							{
								if (output[output.Length - 1].ToString() != c)
								{
									output.Append(c);
								}
							}
						}
					}
					
					//' Pad with zeros
					for (int i = output.Length; i <= 3; i++)
					{
						output.Append("0");
					}
				}
				
				return output.ToString();
			}
			
			private void AssertEquals(string s1, string s2, string @error)
			{
				if (! s1.Equals(s2))
				{
					throw (new Exception(@error + ". Expected " + s2 + " but got " + s1));
				}
			}
			
			public override void ValidateAlgorithm()
			{
				//' Validate the SoundEx agorithm
				//' using Knuth TAOCP volume 3
				
				AssertEquals(GenerateSoundEx("Euler"), "E460", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Ellery"), "E460", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Gauss"), "G200", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Ghosh"), "G200", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Hilbert"), "H416", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Heilbronn"), "H416", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Knuth"), "K530", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Kant"), "K530", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Lloyd"), "L300", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Ladd"), "L300", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Lukasiewicz"), "L222", "SoundEx Algorithm Broken");
				AssertEquals(GenerateSoundEx("Lissajous"), "L222", "SoundEx Algorithm Broken");
				//' Added in second edition of TAOCP for the h-w grouping rule
				AssertEquals(GenerateSoundEx("Wachs"), "W200", "SoundEx Algorithm Broken");
				
			}
			
			
			
		}
	}
}
