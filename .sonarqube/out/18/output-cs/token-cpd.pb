ì

lC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Services\VacancySearchService.cs
	namespace		 	
SearchService		
 
.		 
BLL		 
.		 
Services		 $
.		$ %
Implementations		% 4
;		4 5
public 
class  
VacancySearchService !
:" #!
IVacancySearchService$ 9
{ 
private 
readonly %
IVacancyElasticRepository .
_repository/ :
;: ;
public 
 
VacancySearchService 
(  %
IVacancyElasticRepository  9

repository: D
)D E
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
< 
SearchResponse $
<$ %"
VacancySearchResultDto% ;
>; <
>< =
SearchAsync> I
(I J
GetVacanciesRequestJ ]
request^ e
)e f
{ 
var 
result 
= 
await 
_repository &
.& '
SearchAsync' 2
(2 3
request3 :
): ;
;; <
return 
result 
; 
} 
} ã

kC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Services\ResumeSearchService.cs
	namespace 	
SearchService
 
. 
BLL 
. 
Services $
.$ %
Implementations% 4
;4 5
public

 
class

 
ResumeSearchService

  
:

! " 
IResumeSearchService

# 7
{ 
private 
readonly $
IResumeElasticRepository -
_repository. 9
;9 :
public 

ResumeSearchService 
( $
IResumeElasticRepository 7

repository8 B
)B C
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
< 
SearchResponse $
<$ %!
ResumeSearchResultDto% :
>: ;
>; <
SearchAsync= H
(H I
GetResumesRequestI Z
request[ b
)b c
{ 
var 
result 
= 
await 
_repository &
.& '
SearchAsync' 2
(2 3
request3 :
): ;
;; <
return 
result 
; 
} 
} Š
mC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Services\IVacancySearchService.cs
	namespace 	
SearchService
 
. 
BLL 
. 
Services $
.$ %

Interfaces% /
;/ 0
public 
	interface !
IVacancySearchService &
{		 
Task

 
<

 	
SearchResponse

	 
<

 "
VacancySearchResultDto

 .
>

. /
>

/ 0
SearchAsync

1 <
(

< =
GetVacanciesRequest

= P
request

Q X
)

X Y
;

Y Z
} …
lC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Services\IResumeSearchService.cs
	namespace 	
SearchService
 
. 
BLL 
. 
Services $
.$ %

Interfaces% /
;/ 0
public 
	interface  
IResumeSearchService %
{		 
Task

 
<

 	
SearchResponse

	 
<

 !
ResumeSearchResultDto

 -
>

- .
>

. /
SearchAsync

0 ;
(

; <
GetResumesRequest

< M
request

N U
)

U V
;

V W
} ì6
_C:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\MappingExtension.cs
	namespace 	
SearchService
 
. 
BLL 
. 
Mapping #
;# $
public 
static 
class 
MappingExtensions %
{ 
public		 

static		 
ResumeDocument		  

ToDocument		! +
(		+ ,
this		, 0

ResumeDtos		1 ;
dto		< ?
)		? @
=>

 

new

 
(

 
)

 
{ 	
id 
= 
dto 
. 
id 
, 
workerId 
= 
dto 
. 
	worker_id $
!$ %
,% &
skill 
= 
dto 
. 
skill 
??  
$str! #
,# $
city 
= 
dto 
. 
city 
! 
, 

experience 
= 
dto 
. 

experience '
??( *
$num+ ,
,, -
educationId 
= 
dto 
. 
education_id *
,* +
educationName 
= 
dto 
.  
education_name  .
,. /

incomeDate 
= 
dto 
. 
income_date (
,( )
wantedSalary 
= 
dto 
. 
wantedSalary +
,+ ,
post 
= 
dto 
. 
post 
! 
, 
worker 
= 
new 

WorkerInfo #
{ 
id 
= 
dto 
. 
worker 
!  
.  !
id! #
,# $
	firstName 
= 
dto 
.  
worker  &
.& '

first_name' 1
,1 2

secondName 
= 
dto  
.  !
worker! '
.' (
second_name( 3
,3 4
surname 
= 
dto 
. 
worker $
.$ %
surname% ,
,, -
birthday 
= 
dto 
. 
worker %
.% &
birthday& .
,. /
age 
= 
dto 
. 
worker  
.  !
age! $
} 
, 

activities 
= 
dto 
. 

activities '
?' (
.( )
Select) /
(/ 0
a0 1
=>2 4
new5 8
Activity9 A
{   
id!! 
=!! 
a!! 
.!! 
id!! 
,!! 
	direction"" 
="" 
a"" 
."" 
	direction"" '
,""' (
type## 
=## 
a## 
.## 
type## 
}$$ 
)$$ 
.$$ 
ToList$$ 
($$ 
)$$ 
??$$ 
new$$ 
($$ 
)$$  
}%% 	
;%%	 

public'' 

static'' 
VacancyDocument'' !

ToDocument''" ,
('', -
this''- 1
VacancyDtos''2 =
dto''> A
)''A B
=>(( 

new(( 
((( 
)(( 
{)) 	
id** 
=** 
dto** 
.** 
id** 
,** 
	companyId++ 
=++ 
dto++ 
.++ 

company_id++ &
??++' )
Guid++* .
.++. /
Empty++/ 4
,++4 5
post,, 
=,, 
dto,, 
.,, 
post,, 
,,, 
description-- 
=-- 
dto-- 
.-- 
description-- )
??--* ,
$str--- /
,--/ 0
	minSalary.. 
=.. 
dto.. 
... 

min_salary.. &
,..& '
	maxSalary// 
=// 
dto// 
.// 

max_salary// &
,//& '

experience00 
=00 
dto00 
.00 

experience00 '
,00' (
educationId11 
=11 
dto11 
.11 
education_id11 *
,11* +
educationName22 
=22 
dto22 
.22  
education_name22  .
,22. /

incomeDate33 
=33 
dto33 
.33 
income_date33 (
,33( )

workFormat44 
=44 
dto44 
.44 
work_format_name44 -
,44- .
workHour55 
=55 
dto55 
.55 
work_hour_name55 )
,55) *
location66 
=66 
dto66 
.66 
Location66 #
,66# $
company77 
=77 
new77 
CompanyInfo77 %
{88 
id99 
=99 
dto99 
.99 
company99  
?99  !
.99! "
id99" $
??99% '
Guid99( ,
.99, -
Empty99- 2
,992 3
name:: 
=:: 
dto:: 
.:: 
company:: "
?::" #
.::# $
name::$ (
??::) +
$str::, .
,::. /
phoneNumber;; 
=;; 
dto;; !
.;;! "
company;;" )
?;;) *
.;;* +
phoneNumber;;+ 6
??;;7 9
$str;;: <
,;;< =
email<< 
=<< 
dto<< 
.<< 
company<< #
?<<# $
.<<$ %
email<<% *
??<<+ -
$str<<. 0
,<<0 1
website== 
=== 
dto== 
.== 
company== %
?==% &
.==& '
website==' .
??==/ 1
$str==2 4
,==4 5
}>> 
,>> 

activities?? 
=?? 
dto?? 
.?? 

activities?? '
???' (
.??( )
Select??) /
(??/ 0
a??0 1
=>??2 4
new??5 8
Activity??9 A
{@@ 
idAA 
=AA 
aAA 
.AA 
idAA 
,AA 
	directionBB 
=BB 
aBB 
.BB 
	directionBB '
,BB' (
typeCC 
=CC 
aCC 
.CC 
typeCC 
}DD 
)DD 
.DD 
ToListDD 
(DD 
)DD 
??DD 
newDD 
(DD 
)DD  
}EE 	
;EE	 

}FF ¦
wC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Vacancy\VacancyUpdatedConsumer.cs
	namespace		 	
SearchService		
 
.		 
BLL		 
.		 
	Consumers		 %
;		% &
public 
class "
VacancyUpdatedConsumer #
:$ %
	IConsumer& /
</ 0
VacancyUpdatedEvent0 C
>C D
{ 
private 
readonly %
IVacancyElasticRepository .
_repository/ :
;: ;
public 
"
VacancyUpdatedConsumer !
(! "%
IVacancyElasticRepository" ;

repository< F
)F G
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
VacancyUpdatedEvent- @
>@ A
contextB I
)I J
{ 
VacancyDtos 
dto 
= 
context !
.! "
Message" )
.) *
Vacancy* 1
;1 2
VacancyDocument 
document  
=! "
dto# &
.& '

ToDocument' 1
(1 2
)2 3
;3 4
await 
_repository 
. 
UpdateAsync %
(% &
document& .
.. /
id/ 1
.1 2
ToString2 :
(: ;
); <
,< =
document> F
)F G
;G H
} 
} á
|C:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Vacancy\VacancyFilterDeleteConsumer.cs
	namespace 	
SearchService
 
. 
BLL 
. 
	Consumers %
;% &
public		 
class		 '
VacancyFilterDeleteConsumer		 (
:		) *
	IConsumer		+ 4
<		4 5$
VacancyFilterDeleteEvent		5 M
>		M N
{

 
private 
readonly %
IVacancyElasticRepository .
_repository/ :
;: ;
public 
'
VacancyFilterDeleteConsumer &
(& '%
IVacancyElasticRepository' @

repositoryA K
)K L
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -$
VacancyFilterDeleteEvent- E
>E F
contextG N
)N O
{ 
string 
	vacancyId 
= 
context "
." #
Message# *
.* +

vacancy_id+ 5
.5 6
ToString6 >
(> ?
)? @
;@ A
int 

activityId 
= 
context  
.  !
Message! (
.( )
activity_id) 4
;4 5
try 
{ 	
VacancyDocument 
document $
=% &
await' ,
_repository- 8
.8 9
GetByIdAsync9 E
(E F
	vacancyIdF O
)O P
;P Q
if 
( 
document 
is 
null  
)  !
{ 
document 
. 

activities #
.# $
	RemoveAll$ -
(- .
a. /
=>0 2
a3 4
.4 5
id5 7
==8 :

activityId; E
)E F
;F G
await 
_repository !
.! "
UpdateAsync" -
(- .
	vacancyId. 7
,7 8
document9 A
)A B
;B C
}   
}"" 	
catch## 
(## 
	Exception## 
ex## 
)## 
{$$ 	
throw%% 
ex%% 
;%% 
}&& 	
}'' 
}(( ‹
yC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Vacancy\VacancyFilterAddConsumer.cs
	namespace 	
SearchService
 
. 
BLL 
. 
	Consumers %
;% &
public

 
class

 $
VacancyFilterAddConsumer

 %
:

& '
	IConsumer

( 1
<

1 2!
VacancyFilterAddEvent

2 G
>

G H
{ 
private 
readonly %
IVacancyElasticRepository .
_repository/ :
;: ;
public 
$
VacancyFilterAddConsumer #
(# $%
IVacancyElasticRepository$ =

repository> H
)H I
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -!
VacancyFilterAddEvent- B
>B C
contextD K
)K L
{ 
string 
	vacancyId 
= 
context "
." #
Message# *
.* +

vacancy_id+ 5
.5 6
ToString6 >
(> ?
)? @
;@ A
List 
< "
TypeOfActivityResponse #
># $
activityResponse% 5
=6 7
context8 ?
.? @
Message@ G
.G H

activitiesH R
;R S
try 
{ 	
VacancyDocument 
document $
=% &
await' ,
_repository- 8
.8 9
GetByIdAsync9 E
(E F
	vacancyIdF O
)O P
;P Q
if 
( 
document 
is 
null  
)  !
{ 
return 
; 
} 
List   
<   
Activity   
>   
newActivities   (
=  ) *
activityResponse  + ;
.  ; <
Select  < B
(  B C
a  C D
=>  E G
new  H K
Activity  L T
{!! 
id"" 
="" 
a"" 
."" 
id"" 
,"" 
	direction## 
=## 
a## 
.## 
	direction## '
,##' (
type$$ 
=$$ 
a$$ 
.$$ 
type$$ 
,$$ 
}%% 
)%% 
.%% 
ToList%% 
(%% 
)%% 
;%% 
document'' 
.'' 

activities'' 
.''  
AddRange''  (
(''( )
newActivities'') 6
)''6 7
;''7 8
await)) 
_repository)) 
.)) 
UpdateAsync)) )
())) *
	vacancyId))* 3
