using System;
using System.Text;
using System.Collections.Generic;

namespace StringMatching {
	
	/// <summary>
	/// String matching using Knuth-Morris-Pratt Algorithm
	/// </summary>
	public class KnuthMorrisPratt {
		//field
		private static List<int> longestPreSuf = new List<int>();

		/// <summary>
		/// Computes the LPS array.
		/// </summary>
		/// <param name="needle">The needle we are looking for in haystack</param>
		/// <param name="M">The needle length/param>
		/// <param name="longestPreSuf">Longest Prefix Suffix counted used for jumping when mismatch occured.</param>
		private static void computeLPSArray(string needle, int M, List<int> longestPreSuf) {
			int len = 0;  // length of the previous longest prefix suffix
			int i;

			longestPreSuf.Add(0); // longestPreSuf[0] is always 0
			i = 1;

			// the loop calculates longestPreSuf[i] for i = 1 to M-1
			while (i < M) {
				if (needle[i] == needle[len]) {
					len++;
					//longestPreSuf.IndexOf(i) = len;
					longestPreSuf.Add(len);
					i++;
				}
				else // (needle[i] != needle[len]) 
				{
					if (len != 0) {
						// This is tricky. Consider the example 
						// AAACAAAA and i = 7.
						len = longestPreSuf[len-1];

						// Also, note that we do not increment i here
					}
					else // if (len == 0)
					{
						//longestPreSuf.IndexOf(i) = 0;
						longestPreSuf.Add(0);
						i++;
					}
				}
			}
		}

