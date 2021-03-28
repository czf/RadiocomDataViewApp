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
            ArtistAggregatedEventsRequest request = new ArtistAggregatedEventsRequest() { ArtistIds = new List<int>() { artistId }, TimeSeries = timeRange };
            HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistAggregatedEvents", request);
            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            AggregatedEvent temp = JsonConvert.DeserializeObject<List<AggregatedEvent>>(responseBody)?.FirstOrDefault();
            List<ItemCount> items = temp.AggregatedEventSumSource.Select((x, i) => new ItemCount() { Count = x.Value, ItemId = artistId, Name = x.Timestamp.ToString() }).ToList();
            Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)> buckets = GetItemCountBuckets(timeRange, artistId);
            foreach (var item in items)
            {
                Console.WriteLine(item.Name);
                buckets[item.Name].itemCount.Count = item.Count;
            }
            return buckets.Values.OrderBy(x=>x.sortOrder).Select(x=>x.itemCount).ToList();
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
                    for(int i = 0; i < 8; i++)
                    {
                        string name = GetOverTimeName(i, timeRange, now);
                        Console.WriteLine(now.AddDays(i * -1).ToString());
                        DateTimeOffset key = now.AddDays(i * -1);
                        result[key.ToString()] = (new ItemCount() { Name = name, ItemId = id }, key);
                    }
                    break;
                case AggregateTimeRange.ThreeMonths:
                    for (int i = 0; i < 12; i++)
                    {
                        string name = GetOverTimeName(i, timeRange, now);
                        Console.WriteLine(now.AddDays(i * -7).ToString());

                        DateTimeOffset key = now.AddDays(i * -7);
                        result[key.ToString()] = (new ItemCount() { Name = name, ItemId = id }, key);
                    }
                    break;
                case AggregateTimeRange.OneYear:
                    now = now.Subtract(TimeSpan.FromDays(now.Day-1));
                    Console.WriteLine("now: " + now);
                    for (int i = 0; i < 12; i++)
                    {
                        string name = GetOverTimeName(i, timeRange, now);
                        Console.WriteLine(now.AddMonths(i * -1).ToString());
                        DateTimeOffset key = now.AddMonths(i * -1);
                        result[key.ToString()] = (new ItemCount() { Name = name , ItemId= id}, key);
                    }
                    break;
                default:
                    break;
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

        public List<ItemCount> GetSongPlayedOverTime(AggregateTimeRange timeRange, int artistWorkId)
        {
            return new List<ItemCount>();
        }

        public int GetTotalUniqueSongs(AggregateTimeRange timeRange)
        {
            return 0;
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
                    result = now.AddDays(index * -7).DateTime.ToShortDateString();
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
