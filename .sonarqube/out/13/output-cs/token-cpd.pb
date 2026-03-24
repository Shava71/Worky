Á
lC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Worker\OutboxPublisWorker.cs
	namespace 	
AuthService
 
. 
Infrastructure $
.$ %
Worker% +
;+ ,
public 
class 
OutboxPublisWorker 
:  !
BackgroundService" 3
{ 
private		 
readonly		 
ILogger		 
<		 
OutboxPublisWorker		 /
>		/ 0
_logger		1 8
;		8 9
private

 
readonly

 
IOutboxPublisher

 %
_outboxPublisher

& 6
;

6 7
public 

OutboxPublisWorker 
( 
IOutboxPublisher .
outboxPublisher/ >
,> ?
ILogger@ G
<G H
OutboxPublisWorkerH Z
>Z [
logger\ b
)b c
{ 
_logger 
= 
logger 
; 
_outboxPublisher 
= 
outboxPublisher *
;* +
} 
	protected 
override 
async 
Task !
ExecuteAsync" .
(. /
CancellationToken/ @
stoppingTokenA N
)N O
{ 
_logger 
. 
LogInformation 
( 
$str =
)= >
;> ?
while 
( 
! 
stoppingToken 
. #
IsCancellationRequested 5
)5 6
{ 	
try 
{ 
await 
_outboxPublisher &
.& ''
PublishPendingMessagesAsync' B
(B C
stoppingTokenC P
)P Q
;Q R
} 
catch 
( 
	Exception 
ex 
) 
{ 
_logger 
. 
LogError  
(  !
ex! #
,# $
$str% K
)K L
;L M
} 
await 
Task 
. 
Delay 
( 
TimeSpan %
.% &
FromSeconds& 1
(1 2
$num2 4
)4 5
,5 6
stoppingToken7 D
)D E
;E F
}   	
}!! 
}"" ù
rC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Services\Interface\IUserService.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Services" *
;* +
public 
	interface 
IUserService 
{		 
Task

 
<

 	
IActionResult

	 
>

 
RegisterAsync

 %
(

% &#
RegisterRequestContract

& =
request

> E
)

E F
;

F G
Task 
< 	
LoginResponse	 
> 

LoginAsync "
(" #
LoginRequest# /
request0 7
)7 8
;8 9
Task 
< 	
User	 
> 
GetProfileByIdAsync "
(" #
Guid# '
userId( .
). /
;/ 0
} â
vC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Services\Interface\IOutboxPublisher.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Services" *
;* +
public 
	interface 
IOutboxPublisher !
{ 
Task '
PublishPendingMessagesAsync	 $
($ %
CancellationToken% 6
cancellationToken7 H
)H I
;I J
} ’
qC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Services\Interface\IJwtService.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Services" *
;* +
public 
	interface 
IJwtService 
{ 
public 

string 
GenerateToken 
(  
Guid  $
userId% +
,+ ,
IList- 2
<2 3
string3 9
>9 :
Role; ?
)? @
;@ A
} ıS
vC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Services\Implementation\UserService.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Services" *
;* +
public 
class 
UserService 
: 
IUserService '
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
private 
readonly 
IJwtService $
_jwtService% 0
;0 1
private 
readonly 
ILogger  
<  !
UserService! ,
>, -
_logger. 5
;5 6
public 
UserService 
( 
IUserRepository *
authRepository+ 9
,9 :
IJwtService; F

jwtServiceG Q
,Q R
ILoggerS Z
<Z [
UserService[ f
>f g
loggerh n
)n o
{ 	
_userRepository 
= 
authRepository ,
;, -
_jwtService 
= 

jwtService $
;$ %
_logger 
= 
logger 
; 
} 	
public 
async 
Task 
< 
IActionResult '
>' (
RegisterAsync) 6
(6 7#
RegisterRequestContract7 N
requestO V
)V W
{ 	
try 
{ 
User 
user 
= 
new 
User  $
($ %
request% ,
., -
UserName- 5
,5 6
request7 >
.> ?
Email? D
,D E
requestF M
.M N
PasswordHashN Z
,Z [
request\ c
.c d
PhoneNumberd o
)o p
;p q
User   

userExists   
=    !
await  " '
_userRepository  ( 7
.  7 8
FindByEmailAsync  8 H
(  H I
request  I P
.  P Q
Email  Q V
)  V W
;  W X
if!! 
(!! 

userExists!! 
!=!! !
null!!" &
)!!& '
{"" 
return## 
new## "
BadRequestObjectResult## 5
(##5 6
$str##6 X
)##X Y
;##Y Z
}$$ 
object&& 
@event&& 
;&& 
string'' 
topic'' 
;'' 
if(( 
((( 
request(( 
.(( 
Role((  
.((  !
Equals((! '
(((' (
$str((( 0
)((0 1
)((1 2
{)) 
@event** 
=** 
new**  "
UserWorkerCreatedEvent**! 7
(**7 8
)**8 9
{++ 
UserId,, 
=,,  
user,,! %
.,,% &
Id,,& (
.,,( )
ToString,,) 1
(,,1 2
),,2 3
,,,3 4
second_name-- #
=--$ %
request--& -
.--- .
second_name--. 9
!--9 :
,--: ;

first_name.. "
=..# $
request..% ,
..., -

first_name..- 7
!..7 8
,..8 9
surname// 
=//  !
request//" )
.//) *
surname//* 1
!//1 2
,//2 3
birthday00  
=00! "
request00# *
.00* +
birthday00+ 3
!003 4
.004 5
Value005 :
,00: ;

phone_info22 "
=22# $
request22% ,
.22, -

phone_info22- 7
!227 8
,228 9

email_info33 "
=33# $
request33% ,
.33, -

email_info33- 7
!337 8
,338 9
}44 
;44 
topic55 
=55 
$str55 1
;551 2
}66 
else77 
if77 
(77 
request77  
.77  !
Role77! %
.77% &
Equals77& ,
(77, -
$str77- 6
)776 7
)777 8
{88 
@event99 
=99 
new99  #
UserCompanyCreatedEvent99! 8
(998 9
)999 :
{:: 
UserId;; 
=;;  
user;;! %
.;;% &
Id;;& (
.;;( )
ToString;;) 1
(;;1 2
);;2 3
,;;3 4

email_info<< "
=<<# $
request<<% ,
.<<, -

email_info<<- 7
!<<7 8
,<<8 9
latitude==  
===! "
request==# *
.==* +
latitude==+ 3
!==3 4
,==4 5
	longitude>> !
=>>" #
request>>$ +
.>>+ ,
	longitude>>, 5
!>>5 6
,>>6 7
name?? 
=?? 
request?? &
.??& '
name??' +
!??+ ,
,??, -

phone_info@@ "
=@@# $
request@@% ,
.@@, -

phone_info@@- 7
!@@7 8
,@@8 9
websiteAA 
=AA  !
requestAA" )
.AA) *
websiteAA* 1
!AA1 2
,AA2 3
}BB 
;BB 
topicCC 
=CC 
$strCC 2
;CC2 3
}DD 
elseEE 
{FF 
throwGG 
newGG 
ArgumentExceptionGG /
(GG/ 0
$"GG0 2
$strGG2 D
{GGD E
requestGGE L
.GGL M
RoleGGM Q
}GGQ R
"GGR S
)GGS T
;GGT U
}HH 
stringJJ 
JsonPayloadJJ "
=JJ# $
JsonSerializerJJ% 3
.JJ3 4
	SerializeJJ4 =
(JJ= >
@eventJJ> D
)JJD E
;JJE F
OutboxMessageLL 
outboxMessageLL +
=LL, -
newLL. 1
OutboxMessageLL2 ?
(LL? @
_topicLL@ F
:LLF G
topicLLH M
,LLM N
_typeMM 
:MM 
@eventMM  
.MM  !
GetTypeMM! (
(MM( )
)MM) *
.MM* +
NameMM+ /
,MM/ 0
_payloadNN 
:NN 
JsonPayloadNN )
)NN) *
;NN* +
RoleRR 
roleRR 
=RR 
awaitRR !
_userRepositoryRR" 1
.RR1 2
FindRoleByNameAsyncRR2 E
(RRE F
requestRRF M
.RRM N
RoleRRN R
)RRR S
;RRS T
ifSS 
(SS 
roleSS 
==SS 
nullSS  
)SS  !
returnSS" (
newSS) ,"
BadRequestObjectResultSS- C
(SSC D
$strSSD ]
)SS] ^
;SS^ _
awaitUU 
_userRepositoryUU %
.UU% &,
 CreateUserWithOutboxMessageAsyncUU& F
(UUF G
userUUG K
,UUK L
messageUUM T
:UUT U
outboxMessageUUV c
)UUc d
;UUd e
awaitXX 
_userRepositoryXX %
.XX% &
AddToRoleAsyncXX& 4
(XX4 5
userXX5 9
,XX9 :
roleXX; ?
)XX? @
;XX@ A
return
õõ 
new
õõ 
OkObjectResult
õõ )
(
õõ) *
new
õõ* -
{
õõ. /
Message
õõ0 7
=
õõ8 9
$str
õõ: H
,
õõH I
UserId
õõJ P
=
õõQ R
user
õõS W
.
õõW X
Id
õõX Z
}
õõ[ \
)
õõ\ ]
;
õõ] ^
}
úú 
catch
ùù 
(
ùù 
	Exception
ùù 
ex
ùù 
)
ùù  
{
ûû 
_logger
üü 
.
üü 
LogError
üü  
(
üü  !
ex
üü! #
,
üü# $
$str
üü% 9
)
üü9 :
;
üü: ;
return
†† 
new
†† 
BadRequestResult
†† +
(
††+ ,
)
††, -
;
††- .
}
°° 
}
¢¢ 	
public
§§ 
async
§§ 
Task
§§ 
<
§§ 
LoginResponse
§§ '
>
§§' (

LoginAsync
§§) 3
(
§§3 4
LoginRequest
§§4 @
request
§§A H
)
§§H I
{
•• 	
User
¶¶ 
user
¶¶ 
=
¶¶ 
await
¶¶ 
_userRepository
¶¶ -
.
¶¶- .
FindByEmailAsync
¶¶. >
(
¶¶> ?
request
¶¶? F
.
¶¶F G
Email
¶¶G L
)
¶¶L M
;
¶¶M N
if
ßß 
(
ßß 
user
ßß 
!=
ßß 
null
ßß 
&&
ßß 
await
ßß  %
_userRepository
ßß& 5
.
ßß5 6 
CheckPasswordAsync
ßß6 H
(
ßßH I
user
ßßI M
,
ßßM N
request
ßßO V
.
ßßV W
Password
ßßW _
)
ßß_ `
)
ßß` a
{
®® 
List
©© 
<
©© 
string
©© 
>
©© 
roles
©© "
=
©©# $
await
©©% *
_userRepository
©©+ :
.
©©: ;
GetRolesAsync
©©; H
(
©©H I
user
©©I M
)
©©M N
;
©©N O
var
™™ 
jwt
™™ 
=
™™ 
_jwtService
™™ %
.
™™% &
GenerateToken
™™& 3
(
™™3 4
user
™™4 8
.
™™8 9
Id
™™9 ;
,
™™; <
roles
™™= B
)
™™B C
;
™™C D
return
´´ 
new
´´ 
LoginResponse
´´ (
{
´´) *
Id
´´+ -
=
´´. /
user
´´0 4
.
´´4 5
Id
´´5 7
.
´´7 8
ToString
´´8 @
(
´´@ A
)
´´A B
,
´´B C
Token
´´D I
=
´´J K
jwt
´´L O
,
´´O P
Role
´´Q U
=
´´V W
roles
´´X ]
}
´´^ _
;
´´_ `
}
¨¨ 
return
≠≠ 
null
≠≠ 
;
≠≠ 
}
ÆÆ 	
public
∞∞ 
async
∞∞ 
Task
∞∞ 
<
∞∞ 
User
∞∞ 
?
∞∞ 
>
∞∞  !
GetProfileByIdAsync
∞∞! 4
(
∞∞4 5
Guid
∞∞5 9
userId
∞∞: @
)
∞∞@ A
{
±± 	
User
≤≤ 
user
≤≤ 
=
≤≤ 
await
≤≤ 
_userRepository
≤≤ -
.
≤≤- .
FindByIdAsync
≤≤. ;
(
≤≤; <
userId
≤≤< B
.
≤≤B C
ToString
≤≤C K
(
≤≤K L
)
≤≤L M
)
≤≤M N
;
≤≤N O
return
≥≥ 
user
≥≥ 
;
≥≥ 
}
¥¥ 	
}ºº ›%
C:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Services\Implementation\KafkaOutboxPublisher.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Services" *
;* +
public

 
class

  
KafkaOutboxPublisher

 !
:

" #
IOutboxPublisher

$ 4
{ 
private 
readonly 
AuthDbContext "

_dbContext# -
;- .
private 
readonly 
	IProducer 
< 
string %
,% &
string' -
>- .
	_producer/ 8
;8 9
private 
readonly 
ILogger 
<  
KafkaOutboxPublisher 1
>1 2
_logger3 :
;: ;
public 
 
KafkaOutboxPublisher 
(  
AuthDbContext  -
	dbContext. 7
,7 8 
KafkaProducerFactory9 M 
kafkaProducerFactoryN b
,b c
ILoggerd k
<k l!
KafkaOutboxPublisher	l Ä
>
Ä Å
logger
Ç à
)
à â
{ 

_dbContext 
= 
	dbContext 
; 
	_producer 
=  
kafkaProducerFactory (
.( )
CreateProducer) 7
(7 8
)8 9
;9 :
_logger 
= 
logger 
; 
} 
public 

async 
Task '
PublishPendingMessagesAsync 1
(1 2
CancellationToken2 C
cancellationTokenD U
)U V
{ 
List 
< 
OutboxMessage 
> 
pending #
=$ %
await& +

_dbContext, 6
.6 7
OutboxMessage7 D
. 
Where 
( 
o 
=> 
o 
. 
Sent 
== !
false" '
)' (
. 
OrderBy 
( 
o 
=> 
o 
. 

OccurredAt &
)& '
. 
Take 
( 
$num 
) 
. 
ToListAsync 
( 
cancellationToken *
)* +
;+ ,
if 

( 
! 
pending 
. 
Any 
( 
) 
) 
{   	
return!! 
;!! 
}"" 	
	_producer## 
.## 
BeginTransaction## "
(##" #
)### $
;##$ %
bool%% 
success%% 
=%% 
true%% 
;%% 
foreach'' 
('' 
OutboxMessage'' 
msg'' "
in''# %
pending''& -
)''- .
{(( 	
try)) 
{** 
await++ 
	_producer++ 
.++  
ProduceAsync++  ,
(++, -
msg,, 
.,, 
Topic,, 
,,, 
new-- 
Message-- 
<--  
string--  &
,--& '
string--( .
>--. /
{--0 1
Key.. 
=.. 
msg.. !
...! "
Id.." $
...$ %
ToString..% -
(..- .
)... /
,../ 0
Value// 
=// 
msg//  #
.//# $
Payload//$ +
}//, -
,//- .
cancellationToken00 %
)11 
;11 
msg22 
.22 

MarkAsSent22 
(22 
)22  
;22  !
}33 
catch44 
(44 
	Exception44 
ex44 
)44  
{55 
_logger66 
.66 
LogError66  
(66  !
ex66! #
,66# $
$str66% L
,66L M
msg66N Q
.66Q R
Id66R T
)66T U
;66U V
success77 
=77 
false77 
;77  
break88 
;88 
}99 
if;; 
(;; 
success;; 
);; 
{<< 
await== 

_dbContext==  
.==  !
SaveChangesAsync==! 1
(==1 2
cancellationToken==2 C
)==C D
;==D E
	_producer>> 
.>> 
CommitTransaction>> +
(>>+ ,
)>>, -
;>>- .
}?? 
else@@ 
{AA 
	_producerBB 
.BB 
AbortTransactionBB *
(BB* +
)BB+ ,
;BB, -
}CC 
}DD 	
}EE 
}FF †
gC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Models\LoginResponse.cs
	namespace 	
AuthService
 
. 
Application !
;! "
public 
class 
LoginResponse 
{ 
public 

string 
Id 
{ 
get 
; 
set 
;  
}! "
public 

string 
Token 
{ 
get 
; 
set "
;" #
}$ %
public 

IList 
< 
string 
> 
Role 
{ 
get  #
;# $
set% (
;( )
}* +
} ü
uC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Services\Implementation\JwtService.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Services" *
;* +
public

 
class

 

JwtService

 
:

 
IJwtService

 %
{ 
private 
readonly 
IConfiguration #
_configuration$ 2
;2 3
private 
readonly  
SymmetricSecurityKey )
_key* .
;. /
private 
readonly 
string 
? 
_issuer $
;$ %
private 
readonly 
string 
? 
	_audience &
;& '
public 


JwtService 
( 
IConfiguration $
config% +
,+ , 
SymmetricSecurityKey- A
securityKeyB M
)M N
{ 
_configuration 
= 
config 
;  
_key 
= 
securityKey 
; 
_issuer 
= 
_configuration  
[  !
$str! -
]- .
;. /
	_audience 
= 
_configuration "
[" #
$str# 1
]1 2
;2 3
} 
public77 

string77 
GenerateToken77 
(77  
Guid77  $
userId77% +
,77+ ,
IList77- 2
<772 3
string773 9
>779 :
Roles77; @
)77@ A
{88 
var99 
tokenLifetimeMins99 
=99 
_configuration99  .
.99. /
GetValue99/ 7
<997 8
int998 ;
>99; <
(99< =
$str99= S
)99S T
;99T U
Claim;; 
[;; 
];; 
claims;; 
=;; 
new;; 
[;; 
];; 
{<< 	
new== 
Claim== 
(== 

ClaimTypes==  
.==  !
NameIdentifier==! /
,==/ 0
userId==1 7
.==7 8
ToString==8 @
(==@ A
)==A B
)==B C
,==C D
}>> 	
;>>	 

claims?? 
=?? 
claims?? 
.?? 
Concat?? 
(?? 
Roles?? $
.??$ %
Select??% +
(??+ ,
role??, 0
=>??1 3
new??4 7
Claim??8 =
(??= >

ClaimTypes??> H
.??H I
Role??I M
,??M N
role??O S
)??S T
)??T U
)??U V
.??V W
ToArray??W ^
(??^ _
)??_ `
;??` a
varAA 
signingCredentialsAA 
=AA  
newAA! $
SigningCredentialsAA% 7
(AA7 8
_keyBB 
,BB 
SecurityAlgorithmsBB $
.BB$ %

HmacSha256BB% /
)BB/ 0
;BB0 1
varDD 
JwtTokenDD 
=DD 
newDD 
JwtSecurityTokenDD +
(EE 	
issuerRR 
:RR 
_issuerRR 
,RR 
audienceSS 
:SS 
	_audienceSS 
,SS  
claimsTT 
:TT 
claimsTT 
,TT 
expiresUU 
:UU 
DateTimeUU 
.UU 
UtcNowUU $
.UU$ %

AddMinutesUU% /
(UU/ 0
tokenLifetimeMinsUU0 A
)UUA B
,UUB C
signingCredentialsVV 
:VV 
signingCredentialsVV  2
)WW 	
;WW	 

return[[ 
new[[ #
JwtSecurityTokenHandler[[ *
([[* +
)[[+ ,
.[[, -

WriteToken[[- 7
([[7 8
JwtToken[[8 @
)[[@ A
;[[A B
}\\ 
}]] “
pC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Events\UserWorkerCreatedEvent.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Events" (
;( )
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
} ç
qC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Events\UserCompanyCreatedEvent.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Events" (
;( )
public 
class #
UserCompanyCreatedEvent $
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
 
name

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
[ 
Required 
] 
public 

string 
latitude 
{ 
get  
;  !
set" %
;% &
}' (
[ 
Required 
] 
public 

string 
	longitude 
{ 
get !
;! "
set# &
;& '
}( )
[ 
Required 
] 
public 

string 

email_info 
{ 
get "
;" #
set$ '
;' (
}) *
[ 
Required 
] 
public 

string 

phone_info 
{ 
get "
;" #
set$ '
;' (
}) *
[ 
Required 
] 
public 

string 
website 
{ 
get 
;  
set! $
;$ %
}& '
} ú
tC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Application\Contracts\RegisterRequestContract.cs
	namespace 	
AuthService
 
. 
Application !
;! "
public 
class #
RegisterRequestContract $
{ 
[ 
Required 
( 
ErrorMessage 
= 
$str 1
)1 2
]2 3
public		 

string		 
UserName		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
[ 
Required 
( 
ErrorMessage 
= 
$str 0
)0 1
]1 2
[ 
EmailAddress 
( 
ErrorMessage 
=  
$str! 3
)3 4
]4 5
public 

string 
Email 
{ 
get 
; 
set "
;" #
}$ %
[ 
Phone 

]
 
public 
string 
? 
PhoneNumber &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
[ 
Required 
( 
ErrorMessage 
= 
$str 3
)3 4
]4 5
public 

string 
PasswordHash 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 
Required 
( 
ErrorMessage 
= 
$str /
)/ 0
]0 1
public 

string 
Role 
{ 
get 
; 
set !
;! "
}# $
public 

string 
? 
name 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
latitude 
{ 
get !
;! "
set# &
;& '
}( )
public 

string 
? 
	longitude 
{ 
get "
;" #
set$ '
;' (
}) *
[ 
EmailAddress 
] 
public 
string  
?  !

email_info" ,
{- .
get/ 2
;2 3
set4 7
;7 8
}9 :
[ 
Phone 

]
 
public 
string 
? 

phone_info %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
[ 
Url 
] 	
public
 
string 
? 
website  
{! "
get# &
;& '
set( +
;+ ,
}- .
public   

string   
?   
second_name   
{    
get  ! $
;  $ %
set  & )
;  ) *
}  + ,
public!! 

string!! 
?!! 

first_name!! 
{!! 
get!!  #
;!!# $
set!!% (
;!!( )
}!!* +
public"" 

string"" 
?"" 
surname"" 
{"" 
get""  
;""  !
set""" %
;""% &
}""' (
public## 

DateOnly## 
?## 
birthday## 
{## 
get##  #
;### $
set##% (
;##( )
}##* +
}$$ 