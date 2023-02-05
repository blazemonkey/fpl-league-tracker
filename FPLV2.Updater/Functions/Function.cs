using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FPLV2.Updater.Functions;

public abstract class Function
{
    protected ILogger Logger { get; init; }
    protected IUnitOfWork UnitOfWork { get; init; }

    public Function(ILoggerFactory loggerFactory, IConfiguration configuration, FplClient fplClient, IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    public class FunctionInfo
    {
        public ScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class ScheduleStatus
    {
        public DateTime Last { get; set; }
        public DateTime Next { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
