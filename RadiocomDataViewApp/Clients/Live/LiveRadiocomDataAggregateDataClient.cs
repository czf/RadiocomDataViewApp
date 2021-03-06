﻿using System;
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
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Clients.Live
{
    public class LiveRadiocomDataAggregateDataClient : IRadiocomDataAggregateDataClient
    {
        private const string LOCALSTORAGEKEY_ARTISTPLAYEDOVERTIME = "artistplayedovertime-";
        private const string LOCALSTORAGEKEY_ARTISTWORKPLAYEDOVERTIME = "artistworkplayedovertime-";
        private const string LOCALSTORAGEKEY_ARTISTWORKUNIQUECOUNT = "artistworkuniquecount-";
        private const string LOCALSTORAGEKEY_ARTISTUNIQUECOUNT = "artistuniquecount-";
        private const string LOCALSTORAGEKEY_ALLARTISTWORKAGGREGATEDEVENTS = "allartistworkaggregatedevents-";
        private const string LOCALSTORAGEKEY_ALLARTISTSAGGREGATEDEVENTS = "allartistsaggregatedevents-";
        private const string LOCALSTORAGEKEY_MOSTPLAYEDARTISTWORKS = "mostplayedartistworks-";
        
        private const string LOCALSTORAGEKEY_MOSTPLAYEDARTISTS = "mostplayedartists-";

        private const string LOCALSTORAGEKEY_ARTISTMOSTPLAYEDARTISTWORKS = "artistmostplayedartistworks-";
        private const string LOCALSTORAGEKEY_ARTISTARTISTWORKSPLAYED = "artistartistworksplayed-";
        private const string LOCALSTORAGEKEY_ARTISTARTISTWORKSPLAYEDANDOTHERS = "artistartistworksplayedandothers-";


        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly string _endpointAddress;
        private readonly IRadiocomArtistRepository _radiocomArtistRepository;
        private readonly IRadiocomArtistWorkRepository _radiocomArtistWorkRepository;

        public LiveRadiocomDataAggregateDataClient(HttpClient httpClient, ILocalStorageService localStorageService, string endpointAddress, IRadiocomArtistRepository radiocomArtistRepository, IRadiocomArtistWorkRepository radiocomArtistWorkRepository)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _endpointAddress = endpointAddress;
            _radiocomArtistRepository = radiocomArtistRepository;
            _radiocomArtistWorkRepository = radiocomArtistWorkRepository;
        }

        public async Task<List<ItemCount>> GetArtistPlayedOverTime(AggregateTimeRange timeRange, int artistId)
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTISTPLAYEDOVERTIME + artistId + "-" + timeRange;
            List<ItemCount> result;
            TimeCachedObject<List<ItemCount>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ItemCount>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
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
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<ItemCount>>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }
        public async Task<List<ItemCount>> GetArtistSongsPlayed(AggregateTimeRange timeRange, int artistId)
        {
            List<ItemCount> result = new List<ItemCount>();

            string localStorageKey = LOCALSTORAGEKEY_ARTISTARTISTWORKSPLAYED + artistId + "-" + timeRange;
            TimeCachedObject<List<ItemCount>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ItemCount>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {
                Artist_TopArtistWorkAggregatedEventsRequest request = new Artist_TopArtistWorkAggregatedEventsRequest() { ArtistId = artistId, TimeSeries = timeRange,  TopN = 100};
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "Artist_TopArtistWorkAggregatedEvents", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                List<AggregatedEvent> temp = JsonConvert.DeserializeObject<List<AggregatedEvent>>(responseBody);
                foreach (var work in temp)
                {
                    result.Add(new ItemCount()
                    {
                        Count = work.AggregatedEventSum,
                        ItemId = work.Id,
                        Name = (await _radiocomArtistWorkRepository.GetArtistWorkAsync(work.Id)).Name
                    });
                }
                result = result.OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToList();
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<ItemCount>>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }

        public async Task<List<ItemCount>> GetMostPlayedArtistsAsync(AggregateTimeRange timeRange)
        {
            List<ItemCount> result = new List<ItemCount>();
            string localStorageKey = LOCALSTORAGEKEY_MOSTPLAYEDARTISTS + timeRange;
            TimeCachedObject<List<ItemCount>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ItemCount>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {

                List<AggregatedEvent> artists = await GetAllArtistAggregatedEvents(timeRange);
                foreach (var work in artists
                                        .OrderByDescending(x => x.AggregatedEventSum)
                                        .Take(6))
                {
                    result.Add(new ItemCount()
                    {
                        Count = work.AggregatedEventSum,
                        ItemId = work.Id,
                        Name = (await _radiocomArtistRepository.GetArtistAsync(work.Id)).Name
                    });

                    result = result.OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToList();

                    DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                    cachedObject = new TimeCachedObject<List<ItemCount>>()
                    {
                        CachedObject = result,
                        NextUpdateHour = nextUpdate
                    };
                    await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
                }
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }

        public async Task<List<ItemCount>> GetMostPlayedSongsAsync(AggregateTimeRange timeRange)
        {
            List<ItemCount> result = new List<ItemCount>();
            string localStorageKey = LOCALSTORAGEKEY_MOSTPLAYEDARTISTWORKS + timeRange;
            TimeCachedObject<List<ItemCount>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ItemCount>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {

                List<AggregatedEvent> artistWorks = await GetAllArtistWorkAggregatedEvents(timeRange);
                foreach (var work in artistWorks
                                        .OrderByDescending(x => x.AggregatedEventSum)
                                        .Take(6))
                {
                    result.Add(new ItemCount()
                    {
                        Count = work.AggregatedEventSum,
                        ItemId = work.Id,
                        Name = (await _radiocomArtistWorkRepository.GetArtistWorkAsync(work.Id)).Name
                    });

                    result = result.OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToList();

                    DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                    cachedObject = new TimeCachedObject<List<ItemCount>>()
                    {
                        CachedObject = result,
                        NextUpdateHour = nextUpdate
                    };
                    await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
                }
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
                
                
        }

        public async Task<List<ItemCount>> GetMostPlayedSongsAsync(AggregateTimeRange timeRange, int artistId)
        {
            List<ItemCount> result = new List<ItemCount>();

            string localStorageKey = LOCALSTORAGEKEY_ARTISTMOSTPLAYEDARTISTWORKS + artistId + "-" + timeRange;
            TimeCachedObject<List<ItemCount>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ItemCount>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {
                Artist_TopArtistWorkAggregatedEventsRequest request = new Artist_TopArtistWorkAggregatedEventsRequest() { ArtistId = artistId, TimeSeries = timeRange, };
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "Artist_TopArtistWorkAggregatedEvents", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                List<AggregatedEvent> temp = JsonConvert.DeserializeObject<List<AggregatedEvent>>(responseBody);
                foreach (var work in temp)
                {
                    result.Add(new ItemCount()
                    {
                        Count = work.AggregatedEventSum,
                        ItemId = work.Id,
                        Name = (await _radiocomArtistWorkRepository.GetArtistWorkAsync(work.Id)).Name
                    });
                }
                result = result.OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToList();
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<ItemCount>>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }

        public async Task<List<ItemCount>> GetSongPlayedAndOtherPlayed(AggregateTimeRange timeRange, int artistWorkId)
        {
            List<ItemCount> result = new List<ItemCount>();
            
            string localStorageKey = LOCALSTORAGEKEY_ARTISTARTISTWORKSPLAYEDANDOTHERS + artistWorkId + "-" + timeRange;
            TimeCachedObject<List<ItemCount>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ItemCount>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {

                ArtistInfo info = (await _radiocomArtistWorkRepository.GetArtistWorkAsync(artistWorkId)).ArtistInfo;
                List<ItemCount> works = await GetArtistSongsPlayed(timeRange, info.Id);
                long otherCount = works.Where(x => x.ItemId != artistWorkId).Sum(x => x.Count);
                ItemCount work = works.FirstOrDefault(x => x.ItemId == artistWorkId);
                if (work != null)
                {
                    result.Add(work);
                }
                else
                {
                    result.Add(new ItemCount()
                    {
                        Count = 0,
                        ItemId = artistWorkId,
                        Name = info.Name
                    });
                }
                result.Add(new ItemCount()
                {
                    Count = otherCount,
                    Name = "Other",
                    ItemId = -1
                });
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<ItemCount>>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }

        public async Task<List<ItemCount>> GetSongPlayedOverTime(AggregateTimeRange timeRange, int artistWorkId)
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTISTWORKPLAYEDOVERTIME + artistWorkId + "-" + timeRange;
            List<ItemCount> result;
            TimeCachedObject<List<ItemCount>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ItemCount>>>(localStorageKey);


            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
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
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<ItemCount>>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);

            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }

        public async Task<int> GetTotalUniqueSongsAsync(AggregateTimeRange timeRange)
        {
            int result;
            string localStorageKey = LOCALSTORAGEKEY_ARTISTWORKUNIQUECOUNT + timeRange;
            TimeCachedObject<int> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<int>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {   
                result =(await GetAllArtistWorkAggregatedEvents(timeRange))?.Count(x => x.AggregatedEventSum > 0) ?? 0;
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<int>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }
        public async Task<int> GetTotalUniqueArtistsAsync(AggregateTimeRange timeRange)
        {
            int result;
            string localStorageKey = LOCALSTORAGEKEY_ARTISTUNIQUECOUNT + timeRange;
            TimeCachedObject<int> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<int>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {
                result = (await GetAllArtistAggregatedEvents(timeRange))?.Count(x => x.AggregatedEventSum > 0) ?? 0;
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<int>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }
        private Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)> GetItemCountBuckets(AggregateTimeRange timeRange, int id)
        {
            Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)> result = new Dictionary<string, (ItemCount itemCount, DateTimeOffset sortOrder)>();
            DateTimeOffset now = DateTimeOffset.UtcNow.Add(DateTimeOffset.Now.Offset);
            now = now.Subtract(new TimeSpan(now.Hour, now.Minute, now.Second));
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

        private async Task<List<AggregatedEvent>> GetAllArtistWorkAggregatedEvents(AggregateTimeRange timeRange)
        {
            List<AggregatedEvent> result;
            string localStorageKey = LOCALSTORAGEKEY_ALLARTISTWORKAGGREGATEDEVENTS + timeRange;
            TimeCachedObject<List<AggregatedEvent>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<AggregatedEvent>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {
                ArtistWorkAggregatedEventsRequest request = new ArtistWorkAggregatedEventsRequest() { ArtistWorkIds = new List<int>(), TimeSeries = timeRange };
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistWorkAggregatedEvents", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<AggregatedEvent>>(responseBody);
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<AggregatedEvent>>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
            }
            return result;
        }

        private async Task<List<AggregatedEvent>> GetAllArtistAggregatedEvents(AggregateTimeRange timeRange)
        {
            List<AggregatedEvent> result;
            string localStorageKey = LOCALSTORAGEKEY_ALLARTISTSAGGREGATEDEVENTS + timeRange;
            TimeCachedObject<List<AggregatedEvent>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<AggregatedEvent>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {
                ArtistAggregatedEventsRequest request = new ArtistAggregatedEventsRequest() { ArtistIds = new List<int>(), TimeSeries = timeRange };
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistAggregatedEvents", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<AggregatedEvent>>(responseBody);
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<AggregatedEvent>>()
                {
                    CachedObject = result,
                    NextUpdateHour = nextUpdate
                };
                await _localStorageService.SetItemAsync(localStorageKey, cachedObject);
            }
            else
            {
                result = cachedObject.CachedObject;
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
        private class Artist_TopArtistWorkAggregatedEventsRequest
        {
            public int ArtistId { get; set; }
            public int TopN { get; set; } = 6;
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
