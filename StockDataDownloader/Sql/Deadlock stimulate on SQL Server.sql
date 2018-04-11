--Deadlock stimulate on SQL Server 2008 R2

--Step 1

--note : query window1

create database dbtest

alter database dbtest set read_committed_snapshot on;


use dbtest

Dbcc traceon (1204,-1) 

DBCC Traceon (1222,-1) 

Create table dbo.Test1 (col1 int);
Create table dbo.Test2 (col1 int);

Insert into dbo.Test1 values(1)
Insert into dbo.Test1 values(2) 
Insert into dbo.Test1 values(3) 
Insert into dbo.Test2 values(1)
Insert into dbo.Test2 values(2)
Insert into dbo.Test2 values(3)

set deadlock_priority 1

set TRANSACTION isolation level SERIALIZABLE

--or set TRANSACTION isolation level
set TRANSACTION isolation level read committed

set deadlock_priority 2

begin tran 
update  dbo.Test1  set col1 = 5



--step2

--query windows 2

use dbtest

Dbcc traceon (1204,-1) 

DBCC Traceon (1222,-1) 

set deadlock_priority 2

set TRANSACTION isolation level  SERIALIZABLE

--or set TRANSACTION isolation level
set TRANSACTION isolation level read committed

begin tran
update dbo.Test2    set col1 = 1

--step3

--go to query windows1  and execute below query
update dbo.Test2   set col1 = 6
--step4

--go to query windows2  and execute below query
update  dbo.Test1  set col1 = 3
--now you can see deadlock at query windows1

--Msg 1205, Level 13, State 45, Line 1
--Transaction (Process ID 64) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.

