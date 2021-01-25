using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Clients.Mocks
{
    public class MockRadiocomArtistWorkRepository : IRadiocomArtistWorkRepository
    {
        private static readonly Random random = new Random();
        private readonly IRadiocomArtistRepository _radiocomArtistRepository;

        private List<ArtistWorkInfo> _artistWorks;

        public MockRadiocomArtistWorkRepository(IRadiocomArtistRepository radiocomArtistRepository)
        {
            _radiocomArtistRepository = radiocomArtistRepository;
        }

        public async Task<IEnumerable<ArtistWorkInfo>> GetArtistWorksAsync()
        {
            
            IEnumerable<ArtistInfo> artists = await _radiocomArtistRepository.GetArtistsAsync();

            int artistWorksCount = 0;
            var artistInfosWorkCount = await Task.FromResult( artists.Select(x =>
            {
                int workCount = random.Next(1,15);
                artistWorksCount += workCount;
                return ( artist: x,  workCount);
            }).ToList());



            if (_artistWorks == null)
            {
                _artistWorks = new List<ArtistWorkInfo>(artistWorksCount);

                for (int a = 0; a < artistWorksCount; a++)
                {
                    _artistWorks.Add(new ArtistWorkInfo()
                    {
                        Id = a,
                        Name = MockUtils.RandomString(3, 20),
                        ArtistInfo = GetRandomArtist(artistInfosWorkCount)

                    });
                    if (a % 10 == 0)
                    {
                        await Task.Delay(1);
                    }
                }


            }
            return _artistWorks;
            
        }

        private ArtistInfo GetRandomArtist(List<(ArtistInfo artist, int workCount)> artistsForWorks)
        {
            int indx = random.Next(0, artistsForWorks.Count - 1);
            ArtistInfo result = artistsForWorks[indx].artist;
            if(artistsForWorks[indx].workCount == 1)
            {
                artistsForWorks.RemoveAt(indx);
            }
            return result;
        }

        public IEnumerable<ArtistWorkInfo> GetArtistWorks(char alphaFilter)
        {
            throw new NotImplementedException();
        }

        public async Task<ArtistWorkInfo> GetArtistWorkAsync(int id)
        {
            return (await GetArtistWorksAsync()).First(x => x.Id == id);
        }

        public async Task<IEnumerable<ArtistWorkInfo>> GetArtist_ArtistWorks(int artistId)
        {
            return (await GetArtistWorksAsync()).Where(x => x.ArtistInfo.Id == artistId);
        }
    }
}
