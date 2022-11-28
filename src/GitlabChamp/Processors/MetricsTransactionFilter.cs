using Sentry;
using Sentry.Extensibility;

namespace GitlabChamp.Processors;

public class MetricTransactionFilter : ISentryTransactionProcessor
{
    public Transaction? Process(Transaction transaction)
    {
        if (transaction.Name.Contains("/-/metrics")) return null;
        return transaction;
    }
}