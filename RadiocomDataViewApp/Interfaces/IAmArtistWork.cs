using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IAmArtistWork : IHasName
    {
        public ArtistInfo ArtistInfo { get; set; }
    }
}