,))3 4
document))5 =
)))= >
;))> ?
}** 	
catch++ 
(++ 
	Exception++ 
ex++ 
)++ 
{,, 	
throw-- 
ex-- 
;-- 
}.. 	
}// 
}00 ±
wC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Vacancy\VacancyDeletedConsumer.cs
	namespace 	
SearchService
 
. 
BLL 
. 
	Consumers %
;% &
public 
class "
VacancyDeletedConsumer #
:$ %
	IConsumer& /
</ 0
VacancyDeletedEvent0 C
>C D
{		 
private

 
readonly

 %
IVacancyElasticRepository

 .
_repository

/ :
;

: ;
public 
"
VacancyDeletedConsumer !
(! "%
IVacancyElasticRepository" ;

repository< F
)F G
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
VacancyDeletedEvent- @
>@ A
contextB I
)I J
{ 
string 
id 
= 
context 
. 
Message #
.# $
	vacancyId$ -
.- .
ToString. 6
(6 7
)7 8
;8 9
await 
_repository 
. 
DeleteAsync %
(% &
id& (
)( )
;) *
} 
} Ô
zC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Resume\ResumeFilterDeleteConsumer.cs
	namespace 	
SearchService
 
. 
BLL 
. 
	Consumers %
;% &
public

 
class

 &
