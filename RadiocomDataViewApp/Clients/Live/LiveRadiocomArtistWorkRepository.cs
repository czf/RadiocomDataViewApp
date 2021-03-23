using System;
using System.Collections.Generic;
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
    public class LiveRadiocomArtistWorkRepository : IRadiocomArtistWorkRepository
    {
        private const string LOCALSTORAGEKEY_ARTISTWORK_INFO = "artistwork_info-";
        private readonly IRadiocomArtistRepository _radiocomArtistRepository;
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly string _endpointAddress;

        public LiveRadiocomArtistWorkRepository(IRadiocomArtistRepository radiocomArtistRepository, HttpClient httpClient, ILocalStorageService localStorageService, string endpointAddress)
        {
            _radiocomArtistRepository = radiocomArtistRepository;
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _endpointAddress = endpointAddress;
        }


        public async Task<ArtistWorkInfo> GetArtistWorkAsync(int id)
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTISTWORK_INFO + id;
            ArtistWorkInfo result = await _localStorageService.GetItemAsync<ArtistWorkInfo>(localStorageKey);
            if (result == null)
            {
                ArtistWorkInfosRequest request = new ArtistWorkInfosRequest() { ArtistWorkIds = new List<int>() { id} };

                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistWorkInfos", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<ArtistWorkInfo>>(responseBody)?.FirstOrDefault();
                await _localStorageService.SetItemAsync<ArtistWorkInfo>(localStorageKey, result);
            }
            return result;
        }

        public IEnumerable<ArtistWorkInfo> GetArtistWorks(char alphaFilter)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ArtistWorkInfo>> GetArtistWorksAsync()
        {
            string localStorageKey = LOCALSTORAGEKEY_ARTISTWORK_INFO + "all";
            List<ArtistWorkInfo> result;
            TimeCachedObject<List<ArtistWorkInfo>> cachedObject = await _localStorageService.GetItemAsync<TimeCachedObject<List<ArtistWorkInfo>>>(localStorageKey);
            if (cachedObject == null || cachedObject.NextUpdateHour < DateTimeOffset.UtcNow)
            {
                ArtistWorkInfosRequest request = new ArtistWorkInfosRequest() { ArtistWorkIds = Enumerable.Empty<int>() };
                HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(_endpointAddress + "ArtistWorkInfos", request);
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                List< ArtistWorkInfoSource> temp = JsonConvert.DeserializeObject<List<ArtistWorkInfoSource>>(responseBody);
                IEnumerable<ArtistInfo> artists = await _radiocomArtistRepository.GetArtistsAsync();
                result = artists.Join(temp, x => x.Id, y => y.ArtistId, (x, y) => new ArtistWorkInfo() { ArtistInfo = x, Id = y.Id, Name = y.Title }).ToList();
                DateTimeOffset nextUpdate = TimeCachedObject<object>.CalculateNextUpdateHour();
                cachedObject = new TimeCachedObject<List<ArtistWorkInfo>>()
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

        public Task<IEnumerable<ArtistWorkInfo>> GetArtist_ArtistWorks(int artistId)
        {
            throw new NotImplementedException();
        }

        private class ArtistWorkInfosRequest
        {
            public IEnumerable<int> ArtistWorkIds { get; set; }
        }
        private class ArtistWorkInfoSource
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int ArtistId { get; set; }
        }
        
    }
}
