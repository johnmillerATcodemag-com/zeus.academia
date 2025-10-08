# Business Rules for Academic Management System

These business are partially defined in academia.orm which is an Object Role Model (ORM) diagram. This model is described in the white paper https://orm.net/pdf/ORMwhitePaper.pdf.

This document outlines the business rules implemented in the Academic Management System, ensuring compliance with the specified requirements.
These rules govern the relationships and constraints between various entities such as Academics, Departments, Rooms, Extensions, and more.

## Key Entities
- **Academic**: Represents faculty members with attributes like empNr, EmpName, Rank, and contract details.
- **Dept**: Represents academic departments with attributes like name, budget, and leadership.
- **Room**: Represents physical rooms with unique identifiers.
- **Extension**: Represents communication extensions with access levels.
- **AccessLevel**: Represents different levels of access for academics.
- **Degree**: Represents academic qualifications obtained by academics.
- **University**: Represents institutions from which degrees are obtained.
- **Chair**: Represents leadership positions held by professors.
- **Committee**: Represents committees served by teaching professors.
- **Subject**: Represents subjects taught by academics.

## Conceptual Model

<img src="https://epsenterprise.blob.core.windows.net/permanent-files/FileAttachments/829ed7ac_880b_45a9_b642_0fb5f6a885a6/Academic_Model.png" alt="Academic-Model.png" />

## Facts and Constraints
Academic is an entity type.
Reference Scheme: Academic has empNr.
Reference Mode: empNr.
Data Type: Text: Fixed Length (6).
Academic has empNr.

Academic has EmpName.

Academic works for Dept.

Academic is tenured.

Academic is contracted until Date.

Each Professor is an instance of Academic.

Academic has Rank.

Academic obtained Degree from University.

Academic teaches Subject.

Each Teacher is an instance of Academic.

Academic uses Extension.

Academic occupies Room.
Examples: '715', '430', '139', '544', '721'

EmpName is a value type.
Data Type: Text: Variable Length (15).

Academic has EmpName.
Examples: 'Adams A', 'Codd EF', 'Rankin B', 'Thompson S', 'Zack Z'

Dept is an entity type.
Reference Scheme: Dept has Dept_name.
Reference Mode: .name.
Data Type: Text: Variable Length (15).

Dept has Dept_name.

Academic works for Dept.

Dept has head with home PhoneNr.

Dept has teaching budget of MoneyAmt.

Dept has research budget of MoneyAmt.

Professor heads Dept.
Examples: 'Computer Science', 'Genetics'

Date is an entity type.
Reference Scheme: Date has mdy.
Reference Mode: mdy.
Data Type: Temporal: Date.

Date has mdy.

Academic is contracted until Date.
Examples: 01/3/2028, 01/31/2028

Professor is an entity type.
Reference Scheme: Academic has empNr.
Reference Mode: empNr.
Data Type: Text: Fixed Length (6).

Each Professor is an instance of Academic.

Professor heads Dept.

Professor holds Chair.

Each Teaching Prof is an instance of Professor.
Examples: '430'

Rank is an entity type.
Reference Scheme: Rank has Rank_code.
Reference Mode: .code.
Data Type: Text: Fixed Length (0).
The possible values of Rank are 'P', 'SL', 'L'.

Rank has Rank_code.

Academic has Rank.

Rank ensures AccessLevel.

Degree is an entity type.
Reference Scheme: Degree has Degree_code.
Reference Mode: .code.
Data Type: Text: Fixed Length (0).

Degree has Degree_code.

Academic obtained Degree from University.
Examples: 'PHD', 'MCS', 'BSc'

University is an entity type.
Reference Scheme: University has University_code.
Reference Mode: .code.
Data Type: Text: Fixed Length (0).
University has University_code.

Academic obtained Degree from University.
Examples: 'UCSD', 'MIT', 'USW', 'UQ'

Subject is an entity type.
Reference Scheme: Subject has Subject_code.
Reference Mode: .code.
Data Type: Text: Fixed Length (0).

Subject has Subject_code.

Academic teaches Subject.

Rating is an entity type.
Reference Scheme: Rating has Rating_nr.
Reference Mode: .nr.
Data Type: Numeric: Signed Integer.
Rating has Rating_nr.
The possible values of Rating are at least 1 to at most 7.

Teaching gets Rating.

PhoneNr is a value type.
Data Type: Numeric: Decimal.

Dept has head with home PhoneNr.
Examples: 5551212, 6661212

MoneyAmt is an entity type.
Reference Scheme: MoneyAmt has usd.
Reference Mode: usd.
Data Type: Numeric: Money.
MoneyAmt has usd.

Dept has teaching budget of MoneyAmt.

Dept has research budget of MoneyAmt.
Examples: 150000, 200000, 305000, 450000