ResumeFilterDeleteConsumer

 '
:

( )
	IConsumer

* 3
<

3 4#
ResumeFilterDeleteEvent

4 K
>

K L
{ 
private 
readonly $
IResumeElasticRepository -
_repository. 9
;9 :
public 
&
ResumeFilterDeleteConsumer %
(% &$
IResumeElasticRepository& >

repository? I
)I J
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -#
ResumeFilterDeleteEvent- D
>D E
contextF M
)M N
{ 
string 
resumeId 
= 
context !
.! "
Message" )
.) *
	resume_id* 3
.3 4
ToString4 <
(< =
)= >
;> ?
int 

activityId 
= 
context  
.  !
Message! (
.( )
activity_id) 4
;4 5
try 
{ 	
ResumeDocument 
document #
=$ %
await& +
_repository, 7
.7 8
GetByIdAsync8 D
(D E
resumeIdE M
)M N
;N O
if 
( 
document 
is 
null  
)  !
{ 
document 
. 

activities #
.# $
	RemoveAll$ -
(- .
a. /
=>0 2
a3 4
.4 5
id5 7
==8 :

activityId; E
)E F
;F G
await   
_repository   !
.  ! "
UpdateAsync  " -
(  - .
resumeId  . 6
,  6 7
document  8 @
)  @ A
;  A B
}!! 
}## 	
catch$$ 
($$ 
	Exception$$ 
ex$$ 
)$$ 
{%% 	
throw&& 
ex&& 
;&& 
}'' 	
}(( 
})) ¥
wC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Vacancy\VacancyCreatedConsumer.cs
	namespace		 	