		/// <summary>
		/// Searching needle in the haystack by call this method.
		/// </summary>
		/// <param name="needle">Needle we are looking for</param>
		/// <param name="haystack">Haystack, the source that (maybe) contains needle(s).</param>
		public static int KMPSearch(string haystack, string needle) {
			bool found = false;
			int result=-1;	//-1 by default, means no needle in the haystack found
			int M = needle.Length;
			int N = haystack.Length;
			// create longestPreSuf[] dynamically that will hold the longest prefix suffix
			// values for pattern

			int j  = 0;  // index for pattern

			// Preprocess the pattern (calculate longestPresSuf[] array)
			computeLPSArray(needle, M, longestPreSuf);
			Console.WriteLine ("Needle   : {0} ", needle);
			Console.WriteLine ("Haystack : {0}", haystack);
			int i = 0;  // index for haystack
			while (i < N) {
				if (needle[j] == haystack[i]) {
					j++;
					i++;
				}

				if (j == M)
				{
					found = true;
					result = i - j;
					j = longestPreSuf[j-1];
				}

				// mismatch after j matches
				else if (i < N && needle[j] != haystack[i]) {
					// Do not match longestPreSuf[0..longestPreSuf[j-1]] characters,
					// they will match anyway
					if (j != 0)
						j = longestPreSuf[j-1];
					else
						i = i+1;
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Boyer moore using Bad Character heuristic
	/// </summary>
	class BoyerMoore {

		private static int[] right;     // the bad-character skip array

		public static int BMSearch(String haystack, String needle) {
			int R = 512;
			String pat = needle;
			String txt = haystack;

			// position of rightmost occurrence of c in the pattern
			right = new int[R];
			for (int c = 0; c < R; c++)
				right[c] = -1;
			for (int j = 0; j < pat.Length; j++)
				right[(int)pat[j]] = j;

			int M = pat.Length;
			int N = txt.Length;
			int skip;
			for (int i = 0; i <= N - M; i += skip) {
				skip = 0;
				for (int j = M-1; j >= 0; j--) {
					if (pat[j] != txt[i+j]) {
						if (txt [i + j] >= 512) {
							skip = 1;
						} else {
							skip = Math.Max(1, j - right[txt[i+j]]);
						}
						break;
					}
				}
				if (skip == 0) return i;    // found
			}
			return -1;     //not found
		}
	}
	/*
	///<summary>
	/// Class for string matching using boyer-moore algorithm
	/// </summary>  
	class BoyerMoore {
		/// <summary>
		/// IndexOf is method that used for searching needle in the haystack, and returns index of the needle 
		/// that has been found
		/// </summary>
		/// <returns>index</returns>
		/// <param name="haystack">Haystack.</param>
		/// <param name="needle">Needle.</param>
		public static int BMSearch(string haystack, string needle) {
			
			if (needle.Length == 0) {
				return 0;
			}
			//creating charTable and offsetTable
			List<int> charTable = makeCharTable(needle);
			List<int> offsetTable = makeOffsetTable(needle);
			for (int i = needle.Length - 1, j; i < haystack.Length;) {
				for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j) {
					if (j == 0) {
						return i;
					}
				}
				//i += needle.Length - j; // For naive method
				i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
			}
			return -1;
		}
			
		/// <summary>
		/// Makes the jump table based on the mismatched character information.
		/// </summary>
		/// <returns>The char table.</returns>
		/// <param name="needle">Needle.</param>
		private static List<int> makeCharTable(string needle) {
			List<int> table = new List<int>(256);
			for (int i = 0; i < table.Capacity; ++i) {
				table.Insert(i, needle.Length);
			}
			for (int i = 0; i < needle.Length - 1; ++i) {
				table.Insert (needle [i], needle.Length - 1 - i);
			}
			return table;
		}


		/// <summary>
		/// Makes the offset table. offset table is jump table based on the scan offset which mismatch occurs
		/// </summary>
		/// <returns>The offset table.</returns>
		/// <param name="needle">Needle.</param>
		private static List<int> makeOffsetTable(string needle) {
			List<int> table = new List<int>(needle.Length);
			int lastPrefixPosition = needle.Length;
			for (int i = needle.Length - 1; i >= 0; --i) {
				if (isPrefix(needle, i + 1)) {
					lastPrefixPosition = i + 1;
				}
				//table.Insert (index, Value);
				table.Insert(needle.Length - 1 - i, lastPrefixPosition - i + needle.Length - 1);
			}
			for (int i = 0; i < needle.Length - 1; ++i) {
				int slen = suffixLength(needle, i);
				//table.Insert(index, Value)
				table.Insert(slen, needle.Length - 1 - i + slen); 
			}
			return table;
		}


		/// <summary>
		/// Is needle[p:end] a prefix of needle?
		/// </summary>
		/// <returns>The prefix.</returns>
		/// <param name="needle">Needle.</param>
		/// <param name="p">P.</param>
		private static bool isPrefix(string needle, int p) {
			for (int i = p, j = 0; i < needle.Length; ++i, ++j) {
				if (needle[i] != needle[j]) {
					return false;
				}
			}
			return true;
		}
			
		/// <summary>
		/// Returns the maximum length of the substring ends at p and is a suffix
		/// </summary>
		/// <returns>The length.</returns>
		/// <param name="needle">Needle.</param>
		/// <param name="p">P.</param>
		private static int suffixLength(string needle, int p) {
			int len = 0;
			for (int i = p, j = needle.Length - 1;
				i >= 0 && needle[i] == needle[j]; --i, --j) {
				len += 1;
					}	
					return len;
		}
	}
	*/


	class ExecuteKMP_BM {

		//ini cuma method buat ngecheck
		private static void write(int index, int mode) {
			string tambahan = "";
			if (mode == 1) {
				tambahan = "in KMP";
			} else {
				tambahan = "in BM";
			}
				
			if (index == -1) {
				Console.WriteLine("Needle not found {0}!", tambahan);
			} else
				Console.WriteLine("Needle found at index {0} {1}.", index, tambahan);
		}

		//ini juga cuma method buat checking
		private static void isSama(int res, int hasil) {
			if (res == hasil) {
				Console.WriteLine ("KMP and BM returns same index.");
			} else {
				Console.WriteLine ();
				Console.WriteLine ("\tKMP and BM DID NOT returns same index. ");
				Console.WriteLine ("\tit's so damn weird. Am I doing something wrong over there?");
				Console.WriteLine ("\tOr it is normal? and that's the way it should be?");
				Console.WriteLine ("\tBetter help me check my algorithm.");
				Console.WriteLine ();
			}
		}
			
		//lagi-lagi ini cuma method buat checking
		private static void checking(string haystack,string needle) {
			int hasil = KnuthMorrisPratt.KMPSearch(haystack, needle); //searching pattern using KMP
			write(hasil, 1);
			int res = BoyerMoore.BMSearch (haystack, needle);	  //searching pattern using BM
			write(res, 2);
			isSama (res, hasil);	//checking is KMP result is the same with BM result
		}
		public static void Main (string[] args) {
			string haystack = "ABABDABACDABABCABAB";
			string needle = "ABABCABAB";
			checking (haystack, needle);
			Console.WriteLine ("Testcase 1 finished");	
			Console.WriteLine ("--------------------------");
			Console.WriteLine ();

			haystack = "abacadabrabracabracadabrabrabracad";
			needle = "abacad";
			checking (haystack, needle);
			Console.WriteLine ("Testcase 2 finished");							
			Console.WriteLine ("--------------------------");
			Console.WriteLine ();

			haystack = "abacadabrabracabracadabrabrabracad";
			needle = "rabrabracad";
			checking (haystack, needle);
			Console.WriteLine ("Testcase 3 finished");
			Console.WriteLine ("--------------------------");
			Console.WriteLine ();

			haystack = "abacadabrabracabracadabrabrabracad";
			needle = "bcara";
			checking (haystack, needle);
			Console.WriteLine ("Testcase 3 finished");	
			Console.WriteLine ("--------------------------");
			Console.WriteLine ();
		
			haystack = "huahahahah slalu heheheh huhuhu hehehe";
			needle = "slalu";
			checking (haystack, needle);
			Console.WriteLine ("Testcase 4 finished");	
			Console.WriteLine ("--------------------------");
			Console.WriteLine ();
		}
	}
}
