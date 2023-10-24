alter table Account
add Token nvarchar(max) null
alter table Account
add JwtId nvarchar(max) null
alter table Account
add AddedDate datetime null
alter table Account
add ExpiredDate datetime null