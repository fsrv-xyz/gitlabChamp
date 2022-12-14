using System.Collections.Generic;
using GitlabChamp.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sentry;
using Sentry.Extensibility;

namespace test.SentryProcessorsTest;

[TestClass]
public class HealthCheckTransactionFilterTest
{
    private readonly ISentryTransactionProcessor _processor = new HealthCheckTransactionFilter();

    [TestMethod]
    public void TestProcessMisc()
    {
        Transaction transaction = new("POST /webhook", "test-action");
        transaction.Contexts.Trace.Status = SpanStatus.Ok;
        Assert.IsNotNull(_processor.Process(transaction));
    }

    [TestMethod]
    public void TestProcessHealthCheckTransactionNoBreadcrumb()
    {
        Transaction transaction = new("GET /-/health", "test-action");
        transaction.Contexts.Trace.Status = SpanStatus.Ok;
        Assert.IsNull(_processor.Process(transaction));
    }

    [TestMethod]
    public void TestProcessHealthCheckTransactionHealthy()
    {
        Transaction transaction = new("GET /-/health", "test-action");
        transaction.Contexts.Trace.Status = SpanStatus.Ok;
        transaction.AddBreadcrumb(
            new Breadcrumb(
                type: "http",
                message: "GET /-/health",
                category: "http",
                level: BreadcrumbLevel.Info,
                data: new Dictionary<string, string> { { "status_code", "200" } }));

        Assert.IsNull(_processor.Process(transaction));
    }

    [TestMethod]
    public void TestProcessHealthCheckTransactionUnhealthy()
    {
        Transaction transaction = new("GET /-/health", "test-action");
        transaction.Contexts.Trace.Status = SpanStatus.InternalError;
        transaction.AddBreadcrumb(
            new Breadcrumb(
                type: "http",
                message: "",
                category: "http",
                level: BreadcrumbLevel.Info,
                data: new Dictionary<string, string> { { "status_code", "500" } }));

        Assert.IsNotNull(_processor.Process(transaction));
    }
}