SearchService		
 
.		 
BLL		 
.		 
	Consumers		 %
;		% &
public 
class "
VacancyCreatedConsumer #
:$ %
	IConsumer& /
</ 0
VacancyCreatedEvent0 C
>C D
{ 
private 
readonly %
IVacancyElasticRepository .
_repository/ :
;: ;
public 
"
VacancyCreatedConsumer !
(! "%
IVacancyElasticRepository" ;

repository< F
)F G
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
VacancyCreatedEvent- @
>@ A
contextB I
)I J
{ 
VacancyDtos 
dto 
= 
context !
.! "
Message" )
.) *
Vacancy* 1
;1 2
VacancyDocument 
document  
=! "
dto# &
.& '

ToDocument' 1
(1 2
)2 3
;3 4
await 
_repository 
. 

IndexAsync $
($ %
document% -
.- .
id. 0
.0 1
ToString1 9
(9 :
): ;
,; <
document= E
)E F
;F G
} 
} ›
uC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Resume\ResumeUpdatedConsumer.cs
	namespace

 	
SearchService


 
.

 
BLL

 
.

 
	Consumers

 %
;

% &
public 
class !
ResumeUpdatedConsumer "
:# $
	IConsumer% .
<. /
ResumeUpdatedEvent/ A
>A B
{ 
private 
readonly $
IResumeElasticRepository -
_repository. 9
;9 :
public 
!
ResumeUpdatedConsumer  
(  !$
IResumeElasticRepository! 9

repository: D
)D E
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
ResumeUpdatedEvent- ?
>? @
contextA H
)H I
{ 

ResumeDtos 
dto 
= 
context  
.  !
Message! (
.( )
Resume) /
;/ 0
ResumeDocument 
document 
=  !
dto" %
.% &

ToDocument& 0
(0 1
)1 2
;2 3
await 
_repository 
. 
UpdateAsync %
(% &
document& .
.. /
id/ 1
.1 2
ToString2 :
(: ;
); <
,< =
document> F
)F G
;G H
} 
} þ
wC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Resume\ResumeFilterAddConsumer.cs
	namespace		 	
