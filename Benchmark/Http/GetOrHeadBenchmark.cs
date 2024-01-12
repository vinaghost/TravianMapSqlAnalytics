using BenchmarkDotNet.Attributes;

namespace Benchmark.Http
{
    /// <summary>
    /// Same
    /// </summary>
    ///
    [ShortRunJob]
    public class GetOrHeadBenchmark
    {
        private readonly HttpClient _httpClient = new();

        [Params("https://ts100.x10.america.travian.com/",
                    "https://https://ts8.x1.arabics.travian.com/")]
        public string Url { get; set; } = "";

        [Benchmark]
        public async Task<bool> HttpGet()
        {
            try
            {
                //var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url), cancellationToken);
                var response = await _httpClient.GetAsync(Url);
                if (!response.IsSuccessStatusCode) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        [Benchmark]
        public async Task<bool> HttpHead()
        {
            try
            {
                var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, Url));
                if (!response.IsSuccessStatusCode) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}