using System.Net;

namespace FPLV2.UnitTests.Models;

public class MockHttpParameter
{
    public string RequestUrl { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string ResponseContent { get; set; }
}
