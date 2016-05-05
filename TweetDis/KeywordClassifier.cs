using System;
using System.Collections.Generic;
using StringMatching;

namespace TweetDis
{
	public class KeywordClassifier
	{
		List<string> dinasList;		// List dari dinas-dinas Kota Bandung
		public List<int> data;
		private string[][] keyword;
		private TypoCorrector corrector;
		private bool useBm;

		// Constructor
		public KeywordClassifier (string[][] keyw, bool ub)
		{
			dinasList = new List<string>();
			data = new List<int> ();
			dinasList.Add("Dinas Bina Marga dan Pengairan");	// 0
			dinasList.Add("Dinas Kesehatan");					// 1
			dinasList.Add("Dinas Pendidikan");					// 2
			dinasList.Add("Dinas Perhubungan");					// 3
			dinasList.Add("Dinas Sosial");						// 4
			dinasList.Add("Unknown");							// 5

			keyword = (string[][]) keyw.Clone ();
			List<string> words = new List<string> ();
			for (int i = 0; i < keyword.Length; i++) {
				for (int j = 0; j < keyword[i].Length; j++) {
					words.Add (keyword [i] [j]);
				}
			}

			corrector = new TypoCorrector (words);

			useBm = ub;
		}

		// Cetak dinasList ke layar
		public void printDinasList() 
		{
			Console.WriteLine("{0} dinas", dinasList.Count);
			foreach (string str in dinasList)
				Console.WriteLine (str);
		}

		// FUNGSI UTAMA
		// Mengklasifikasikan 'tweet' berdasarkan 'keyword' ke dalam dinas yang tepat pada dinasList
		public int classify(string tweet_initial) {
			int index = -1, iDinas = -1, i = 0;
			int min = -1;
			string str = null;
			string tweet = corrector.Corrected (tweet_initial);
			// Search keyword in tweet using KMP
			foreach (string[] dinas in keyword) {
				foreach (string key in dinas) {
					if (!useBm) {
						index = KnuthMorrisPratt.KMPSearch (tweet, key);
					} else {
						index = BoyerMoore.BMSearch (tweet, key);
					}

					if (index != -1) {
						if (min == -1) {
							min = index;
							str = key;
							iDinas = i;
						} else {
							if (index < min) {
								min = index;
								str = key;
								iDinas = i;
							}
						}
					}
				}
				i++;
			}
			Console.WriteLine ();
			if (min != -1) {
				Console.WriteLine ("Found {0} in index {1}\nDisposisi \"{2}\" ke dinas {3}", str, min, tweet, this.dinasList [iDinas]);
				data.Add(iDinas);
				return iDinas;
			} else {
				Console.WriteLine ("Couldn't find any keyword in tweet\nDisposisi \"{0}\" ke dinas {1}", tweet, this.dinasList [dinasList.Count - 1]);
				data.Add(dinasList.Count - 1);
				return dinasList.Count - 1;
			}
		}

		/*
		public static void Main() {
			// Input keyword dari user
			string[][] keywords = new string[][] {
				new string[] {"pipa", "bocor", "mati"},		// 0
				new string[] {"sakit", "sehat"},	// 1
				new string[] {"spp", "ijazah"},	// 2
				new string[] {"macet", "padat", "lalu", "lintas"},	// 3
				new string[] {"pengemis", "rusak", "bising"}		// 4 
			}; 

			// Input tweet dari API
			string[] tweet = new string[] {
				"Aku bukan pipa rusak",
				"taman bunga pipa bocor rusak dan mati.",
				"Aku galau qaqa",
				"Pak @ridwankamil, ijazah saya disembunyiin guru saya",
				"macet parah di jalan ke hatimu"
			};

			int n = tweet.Length;
			KeywordClassifier kc = new KeywordClassifier(n);

			// Contoh penggunaan fungsi classify
			for (int i = 0; i < tweet.Length; i++) {
				kc.classify (tweet[i], keywords, i);
				Console.WriteLine ();
			}
		}
		*/
	}
}

