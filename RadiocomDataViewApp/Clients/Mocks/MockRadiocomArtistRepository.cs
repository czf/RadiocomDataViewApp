using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Clients.Mocks
{
    public class MockRadiocomArtistRepository : IRadiocomArtistRepository
    {
        private static Random random = new Random();

        private List<ArtistInfo> _artists;

        public IEnumerable<ArtistInfo> GetArtists()
        {
            int artistsCount = random.Next(215, 300);
            if(_artists == null)
            {
                _artists = new List<ArtistInfo>(artistsCount);

                for (int a = 0; a < artistsCount; a++)
                {
                    _artists.Add(new ArtistInfo()
                    {
                        Id = a,
                        Name = MockUtils.RandomString(3, 20)
                    }) ;
                }
            }
            return _artists;
            
        }

        public IEnumerable<ArtistInfo> GetArtists(char alphaFilter)
        {
            throw new NotImplementedException();
        }

        public async Task<ArtistInfo> GetArtist(int artistId)
        {
            return await Task.FromResult( GetArtists().FirstOrDefault(x => x.Id == artistId));
        }
    }
}
