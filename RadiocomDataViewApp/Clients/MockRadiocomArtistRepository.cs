using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Clients
{
    public class MockRadiocomArtistRepository : IRadiocomArtistRepository
    {
        private static Random random = new Random();
        public static string RandomString(int length)//https://stackoverflow.com/a/1344258
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz     0123456789";
            char[] resultArray = new char[length];
            for(int a = 0; a<length; a++)
            {
                resultArray[a] = chars[random.Next(chars.Length)];
            }
            return new String(resultArray);
        }

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
                        Name = RandomString(random.Next(3, 20))
                    });
                }
            }
            return _artists;
            
        }

        public IEnumerable<ArtistInfo> GetArtists(char alphaFilter)
        {
            throw new NotImplementedException();
        }
    }
}
