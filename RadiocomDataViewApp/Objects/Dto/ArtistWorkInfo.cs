using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;

namespace RadiocomDataViewApp.Objects.Dto
{
    public class ArtistWorkInfo : IAmArtistWork
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ArtistInfo ArtistInfo { get; set ; }

    }
}
