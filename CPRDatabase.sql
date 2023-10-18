create database CPR

use CPR

create table Semester(
Id int identity(1,1) primary key not null,
Code varchar(10) not null,
StartDate date,
EndDate date,
Status bit
)
create table Specialization(
Id int identity(1,1) primary key not null,
Code nvarchar(10) not null,
Name nvarchar(50) not null,
Description varchar(max),
Status bit
)

create table Topic(
Id int identity(1,1) primary key not null,
Name nvarchar(max) not null,
Description nvarchar(max),
Status bit,
SemesterId int not null foreign key references Semester(Id),
SpecializationId int not null foreign key references Specialization(Id)
)

create table GroupProject(
Id int identity(1,1) primary key not null,
Name nvarchar(max) not null,
Status bit
)

create table TopicOfGroup(
TopicId int not null foreign key references Topic(Id),
GroupProjectId int not null foreign key references GroupProject(Id),
Status bit,
CONSTRAINT [PK_TopicOfGroup] PRIMARY KEY CLUSTERED 
(
	[TopicId] ASC,
	[GroupProjectId] ASC
)
) ON [PRIMARY]
GO
create table Role(
Id int identity(1,1) primary key not null,
Name nvarchar(20) not null,
)

create table Account(
Id int identity(1,1) primary key not null,
Code varchar(max) not null,
Name nvarchar(max) not null,
Email varchar(max) not null,
PasswordHash varbinary(max),
PasswordSalt varbinary(max),
Gender varchar(10),
Phone varchar(10),
RoleId int foreign key references Role(Id),
DateOfBirth date,
Avatar varchar(max),
Status bit,
SpecializationId int not null foreign key references Specialization(Id)
) 
create table Subject(
Id int identity(1,1) primary key not null,
Code nvarchar(10) not null,
Name nvarchar(max) not null,
isPrerequisite bit,
SpecializationId int not null foreign key references Specialization(Id),
Status bit
) 
create table StudentInSemester(
Id int identity(1,1) primary key not null,
Status bit,
StudentId int not null foreign key references Account(Id),
SubjectId int not null foreign key references Subject(Id),
SemesterId int not null foreign key references Semester(Id),
)

create table StudentInGroup(
Id int identity(1,1) primary key not null,
JoinDate date,
Status bit,
Role nvarchar(20),
Description nvarchar(max),
GroupId int not null foreign key references GroupProject(Id),
StudentId int not null foreign key references Account(Id),
)

create table TopicOfLecturer(
isSuperLecturer bit not null,
LecturerId int not null foreign key references Account(Id),
TopicId int not null foreign key references Topic(Id),
Status bit,
CONSTRAINT [PK_TopicOfLecturer] PRIMARY KEY CLUSTERED 
(
	[LecturerId] ASC,
	[TopicId] ASC
)
) ON [PRIMARY]
GO
insert into Role(Name) values('Student')
insert into Role(Name) values('Lecturer')

insert into Semester(Code,StartDate,EndDate,Status) values('Spring23','2023-01-02','2023-05-07',0)
insert into Semester(Code,StartDate,EndDate,Status) values('Summer23','2023-05-08','2023-09-03',0)
insert into Semester(Code,StartDate,EndDate,Status) values('Fall23','2023-09-04','2023-12-31',1)

insert into Specialization(Code,Name,Description,Status) values('SS','Business Administration','6 majors including Marketing, Finance, International Business, Hotel Management, Travel and Tourism Management, and Multimedia Management.',1)
insert into Specialization(Code,Name,Description,Status) values('SE','Information technology','6 majors include Software Engineering, Information Systems, Information Security, Artificial Intelligence - AI, Internet of Things - IOT, Digital Art Design.',1)
