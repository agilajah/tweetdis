using System;
using System.Web;
using System.Web.UI;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TweetDis;

using TwitterSample;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TweetDis
{
	
	public class GetDis : System.Web.IHttpHandler
	{
	
		public void ProcessRequest (HttpContext context)
		{
			
			// Get keywordSearch from user
			// Contoh keywordSearch: #pemkotbdg, @ridwan_kamil
			string keywordSearch = context.Request.QueryString.Get ("keyword");

			bool useBm = context.Request.QueryString.Get ("algoritma") == "bm";

			// Get keywords for each dinas
			// Contoh keyword: dinas1->pipa, bocor, air
			string[][] keywordDinas = new string[5][];
				keywordDinas[0] = context.Request.QueryString.Get ("dinas1").Split (',');
				keywordDinas[1] = context.Request.QueryString.Get ("dinas2").Split (',');
				keywordDinas[2] = context.Request.QueryString.Get ("dinas3").Split (',');
				keywordDinas[3] = context.Request.QueryString.Get ("dinas4").Split (',');
				keywordDinas[4] = context.Request.QueryString.Get ("dinas5").Split (',');

			// Get tweets from Twitter API
			JToken feed = null;
			Task<JToken> feedTask = Program.RunClient(keywordSearch);
			KeywordClassifier kc = new KeywordClassifier(keywordDinas, useBm);
			feed = feedTask.Result;

			/*
			List<string> tweets = new List<string>();
			List<string> tweetsID = new List<string>();
			List<string> tweetsUser = new List<string>();
			List<string> tweetsCoord = new List<string>();
			foreach (var status in feed["statuses"])
			{
				tweets.Add(status["text"].ToString());
				tweetsID.Add(status ["id_str"].ToString());
				tweetsUser.Add (status ["user"] ["name"].ToString ());
			}

			// Classify each tweet 
			for (int i = 0; i < tweets.Count; i++) {
				kc.classify (tweets[i], i);
				Console.WriteLine ();
			}
			*/
			// Save tweets, 
			JArray jTweets = new JArray ();
			foreach (var status in feed["statuses"]) {
				JObject jTweet = new JObject ();
				jTweet ["id"] = status ["id_str"].ToString ();
				string isiTweet = status ["text"].ToString ();
				jTweet ["isi"] = isiTweet;
				jTweet ["dinas"] = kc.classify (isiTweet);
				jTweet ["penulis"] = status ["user"] ["name"].ToString ();

				if (status ["coordinates"].Type != JTokenType.Null) {
					Console.WriteLine("coords!!!");
					jTweet ["koordinat"] = status ["coordinates"];
				}

				string tempat = Program.searchFirstPlace (isiTweet);
				if (tempat.Length > 0) {
					jTweet ["lokasi"] = tempat;
				}
					
				jTweets.Add (jTweet);
			}
		
			// pemrosesan selesai
			context.Response.Write(JsonConvert.SerializeObject(jTweets));
		}

		public bool IsReusable {
			get {
				return false;
			}
		}
	}
}
	
