ß
lC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Services\IFeedbackService.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
Services &
.& '

Interfaces' 1
;1 2
public 
	interface 
IFeedbackService !
{ 
Task 
< 	
Feedback	 
? 
>  
GetFeedbackByIdAsync (
(( )
Guid) -

feedbackId. 8
,8 9
Guid: >
userId? E
)E F
;F G
Task 
< 	
Guid	 
> 
AddFeedbackAsync 
(  
Guid  $
resumeId% -
,- .
Guid/ 3
	vacancyId4 =
)= >
;> ?
Task		 
<		 	
Guid			 
?		 
>		 
DeleteFeedbackAsync		 #
(		# $
Guid		$ (

feedbackId		) 3
,		3 4
Guid		5 9
userId		: @
)		@ A
;		A B
Task

 
ChangeStatusAsync

	 
(

 
Guid

 

feedbackId

  *
,

* +
FeedbackStatus

, :
status

; A
,

A B
Guid

C G
userId

H N
)

N O
;

O P
Task 
< 	
IEnumerable	 
< 
Feedback 
> 
? 
>   
GetAllFeedbacksAsync! 5
(5 6
Guid6 :
userId; A
,A B
GuidC G
?G H
IdI K
)K L
;L M
} œ%
kC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Services\FeedbackService.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
Services &
.& '
Implementations' 6
;6 7
public 
class 
FeedbackService 
: 
IFeedbackService /
{ 
IFeedbackRepository		 
_repository		 #
;		# $
public 

FeedbackService 
( 
IFeedbackRepository .

repository/ 9
)9 :
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
< 
IEnumerable !
<! "
Feedback" *
>* +
?+ ,
>, - 
GetAllFeedbacksAsync. B
(B C
GuidC G
userIdH N
,N O
GuidP T
?T U
IdV X
)X Y
{ 
return 
await 
_repository  
.  ! 
GetAllFeedbacksAsync! 5
(5 6
userId6 <
,< =
Id> @
)@ A
;A B
} 
public 

async 
Task 
< 
Feedback 
? 
>   
GetFeedbackByIdAsync! 5
(5 6
Guid6 :

feedbackId; E
,E F
GuidG K
userIdL R
)R S
{ 
return 
await 
_repository  
.  ! 
GetFeedbackByIdAsync! 5
(5 6

feedbackId6 @
,@ A
userIdB H
)H I
;I J
} 
public 

async 
Task 
< 
Guid 
> 
AddFeedbackAsync ,
(, -
Guid- 1
resumeId2 :
,: ;
Guid< @
	vacancyIdA J
)J K
{ 
Feedback 
feedback 
= 
new 
Feedback  (
(( )
)) *
{ 	
Id 
= 
Guid 
. 
NewGuid 
( 
) 
,  
resumeId 
= 
resumeId 
,  
	vacancyId   
=   
	vacancyId   !
,  ! "
}!! 	
;!!	 

Guid"" 
id"" 
="" 
await"" 
_repository"" #
.""# $
AddFeedbackAsync""$ 4
(""4 5
feedback""5 =
)""= >
;""> ?
return## 
id## 
;## 
}$$ 
public&& 

async&& 
Task&& 
<&& 
Guid&& 
?&& 
>&& 
DeleteFeedbackAsync&& 0
(&&0 1
Guid&&1 5

feedbackId&&6 @
,&&@ A
Guid&&B F
userId&&G M
)&&M N
{'' 
Guid)) 
?)) 
id)) 
=)) 
await)) 
_repository)) $
.))$ %
DeleteFeedbackAsync))% 8
())8 9

feedbackId))9 C
,))C D
userId))E K
)))K L
;))L M
return** 
id** 
;** 
}++ 
public-- 

