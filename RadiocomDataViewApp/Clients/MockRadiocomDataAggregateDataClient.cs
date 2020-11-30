using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Clients
{
    public class MockRadiocomDataAggregateDataClient : IRadiocomDataAggregateDataClient
    {
        public List<ItemCount> GetMostPlayedSongs(MostPlayedTimeRange timeRange)
        {
            int multiplier = 0;
            switch (timeRange)
            {
                case MostPlayedTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case MostPlayedTimeRange.SevenDays:
                    multiplier = 1;
                    break;
                case MostPlayedTimeRange.ThreeMonths:
                    multiplier = 20;
                    break;
                case MostPlayedTimeRange.AllTime:
                    multiplier = 50;
                    break;
                default:
                    throw new NotSupportedException("Value: " + timeRange.ToString());
                    break;
            }
            return new List<ItemCount>()
            {
                new ItemCount(){Count=100*multiplier,Name="song name", ItemId = 77},
                new ItemCount(){Count=100*multiplier,Name="song", ItemId = 53},
                new ItemCount(){Count=75*multiplier, Name="song name thats longer", ItemId=12},
                new ItemCount(){Count=51*multiplier, Name="song 1111", ItemId = 9},
                new ItemCount(){Count=51*multiplier, Name="song something", ItemId = 204},
                new ItemCount(){Count=51*multiplier, Name="something song", ItemId = 513}
            };
        }
        
        public List<ItemCount> GetMostPlayedArtists(MostPlayedTimeRange timeRange)
        {
            int multiplier = 0;
            switch (timeRange)
            {
                case MostPlayedTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case MostPlayedTimeRange.SevenDays:
                    multiplier = 1;
                    break;
                case MostPlayedTimeRange.ThreeMonths:
                    multiplier = 20;
                    break;
                case MostPlayedTimeRange.AllTime:
                    multiplier = 50;
                    break;
                default:
                    throw new NotSupportedException("Value: " + timeRange.ToString());
                    break;
            }
            return new List<ItemCount>()
            {
                new ItemCount(){Count=100*multiplier,Name="artist name", ItemId = 77},
                new ItemCount(){Count=100*multiplier,Name="artist", ItemId = 53},
                new ItemCount(){Count=75*multiplier, Name="artist name thats longer", ItemId=12},
                new ItemCount(){Count=51*multiplier, Name="artist 1111", ItemId = 9},
                new ItemCount(){Count=51*multiplier, Name="artist something", ItemId = 204},
                new ItemCount(){Count=51*multiplier, Name="something artist", ItemId = 513}
            };
        }
    }
}
