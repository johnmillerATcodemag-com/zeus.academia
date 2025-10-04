using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Zeus.Academia.SimpleTests;

/// <summary>
/// Simple integration tests that can be run independently without complex test framework dependencies.
/// This serves as a backup testing approach when xUnit package resolution fails.
/// </summary>
public class SimpleApiTests
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string API_BASE_URL = "http://localhost:5000";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 Zeus Academia API - Simple Integration Tests");
        Console.WriteLine("================================================");

        var tests = new List<Func<Task<TestResult>>>
        {
            TestHealthEndpoint,
            TestAuthLogin,
            TestAuthRefresh,
            TestStudentProfile,
            TestStudentProfileUpdate,
            TestStudentEnrollments,
            TestCourseEnrollment,
            TestCourseDrop,
            TestPaginatedCourses,
            TestPerformanceEndpoint,
            TestCompleteWorkflow
        };

        var results = new List<TestResult>();

        foreach (var test in tests)
        {
            try
            {
                var result = await test();
                results.Add(result);

                if (result.Success)
                    Console.WriteLine($"✅ {result.TestName}: PASSED");
                else
                    Console.WriteLine($"❌ {result.TestName}: FAILED - {result.ErrorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test Exception: {ex.Message}");
                results.Add(new TestResult { TestName = "Unknown", Success = false, ErrorMessage = ex.Message });
            }
        }

        // Summary
        Console.WriteLine("\n📊 Test Results Summary");
        Console.WriteLine("=======================");
        var passed = results.Count(r => r.Success);
        var failed = results.Count(r => !r.Success);
        var total = results.Count;

        Console.WriteLine($"Total Tests: {total}");
        Console.WriteLine($"Passed: {passed}");
        Console.WriteLine($"Failed: {failed}");
        Console.WriteLine($"Success Rate: {(double)passed / total * 100:F1}%");

        if (passed == total)
        {
            Console.WriteLine("\n🎉 ALL TESTS PASSED! API is ready for production use.");
        }
        else if (passed >= total * 0.8)
        {
            Console.WriteLine("\n⚠️  Most tests passed. API is mostly functional.");
        }
        else
        {
            Console.WriteLine("\n❌ Multiple tests failed. API needs attention.");
        }
    }

    private static async Task<TestResult> TestHealthEndpoint()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/health");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("Healthy"))
            {
                return new TestResult { TestName = "Health Check", Success = true };
            }

            return new TestResult { TestName = "Health Check", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Health Check", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestAuthLogin()
    {
        try
        {
            var loginData = new { username = "student", password = "test123" };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{API_BASE_URL}/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && responseContent.Contains("success") && responseContent.Contains("token"))
            {
                return new TestResult { TestName = "Auth Login", Success = true };
            }

            return new TestResult { TestName = "Auth Login", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Auth Login", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestAuthRefresh()
    {
        try
        {
            var refreshData = new { refreshToken = "test-token" };
            var json = JsonSerializer.Serialize(refreshData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{API_BASE_URL}/api/auth/refresh", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && responseContent.Contains("success") && responseContent.Contains("token"))
            {
                return new TestResult { TestName = "Auth Refresh", Success = true };
            }

            return new TestResult { TestName = "Auth Refresh", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Auth Refresh", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestStudentProfile()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/api/student/profile");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("studentId") && content.Contains("firstName"))
            {
                return new TestResult { TestName = "Student Profile", Success = true };
            }

            return new TestResult { TestName = "Student Profile", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Student Profile", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestStudentProfileUpdate()
    {
        try
        {
            var updateData = new { firstName = "Updated", lastName = "Student" };
            var json = JsonSerializer.Serialize(updateData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{API_BASE_URL}/api/student/profile", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && responseContent.Contains("success"))
            {
                return new TestResult { TestName = "Profile Update", Success = true };
            }

            return new TestResult { TestName = "Profile Update", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Profile Update", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestStudentEnrollments()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/api/student/enrollments");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("enrollments") && content.Contains("totalCredits"))
            {
                return new TestResult { TestName = "Student Enrollments", Success = true };
            }

            return new TestResult { TestName = "Student Enrollments", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Student Enrollments", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestCourseEnrollment()
    {
        try
        {
            var response = await _httpClient.PostAsync($"{API_BASE_URL}/api/student/enroll/1", null);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("success") && content.Contains("enrolled"))
            {
                return new TestResult { TestName = "Course Enrollment", Success = true };
            }

            return new TestResult { TestName = "Course Enrollment", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Course Enrollment", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestCourseDrop()
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{API_BASE_URL}/api/student/enroll/2");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("success") && content.Contains("dropped"))
            {
                return new TestResult { TestName = "Course Drop", Success = true };
            }

            return new TestResult { TestName = "Course Drop", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Course Drop", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestPaginatedCourses()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/api/courses/paginated?page=1&size=10");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("items") && content.Contains("pageNumber"))
            {
                return new TestResult { TestName = "Paginated Courses", Success = true };
            }

            return new TestResult { TestName = "Paginated Courses", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Paginated Courses", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestPerformanceEndpoint()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/api/test/performance?delay=50");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && content.Contains("processedAt") && content.Contains("delayMs"))
            {
                return new TestResult { TestName = "Performance Test", Success = true };
            }

            return new TestResult { TestName = "Performance Test", Success = false, ErrorMessage = $"Status: {response.StatusCode}" };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Performance Test", Success = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<TestResult> TestCompleteWorkflow()
    {
        try
        {
            // Complete workflow: Login -> Profile -> Courses -> Enroll
            var loginData = new { username = "student", password = "test123" };
            var loginJson = JsonSerializer.Serialize(loginData);
            var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var loginResponse = await _httpClient.PostAsync($"{API_BASE_URL}/api/auth/login", loginContent);
            if (!loginResponse.IsSuccessStatusCode)
                return new TestResult { TestName = "Complete Workflow", Success = false, ErrorMessage = "Login failed" };

            var profileResponse = await _httpClient.GetAsync($"{API_BASE_URL}/api/student/profile");
            if (!profileResponse.IsSuccessStatusCode)
                return new TestResult { TestName = "Complete Workflow", Success = false, ErrorMessage = "Profile failed" };

            var coursesResponse = await _httpClient.GetAsync($"{API_BASE_URL}/api/courses/paginated");
            if (!coursesResponse.IsSuccessStatusCode)
                return new TestResult { TestName = "Complete Workflow", Success = false, ErrorMessage = "Courses failed" };

            var enrollResponse = await _httpClient.PostAsync($"{API_BASE_URL}/api/student/enroll/1", null);
            if (!enrollResponse.IsSuccessStatusCode)
                return new TestResult { TestName = "Complete Workflow", Success = false, ErrorMessage = "Enrollment failed" };

            return new TestResult { TestName = "Complete Workflow", Success = true };
        }
        catch (Exception ex)
        {
            return new TestResult { TestName = "Complete Workflow", Success = false, ErrorMessage = ex.Message };
        }
    }
}

public class TestResult
{
    public string TestName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}