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
Description varchar(max)
)

alter table Specialization
add Status bit

create table GroupProject(
Id int identity(1,1) primary key not null,
Name nvarchar(50) not null,
Status bit
)
create table Topic(
Id int identity(1,1) primary key not null,
Name nvarchar(50) not null,
Description nvarchar(200),
Status bit,
GroupId int foreign key references GroupProject(Id),
SemesterId int foreign key references Semester(Id),
SpecializationId int foreign key references Specialization(Id)
)


create table Role(
Id int identity(1,1) primary key not null,
Name nvarchar(20) not null,
)

create table Account(
Id int identity(1,1) primary key not null,
Code varchar(max) not null,
Name nvarchar(50) not null,
Email varchar(50) not null,
PasswordHash varbinary(max) ,
PasswordSalt varbinary(max) ,
Gender varchar(10),
GoogleId varchar(max) null,
Phone varchar(10),
RoleId int foreign key references Role(Id),
DateOfBirth date,
Avatar varchar(max)
)

alter table Account
add Status bit

create table StudentInGroup(
Id int identity(1,1) primary key not null,
JoinDate date,
Status bit,
Role nvarchar(20),
Description nvarchar(200),
GroupProjectId int foreign key references GroupProject(Id),
StudentId int foreign key references Account(Id)
)

create table Subject(
Id int identity(1,1) primary key not null,
Code nvarchar(10) not null,
Name nvarchar(50) not null,
isPrerequisite bit,
SpecializationId int foreign key references Specialization(Id),
)

alter table Subject
add Status bit

create table StudentInSemester(
Id int identity(1,1) primary key not null,
Status bit,
StudentId int foreign key references Account(Id),
SubjectId int foreign key references Subject(Id),
SemesterId int foreign key references Semester(Id)
)

create table TopicOfLecturer(
Id int identity(1,1) primary key not null,
isSuperLecturer bit not null,
LecturerId int foreign key references Account(Id),
TopicId int foreign key references Topic(Id)
)
create table TopicOfGroup(
Id int identity(1,1) primary key not null,
TopicId int not null references Topic(Id),
GroupProjectId int not null references GroupProject(Id)
)
alter table TopicOfLecturer
add Status bit

alter table Account
add SpecializationId int foreign key references Specialization(Id)
