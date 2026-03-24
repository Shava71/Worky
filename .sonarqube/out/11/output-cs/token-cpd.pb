¬
qC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Interfaces\IWorkerService.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Services $
.$ %

Interfaces% /
;/ 0
public 
	interface 
IWorkerService 
{ 
Task 
< 	

ResumeDtos	 
> 
GetResumeInfoAsync '
(' (
Guid( ,
resumeId- 5
)5 6
;6 7
Task 
< 	
IEnumerable	 
< 

ResumeDtos 
>  
>  !
GetMyResumesAsync" 3
(3 4
string4 :
workerId; C
,C D
GuidE I
?I J
resumeIdK S
)S T
;T U
Task 
< 	
Guid	 
> 
CreateResumeAsync  
(  !
CreateResume! -
resume. 4
,4 5
string6 <
workerId= E
)E F
;F G
Task 
UpdateResumeAsync	 
( 
UpdateResume '
resume( .
,. /
string0 6
workerId7 ?
)? @
;@ A
Task 
DeleteResumeAsync	 
( 
Guid 
id  "
," #
string$ *
workerId+ 3
)3 4
;4 5
Task 
< 	
IEnumerable	 
< 
Guid 
> 
>  
AddResumeFilterAsync 0
(0 1
	AddFilter1 :
filter; A
,A B
stringC I
workerIdJ R
)R S
;S T
Task #
DeleteResumeFilterAsync	  
(  !
Guid! %
filterId& .
,. /
string0 6
workerId7 ?
)? @
;@ A
Task 
< 	
WorkerProfileDto	 
> 
GetProfileAsync *
(* +
string+ 1
workerId2 :
,: ;
string< B
tokenC H
,H I
CancellationTokenJ [
cancellationToken\ m
=n o
defaultp w
)w x
;x y
} ª
vC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Interfaces\IFilterCacheService.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Services $
.$ %

Interfaces% /
;/ 0
public 
	interface 
IFilterCacheService $
{ 
Task 
< 	
List	 
< "
TypeOfActivityResponse $
>$ %
?% &
>& ' 
GetFiltersByIdsAsync( <
(< =
List= A
<A B
intB E
>E F
idsG J
)J K
;K L
} ÿ÷
uC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Implementations\WorkerService.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Services $
.$ %
Implementations% 4
;4 5
public 
class 
WorkerService 
: 
IWorkerService +
{ 
private 
readonly 
IResumeRepository *
_resumeRepository+ <
;< =
private 
readonly 
IWorkerRepository *
_workerRepository+ <
;< =
private 
readonly 
ILogger  
<  !
WorkerService! .
>. /
_logger0 7
;7 8
private 
readonly 
IAuthClient $
_authClient% 0
;0 1
private 
readonly 
IFilterCacheService ,
_filterCacheService- @
;@ A
private 
readonly 
ITopicProducer '
<' (
ResumeCreatedEvent( :
>: ;'
_resumeCreatedTopicProducer< W
;W X
private 
readonly 
ITopicProducer '
<' (
ResumeUpdatedEvent( :
>: ;'
_resumeUpdatedTopicProducer< W
;W X
private 
readonly 
ITopicProducer '
<' (
ResumeDeletedEvent( :
>: ;'
_resumeDeletedTopicProducer< W
;W X
private 
readonly 
ITopicProducer '
<' ( 
ResumeFilterAddEvent( <
>< =)
_resumeFilterAddTopicProducer> [
;[ \
private 
readonly 
ITopicProducer '
<' (#
ResumeFilterDeleteEvent( ?
>? @,
 _resumeFilterDeleteTopicProducerA a
;a b
public   
WorkerService   
(   
IResumeRepository!! 
resumeRepository!! .
,!!. /
IWorkerRepository"" 
workerRepository"" .
,"". /
ILogger## 
<## 
WorkerService## !
>##! "
logger### )
,##) *
IAuthClient$$ 

authClient$$ "
,$$" #
IFilterCacheService%% 
filterCacheService%%  2
,%%2 3
ITopicProducer&& 
<&& 
ResumeCreatedEvent&& -
>&&- .&
resumeCreatedTopicProducer&&/ I
,&&I J
ITopicProducer'' 
<'' 
ResumeUpdatedEvent'' -
>''- .&
resumeUpdatedTopicProducer''/ I
,''I J
ITopicProducer(( 
<(( 
ResumeDeletedEvent(( -
>((- .&
resumeDeletedTopicProducer((/ I
,((I J
ITopicProducer)) 
<))  
ResumeFilterAddEvent)) /
>))/ 0(
resumeFilterAddTopicProducer))1 M
,))M N
ITopicProducer** 
<** #
ResumeFilterDeleteEvent** 2
>**2 3+
resumeFilterDeleteTopicProducer**4 S
)**S T
{++ 	
_resumeRepository,, 
=,, 
resumeRepository,,  0
;,,0 1
_workerRepository-- 
=-- 
workerRepository--  0
;--0 1
_logger.. 
=.. 
logger.. 
;.. 
_authClient// 
=// 

authClient// $
;//$ %
_filterCacheService00 
=00  !
filterCacheService00" 4
;004 5'
_resumeCreatedTopicProducer11 '
=11( )&
resumeCreatedTopicProducer11* D
;11D E'
_resumeUpdatedTopicProducer22 '
=22( )&
resumeUpdatedTopicProducer22* D
;22D E'
_resumeDeletedTopicProducer33 '
=33( )&
resumeDeletedTopicProducer33* D
;33D E)
_resumeFilterAddTopicProducer44 )
=44* +(
resumeFilterAddTopicProducer44, H
;44H I,
 _resumeFilterDeleteTopicProducer55 ,
=55- .+
resumeFilterDeleteTopicProducer55/ N
;55N O
}66 	
publicUU 
asyncUU 
TaskUU 
<UU 

ResumeDtosUU $
>UU$ %
GetResumeInfoAsyncUU& 8
(UU8 9
GuidUU9 =
resumeIdUU> F
)UUF G
{VV 	

ResumeDtosWW 
resumeWW 
=WW 
awaitWW  % 
BuildFullResumeAsyncWW& :
(WW: ;
resumeIdWW; C
)WWC D
;WWD E
returnYY 
resumeYY 
;YY 
}ZZ 	
public\\ 
async\\ 
Task\\ 
<\\ 
IEnumerable\\ %
<\\% &

ResumeDtos\\& 0
>\\0 1
?\\1 2
>\\2 3
GetMyResumesAsync\\4 E
(\\E F
string\\F L
workerId\\M U
,\\U V
Guid\\W [
?\\[ \
resumeId\\] e
)\\e f
{]] 	
IEnumerable^^ 
<^^ 

ResumeDtos^^ "
>^^" #
resumes^^$ +
=^^, -
await^^. 3
_resumeRepository^^4 E
.^^E F
GetMyResumesAsync^^F W
(^^W X
workerId^^X `
,^^` a
resumeId^^b j
)^^j k
;^^k l
List__ 
<__ 

ResumeDtos__ 
>__ 

resumeList__ '
=__( )
resumes__* 1
.__1 2
ToList__2 8
(__8 9
)__9 :
;__: ;
ifaa 
(aa 
!aa 

resumeListaa 
.aa 
Anyaa 
(aa  
)aa  !
)aa! "
returnbb 

resumeListbb !
;bb! "
Listdd 
<dd 
intdd 
>dd 
allActivityIdsdd $
=dd% &

resumeListdd' 1
.ee 

SelectManyee 
(ee 
ree 
=>ee  
ree! "
.ee" #

activitiesee# -
.ee- .
Selectee. 4
(ee4 5
aee5 6
=>ee7 9
aee: ;
.ee; <
idee< >
)ee> ?
)ee? @
.ff 
Distinctff 
(ff 
)ff 
.gg 
ToListgg 
(gg 
)gg 
;gg 
ifii 
(ii 
allActivityIdsii 
.ii 
Anyii "
(ii" #
)ii# $
)ii$ %
{jj 
Listkk 
<kk "
TypeOfActivityResponsekk +
>kk+ ,

activitieskk- 7
=kk8 9
awaitkk: ?
_filterCacheServicekk@ S
.kkS T 
GetFiltersByIdsAsynckkT h
(kkh i
allActivityIdskki w
)kkw x
;kkx y

Dictionaryll 
<ll 
intll 
,ll "
TypeOfActivityResponsell  6
>ll6 7
activityDictll8 D
=llE F

activitiesllG Q
.llQ R
ToDictionaryllR ^
(ll^ _
all_ `
=>lla c
alld e
.lle f
idllf h
,llh i
allj k
=>lll n
allo p
)llp q
;llq r
foreachnn 
(nn 

ResumeDtosnn #
resumenn$ *
innn+ -

resumeListnn. 8
)nn8 9
{oo 
resumepp 
.pp 

activitiespp %
=pp& '
resumepp( .
.pp. /

activitiespp/ 9
.qq 
Whereqq 
(qq 
aqq  
=>qq! #
activityDictqq$ 0
.qq0 1
ContainsKeyqq1 <
(qq< =
aqq= >
.qq> ?
idqq? A
)qqA B
)qqB C
.rr 
Selectrr 
(rr  
arr  !
=>rr" $
activityDictrr% 1
[rr1 2
arr2 3
.rr3 4
idrr4 6
]rr6 7
)rr7 8
.ss 
ToListss 
(ss  
)ss  !
;ss! "
}tt 
}uu 
Workerww 
workerww 
=ww 
awaitww !
_workerRepositoryww" 3
.ww3 4
GetWorkerByIdAsyncww4 F
(wwF G
GuidwwG K
.wwK L
ParsewwL Q
(wwQ R
workerIdwwR Z
)wwZ [
)ww[ \
;ww\ ]
varxx 
todayxx 
=xx 
DateOnlyxx  
.xx  !
FromDateTimexx! -
(xx- .
DateTimexx. 6
.xx6 7
UtcNowxx7 =
)xx= >
;xx> ?
intyy 
ageyy 
=yy 
todayyy 
.yy 
Yearyy  
-yy! "
workeryy# )
.yy) *
birthdayyy* 2
.yy2 3
Yearyy3 7
;yy7 8
if{{ 
({{ 
today{{ 
<{{ 
worker{{ 
.{{ 
birthday{{ '
.{{' (
AddYears{{( 0
({{0 1
age{{1 4
){{4 5
){{5 6
age|| 
--|| 
;|| 

WorkerDtos}} 
	workerDto}}  
=}}! "
new}}# &

WorkerDtos}}' 1
{~~ 
id 
= 
worker 
. 
UserId "
." #
ToString# +
(+ ,
), -
,- .

first_name
ÄÄ 
=
ÄÄ 
worker
ÄÄ #
.
ÄÄ# $

first_name
ÄÄ$ .
,
ÄÄ. /
second_name
ÅÅ 
=
ÅÅ 
worker
ÅÅ $
.
ÅÅ$ %
second_name
ÅÅ% 0
,
ÅÅ0 1
surname
ÇÇ 
=
ÇÇ 
worker
ÇÇ  
.
ÇÇ  !
surname
ÇÇ! (
,
ÇÇ( )
birthday
ÉÉ 
=
ÉÉ 
worker
ÉÉ !
.
ÉÉ! "
birthday
ÉÉ" *
,
ÉÉ* +
phone
ÑÑ 
=
ÑÑ 
worker
ÑÑ 
.
ÑÑ 
PhoneNumber
ÑÑ *
,
ÑÑ* +
email
ÖÖ 
=
ÖÖ 
worker
ÖÖ 
.
ÖÖ 
Email
ÖÖ $
,
ÖÖ$ %
age
ÜÜ 
=
ÜÜ 
age
ÜÜ 
}
áá 
;
áá 
foreach
ââ 
(
ââ 
var
ââ 
resume
ââ 
in
ââ  "

resumeList
ââ# -
)
ââ- .
{
ää 
resume
ãã 
.
ãã 
worker
ãã 
=
ãã 
	workerDto
ãã  )
;
ãã) *
}
åå 
return
éé 

resumeList
éé 
;
éé 
}
èè 	
public
ëë 
async
ëë 
Task
ëë 
<
ëë 
Guid
ëë 
>
ëë 
CreateResumeAsync
ëë  1
(
ëë1 2
CreateResume
ëë2 >
resume
ëë? E
,
ëëE F
string
ëëG M
workerId
ëëN V
)
ëëV W
{
íí 	
Guid
ìì 
resumeId
ìì 
=
ìì 
await
ìì !
_resumeRepository
ìì" 3
.
ìì3 4
CreateResumeAsync
ìì4 E
(
ììE F
resume
ììF L
,
ììL M
workerId
ììN V
)
ììV W
;
ììW X

ResumeDtos
ïï 

fullResume
ïï !
=
ïï" #
await
ïï$ )"
BuildFullResumeAsync
ïï* >
(
ïï> ?
resumeId
ïï? G
)
ïïG H
;
ïïH I
await
óó )
_resumeCreatedTopicProducer
óó -
.
óó- .
Produce
óó. 5
(
óó5 6
new
óó6 9 
ResumeCreatedEvent
óó: L
(
óóL M

fullResume
óóM W
)
óóW X
)
óóX Y
;
óóY Z
_logger
òò 
.
òò 
LogInformation
òò "
(
òò" #
$str
òò# W
,
òòW X
resumeId
òòY a
)
òòa b
;
òòb c
return
õõ 
resumeId
õõ 
;
õõ 
}
úú 	
public
ûû 
async
ûû 
Task
ûû 
UpdateResumeAsync
ûû +
(
ûû+ ,
UpdateResume
ûû, 8
resume
ûû9 ?
,
ûû? @
string
ûûA G
workerId
ûûH P
)
ûûP Q
{
üü 	
await
†† 
_resumeRepository
†† #
.
††# $
UpdateResumeAsync
††$ 5
(
††5 6
resume
††6 <
)
††< =
;
††= >

ResumeDtos
¢¢ 

fullResume
¢¢ !
=
¢¢" #
await
¢¢$ )"
BuildFullResumeAsync
¢¢* >
(
¢¢> ?
resume
¢¢? E
.
¢¢E F
id
¢¢F H
)
¢¢H I
;
¢¢I J
await
££ )
_resumeUpdatedTopicProducer
££ -
.
££- .
Produce
££. 5
(
££5 6
new
££6 9 
ResumeUpdatedEvent
££: L
(
££L M

fullResume
££M W
)
££W X
)
££X Y
;
££Y Z
_logger
•• 
.
•• 
LogInformation
•• "
(
••" #
$str
••# W
,
••W X
resume
••Y _
.
••_ `
id
••` b
)
••b c
;
••c d
}
¶¶ 	
public
®® 
async
®® 
Task
®® 
DeleteResumeAsync
®® +
(
®®+ ,
Guid
®®, 0
id
®®1 3
,
®®3 4
string
®®5 ;
workerId
®®< D
)
®®D E
{
©© 	
await
™™ 
_resumeRepository
™™ #
.
™™# $
DeleteResumeAsync
™™$ 5
(
™™5 6
id
™™6 8
)
™™8 9
;
™™9 :
await
¨¨ )
_resumeDeletedTopicProducer
¨¨ -
.
¨¨- .
Produce
¨¨. 5
(
¨¨5 6
new
¨¨6 9 
ResumeDeletedEvent
¨¨: L
(
¨¨L M
id
¨¨M O
)
¨¨O P
)
¨¨P Q
;
¨¨Q R
_logger
≠≠ 
.
≠≠ 
LogInformation
≠≠ "
(
≠≠" #
$str
≠≠# W
,
≠≠W X
id
≠≠Y [
)
≠≠[ \
;
≠≠\ ]
}
ÆÆ 	
public
∞∞ 
async
∞∞ 
Task
∞∞ 
<
∞∞ 
IEnumerable
∞∞ %
<
∞∞% &
Guid
∞∞& *
>
∞∞* +
>
∞∞+ ,"
AddResumeFilterAsync
∞∞- A
(
∞∞A B
	AddFilter
∞∞B K
filter
∞∞L R
,
∞∞R S
string
∞∞T Z
workerId
∞∞[ c
)
∞∞c d
{
±± 	
if
≤≤ 
(
≤≤ 
!
≤≤ 
await
≤≤ 
WorkerHasResume
≤≤ &
(
≤≤& '
Guid
≤≤' +
.
≤≤+ ,
Parse
≤≤, 1
(
≤≤1 2
workerId
≤≤2 :
)
≤≤: ;
,
≤≤; <
filter
≤≤= C
.
≤≤C D
id
≤≤D F
)
≤≤F G
)
≤≤G H
{
≥≥ 
return
µµ 
[
µµ 
]
µµ 
;
µµ 
}
∂∂ 
List
∏∏ 
<
∏∏ $
TypeOfActivityResponse
∏∏ '
>
∏∏' (

activities
∏∏) 3
=
∏∏4 5
await
∏∏6 ;!
_filterCacheService
∏∏< O
.
∏∏O P"
GetFiltersByIdsAsync
∏∏P d
(
∏∏d e
filter
∏∏e k
.
∏∏k l
typeOfActivity_id
∏∏l }
)
∏∏} ~
;
∏∏~ 
try
ππ 
{
∫∫ 
await
ªª +
_resumeFilterAddTopicProducer
ªª 3
.
ªª3 4
Produce
ªª4 ;
(
ªª; <
new
ªª< ?"
ResumeFilterAddEvent
ªª@ T
(
ªªT U
filter
ªªU [
.
ªª[ \
id
ªª\ ^
,
ªª^ _

activities
ªª` j
)
ªªj k
)
ªªk l
;
ªªl m
}
ºº 
catch
ΩΩ 
(
ΩΩ 
	Exception
ΩΩ 
ex
ΩΩ 
)
ΩΩ  
{
ææ 
_logger
øø 
.
øø 
LogError
øø  
(
øø  !
ex
øø! #
,
øø# $
$str
øø% Q
,
øøQ R
workerId
øøS [
)
øø[ \
;
øø\ ]
return
¿¿ 
[
¿¿ 
]
¿¿ 
;
¿¿ 
}
¡¡ 
IEnumerable
√√ 
<
√√ 
Guid
√√ 
>
√√ 
	filter_id
√√ '
=
√√( )
await
√√* /
_resumeRepository
√√0 A
.
√√A B#
AddResumeFiltersAsync
√√B W
(
√√W X
filter
√√X ^
)
√√^ _
;
√√_ `
if
ƒƒ 
(
ƒƒ 
!
ƒƒ 
	filter_id
ƒƒ 
.
ƒƒ 
Any
ƒƒ 
(
ƒƒ 
)
ƒƒ  
)
ƒƒ  !
{
≈≈ 
return
∆∆ 
[
∆∆ 
]
∆∆ 
;
∆∆ 
}
«« 
return
…… 
	filter_id
…… 
;
…… 
}
   	
public
ÃÃ 
async
ÃÃ 
Task
ÃÃ %
DeleteResumeFilterAsync
ÃÃ 1
(
ÃÃ1 2
Guid
ÃÃ2 6
filterId
ÃÃ7 ?
,
ÃÃ? @
string
ÃÃA G
workerId
ÃÃH P
)
ÃÃP Q
{
ÕÕ 	
Resume_filter
ŒŒ 
resume_filter
ŒŒ '
=
ŒŒ( )
await
ŒŒ* /
_resumeRepository
ŒŒ0 A
.
ŒŒA B&
GetResumeFilterByIdAsync
ŒŒB Z
(
ŒŒZ [
filterId
ŒŒ[ c
)
ŒŒc d
;
ŒŒd e
if
œœ 
(
œœ 
resume_filter
œœ 
==
œœ  
null
œœ! %
)
œœ% &
{
–– 
return
—— 
;
—— 
}
““ %
ResumeFilterDeleteEvent
‘‘ #
@event
‘‘$ *
=
‘‘+ ,
new
‘‘- 0%
ResumeFilterDeleteEvent
‘‘1 H
(
‘‘H I
	resume_id
’’ 
:
’’ 
resume_filter
’’ (
.
’’( )
	resume_id
’’) 2
,
’’2 3
activity_id
÷÷ 
:
÷÷ 
resume_filter
÷÷ *
.
÷÷* +
typeOfActivity_id
÷÷+ <
)
◊◊ 
;
◊◊ 
try
ÿÿ 
{
ŸŸ 
await
⁄⁄ .
 _resumeFilterDeleteTopicProducer
⁄⁄ 6
.
⁄⁄6 7
Produce
⁄⁄7 >
(
⁄⁄> ?
@event
⁄⁄? E
)
⁄⁄E F
;
⁄⁄F G
}
‹‹ 
catch
›› 
(
›› 
	Exception
›› 
ex
›› 
)
››  
{
ﬁﬁ 
_logger
ﬂﬂ 
.
ﬂﬂ 
LogError
ﬂﬂ  
(
ﬂﬂ  !
ex
ﬂﬂ! #
,
ﬂﬂ# $
$str
ﬂﬂ% S
,
ﬂﬂS T
workerId
ﬂﬂU ]
)
ﬂﬂ] ^
;
ﬂﬂ^ _
return
‡‡ 
;
‡‡ 
}
·· 
await
‚‚ 
_resumeRepository
‚‚ #
.
‚‚# $%
DeleteResumeFilterAsync
‚‚$ ;
(
‚‚; <
filterId
‚‚< D
)
‚‚D E
;
‚‚E F
}
„„ 	
public
ÂÂ 
async
ÂÂ 
Task
ÂÂ 
<
ÂÂ 
WorkerProfileDto
ÂÂ *
>
ÂÂ* +
GetProfileAsync
ÂÂ, ;
(
ÂÂ; <
string
ÂÂ< B
workerId
ÂÂC K
,
ÂÂK L
string
ÂÂM S
token
ÂÂT Y
,
ÂÂY Z
CancellationToken
ÂÂ[ l
cancellationToken
ÂÂm ~
=ÂÂ Ä
defaultÂÂÅ à
)ÂÂà â
{
ÊÊ 	
Worker
ÁÁ 
worker
ÁÁ 
=
ÁÁ 
await
ÁÁ !
_workerRepository
ÁÁ" 3
.
ÁÁ3 4 
GetWorkerByIdAsync
ÁÁ4 F
(
ÁÁF G
Guid
ÁÁG K
.
ÁÁK L
Parse
ÁÁL Q
(
ÁÁQ R
workerId
ÁÁR Z
)
ÁÁZ [
)
ÁÁ[ \
;
ÁÁ\ ]
UserResponse
ËË 
?
ËË 
user
ËË 
=
ËË  
await
ËË! &
_authClient
ËË' 2
.
ËË2 3
GetUserByIdAsync
ËË3 C
(
ËËC D
workerId
ËËD L
,
ËËL M
token
ËËN S
,
ËËS T
cancellationToken
ËËU f
)
ËËf g
;
ËËg h
var
ÈÈ 
today
ÈÈ 
=
ÈÈ 
DateOnly
ÈÈ  
.
ÈÈ  !
FromDateTime
ÈÈ! -
(
ÈÈ- .
DateTime
ÈÈ. 6
.
ÈÈ6 7
UtcNow
ÈÈ7 =
)
ÈÈ= >
;
ÈÈ> ?
int
ÍÍ 
age
ÍÍ 
=
ÍÍ 
today
ÍÍ 
.
ÍÍ 
Year
ÍÍ  
-
ÍÍ! "
worker
ÍÍ# )
.
ÍÍ) *
birthday
ÍÍ* 2
.
ÍÍ2 3
Year
ÍÍ3 7
;
ÍÍ7 8
if
ÏÏ 
(
ÏÏ 
today
ÏÏ 
<
ÏÏ 
worker
ÏÏ 
.
ÏÏ 
birthday
ÏÏ '
.
ÏÏ' (
AddYears
ÏÏ( 0
(
ÏÏ0 1
age
ÏÏ1 4
)
ÏÏ4 5
)
ÏÏ5 6
age
ÌÌ 
--
ÌÌ 
;
ÌÌ 

WorkerDtos
ÔÔ 

workerDtos
ÔÔ !
=
ÔÔ" #
new
ÔÔ$ '

WorkerDtos
ÔÔ( 2
(
ÔÔ2 3
)
ÔÔ3 4
{
 
birthday
ÒÒ 
=
ÒÒ 
worker
ÒÒ !
.
ÒÒ! "
birthday
ÒÒ" *
,
ÒÒ* +
surname
ÚÚ 
=
ÚÚ 
worker
ÚÚ  
.
ÚÚ  !
surname
ÚÚ! (
,
ÚÚ( )

first_name
ÛÛ 
=
ÛÛ 
worker
ÛÛ #
.
ÛÛ# $

first_name
ÛÛ$ .
,
ÛÛ. /
second_name
ÙÙ 
=
ÙÙ 
worker
ÙÙ $
.
ÙÙ$ %
second_name
ÙÙ% 0
,
ÙÙ0 1
image
ıı 
=
ıı 
user
ıı 
?
ıı 
.
ıı 
image
ıı #
,
ıı# $
id
ˆˆ 
=
ˆˆ 
worker
ˆˆ 
.
ˆˆ 
UserId
ˆˆ "
.
ˆˆ" #
ToString
ˆˆ# +
(
ˆˆ+ ,
)
ˆˆ, -
,
ˆˆ- .
age
˜˜ 
=
˜˜ 
age
˜˜ 
}
¯¯ 
;
¯¯ 
return
˙˙ 
new
˙˙ 
WorkerProfileDto
˙˙ '
{
˙˙( )
worker
˙˙* 0
=
˙˙1 2

workerDtos
˙˙3 =
,
˙˙= >
UserResponse
˙˙? K
=
˙˙L M
user
˙˙N R
}
˙˙S T
;
˙˙T U
}
˚˚ 	
private
˝˝ 
async
˝˝ 
Task
˝˝ 
<
˝˝ 
bool
˝˝ 
>
˝˝  
WorkerHasResume
˝˝! 0
(
˝˝0 1
Guid
˝˝1 5
workerid
˝˝6 >
,
˝˝> ?
Guid
˝˝@ D
resumeId
˝˝E M
)
˝˝M N
{
˛˛ 	
var
ˇˇ 
myResume
ˇˇ 
=
ˇˇ 
await
ˇˇ  
_resumeRepository
ˇˇ! 2
.
ˇˇ2 3
GetMyResumesAsync
ˇˇ3 D
(
ˇˇD E
workerid
ˇˇE M
.
ˇˇM N
ToString
ˇˇN V
(
ˇˇV W
)
ˇˇW X
,
ˇˇX Y
resumeId
ˇˇZ b
)
ˇˇb c
;
ˇˇc d
if
ÄÄ 
(
ÄÄ 
myResume
ÄÄ 
.
ÄÄ 
Any
ÄÄ 
(
ÄÄ 
)
ÄÄ 
)
ÄÄ 
{
ÅÅ 
return
ÇÇ 
true
ÇÇ 
;
ÇÇ 
}
ÉÉ 
return
ÑÑ 
false
ÑÑ 
;
ÑÑ 
}
ÖÖ 	
private
áá 
async
áá 
Task
áá 
<
áá 

ResumeDtos
áá %
>
áá% &"
BuildFullResumeAsync
áá' ;
(
áá; <
Guid
áá< @
resumeId
ááA I
)
ááI J
{
àà 	

ResumeDtos
ââ 
resume
ââ 
=
ââ 
await
ââ  %
_resumeRepository
ââ& 7
.
ââ7 8 
GetResumeByIdAsync
ââ8 J
(
ââJ K
resumeId
ââK S
)
ââS T
;
ââT U
if
ää 
(
ää 
resume
ää 
==
ää 
null
ää 
)
ää 
{
ãã 
return
åå 
null
åå 
;
åå 
}
çç 
List
èè 
<
èè 
int
èè 
>
èè 
allIds
èè 
=
èè 
resume
èè %
.
èè% &

activities
èè& 0
.
èè0 1
Select
èè1 7
(
èè7 8
a
èè8 9
=>
èè: <
a
èè= >
.
èè> ?
id
èè? A
)
èèA B
.
èèB C
Distinct
èèC K
(
èèK L
)
èèL M
.
èèM N
ToList
èèN T
(
èèT U
)
èèU V
;
èèV W
if
êê 
(
êê 
allIds
êê 
.
êê 
Any
êê 
(
êê 
)
êê 
)
êê 
{
ëë 
List
ìì 
<
ìì $
TypeOfActivityResponse
ìì +
>
ìì+ ,

activities
ìì- 7
=
ìì8 9
await
ìì: ?!
_filterCacheService
ìì@ S
.
ììS T"
GetFiltersByIdsAsync
ììT h
(
ììh i
allIds
ììi o
)
ììo p
;
ììp q

Dictionary
îî 
<
îî 
int
îî 
,
îî $
TypeOfActivityResponse
îî  6
>
îî6 7
activityDict
îî8 D
=
îîE F

activities
îîG Q
.
îîQ R
ToDictionary
îîR ^
(
îî^ _
a
îî_ `
=>
îîa c
a
îîd e
.
îîe f
id
îîf h
,
îîh i
a
îîj k
=>
îîl n
a
îîo p
)
îîp q
;
îîq r
resume
ññ 
.
ññ 

activities
ññ !
=
ññ" #
resume
ññ$ *
.
ññ* +

activities
ññ+ 5
.
óó 
Where
óó 
(
óó 
a
óó 
=>
óó 
activityDict
óó  ,
.
óó, -
ContainsKey
óó- 8
(
óó8 9
a
óó9 :
.
óó: ;
id
óó; =
)
óó= >
)
óó> ?
.
òò 
Select
òò 
(
òò 
a
òò 
=>
òò  
activityDict
òò! -
[
òò- .
a
òò. /
.
òò/ 0
id
òò0 2
]
òò2 3
)
òò3 4
.
ôô 
ToList
ôô 
(
ôô 
)
ôô 
;
ôô 
}
öö 
string
úú 
workerId
úú 
=
úú 
resume
úú $
.
úú$ %
	worker_id
úú% .
!
úú. /
;
úú/ 0
Worker
ûû 
worker
ûû 
=
ûû 
await
ûû !
_workerRepository
ûû" 3
.
ûû3 4 
GetWorkerByIdAsync
ûû4 F
(
ûûF G
Guid
ûûG K
.
ûûK L
Parse
ûûL Q
(
ûûQ R
workerId
ûûR Z
)
ûûZ [
)
ûû[ \
;
ûû\ ]

WorkerDtos
üü 
	workerDto
üü  
=
üü! "
new
üü# &

WorkerDtos
üü' 1
{
†† 
id
°° 
=
°° 
worker
°° 
.
°° 
UserId
°° "
.
°°" #
ToString
°°# +
(
°°+ ,
)
°°, -
,
°°- .

first_name
¢¢ 
=
¢¢ 
worker
¢¢ #
.
¢¢# $

first_name
¢¢$ .
,
¢¢. /
second_name
££ 
=
££ 
worker
££ $
.
££$ %
second_name
££% 0
,
££0 1
surname
§§ 
=
§§ 
worker
§§  
.
§§  !
surname
§§! (
,
§§( )
birthday
•• 
=
•• 
worker
•• !
.
••! "
birthday
••" *
,
••* +
phone
¶¶ 
=
¶¶ 
worker
¶¶ 
.
¶¶ 
PhoneNumber
¶¶ *
,
¶¶* +
email
ßß 
=
ßß 
worker
ßß 
.
ßß 
Email
ßß $
,
ßß$ %
}
®® 
;
®® 
resume
™™ 
.
™™ 
worker
™™ 
=
™™ 
	workerDto
™™ %
;
™™% &
return
´´ 
resume
´´ 
;
´´ 
}
¨¨ 	
}≠≠ ˘2
zC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Implementations\FilterCacheService.cs
	namespace 	
WorkerService
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
FilterCacheService

 
:

  !
IFilterCacheService

" 5
{ 
private 
readonly 
ILogger 
< 
FilterCacheService /
>/ 0
_logger1 8
;8 9
private 
readonly 
IRedisRepository %
_redisRepository& 6
;6 7
private 
readonly 
IFilterClient "
_filterClient# 0
;0 1
private 
const 
string 
FilterKeyPrefix (
=) *
$str+ 4
;4 5
public 

FilterCacheService 
( 
ILogger %
<% &
FilterCacheService& 8
>8 9
logger: @
,@ A
IRedisRepository 
redisRepository (
,( )
IFilterClient 
filterClient "
)" #
{ 
_logger 
= 
logger 
; 
_redisRepository 
= 
redisRepository *
;* +
_filterClient 
= 
filterClient $
;$ %
} 
public 

async 
Task 
< 
List 
< "
TypeOfActivityResponse 1
>1 2
?2 3
>3 4 
GetFiltersByIdsAsync5 I
(I J
ListJ N
<N O
intO R
>R S
idsT W
)W X
{ 
if 

( 
ids 
== 
null 
|| 
ids 
. 
Count $
==% '
$num( )
)) *
{ 	
return 
null 
; 
} 	
List!! 
<!! 
string!! 
>!! 
keys!! 
=!! 
ids!! 
.!!  
Select!!  &
(!!& '
id!!' )
=>!!* ,
$"!!- /
{!!/ 0
FilterKeyPrefix!!0 ?
}!!? @
{!!@ A
id!!A C
}!!C D
"!!D E
)!!E F
.!!F G
ToList!!G M
(!!M N
)!!N O
;!!O P

Dictionary## 
<## 
string## 
,## "
TypeOfActivityResponse## 1
>##1 2
cachedFilters##3 @
=##A B
await##C H
_redisRepository##I Y
.##Y Z
GetManyAsync##Z f
<##f g"
TypeOfActivityResponse##g }
>##} ~
(##~ 
keys	## É
)
##É Ñ
;
##Ñ Ö

Dictionary%% 
<%% 
string%% 
,%% "
TypeOfActivityResponse%% 1
>%%1 2
found%%3 8
=%%9 :
cachedFilters%%; H
.&& 
Where&& 
(&& 
kv&& 
=>&& 
kv&& 
.&& 
Value&& !
!=&&" $
null&&% )
)&&) *
.&&* +
ToDictionary'' 
('' 
kv'' 
=>'' 
kv'' !
.''! "
Key''" %
,''% &
kv''' )
=>''* ,
kv''- /
.''/ 0
Value''0 5
)''5 6
;''6 7
List)) 
<)) 
int)) 
>)) 
missing)) 
=)) 
ids)) 
.** 
Where** 
(** 
id** 
=>** 
!** 
found** 
.**  
ContainsKey**  +
(**+ ,
$"**, .
{**. /
FilterKeyPrefix**/ >
}**> ?
{**? @
id**@ B
}**B C
"**C D
)**D E
)**E F
.++ 
ToList++ 
(++ 
)++ 
;++ 
if-- 

(-- 
missing-- 
.-- 
Count-- 
>-- 
$num-- 
)-- 
{.. 	
List// 
<// "
TypeOfActivityResponse// '
>//' (
acvtivityFilters//) 9
=//: ;
await//< A
_filterClient//B O
.//O P
GetFiltersByIdAsync//P c
(//c d
missing//d k
)//k l
;//l m
if11 
(11 
acvtivityFilters11  
!=11! #
null11$ (
&&11) +
acvtivityFilters11, <
.11< =
Count11= B
>11C D
$num11E F
)11F G
{22 

Dictionary33 
<33 
string33 !
,33! ""
TypeOfActivityResponse33# 9
>339 :
dict33; ?
=33@ A
acvtivityFilters33B R
.33R S
ToDictionary33S _
(33_ `
f44 
=>44 
$"44 
{44 
FilterKeyPrefix44 +
}44+ ,
{44, -
f44- .
.44. /
id44/ 1
}441 2
"442 3
,443 4
f55 
=>55 
f55 
)55 
;55 
await77 
_redisRepository77 &
.77& '
SetManyAsync77' 3
<773 4"
TypeOfActivityResponse774 J
>77J K
(77K L
dict77L P
,77P Q
TimeSpan77R Z
.77Z [
	FromHours77[ d
(77d e
$num77e f
)77f g
)77g h
;77h i
foreach99 
(99 
KeyValuePair99 %
<99% &
string99& ,
,99, -"
TypeOfActivityResponse99. D
>99D E
kv99F H
in99I K
dict99L P
)99P Q
{:: 
found;; 
.;; 
Add;; 
(;; 
$";;  
{;;  !
FilterKeyPrefix;;! 0
};;0 1
{;;1 2
kv;;2 4
.;;4 5
Key;;5 8
};;8 9
";;9 :
,;;: ;
kv;;< >
.;;> ?
Value;;? D
);;D E
;;;E F
}<< 
}== 
}>> 	
return@@ 
found@@ 
.@@ 
Values@@ 
.@@ 
ToList@@ "
(@@" #
)@@# $
;@@$ %
}AA 
}BB À
uC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Http\Interfaces\IFilterClient.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Services $
.$ %
Http% )
.) *

Interfaces* 4
;4 5
public 
	interface 
IFilterClient 
{ 
Task 
< 	
List	 
< "
TypeOfActivityResponse $
?$ %
>% &
>& '
GetFiltersByIdAsync( ;
(; <
List< @
<@ A
intA D
>D E
	filterIdsF O
,O P
CancellationToken 
cancellationToken +
=, -
default. 5
)5 6
;6 7
}		 ê
sC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Http\Interfaces\IAuthClient.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Services $
.$ %
Http% )
.) *

Interfaces* 4
;4 5
public 
	interface 
IAuthClient 
{ 
Task 
< 	
UserResponse	 
? 
> 
GetUserByIdAsync (
(( )
string) /
userId0 6
,6 7
string8 >
token? D
,D E
CancellationTokenF W
cancellationTokenX i
=j k
defaultl s
)s t
;t u
} ∫!
yC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Http\Implementations\FilterClient.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Services $
.$ %
Http% )
.) *
Implementations* 9
;9 :
public		 
class		 
FilterClient		 
:		 
IFilterClient		 )
{

 
private 
readonly 
ILogger 
< 
FilterClient )
>) *
_logger+ 2
;2 3
private 
readonly 

HttpClient 
_httpClient  +
;+ ,
public 

FilterClient 
( 

HttpClient "
client# )
,) *
ILogger+ 2
<2 3
FilterClient3 ?
>? @
loggerA G
)G H
{ 
_httpClient 
= 
client 
; 
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
List 
< "
TypeOfActivityResponse 1
?1 2
>2 3
>3 4
GetFiltersByIdAsync5 H
(H I
ListI M
<M N
intN Q
>Q R
	filterIdsS \
,\ ]
CancellationToken 
cancellationToken +
=, -
default. 5
)5 6
{ 
try 
{ 	
string 
url 
= 
$str 0
;0 1
if 
( 
	filterIds 
. 
Count 
!=  "
$num# $
)$ %
{ 
IEnumerable 
< 
KeyValuePair (
<( )
string) /
,/ 0
string0 6
>6 7
>7 8
queryParams9 D
=E F
	filterIdsG P
.P Q
SelectQ W
(W X
idX Z
=>[ ]
new^ a
KeyValuePairb n
<n o
stringo u
,u v
stringw }
>} ~
(~ 
$str	 ä
,
ä ã
id
å é
.
é è
ToString
è ó
(
ó ò
)
ò ô
)
ô ö
)
ö õ
;
õ ú
url 
= 
QueryHelpers "
." #
AddQueryString# 1
(1 2
url2 5
,5 6
queryParams7 B
)B C
;C D
} 
HttpResponseMessage   
response    (
=  * +
await  , 1
_httpClient  2 =
.  = >
GetAsync  > F
(  F G
url  G J
,  J K
cancellationToken  L ]
)  ] ^
;  ^ _
if"" 
("" 
!"" 
response"" 
."" 
IsSuccessStatusCode"" -
)""- .
{## 
_logger$$ 
.$$ 
LogError$$  
($$  !
$"$$! #
$str$$# F
{$$F G
response$$G O
.$$O P

StatusCode$$P Z
}$$Z [
"$$[ \
)$$\ ]
;$$] ^
return%% 
null%% 
;%% 
}&& 
return'' 
await'' 
response'' !
.''! "
Content''" )
.'') *
ReadFromJsonAsync''* ;
<''; <
List''< @
<''@ A"
TypeOfActivityResponse''A W
>''W X
>''X Y
(''Y Z
cancellationToken''Z k
:''k l
cancellationToken''m ~
)''~ 
;	'' Ä
}(( 	
catch)) 
()) 
	Exception)) 
ex)) 
))) 
{** 	
_logger++ 
.++ 
LogError++ 
(++ 
ex++ 
,++  
$"++! #
$str++# ?
{++? @
ex++@ B
.++B C
Message++C J
}++J K
"++K L
)++L M
;++M N
return,, 
null,, 
;,, 
}-- 	
}.. 
}// §
wC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Services\Http\Implementations\AuthClient.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Services $
.$ %
Http% )
.) *
Implementations* 9
;9 :
public		 
class		 

AuthClient		 
:		 
IAuthClient		 %
{

 
private 
readonly 
ILogger 
< 

AuthClient '
>' (
_logger) 0
;0 1
private 
readonly 

HttpClient 
_httpClient  +
;+ ,
public 


AuthClient 
( 
ILogger 
< 

AuthClient (
>( )
logger* 0
,0 1

HttpClient2 <

httpClient= G
)G H
{ 
_logger 
= 
logger 
; 
_httpClient 
= 

httpClient  
;  !
} 
public 

async 
Task 
< 
UserResponse "
?" #
># $
GetUserByIdAsync% 5
(5 6
string6 <
userId= C
,C D
stringE K
tokenL Q
,Q R
CancellationToken 
cancellationToken +
=, -
default. 5
)5 6
{ 
try 
{ 	
_httpClient 
. !
DefaultRequestHeaders -
.- .
Authorization. ;
=< =
new> A%
AuthenticationHeaderValueB [
([ \
$str\ d
,d e
tokenf k
)k l
;l m
HttpResponseMessage 
response  (
=) *
await+ 0
_httpClient1 <
.< =
GetAsync= E
(E F
$"F H
$strH `
{` a
userIda g
}g h
"h i
,i j
cancellationTokenk |
)| }
;} ~
if 
( 
! 
response 
. 
IsSuccessStatusCode -
)- .
{ 
_logger 
. 
LogError  
(  !
$"! #
$str# L
{L M
responseM U
.U V

StatusCodeV `
}` a
"a b
)b c
;c d
return   
null   
;   
}!! 
return"" 
await"" 
response"" !
.""! "
Content""" )
."") *
ReadFromJsonAsync""* ;
<""; <
UserResponse""< H
>""H I
(""I J
cancellationToken""J [
:""[ \
cancellationToken""] n
)""n o
;""o p
}## 	
catch$$ 
($$ 
	Exception$$ 
ex$$ 
)$$ 
{%% 	
_logger&& 
.&& 
LogError&& 
(&& 
ex&& 
,&&  
$str&&! N
,&&N O
userId&&P V
)&&V W
;&&W X
return'' 
null'' 
;'' 
}(( 	
})) 
}** ˜
qC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Events\UserWorkerCreateFailedEvent.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Events "
;" #
public 
class '
UserWorkerCreateFailedEvent (
{ 
[ 
Required 
] 
public 

string 
UserId 
{ 
get 
; 
set  #
;# $
}% &
=' (
null) -
!- .
;. /
[		 
Required		 
]		 
public

 

string

 
Reason

 
{

 
get

 
;

 
set

  #
;

# $
}

% &
=

' (
null

) -
!

- .
;

. /
} »
lC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Events\UserWorkerCreatedEvent.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Events "
;" #
public 
class "
UserWorkerCreatedEvent #
{ 
[ 
Required 
] 
public 

string 
UserId 
{ 
get 
; 
set  #
;# $
}% &
[		 
Required		 
]		 
public

 

string

 
second_name

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
=

, -
null

. 2
!

2 3
;

3 4
[ 
Required 
] 
public 

string 

first_name 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
null- 1
!1 2
;2 3
[ 
Required 
] 
public 

string 
surname 
{ 
get 
;  
set! $
;$ %
}& '
=( )
null* .
!. /
;/ 0
[ 
Required 
] 
public 

DateOnly 
birthday 
{ 
get "
;" #
set$ '
;' (
}) *
[ 
Required 
] 
public 

string 

email_info 
{ 
get "
;" #
set$ '
;' (
}) *
[ 
Required 
] 
public 

string 

phone_info 
{ 
get "
;" #
set$ '
;' (
}) *
} Ï
hC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Events\ResumeUpdatedEvent.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Events "
;" #
public 
record 
ResumeUpdatedEvent  
(  !

ResumeDtos! +
Resume, 2
)2 3
:4 5
ResumeCreatedEvent6 H
(H I
ResumeI O
)O P
;P QÃ
mC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Events\ResumeFilterDeleteEvent.cs
	namespace 	
CompanyService
 
. 
DAL 
. 
Events #
;# $
public 
record #
ResumeFilterDeleteEvent %
(% &
Guid& *
	resume_id+ 4
,4 5
int6 9
activity_id: E
)E F
;F Gà
jC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Events\ResumeFilterAddEvent.cs
	namespace 	
CompanyService
 
. 
DAL 
. 
Events #
;# $
public 
record  
ResumeFilterAddEvent "
(" #
Guid# '
	resume_id( 1
,1 2
List3 7
<7 8"
TypeOfActivityResponse8 N
>N O

activitiesP Z
)Z [
;[ \á
hC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Events\ResumeDeletedEvent.cs
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
;/ 0ã
hC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Events\ResumeCreatedEvent.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
Events "
;" #
public 
record 
ResumeCreatedEvent  
(  !

ResumeDtos! +
Resume, 2
)2 3
;3 4œ&
rC:\Users\demde\Desktop\Worky\Worky\Services\WorkerService\WorkerService.BLL\Consumers\UserWorkerCreatedConsumer.cs
	namespace 	
WorkerService
 
. 
BLL 
. 
	Consumers %
;% &
public		 
class		 %
UserWorkerCreatedConsumer		 &
:		' (
	IConsumer		) 2
<		2 3"
UserWorkerCreatedEvent		3 I
>		I J
{

 
private 
readonly 
ILogger 
< %
UserWorkerCreatedConsumer 6
>6 7
_logger8 ?
;? @
private 
readonly 
IWorkerRepository &
_workerRepository' 8
;8 9
private 
readonly 
ITopicProducer #
<# $'
UserWorkerCreateFailedEvent$ ?
>? @
_publishEndpointA Q
;Q R
public 
%
UserWorkerCreatedConsumer $
($ %
ILogger 
< %
UserWorkerCreatedConsumer )
>) *
logger+ 1
,1 2
IWorkerRepository 
workerRepository *
,* +
ITopicProducer 
< '
UserWorkerCreateFailedEvent 2
>2 3
publishEndpoint4 C
) 	
{ 
_logger 
= 
logger 
; 
_workerRepository 
= 
workerRepository ,
;, -
_publishEndpoint 
= 
publishEndpoint *
;* +
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -"
UserWorkerCreatedEvent- C
>C D
contextE L
)L M
{ "
UserWorkerCreatedEvent 
message &
=' (
context) 0
.0 1
Message1 8
;8 9
try 
{ 	
Worker   
?   
worker   
=   
await   "
_workerRepository  # 4
.  4 5
GetWorkerByIdAsync  5 G
(  G H
Guid  H L
.  L M
Parse  M R
(  R S
message  S Z
.  Z [
UserId  [ a
)  a b
)  b c
;  c d
if!! 
(!! 
worker!! 
!=!! 
null!! 
)!! 
{"" 
_logger## 
.## 
LogInformation## &
(##& '
$"##' )
$str##) 8
{##8 9
message##9 @
.##@ A
UserId##A G
}##G H
$str##H Z
"##Z [
)##[ \
;##\ ]
return$$ 
;$$ 
}%% 
Worker'' 
	newWorker'' 
='' 
new'' "
Worker''# )
('') *
)''* +
{(( 
UserId)) 
=)) 
Guid)) 
.)) 
Parse)) #
())# $
message))$ +
.))+ ,
UserId)), 2
)))2 3
,))3 4
birthday** 
=** 
message** "
.**" #
birthday**# +
,**+ ,

first_name++ 
=++ 
message++ $
.++$ %

first_name++% /
,++/ 0
surname,, 
=,, 
message,, !
.,,! "
surname,," )
,,,) *
second_name-- 
=-- 
message-- %
.--% &
second_name--& 1
,--1 2
Email// 
=// 
message// 
.//  

email_info//  *
,//* +
PhoneNumber00 
=00 
message00 %
.00% &

phone_info00& 0
}11 
;11 
await33 
_workerRepository33 #
.33# $
CreateWorkerAsync33$ 5
(335 6
	newWorker336 ?
)33? @
;33@ A
_logger44 
.44 
LogInformation44 "
(44" #
$str44# J
,44J K
message44L S
.44S T
UserId44T Z
)44Z [
;44[ \
}55 	
catch66 
(66 
	Exception66 
ex66 
)66 
{77 	
_logger88 
.88 
LogError88 
(88 
ex88 
,88  
ex88! #
.88# $
Message88$ +
)88+ ,
;88, -
await99 
_publishEndpoint99 "
.99" #
Produce99# *
(99* +
new99+ .'
UserWorkerCreateFailedEvent99/ J
{:: 
UserId;; 
=;; 
message;;  
.;;  !
UserId;;! '
,;;' (
Reason<< 
=<< 
ex<< 
.<< 
Message<< #
}== 
)== 
;== 
}>> 	
}?? 
}@@ 