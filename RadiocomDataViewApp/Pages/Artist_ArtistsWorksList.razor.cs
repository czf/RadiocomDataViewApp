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
    public partial class Artist_ArtistsWorksList : ComponentBase
    {
        [Inject]
        public IRadiocomArtistWorkRepository RadiocomArtistWorkRepository { get; set; }
        [Parameter]
        public int ArtistId { get; set; }
        private string _artistName;
        private static readonly Func<ArtistWorkDisplay, string> HrefGenerator = item => $"artistwork/{item.Id}";
        private IEnumerable<ArtistWorkInfo> _artistWorkInfos;
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            _artistWorkInfos = await RadiocomArtistWorkRepository.GetArtist_ArtistWorks(ArtistId);
            _artistName = _artistWorkInfos.FirstOrDefault()?.ArtistInfo.Name;
        }
    }
}
