using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomDataAggregateDataClient
    {
        Task<List<ItemCount>> GetMostPlayedSongsAsync(AggregateTimeRange timeRange);
        
        /// <summary>
        /// Get the ArtistWorks for within the specfied timerange for the specified artist
        /// </summary>
        /// <param name="timeRange">Time range to query</param>
        /// <param name="artistId">Artist id to query forr</param>
        /// <returns></returns>
        Task<List<ItemCount>> GetMostPlayedSongsAsync(AggregateTimeRange timeRange, int artistId);

        Task<List<ItemCount>> GetMostPlayedArtistsAsync(AggregateTimeRange timeRange);

        Task<int> GetTotalUniqueSongsAsync(AggregateTimeRange timeRange);
        Task<int> GetTotalUniqueArtistsAsync(AggregateTimeRange timeRange);

        Task<List<ItemCount>> GetSongPlayedOverTime(AggregateTimeRange timeRange, int artistWorkId);

        Task<List<ItemCount>> GetSongPlayedAndOtherPlayed(AggregateTimeRange timeRange, int artistWorkId);
        Task<List<ItemCount>> GetArtistPlayedOverTime(AggregateTimeRange timeRange, int artistId);
        Task<List<ItemCount>> GetArtistSongsPlayed(AggregateTimeRange timeRange, int artistId);
    }
}
