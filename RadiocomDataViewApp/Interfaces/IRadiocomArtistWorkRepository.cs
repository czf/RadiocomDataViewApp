using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomArtistWorkRepository
    {
        public IEnumerable<ArtistWorkInfo> GetArtistWorks();
        public IEnumerable<ArtistWorkInfo> GetArtistWorks(char alphaFilter);
    }
}
