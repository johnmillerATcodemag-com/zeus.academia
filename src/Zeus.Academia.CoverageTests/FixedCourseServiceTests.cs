using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.CoverageTests
{
    /// <summary>
    /// Comprehensive tests for CourseService covering enrollment validation and course management.
    /// Tests student enrollment verification, faculty teaching assignments, and course operations.
    /// </summary>
    public class FixedCourseServiceTests : IDisposable
    {
        private readonly AcademiaDbContext _context;
        private readonly Mock<ILogger<CourseService>> _logger;
        private readonly CourseService _courseService;

        public FixedCourseServiceTests()
        {
            // Create test database
            var options = new DbContextOptionsBuilder<AcademiaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Create test configuration
            var configDict = new Dictionary<string, string?>
            {
                {"CourseSettings:DefaultCreditHours", "3"}
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();

            _context = new AcademiaDbContext(options, config);

            // Setup mocks
            _logger = new Mock<ILogger<CourseService>>();

            // Create CourseService
            _courseService = new CourseService(_context, _logger.Object);

            SeedTestData();
        }

        [Fact]
        public async Task IsStudentEnrolledAsync_StudentEnrolled_ReturnsTrue()
        {
            // Arrange
            var studentId = 1001;
            var subjectCode = "CS101";

            var enrollment = new StudentEnrollment
            {
                StudentEmpNr = studentId,
                SubjectCode = subjectCode,
                Status = "enrolled"
            };

            _context.StudentEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.IsStudentEnrolledAsync(studentId, subjectCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsStudentEnrolledAsync_StudentNotEnrolled_ReturnsFalse()
        {
            // Arrange
            var studentId = 1002;
            var subjectCode = "CS102";

            // Act
            var result = await _courseService.IsStudentEnrolledAsync(studentId, subjectCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DoesFacultyTeachCourseAsync_FacultyTeachesCourse_ReturnsTrue()
        {
            // Arrange
            var facultyId = 2001;
            var subjectCode = "CS201";

            var teaching = new Teaching
            {
                AcademicEmpNr = facultyId,
                SubjectCode = subjectCode
            };

            _context.Teachings.Add(teaching);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.DoesFacultyTeachCourseAsync(facultyId, subjectCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DoesFacultyTeachCourseAsync_FacultyDoesNotTeachCourse_ReturnsFalse()
        {
            // Arrange
            var facultyId = 2002;
            var subjectCode = "CS202";

            // Act
            var result = await _courseService.DoesFacultyTeachCourseAsync(facultyId, subjectCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetStudentEnrolledCoursesAsync_StudentHasCourses_ReturnsCourseCodes()
        {
            // Arrange
            var studentId = 1003;
            var courseCodes = new[] { "CS301", "CS302", "CS303" };

            var enrollments = courseCodes.Select(code => new StudentEnrollment
            {
                StudentEmpNr = studentId,
                SubjectCode = code,
                Status = "enrolled"
            });

            _context.StudentEnrollments.AddRange(enrollments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetStudentEnrolledCoursesAsync(studentId);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains("CS301", result);
            Assert.Contains("CS302", result);
            Assert.Contains("CS303", result);
        }

        [Fact]
        public async Task GetStudentEnrolledCoursesAsync_StudentHasNoCourses_ReturnsEmpty()
        {
            // Arrange
            var studentId = 1004;

            // Act
            var result = await _courseService.GetStudentEnrolledCoursesAsync(studentId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFacultyTaughtCoursesAsync_FacultyTeachesCourses_ReturnsCourseCodes()
        {
            // Arrange
            var facultyId = 2003;
            var courseCodes = new[] { "CS401", "CS402" };

            var teachings = courseCodes.Select(code => new Teaching
            {
                AcademicEmpNr = facultyId,
                SubjectCode = code
            });

            _context.Teachings.AddRange(teachings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetFacultyTaughtCoursesAsync(facultyId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains("CS401", result);
            Assert.Contains("CS402", result);
        }

        [Fact]
        public async Task GetFacultyTaughtCoursesAsync_FacultyTeachesNoCourses_ReturnsEmpty()
        {
            // Arrange
            var facultyId = 2004;

            // Act
            var result = await _courseService.GetFacultyTaughtCoursesAsync(facultyId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task DoesCourseExistAsync_CourseExists_ReturnsTrue()
        {
            // Arrange
            var subjectCode = "CS501";

            var subject = new Subject
            {
                Code = subjectCode,
                Title = "Advanced Computer Science",
                CreditHours = 3,
                IsActive = true
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.DoesCourseExistAsync(subjectCode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DoesCourseExistAsync_CourseDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var subjectCode = "CS999";

            // Act
            var result = await _courseService.DoesCourseExistAsync(subjectCode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetCourseAsync_CourseExists_ReturnsCourse()
        {
            // Arrange
            var subjectCode = "CS502";
            var courseTitle = "Software Engineering";

            var subject = new Subject
            {
                Code = subjectCode,
                Title = courseTitle,
                CreditHours = 4,
                IsActive = true
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetCourseAsync(subjectCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(subjectCode, result!.Code);
            Assert.Equal(courseTitle, result.Title);
            Assert.Equal(4, result.CreditHours);
        }

        [Fact]
        public async Task GetCourseAsync_CourseDoesNotExist_ReturnsNull()
        {
            // Arrange
            var subjectCode = "CS998";

            // Act
            var result = await _courseService.GetCourseAsync(subjectCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IsStudentEnrolledAsync_MultipleEnrollments_FiltersCorrectly()
        {
            // Arrange
            var studentId = 1005;
            var targetCourse = "CS601";
            var otherCourse = "CS602";

            var enrollments = new[]
            {
                new StudentEnrollment { StudentEmpNr = studentId, SubjectCode = targetCourse, Status = "enrolled" },
                new StudentEnrollment { StudentEmpNr = studentId, SubjectCode = otherCourse, Status = "enrolled" },
                new StudentEnrollment { StudentEmpNr = 9999, SubjectCode = targetCourse, Status = "enrolled" } // Different student
            };

            _context.StudentEnrollments.AddRange(enrollments);
            await _context.SaveChangesAsync();

            // Act
            var resultTarget = await _courseService.IsStudentEnrolledAsync(studentId, targetCourse);
            var resultOther = await _courseService.IsStudentEnrolledAsync(studentId, otherCourse);
            var resultWrongStudent = await _courseService.IsStudentEnrolledAsync(8888, targetCourse);

            // Assert
            Assert.True(resultTarget);
            Assert.True(resultOther);
            Assert.False(resultWrongStudent);
        }

        [Fact]
        public async Task DoesFacultyTeachCourseAsync_MultipleFaculty_FiltersCorrectly()
        {
            // Arrange
            var facultyId = 2005;
            var targetCourse = "CS701";
            var otherCourse = "CS702";

            var teachings = new[]
            {
                new Teaching { AcademicEmpNr = facultyId, SubjectCode = targetCourse },
                new Teaching { AcademicEmpNr = facultyId, SubjectCode = otherCourse },
                new Teaching { AcademicEmpNr = 9999, SubjectCode = targetCourse } // Different faculty
            };

            _context.Teachings.AddRange(teachings);
            await _context.SaveChangesAsync();

            // Act
            var resultTarget = await _courseService.DoesFacultyTeachCourseAsync(facultyId, targetCourse);
            var resultOther = await _courseService.DoesFacultyTeachCourseAsync(facultyId, otherCourse);
            var resultWrongFaculty = await _courseService.DoesFacultyTeachCourseAsync(8888, targetCourse);

            // Assert
            Assert.True(resultTarget);
            Assert.True(resultOther);
            Assert.False(resultWrongFaculty);
        }

        private void SeedTestData()
        {
            // Create test subjects
            var subjects = new[]
            {
                new Subject { Code = "SEED101", Title = "Introduction to Testing", CreditHours = 3, IsActive = true },
                new Subject { Code = "SEED102", Title = "Advanced Testing", CreditHours = 4, IsActive = true }
            };

            _context.Subjects.AddRange(subjects);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}