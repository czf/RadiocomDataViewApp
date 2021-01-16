using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomArtistRepository
    {
        IEnumerable<ArtistInfo> GetArtists();
        IEnumerable<ArtistInfo> GetArtists(char alphaFilter);
        Task<ArtistInfo> GetArtist(int artistId);
    }
}
