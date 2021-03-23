using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Clients.Live
{
    public class LiveRadiocomArtistRepository : IRadiocomArtistRepository
    {
        private const string LOCALSTORAGEKEY_ARTIST_INFO = "artist_info-";
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly string _endpointAddress;

        public LiveRadiocomArtistRepository(HttpClient httpClient, ILocalStorageService localStorageService, string endpointAddress)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _endpointAddress = endpointAddress;
        }

        public async Task<ArtistInfo> GetArtistAsync(int artistId)
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTIST_INFO + artistId;
            ArtistInfo result = await _localStorageService.GetItemAsync<ArtistInfo>(localStorageKey);
            if (result == null)
            {
                ArtistInfosRequest request = new ArtistInfosRequest() { ArtistIds = new List<int>() { artistId } };

                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistInfos", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<ArtistInfo>>(responseBody)?.FirstOrDefault();
                await _localStorageService.SetItemAsync<ArtistInfo>(localStorageKey, result);
            }
            return result;

        }

        public IEnumerable<ArtistInfo> GetArtists(char alphaFilter)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ArtistInfo>> GetArtistsAsync()
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTIST_INFO + "all";
            List<ArtistInfo> result;
            TimeCachedObject<List<ArtistInfo>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ArtistInfo>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {
                ArtistInfosRequest request = new ArtistInfosRequest() { ArtistIds = Enumerable.Empty<int>() };
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistInfos", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<ArtistInfo>>(responseBody);
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<ArtistInfo>>()
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

        private class ArtistInfosRequest
        {
            public IEnumerable<int> ArtistIds { get; set; }
        }
    }
}
