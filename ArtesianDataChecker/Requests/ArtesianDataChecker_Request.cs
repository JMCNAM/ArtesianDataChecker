using System;
using System.Collections.Generic;
using Artesian.SDK.Dto;
using Artesian.SDK.Service;
using ArtesianDataChecker.ClassObjects;
using ArtesianDataChecker.Configuration;
using NodaTime;

namespace ArtesianDataChecker.Requests
{
    class ArtesianDataChecker_Request
    {
        private ArtesianServiceConfig _config(string provider)
        {
            ArtesianDataChecker_Config configs = new ArtesianDataChecker_Config();
            Config config = configs.GetConfig(provider);

            var _cfg = new ArtesianServiceConfig()
            {
                BaseAddress = new Uri(config.baseAddress),
                Audience = config.audiance,
                Domain = config.domain,
                ClientId = config.clientId,
                ClientSecret = config.clientSec,
            };
            return _cfg;
        }

        public List<MarketDataEntity.Output> MetadataRequest(string curveName, string provider)
        {
            var _cfg = _config(provider);
            var mds = new MetaDataService(_cfg);
            var filter = new ArtesianSearchFilter();

            filter.SearchText = curveName;
            filter.Page = 1;
            filter.PageSize = 1;

            try
            {
                return mds.SearchFacetAsync(filter)
                    .ConfigureAwait(true)
                    .GetAwaiter()
                    .GetResult()
                    .Results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }

            return null;
        }

        public IEnumerable<TimeSerieRow.Actual> DataRequest(int id, string provider, LocalDate start, LocalDate end)
        {
            var _cfg = _config(provider);

            QueryService qs = new QueryService(_cfg);
            try
            {
                var query = qs.CreateActual()
                    .InAbsoluteDateRange(start, end)
                    .ForMarketData(id)
                    .InGranularity(Granularity.Day)
                    .ExecuteAsync().GetAwaiter().GetResult();
                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }

        public IEnumerable<TimeSerieRow.Actual> DataRequest(List<int> id, string provider, LocalDate start, LocalDate end)
        {
            var _cfg = _config(provider);

            QueryService qs = new QueryService(_cfg);
            try
            {
                var query = qs.CreateActual()
                    .InAbsoluteDateRange(start, end)
                    .ForMarketData(id.ToArray())
                    .InGranularity(Granularity.Day)
                    .ExecuteAsync().GetAwaiter().GetResult();
                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }

        public IEnumerable<CurveRange> MetadataRequestVersioned(string curveId, string provider, LocalDateTime start, LocalDateTime end)
        {
            Console.WriteLine("Requesting versioned metadata...");
            var _cfg = _config(provider);
            var mds = new MetaDataService(_cfg);

            var numVersions = mds.ReadCurveRangeAsync(Convert.ToInt32(curveId), 1, 1, null, start, end).Result; // Get number of available curves.
            var versions = mds.ReadCurveRangeAsync(Convert.ToInt32(curveId), 1, Convert.ToInt32(numVersions.Count), null, start, end).Result.Data;

            //Console.WriteLine("Versions");
            //foreach (var v in versions)
            //{
            //    Console.WriteLine(v.Version);
            //}
            //Console.WriteLine("Total : " + versions.Count());

            return versions;
        }

        public List<IEnumerable<TimeSerieRow.Versioned>> DataRequestVersioned(int id, string provider, LocalDate start, LocalDate end)
        {           
            var _cfg = _config(provider);

            LocalDateTime s = new LocalDateTime(start.Year, start.Month, start.Day, 0, 0);
            LocalDateTime e = new LocalDateTime(end.Year, end.Month, end.Day, 0, 0);
            List<IEnumerable<TimeSerieRow.Versioned>> versionList = new List<IEnumerable<TimeSerieRow.Versioned>>();

            QueryService qs = new QueryService(_cfg);
            var versions = MetadataRequestVersioned(id.ToString(), provider, s, e); // Get all versions for date range.

            Console.WriteLine("Requesting versioned data...");
            foreach (var v in versions)
            {
                
                try
                {
                    var query = qs.CreateVersioned()
                        .ForMarketData(id)
                        .InAbsoluteDateRange(v.RangeStart, v.RangeEnd)
                        .InGranularity(Granularity.Day)
                        .ForVersion(v.Version.Value)
                        .ExecuteAsync().ConfigureAwait(true).GetAwaiter().GetResult();

                    versionList.Add(query);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                }
            }
            return versionList;
        }
    }
}
