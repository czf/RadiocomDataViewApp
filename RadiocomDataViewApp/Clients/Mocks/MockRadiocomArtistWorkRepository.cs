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

        public IEnumerable<ArtistWorkInfo> GetArtistWorks()
        {
            IEnumerable<ArtistInfo> artists = _radiocomArtistRepository.GetArtists();

            int artistWorksCount = 0;
            var artistInfosWorkCount = artists.Select(x =>
            {
                int workCount = random.Next(1,15);
                artistWorksCount += workCount;
                return ( artist: x,  workCount);
            }).ToList();



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

        public async Task<ArtistWorkInfo> GetArtistWork(int id)
        {
            return await Task.FromResult(GetArtistWorks().First(x => x.Id == id));
        }
    }
}
