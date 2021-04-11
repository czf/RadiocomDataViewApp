using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Clients.Live
{
    public class LiveRadiocomDataAggregateDataClient : IRadiocomDataAggregateDataClient
    {
        private const string LOCALSTORAGEKEY_ARTISTPLAYEDOVERTIME = "artistplayedovertime-";
        private const string LOCALSTORAGEKEY_ARTISTWORKPLAYEDOVERTIME = "artistworkplayedovertime-";

        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly string _endpointAddress;

        public LiveRadiocomDataAggregateDataClient(HttpClient httpClient, ILocalStorageService localStorageService, string endpointAddress)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _endpointAddress = endpointAddress;
        }

        public async Task<List<ItemCount>> GetArtistPlayedOverTime(AggregateTimeRange timeRange, int artistId)
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTISTPLAYEDOVERTIME + artistId + "-" + timeRange;
            List<ItemCount> result = await _localStorageService.GetItemAsync<List<ItemCount>>(localStorageKey);
            if (result == null)
            {

                ArtistAggregatedEventsRequest request = new ArtistAggregatedEventsRequest() { ArtistIds = new List<int>() { artistId }, TimeSeries = timeRange };
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistAggregatedEvents", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                AggregatedEvent temp = JsonConvert.DeserializeObject<List<AggregatedEvent>>(responseBody)?.FirstOrDefault();
                List<ItemCount> items = temp.AggregatedEventSumSource.Select((x, i) => new ItemCount() { Count = x.Value, ItemId = artistId, Name = x.Timestamp.ToString() }).ToList();
                Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)> buckets = GetItemCountBuckets(timeRange, artistId);
                foreach (var item in items)
                {
                    if (buckets.ContainsKey(item.Name))
                    {
                        buckets[item.Name].itemCount.Count = item.Count;
                    }
                }
                result = buckets.Values.OrderBy(x => x.sortOrder).Select(x => x.itemCount).ToList();
                await _localStorageService.SetItemAsync(localStorageKey, result);
            }
            return result;
        }
        public Task<List<ItemCount>> GetArtistSongsPlayed(AggregateTimeRange timeRange, int artistId)
        {
            return Task.FromResult(new List<ItemCount>());
        }

        public List<ItemCount> GetMostPlayedArtists(AggregateTimeRange timeRange)
        {
            return new List<ItemCount>();
        }

        public List<ItemCount> GetMostPlayedSongs(AggregateTimeRange timeRange)
        {
            return new List<ItemCount>();
        }

        public List<ItemCount> GetMostPlayedSongs(AggregateTimeRange timeRange, int artistId)
        {
            return new List<ItemCount>();
        }

        public Task<List<ItemCount>> GetSongPlayedAndOtherPlayed(AggregateTimeRange timeRange, int artistWorkId)
        {
            return Task.FromResult(new List<ItemCount>());
        }

        public async Task<List<ItemCount>> GetSongPlayedOverTime(AggregateTimeRange timeRange, int artistWorkId)
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTISTWORKPLAYEDOVERTIME + artistWorkId + "-" + timeRange;
            List<ItemCount> result = await _localStorageService.GetItemAsync<List<ItemCount>>(localStorageKey);

            if (result == null)
            {
                ArtistWorkAggregatedEventsRequest request = new ArtistWorkAggregatedEventsRequest() { ArtistWorkIds = new List<int>() { artistWorkId }, TimeSeries = timeRange };
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistWorkAggregatedEvents", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                AggregatedEvent temp = JsonConvert.DeserializeObject<List<AggregatedEvent>>(responseBody)?.FirstOrDefault();
                List<ItemCount> items = temp.AggregatedEventSumSource.Select((x, i) => new ItemCount() { Count = x.Value, ItemId = artistWorkId, Name = x.Timestamp.ToString() }).ToList();
                Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)> buckets = GetItemCountBuckets(timeRange, artistWorkId);
                foreach (var item in items)
                {
                    if (buckets.ContainsKey(item.Name))
                    {
                        buckets[item.Name].itemCount.Count = item.Count;
                    }
                }
                result = buckets.Values.OrderBy(x => x.sortOrder).Select(x => x.itemCount).ToList();
                await _localStorageService.SetItemAsync(localStorageKey, result);

            }
            return result;
        }

        public int GetTotalUniqueSongs(AggregateTimeRange timeRange)
        {
            return 0;
        }
        private Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)> GetItemCountBuckets(AggregateTimeRange timeRange, int id)
        {
            Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)> result = new Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)>();
            DateTimeOffset now = DateTimeOffset.UtcNow.Add(DateTimeOffset.Now.Offset);
            now = now.Subtract(new TimeSpan(now.Hour, now.Minute, now.Second));
            Console.WriteLine(now);
            switch (timeRange)
            {
                case AggregateTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case AggregateTimeRange.SevenDays:
                    for (int i = 0; i < 8; i++)
                    {
                        string name = GetOverTimeName(i, timeRange, now);
                        DateTimeOffset key = now.AddDays(i * -1);
                        result[key.ToString()] = (new ItemCount() { Name = name, ItemId = id }, key);
                    }
                    break;
                case AggregateTimeRange.ThreeMonths:
                    for (int i = 0; i < 12; i++)
                    {
                        string name = GetOverTimeName(i, timeRange, now);

                        DateTimeOffset key = now.AddDays(-1 * (int)now.DayOfWeek).AddDays(i * -7);
                        result[key.ToString()] = (new ItemCount() { Name = name, ItemId = id }, key);
                    }
                    break;
                case AggregateTimeRange.OneYear:
                    now = now.Subtract(TimeSpan.FromDays(now.Day - 1));
                    Console.WriteLine("now: " + now);
                    for (int i = 0; i < 12; i++)
                    {
                        string name = GetOverTimeName(i, timeRange, now);
                        DateTimeOffset key = now.AddMonths(i * -1);
                        result[key.ToString()] = (new ItemCount() { Name = name, ItemId = id }, key);
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        private string GetOverTimeName(int index, AggregateTimeRange timeRange, DateTimeOffset now)
        {
            string result = string.Empty;
            switch (timeRange)
            {
                case AggregateTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case AggregateTimeRange.SevenDays:
                    result = now.AddDays(index * -1).DayOfWeek.ToString();
                    break;
                case AggregateTimeRange.ThreeMonths:
                    result = now.AddDays(-1 * (int)now.DayOfWeek).AddDays(index * -7).DateTime.ToShortDateString();
                    break;
                case AggregateTimeRange.OneYear:
                    result = now.AddMonths(index * -1).DateTime.ToShortDateString();
                    break;
                default:
                    throw new NotSupportedException("Value: " + timeRange.ToString());

            }
            return result;
        }
        private class ArtistAggregatedEventsRequest
        {
            public IEnumerable<int> ArtistIds { get; set; }
            public AggregateTimeRange TimeSeries { get; set; }
        }

        private class ArtistWorkAggregatedEventsRequest
        {
            public IEnumerable<int> ArtistWorkIds { get; set; }
            public AggregateTimeRange TimeSeries { get; set; }
        }

        public class AggregatedEvent
        {
            public int Id { get; set; }
            public long AggregatedEventSum { get; set; }
            public AggregateTimeRange AggregationTimeSeries { get; set; }
            public IEnumerable<AggregatedEventSource> AggregatedEventSumSource { get; set; }
        }

        public class AggregatedEventSource 
        {
            public DateTimeOffset Timestamp { get; set; }
            public long Value { get; set; }
        }
    }
}
