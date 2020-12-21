using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomArtistRepository
    {
        public IEnumerable<ArtistInfo> GetArtists();
        public IEnumerable<ArtistInfo> GetArtists(char alphaFilter);

    }
}
