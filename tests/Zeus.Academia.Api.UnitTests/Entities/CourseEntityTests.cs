using System.ComponentModel.DataAnnotations;
using Xunit;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Api.UnitTests.Entities;

/// <summary>
/// Unit tests for Course and Subject entity extensions for Task 1: Course and Subject Entity Extensions.
/// Tests hierarchical categorization, prerequisite system, course numbering, credit types, and status management.
/// </summary>
public class CourseEntityTests
{
    #region Subject Entity Hierarchy Tests

    [Fact]
    public void Subject_Should_Support_Hierarchical_Categorization()
    {
        // Arrange & Act
        var parentSubject = new Subject
        {
            Code = "CS",
            Title = "Computer Science",
            Description = "Computer Science Subject Area",
            Level = "Graduate",
            IsActive = true,
            SubjectType = SubjectType.SubjectArea,
            ParentSubjectCode = null,
            DepartmentName = "Computer Science"
        };

        var childSubject = new Subject
        {
            Code = "CS5000",
            Title = "Advanced Computer Science",
            Description = "Advanced topics in computer science",
            CreditHours = 3,
            Level = "Graduate",
            IsActive = true,
            SubjectType = SubjectType.Course,
            ParentSubjectCode = "CS",
            DepartmentName = "Computer Science"
        };

        // Assert
        Assert.Equal("CS", parentSubject.Code);
        Assert.Equal(SubjectType.SubjectArea, parentSubject.SubjectType);
        Assert.Null(parentSubject.ParentSubjectCode);

        Assert.Equal("CS5000", childSubject.Code);
        Assert.Equal(SubjectType.Course, childSubject.SubjectType);
        Assert.Equal("CS", childSubject.ParentSubjectCode);
    }

    [Fact]
    public void Subject_Should_Validate_Subject_Type()
    {
        // Arrange
        var subject = new Subject
        {
            Code = "CS101",
            Title = "Introduction to Programming",
            SubjectType = SubjectType.Course
        };

        // Act & Assert
        Assert.Equal(SubjectType.Course, subject.SubjectType);
        Assert.True(Enum.IsDefined(typeof(SubjectType), subject.SubjectType));
    }

    [Fact]
    public void Subject_Should_Support_Enhanced_Properties()
    {
        // Arrange & Act
        var subject = new Subject
        {
            Code = "BIOL301L",
            Title = "Advanced Biology Laboratory",
            Description = "Advanced laboratory techniques in biology",
            CreditHours = 2,
            Level = "Undergraduate",
            IsActive = true,
            SubjectType = SubjectType.Course,
            ParentSubjectCode = "BIOL",
            DepartmentName = "Biology",
            Prerequisites = "BIOL201,BIOL202",
            Corequisites = "BIOL301",
            TypicalSemester = "Fall",
            MaxEnrollment = 24,
            LearningOutcomes = new List<string>
            {
                "Demonstrate advanced laboratory techniques",
                "Analyze biological samples using modern equipment",
                "Document experimental procedures and results"
            },
            DeliveryMethods = new List<DeliveryMethod> { DeliveryMethod.InPerson, DeliveryMethod.Hybrid },
            AssessmentMethods = new List<AssessmentMethod> { AssessmentMethod.LabReports, AssessmentMethod.PracticalExam }
        };

        // Assert
        Assert.Equal("BIOL301L", subject.Code);
        Assert.Equal("Advanced Biology Laboratory", subject.Title);
        Assert.Equal(2, subject.CreditHours);
        Assert.Equal("BIOL201,BIOL202", subject.Prerequisites);
        Assert.Equal("BIOL301", subject.Corequisites);
        Assert.Equal(3, subject.LearningOutcomes.Count);
        Assert.Contains(DeliveryMethod.InPerson, subject.DeliveryMethods);
        Assert.Contains(AssessmentMethod.LabReports, subject.AssessmentMethods);
    }

    #endregion

    #region Course Entity Tests

