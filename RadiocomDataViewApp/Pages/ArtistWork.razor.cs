using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Pages
{
    public partial class ArtistWork : ComponentBase
    {
        private ArtistWorkInfo _artistWorkInfo;
        [Parameter]
        public int ArtistWorkId { get; set; }


        public string ArtistWorkName { get => _artistWorkInfo.Name; }
        public string ArtistName { get => _artistWorkInfo.ArtistInfo.Name; }
        public int ArtistId { get => _artistWorkInfo.ArtistInfo.Id; }


        protected override Task OnParametersSetAsync()
        {
            //await base.OnParametersSetAsync();
            //_artistWorkInfo = await RadiocomArtistWorkRepository.GetArtistWork(ArtistWorkId);

            return base.OnParametersSetAsync().ContinueWith(x =>
                InvokeAsync(async () => _artistWorkInfo = (await RadiocomArtistWorkRepository.GetArtistWorkAsync(ArtistWorkId)))
            ).Unwrap();

        }


        [Inject]
        public IRadiocomArtistWorkRepository RadiocomArtistWorkRepository { get; set; }
    }
}
