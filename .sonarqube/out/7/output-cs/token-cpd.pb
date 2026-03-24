ö

WC:\Users\demde\Desktop\Worky\Worky\Services\ApiGateway\ApiGateway\PublicRouteService.cs
	namespace 	

ApiGateway
 
. 
Service 
; 
public 
class 
PublicRouteService 
:  !
IPublicRouteService" 5
{ 
private 
readonly 
HashSet 
< 
string #
># $
_publicRoutes% 2
;2 3
public		 

PublicRouteService		 
(		 
IConfiguration		 ,
configuration		- :
)		: ;
{

 
_publicRoutes 
= 
configuration %
.% &

GetSection& 0
(0 1
$str1 ?
)? @
.@ A
GetA D
<D E
stringE K
[K L
]L M
>M N
(N O
)O P
?P Q
.Q R
	ToHashSetR [
([ \
)\ ]
??^ `
new 
HashSet #
<# $
string$ *
>* +
(+ ,
), -
;- .
} 
public 

bool 
IsPublicRoute 
( 
string $
route% *
)* +
{ 
return 
true 
; 
} 
} òA
LC:\Users\demde\Desktop\Worky\Worky\Services\ApiGateway\ApiGateway\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
var 
connectionString 
= 
builder 
. 
Configuration ,
., -
GetConnectionString- @
(@ A
$strA T
)T U
??V X
throw 
new  %
InvalidOperationException! :
(: ;
$str; m
)m n
;n o
builder 
. 
Services 
. 3
'AddDatabaseDeveloperPageExceptionFilter 8
(8 9
)9 :
;: ;
builder 
. 
Services 
. 
AddCors 
( 
options  
=>! #
{ 
options 
. 
	AddPolicy 
( 
$str  
,  !
policy" (
=>) +
{ 
policy 
. 
WithOrigins 
( 
$str 0
)0 1
. 
AllowAnyHeader 
( 
) 
. 
AllowAnyMethod 
( 
) 
. 
AllowCredentials 
( 
) 
;  
} 
) 
; 
} 
) 
; 
builder 
. 
Services 
. 
AddAuthentication "
(" #
options# *
=>+ -
{   
options!! 
.!! %
DefaultAuthenticateScheme!! )
=!!* +
JwtBearerDefaults!!, =
.!!= > 
AuthenticationScheme!!> R
;!!R S
options"" 
."" 
DefaultSignInScheme"" #
=""$ %
JwtBearerDefaults""& 7
.""7 8 
AuthenticationScheme""8 L
;""L M
options## 
.## "
DefaultChallengeScheme## &
=##' (
JwtBearerDefaults##) :
.##: ; 
AuthenticationScheme##; O
;##O P
}%% 
)%% 
.&& 
AddJwtBearer&& 
(&& 
options&& 
=>&& 
{'' 
options** 
.** %
TokenValidationParameters** )
=*** +
new**, /%
TokenValidationParameters**0 I
{++ 	
ValidateIssuer-- 
=-- 
true-- !
,--! "
ValidIssuer// 
=// 
builder// !
.//! "
Configuration//" /
[/// 0
$str//0 <
]//< =
,//= >
ValidateAudience11 
=11 
true11 #
,11# $
ValidAudience33 
=33 
builder33 #
.33# $
Configuration33$ 1
[331 2
$str332 @
]33@ A
,33A B
ValidateLifetime55 
=55 
true55 #
,55# $
IssuerSigningKey77 
=77 
new77 " 
SymmetricSecurityKey77# 7
(777 8
Encoding778 @
.77@ A
UTF877A E
.77E F
GetBytes77F N
(77N O
builder77O V
.77V W
Configuration77W d
[77d e
$str77e n
]77n o
!77o p
)77p q
)77q r
,77r s$
ValidateIssuerSigningKey99 $
=99% &
true99' +
,99+ ,
}:: 	
;::	 

options<< 
.<< 
Events<< 
=<< 
new<< 
JwtBearerEvents<< ,
{== 	
OnChallenge>> 
=>> 
context>> !
=>>>" $
{?? 
context@@ 
.@@ 
HandleResponse@@ &
(@@& '
)@@' (
;@@( )
contextAA 
.AA 
ResponseAA  
.AA  !

StatusCodeAA! +
=AA, -
$numAA. 1
;AA1 2
contextBB 
.BB 
ResponseBB  
.BB  !
HeadersBB! (
.BB( )
AddBB) ,
(BB, -
$strBB- 7
,BB7 8
contextBB9 @
.BB@ A
HttpContextBBA L
.BBL M
TraceIdentifierBBM \
)BB\ ]
;BB] ^
returnCC 
contextCC 
.CC 
ResponseCC '
.CC' (
WriteAsJsonAsyncCC( 8
(CC8 9
newCC9 <
{CC= >
messageCC? F
=CCG H
$strCCI f
}CCg h
,CCh i!
JsonSerializerOptionsDD )
.DD) *
DefaultDD* 1
)DD1 2
;DD2 3
}EE 
,EE 
OnForbiddenFF 
=FF 
contextFF !
=>FF" $
{GG 
contextHH 
.HH 
NoResultHH  
(HH  !
)HH! "
;HH" #
contextII 
.II 
ResponseII  
.II  !

StatusCodeII! +
=II, -
$numII. 1
;II1 2
contextJJ 
.JJ 
ResponseJJ  
.JJ  !
HeadersJJ! (
.JJ( )
AddJJ) ,
(JJ, -
$strJJ- 7
,JJ7 8
contextJJ9 @
.JJ@ A
HttpContextJJA L
.JJL M
TraceIdentifierJJM \
)JJ\ ]
;JJ] ^
returnKK 
contextKK 
.KK 
ResponseKK '
.KK' (
WriteAsJsonAsyncKK( 8
(KK8 9
newKK9 <
{KK= >
messageKK? F
=KKG H
$strKKI n
}KKo p
,KKp q!
JsonSerializerOptionsLL )
.LL) *
DefaultLL* 1
)LL1 2
;LL2 3
}MM 
}NN 	
;NN	 

}OO 
)OO 
;OO 
builderPP 
.PP 
ServicesPP 
.PP 
AddControllersPP 
(PP  
)PP  !
;PP! "
builderQQ 
.QQ 
ServicesQQ 
.QQ 
AddAuthorizationQQ !
(QQ! "
)QQ" #
;QQ# $
builderSS 
.SS 
ServicesSS 
.SS 
AddReverseProxySS  
(SS  !
)SS! "
.SS" #
LoadFromConfigTT 
(TT 
builderTT 
.TT 
ConfigurationTT (
.TT( )

GetSectionTT) 3
(TT3 4
$strTT4 B
)TTB C
)TTC D
;TTD E
builderVV 
.VV 
ServicesVV 
.VV 
AddJwtValidationVV !
(VV! "
builderVV" )
.VV) *
ConfigurationVV* 7
)VV7 8
;VV8 9
varXX 
appXX 
=XX 	
builderXX
 
.XX 
BuildXX 
(XX 
)XX 
;XX 
if[[ 
([[ 
app[[ 
.[[ 
Environment[[ 
.[[ 
IsDevelopment[[ !
([[! "
)[[" #
)[[# $
{\\ 
app]] 
.]] !
UseMigrationsEndPoint]] 
(]] 
)]] 
;]]  
}^^ 
else__ 
{`` 
appaa 
.aa 
UseExceptionHandleraa 
(aa 
$straa )
)aa) *
;aa* +
appcc 
.cc 
UseHstscc 
(cc 
)cc 
;cc 
}dd 
appff 
.ff 
UseHttpsRedirectionff 
(ff 
)ff 
;ff 
appgg 
.gg 

UseRoutinggg 
(gg 
)gg 
;gg 
appii 
.ii 
UseCorsii 
(ii 
$strii 
)ii 
;ii 
appkk 
.kk 
UseAuthenticationkk 
(kk 
)kk 
;kk 
appll 
.ll 
UseAuthorizationll 
(ll 
)ll 
;ll 
app„„ 
.
„„ 
UseMiddleware
„„ 
<
„„ %
JwtValidationMiddleware
„„ )
>
„„) *
(
„„* +
)
„„+ ,
;
„„, -
app†† 
.
†† 
MapReverseProxy
†† 
(
†† 
)
†† 
;
†† 
app‰‰ 
.
‰‰ 
Run
‰‰ 
(
‰‰ 
)
‰‰ 	
;
‰‰	 
˜
^C:\Users\demde\Desktop\Worky\Worky\Services\ApiGateway\ApiGateway\Models\PublicRoutesConfig.cs
	namespace 	

ApiGateway
 
. 
Models 
; 
public 
class 
PublicRoutesConfig 
{ 
public 

string 
[ 
] 
PublicRoutes  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
[1 2
]2 3
;3 4
} Õ
\C:\Users\demde\Desktop\Worky\Worky\Services\ApiGateway\ApiGateway\JwtValidationMiddleware.cs
	namespace 	

ApiGateway
 
. 

Middleware 
;  
public 
class #
JwtValidationMiddleware $
{ 
private

 
readonly

 
RequestDelegate

 $
_next

% *
;

* +
private 
readonly 
IPublicRouteService (
_publicRouteService) <
;< =
public 
#
JwtValidationMiddleware "
(" #
IPublicRouteService# 6
publicRouteService7 I
,I J
RequestDelegateK Z
next[ _
)_ `
{ 
_publicRouteService 
= 
publicRouteService 0
;0 1
_next 
= 
next 
; 
} 
public 

async 
Task 
InvokeAsync !
(! "
HttpContext" -
context. 5
)5 6
{ 
string 
? 
path 
= 
context 
. 
Request &
.& '
Path' +
.+ ,
Value, 1
?1 2
.2 3
ToLower3 :
(: ;
); <
;< =
if 

( 
_publicRouteService 
.  
IsPublicRoute  -
(- .
path. 2
)2 3
)3 4
{ 	
await 
_next 
. 
Invoke 
( 
context &
)& '
;' (
return 
; 
} 	
if 

( 
! 
context 
. 
User 
. 
Identity "
!" #
.# $
IsAuthenticated$ 3
)3 4
{ 	
context 
. 
Response 
. 

StatusCode '
=( )
$num* -
;- .
await   
context   
.   
Response   "
.  " #

WriteAsync  # -
(  - .
$str  . <
)  < =
;  = >
return!! 
;!! 
}"" 	
await$$ 
_next$$ 
.$$ 
Invoke$$ 
($$ 
context$$ "
)$$" #
;$$# $
}%% 
}'' ä
TC:\Users\demde\Desktop\Worky\Worky\Services\ApiGateway\ApiGateway\JwtValidationDI.cs
	namespace 	

ApiGateway
 
. 
DI 
; 
public 
static 
class 
JwtValidationDI #
{ 
public		 

static		 
IServiceCollection		 $
AddJwtValidation		% 5
(		5 6
this		6 :
IServiceCollection		; M
services		N V
,		V W
IConfiguration		X f
configuration		g t
)		t u
{

 
services 
. 
AddSingleton 
< 
IPublicRouteService 1
,1 2
PublicRouteService3 E
>E F
(F G
)G H
;H I
return 
services 
; 
} 
} Å
XC:\Users\demde\Desktop\Worky\Worky\Services\ApiGateway\ApiGateway\IPublicRouteService.cs
	namespace 	

ApiGateway
 
. 
service 
. 
	Interface &
;& '
public 
	interface 
IPublicRouteService $
{ 
bool 
IsPublicRoute	 
( 
string 
route #
)# $
;$ %
} 