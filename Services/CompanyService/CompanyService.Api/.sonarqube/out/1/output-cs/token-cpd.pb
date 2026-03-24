Ύ
xC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Interfaces\IFilterCacheService.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &

Interfaces& 0
;0 1
public 
	interface 
IFilterCacheService $
{ 
Task 
< 	
List	 
< "
TypeOfActivityResponse $
>$ %
?% &
>& ' 
GetFiltersByIdsAsync( <
(< =
List= A
<A B
intB E
>E F
idsG J
)J K
;K L
}		 β

qC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Interfaces\IDealService.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &

Interfaces& 0
;0 1
public 
	interface 
IDealService 
{ 
public 

Task 
< 
Guid 
> 

CreateDeal  
(  !
MakeDealRequest! 0
request1 8
,8 9
Guid: >

company_id? I
,I J
CancellationTokenK \
cancellationToken] n
=o p
defaultq x
)x y
;y z
public		 

Task		 
<		 
Tarrif		 
>		 
	GetTariff		 !
(		! "
int		" %
?		% &
dealId		' -
,		- .
CancellationToken		/ @
cancellationToken		A R
=		S T
default		U \
)		\ ]
;		] ^
public

 

Task

 
<

 
List

 
<

 
Tarrif

 
>

 
>

 
	GetTariff

 '
(

' (
CancellationToken

( 9
cancellationToken

: K
=

L M
default

N U
)

U V
;

V W
} γ
tC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Interfaces\ICompnayService.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &

Interfaces& 0
;0 1
public 
	interface 
ICompnayService  
{ 
Task		 
<		 	
VacancyDtos			 
>		 
GetVacancyInfoAsync		 )
(		) *
Guid		* .
	vacancyId		/ 8
)		8 9
;		9 :
Task

 
<

 	
IEnumerable

	 
<

 
VacancyDtos

  
>

  !
>

! "
GetMyVacanciesAsync

# 6
(

6 7
Guid

7 ;
	companyId

< E
,

E F
Guid

G K
?

K L
	vacancyId

M V
)

V W
;

