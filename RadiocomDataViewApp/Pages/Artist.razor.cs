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
        public IRadiocomArtistRepository RadiocomArtistRepository { get; set; }

        protected override Task OnParametersSetAsync()
        {
            //_artist = await RadiocomArtistClient.GetArtist(ArtistId);
             return base.OnParametersSetAsync().ContinueWith(x =>
                InvokeAsync(async () => _artist = (await RadiocomArtistRepository.GetArtistAsync(ArtistId)))
            ).Unwrap();

        }

    }
}