using System;
using System.Collections.Generic;
using System.Text;

namespace TweetDis
{
	[Serializable]
	public class TypoCorrector
	{
		private HashSet<string> words;
		private Dictionary<string, string> prebuiltCorrector;

		private bool IsAToZ(char c) {
			return c >= 'a' && c <= 'z';
		}

		private ISet<string> KnownEdits(string word, int level) {
			HashSet<string> candidates = new HashSet<string> ();
			HashSet<string> result = new HashSet<string> ();

			candidates.Add (word);


			for (int cl = 0; cl < level; cl++) {
				HashSet<string> nc = new HashSet<string> ();



				foreach (var w in candidates) {
					foreach (var rw in Edits(w)) {
						if (words.Contains (rw))
							result.Add (rw);
						if (cl != level - 1)
							nc.Add (rw);
					}
				}

				candidates = nc;
			}

			return result;
		}

		private ISet<string> Edits(string word) {
			int n = word.Length;

			HashSet<string> result = new HashSet<string>();

			result.Add (word);

			for (int i = 0; i < n+1; i++) {

				string a = word.Substring (0, i);
				string b = word.Substring (i, n - i);

				// deletes
				if (b.Length > 0) result.Add(a + b.Substring(1));

				// transposes
				if (b.Length > 1) result.Add(a + b[1] + b[0] + b.Substring(2));

				for (char c = 'a'; c <= 'z'; c++) {
					// replaces
					if (b.Length > 0) result.Add(a + c + b.Substring(1));

					// inserts
					result.Add(a + c + b);
				}
			}

			return result;

		}

		public TypoCorrector (IReadOnlyList<string> ws)
		{
			words = new HashSet<string> (ws);
			prebuiltCorrector = new Dictionary<string, string> ();
			foreach (var word in words) {
				if (word.Length > 3) {
					foreach (var ke in KnownEdits(word, 2)) {
						if (!prebuiltCorrector.ContainsKey (ke)) {
							prebuiltCorrector.Add (ke, word);
						} else if (prebuiltCorrector [ke].Length < word.Length) {
							prebuiltCorrector.Remove (ke);
							prebuiltCorrector.Add (ke, word);
						}
					}
				}
			}

			foreach (var word in words) {
				if (prebuiltCorrector.ContainsKey (word)) {
					prebuiltCorrector.Remove (word);
					prebuiltCorrector.Add (word, word);
				}
			}
		}

		public string CorrectWord(string s) {
			if (prebuiltCorrector.ContainsKey (s))
				return prebuiltCorrector [s];

			return s;
		}

		public string Corrected(string s) {
			StringBuilder sb = new StringBuilder ();

			string ls = s.ToLower ();

			bool text = IsAToZ(ls[0]);

			for (int i = 0, j = 0; j <= ls.Length; j++) {
				bool iaz = false;
				if (j < ls.Length)
					iaz = IsAToZ (ls [j]);
				else
					iaz = !text;

				if (text && !iaz ||
					!text && iaz) {
					text = iaz;

					string sub = ls.Substring(i, j-i);

					if (!iaz) {
						sub = CorrectWord (sub);
					}

					sb.Append (sub);

					i = j;
				}
			}

			return sb.ToString();
		}
	}
}
