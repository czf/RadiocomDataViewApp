using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;
using RadiocomDataViewApp.Objects.Dto;
namespace RadiocomDataViewApp.Pages
{
    public partial class ArtistWorks : ComponentBase
    {
        [Inject]
        public IRadiocomArtistWorkRepository RadiocomArtistWorkRepository { get; set; }

        [Parameter]
        public string AlphaChar { get; set; }

        Func<ArtistWorkDisplay, string> HrefGenerator = item => $"/artistwork/{item.Id}";


        private class ArtistWorkDisplay : IHasName
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
}