Chair is an entity type.
Reference Scheme: Chair has Chair_name.
Reference Mode: .name.
Data Type: Text: Variable Length (0).
Chair has Chair_name.

Professor holds Chair.
Examples: 'Databases'

Teacher is an entity type.
Reference Scheme: Academic has empNr.
Reference Mode: empNr.
Data Type: Text: Fixed Length (6).

Teacher is audited by Teacher.
Each Teacher is an instance of Academic.
Each Teaching Prof is an instance of Teacher.

Commitee is an entity type.
Reference Scheme: Commitee has Commitee_name.
Reference Mode: .name.
Data Type: Text: Variable Length (0).
Commitee has Commitee_name.

Teaching Prof serves on Commitee.

Teaching Prof is an entity type.
Reference Scheme: Academic has empNr.
Reference Mode: empNr.
Data Type: Text: Fixed Length (6).

Teaching Prof serves on Commitee.

Each Teaching Prof is an instance of Teacher.

Each Teaching Prof is an instance of Professor.

AccessLevel is an entity type.
Reference Scheme: AccessLevel has AccessLevel_code.
Reference Mode: .code.
Data Type: Text: Fixed Length (0).

Rank ensures AccessLevel.
AccessLevel has AccessLevel_code.
The possible values of AccessLevel are 'INT', 'NAT', 'LOC'.

Extension is an entity type.
Reference Scheme: Extension has extNr.
Reference Mode: extNr.
Data Type: Numeric: Decimal.
Extension has extNr.

Academic uses Extension.
Examples: 2345, 3456, 4567, 5678, 6789

Room is an entity type.
Reference Scheme: Room is in Building; Room has RoomNr.
Room has RoomNr.

Academic occupies Room.

Room is in Building.
Examples: (211, 301), (101, 132), (211, 331), (512, 434), (512, 222)

RoomNr is a value type.
Data Type: Numeric: Decimal.

Room has RoomNr.
Examples: 301, 331, 132, 434, 222

Building is an entity type.
Reference Scheme: Building has bldgNr.
Reference Mode: bldgNr.
Data Type: Numeric: Decimal.

Building has bldgNr.
Room is in Building.
Building has BldgName.
Examples: 101, 211, 312, 512, 435

BldgName is a value type.
Data Type: Text: Variable Length (15).

Building has BldgName.
Examples: 'Ada Hall', 'Dermink Hall', 'Merlin Hall', 'Minnow Hall', 'Patterson Tower'

Academic has EmpName.

Each Academic has exactly one EmpName.
It is possible that more than one Academic has the same EmpName.
Examples:
Academic '715' has EmpName 'Adams A'.
Academic '430' has EmpName 'Codd EF'.
Academic '139' has EmpName 'Rankin B'.
Academic '544' has EmpName 'Thompson S'.
Academic '721' has EmpName 'Zack Z'.

Academic works for Dept.
Each Academic works for exactly one Dept.
It is possible that more than one Academic works for the same Dept.
Examples:
Academic '715' works for Dept 'Computer Science'.
Academic '430' works for Dept 'Computer Science'.
Academic '139' works for Dept 'Genetics'.
Academic '544' works for Dept 'Genetics'.
Academic '721' works for Dept 'Computer Science'.

Academic is tenured.
In each population of Academic is tenured, each Academic occurs at most once.
Examples:
Academic '139' is tenured.

Academic is contracted until Date.
Each Academic is contracted until at most one Date.
It is possible that more than one Academic is contracted until the same Date.
Examples:
Academic '715' is contracted until Date 01/31/2028.
Academic '430' is contracted until Date 01/3/2028.

Academic has Rank.
Each Academic has exactly one Rank.
It is possible that more than one Academic has the same Rank.
Examples:
Academic '715' has Rank 'P'.
Academic '139' has Rank 'SL'.
Academic '430' has Rank 'L'.
Academic '544' has Rank 'P'.
Academic '721' has Rank 'SL'.

Academic obtained Degree from University.
For each Academic and Degree,
that Academic obtained that Degree from at most one University.
This association with Academic, Degree provides the preferred identification scheme for AcademicObtainedDegreeFromUniversity.
Each Academic obtained some Degree from some University.
Examples:
Academic '139' obtained Degree 'PHD' from University 'UCSD'.
Academic '430' obtained Degree 'BSc' from University 'UQ'.
Academic '715' obtained Degree 'PHD' from University 'USW'.
Academic '721' obtained Degree 'MCS' from University 'MIT'.
Academic '544' obtained Degree 'PHD' from University 'USW'.