SearchService		
 
.		 
BLL		 
.		 
	Consumers		 %
;		% &
public 
class #
ResumeFilterAddConsumer $
:% &
	IConsumer' 0
<0 1 
ResumeFilterAddEvent1 E
>E F
{ 
private 
readonly $
IResumeElasticRepository -
_repository. 9
;9 :
public 
#
ResumeFilterAddConsumer "
(" #$
IResumeElasticRepository# ;

repository< F
)F G
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, - 
ResumeFilterAddEvent- A
>A B
contextC J
)J K
{ 
string 
resumeId 
= 
context !
.! "
Message" )
.) *
	resume_id* 3
.3 4
ToString4 <
(< =
)= >
;> ?
List 
< "
TypeOfActivityResponse #
># $
activityResponse% 5
=6 7
context8 ?
.? @
Message@ G
.G H

activitiesH R
;R S
try 
{ 	
ResumeDocument 
document #
=$ %
await& +
_repository, 7
.7 8
GetByIdAsync8 D
(D E
resumeIdE M
)M N
;N O
if 
( 
document 
is 
null  
)  !
{ 
return 
; 
} 
List!! 
<!! 
Activity!! 
>!! 
newActivities!! (
=!!) *
activityResponse!!+ ;
.!!; <
Select!!< B
(!!B C
a!!C D
=>!!E G
new!!H K
Activity!!L T
{"" 
id## 
=## 
a## 
.## 
id## 
,## 
	direction$$ 
=$$ 
a$$ 
.$$ 
	direction$$ '
,$$' (
type%% 
=%% 
a%% 
.%% 
type%% 
,%% 
}&& 
)&& 
.&& 
ToList&& 
(&& 
)&& 
;&& 
document(( 
.(( 

activities(( 
.((  
AddRange((  (
(((( )
newActivities(() 6
)((6 7
;((7 8
await** 
_repository** 
.** 
UpdateAsync** )
(**) *
resumeId*** 2
,**2 3
document**4 <
)**< =
;**= >
}++ 	
catch,, 
(,, 
	Exception,, 
ex,, 
),, 
{-- 	
throw.. 
ex.. 
;.. 
}// 	
}00 
}11 ¨
uC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Resume\ResumeDeletedConsumer.cs
	namespace 	
SearchService
 
. 
BLL 
. 
	Consumers %
;% &
public		 
class		 !
ResumeDeletedConsumer		 "
:		# $
	IConsumer		% .
<		. /
ResumeDeletedEvent		/ A
>		A B
{

 
private 
readonly $
IResumeElasticRepository -
_repository. 9
;9 :
public 
!
ResumeDeletedConsumer  
(  !$
IResumeElasticRepository! 9

repository: D
)D E
{ 
_repository 
= 

repository  
;  !
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -
ResumeDeletedEvent- ?
>? @
contextA H
)H I
{ 
string 
id 
= 
context 
. 
Message #
.# $
resumeId$ ,
., -
ToString- 5
(5 6
)6 7
;7 8
await 
_repository 
. 
DeleteAsync %
(% &
id& (
)( )
;) *
} 
} š
uC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.BLL\Consumers\Resume\ResumeCreatedConsumer.cs
	namespace		 	
SearchService		
 
.		 
BLL		 
.		 
	Consumers		 %
;		% &
public 
class !
ResumeCreatedConsumer "
:# $
	IConsumer% .
<. /
ResumeCreatedEvent/ A
>A B
{ 
private 
readonly $
IResumeElasticRepository -
_repository. 9
;9 :
public 
!
ResumeCreatedConsumer  
(  !$
IResumeElasticRepository! 9

repository: D
)D E
{ 
_repository 
= 

repository  
;  !
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
{ 

ResumeDtos 
dto 
= 
context  
.  !
Message! (
.( )
Resume) /
;/ 0
ResumeDocument 
document 
=  !
dto" %
.% &

ToDocument& 0
(0 1
)1 2
;2 3
await 
_repository 
. 

IndexAsync $
($ %
document% -
.- .
id. 0
.0 1
ToString1 9
(9 :
): ;
,; <
document= E
)E F
;F G
} 
} 