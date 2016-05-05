using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using TwitterSample.OAuth;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace TwitterSample
{
	/// <summary>
	/// Sample illustrating how to write a simple twitter client using HttpClient. The sample uses a 
	/// HttpMessageHandler to insert the appropriate OAuth authentication information into the outgoing
	/// HttpRequestMessage. The result from twitter is read as a JToken.
	/// </summary>
	/// <remarks>
	/// Before you can run this sample you must obtain an application key from twitter, and 
	/// fill in the information in the OAuthMessageHandler class, see 
	/// http://dev.twitter.com/ for details.
	/// </remarks>
	class Program
	{

		/// <summary>
		/// Runs the client.
		/// </summary>
		/// <returns>The client.</returns>
		private static async Task<JToken> RunClient(string searchKey) {
			string _address = setAddress (searchKey);
			// Create client and insert an OAuth message handler in the message path that 
			// inserts an OAuth authentication header in the request
			HttpClient client = new HttpClient(new OAuthMessageHandler(new HttpClientHandler()));

			// Send asynchronous request to twitter and read the response as JToken
			HttpResponseMessage response = await client.GetAsync(_address);
			JToken statuses = null;
	
			statuses = await response.Content.ReadAsAsync<JToken>();
			return statuses;
		}

		/// <summary>
		/// set the address.
		/// </summary>
		/// <param name="searchKey">Search key.</param>
		private static string setAddress(string searchKey) {
			string tmp = HttpUtility.UrlEncode (searchKey).Replace ('+', ' ');
			//string tmp1 = tmp.Replace (',', ' ');
			//string tmp2 = tmp1.Replace ("  ", " ");
			//string tmp3 = tmp2.Replace ("  ", " ");

			Console.WriteLine ("Ini adalah tmp : {0} ", tmp);
			string _address = "https://api.twitter.com/1.1/search/tweets.json?q="+tmp+"&result_type=mixed&count=100";
			Console.WriteLine ("ini adalah url : {0} ", _address);

			return _address;
		}

		/// <summary>
		/// Searchs the first place.
		/// </summary>
		/// <returns>The first place.</returns>
		/// <param name="text">Text.</param>
		private static string searchFirstPlace(string text) {
			int index;
			StringBuilder place = new StringBuilder ();

			string temptemp = text.ToLower ();
			string searchForThis1 = "d ";
			string searchForThis2 = "di ";
			int firstCharacterD = temptemp.IndexOf(searchForThis1);	//atau pake KMP, BM, bebas
			int firstCharacterDii = temptemp.IndexOf (searchForThis2);
			if (firstCharacterDii != -1) { //di found
				Console.WriteLine("di found!");
				index = firstCharacterDii + 3;
			} else {
				if (firstCharacterD != -1) { //d found
					Console.WriteLine ("d found!");
					index = firstCharacterD + 2;
				} else
					index = -1;
			}
				
			if (index != -1) {
				int i = index;
				while (text [i] != ' ') {
					place.Append(text[i]);
					i++;
				}
			}

			//check if the place is 'sana' or 'sini'
			string temp = place.ToString();
			if (temp == "sana" || temp == "sini" || temp == "sni" || temp == "sna") {
				place.Clear();
			}

			return place.ToString();
		}

		//ini buat uji coba, hapus aja
		private static void coba(string placeKey) {
			Console.WriteLine ();
			Console.WriteLine (placeKey);
			string place = searchFirstPlace (placeKey);
			if (place.Length != 0) {
				Console.WriteLine ("tempat : {0}", place);
			} else
				Console.WriteLine("Place not found!");
		}


		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		static void Main(string[] args) {
			string placeKey = "di sini ku sendiri";
			coba (placeKey);
			placeKey = "disana kau menungguku";
			coba (placeKey);
			placeKey = "d sni hujan";
			coba (placeKey);
			placeKey = "Di Bandung hujan";
			coba (placeKey);
			placeKey = "dI jKarta banjir";
			coba (placeKey);
			placeKey = "d SuRaVaya ku menangis";
			coba (placeKey);



			//JToken feed = null;
			//feed = RunClient (searchKey).Result;
		


			/*Console.WriteLine("Most recent statuses from {'s twitter account:");
			Console.WriteLine();
			Type str = typeof(String);
			var temp = System.Convert.ChangeType(feed.ToString(), str);
			System.IO.StreamWriter objWriter;
			string filename = "test.txt";
			objWriter = new System.IO.StreamWriter (filename);
			objWriter.Write (temp);
			objWriter.Close ();
			Console.WriteLine();
			//Console.WriteLine(feed);
			foreach (var status in feed["statuses"])
			{
				Console.WriteLine("   {0}", status["text"]);
				Console.WriteLine ("id : {0}", status ["id_str"]);
				Console.WriteLine();
			}

			Console.WriteLine("Hit ENTER to exit...");
			Console.ReadLine(); */
		}
	}
}
