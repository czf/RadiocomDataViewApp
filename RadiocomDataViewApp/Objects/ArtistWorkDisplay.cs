using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Objects
{
    public class ArtistWorkDisplay : IHasName
    {
        private ArtistWorkInfo _artistWorkInfo;

        public ArtistWorkDisplay(ArtistWorkInfo artistWorkInfo)
        {
            _artistWorkInfo = artistWorkInfo;
        }

        public string Name
        {
            get => FormatDisplayName(_artistWorkInfo);
            set => _artistWorkInfo.Name = value;
        }
        public int Id
        {
            get => _artistWorkInfo.Id;
            set => _artistWorkInfo.Id = value;
        }

        private static string FormatDisplayName(ArtistWorkInfo artistWorkInfo)
        => $"{artistWorkInfo.Name} ({artistWorkInfo.ArtistInfo.Name})";
    }
}
