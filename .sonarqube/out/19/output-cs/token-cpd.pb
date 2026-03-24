ª`
RC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Api\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder 
. 
Services 
. 
AddControllers 
(  
)  !
;! "
builder 
. 
Services 
. #
AddEndpointsApiExplorer (
(( )
)) *
;* +
builder 
. 
Services 
. 
AddAuthentication "
(" #
options# *
=>+ -
{ 
options 
. %
DefaultAuthenticateScheme )
=* +
JwtBearerDefaults, =
.= > 
AuthenticationScheme> R
;R S
options 
. 
DefaultSignInScheme #
=$ %
JwtBearerDefaults& 7
.7 8 
AuthenticationScheme8 L
;L M
options 
. "
DefaultChallengeScheme &
=' (
JwtBearerDefaults) :
.: ; 
AuthenticationScheme; O
;O P
} 
) 
. 
AddJwtBearer 
( 
options 
=> 
{ 
options 
. %
TokenValidationParameters )
=* +
new, /%
TokenValidationParameters0 I
{ 	
ValidateIssuer 
= 
true !
,! "
ValidIssuer!! 
=!! 
builder!! !
.!!! "
Configuration!!" /
[!!/ 0
$str!!0 <
]!!< =
,!!= >
ValidateAudience## 
=## 
true## #
,### $
ValidAudience%% 
=%% 
builder%% #
.%%# $
Configuration%%$ 1
[%%1 2
$str%%2 @
]%%@ A
,%%A B
ValidateLifetime'' 
='' 
true'' #
,''# $
IssuerSigningKey)) 
=)) 
new)) " 
SymmetricSecurityKey))# 7
())7 8
Encoding))8 @
.))@ A
UTF8))A E
.))E F
GetBytes))F N
())N O
builder))O V
.))V W
Configuration))W d
[))d e
$str))e n
]))n o
!))o p
)))p q
)))q r
,))r s$
ValidateIssuerSigningKey++ $
=++% &
true++' +
,+++ ,
},, 	
;,,	 

options.. 
... 
Events.. 
=.. 
new.. 
JwtBearerEvents.. ,
{// 	
OnChallenge00 
=00 
context00 !
=>00" $
{11 
context22 
.22 
HandleResponse22 &
(22& '
)22' (
;22( )
context33 
.33 
Response33  
.33  !

StatusCode33! +
=33, -
$num33. 1
;331 2
context44 
.44 
Response44  
.44  !
Headers44! (
.44( )
Add44) ,
(44, -
$str44- 7
,447 8
context449 @
.44@ A
HttpContext44A L
.44L M
TraceIdentifier44M \
)44\ ]
;44] ^
return55 
context55 
.55 
Response55 '
.55' (
WriteAsJsonAsync55( 8
(558 9
new559 <
{55= >
message55? F
=55G H
$str55I f
}55g h
,55h i!
JsonSerializerOptions66 )
.66) *
Default66* 1
)661 2
;662 3
}77 
,77 
OnForbidden88 
=88 
context88 !
=>88" $
{99 
context:: 
.:: 
NoResult::  
(::  !
)::! "
;::" #
context;; 
.;; 
Response;;  
.;;  !

StatusCode;;! +
=;;, -
$num;;. 1
;;;1 2
context<< 
.<< 
Response<<  
.<<  !
Headers<<! (
.<<( )
Add<<) ,
(<<, -
$str<<- 7
,<<7 8
context<<9 @
.<<@ A
HttpContext<<A L
.<<L M
TraceIdentifier<<M \
)<<\ ]
;<<] ^
return== 
context== 
.== 
Response== '
.==' (
WriteAsJsonAsync==( 8
(==8 9
new==9 <
{=== >
message==? F
===G H
$str==I n
}==o p
,==p q!
JsonSerializerOptions>> )
.>>) *
Default>>* 1
)>>1 2
;>>2 3
}?? 
}@@ 	
;@@	 

}AA 
)AA 
;AA 
builderBB 
.BB 
ServicesBB 
.BB 
AddAuthorizationBB !
(BB! "
)BB" #
;BB# $
builderDD 
.DD 
ServicesDD 
.DD 
AddSwaggerGenDD 
(DD 
cDD  
=>DD! #
{EE 
tryFF 
{GG 
cHH 	
.HH	 


SwaggerDocHH
 
(HH 
$strHH 
,HH 
newHH 
OpenApiInfoHH *
{HH+ ,
TitleHH- 2
=HH3 4
$strHH5 <
,HH< =
VersionHH> E
=HHF G
$strHHH L
}HHM N
)HHN O
;HHO P
cKK 	
.KK	 
!
AddSecurityDefinitionKK
 
(KK  
$strKK  (
,KK( )
newKK* -!
OpenApiSecuritySchemeKK. C
{LL 	
InMM 
=MM 
ParameterLocationMM "
.MM" #
HeaderMM# )
,MM) *
DescriptionNN 
=NN 
$strNN C
,NNC D
NameOO 
=OO 
$strOO "
,OO" #
TypePP 
=PP 
SecuritySchemeTypePP %
.PP% &
ApiKeyPP& ,
,PP, -
SchemeQQ 
=QQ 
$strQQ 
}RR 	
)RR	 

;RR
 
cUU 	
.UU	 
"
AddSecurityRequirementUU
  
(UU  !
newUU! $&
OpenApiSecurityRequirementUU% ?
(UU? @
)UU@ A
{VV 	
{WW 
newXX !
OpenApiSecuritySchemeXX )
{YY 
	ReferenceZZ 
=ZZ 
newZZ  #
OpenApiReferenceZZ$ 4
{[[ 
Type\\ 
=\\ 
ReferenceType\\ ,
.\\, -
SecurityScheme\\- ;
,\\; <
Id]] 
=]] 
$str]] %
}^^ 
,^^ 
Scheme__ 
=__ 
$str__ %
,__% &
Name`` 
=`` 
$str`` #
,``# $
Inaa 
=aa 
ParameterLocationaa *
.aa* +
Headeraa+ 1
,aa1 2
}bb 
,bb 
newcc 
Listcc 
<cc 
stringcc 
>cc  
(cc  !
)cc! "
}dd 
}ee 	
)ee	 

;ee
 
}ff 
catchgg 	
(gg
 
	Exceptiongg 
exgg 
)gg 
{hh 
Consoleii 
.ii 
	WriteLineii 
(ii 
$strii (
+ii) *
exii+ -
.ii- .
Messageii. 5
)ii5 6
;ii6 7
throwjj 
;jj 
}kk 
}ll 
)ll 
;ll 
builderoo 
.oo 
Servicesoo 
.oo 
AddDbContextoo 
<oo 
AuthDbContextoo +
>oo+ ,
(oo, -
optionsoo- 4
=>oo5 7
optionspp 
.pp 
	UseNpgsqlpp 
(pp 
builderpp 
.pp 
Configurationpp +
.pp+ ,
GetConnectionStringpp, ?
(pp? @
$strpp@ S
)ppS T
)ppT U
)ppU V
;ppV W
varss 
keyss 
=ss 	
Encodingss
 
.ss 
UTF8ss 
.ss 
GetBytesss  
(ss  !
builderss! (
.ss( )
Configurationss) 6
[ss6 7
$strss7 @
]ss@ A
)ssA B
;ssB C
vartt 

signingKeytt 
=tt 
newtt  
SymmetricSecurityKeytt )
(tt) *
keytt* -
)tt- .
;tt. /
builderuu 
.uu 
Servicesuu 
.uu 
AddSingletonuu 
(uu 

signingKeyuu (
)uu( )
;uu) *
buildervv 
.vv 
Servicesvv 
.vv 
AddAuthServicesvv  
(vv  !
buildervv! (
.vv( )
Configurationvv) 6
)vv6 7
;vv7 8
builderyy 
.yy 
Servicesyy 
.yy 
AddCorsyy 
(yy 
oyy 
=>yy 
oyy 
.yy  
AddDefaultPolicyyy  0
(yy0 1
pyy1 2
=>yy3 5
pzz 
.zz 
AllowAnyOriginzz 
(zz 
)zz 
.zz 
AllowAnyHeaderzz %
(zz% &
)zz& '
.zz' (
AllowAnyMethodzz( 6
(zz6 7
)zz7 8
)zz8 9
)zz9 :
;zz: ;
var|| 
app|| 
=|| 	
builder||
 
.|| 
Build|| 
(|| 
)|| 
;|| 
app 
. 

UseSwagger 
( 
) 
; 
appÄÄ 
.
ÄÄ 
UseSwaggerUI
ÄÄ 
(
ÄÄ 
c
ÄÄ 
=>
ÄÄ 
{ÅÅ 
c
ÇÇ 
.
ÇÇ 
SwaggerEndpoint
ÇÇ 
(
ÇÇ 
$str
ÇÇ 0
,
ÇÇ0 1
$str
ÇÇ2 F
)
ÇÇF G
;
ÇÇG H
c
ÉÉ 
.
ÉÉ 
RoutePrefix
ÉÉ 
=
ÉÉ 
string
ÉÉ 
.
ÉÉ 
Empty
ÉÉ  
;
ÉÉ  !
}ÑÑ 
)
ÑÑ 
;
ÑÑ 
appÜÜ 
.
ÜÜ 
UseCors
ÜÜ 
(
ÜÜ 
)
ÜÜ 
;
ÜÜ 
appáá 
.
áá 

UseRouting
áá 
(
áá 
)
áá 
;
áá 
appàà 
.
àà 
UseAuthentication
àà 
(
àà 
)
àà 
;
àà 
appââ 
.
ââ 
UseAuthorization
ââ 
(
ââ 
)
ââ 
;
ââ 
appää 
.
ää 
MapControllers
ää 
(
ää 
)
ää 
;
ää 
usingåå 
(
åå 
var
åå 

scope
åå 
=
åå 
app
åå 
.
åå 
Services
åå 
.
åå  
CreateScope
åå  +
(
åå+ ,
)
åå, -
)
åå- .
{çç 
var
éé 
db
éé 

=
éé 
scope
éé 
.
éé 
ServiceProvider
éé "
.
éé" # 
GetRequiredService
éé# 5
<
éé5 6
AuthDbContext
éé6 C
>
ééC D
(
ééD E
)
ééE F
;
ééF G
db
èè 
.
èè 
Database
èè 
.
èè 
Migrate
èè 
(
èè 
)
èè 
;
èè 
}êê 
usingíí 
(
íí 
var
íí 

scope
íí 
=
íí 
app
íí 
.
íí 
Services
íí 
.
íí  
CreateScope
íí  +
(
íí+ ,
)
íí, -
)
íí- .
{ìì 
var
îî 
initializer
îî 
=
îî 
scope
îî 
.
îî 
ServiceProvider
îî +
.
îî+ , 
GetRequiredService
îî, >
<
îî> ?$
MinioBucketInitializer
îî? U
>
îîU V
(
îîV W
)
îîW X
;
îîX Y
await
ïï 	
initializer
ïï
 
.
ïï 
InitializeAsync
ïï %
(
ïï% &
)
ïï& '
;
ïï' (
}ññ 
appòò 
.
òò 
Run
òò 
(
òò 
)
òò 	
;
òò	 
≤
aC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Api\MinioBucketInitializer.cs
	namespace 	
AuthService
 
. 
Api 
; 
public 
class "
MinioBucketInitializer #
{ 
private 
readonly 
IMinioClient !
_minio" (
;( )
private		 
readonly		 
IConfiguration		 #
_config		$ +
;		+ ,
public 
"
MinioBucketInitializer !
(! "
IMinioClient" .
minio/ 4
,4 5
IConfiguration6 D
configE K
)K L
{ 
_minio 
= 
minio 
; 
_config 
= 
config 
; 
} 
public 

async 
Task 
InitializeAsync %
(% &
)& '
{ 
var 
bucket 
= 
_config 
[ 
$str +
]+ ,
;, -
bool 
exists 
= 
await 
_minio "
." #
BucketExistsAsync# 4
(4 5
new 
BucketExistsArgs  
(  !
)! "
." #

WithBucket# -
(- .
bucket. 4
)4 5
) 	
;	 

if 

( 
! 
exists 
) 
{ 	
await 
_minio 
. 
MakeBucketAsync (
(( )
new 
MakeBucketArgs "
(" #
)# $
.$ %

WithBucket% /
(/ 0
bucket0 6
)6 7
) 
; 
Console 
. 
	WriteLine 
( 
$"  
$str  8
{8 9
bucket9 ?
}? @
"@ A
)A B
;B C
}   	
else!! 
{"" 	
Console## 
.## 
	WriteLine## 
(## 
$"##  
$str##  ?
{##? @
bucket##@ F
}##F G
"##G H
)##H I
;##I J
}$$ 	
}%% 
}&& ¿9
mC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Api\Controllers\ProfilePhotoController.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Controllers" -
;- .
[ 
ApiController 
] 
[		 
Route		 
(		 
$str		 
)		  
]		  !
public

 
class

 "
ProfilePhotoController

 #
:

$ %

Controller

& 0
{ 
private 
readonly 
IMinioClient !
_minio" (
;( )
private 
readonly 
IConfiguration #
_config$ +
;+ ,
public 
"
ProfilePhotoController !
(! "
IMinioClient" .
minio/ 4
,4 5
IConfiguration6 D
configE K
)K L
{ 
_minio 
= 
minio 
; 
_config 
= 
config 
; 
} 
private 
string 
Bucket 
=> 
_config $
[$ %
$str% 3
]3 4
;4 5
private 
string 
GetObjectName  
(  !
Guid! %
userId& ,
), -
{ 
return 
$" 
{ 
userId 
} 
$str 
" 
;  
} 
private 
bool 
CheckUserId 
( 
Guid !
userId" (
)( )
{ 
Guid 
currentUser 
= 
Guid 
.  
Parse  %
(% &
User& *
.* +
FindFirstValue+ 9
(9 :

ClaimTypes: D
.D E
NameIdentifierE S
)S T
)T U
;U V
return 
currentUser 
== 
userId $
;$ %
} 
[$$ 
HttpGet$$ 
($$ 
$str$$ 
)$$ 
]$$ 
public%% 

async%% 
Task%% 
<%% 
IActionResult%% #
>%%# $
Get%%% (
(%%( )
Guid%%) -
userId%%. 4
)%%4 5
{&& 
var'' 

objectName'' 
='' 
GetObjectName'' &
(''& '
userId''' -
)''- .
;''. /
var(( 
url(( 
=(( 
await(( 
_minio(( 
.(( #
PresignedGetObjectAsync(( 6
(((6 7
new)) "
PresignedGetObjectArgs)) &
())& '
)))' (
.** 

WithBucket** 
(** 
Bucket** "
)**" #
.++ 

WithObject++ 
(++ 

objectName++ &
)++& '
.,, 

WithExpiry,, 
(,, 
$num,, 
*,,  
$num,,! #
),,# $
)-- 	
;--	 

return.. 
Ok.. 
(.. 
new.. 
{.. 
url.. 
}.. 
).. 
;.. 
}// 
[55 
HttpPost55 
(55 
$str55 
)55  
]55  !
public66 

async66 
Task66 
<66 
IActionResult66 #
>66# $
Upload66% +
(66+ ,
Guid66, 0
userId661 7
)667 8
{77 
if88 

(88 
!88 
CheckUserId88 
(88 
userId88 
)88  
)88  !
return99 
Unauthorized99 
(99  
)99  !
;99! "
var:: 

objectName:: 
=:: 
GetObjectName:: &
(::& '
userId::' -
)::- .
;::. /
var;; 
url;; 
=;; 
await;; 
_minio;; 
.;; #
PresignedPutObjectAsync;; 6
(;;6 7
new<< "
PresignedPutObjectArgs<< &
(<<& '
)<<' (
.== 

WithBucket== 
(== 
Bucket== "
)==" #
.>> 

WithObject>> 
(>> 

objectName>> &
)>>& '
.?? 

WithExpiry?? 
(?? 
$num?? 
*??  
$num??! #
)??# $
)@@ 	
;@@	 

returnAA 
OkAA 
(AA 
newAA 
{AA 
urlAA 
}AA 
)AA 
;AA 
}BB 
[HH 
HttpPutHH 
(HH 
$strHH 
)HH 
]HH 
publicII 

asyncII 
TaskII 
<II 
IActionResultII #
>II# $
UpdateII% +
(II+ ,
GuidII, 0
userIdII1 7
)II7 8
{JJ 
ifKK 

(KK 
!KK 
CheckUserIdKK 
(KK 
userIdKK 
)KK  
)KK  !
returnLL 
UnauthorizedLL 
(LL  
)LL  !
;LL! "
varMM 

objectNameMM 
=MM 
GetObjectNameMM &
(MM& '
userIdMM' -
)MM- .
;MM. /
varNN 
urlNN 
=NN 
awaitNN 
_minioNN 
.NN #
PresignedPutObjectAsyncNN 6
(NN6 7
newOO "
PresignedPutObjectArgsOO &
(OO& '
)OO' (
.PP 

WithBucketPP 
(PP 
BucketPP "
)PP" #
.QQ 

WithObjectQQ 
(QQ 

objectNameQQ &
)QQ& '
.RR 

WithExpiryRR 
(RR 
$numRR 
*RR  
$numRR! #
)RR# $
)SS 	
;SS	 

returnTT 
OkTT 
(TT 
newTT 
{TT 
urlTT 
}TT 
)TT 
;TT 
}UU 
[ZZ 

HttpDeleteZZ 
(ZZ 
$strZZ 
)ZZ 
]ZZ 
public[[ 

async[[ 
Task[[ 
<[[ 
IActionResult[[ #
>[[# $
Delete[[% +
([[+ ,
Guid[[, 0
userId[[1 7
)[[7 8
{\\ 
if]] 

(]] 
!]] 
CheckUserId]] 
(]] 
userId]] 
)]]  
)]]  !
return^^ 
Unauthorized^^ 
(^^  
)^^  !
;^^! "
var__ 

objectName__ 
=__ 
GetObjectName__ &
(__& '
userId__' -
)__- .
;__. /
await`` 
_minio`` 
.`` 
RemoveObjectAsync`` &
(``& '
newaa 
RemoveObjectArgsaa  
(aa  !
)aa! "
.bb 

WithBucketbb 
(bb 
Bucketbb "
)bb" #
.cc 

WithObjectcc 
(cc 

objectNamecc &
)cc& '
)dd 	
;dd	 

returnff 
Okff 
(ff 
)ff 
;ff 
}gg 
}hh Õ
eC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Api\Controllers\UserController.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Controllers" -
;- .
[		 
ApiController		 
]		 
[

 
Route

 
(

 
$str

 
)

 
]

 
[ 
	Authorize 

(
 !
AuthenticationSchemes  
=! "
JwtBearerDefaults# 4
.4 5 
AuthenticationScheme5 I
)I J
]J K
public 
class 
UserController 
: 

Controller (
{ 
private 
readonly 
IUserService !
_userService" .
;. /
public 

UserController 
( 
IUserService &
userService' 2
)2 3
{ 
_userService 
= 
userService "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
User 
? 
> 
GetProfileByIdAsync 0
(0 1
Guid1 5
userId6 <
)< =
{ 
return 
await 
_userService !
.! "
GetProfileByIdAsync" 5
(5 6
userId6 <
)< =
;= >
} 
} ˚
eC:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Api\Controllers\AuthController.cs
	namespace 	
AuthService
 
. 
Application !
.! "
Controllers" -
;- .
[		 
ApiController		 
]		 
[

 
Route

 
(

 
$str

 
)

 
]

 
[ 
AllowAnonymous 
] 
public 
class 
AuthController 
: 

Controller (
{ 
private 
readonly 
IUserService !
_userService" .
;. /
public 

AuthController 
( 
IUserService &
userService' 2
)2 3
{ 
_userService 
= 
userService "
;" #
} 
[ 
HttpPost 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
Register% -
(- .
[. /
FromBody/ 7
]7 8#
RegisterRequestContract9 P
registerRequestQ `
)` a
{ 
return 
await 
_userService !
.! "
RegisterAsync" /
(/ 0
registerRequest0 ?
)? @
;@ A
} 
[ 
HttpPost 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
Login% *
(* +
[+ ,
FromBody, 4
]4 5
LoginRequest6 B
loginRequestC O
)O P
{ 
var   
response   
=   
await   
_userService   )
.  ) *

LoginAsync  * 4
(  4 5
loginRequest  5 A
)  A B
;  B C
if!! 

(!! 
response!! 
==!! 
null!! 
)!! 
return!! $
Unauthorized!!% 1
(!!1 2
)!!2 3
;!!3 4
return"" 
Ok"" 
("" 
response"" 
)"" 
;"" 
}## 
}%% Ä
[C:\Users\demde\Desktop\Worky\Worky\Services\AuthService\AuthService.Api\AuthDIExtensions.cs
	namespace		 	
AuthService		
 
.		 
Api		 
.		 

Extentions		 $
;		$ %
public 
static 
class 
AuthDIExtensions $
{ 
public 

static 
IServiceCollection $
AddAuthServices% 4
(4 5
this5 9
IServiceCollection: L
servicesM U
,U V 
ConfigurationManagerW k
configurationl y
)y z
{ 
services 
. 
	AddScoped 
< 
IUserRepository *
,* +
UserRepository, :
>: ;
(; <
)< =
;= >
services 
. 
	AddScoped 
< 
IUserService '
,' (
UserService) 4
>4 5
(5 6
)6 7
;7 8
services 
. 
	AddScoped 
< 
IJwtService &
,& '

JwtService( 2
>2 3
(3 4
)4 5
;5 6
services 
. 
AddSingleton 
<  
KafkaProducerFactory 2
>2 3
(3 4
)4 5
;5 6
services 
. 
	AddScoped 
< 
IOutboxPublisher +
,+ , 
KafkaOutboxPublisher- A
>A B
(B C
)C D
;D E
services 
. 
AddHostedService !
<! "
OutboxPublisWorker" 4
>4 5
(5 6
)6 7
;7 8
services 
. 
AddSingleton 
< 
IMinioClient *
>* +
(+ ,
sp, .
=>/ 1
{ 	
var 
config 
= 
configuration &
.& '

GetSection' 1
(1 2
$str2 9
)9 :
;: ;
return 
new 
MinioClient "
(" #
)# $
. 
WithEndpoint 
( 
config $
[$ %
$str% /
]/ 0
)0 1
. 
WithCredentials  
(  !
config! '
[' (
$str( 3
]3 4
,4 5
config6 <
[< =
$str= H
]H I
)I J
. 
WithSSL 
( 
false 
) 
. 
Build 
( 
) 
; 
}   	
)  	 

;  
 
services!! 
.!! 
AddSingleton!! 
<!! "
MinioBucketInitializer!! 4
>!!4 5
(!!5 6
)!!6 7
;!!7 8
return## 
services## 
;## 
}$$ 
}%% 