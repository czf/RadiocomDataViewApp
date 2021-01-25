using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomArtistWorkRepository
    {
        Task<IEnumerable<ArtistWorkInfo>> GetArtistWorksAsync();
        IEnumerable<ArtistWorkInfo> GetArtistWorks(char alphaFilter);
        Task<IEnumerable<ArtistWorkInfo>> GetArtist_ArtistWorks(int artistId);
        Task<ArtistWorkInfo> GetArtistWorkAsync(int id);

    }
}
