﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Develappers.BillomatNet.Api
{
    internal abstract class PagedList<T>
    {
        public abstract List<T> List { get; set; }

        [JsonProperty("@page")]
        public int Page { get; set; }

        [JsonProperty("@per_page")]
        public int PerPage { get; set; }

        [JsonProperty("@total")]
        public int Total { get; set; }
    }
}
