using NodaTime;
using System;
using System.Collections;
using System.Collections.Generic;


namespace ArtesianDataChecker.ClassObjects
{
    public class CheckResult
    {
        public int ProdFileId { get; set; }
        public int TestFileId { get; set; }
        public string ProdFileName { get; set; }
        public string TestFileName { get; set; }

        public int NullMatched { get; set; }
        public int NullProd { get; set; }
        public int NullTest { get; set; }

        public int Matched { get; set; }
        public int Mismatch { get; set; }

        public int ErrorId { get; set; }
        public string ErrorNote { get; set; }

        public bool Versioned { get; set; }
        public DateTime? Version { get; set; }
    }

    public class GapResult
    {
        public string FileName { get; set; }
        public int FileId { get; set; }
        public int NumRecords { get; set; }
        public List<int> GapIndices { get; set; }
        public TimeSpan Interval { get; set; }

        public bool Versioned { get; set; }
        public DateTime? Version { get; set; }
    }

    public class ResultsContainer
    {
        public List<CheckResult> Results { get; set; }
        public List<GapResult> GapResults { get; set; }
    }

    public class Config
    {
        public string baseAddress { get; set; }
        public string audiance { get; set; }
        public string domain { get; set; }
        public string clientId { get; set; }
        public string clientSec { get; set; }
    }

    public class Paramaters
    {
        public List<int> TestIds { get; set; }
        public string Provider1 { get; set; }
        public List<int> ProdIds { get; set; }
        public string Provider2 { get; set; }
        public LocalDate Start { get; set; }
        public LocalDate End { get; set; }
    }
}