async-- 
Task-- 
ChangeStatusAsync-- '
(--' (
Guid--( ,

feedbackId--- 7
,--7 8
FeedbackStatus--9 G
status--H N
,--N O
Guid--P T
userId--U [
)--[ \
{.. 
Feedback// 
?// 
feedback// 
=// 
await// "
_repository//# .
.//. / 
GetFeedbackByIdAsync/// C
(//C D

feedbackId//D N
,//N O
userId//P V
)//V W
;//W X
if00 

(00 
feedback00 
is00 
not00 
{00 
status00 $
:00$ %
FeedbackStatus00& 4
.004 5

InProgress005 ?
}00@ A
||00B D
feedback00E M
==00N P
null00Q U
)00U V
{11 	
throw22 
new22  
KeyNotFoundException22 *
(22* +
$"22+ -
$str22- >
{22> ?

feedbackId22? I
}22I J
$str22J T
"22T U
)22U V
;22V W
}33 	
await44 
_repository44 
.44 
ChangeStatusAsync44 +
(44+ ,
feedback44, 4
,444 5
status446 <
)44< =
;44= >
}55 
}77 ê
mC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Events\VacancyDeletedEvent.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
Events $
;$ %
public 
record 
VacancyDeletedEvent !
(! "
Guid" &
	vacancyId' 0
)0 1
;1 2î
mC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Events\VacancyCreatedEvent.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
Events $
;$ %
public 
record 
VacancyCreatedEvent !
(! "

VacancyDto" ,
Vacancy- 4
)4 5
;5 6ã
lC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Events\ResumeDeletedEvent.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Events "
;" #
public 
record 
ResumeDeletedEvent  
(  !
Guid! %
resumeId& .
). /
;/ 0ë
lC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Events\ResumeCreatedEvent.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
Events $
;$ %
public 
record 
ResumeCreatedEvent  
(  !

ResumeDtos! +
Resume, 2
)2 3
;3 4é
xC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Consumers\VacancyDeletedEventConsumer.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
	Consumers '
;' (
public 
class '
VacancyDeletedEventConsumer (
:) *
	IConsumer+ 4
<4 5
VacancyDeletedEvent5 H
>H I
{		 
private

 
readonly

 
IVacancyRepository

 '
_repository

( 3
;

3 4
private 
readonly 
ILogger 
< '
VacancyDeletedEventConsumer 8
>8 9
_logger: A
;A B
public 
'
VacancyDeletedEventConsumer &
(& '
IVacancyRepository' 9

repository: D
,D E
ILoggerF M
<M N'
VacancyDeletedEventConsumerN i
>i j
loggerk q
)q r
{ 
_repository 
= 

repository  
;  !
_logger 
= 
logger 
; 
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
VacancyDeletedEvent- @
>@ A
contextB I
)I J
{ 
try 
{ 	
Guid 
resumeId 
= 
context #
.# $
Message$ +
.+ ,
	vacancyId, 5
;5 6
await 
_repository 
. 
DeleteVacancyAsync 0
(0 1
resumeId1 9
)9 :
;: ;
} 	
catch 
( 
	Exception 
e 
) 
{ 	
_logger 
. 
LogError 
( 
e 
, 
$str  M
+N O
eP Q
.Q R
MessageR Y
)Y Z
;Z [
throw 
; 
} 	
}   
}!! Á
xC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Consumers\VacancyCreatedEventConsumer.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
	Consumers '
;' (
public

 
class

 '
VacancyCreatedEventConsumer

 (
:

) *
	IConsumer

+ 4
<

4 5
VacancyCreatedEvent

5 H
>

H I
{ 
private 
readonly 
IVacancyRepository '
_repository( 3
;3 4
private 
readonly 
ILogger 
< '
VacancyCreatedEventConsumer 8
>8 9
_logger: A
;A B
public 
'
VacancyCreatedEventConsumer &
(& '
IVacancyRepository' 9
vacancyRepository: K
,K L
ILoggerM T
<T U'
VacancyCreatedEventConsumerU p
>p q
loggerr x
)x y
{ 
_repository 
= 
vacancyRepository '
;' (
_logger 
= 
logger 
; 
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
VacancyCreatedEvent- @
>@ A
contextB I
)I J
{ 
try 
{ 	
Vacancy 
resume 
= 
new  
Vacancy! (
(( )
)) *
{ 
	vacancyId 
= 
context #
.# $
Message$ +
.+ ,
Vacancy, 3
.3 4
id4 6
,6 7
	companyId 
= 
context #
.# $
Message$ +
.+ ,
Vacancy, 3
.3 4

company_id4 >
,> ?
} 
; 
await 
_repository 
. 
AddVacancyAsync -
(- .
resume. 4
)4 5
;5 6
}   	
catch!! 
(!! 
	Exception!! 
ex!! 
)!! 
{"" 	
_logger## 
.## 
LogError## 
(## 
ex## 
,##  
ex##! #
.### $
Message##$ +
)##+ ,
;##, -
throw$$ 
;$$ 
}%% 	
}&& 
}'' É
wC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Consumers\ResumeDeletedEventConsumer.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
	Consumers '
;' (
public		 
class		 &
ResumeDeletedEventConsumer		 '
:		( )
	IConsumer		* 3
<		3 4
ResumeDeletedEvent		4 F
>		F G
{

 
private 
readonly 
IResumeRepository &
_repository' 2
;2 3
private 
readonly 
ILogger 
< &
ResumeDeletedEventConsumer 7
>7 8
_logger9 @
;@ A
public 
&
ResumeDeletedEventConsumer %
(% &
IResumeRepository& 7

repository8 B
,B C
ILoggerD K
<K L&
ResumeDeletedEventConsumerL f
>f g
loggerh n
)n o
{ 
_repository 
= 

repository  
;  !
_logger 
= 
logger 
; 
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
ResumeDeletedEvent- ?
>? @
contextA H
)H I
{ 
try 
{ 	
Guid 
resumeId 
= 
context #
.# $
Message$ +
.+ ,
resumeId, 4
;4 5
await 
_repository 
. 
DeleteResumeAsync /
(/ 0
resumeId0 8
)8 9
;9 :
} 	
catch 
( 
	Exception 
e 
) 
{ 	
_logger 
. 
LogError 
( 
e 
, 
$str  M
+N O
eP Q
.Q R
MessageR Y
)Y Z
;Z [
throw 
; 
}   	
}!! 
}"" ©
wC:\Users\demde\Desktop\Worky\Worky\Services\FeedbackService\FeedbackService.BLL\Consumers\ResumeCreatedEventConsumer.cs
	namespace 	
FeedbackService
 
. 
BLL 
. 
	Consumers '
;' (
public		 
class		 &
ResumeCreatedEventConsumer		 '
:		( )
	IConsumer		* 3
<		3 4
ResumeCreatedEvent		4 F
>		F G
{

 
private 
readonly 
IResumeRepository &
_repository' 2
;2 3
private 
readonly 
ILogger 
< &
ResumeCreatedEventConsumer 7
>7 8
_logger9 @
;@ A
public 
&
ResumeCreatedEventConsumer %
(% &
IResumeRepository& 7

repository8 B
,B C
ILoggerD K
<K L&
ResumeCreatedEventConsumerL f
>f g
loggerh n
)n o
{ 
_repository 
= 

repository  
;  !
_logger 
= 
logger 
; 
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
ResumeCreatedEvent- ?
>? @
contextA H
)H I
{ 
try 
{ 	
Resume 
resume 
= 
new 
Resume  &
(& '
)' (
{ 
resumeId 
= 
context "
." #
Message# *
.* +
Resume+ 1
.1 2
id2 4
,4 5
workerId 
= 
Guid 
.  
Parse  %
(% &
context& -
.- .
Message. 5
.5 6
Resume6 <
.< =
	worker_id= F
)F G
} 
; 
await 
_repository 
. 
AddResumeAsync ,
(, -
resume- 3
)3 4
;4 5
} 	
catch   
(   
	Exception   
e   
)   
{!! 	
_logger"" 
."" 
LogError"" 
("" 
e"" 
,"" 
$str""  N
+""O P
e""Q R
.""R S
Message""S Z
)""Z [
;""[ \
throw## 
;## 
}$$ 	
}&& 
}'' 