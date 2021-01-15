using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomArtistWorkRepository
    {
        IEnumerable<ArtistWorkInfo> GetArtistWorks();
        IEnumerable<ArtistWorkInfo> GetArtistWorks(char alphaFilter);
        Task<ArtistWorkInfo> GetArtistWork(int id);

    }
}
