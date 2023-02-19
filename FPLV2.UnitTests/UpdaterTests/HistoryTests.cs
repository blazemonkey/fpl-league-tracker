using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLV2.UnitTests.UpdaterTests;

/// <summary>
/// Tests the corresponding HistoryApi class
/// </summary>
[TestClass]
public class HistoryTests : UpdaterTests
{
    /// <summary>
    /// Gets the request url to be matched against
    /// </summary>
    protected override string RequestUrl => "entry/{0}/history/";

    /// <summary>
    /// Gets the Sample Data json file name
    /// </summary>
    protected override string ResourceName => "history";
}