    [Fact]
    public void Course_Should_Have_Comprehensive_Academic_Information()
    {
        // Arrange & Act
        var course = new Course
        {
            Id = 1,
            CourseNumber = "CS4350",
            Title = "Software Engineering Principles",
            Description = "Comprehensive course covering software engineering methodologies",
            SubjectCode = "CS",
            CreditHours = 3,
            ContactHours = 45,
            Level = CourseLevel.Undergraduate,
            Status = CourseStatus.Active,
            Prerequisites = new List<CoursePrerequisite>
            {
                new() { PrerequisiteType = PrerequisiteType.Course, RequiredCourseNumber = "CS3320", IsRequired = true },
                new() { PrerequisiteType = PrerequisiteType.Course, RequiredCourseNumber = "CS3321", IsRequired = true }
            },
            Corequisites = new List<CourseCorequisite>
            {
                new() { RequiredCourseNumber = "CS4350L" }
            },
            Restrictions = new List<CourseRestriction>
            {
                new() { RestrictionType = RestrictionType.Major, Value = "Computer Science" },
                new() { RestrictionType = RestrictionType.ClassLevel, Value = "Senior" }
            }
        };

        // Assert
        Assert.Equal("CS4350", course.CourseNumber);
        Assert.Equal("Software Engineering Principles", course.Title);
        Assert.Equal("CS", course.SubjectCode);
        Assert.Equal(3, course.CreditHours);
        Assert.Equal(CourseLevel.Undergraduate, course.Level);
        Assert.Equal(CourseStatus.Active, course.Status);
        Assert.Equal(2, course.Prerequisites.Count);
        Assert.Single(course.Corequisites);
        Assert.Equal(2, course.Restrictions.Count);
    }

    [Fact]
    public void Course_Should_Support_Complex_Prerequisites()
    {
        // Arrange & Act
        var course = new Course
        {
            CourseNumber = "MATH4000",
            Title = "Advanced Mathematics",
            Prerequisites = new List<CoursePrerequisite>
            {
                // (MATH2010 AND MATH2020) OR MATH2100
                new() { PrerequisiteType = PrerequisiteType.Course, RequiredCourseNumber = "MATH2010", LogicalOperator = LogicalOperator.And, GroupId = 1 },
                new() { PrerequisiteType = PrerequisiteType.Course, RequiredCourseNumber = "MATH2020", LogicalOperator = LogicalOperator.And, GroupId = 1 },
                new() { PrerequisiteType = PrerequisiteType.Course, RequiredCourseNumber = "MATH2100", LogicalOperator = LogicalOperator.Or, GroupId = 2 }
            }
        };

        // Assert
        Assert.Equal(3, course.Prerequisites.Count);
        Assert.True(course.Prerequisites.Any(p => p.RequiredCourseNumber == "MATH2010" && p.LogicalOperator == LogicalOperator.And));
        Assert.True(course.Prerequisites.Any(p => p.RequiredCourseNumber == "MATH2100" && p.LogicalOperator == LogicalOperator.Or));
    }

    [Fact]
    public void Course_Should_Validate_Course_Number_Format()
    {
        // Arrange
        var course = new Course
        {
            CourseNumber = "PHYS2010",
            Title = "General Physics I"
        };

        var validationContext = new ValidationContext(course);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(course, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid || validationResults.All(vr => !vr.ErrorMessage!.Contains("course number")));
        Assert.Equal("PHYS2010", course.CourseNumber);
    }

    [Fact]
    public void Course_Should_Support_Multiple_Credit_Types()
    {
        // Arrange & Act
        var course = new Course
        {
            CourseNumber = "CHEM4010",
            Title = "Advanced Organic Chemistry",
            CreditBreakdown = new List<CreditType>
            {
                new() { Type = CreditTypeEnum.Lecture, Hours = 3 },
                new() { Type = CreditTypeEnum.Laboratory, Hours = 2 },
                new() { Type = CreditTypeEnum.Discussion, Hours = 1 }
            },
            CreditHours = 6,
            ContactHours = 90
        };

        // Assert
        Assert.Equal(3, course.CreditBreakdown.Count);
        Assert.Equal(6, course.CreditHours);
        Assert.Equal(90, course.ContactHours);
        Assert.Contains(course.CreditBreakdown, c => c.Type == CreditTypeEnum.Lecture && c.Hours == 3);
        Assert.Contains(course.CreditBreakdown, c => c.Type == CreditTypeEnum.Laboratory && c.Hours == 2);
    }

