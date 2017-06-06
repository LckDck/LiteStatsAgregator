using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LiteStatsAgregator
{
	public class HtmlReader
	{
		private static HtmlReader _instance;

		private HtmlReader ()
		{ }

		public static HtmlReader Instance {
			get {
				if (_instance == null)
					_instance = new HtmlReader ();
				return _instance;
			}
		}

		public async Task<string> ReadHtml (string url)
		{
			var result = string.Empty;
			using (var httpClient = new HttpClient ()) {
				try {
					result = await httpClient.GetStringAsync (url);
				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
			}

			return result;
		}
	}
}
