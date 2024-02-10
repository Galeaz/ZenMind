using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ZenMind.Resources.Functions.Pseudo
{
    public class FitbitApiClient
    {
        private readonly HttpClient _httpClient;

        public FitbitApiClient(string accessToken)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<string> GetUserDataAsync(string apiEndpoint)
        {
            var response = await _httpClient.GetAsync($"https://api.fitbit.com/1/user/-/{apiEndpoint}.json");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

    }

    public class SleepData
    {
        public DateTime Date { get; set; }
        public TimeSpan AmountSlept { get; set; }

        public TimeSpan REMSleep { get; set; }

        public bool ManualEntry { get; set; }
    }

    public class SleepRecommendationSystem
    {
        private List<SleepData> _sleepEntry;
        private readonly FitbitApiClient _fitbitApiClient;

        public SleepRecommendationSystem(string accessToken)
        {
            _sleepEntry = new List<SleepData>();
            _fitbitApiClient = new FitbitApiClient(accessToken);
        }

        public void AddSleepEntry(SleepData sleepData)
        {
            _sleepEntry.Add(sleepData);
        }

        public TimeSpan GetSleepTime()
        {
            if(!_sleepEntry.Any()) return TimeSpan.Zero;

            var averageSleep = (long)_sleepEntry.Average(s => s.AmountSlept.Ticks);
            return new TimeSpan(averageSleep);
        }

        public async Task ImportSleepDataFromFitBit(DateTime startDate,DateTime endDate)
        {
            string dateFormat = "yyyy-MM-dd";
            string apiEndpoint = $"sleep/date/{startDate.ToString(dateFormat)}/{endDate.ToString(dateFormat)}";

            string content = await _fitbitApiClient.GetUserDataAsync(apiEndpoint);

            var sleepData = ParseFitbitSleepData(content);
            foreach(var sleep in sleepData)
            {
                AddSleepEntry(sleep);
            }
        }
        private List<SleepData> ParseFitbitSleepData(string jsonContent)
        {
            List<SleepData> parsedData = new List<SleepData>();
            return parsedData;

        }

        public void AddManualSleep(DateTime date, TimeSpan AmountSlept, TimeSpan REMSleep)
        {
            var manualSleepData = new SleepData
            {
                Date = date,
                AmountSlept = AmountSlept,
                REMSleep = REMSleep,
                ManualEntry = true
            };
            AddSleepEntry(manualSleepData);
        }

    }


}
