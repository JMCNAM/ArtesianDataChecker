using Artesian.SDK.Dto;
using ArtesianDataChecker.ClassObjects;
using ArtesianDataChecker.Requests;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArtesianDataChecker.DataCheck
{
    class ArtesianDataChecker_GapCheck
    {
        public GapResult CheckIntervals(IEnumerable<TimeSerieRow.Actual> data)
        {
            GapResult result = new GapResult();
            result.GapIndices = new List<int>();
            var today = LocalDate.FromDateTime(DateTime.Now);
            List<TimeSerieRow.Actual> dataList = data as List<TimeSerieRow.Actual>;
            var interval = _getSampleInterval(dataList);

            for (int i = 0; i < dataList.Count - 1; i++)
            {
                var span = dataList[i + 1].Time - dataList[i].Time;
                if (span > interval)
                { 
                    result.FileId = dataList[i].TSID;
                    result.FileName = dataList[i].CurveName;
                    result.Interval = interval;                  
                    result.GapIndices.Add(i);
                    result.NumRecords = dataList.Count;
                }
            }
            string gaps = "";
            foreach (var i in result.GapIndices)
            {
                gaps += i + " ";
            }
            return result;
        }

        public GapResult CheckIntervals(IEnumerable<TimeSerieRow.Versioned> data)
        {
            GapResult result = new GapResult();
            result.GapIndices = new List<int>();
            var today = LocalDate.FromDateTime(DateTime.Now);
            List<TimeSerieRow.Versioned> dataList = data as List<TimeSerieRow.Versioned>;
            var interval = _getSampleInterval(dataList);

            for (int i = 0; i < dataList.Count - 1; i++)
            {
                var span = dataList[i + 1].Time - dataList[i].Time;
                if (span > interval)
                {
                    result.FileId = dataList[i].TSID;
                    result.FileName = dataList[i].CurveName;
                    result.Interval = interval;
                    result.GapIndices.Add(i);
                    result.NumRecords = dataList.Count;
                    result.Versioned = true;
                    result.Version = dataList[i].Version;
                }
            }
            string gaps = "";
            foreach (var i in result.GapIndices)
            {
                gaps += i + " ";
            }
            return result;
        }

        public TimeSpan _getSampleInterval(List<TimeSerieRow.Actual> dataList)
        {
            TimeSpan interval = dataList[1].Time - dataList[0].Time;
            for (int i = 0; i < dataList.Count / 10; i++)
            {
                var span = dataList[i + 1].Time - dataList[i].Time;
                if (interval > span)
                    interval = span;
            }
            return interval;
        }

        public TimeSpan _getSampleInterval(List<TimeSerieRow.Versioned> dataList)
        {
            TimeSpan interval = dataList[1].Time - dataList[0].Time;
            for (int i = 0; i < dataList.Count / 10; i++)
            {
                var span = dataList[i + 1].Time - dataList[i].Time;
                if (interval > span)
                    interval = span;
            }
            return interval;
        }

    }
}
