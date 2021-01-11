using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Interfaces;

namespace RadiocomDataViewApp.Pages
{
    public partial class ArtistWork : ComponentBase
    {
        [Parameter]
        public int ArtistWorkId { get; set; }

        public string ArtistWorkName { get; set; }


        public ArtistWork()
        {
            SetupHeaderButtons();
        }





        [Inject]
        public IRadiocomDataAggregateDataClient RadiocomDataAggregateDataClient { get; set; }
    }
}
