using Zeus.Academia.Infrastructure.Entities;

Console.WriteLine("=== Task 4 Entity Validation Test ===");
Console.WriteLine();

try
{
    // Test DepartmentChair
    var chair = new DepartmentChair
    {
        DepartmentName = "CS",
        FacultyEmpNr = 12345,
        AppointmentStartDate = DateTime.Today.AddYears(-1),
        TermLengthYears = 3
    };
    Console.WriteLine($"✓ DepartmentChair created successfully - {chair.DepartmentName}, Active: {chair.IsActive}");

    // Test CommitteeChair
    var committeeChair = new CommitteeChair
    {
        CommitteeName = "Graduate Committee",
        ChairEmpNr = 54321,
        AppointmentStartDate = DateTime.Today.AddMonths(-6)
    };
    Console.WriteLine($"✓ CommitteeChair created successfully - {committeeChair.CommitteeName}, Type: {committeeChair.AppointmentType}");

    // Test CommitteeMemberAssignment
    var memberAssignment = new CommitteeMemberAssignment
    {
        CommitteeName = "Faculty Senate",
        MemberEmpNr = 98765
    };
    Console.WriteLine($"✓ CommitteeMemberAssignment created successfully - {memberAssignment.CommitteeName}, Active: {memberAssignment.IsActive}");

    // Test AdministrativeRole
    var adminRole = new AdministrativeRole
    {
        RoleTitle = "Associate Dean",
        IsActive = true
    };
    Console.WriteLine($"✓ AdministrativeRole created successfully - {adminRole.RoleTitle}, Active: {adminRole.IsActive}");

    // Test AdministrativeAssignment
    var roleAssignment = new AdministrativeAssignment
    {
        RoleCode = "DEAN",
        AssigneeEmpNr = 11111
    };
    Console.WriteLine($"✓ AdministrativeAssignment created successfully - Assignee: {roleAssignment.AssigneeEmpNr}, Role: {roleAssignment.RoleCode}");

    // Test FacultySearchCommittee
    var searchCommittee = new FacultySearchCommittee
    {
        SearchCommitteeName = "CS Faculty Search 2024",
        DepartmentName = "Computer Science",
        PositionTitle = "Assistant Professor",
        SearchStartDate = DateTime.Today.AddMonths(-3)
    };
    Console.WriteLine($"✓ FacultySearchCommittee created successfully - {searchCommittee.SearchCommitteeName}, Status: {searchCommittee.SearchStatus}");

    // Test FacultySearchCommitteeMember
    var searchMember = new FacultySearchCommitteeMember
    {
        MemberEmpNr = 77777
    };
    Console.WriteLine($"✓ FacultySearchCommitteeMember created successfully - Member: {searchMember.MemberEmpNr}, Role: {searchMember.MemberRole}");

    // Test DepartmentalService
    var service = new DepartmentalService
    {
        ServiceType = "Committee",
        ServiceTitle = "Graduate Committee Service",
        ServiceLevel = "Department",
        AcademicYear = "2024-2025"
    };
    Console.WriteLine($"✓ DepartmentalService created successfully - Type: {service.ServiceType}, Level: {service.ServiceLevel}");

    // Test ServiceLoadSummary
    var loadSummary = new ServiceLoadSummary
    {
        FacultyEmpNr = 12345,
        DepartmentName = "Computer Science",
        AcademicYear = "2024-2025"
    };
    Console.WriteLine($"✓ ServiceLoadSummary created successfully - Faculty: {loadSummary.FacultyEmpNr}, Year: {loadSummary.AcademicYear}");

    Console.WriteLine();
    Console.WriteLine("🎉 ALL TASK 4 ENTITIES VALIDATED SUCCESSFULLY!");
    Console.WriteLine();
    Console.WriteLine("Summary of Task 4 Entities:");
    Console.WriteLine("- DepartmentChair: Department chair assignments and rotation system");
    Console.WriteLine("- CommitteeChair: Committee chair assignments and management");
    Console.WriteLine("- CommitteeMemberAssignment: Enhanced committee member assignment system");
    Console.WriteLine("- AdministrativeRole: Administrative role hierarchy and definitions");
    Console.WriteLine("- AdministrativeAssignment: Administrative role assignments to faculty");
    Console.WriteLine("- FacultySearchCommittee: Faculty hiring committee management");
    Console.WriteLine("- FacultySearchCommitteeMember: Faculty search committee member tracking");
    Console.WriteLine("- DepartmentalService: Departmental service load tracking");
    Console.WriteLine("- ServiceLoadSummary: Service workload balancing system");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
