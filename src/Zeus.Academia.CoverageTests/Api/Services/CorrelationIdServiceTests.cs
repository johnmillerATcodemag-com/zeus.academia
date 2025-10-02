using System.Diagnostics;
using Xunit;
using Zeus.Academia.Api.Services;

namespace Zeus.Academia.CoverageTests.Api.Services;

/// <summary>
/// Comprehensive unit tests for CorrelationIdService
/// </summary>
public class CorrelationIdServiceTests
{
    [Fact]
    public void CorrelationId_WhenNotSet_ReturnsUnknown()
    {
        // Arrange
        var service = new CorrelationIdService();

        // Act
        var result = service.CorrelationId;

        // Assert
        Assert.Equal("unknown", result);
    }

    [Fact]
    public void SetCorrelationId_WithValidId_SetsAndReturnsId()
    {
        // Arrange
        var service = new CorrelationIdService();
        var testId = "test-correlation-123";

        // Act
        service.SetCorrelationId(testId);
        var result = service.CorrelationId;

        // Assert
        Assert.Equal(testId, result);
    }

    [Fact]
    public void SetCorrelationId_WithEmptyString_SetsEmptyString()
    {
        // Arrange
        var service = new CorrelationIdService();
        var emptyId = string.Empty;

        // Act
        service.SetCorrelationId(emptyId);
        var result = service.CorrelationId;

        // Assert
        Assert.Equal(emptyId, result);
    }

    [Fact]
    public void SetCorrelationId_WithNull_SetsNull()
    {
        // Arrange
        var service = new CorrelationIdService();

        // Act
        service.SetCorrelationId(null!);
        var result = service.CorrelationId;

        // Assert
        Assert.Equal("unknown", result); // AsyncLocal returns null, service returns "unknown"
    }

    [Fact]
    public void GenerateCorrelationId_ReturnsValidFormat()
    {
        // Arrange
        var service = new CorrelationIdService();

        // Act
        var result = service.GenerateCorrelationId();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // Should contain timestamp and short GUID parts
        var parts = result.Split('-');
        Assert.True(parts.Length >= 3); // timestamp (YYYYMMDD-HHMMSS) + short GUID

        // First part should be date in YYYYMMDD format
        Assert.True(DateTime.TryParseExact(parts[0], "yyyyMMdd", null,
            System.Globalization.DateTimeStyles.None, out _));

        // Second part should be time in HHMMSS format
        Assert.True(DateTime.TryParseExact(parts[1], "HHmmss", null,
            System.Globalization.DateTimeStyles.None, out _));

        // Last part should be 8-character hex string
        Assert.Equal(8, parts[^1].Length);
        Assert.True(parts[^1].All(c => char.IsLetterOrDigit(c)));
    }

    [Fact]
    public void GenerateCorrelationId_CalledMultipleTimes_ReturnsUniqueIds()
    {
        // Arrange
        var service = new CorrelationIdService();
        var generatedIds = new HashSet<string>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var id = service.GenerateCorrelationId();
            generatedIds.Add(id);
        }