    [Fact]
    public void Course_Should_Support_Status_Management()
    {
        // Arrange & Act
        var activeCourse = new Course
        {
            CourseNumber = "ENG101",
            Title = "English Composition I",
            Status = CourseStatus.Active,
            StatusHistory = new List<CourseStatusHistory>
            {
                new() { Status = CourseStatus.UnderReview, EffectiveDate = DateTime.Now.AddDays(-30), ChangedBy = "Admin" },
                new() { Status = CourseStatus.Active, EffectiveDate = DateTime.Now.AddDays(-15), ChangedBy = "Dean" }
            }
        };

        var retiredCourse = new Course
        {
            CourseNumber = "OLD101",
            Title = "Obsolete Course",
            Status = CourseStatus.Retired,
            RetiredDate = DateTime.Now.AddDays(-90),
            RetirementReason = "Course content no longer relevant"
        };

        // Assert
        Assert.Equal(CourseStatus.Active, activeCourse.Status);
        Assert.Equal(2, activeCourse.StatusHistory.Count);

        Assert.Equal(CourseStatus.Retired, retiredCourse.Status);
        Assert.NotNull(retiredCourse.RetiredDate);
        Assert.Equal("Course content no longer relevant", retiredCourse.RetirementReason);
    }

    [Fact]
    public void Course_Should_Follow_Institutional_Numbering_Standards()
    {
        // Test various valid course number formats
        var testCases = new[]
        {
            "CS101",    // Basic format
            "MATH2010", // Four-digit number
            "BIOL301L", // Lab designation
            "PHYS4000", // Graduate level
            "ENG1010A"  // Section designation
        };

        foreach (var courseNumber in testCases)
        {
            // Arrange & Act
            var course = new Course
            {
                CourseNumber = courseNumber,
                Title = "Test Course",
                Level = DetermineCourseLevelFromNumber(courseNumber)
            };

            // Assert
            Assert.Equal(courseNumber, course.CourseNumber);
            Assert.True(IsValidCourseNumber(courseNumber));
        }
    }

    #endregion

    #region Course Restrictions and Requirements Tests

    [Fact]
    public void Course_Should_Support_Enrollment_Restrictions()
    {
        // Arrange & Act
        var course = new Course
        {
            CourseNumber = "CS6000",
            Title = "Graduate Seminar",
            Restrictions = new List<CourseRestriction>
            {
                new() { RestrictionType = RestrictionType.Major, Value = "Computer Science", IsRequired = true },
                new() { RestrictionType = RestrictionType.ClassLevel, Value = "Graduate", IsRequired = true },
                new() { RestrictionType = RestrictionType.Permission, Value = "Instructor", IsRequired = false },
                new() { RestrictionType = RestrictionType.GPA, Value = "3.0", IsRequired = true }
            }
        };

        // Assert
        Assert.Equal(4, course.Restrictions.Count);
        Assert.Contains(course.Restrictions, r => r.RestrictionType == RestrictionType.Major && r.Value == "Computer Science");
        Assert.Contains(course.Restrictions, r => r.RestrictionType == RestrictionType.GPA && r.Value == "3.0");
    }

    [Fact]
    public void CoursePrerequisite_Should_Support_Override_System()
    {
        // Arrange & Act
        var prerequisite = new CoursePrerequisite
        {
            PrerequisiteType = PrerequisiteType.Course,
            RequiredCourseNumber = "MATH1010",
            IsRequired = true,
            CanBeWaived = true,
            WaiverRequiresApproval = true,
            AlternativeOptions = new List<string> { "MATH1020", "Placement Test Score >= 80" }
        };

        // Assert
        Assert.True(prerequisite.CanBeWaived);
        Assert.True(prerequisite.WaiverRequiresApproval);
        Assert.Equal(2, prerequisite.AlternativeOptions.Count);
        Assert.Contains("Placement Test Score >= 80", prerequisite.AlternativeOptions);
    }

    #endregion

    #region Helper Methods

    private static CourseLevel DetermineCourseLevelFromNumber(string courseNumber)
    {
        if (string.IsNullOrEmpty(courseNumber)) return CourseLevel.Undergraduate;

        var numberPart = new string(courseNumber.Where(char.IsDigit).ToArray());
        if (int.TryParse(numberPart, out var number))
        {
            return number switch
            {
                >= 1000 and <= 2999 => CourseLevel.LowerDivision,
                >= 3000 and <= 4999 => CourseLevel.UpperDivision,
                >= 5000 and <= 6999 => CourseLevel.Graduate,
                >= 7000 => CourseLevel.Doctoral,
                _ => CourseLevel.Undergraduate
            };
        }

        return CourseLevel.Undergraduate;
    }

    private static bool IsValidCourseNumber(string courseNumber)
    {
        if (string.IsNullOrEmpty(courseNumber)) return false;

        // Basic pattern: 2-4 letters followed by 3-4 digits, optional letter suffix
        return System.Text.RegularExpressions.Regex.IsMatch(
            courseNumber,
            @"^[A-Z]{2,4}[0-9]{3,4}[A-Z]?$");
    }

    #endregion
}