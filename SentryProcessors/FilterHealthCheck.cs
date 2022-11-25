using Sentry;
using Sentry.Extensibility;

namespace gitlabChamp.SentryProcessors;

public class FilterHealthCheck : ISentryTransactionProcessor
{
    public Transaction? Process(Transaction transaction)
    {
        return transaction.Name.Contains("/-/health") ? null : transaction;
    }
}