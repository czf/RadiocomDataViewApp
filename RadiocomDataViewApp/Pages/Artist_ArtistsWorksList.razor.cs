using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Pages
{
    public partial class Artist_ArtistsWorksList : ComponentBase
    {
        [Parameter]
        public int ArtistId { get; set; }
        
    }
}
