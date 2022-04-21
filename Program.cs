using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExchangeRate
{
    class Rates
    {
        internal static string currencyCodesUrl = "https://v6.exchangerate-api.com/v6/20c883f5603cbacae1df520e/codes";
        internal static string currencyRatesUrl = "https://open.er-api.com/v6/latest/";

        internal static string app { get; set; } = string.Empty;
        internal static string fromCurrency { get; set; } = string.Empty;
        internal static string toCurrency { get; set; } = string.Empty;
        internal static List<string> availableCurrencies { get; set; } = new List<string>();

        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();

            if (arguments.Length != 3) 
            {
                ShowUsage();
                return;
            }

            app = Path.GetFileName(arguments[0]);
            fromCurrency = arguments[1];
            toCurrency = arguments[2];

            LoadCurrencyCodes();

            if (!availableCurrencies.Contains(fromCurrency)) 
            {
                Console.WriteLine("Unknown currency: {0} ", fromCurrency);
                return;
            }

            if (!availableCurrencies.Contains(toCurrency)) 
            {
                Console.WriteLine("Unknown currency: {0} ", toCurrency);
                return;
            }
			
            string URLString = string.Format("{0}{1}", currencyRatesUrl, fromCurrency);
            JObject response = WebClient(URLString);
			if ((string) response["result"] == "error") {
				Console.WriteLine("API error: {0}", response["error-type"]);
				return;
			}
            Console.WriteLine("Exchange rate {0} -> {1} is: {2}", fromCurrency, toCurrency, response["rates"][toCurrency]);

            return;
        } 

        private static JObject WebClient(string url)
        {
            var webClient = new System.Net.WebClient();
            return (JObject)JsonConvert.DeserializeObject( webClient.DownloadString(url) );
        }

        private static void ShowUsage() 
        {
            Console.WriteLine("\n");
            Console.WriteLine("Usage :   {0} <currency> <currency>", app);
            Console.WriteLine("Example : {0} USD EUR", app);
        }

        private static void LoadCurrencyCodes()
        {
            JObject response = WebClient(currencyCodesUrl);
            foreach (var currency in response["supported_codes"])
            {
                availableCurrencies.Add((string) currency[0]);
            }
        }
    }
}