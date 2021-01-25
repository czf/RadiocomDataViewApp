using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomArtistRepository
    {
        Task<IEnumerable<ArtistInfo>> GetArtistsAsync();
        IEnumerable<ArtistInfo> GetArtists(char alphaFilter);
        Task<ArtistInfo> GetArtistAsync(int artistId);
    }
}