Academic teaches Subject.
It is possible that some Academic teaches more than one Subject
and that for some Subject, more than one Academic teaches that Subject.
In each population of Academic teaches Subject, each Academic, Subject combination occurs at most once.
This association with Academic, Subject provides the preferred identification scheme for Teaching.
Teaching gets Rating.
Each Teaching gets at most one Rating.
It is possible that more than one Teaching gets the same Rating.
Dept has head with home PhoneNr.
Each Dept has head with home exactly one PhoneNr.
It is possible that more than one Dept has head with home the same PhoneNr.
Examples:
Dept 'Genetics' has head with home PhoneNr 5551212.
Dept 'Computer Science' has head with home PhoneNr 6661212.

Dept has teaching budget of MoneyAmt.
Each Dept has teaching budget of exactly one MoneyAmt.
It is possible that more than one Dept has teaching budget of the same MoneyAmt.
Examples:
Dept 'Genetics' has teaching budget of MoneyAmt 200000.
Dept 'Computer Science' has teaching budget of MoneyAmt 305000.

Dept has research budget of MoneyAmt.
Each Dept has research budget of exactly one MoneyAmt.
It is possible that more than one Dept has research budget of the same MoneyAmt.
Examples:
Dept 'Computer Science' has research budget of MoneyAmt 150000.
Dept 'Genetics' has research budget of MoneyAmt 450000.

Professor heads Dept.
Each Professor heads at most one Dept.
For each Dept, at most one Professor heads that Dept.
Examples:
Professor '430' heads Dept 'Computer Science'.

Professor holds Chair.
Each Professor holds exactly one Chair.
For each Chair, at most one Professor holds that Chair.
Examples:
Professor '430' holds Chair 'Databases'.

Teacher is audited by Teacher.
Each Teacher is audited by at most one Teacher.
It is possible that some Teacher audits more than one Teacher.
No Teacher is audited by the same Teacher.
Teaching Prof serves on Commitee.
It is possible that some Teaching Prof serves on more than one Commitee
and that for some Commitee, more than one Teaching Prof serves on that Commitee.
In each population of Teaching Prof serves on Commitee, each Teaching Prof, Commitee combination occurs at most once.
This association with Teaching Prof, Commitee provides the preferred identification scheme for TeachingProfServesOnCommitee.

Rank ensures AccessLevel.
Each Rank ensures exactly one AccessLevel.
It is possible that more than one Rank ensures the same AccessLevel.
Examples:
Rank 'P' ensures AccessLevel 'INT'.
Rank 'SL' ensures AccessLevel 'NAT'.
Rank 'L' ensures AccessLevel 'LOC'.

Academic uses Extension.
Each Academic uses exactly one Extension.
Each Extension is used by at most one Academic.
Examples:
Academic '715' uses Extension 2345.
Academic '139' uses Extension 3456.
Academic '430' uses Extension 4567.
Academic '721' uses Extension 5678.
Academic '544' uses Extension 6789.

Academic occupies Room.
Each Academic occupies exactly one Room.
It is possible that more than one Academic occupies the same Room.
Examples:
Academic '715' occupies Room (211, 301).
Academic '139' occupies Room (101, 132).
Academic '430' occupies Room (211, 331).
Academic '544' occupies Room (512, 434).
Academic '721' occupies Room (512, 222).

Room has RoomNr.
Each Room has exactly one RoomNr.
It is possible that more than one Room has the same RoomNr.
Room is in Building.
Each Room is in exactly one Building.
It is possible that more than one Room is in the same Building.
Building has BldgName.
Each Building has exactly one BldgName.
For each BldgName, at most one Building has that BldgName.
Examples:
Building 211 has BldgName 'Ada Hall'.
Building 101 has BldgName 'Dermink Hall'.
Building 512 has BldgName 'Merlin Hall'.
Building 435 has BldgName 'Minnow Hall'.
Building 312 has BldgName 'Patterson Tower'.

For each Dept and EmpName,
at most one Academic works for that Dept and
has that EmpName.
For each Academic, at most one of the following holds:
that Academic is tenured;
that Academic is contracted until some Date.
No Teacher is audited by the same Teacher.
For each Building and RoomNr,
at most one Room is in that Building and
has that RoomNr.
This association with Building, RoomNr provides the preferred identification scheme for Room.
If some Professor heads some Dept then some Academic that is that Professor works for that Dept.

## Workflows
Workflows are use cases that guide data capture and processing in the application. Implement the following workflows and ensure that all business rules are enforced during their execution:

Promote a Lecturer to Senior Lecturer
Promote a Senior Lecturer to Associate Professor
Promote an Associate Professor to Professor
Assign a Class to an Academic
Add a new Academic to the faculty capturing all required information and allowing the capture of optional information.
Demote a Professor to Associate Professor
Demote an Associate Professor to Senior Lecturer
Demote a Senior Lecturer to Lecturer
Remove an Academic from the faculty.
Transfer an Academic between departments, validating eligibility and updating all related information
Update an Academic's personal or professional information.
Assign administrative roles (e.g., Head of Department) to eligible Academics.
