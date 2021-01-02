using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IAmArtistWork : IHasName
    {
        public IHasName ArtistInfo { get; set; }
    }
}