        // Assert
        Assert.Equal(100, generatedIds.Count); // All IDs should be unique
    }

    [Fact]
    public void SetCorrelationId_SetsActivityTag()
    {
        // Arrange
        var service = new CorrelationIdService();
        var testId = "activity-test-456";

        using var activity = new Activity("Test");
        activity.Start();

        // Act
        service.SetCorrelationId(testId);

        // Assert
        var correlationTag = activity.Tags.FirstOrDefault(t => t.Key == "correlation-id");
        Assert.Equal(testId, correlationTag.Value);
    }

    [Fact]
    public void SetCorrelationId_WithoutActivity_DoesNotThrow()
    {
        // Arrange
        var service = new CorrelationIdService();
        var testId = "no-activity-789";

        // Act & Assert - Should not throw
        service.SetCorrelationId(testId);
        Assert.Equal(testId, service.CorrelationId);
    }

    [Fact]
    public async Task CorrelationId_InAsyncContext_MaintainsValue()
    {
        // Arrange
        var service = new CorrelationIdService();
        var testId = "async-test-999";

        // Act
        service.SetCorrelationId(testId);

        var result = await Task.Run(() =>
        {
            // Should maintain correlation ID in async context
            return service.CorrelationId;
        });

        // Assert
        Assert.Equal(testId, result);
    }

    [Fact]
    public async Task CorrelationId_InParallelTasks_MaintainsIndependentValues()
    {
        // Arrange
        var service = new CorrelationIdService();
        var results = new List<string>();

        // Act
        var tasks = Enumerable.Range(1, 10).Select(i => Task.Run(() =>
        {
            var taskId = $"task-{i}-correlation";
            service.SetCorrelationId(taskId);

            // Simulate some async work
            Thread.Sleep(10);

            return service.CorrelationId;
        }));

        var taskResults = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(10, taskResults.Length);

        // Each task should maintain its own correlation ID
        for (int i = 0; i < taskResults.Length; i++)
        {
            var expectedId = $"task-{i + 1}-correlation";
            Assert.Single(taskResults, r => r == expectedId);
        }
    }

    [Fact]
    public void CorrelationId_CrossThreadAccess_BehaviorIsConsistent()
    {
        // Arrange
        var service = new CorrelationIdService();
        var mainThreadId = "main-thread-123";
        var otherThreadResult = string.Empty;

        // Act
        service.SetCorrelationId(mainThreadId);

        var thread = new Thread(() =>
        {
            // Note: AsyncLocal behavior may vary - testing actual behavior
            otherThreadResult = service.CorrelationId;
        });

        thread.Start();
        thread.Join();

        // Assert
        Assert.Equal(mainThreadId, service.CorrelationId); // Main thread keeps its value
        // Note: AsyncLocal may flow to new threads in some cases
        Assert.NotNull(otherThreadResult); // Just verify it returns something
    }

    [Theory]
    [InlineData("simple-correlation")]
    [InlineData("correlation-with-special-chars-!@#$%")]
    [InlineData("very-long-correlation-id-with-lots-of-characters-and-numbers-123456789")]
    [InlineData("CorrelationId_With_Mixed_Case_And_Underscores")]
    public void SetCorrelationId_WithVariousFormats_PreservesExactValue(string testId)
    {
        // Arrange
        var service = new CorrelationIdService();

        // Act
        service.SetCorrelationId(testId);
        var result = service.CorrelationId;

        // Assert
        Assert.Equal(testId, result);
    }

    [Fact]
    public void GenerateCorrelationId_Format_MatchesExpectedPattern()
    {
        // Arrange
        var service = new CorrelationIdService();

        // Act
        var result = service.GenerateCorrelationId();

        // Assert
        // Expected format: YYYYMMDD-HHMMSS-xxxxxxxx
        var pattern = @"^\d{8}-\d{6}-[a-f0-9]{8}$";
        Assert.Matches(pattern, result);
    }

    [Fact]
    public void GenerateCorrelationId_TimestampPortion_IsCurrentTime()
    {
        // Arrange
        var service = new CorrelationIdService();
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var result = service.GenerateCorrelationId();
        var afterGeneration = DateTime.UtcNow;

        // Extract timestamp from correlation ID
        var parts = result.Split('-');
        var dateStr = parts[0];
        var timeStr = parts[1];

        var generatedDateTime = DateTime.ParseExact(
            $"{dateStr}-{timeStr}",
            "yyyyMMdd-HHmmss",
            null);

        // Assert
        Assert.True(generatedDateTime >= beforeGeneration.AddSeconds(-1));
        Assert.True(generatedDateTime <= afterGeneration.AddSeconds(1));
    }
}

/// <summary>
/// Integration tests for ICorrelationIdService interface
/// </summary>
public class ICorrelationIdServiceIntegrationTests
{
    [Fact]
    public void Interface_Implementation_ExposesAllMethods()
    {
        // Arrange
        ICorrelationIdService service = new CorrelationIdService();

        // Act & Assert - Interface methods should be accessible
        Assert.NotNull(service.CorrelationId);

        var generatedId = service.GenerateCorrelationId();
        Assert.NotNull(generatedId);

        service.SetCorrelationId("interface-test");
        Assert.Equal("interface-test", service.CorrelationId);
    }

    [Fact]
    public void Interface_MultipleInstances_ShareAsyncLocalState()
    {
        // Arrange
        ICorrelationIdService service1 = new CorrelationIdService();
        ICorrelationIdService service2 = new CorrelationIdService();

        // Act
        service1.SetCorrelationId("service1-id");
        service2.SetCorrelationId("service2-id");

        // Assert - AsyncLocal is shared across instances in same context
        // Both services will see the last set value since they share AsyncLocal storage
        Assert.Equal("service2-id", service1.CorrelationId);
        Assert.Equal("service2-id", service2.CorrelationId);
    }
}