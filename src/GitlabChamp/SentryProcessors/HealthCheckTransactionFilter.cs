using System.Linq;
using Sentry;
using Sentry.Extensibility;

namespace GitlabChamp.SentryProcessors;

public class HealthCheckTransactionFilter : ISentryTransactionProcessor
{
    public Transaction? Process(Transaction transaction)
    {
        if (transaction.Status != SpanStatus.Ok) return transaction;
        if (transaction.Name.Contains("/-/health"))
        {
            // only drop health check transactions without errors
            var unsuccessfullHealthchecks = transaction.Breadcrumbs
                .Where(x => x.Category.Contains("http") && x.Data?["status_code"] != "200")
                .ToList()
                .Count;

            if (unsuccessfullHealthchecks == 0) return null;
        }

        return transaction;
    }
}