W X
Task 
< 	
Guid	 
> 
CreateVacancyAsync !
(! "
CreateVacancy" /
vacancy0 7
,7 8
string9 ?
	companyId@ I
)I J
;J K
Task 
UpdateVacancyAsync	 
( 
UpdateVacancy )
vacancy* 1
,1 2
string3 9
	companyId: C
)C D
;D E
Task 
DeleteVacancyAsync	 
( 
Guid  
id! #
,# $
string% +
	companyId, 5
)5 6
;6 7
Task 
< 	
IEnumerable	 
< 
Guid 
> 
> !
AddVacancyFilterAsync 1
(1 2
	AddFilter2 ;
filter< B
,B C
stringD J
	companyIdK T
)T U
;U V
Task $
DeleteVacancyFilterAsync	 !
(! "
Guid" &
filterId' /
,/ 0
string1 7
	companyId8 A
)A B
;B C
Task 
< 	
byte	 
[ 
] 
> 
GetFlyerAsync 
( 
Guid #
	vacancyId$ -
,- .
string/ 5
url6 9
)9 :
;: ;
Task 
< 	
CompanyProfileDtos	 
> 
GetProfileAsync ,
(, -
string- 3
	companyId4 =
,= >
string? E
tokenF K
,K L
CancellationTokenM ^
cancellationToken_ p
=q r
defaults z
)z {
;{ |
} ό2
|C:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Implementations\FilterCacheService.cs
	namespace		 	
CompanyService		
 
.		 
BLL		 
.		 
Services		 %
.		% &
Implementations		& 5
;		5 6
public 
class 
FilterCacheService 
:  !
IFilterCacheService" 5
{ 
private 
readonly 
ILogger 
< 
FilterCacheService /
>/ 0
_logger1 8
;8 9
private 
readonly 
IRedisRepository %
_redisRepository& 6
;6 7
private 
readonly 
IFilterClient "
_filterClient# 0
;0 1
private 
const 
string 
FilterKeyPrefix (
=) *
$str+ 4
;4 5
public 

FilterCacheService 
( 
ILogger %
<% &
FilterCacheService& 8
>8 9
logger: @
,@ A
IRedisRepository 
redisRepository (
,( )
IFilterClient 
filterClient "
)" #
{ 
_logger 
= 
logger 
; 
_redisRepository 
= 
redisRepository *
;* +
_filterClient 
= 
filterClient $
;$ %
} 
public 

async 
Task 
< 
List 
< "
TypeOfActivityResponse 1
>1 2
?2 3
>3 4 
GetFiltersByIdsAsync5 I
(I J
ListJ N
<N O
intO R
>R S
idsT W
)W X
{ 
if 

( 
ids 
== 
null 
|| 
ids 
. 
Count $
==% '
$num( )
)) *
{ 	
return 
null 
; 
}   	
List"" 
<"" 
string"" 
>"" 
keys"" 
="" 
ids"" 
.""  
Select""  &
(""& '
id""' )
=>""* ,
$"""- /
{""/ 0
FilterKeyPrefix""0 ?
}""? @
{""@ A
id""A C
}""C D
"""D E
)""E F
.""F G
ToList""G M
(""M N
)""N O
;""O P

Dictionary$$ 
<$$ 
string$$ 
,$$ "
TypeOfActivityResponse$$ 1
>$$1 2
cachedFilters$$3 @
=$$A B
await$$C H
_redisRepository$$I Y
.$$Y Z
GetManyAsync$$Z f
<$$f g"
TypeOfActivityResponse$$g }
>$$} ~
($$~ 
keys	$$ ƒ
)
$$ƒ „
;
$$„ …

Dictionary&& 
<&& 
string&& 
,&& "
TypeOfActivityResponse&& 1
>&&1 2
found&&3 8
=&&9 :
cachedFilters&&; H
.'' 
Where'' 
('' 
kv'' 
=>'' 
kv'' 
.'' 
Value'' !
!=''" $
null''% )
)'') *
.''* +
ToDictionary(( 
((( 
kv(( 
=>(( 
kv(( !
.((! "
Key((" %
,((% &
kv((' )
=>((* ,
kv((- /
.((/ 0
Value((0 5
)((5 6
;((6 7
List** 
<** 
int** 
>** 
missing** 
=** 
ids** 
.++ 
Where++ 
(++ 
id++ 
=>++ 
!++ 
found++ 
.++  
ContainsKey++  +
(+++ ,
$"++, .
{++. /
FilterKeyPrefix++/ >
}++> ?
{++? @
id++@ B
}++B C
"++C D
)++D E
)++E F
.,, 
ToList,, 
(,, 
),, 
;,, 
if.. 

(.. 
missing.. 
... 
Count.. 
>.. 
$num.. 
).. 
{// 	
List00 
<00 "
TypeOfActivityResponse00 '
>00' (
acvtivityFilters00) 9
=00: ;
await00< A
_filterClient00B O
.00O P
GetFiltersByIdAsync00P c
(00c d
missing00d k
)00k l
;00l m
if22 
(22 
acvtivityFilters22  
!=22! #
null22$ (
&&22) +
acvtivityFilters22, <
.22< =
Count22= B
>22C D
$num22E F
)22F G
{33 

Dictionary44 
<44 
string44 !
,44! ""
TypeOfActivityResponse44# 9
>449 :
dict44; ?
=44@ A
acvtivityFilters44B R
.44R S
ToDictionary44S _
(44_ `
f55 
=>55 
$"55 
{55 
FilterKeyPrefix55 +
}55+ ,
{55, -
f55- .
.55. /
id55/ 1
}551 2
"552 3
,553 4
f66 
=>66 
f66 
)66 
;66 
await88 
_redisRepository88 &
.88& '
SetManyAsync88' 3
<883 4"
TypeOfActivityResponse884 J
>88J K
(88K L
dict88L P
,88P Q
TimeSpan88R Z
.88Z [
	FromHours88[ d
(88d e
$num88e f
)88f g
)88g h
;88h i
foreach:: 
(:: 
KeyValuePair:: %
<::% &
string::& ,
,::, -"
TypeOfActivityResponse::. D
>::D E
kv::F H
in::I K
dict::L P
)::P Q
{;; 
found<< 
.<< 
Add<< 
(<< 
$"<<  
{<<  !
FilterKeyPrefix<<! 0
}<<0 1
{<<1 2
kv<<2 4
.<<4 5
Key<<5 8
}<<8 9
"<<9 :
,<<: ;
kv<<< >
.<<> ?
Value<<? D
)<<D E
;<<E F
}== 
}>> 
}?? 	
returnAA 
foundAA 
.AA 
ValuesAA 
.AA 
ToListAA "
(AA" #
)AA# $
;AA$ %
}BB 
}CC ­/
uC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Implementations\DealService.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &
Implementations& 5
;5 6
public		 
class		 
DealService		 
:		 
IDealService		 '
{

 
private 
readonly 
IDealRepository $
_dealRepository% 4
;4 5
private 
readonly 
ITariffRepository &
_tariffRepository' 8
;8 9
private 
readonly 
ILogger 
< 
DealService (
>( )
_logger* 1
;1 2
public 

DealService 
( 
IDealRepository &
dealRepository' 5
,5 6
ITariffRepository7 H
tariffRepositoryI Y
,Y Z
ILogger[ b
<b c
DealServicec n
>n o
loggerp v
)v w
{ 
_dealRepository 
= 
dealRepository (
;( )
_tariffRepository 
= 
tariffRepository ,
;, -
_logger 
= 
logger 
; 
} 
public 

async 
Task 
< 
Guid 
> 

CreateDeal &
(& '
MakeDealRequest' 6
request7 >
,> ?
Guid@ D

company_idE O
,O P
CancellationTokenQ b
cancellationTokenc t
=u v
defaultw ~
)~ 
{ 
Tarrif 
tariff 
= 
await 
_tariffRepository /
./ 0
	GetTariff0 9
(9 :
request: A
.A B
	tarrif_idB K
,K L
cancellationTokenM ^
)^ _
;_ `
if 

( 
tariff 
== 
null 
) 
{ 	
_logger 
. 
LogError 
( 
$str =
,= >
request? F
.F G
	tarrif_idG P
)P Q
;Q R
throw 
new  
ApplicationException *
(* +
$str+ =
)= >
;> ?
} 	
DateTime 
dateTime 
= 
DateTime $
.$ %
UtcNow% +
.+ ,
Date, 0
;0 1
DateOnly   
currentDate   
=   
DateOnly   '
.  ' (
FromDateTime  ( 4
(  4 5
dateTime  5 =
)  = >
;  > ?
Deal"" 
?"" 
currentDeal"" 
="" 
await"" !
_dealRepository""" 1
.""1 2"
CurrentActiveDealAsync""2 H
(""H I
currentDate""I T
,""T U

company_id""V `
,""` a
cancellationToken""b s
)""s t
;""t u
if## 

(## 
currentDeal## 
is## 
not## 
null## #
)### $
{$$ 	
_logger%% 
.%% 
LogError%% 
(%% 
$str%% E
,%%E F
currentDeal%%G R
.%%R S
id%%S U
,%%U V
currentDeal%%W b
.%%b c
	tariff_id%%c l
)%%l m
;%%m n
throw&& 
new&&  
ApplicationException&& *
(&&* +
$str&&+ 9
)&&9 :
;&&: ;
}'' 	
Deal)) 
newDeal)) 
=)) 
new)) 
Deal)) 
())  
)))  !
{** 	
id++ 
=++ 
Guid++ 
.++ 
NewGuid++ 
(++ 
)++ 
,++  
	tariff_id,, 
=,, 
tariff,, 
.,, 
id,, !
,,,! "

company_id-- 
=-- 

company_id-- #
,--# $

date_start.. 
=.. 
currentDate.. $
,..$ %
date_end// 
=// 
currentDate// "
.//" #
	AddMonths//# ,
(//, -
request//- 4
.//4 5

countMonth//5 ?
)//? @
,//@ A
sum00 
=00 
tariff00 
.00 
price00 
*00  
request00! (
.00( )

countMonth00) 3
}11 	
;11	 

return33 
await33 
_dealRepository33 $
.33$ %

CreateDeal33% /
(33/ 0
newDeal330 7
,337 8
cancellationToken339 J
)33J K
;33K L
}44 
public77 

async77 
Task77 
<77 
Tarrif77 
?77 
>77 
	GetTariff77 (
(77( )
int77) ,
?77, -
dealId77. 4
,774 5
CancellationToken776 G
cancellationToken77H Y
=77Z [
default77\ c
)77c d
{88 
return99 
await99 
_tariffRepository99 &
.99& '
	GetTariff99' 0
(990 1
dealId991 7
,997 8
cancellationToken999 J
)99J K
;99K L
}:: 
public<< 

async<< 
Task<< 
<<< 
List<< 
<<< 
Tarrif<< !
><<! "
><<" #
	GetTariff<<$ -
(<<- .
CancellationToken<<. ?
cancellationToken<<@ Q
=<<R S
default<<T [
)<<[ \
{== 
return>> 
await>> 
_tariffRepository>> &
.>>& '
	GetTariff>>' 0
(>>0 1
cancellationToken>>1 B
)>>B C
;>>C D
}?? 
}@@ τέ
xC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Implementations\CompanyService.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &
Implementations& 5
;5 6
public 
class 
CompanyService 
: 
ICompnayService -
{ 
private 
readonly 
IVacancyRepository +
_vacancyRepository, >
;> ?
private 
readonly 
ICompanyRepository +
_companyRepository, >
;> ?
private 
readonly 
IDealRepository (
_dealRepository) 8
;8 9
private 
readonly 
ILogger  
<  !
CompanyService! /
>/ 0
_logger1 8
;8 9
private 
readonly 
IAuthClient $
_authClient% 0
;0 1
private 
readonly 
IFilterCacheService ,
_filterCacheService- @
;@ A
private!! 
readonly!! 
ITopicProducer!! '
<!!' (
VacancyCreatedEvent!!( ;
>!!; <(
_vacancyCreatedTopicProducer!!= Y
;!!Y Z
private"" 
readonly"" 
ITopicProducer"" '
<""' (
VacancyUpdatedEvent""( ;
>""; <(
_vacancyUpdatedTopicProducer""= Y
;""Y Z
private## 
readonly## 
ITopicProducer## '
<##' (
VacancyDeletedEvent##( ;
>##; <(
_vacancyDeletedTopicProducer##= Y
;##Y Z
private%% 
readonly%% 
ITopicProducer%% '
<%%' (!
VacancyFilterAddEvent%%( =
>%%= >*
_vacancyFilterAddTopicProducer%%? ]
;%%] ^
private&& 
readonly&& 
ITopicProducer&& '
<&&' ($
VacancyFilterDeleteEvent&&( @
>&&@ A-
!_vacancyFilterDeleteTopicProducer&&B c
;&&c d
public(( 
CompanyService(( 
((( 
IVacancyRepository)) 
vacancyRepository)) 0
,))0 1
ICompanyRepository** 
companyRepository** 0
,**0 1
ILogger++ 
<++ 
CompanyService++ "
>++" #
logger++$ *
,++* +
IAuthClient,, 

authClient,, "
,,," #
IFilterCacheService-- 
filterCacheService--  2
,--2 3
IDealRepository.. 
dealRepository.. *
,..* +
ITopicProducer00 
<00 
VacancyCreatedEvent00 .
>00. /'
vacancyCreatedTopicProducer000 K
,00K L
ITopicProducer11 
<11 
VacancyUpdatedEvent11 .
>11. /'
vacancyUpdatedTopicProducer110 K
,11K L
ITopicProducer22 
<22 
VacancyDeletedEvent22 .
>22. /'
vacancyDeletedTopicProducer220 K
,22K L
ITopicProducer44 
<44 !
VacancyFilterAddEvent44 0
>440 1)
vacancyFilterAddTopicProducer442 O
,44O P
ITopicProducer55 
<55 $
VacancyFilterDeleteEvent55 3
>553 4,
 vacancyFilterDeleteTopicProducer555 U
)55U V
{66 	
_vacancyRepository77 
=77  
vacancyRepository77! 2
;772 3
_companyRepository88 
=88  
companyRepository88! 2
;882 3
_logger99 
=99 
logger99 
;99 
_authClient:: 
=:: 

authClient:: $
;::$ %
_filterCacheService;; 
=;;  !
filterCacheService;;" 4
;;;4 5
_dealRepository<< 
=<< 
dealRepository<< ,
;<<, -(
_vacancyCreatedTopicProducer>> (
=>>) *'
vacancyCreatedTopicProducer>>+ F
;>>F G(
_vacancyUpdatedTopicProducer?? (
=??) *'
vacancyUpdatedTopicProducer??+ F
;??F G(
_vacancyDeletedTopicProducer@@ (
=@@) *'
vacancyDeletedTopicProducer@@+ F
;@@F G*
_vacancyFilterAddTopicProducerBB *
=BB+ ,)
vacancyFilterAddTopicProducerBB- J
;BBJ K-
!_vacancyFilterDeleteTopicProducerCC -
=CC. /,
 vacancyFilterDeleteTopicProducerCC0 P
;CCP Q
}DD 	
publicFF 
asyncFF 
TaskFF 
<FF 
VacancyDtosFF %
>FF% &
GetVacancyInfoAsyncFF' :
(FF: ;
GuidFF; ?
	vacancyIdFF@ I
)FFI J
{GG 	
returnHH 
awaitHH !
BuildFullVacancyAsyncHH .
(HH. /
	vacancyIdHH/ 8
)HH8 9
;HH9 :
}II 	
publicKK 
asyncKK 
TaskKK 
<KK 
IEnumerableKK %
<KK% &
VacancyDtosKK& 1
>KK1 2
>KK2 3
GetMyVacanciesAsyncKK4 G
(KKG H
GuidKKH L
	companyIdKKM V
,KKV W
GuidKKX \
?KK\ ]
	vacancyIdKK^ g
)KKg h
{LL 	
IEnumerableMM 
<MM 
VacancyDtosMM #
>MM# $
	vacanciesMM% .
=MM/ 0
awaitMM1 6
_vacancyRepositoryMM7 I
.MMI J
GetMyVacanciesAsyncMMJ ]
(MM] ^
	companyIdMM^ g
.MMg h
ToStringMMh p
(MMp q
)MMq r
,MMr s
	vacancyIdMMt }
)MM} ~
;MM~ 
ListNN 
<NN 
VacancyDtosNN 
>NN 
vacancyListNN )
=NN* +
	vacanciesNN, 5
.NN5 6
ToListNN6 <
(NN< =
)NN= >
;NN> ?
ifPP 
(PP 
!PP 
vacancyListPP 
.PP 
AnyPP  
(PP  !
)PP! "
)PP" #
returnQQ 
vacancyListQQ "
;QQ" #
ListSS 
<SS 
intSS 
>SS 
allActivityIdsSS $
=SS% &
vacancyListSS' 2
.TT 

SelectManyTT 
(TT 
vTT 
=>TT  
vTT! "
.TT" #

activitiesTT# -
.TT- .
SelectTT. 4
(TT4 5
aTT5 6
=>TT7 9
aTT: ;
.TT; <
idTT< >
)TT> ?
)TT? @
.UU 
DistinctUU 
(UU 
)UU 
.VV 
ToListVV 
(VV 
)VV 
;VV 
ifXX 
(XX 
allActivityIdsXX 
.XX 
AnyXX "
(XX" #
)XX# $
)XX$ %
{YY 
ListZZ 
<ZZ "
TypeOfActivityResponseZZ +
>ZZ+ ,

activitiesZZ- 7
=ZZ8 9
awaitZZ: ?
_filterCacheServiceZZ@ S
.ZZS T 
GetFiltersByIdsAsyncZZT h
(ZZh i
allActivityIdsZZi w
)ZZw x
;ZZx y

Dictionary\\ 
<\\ 
int\\ 
,\\  "
TypeOfActivityResponse\\! 7
>\\7 8
activityDict\\9 E
=\\F G

activities\\H R
.\\R S
ToDictionary\\S _
(\\_ `
a\\` a
=>\\b d
a\\e f
.\\f g
id\\g i
,\\i j
a\\k l
=>\\m o
a\\p q
)\\q r
;\\r s
foreachee 
(ee 
VacancyDtosee $
vacancyee% ,
inee- /
vacancyListee0 ;
)ee; <
{ff 
vacancygg 
.gg 

activitiesgg &
=gg' (
vacancygg) 0
.gg0 1

activitiesgg1 ;
.hh 
Wherehh 
(hh 
ahh  
=>hh! #
activityDicthh$ 0
.hh0 1
ContainsKeyhh1 <
(hh< =
ahh= >
.hh> ?
idhh? A
)hhA B
)hhB C
.ii 
Selectii 
(ii  
aii  !
=>ii" $
{jj 
varkk 
cachedkk  &
=kk' (
activityDictkk) 5
[kk5 6
akk6 7
.kk7 8
idkk8 :
]kk: ;
;kk; <
returnmm "
newmm# &"
TypeOfActivityResponsemm' =
{nn 
idoo  "
=oo# $
cachedoo% +
.oo+ ,
idoo, .
,oo. /
	directionpp  )
=pp* +
cachedpp, 2
.pp2 3
	directionpp3 <
,pp< =
typeqq  $
=qq% &
cachedqq' -
.qq- .
typeqq. 2
,qq2 3
	filter_idss  )
=ss* +
ass, -
.ss- .
	filter_idss. 7
}tt 
;tt 
}uu 
)uu 
.vv 
ToListvv 
(vv  
)vv  !
;vv! "
}ww 
}xx 
Companyzz 
companyzz 
=zz 
awaitzz #
_companyRepositoryzz$ 6
.zz6 7
GetCompanyByIdAsynczz7 J
(zzJ K
	companyIdzzK T
)zzT U
;zzU V

CompanyDto{{ 

companyDto{{ !
={{" #
new{{$ '

CompanyDto{{( 2
{|| 
id}} 
=}} 
company}} 
.}} 
UserId}} #
,}}# $
email~~ 
=~~ 
company~~ 
.~~  
email~~  %
,~~% &
	longitude 
= 
company #
.# $
	longitude$ -
,- .
latitude
€€ 
=
€€ 
company
€€ "
.
€€" #
latitude
€€# +
,
€€+ ,
name
 
=
 
company
 
.
 
name
 #
,
# $
phoneNumber
‚‚ 
=
‚‚ 
company
‚‚ %
.
‚‚% &
phoneNumber
‚‚& 1
,
‚‚1 2
website
ƒƒ 
=
ƒƒ 
company
ƒƒ !
.
ƒƒ! "
website
ƒƒ" )
,
ƒƒ) *
}
„„ 
;
„„ 
foreach
†† 
(
†† 
VacancyDtos
††  
vacancy
††! (
in
††) +
vacancyList
††, 7
)
††7 8
{
‡‡ 
vacancy
 
.
 
company
 
=
  !

companyDto
" ,
;
, -
}
‰‰ 
return
‹‹ 
vacancyList
‹‹ 
;
‹‹ 
}
 	
public
 
async
 
Task
 
<
 
Guid
 
>
  
CreateVacancyAsync
  2
(
2 3
CreateVacancy
3 @
vacancy
A H
,
H I
string
J P
	companyId
Q Z
)
Z [
{
 	
int
‘‘ #
currentVacanciesCount
‘‘ %
=
‘‘& '
await
‘‘( - 
_vacancyRepository
‘‘. @
.
‘‘@ A&
GetMyVacanciesCountAsync
‘‘A Y
(
‘‘Y Z
Guid
‘‘Z ^
.
‘‘^ _
Parse
‘‘_ d
(
‘‘d e
	companyId
‘‘e n
)
‘‘n o
)
‘‘o p
;
‘‘p q
DateTime
““ 
dateTime
““ 
=
““ 
DateTime
““  (
.
““( )
UtcNow
““) /
.
““/ 0
Date
““0 4
;
““4 5
DateOnly
”” 
currentDate
””  
=
””! "
DateOnly
””# +
.
””+ ,
FromDateTime
””, 8
(
””8 9
dateTime
””9 A
)
””A B
;
””B C
Deal
•• 
?
•• 
currentDeal
•• 
=
•• 
await
••  %
_dealRepository
••& 5
.
••5 6$
CurrentActiveDealAsync
••6 L
(
••L M
currentDate
••M X
,
••X Y
Guid
••Z ^
.
••^ _
Parse
••_ d
(
••d e
	companyId
••e n
)
••n o
)
••o p
;
••p q
if
—— 
(
—— 
currentDeal
—— 
is
—— 
null
—— #
)
——# $
{
 
throw
™™ 
new
™™ 
	Exception
™™ #
(
™™# $
$str
™™$ 4
)
™™4 5
;
™™5 6
}
 
if
 
(
 #
currentVacanciesCount
 %
>=
& (
currentDeal
) 4
.
4 5
tariff
5 ;
.
; <
vacancy_count
< I
)
I J
{
 
throw
 
new
 
	Exception
 #
(
# $
$str
$ <
)
< =
;
= >
}
 
Guid
΅΅ 
	vacancyId
΅΅ 
=
΅΅ 
await
΅΅ " 
_vacancyRepository
΅΅# 5
.
΅΅5 6 
CreateVacancyAsync
΅΅6 H
(
΅΅H I
vacancy
΅΅I P
,
΅΅P Q
	companyId
΅΅R [
)
΅΅[ \
;
΅΅\ ]
VacancyDtos
££ 
fullVacancy
££ #
=
££$ %
await
££& +#
BuildFullVacancyAsync
££, A
(
££A B
	vacancyId
££B K
)
££K L
;
££L M
await
¥¥ *
_vacancyCreatedTopicProducer
¥¥ .
.
¥¥. /
Produce
¥¥/ 6
(
¥¥6 7
new
¥¥7 :!
VacancyCreatedEvent
¥¥; N
(
¥¥N O
fullVacancy
¥¥O Z
)
¥¥Z [
)
¥¥[ \
;
¥¥\ ]
_logger
¦¦ 
.
¦¦ 
LogInformation
¦¦ "
(
¦¦" #
$str
¦¦# Z
,
¦¦Z [
	vacancyId
¦¦\ e
)
¦¦e f
;
¦¦f g
return
¨¨ 
	vacancyId
¨¨ 
;
¨¨ 
}
©© 	
public
«« 
async
«« 
Task
««  
UpdateVacancyAsync
«« ,
(
««, -
UpdateVacancy
««- :
vacancy
««; B
,
««B C
string
««D J
	companyId
««K T
)
««T U
{
¬¬ 	
try
­­ 
{
®® 
if
―― 
(
―― 
!
―― 
await
―― 
CompanyHasVacancy
―― ,
(
――, -
Guid
――- 1
.
――1 2
Parse
――2 7
(
――7 8
	companyId
――8 A
)
――A B
,
――B C
vacancy
――D K
.
――K L
Id
――L N
)
――N O
)
――O P
{
°° 
return
²² 
;
²² 
}
³³ 
await
µµ  
_vacancyRepository
µµ (
.
µµ( ) 
UpdateVacancyAsync
µµ) ;
(
µµ; <
vacancy
µµ< C
,
µµC D
Guid
µµE I
.
µµI J
Parse
µµJ O
(
µµO P
	companyId
µµP Y
)
µµY Z
)
µµZ [
;
µµ[ \
VacancyDtos
·· 

fullResume
·· &
=
··' (
await
··) .#
BuildFullVacancyAsync
··/ D
(
··D E
vacancy
··E L
.
··L M
Id
··M O
)
··O P
;
··P Q
await
ΈΈ *
_vacancyUpdatedTopicProducer
ΈΈ 2
.
ΈΈ2 3
Produce
ΈΈ3 :
(
ΈΈ: ;
new
ΈΈ; >!
VacancyUpdatedEvent
ΈΈ? R
(
ΈΈR S

fullResume
ΈΈS ]
)
ΈΈ] ^
)
ΈΈ^ _
;
ΈΈ_ `
_logger
ΊΊ 
.
ΊΊ 
LogInformation
ΊΊ &
(
ΊΊ& '
$str
ΊΊ' ^
,
ΊΊ^ _
vacancy
ΊΊ` g
.
ΊΊg h
Id
ΊΊh j
)
ΊΊj k
;
ΊΊk l
}
»» 
catch
ΌΌ 
(
ΌΌ 
	Exception
ΌΌ 
ex
ΌΌ 
)
ΌΌ  
{
½½ 
_logger
ΎΎ 
.
ΎΎ 
LogError
ΎΎ  
(
ΎΎ  !
ex
ΎΎ! #
,
ΎΎ# $
$str
ΎΎ% M
,
ΎΎM N
	companyId
ΎΎO X
)
ΎΎX Y
;
ΎΎY Z
throw
ΏΏ 
;
ΏΏ 
}
ΐΐ 
}
ΑΑ 	
public
ΓΓ 
async
ΓΓ 
Task
ΓΓ  
DeleteVacancyAsync
ΓΓ ,
(
ΓΓ, -
Guid
ΓΓ- 1
id
ΓΓ2 4
,
ΓΓ4 5
string
ΓΓ6 <
	companyId
ΓΓ= F
)
ΓΓF G
{
ΔΔ 	
if
ΕΕ 
(
ΕΕ 
!
ΕΕ 
await
ΕΕ 
CompanyHasVacancy
ΕΕ (
(
ΕΕ( )
Guid
ΕΕ) -
.
ΕΕ- .
Parse
ΕΕ. 3
(
ΕΕ3 4
	companyId
ΕΕ4 =
)
ΕΕ= >
,
ΕΕ> ?
id
ΕΕ@ B
)
ΕΕB C
)
ΕΕC D
{
ΖΖ 
return
ΘΘ 
;
ΘΘ 
}
ΙΙ 
await
ΚΚ  
_vacancyRepository
ΚΚ $
.
ΚΚ$ % 
DeleteVacancyAsync
ΚΚ% 7
(
ΚΚ7 8
id
ΚΚ8 :
,
ΚΚ: ;
Guid
ΚΚ< @
.
ΚΚ@ A
Parse
ΚΚA F
(
ΚΚF G
	companyId
ΚΚG P
)
ΚΚP Q
)
ΚΚQ R
;
ΚΚR S
await
ΜΜ *
_vacancyDeletedTopicProducer
ΜΜ .
.
ΜΜ. /
Produce
ΜΜ/ 6
(
ΜΜ6 7
new
ΜΜ7 :!
VacancyDeletedEvent
ΜΜ; N
(
ΜΜN O
id
ΜΜO Q
)
ΜΜQ R
)
ΜΜR S
;
ΜΜS T
_logger
ΝΝ 
.
ΝΝ 
LogInformation
ΝΝ "
(
ΝΝ" #
$str
ΝΝ# Z
,
ΝΝZ [
id
ΝΝ\ ^
)
ΝΝ^ _
;
ΝΝ_ `
}
ΞΞ 	
public
ΠΠ 
async
ΠΠ 
Task
ΠΠ 
<
ΠΠ 
IEnumerable
ΠΠ %
<
ΠΠ% &
Guid
ΠΠ& *
>
ΠΠ* +
>
ΠΠ+ ,#
AddVacancyFilterAsync
ΠΠ- B
(
ΠΠB C
	AddFilter
ΠΠC L
filter
ΠΠM S
,
ΠΠS T
string
ΠΠU [
	companyId
ΠΠ\ e
)
ΠΠe f
{
ΡΡ 	
if
ÒÒ 
(
ÒÒ 
!
ÒÒ 
await
ÒÒ 
CompanyHasVacancy
ÒÒ (
(
ÒÒ( )
Guid
ÒÒ) -
.
ÒÒ- .
Parse
ÒÒ. 3
(
ÒÒ3 4
	companyId
ÒÒ4 =
)
ÒÒ= >
,
ÒÒ> ?
filter
ÒÒ@ F
.
ÒÒF G
id
ÒÒG I
)
ÒÒI J
)
ÒÒJ K
{
ΣΣ 
return
ΥΥ 
[
ΥΥ 
]
ΥΥ 
;
ΥΥ 
}
ΦΦ 
List
ΨΨ 
<
ΨΨ $
TypeOfActivityResponse
ΨΨ '
>
ΨΨ' (

activities
ΨΨ) 3
=
ΨΨ4 5
await
ΨΨ6 ;!
_filterCacheService
ΨΨ< O
.
ΨΨO P"
GetFiltersByIdsAsync
ΨΨP d
(
ΨΨd e
filter
ΨΨe k
.
ΨΨk l
typeOfActivity_id
ΨΨl }
)
ΨΨ} ~
;
ΨΨ~ 
try
ΩΩ 
{
ΪΪ 
await
ΫΫ ,
_vacancyFilterAddTopicProducer
ΫΫ 4
.
ΫΫ4 5
Produce
ΫΫ5 <
(
ΫΫ< =
new
ΫΫ= @#
VacancyFilterAddEvent
ΫΫA V
(
ΫΫV W
filter
ΫΫW ]
.
ΫΫ] ^
id
ΫΫ^ `
,
ΫΫ` a

activities
ΫΫb l
)
ΫΫl m
)
ΫΫm n
;
ΫΫn o
IEnumerable
άά 
<
άά 
Guid
άά  
>
άά  !
	filter_id
άά" +
=
άά, -
await
άά. 3 
_vacancyRepository
άά4 F
.
άάF G$
AddVacancyFiltersAsync
άάG ]
(
άά] ^
filter
άά^ d
)
άάd e
;
άάe f
if
ήή 
(
ήή 
!
ήή 
	filter_id
ήή 
.
ήή 
Any
ήή "
(
ήή" #
)
ήή# $
)
ήή$ %
{
ίί 
throw
ΰΰ 
new
ΰΰ "
KeyNotFoundException
ΰΰ 2
(
ΰΰ2 3
$str
ΰΰ3 E
)
ΰΰE F
;
ΰΰF G
}
αα 
return
γγ 
	filter_id
γγ  
;
γγ  !
}
εε 
catch
ζζ 
(
ζζ 
	Exception
ζζ 
ex
ζζ 
)
ζζ  
{
ηη 
_logger
θθ 
.
θθ 
LogError
θθ  
(
θθ  !
ex
θθ! #
,
θθ# $
$str
θθ% R
,
θθR S
	companyId
θθT ]
)
θθ] ^
;
θθ^ _
return
ιι 
[
ιι 
]
ιι 
;
ιι 
}
κκ 
}
λλ 	
public
νν 
async
νν 
Task
νν &
DeleteVacancyFilterAsync
νν 2
(
νν2 3
Guid
νν3 7
filterId
νν8 @
,
νν@ A
string
ννB H
	companyId
ννI R
)
ννR S
{
ξξ 	
Vacancy_filter
οο 
?
οο 
vacancy_filter
οο *
=
οο+ ,
await
οο- 2 
_vacancyRepository
οο3 E
.
οοE F'
GetVacancyFilterByIdAsync
οοF _
(
οο_ `
filterId
οο` h
)
οοh i
;
οοi j
if
ππ 
(
ππ 
vacancy_filter
ππ 
==
ππ !
null
ππ" &
)
ππ& '
{
ρρ 
throw
ςς 
new
ςς "
KeyNotFoundException
ςς .
(
ςς. /
$str
ςς/ A
)
ςςA B
;
ςςB C
}
σσ &
VacancyFilterDeleteEvent
υυ $
@event
υυ% +
=
υυ, -
new
υυ. 1&
VacancyFilterDeleteEvent
υυ2 J
(
υυJ K

vacancy_id
φφ 
:
φφ 
vacancy_filter
φφ *
.
φφ* +

vacancy_id
φφ+ 5
,
φφ5 6
activity_id
χχ 
:
χχ 
vacancy_filter
χχ +
.
χχ+ ,
typeOfActivity_id
χχ, =
)
ψψ 
;
ψψ 
try
ωω 
{
ϊϊ 
await
ϋϋ /
!_vacancyFilterDeleteTopicProducer
ϋϋ 7
.
ϋϋ7 8
Produce
ϋϋ8 ?
(
ϋϋ? @
@event
ϋϋ@ F
)
ϋϋF G
;
ϋϋG H
await
όό  
_vacancyRepository
όό (
.
όό( )&
DeleteVacancyFilterAsync
όό) A
(
όόA B
filterId
όόB J
)
όόJ K
;
όόK L
}
ύύ 
catch
ώώ 
(
ώώ 
	Exception
ώώ 
ex
ώώ 
)
ώώ  
{
ÿÿ 
_logger
€€ 
.
€€ 
LogError
€€  
(
€€  !
ex
€€! #
,
€€# $
$str
€€% T
,
€€T U
	companyId
€€V _
)
€€_ `
;
€€` a
return
 
;
 
}
‚‚ 
}
„„ 	
public
‹‹ 
async
‹‹ 
Task
‹‹ 
<
‹‹ 
byte
‹‹ 
[
‹‹ 
]
‹‹  
>
‹‹  !
GetFlyerAsync
‹‹" /
(
‹‹/ 0
Guid
‹‹0 4
	vacancyId
‹‹5 >
,
‹‹> ?
string
‹‹@ F
url
‹‹G J
)
‹‹J K
{
 	
var
 
vacancy
 
=
 
await
  
_vacancyRepository
  2
.
2 3!
GetVacancyByIdAsync
3 F
(
F G
	vacancyId
G P
)
P Q
;
Q R
byte
 
[
 
]
 
flyer
 
=
 
Document
 #
.
# $
Create
$ *
(
* +
	container
+ 4
=>
5 7
{
 
	container
 
.
 
Page
 
(
 
page
 #
=>
$ &
{
‘‘ 
page
’’ 
.
’’ 
Size
’’ !
(
’’! "
	PageSizes
’’" +
.
’’+ ,
A4
’’, .
)
’’. /
;
’’/ 0
page
““ 
.
““ 
Margin
““ #
(
““# $
$num
““$ %
,
““% &
Unit
““' +
.
““+ ,

Centimetre
““, 6
)
““6 7
;
““7 8
page
”” 
.
”” 
DefaultTextStyle
”” -
(
””- .
x
””. /
=>
””0 2
x
””3 4
.
””4 5
FontSize
””5 =
(
””= >
$num
””> @
)
””@ A
.
””A B

FontFamily
””B L
(
””L M
$str
””M U
)
””U V
)
””V W
;
””W X
page
–– 
.
–– 
Header
–– #
(
––# $
)
––$ %
.
—— 
Text
—— !
(
——! "
$"
  "
$str
" ?
{
? @
vacancy
@ G
.
G H
post
H L
}
L M
$str
M ^
{
^ _
vacancy
_ f
.
f g
company
g n
.
n o
name
o s
}
s t
$str
t v
"
v w
)
w x
.
™™ 
AlignCenter
™™ (
(
™™( )
)
™™) *
.
™™* +
FontSize
™™+ 3
(
™™3 4
$num
™™4 6
)
™™6 7
.
™™7 8
Bold
™™8 <
(
™™< =
)
™™= >
;
™™> ?
page
›› 
.
›› 
Content
›› $
(
››$ %
)
››% &
.
››& '
PaddingVertical
››' 6
(
››6 7
$num
››7 8
)
››8 9
.
››9 :
Column
››: @
(
››@ A

descriptor
››A K
=>
››L N
{
 

descriptor
 &
.
& '
Item
' +
(
+ ,
)
, -
.
- .
Text
. 2
(
2 3
$str
3 J
)
J K
.
K L
SemiBold
L T
(
T U
)
U V
.
V W
FontSize
W _
(
_ `
$num
` b
)
b c
;
c d

descriptor
 &
.
& '
Item
' +
(
+ ,
)
, -
.
- .
Text
. 2
(
2 3
$"
3 5
$str
5 ?
{
? @
vacancy
@ G
.
G H
company
H O
.
O P
name
P T
}
T U
"
U V
)
V W
;
W X

descriptor
 &
.
& '
Item
' +
(
+ ,
)
, -
.
- .
Text
. 2
(
2 3
$"
3 5
$str
5 <
{
< =
vacancy
= D
.
D E
company
E L
.
L M
email
M R
??
S U
$str
V Y
}
Y Z
"
Z [
)
[ \
;
\ ]

descriptor
   &
.
  & '
Item
  ' +
(
  + ,
)
  , -
.
  - .
Text
  . 2
(
  2 3
$"
  3 5
$str
  5 >
{
  > ?
vacancy
  ? F
.
  F G
company
  G N
.
  N O
phoneNumber
  O Z
??
  [ ]
$str
  ^ a
}
  a b
"
  b c
)
  c d
;
  d e

descriptor
΅΅ &
.
΅΅& '
Item
΅΅' +
(
΅΅+ ,
)
΅΅, -
.
΅΅- .
Text
΅΅. 2
(
΅΅2 3
$"
΅΅3 5
$str
΅΅5 ;
{
΅΅; <
vacancy
΅΅< C
.
΅΅C D
company
΅΅D K
.
΅΅K L
website
΅΅L S
??
΅΅T V
$str
΅΅W Z
}
΅΅Z [
"
΅΅[ \
)
΅΅\ ]
;
΅΅] ^

descriptor
ΆΆ &
.
ΆΆ& '
Item
ΆΆ' +
(
ΆΆ+ ,
)
ΆΆ, -
.
££  !
Text
££! %
(
££% &
$"
¤¤$ &
$str
¤¤& 3
{
¤¤3 4
vacancy
¤¤4 ;
.
¤¤; <
company
¤¤< C
.
¤¤C D
latitude
¤¤D L
}
¤¤L M
$str
¤¤M O
{
¤¤O P
vacancy
¤¤P W
.
¤¤W X
company
¤¤X _
.
¤¤_ `
	longitude
¤¤` i
}
¤¤i j
"
¤¤j k
)
¤¤k l
;
¤¤l m

descriptor
¦¦ &
.
¦¦& '
Item
¦¦' +
(
¦¦+ ,
)
¦¦, -
.
¦¦- .

PaddingTop
¦¦. 8
(
¦¦8 9
$num
¦¦9 ;
)
¦¦; <
.
¦¦< =
LineHorizontal
¦¦= K
(
¦¦K L
$num
¦¦L M
)
¦¦M N
;
¦¦N O

descriptor
§§ &
.
§§& '
Item
§§' +
(
§§+ ,
)
§§, -
.
§§- .
Text
§§. 2
(
§§2 3
$str
§§3 D
)
§§D E
.
§§E F
SemiBold
§§F N
(
§§N O
)
§§O P
.
§§P Q
FontSize
§§Q Y
(
§§Y Z
$num
§§Z \
)
§§\ ]
;
§§] ^

descriptor
¨¨ &
.
¨¨& '
Item
¨¨' +
(
¨¨+ ,
)
¨¨, -
.
¨¨- .
Text
¨¨. 2
(
¨¨2 3
$"
¨¨3 5
$str
¨¨5 @
{
¨¨@ A
vacancy
¨¨A H
.
¨¨H I
post
¨¨I M
}
¨¨M N
"
¨¨N O
)
¨¨O P
;
¨¨P Q

descriptor
©© &
.
©©& '
Item
©©' +
(
©©+ ,
)
©©, -
.
©©- .
Text
©©. 2
(
©©2 3
$"
©©3 5
$str
©©5 ?
{
©©? @
vacancy
©©@ G
.
©©G H
description
©©H S
}
©©S T
"
©©T U
)
©©U V
;
©©V W

descriptor
ªª &
.
ªª& '
Item
ªª' +
(
ªª+ ,
)
ªª, -
.
ªª- .
Text
ªª. 2
(
ªª2 3
$"
ªª3 5
$str
ªª5 K
{
ªªK L
vacancy
ªªL S
.
ªªS T

min_salary
ªªT ^
}
ªª^ _
$str
ªª_ a
"
ªªa b
)
ªªb c
;
ªªc d

descriptor
«« &
.
««& '
Item
««' +
(
««+ ,
)
««, -
.
¬¬  !
Text
¬¬! %
(
¬¬% &
$"
­­$ &
$str
­­& =
{
­­= >
vacancy
­­> E
.
­­E F

max_salary
­­F P
?
­­P Q
.
­­Q R
ToString
­­R Z
(
­­Z [
)
­­[ \
??
­­] _
$str
­­` l
}
­­l m
$str
­­m o
"
­­o p
)
­­p q
;
­­q r

descriptor
®® &
.
®®& '
Item
®®' +
(
®®+ ,
)
®®, -
.
®®- .
Text
®®. 2
(
®®2 3
$"
®®3 5
$str
®®5 B
{
®®B C
vacancy
®®C J
.
®®J K

experience
®®K U
}
®®U V
$str
®®V Z
"
®®Z [
)
®®[ \
;
®®\ ]

descriptor
°° &
.
°°& '
Item
°°' +
(
°°+ ,
)
°°, -
.
°°- .

PaddingTop
°°. 8
(
°°8 9
$num
°°9 ;
)
°°; <
.
°°< =
LineHorizontal
°°= K
(
°°K L
$num
°°L M
)
°°M N
;
°°N O

descriptor
±± &
.
±±& '
Item
±±' +
(
±±+ ,
)
±±, -
.
±±- .
Text
±±. 2
(
±±2 3
$str
±±3 L
)
±±L M
.
±±M N
SemiBold
±±N V
(
±±V W
)
±±W X
.
±±X Y
FontSize
±±Y a
(
±±a b
$num
±±b d
)
±±d e
;
±±e f
if
³³ 
(
³³  
vacancy
³³  '
.
³³' (

activities
³³( 2
!=
³³3 5
null
³³6 :
&&
³³; =
vacancy
³³> E
.
³³E F

activities
³³F P
.
³³P Q
Count
³³Q V
>
³³W X
$num
³³Y Z
)
³³Z [
{
΄΄ 
foreach
µµ  '
(
µµ( )
var
µµ) ,
activity
µµ- 5
in
µµ6 8
vacancy
µµ9 @
.
µµ@ A

activities
µµA K
)
µµK L
{
¶¶  !

descriptor
··$ .
.
··. /
Item
··/ 3
(
··3 4
)
··4 5
.
··5 6
Text
··6 :
(
··: ;
$"
··; =
$str
··= ?
{
··? @
activity
··@ H
.
··H I
	direction
··I R
}
··R S
$str
··S U
{
··U V
activity
··V ^
.
··^ _
type
··_ c
}
··c d
$str
··d e
"
··e f
)
··f g
;
··g h
}
ΈΈ  !
}
ΉΉ 

descriptor
½½ &
.
½½& '
Item
½½' +
(
½½+ ,
)
½½, -
.
½½- .
Row
½½. 1
(
½½1 2
row
½½2 5
=>
½½6 8
{
ΎΎ 
row
ΏΏ  #
.
ΏΏ# $
ConstantItem
ΏΏ$ 0
(
ΏΏ0 1
$num
ΏΏ1 2
,
ΏΏ2 3
Unit
ΏΏ4 8
.
ΏΏ8 9

Centimetre
ΏΏ9 C
)
ΏΏC D
.
ΐΐ$ %
AspectRatio
ΐΐ% 0
(
ΐΐ0 1
$num
ΐΐ1 2
)
ΐΐ2 3
.
ΑΑ$ %

Background
ΑΑ% /
(
ΑΑ/ 0
Colors
ΑΑ0 6
.
ΑΑ6 7
White
ΑΑ7 <
)
ΑΑ< =
.
ΒΒ$ %
Svg
ΒΒ% (
(
ΒΒ( )
size
ΒΒ) -
=>
ΒΒ. 0
{
ΓΓ$ %
var
ΔΔ( +
writer
ΔΔ, 2
=
ΔΔ3 4
new
ΔΔ5 8
QRCodeWriter
ΔΔ9 E
(
ΔΔE F
)
ΔΔF G
;
ΔΔG H
var
ΕΕ( +
qrCode
ΕΕ, 2
=
ΕΕ3 4
writer
ΕΕ5 ;
.
ΕΕ; <
encode
ΕΕ< B
(
ΕΕB C
url
ΕΕC F
,
ΕΕF G
BarcodeFormat
ΕΕH U
.
ΕΕU V
QR_CODE
ΕΕV ]
,
ΕΕ] ^
(
ΕΕ_ `
int
ΕΕ` c
)
ΕΕc d
size
ΕΕd h
.
ΕΕh i
Width
ΕΕi n
,
ΕΕn o
(
ΖΖ, -
int
ΖΖ- 0
)
ΖΖ0 1
size
ΖΖ1 5
.
ΖΖ5 6
Height
ΖΖ6 <
)
ΖΖ< =
;
ΖΖ= >
var
ΗΗ( +
renderer
ΗΗ, 4
=
ΗΗ5 6
new
ΗΗ7 :
SvgRenderer
ΗΗ; F
{
ΗΗG H
FontName
ΗΗI Q
=
ΗΗR S
$str
ΗΗT Z
}
ΗΗ[ \
;
ΗΗ\ ]
return
ΘΘ( .
renderer
ΘΘ/ 7
.
ΘΘ7 8
Render
ΘΘ8 >
(
ΘΘ> ?
qrCode
ΘΘ? E
,
ΘΘE F
BarcodeFormat
ΘΘG T
.
ΘΘT U
EAN_13
ΘΘU [
,
ΘΘ[ \
null
ΘΘ] a
)
ΘΘa b
.
ΘΘb c
Content
ΘΘc j
;
ΘΘj k
}
ΙΙ$ %
)
ΙΙ% &
;
ΙΙ& '
}
ΚΚ 
)
ΚΚ 
;
ΚΚ 
}
ΡΡ 
)
ΡΡ 
;
ΡΡ 
page
ΣΣ 
.
ΣΣ 
Footer
ΣΣ #
(
ΣΣ# $
)
ΣΣ$ %
.
ΤΤ 
AlignCenter
ΤΤ (
(
ΤΤ( )
)
ΤΤ) *
.
ΥΥ 
Text
ΥΥ !
(
ΥΥ! "
$str
ΥΥ" 7
)
ΥΥ7 8
.
ΦΦ 
Italic
ΦΦ #
(
ΦΦ# $
)
ΦΦ$ %
.
ΧΧ 
FontSize
ΧΧ %
(
ΧΧ% &
$num
ΧΧ& (
)
ΧΧ( )
;
ΧΧ) *
}
ΨΨ 
)
ΨΨ 
;
ΨΨ 
}
ΩΩ 
)
ΩΩ 
.
ΩΩ 
GeneratePdf
ΩΩ 
(
ΩΩ 
)
ΩΩ 
;
ΩΩ 
return
ΪΪ 
flyer
ΪΪ 
;
ΪΪ 
}
ΫΫ 	
public
έέ 
async
έέ 
Task
έέ 
<
έέ  
CompanyProfileDtos
έέ ,
>
έέ, -
GetProfileAsync
έέ. =
(
έέ= >
string
έέ> D
	companyId
έέE N
,
έέN O
string
έέP V
token
έέW \
,
έέ\ ]
CancellationToken
έέ^ o 
cancellationTokenέέp 
=έέ‚ ƒ
defaultέέ„ ‹
)έέ‹ 
{
ήή 	
Company
ίί 
company
ίί 
=
ίί 
await
ίί # 
_companyRepository
ίί$ 6
.
ίί6 7!
GetCompanyByIdAsync
ίί7 J
(
ίίJ K
Guid
ίίK O
.
ίίO P
Parse
ίίP U
(
ίίU V
	companyId
ίίV _
)
ίί_ `
)
ίί` a
;
ίίa b
List
αα 
<
αα 
Deal
αα 
?
αα 
>
αα 
deals
αα 
=
αα 
await
αα  %
_dealRepository
αα& 5
.
αα5 6!
GetDealsByCompanyId
αα6 I
(
ααI J
Guid
ααJ N
.
ααN O
Parse
ααO T
(
ααT U
	companyId
ααU ^
)
αα^ _
,
αα_ `
cancellationToken
ααa r
)
ααr s
;
ααs t

CompanyDto
γγ 

companyDto
γγ !
=
γγ" #
new
γγ$ '

CompanyDto
γγ( 2
(
γγ2 3
)
γγ3 4
{
δδ 
id
εε 
=
εε 
company
εε 
.
εε 
UserId
εε #
,
εε# $
name
ζζ 
=
ζζ 
company
ζζ 
.
ζζ 
name
ζζ #
,
ζζ# $
email
ηη 
=
ηη 
company
ηη 
.
ηη  
email
ηη  %
,
ηη% &
phoneNumber
θθ 
=
θθ 
company
θθ %
.
θθ% &
phoneNumber
θθ& 1
,
θθ1 2
website
ιι 
=
ιι 
company
ιι !
.
ιι! "
website
ιι" )
,
ιι) *
latitude
κκ 
=
κκ 
company
κκ "
.
κκ" #
latitude
κκ# +
,
κκ+ ,
	longitude
λλ 
=
λλ 
company
λλ #
.
λλ# $
	longitude
λλ$ -
,
λλ- .
}
μμ 
;
μμ 
List
ξξ 
<
ξξ 
DealDto
ξξ 
>
ξξ 
dealDtos
ξξ "
=
ξξ# $
deals
ξξ% *
.
ξξ* +
Select
ξξ+ 1
(
ξξ1 2
d
ξξ2 3
=>
ξξ4 6
new
ξξ7 :
DealDto
ξξ; B
(
ξξB C
)
ξξC D
{
οο 
id
ππ 
=
ππ 
d
ππ 
.
ππ 
id
ππ 
,
ππ 
	tariff_id
ρρ 
=
ρρ 
d
ρρ 
.
ρρ 
	tariff_id
ρρ '
,
ρρ' (
tariff
ςς 
=
ςς 
d
ςς 
.
ςς 
tariff
ςς !
,
ςς! "

company_id
σσ 
=
σσ 
d
σσ 
.
σσ 

company_id
σσ )
.
σσ) *
ToString
σσ* 2
(
σσ2 3
)
σσ3 4
,
σσ4 5

date_start
ττ 
=
ττ 
d
ττ 
.
ττ 

date_start
ττ )
,
ττ) *
date_end
υυ 
=
υυ 
d
υυ 
.
υυ 
date_end
υυ %
,
υυ% &
sum
φφ 
=
φφ 
d
φφ 
.
φφ 
tariff
φφ 
.
φφ 
price
φφ $
*
φφ% &
(
φφ' (
(
φφ( )
d
φφ) *
.
φφ* +
date_end
φφ+ 3
.
φφ3 4
Year
φφ4 8
-
φφ9 :
d
φφ; <
.
φφ< =

date_start
φφ= G
.
φφG H
Year
φφH L
)
φφL M
*
φφN O
$num
φφP R
+
φφS T
(
φφU V
d
φφV W
.
φφW X
date_end
φφX `
.
φφ` a
Month
φφa f
-
φφg h
d
φφi j
.
φφj k

date_start
φφk u
.
φφu v
Month
φφv {
)
φφ{ |
)
φφ| }
}
χχ 
)
χχ 
.
χχ 
ToList
χχ 
(
χχ 
)
χχ 
;
χχ 
return
ωω 
new
ωω  
CompanyProfileDtos
ωω )
{
ωω* +
company
ωω, 3
=
ωω4 5

companyDto
ωω6 @
,
ωω@ A
deals
ϋϋ 
=
ϋϋ 
dealDtos
ϋϋ  
}
όό 
;
όό 
}
ύύ 	
private
ÿÿ 
async
ÿÿ 
Task
ÿÿ 
<
ÿÿ 
bool
ÿÿ 
>
ÿÿ  
CompanyHasVacancy
ÿÿ! 2
(
ÿÿ2 3
Guid
ÿÿ3 7
	companyid
ÿÿ8 A
,
ÿÿA B
Guid
ÿÿC G
	vacancyId
ÿÿH Q
)
ÿÿQ R
{
€€ 	
var
 
	myVacancy
 
=
 
await
 ! 
_vacancyRepository
" 4
.
4 5!
GetMyVacanciesAsync
5 H
(
H I
	companyid
I R
.
R S
ToString
S [
(
[ \
)
\ ]
,
] ^
	vacancyId
_ h
)
h i
;
i j
if
‚‚ 
(
‚‚ 
	myVacancy
‚‚ 
.
‚‚ 
Any
‚‚ 
(
‚‚ 
)
‚‚ 
)
‚‚  
{
ƒƒ 
return
„„ 
true
„„ 
;
„„ 
}
…… 
return
†† 
false
†† 
;
†† 
}
‡‡ 	
private
‰‰ 
async
‰‰ 
Task
‰‰ 
<
‰‰ 
VacancyDtos
‰‰ &
>
‰‰& '#
BuildFullVacancyAsync
‰‰( =
(
‰‰= >
Guid
‰‰> B
	vacancyId
‰‰C L
)
‰‰L M
{
 	
VacancyDtos
‹‹ 
vacancy
‹‹ 
=
‹‹  !
await
‹‹" ' 
_vacancyRepository
‹‹( :
.
‹‹: ;!
GetVacancyByIdAsync
‹‹; N
(
‹‹N O
	vacancyId
‹‹O X
)
‹‹X Y
;
‹‹Y Z
if
 
(
 
vacancy
 
==
 
null
 
)
  
{
 
return
 
null
 
;
 
}
 
List
‘‘ 
<
‘‘ 
int
‘‘ 
>
‘‘ 
allIds
‘‘ 
=
‘‘ 
vacancy
‘‘ &
.
‘‘& '

activities
‘‘' 1
.
‘‘1 2
Select
‘‘2 8
(
‘‘8 9
a
‘‘9 :
=>
‘‘; =
a
‘‘> ?
.
‘‘? @
id
‘‘@ B
)
‘‘B C
.
‘‘C D
Distinct
‘‘D L
(
‘‘L M
)
‘‘M N
.
‘‘N O
ToList
‘‘O U
(
‘‘U V
)
‘‘V W
??
‘‘X Z
new
‘‘[ ^
List
‘‘_ c
<
‘‘c d
int
‘‘d g
>
‘‘g h
(
‘‘h i
)
‘‘i j
;
‘‘j k
if
’’ 
(
’’ 
allIds
’’ 
.
’’ 
Any
’’ 
(
’’ 
)
’’ 
)
’’ 
{
““ 
List
•• 
<
•• $
TypeOfActivityResponse
•• +
>
••+ ,

activities
••- 7
=
••8 9
await
••: ?!
_filterCacheService
••@ S
.
••S T"
GetFiltersByIdsAsync
••T h
(
••h i
allIds
••i o
)
••o p
;
••p q

Dictionary
–– 
<
–– 
int
–– 
,
–– $
TypeOfActivityResponse
––  6
>
––6 7
activityDict
––8 D
=
––E F

activities
––G Q
.
––Q R
ToDictionary
––R ^
(
––^ _
a
––_ `
=>
––a c
a
––d e
.
––e f
id
––f h
,
––h i
a
––j k
=>
––l n
a
––o p
)
––p q
;
––q r
vacancy
 
.
 

activities
 "
=
# $
vacancy
% ,
.
, -

activities
- 7
.
™™ 
Where
™™ 
(
™™ 
a
™™ 
=>
™™ 
activityDict
™™  ,
.
™™, -
ContainsKey
™™- 8
(
™™8 9
a
™™9 :
.
™™: ;
id
™™; =
)
™™= >
)
™™> ?
.
 
Select
 
(
 
a
 
=>
  
activityDict
! -
[
- .
a
. /
.
/ 0
id
0 2
]
2 3
)
3 4
.
›› 
ToList
›› 
(
›› 
)
›› 
;
›› 
}
 
Guid
 
	copmanyId
 
=
 
(
 
Guid
 "
)
" #
vacancy
# *
.
* +

company_id
+ 5
!
5 6
;
6 7
Company
   
company
   
=
   
await
   # 
_companyRepository
  $ 6
.
  6 7!
GetCompanyByIdAsync
  7 J
(
  J K
	copmanyId
  K T
)
  T U
;
  U V

CompanyDto
΅΅ 

companyDto
΅΅ !
=
΅΅" #
new
΅΅$ '

CompanyDto
΅΅( 2
(
΅΅2 3
)
΅΅3 4
{
ΆΆ 
id
££ 
=
££ 
	copmanyId
££ 
,
££ 
email
¤¤ 
=
¤¤ 
company
¤¤ 
.
¤¤  
email
¤¤  %
,
¤¤% &
	longitude
¥¥ 
=
¥¥ 
company
¥¥ #
.
¥¥# $
	longitude
¥¥$ -
,
¥¥- .
latitude
¦¦ 
=
¦¦ 
company
¦¦ "
.
¦¦" #
latitude
¦¦# +
,
¦¦+ ,
name
§§ 
=
§§ 
company
§§ 
.
§§ 
name
§§ #
,
§§# $
phoneNumber
¨¨ 
=
¨¨ 
company
¨¨ %
.
¨¨% &
phoneNumber
¨¨& 1
,
¨¨1 2
website
©© 
=
©© 
company
©© !
.
©©! "
website
©©" )
,
©©) *
}
ªª 
;
ªª 
vacancy
¬¬ 
.
¬¬ 
company
¬¬ 
=
¬¬ 

companyDto
¬¬ (
;
¬¬( )
return
­­ 
vacancy
­­ 
;
­­ 
}
®® 	
}―― Ξ
wC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Http\Interfaces\IFilterClient.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &
Http& *
.* +

Interfaces+ 5
;5 6
public 
	interface 
IFilterClient 
{ 
Task 
< 	
List	 
< "
TypeOfActivityResponse $
?$ %
>% &
>& '
GetFiltersByIdAsync( ;
(; <
List< @
<@ A
intA D
>D E
	filterIdsF O
,O P
CancellationToken		 
cancellationToken		 +
=		, -
default		. 5
)		5 6
;		6 7
}

 “
uC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Http\Interfaces\IAuthClient.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &
Http& *
.* +

Interfaces+ 5
;5 6
public 
	interface 
IAuthClient 
{ 
Task 
< 	
UserResponse	 
? 
> 
GetUserByIdAsync (
(( )
string) /
userId0 6
,6 7
string8 >
token? D
,D E
CancellationTokenF W
cancellationTokenX i
=j k
defaultl s
)s t
;t u
}		 ½!
{C:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Http\Implementations\FilterClient.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &
Http& *
.* +
Implementations+ :
;: ;
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
$str	 
,
 ‹
id
 
.
 
ToString
 —
(
— 
)
 ™
)
™ 
)
 ›
;
› 
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
;	'' €
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
yC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Services\Http\Implementations\AuthClient.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
Services %
.% &
Http& *
.* +
Implementations+ :
;: ;
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
}** ‡'
uC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.BLL\Consumers\UserCompanyCreatedConsumer.cs
	namespace 	
CompanyService
 
. 
BLL 
. 
	Consumers &
;& '
public

 
class

 &
UserCompanyCreatedConsumer
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
UserCompanyCreatedEvent

4 K
>

K L
{ 
private 
readonly 
ILogger 
< &
UserCompanyCreatedConsumer 7
>7 8
_logger9 @
;@ A
private 
readonly 
ICompanyRepository '
_companyRepository( :
;: ;
private 
readonly 
ITopicProducer #
<# $(
UserCompanyCreateFailedEvent$ @
>@ A
_publishEndpointB R
;R S
public 
&
UserCompanyCreatedConsumer %
(% &
ILogger 
< &
UserCompanyCreatedConsumer *
>* +
logger, 2
,2 3
ICompanyRepository 
companyRepository ,
,, -
ITopicProducer 
< (
UserCompanyCreateFailedEvent 3
>3 4
publishEndpoint5 D
) 	
{ 
_logger 
= 
logger 
; 
_companyRepository 
= 
companyRepository .
;. /
_publishEndpoint 
= 
publishEndpoint *
;* +
} 
public 

async 
Task 
Consume 
( 
ConsumeContext ,
<, -#
UserCompanyCreatedEvent- D
>D E
contextF M
)M N
{ #
UserCompanyCreatedEvent 
message  '
=( )
context* 1
.1 2
Message2 9
;9 :
try 
{   	
Company!! 
?!! 
worker!! 
=!! 
await!! #
_companyRepository!!$ 6
.!!6 7
GetCompanyByIdAsync!!7 J
(!!J K
Guid!!K O
.!!O P
Parse!!P U
(!!U V
message!!V ]
.!!] ^
UserId!!^ d
)!!d e
)!!e f
;!!f g
if"" 
("" 
worker"" 
!="" 
null"" 
)"" 
{## 
_logger$$ 
.$$ 
LogInformation$$ &
($$& '
$"$$' )
$str$$) 9
{$$9 :
message$$: A
.$$A B
UserId$$B H
}$$H I
$str$$I [
"$$[ \
)$$\ ]
;$$] ^
return%% 
;%% 
}&& 
Company(( 
	newWorker(( 
=(( 
new((  #
Company(($ +
(((+ ,
)((, -
{)) 
UserId** 
=** 
Guid** 
.** 
Parse** #
(**# $
message**$ +
.**+ ,
UserId**, 2
)**2 3
,**3 4
name++ 
=++ 
message++ 
.++ 
name++ #
,++# $
email,, 
=,, 
message,, 
.,,  

email_info,,  *
,,,* +
phoneNumber-- 
=-- 
message-- %
.--% &

phone_info--& 0
,--0 1
	longitude.. 
=.. 
message.. #
...# $
	longitude..$ -
,..- .
latitude// 
=// 
message// "
.//" #
latitude//# +
,//+ ,
website00 
=00 
message00 !
.00! "
website00" )
,00) *
}11 
;11 
await33 
_companyRepository33 $
.33$ %
CreateCompanyAsync33% 7
(337 8
	newWorker338 A
)33A B
;33B C
_logger44 
.44 
LogInformation44 "
(44" #
$str44# K
,44K L
message44M T
.44T U
UserId44U [
)44[ \
;44\ ]
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
new99+ .(
UserCompanyCreateFailedEvent99/ K
(99K L
)99L M
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