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
        private static Random _random = new Random();

        public List<ItemCount> GetMostPlayedSongs(AggregateTimeRange timeRange)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);
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

        private static int GetTimeRangeMultipler(AggregateTimeRange timeRange)
        {
            int multiplier;
            switch (timeRange)
            {
                case AggregateTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case AggregateTimeRange.SevenDays:
                    multiplier = 1;
                    break;
                case AggregateTimeRange.ThreeMonths:
                    multiplier = 20;
                    break;
                case AggregateTimeRange.OneYear:
                    multiplier = 50;
                    break;
                default:
                    throw new NotSupportedException("Value: " + timeRange.ToString());
            }

            return multiplier;
        }

        public List<ItemCount> GetMostPlayedArtists(AggregateTimeRange timeRange)
        {   
            int multiplier = GetTimeRangeMultipler(timeRange);
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

        public int GetTotalUniqueSongs(AggregateTimeRange timeRange)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);

            return multiplier * _random.Next(300, 350);
        }

        public int GetTotalUniqueArtists(AggregateTimeRange timeRange)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);

            return multiplier * _random.Next(300, 350);
        }

        public List<ItemCount> GetMostPlayedSongs(AggregateTimeRange timeRange, int artistId)
        {
            return GetMostPlayedSongs(timeRange);
            
        }

        public Task<List<ItemCount>> GetSongPlayedOverTime(AggregateTimeRange timeRange, int artistWorkId)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);

            List<ItemCount> result = new List<ItemCount>()
            {
                new ItemCount(){Count = 10 * multiplier, Name= GetOverTimeName(0, timeRange), ItemId = artistWorkId},
                new ItemCount(){Count = 2 * multiplier, Name= GetOverTimeName(1, timeRange), ItemId = artistWorkId},
                new ItemCount(){Count = 7 * multiplier, Name= GetOverTimeName(2, timeRange), ItemId = artistWorkId},
                new ItemCount(){Count = 2 * multiplier, Name= GetOverTimeName(3, timeRange), ItemId = artistWorkId},
                new ItemCount(){Count = 2 * multiplier, Name= GetOverTimeName(4, timeRange), ItemId = artistWorkId},
                new ItemCount(){Count = 1 * multiplier, Name= GetOverTimeName(5, timeRange), ItemId = artistWorkId},
                new ItemCount(){Count = 0 * multiplier, Name= GetOverTimeName(6, timeRange), ItemId = artistWorkId},
            };
            if(timeRange == AggregateTimeRange.ThreeMonths || timeRange == AggregateTimeRange.OneYear)
            {
                result.Add(new ItemCount() { Count = 10 * multiplier, Name = GetOverTimeName(7, timeRange), ItemId = artistWorkId });
                result.Add(new ItemCount() { Count = 2 * multiplier, Name = GetOverTimeName(8, timeRange), ItemId = artistWorkId } );
                result.Add(new ItemCount() { Count = 7 * multiplier, Name = GetOverTimeName(9, timeRange), ItemId = artistWorkId } );
                result.Add(new ItemCount() { Count = 2 * multiplier, Name = GetOverTimeName(10, timeRange), ItemId = artistWorkId });
                result.Add(new ItemCount() { Count = 2 * multiplier, Name = GetOverTimeName(11, timeRange), ItemId = artistWorkId });
            }
            result.Reverse();
            return Task.FromResult(result);
            
        }

        public async Task<List<ItemCount>> GetSongPlayedAndOtherPlayed(AggregateTimeRange timeRange, int artistWorkId)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);
            List<ItemCount> result = new List<ItemCount>()
            {
                new ItemCount(){Count = 98 * multiplier, Name= "Other", ItemId = 0},
                new ItemCount(){Count = 2 * multiplier, Name= GetOverTimeName(1, timeRange), ItemId = artistWorkId},
            };
            return await Task.FromResult(result);
        }

        private string GetOverTimeName(int index, AggregateTimeRange timeRange)
        {
            string result = string.Empty;
            switch (timeRange)
            {
                case AggregateTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case AggregateTimeRange.SevenDays:
                    result = DateTime.Now.AddDays(index * -1).DayOfWeek.ToString();
                    break;
                case AggregateTimeRange.ThreeMonths:
                    result = DateTime.Now.AddDays(index * -7).ToShortDateString();
                    break;
                case AggregateTimeRange.OneYear:
                    result = DateTime.Now.AddMonths(index * -1).ToShortDateString();
                    break;
                default:
                    throw new NotSupportedException("Value: " + timeRange.ToString());

            }
            return result;
        }

        public async Task<List<ItemCount>> GetArtistPlayedOverTime(AggregateTimeRange timeRange, int artistId)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);

            List<ItemCount> result = new List<ItemCount>()
            {
                new ItemCount(){Count = 10 * multiplier, Name= GetOverTimeName(0, timeRange), ItemId = artistId},
                new ItemCount(){Count = 2 * multiplier, Name= GetOverTimeName(1, timeRange), ItemId = artistId},
                new ItemCount(){Count = 7 * multiplier, Name= GetOverTimeName(2, timeRange), ItemId = artistId},
                new ItemCount(){Count = 2 * multiplier, Name= GetOverTimeName(3, timeRange), ItemId = artistId},
                new ItemCount(){Count = 2 * multiplier, Name= GetOverTimeName(4, timeRange), ItemId = artistId},
                new ItemCount(){Count = 1 * multiplier, Name= GetOverTimeName(5, timeRange), ItemId = artistId},
                new ItemCount(){Count = 0 * multiplier, Name= GetOverTimeName(6, timeRange), ItemId = artistId},
            };
            if (timeRange == AggregateTimeRange.ThreeMonths || timeRange == AggregateTimeRange.OneYear)
            {
                result.Add(new ItemCount() { Count = 10 * multiplier, Name = GetOverTimeName(7, timeRange), ItemId = artistId });
                result.Add(new ItemCount() { Count = 2 * multiplier, Name = GetOverTimeName(8, timeRange), ItemId = artistId });
                result.Add(new ItemCount() { Count = 7 * multiplier, Name = GetOverTimeName(9, timeRange), ItemId = artistId });
                result.Add(new ItemCount() { Count = 2 * multiplier, Name = GetOverTimeName(10, timeRange), ItemId = artistId });
                result.Add(new ItemCount() { Count = 2 * multiplier, Name = GetOverTimeName(11, timeRange), ItemId = artistId });
            }
            result.Reverse();
            return await Task.FromResult(result);
        }

        public async Task<List<ItemCount>> GetArtistSongsPlayed(AggregateTimeRange timeRange, int artistId)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);
            return await Task.FromResult(new List<ItemCount>()
            {
                new ItemCount(){Count=100*multiplier,Name="song name", ItemId = 77},
                new ItemCount(){Count=100*multiplier,Name="song", ItemId = 53},
                new ItemCount(){Count=75*multiplier, Name="song name thats longer", ItemId=12},
                new ItemCount(){Count=51*multiplier, Name="song 1111", ItemId = 9},
                new ItemCount(){Count=51*multiplier, Name="song something", ItemId = 204},
                new ItemCount(){Count=51*multiplier, Name="something song", ItemId = 513}
            });
        }
    }
}
