using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects.Dto;

namespace RadiocomDataViewApp.Pages
{
    public partial class Artist : ComponentBase
    {
        private ArtistInfo _artist;

        [Parameter]
        public int ArtistId{ get; set; }

        public string ArtistName { get => _artist.Name; }
        [Inject]
        public IRadiocomArtistRepository RadiocomArtistClient { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            _artist = await RadiocomArtistClient.GetArtist(ArtistId);


        }

    }
}