ãå
XC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.Api\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder 
. 
Services 
. 
AddControllers 
(  
)  !
;! "
builder 
. 
Services 
. 
AddDbContext 
< 
CompanyDbContext .
>. /
(/ 0
options0 7
=>8 :
{ 
options 
. 
	UseNpgsql 
( 
builder 
. 
Configuration +
.+ ,
GetConnectionString, ?
(? @
$str@ S
)S T
,T U
npgsql 
=> 
npgsql 
. 
MigrationsAssembly +
(+ ,
$str, @
)@ A
)A B
;B C
} 
) 
; 
var !
redisConnectionString 
= 
builder #
.# $
Configuration$ 1
.1 2
GetConnectionString2 E
(E F
$strF M
)M N
;N O
builder 
. 
Services 
. 
AddSingleton 
< "
IConnectionMultiplexer 4
>4 5
(5 6
sp6 8
=>9 ;
{ 
return 
!
ConnectionMultiplexer  
.  !
Connect! (
(( )!
redisConnectionString) >
)> ?
;? @
} 
) 
; 
var!! 
key!! 
=!! 	
Encoding!!
 
.!! 
UTF8!! 
.!! 
GetBytes!!  
(!!  !
builder!!! (
.!!( )
Configuration!!) 6
[!!6 7
$str!!7 @
]!!@ A
)!!A B
;!!B C
var"" 

signingKey"" 
="" 
new""  
SymmetricSecurityKey"" )
("") *
key""* -
)""- .
;"". /
builder## 
.## 
Services## 
.## 
AddSingleton## 
(## 

signingKey## (
)##( )
;##) *
builder$$ 
.$$ 
Services$$ 
.$$ 
AddAuthentication$$ "
($$" #
options$$# *
=>$$+ -
{%% 
options&& 
.&& %
DefaultAuthenticateScheme&& )
=&&* +
JwtBearerDefaults&&, =
.&&= > 
AuthenticationScheme&&> R
;&&R S
options'' 
.'' 
DefaultSignInScheme'' #
=''$ %
JwtBearerDefaults''& 7
.''7 8 
AuthenticationScheme''8 L
;''L M
options(( 
.(( "
DefaultChallengeScheme(( &
=((' (
JwtBearerDefaults(() :
.((: ; 
AuthenticationScheme((; O
;((O P
}** 
)** 
.++ 
AddJwtBearer++ 
(++ 
options++ 
=>++ 
{,, 
options// 
.// %
TokenValidationParameters// )
=//* +
new//, /%
TokenValidationParameters//0 I
{00 	
ValidateIssuer22 
=22 
true22 !
,22! "
ValidIssuer44 
=44 
builder44 !
.44! "
Configuration44" /
[44/ 0
$str440 <
]44< =
,44= >
ValidateAudience66 
=66 
true66 #
,66# $
ValidAudience88 
=88 
builder88 #
.88# $
Configuration88$ 1
[881 2
$str882 @
]88@ A
,88A B
ValidateLifetime:: 
=:: 
true:: #
,::# $
IssuerSigningKey<< 
=<< 
new<< " 
SymmetricSecurityKey<<# 7
(<<7 8
Encoding<<8 @
.<<@ A
UTF8<<A E
.<<E F
GetBytes<<F N
(<<N O
builder<<O V
.<<V W
Configuration<<W d
[<<d e
$str<<e n
]<<n o
!<<o p
)<<p q
)<<q r
,<<r s$
ValidateIssuerSigningKey>> $
=>>% &
true>>' +
,>>+ ,
}?? 	
;??	 

optionsAA 
.AA 
EventsAA 
=AA 
newAA 
JwtBearerEventsAA ,
{BB 	
OnChallengeCC 
=CC 
contextCC !
=>CC" $
{DD 
contextEE 
.EE 
HandleResponseEE &
(EE& '
)EE' (
;EE( )
contextFF 
.FF 
ResponseFF  
.FF  !

StatusCodeFF! +
=FF, -
$numFF. 1
;FF1 2
contextGG 
.GG 
ResponseGG  
.GG  !
HeadersGG! (
.GG( )
AddGG) ,
(GG, -
$strGG- 7
,GG7 8
contextGG9 @
.GG@ A
HttpContextGGA L
.GGL M
TraceIdentifierGGM \
)GG\ ]
;GG] ^
returnHH 
contextHH 
.HH 
ResponseHH '
.HH' (
WriteAsJsonAsyncHH( 8
(HH8 9
newHH9 <
{HH= >
messageHH? F
=HHG H
$strHHI f
}HHg h
,HHh i!
JsonSerializerOptionsII )
.II) *
DefaultII* 1
)II1 2
;II2 3
}JJ 
,JJ 
OnForbiddenKK 
=KK 
contextKK !
=>KK" $
{LL 
contextMM 
.MM 
NoResultMM  
(MM  !
)MM! "
;MM" #
contextNN 
.NN 
ResponseNN  
.NN  !

StatusCodeNN! +
=NN, -
$numNN. 1
;NN1 2
contextOO 
.OO 
ResponseOO  
.OO  !
HeadersOO! (
.OO( )
AddOO) ,
(OO, -
$strOO- 7
,OO7 8
contextOO9 @
.OO@ A
HttpContextOOA L
.OOL M
TraceIdentifierOOM \
)OO\ ]
;OO] ^
returnPP 
contextPP 
.PP 
ResponsePP '
.PP' (
WriteAsJsonAsyncPP( 8
(PP8 9
newPP9 <
{PP= >
messagePP? F
=PPG H
$strPPI n
}PPo p
,PPp q!
JsonSerializerOptionsQQ )
.QQ) *
DefaultQQ* 1
)QQ1 2
;QQ2 3
}RR 
}SS 	
;SS	 

}TT 
)TT 
;TT 
builderUU 
.UU 
ServicesUU 
.UU 
AddAuthorizationUU !
(UU! "
)UU" #
;UU# $
builderWW 
.WW 
ServicesWW 
.WW 
AddSwaggerGenWW 
(WW 
cWW  
=>WW! #
{XX 
tryYY 
{ZZ 
c[[ 	
.[[	 


SwaggerDoc[[
 
([[ 
$str[[ 
,[[ 
new[[ 
OpenApiInfo[[ *
{[[+ ,
Title[[- 2
=[[3 4
$str[[5 <
,[[< =
Version[[> E
=[[F G
$str[[H L
}[[M N
)[[N O
;[[O P
c^^ 	
.^^	 
!
AddSecurityDefinition^^
 
(^^  
$str^^  (
,^^( )
new^^* -!
OpenApiSecurityScheme^^. C
{__ 	
In`` 
=`` 
ParameterLocation`` "
.``" #
Header``# )
,``) *
Descriptionaa 
=aa 
$straa C
,aaC D
Namebb 
=bb 
$strbb "
,bb" #
Typecc 
=cc 
SecuritySchemeTypecc %
.cc% &
ApiKeycc& ,
,cc, -
Schemedd 
=dd 
$strdd 
}ee 	
)ee	 

;ee
 
chh 	
.hh	 
"
AddSecurityRequirementhh
  
(hh  !
newhh! $&
OpenApiSecurityRequirementhh% ?
(hh? @
)hh@ A
{ii 	
{jj 
newkk !
OpenApiSecuritySchemekk )
{ll 
	Referencemm 
=mm 
newmm  #
OpenApiReferencemm$ 4
{nn 
Typeoo 
=oo 
ReferenceTypeoo ,
.oo, -
SecuritySchemeoo- ;
,oo; <
Idpp 
=pp 
$strpp %
}qq 
,qq 
Schemerr 
=rr 
$strrr %
,rr% &
Namess 
=ss 
$strss #
,ss# $
Intt 
=tt 
ParameterLocationtt *
.tt* +
Headertt+ 1
,tt1 2
}uu 
,uu 
newvv 
Listvv 
<vv 
stringvv 
>vv  
(vv  !
)vv! "
}ww 
}xx 	
)xx	 

;xx
 
}yy 
catchzz 	
(zz
 
	Exceptionzz 
exzz 
)zz 
{{{ 
Console|| 
.|| 
	WriteLine|| 
(|| 
$str|| (
+||) *
ex||+ -
.||- .
Message||. 5
)||5 6
;||6 7
throw}} 
;}} 
}~~ 
} 
) 
; 
builderÅÅ 
.
ÅÅ 
Services
ÅÅ 
.
ÅÅ 
AddMassTransit
ÅÅ 
(
ÅÅ  
config
ÅÅ  &
=>
ÅÅ' )
{ÇÇ 
config
ÉÉ 

.
ÉÉ
 
AddConsumer
ÉÉ 
<
ÉÉ (
UserCompanyCreatedConsumer
ÉÉ 1
>
ÉÉ1 2
(
ÉÉ2 3
)
ÉÉ3 4
;
ÉÉ4 5
config
ÖÖ 

.
ÖÖ
 &
AddEntityFrameworkOutbox
ÖÖ #
<
ÖÖ# $
CompanyDbContext
ÖÖ$ 4
>
ÖÖ4 5
(
ÖÖ5 6
o
ÖÖ6 7
=>
ÖÖ8 :
{
ÜÜ 
o
àà 	
.
àà	 

UsePostgres
àà
 
(
àà 
)
àà 
.
àà 
UseBusOutbox
àà $
(
àà$ %
)
àà% &
;
àà& '
}
ââ 
)
ââ 
;
ââ 
config
ãã 

.
ãã
 
UsingInMemory
ãã 
(
ãã 
(
ãã 
context
ãã !
,
ãã! "
cfg
ãã# &
)
ãã& '
=>
ãã( *
{
åå 
cfg
çç 
.
çç  
ConfigureEndpoints
çç 
(
çç 
context
çç &
)
çç& '
;
çç' (
}
éé 
)
éé 
;
éé 
config
êê 

.
êê
 
AddRider
êê 
(
êê 
rider
êê 
=>
êê 
{
ëë 
rider
íí 
.
íí 
AddProducer
íí 
<
íí *
UserCompanyCreateFailedEvent
íí 6
>
íí6 7
(
íí7 8
$str
íí8 S
)
ííS T
;
ííT U
rider
ìì 
.
ìì 
AddProducer
ìì 
<
ìì !
VacancyCreatedEvent
ìì -
>
ìì- .
(
ìì. /
$str
ìì/ @
)
ìì@ A
;
ììA B
rider
îî 
.
îî 
AddProducer
îî 
<
îî !
VacancyUpdatedEvent
îî -
>
îî- .
(
îî. /
$str
îî/ @
)
îî@ A
;
îîA B
rider
ïï 
.
ïï 
AddProducer
ïï 
<
ïï !
VacancyDeletedEvent
ïï -
>
ïï- .
(
ïï. /
$str
ïï/ @
)
ïï@ A
;
ïïA B
rider
óó 
.
óó 
AddProducer
óó 
<
óó #
VacancyFilterAddEvent
óó /
>
óó/ 0
(
óó0 1
$str
óó1 E
)
óóE F
;
óóF G
rider
òò 
.
òò 
AddProducer
òò 
<
òò &
VacancyFilterDeleteEvent
òò 2
>
òò2 3
(
òò3 4
$str
òò4 K
)
òòK L
;
òòL M
rider
öö 
.
öö 
AddConsumer
öö 
<
öö (
UserCompanyCreatedConsumer
öö 4
>
öö4 5
(
öö5 6
)
öö6 7
;
öö7 8
rider
úú 
.
úú 

UsingKafka
úú 
(
úú 
(
úú 
context
úú !
,
úú! "
k
úú# $
)
úú$ %
=>
úú& (
{
ùù 	#
IConfigurationSection
ûû !
kafkaSettings
ûû" /
=
ûû0 1
builder
ûû2 9
.
ûû9 :
Configuration
ûû: G
.
ûûG H

GetSection
ûûH R
(
ûûR S
$str
ûûS Z
)
ûûZ [
;
ûû[ \
string
üü 
bootstrapServers
üü #
=
üü$ %
kafkaSettings
üü& 3
[
üü3 4
$str
üü4 F
]
üüF G
;
üüG H
k
†† 
.
†† 
Host
†† 
(
†† 
bootstrapServers
†† #
)
††# $
;
††$ %
k
££ 
.
££ 
TopicEndpoint
££ 
<
££ %
UserCompanyCreatedEvent
££ 3
>
££3 4
(
££4 5
$str
££5 K
,
££K L
$str
££M d
,
££d e
e
££f g
=>
££h j
{
§§ 
e
•• 
.
•• 
AutoOffsetReset
•• !
=
••" #
	Confluent
••$ -
.
••- .
Kafka
••. 3
.
••3 4
AutoOffsetReset
••4 C
.
••C D
Earliest
••D L
;
••L M
e
¶¶ 
.
¶¶ 
ConfigureConsumer
¶¶ #
<
¶¶# $(
UserCompanyCreatedConsumer
¶¶$ >
>
¶¶> ?
(
¶¶? @
context
¶¶@ G
)
¶¶G H
;
¶¶H I
e
ßß 
.
ßß 
CreateIfMissing
ßß !
(
ßß! "
)
ßß" #
;
ßß# $
}
®® 
)
®® 
;
®® 
}
©© 	
)
©©	 

;
©©
 
}
™™ 
)
™™ 
;
™™ 
}´´ 
)
´´ 
;
´´ 
builder≠≠ 
.
≠≠ 
Services
≠≠ 
.
≠≠ (
AddStackExchangeRedisCache
≠≠ +
(
≠≠+ ,
options
≠≠, 3
=>
≠≠4 6
{ÆÆ 
string
ØØ 

con
ØØ 
=
ØØ 
builder
ØØ 
.
ØØ 
Configuration
ØØ &
.
ØØ& '!
GetConnectionString
ØØ' :
(
ØØ: ;
$str
ØØ; B
)
ØØB C
;
ØØC D
options
∞∞ 
.
∞∞ 
Configuration
∞∞ 
=
∞∞ 
con
∞∞ 
;
∞∞  
options
±± 
.
±± 
InstanceName
±± 
=
±± 
$str
±± +
;
±±+ ,
}≤≤ 
)
≤≤ 
;
≤≤ 
builder¥¥ 
.
¥¥ 
Services
¥¥ 
.
¥¥ 
AddCompanyService
¥¥ "
(
¥¥" #
)
¥¥# $
;
¥¥$ %
builderµµ 
.
µµ 
Services
µµ 
.
µµ #
AddExternalHttpClient
µµ &
(
µµ& '
builder
µµ' .
.
µµ. /
Configuration
µµ/ <
)
µµ< =
;
µµ= >
var∑∑ 
app
∑∑ 
=
∑∑ 	
builder
∑∑
 
.
∑∑ 
Build
∑∑ 
(
∑∑ 
)
∑∑ 
;
∑∑ 
if∫∫ 
(
∫∫ 
app
∫∫ 
.
∫∫ 
Environment
∫∫ 
.
∫∫ 
IsDevelopment
∫∫ !
(
∫∫! "
)
∫∫" #
)
∫∫# $
{ªª 
app
ºº 
.
ºº 

MapOpenApi
ºº 
(
ºº 
)
ºº 
;
ºº 
}ΩΩ 
app¡¡ 
.
¡¡ 
UseCors
¡¡ 
(
¡¡ 
)
¡¡ 
;
¡¡ 
app¬¬ 
.
¬¬ 

UseRouting
¬¬ 
(
¬¬ 
)
¬¬ 
;
¬¬ 
app√√ 
.
√√ 
MapControllers
√√ 
(
√√ 
)
√√ 
;
√√ 
appƒƒ 
.
ƒƒ !
UseHttpsRedirection
ƒƒ 
(
ƒƒ 
)
ƒƒ 
;
ƒƒ 
app≈≈ 
.
≈≈ 
UseAuthentication
≈≈ 
(
≈≈ 
)
≈≈ 
;
≈≈ 
app∆∆ 
.
∆∆ 
UseAuthorization
∆∆ 
(
∆∆ 
)
∆∆ 
;
∆∆ 
app«« 
.
«« 

UseSwagger
«« 
(
«« 
)
«« 
;
«« 
app»» 
.
»» 
UseSwaggerUI
»» 
(
»» 
c
»» 
=>
»» 
{…… 
c
   
.
   
SwaggerEndpoint
   
(
   
$str
   0
,
  0 1
$str
  2 H
)
  H I
;
  I J
c
ÀÀ 
.
ÀÀ 
RoutePrefix
ÀÀ 
=
ÀÀ 
string
ÀÀ 
.
ÀÀ 
Empty
ÀÀ  
;
ÀÀ  !
}ÃÃ 
)
ÃÃ 
;
ÃÃ 
usingŒŒ 
(
ŒŒ 
var
ŒŒ 

serviceScope
ŒŒ 
=
ŒŒ 
app
ŒŒ 
.
ŒŒ 
Services
ŒŒ &
.
ŒŒ& '
CreateScope
ŒŒ' 2
(
ŒŒ2 3
)
ŒŒ3 4
)
ŒŒ4 5
{œœ 
var
–– 
context
–– 
=
–– 
serviceScope
–– 
.
–– 
ServiceProvider
–– .
.
––. / 
GetRequiredService
––/ A
<
––A B
CompanyDbContext
––B R
>
––R S
(
––S T
)
––T U
;
––U V
context
—— 
.
—— 
Database
—— 
.
—— 
Migrate
—— 
(
—— 
)
—— 
;
—— 
}““ 
app‘‘ 
.
‘‘ 
Run
‘‘ 
(
‘‘ 
)
‘‘ 	
;
‘‘	 
‡&
qC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.Api\HttpClientRegistrationExtensions.cs
	namespace 	
CompanyService
 
. 
Api 
. 

Extentions '
;' (
public 
static 
class ,
 HttpClientRegistrationExtensions 4
{		 
public

 

static

 
IServiceCollection

 $!
AddExternalHttpClient

% :
(

: ;
this

; ?
IServiceCollection

@ R
services

S [
,

[ \
IConfiguration

] k
configuration

l y
)

y z
{ 
services 
. 
AddHttpClient 
< 
IAuthClient *
,* +

AuthClient, 6
>6 7
(7 8
client8 >
=>? A
{ 	
string 
baseUrl 
= 
configuration *
[* +
$str+ <
]< =
;= >
if 
( 
string 
. 
IsNullOrEmpty $
($ %
baseUrl% ,
), -
)- .
throw 
new %
InvalidOperationException 3
(3 4
$str4 X
)X Y
;Y Z
client 
. 
BaseAddress 
=  
new! $
Uri% (
(( )
baseUrl) 0
)0 1
;1 2
client 
. 
Timeout 
= 
TimeSpan %
.% &
FromSeconds& 1
(1 2
$num2 4
)4 5
;5 6
} 	
)	 

. 
AddPolicyHandler 
( 
GetRetryPolicy ,
(, -
)- .
). /
. 
AddPolicyHandler 
( #
GetCircuitBreakerPolicy 5
(5 6
)6 7
)7 8
;8 9
services 
. 
AddHttpClient 
< 
IFilterClient ,
,, -
FilterClient. :
>: ;
(; <
client< B
=>C E
{ 	
string 
baseUrl 
= 
configuration *
[* +
$str+ >
]> ?
;? @
if 
( 
string 
. 
IsNullOrEmpty $
($ %
baseUrl% ,
), -
)- .
throw 
new %
InvalidOperationException 3
(3 4
$str4 X
)X Y
;Y Z
client 
. 
BaseAddress 
=  
new! $
Uri% (
(( )
baseUrl) 0
)0 1
;1 2
client   
.   
Timeout   
=   
TimeSpan   %
.  % &
FromSeconds  & 1
(  1 2
$num  2 4
)  4 5
;  5 6
}!! 	
)!!	 

."" 
AddPolicyHandler"" 
("" 
GetRetryPolicy"" ,
("", -
)""- .
)"". /
.## 
AddPolicyHandler## 
(## #
GetCircuitBreakerPolicy## 5
(##5 6
)##6 7
)##7 8
;##8 9
;##9 :
return&& 
services&& 
;&& 
}'' 
private)) 
static)) 
IAsyncPolicy)) 
<))  
HttpResponseMessage))  3
>))3 4
GetRetryPolicy))5 C
())C D
)))D E
=>))F H 
HttpPolicyExtensions** 
.++ $
HandleTransientHttpError++ %
(++% &
)++& '
.,, 
WaitAndRetryAsync,, 
(,, 

retryCount-- 
:-- 
$num-- 
,-- !
sleepDurationProvider.. %
:..% &
retryAttempt..' 3
=>..4 6
TimeSpan..7 ?
...? @
FromSeconds..@ K
(..K L
Math..L P
...P Q
Pow..Q T
(..T U
$num..U V
,..V W
retryAttempt..X d
)..d e
)..e f
)// 
;// 
private11 
static11 
IAsyncPolicy11 
<11  
HttpResponseMessage11  3
>113 4#
GetCircuitBreakerPolicy115 L
(11L M
)11M N
=>11O Q 
HttpPolicyExtensions22 
.33 $
HandleTransientHttpError33 %
(33% &
)33& '
.44 
CircuitBreakerAsync44  
(44  !.
"handledEventsAllowedBeforeBreaking55 2
:552 3
$num554 5
,555 6
durationOfBreak66 
:66  
TimeSpan66! )
.66) *
FromSeconds66* 5
(665 6
$num666 8
)668 9
)77 
;77 
}88 ≠&
kC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.Api\Controllers\DealController.cs
	namespace

 	
CompanyService


 
.

 
Api

 
.

 
Controllers

 (
;

( )
[ 
ApiController 
] 
[ 
Route 
( 
$str 
) 
] 
[ 
	Authorize 

(
 
Roles 
= 
$str 
, !
AuthenticationSchemes 3
=4 5
JwtBearerDefaults6 G
.G H 
AuthenticationSchemeH \
)\ ]
]] ^
public 
class 
DealController 
: 

Controller (
{ 
private 
readonly 
ILogger 
< 
DealController +
>+ ,
_logger- 4
;4 5
private 
readonly 
IDealService !
_dealService" .
;. /
public 

DealController 
( 
ILogger !
<! "
DealController" 0
>0 1
logger2 8
,8 9
IDealService: F
dealServiceG R
)R S
{ 
_logger 
= 
logger 
; 
_dealService 
= 
dealService "
;" #
} 
[ 
AllowAnonymous 
] 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
	GetTarrif% .
(. /
[/ 0
	FromQuery0 9
]9 :
int; >
?> ?
tariffId@ H
)H I
{ 
try 
{ 	
if&& 
(&& 
tariffId&& 
.&& 
HasValue&& !
)&&! "
{'' 
Tarrif(( 
?(( 
tarrif(( 
=((  
await((! &
_dealService((' 3
.((3 4
	GetTariff((4 =
(((= >
tariffId((> F
)((F G
;((G H
return)) 
Ok)) 
()) 
new)) 
{)) 
tarrif))  &
=))' (
tarrif))) /
}))0 1
)))1 2
;))2 3
}** 
else++ 
{,, 
List-- 
<-- 
Tarrif-- 
>-- 
?-- 
tarrifs-- %
=--& '
await--( -
_dealService--. :
.--: ;
	GetTariff--; D
(--D E
)--E F
;--F G
return.. 
Ok.. 
(.. 
new.. 
{.. 
tarrifs..  '
=..( )
tarrifs..* 1
}..2 3
)..3 4
;..4 5
}// 
}11 	
catch22 
(22 
	Exception22 
ex22 
)22 
{33 	
_logger44 
.44 
LogError44 
(44 
ex44 
,44  
$str44! P
)44P Q
;44Q R
return55 

BadRequest55 
(55 
$num55 !
)55! "
;55" #
}66 	
}77 
[99 
HttpPost99 
(99 
$str99 
)99 
]99 
public:: 

async:: 
Task:: 
<:: 
IActionResult:: #
>::# $
MakeDeal::% -
(::- .
MakeDealRequest::. =
request::> E
)::E F
{;; 
try<< 
{== 	
stringOO 
	companyIdOO 
=OO 
UserOO #
.OO# $
FindFirstValueOO$ 2
(OO2 3

ClaimTypesOO3 =
.OO= >
NameIdentifierOO> L
)OOL M
!OOM N
;OON O
GuidPP 
idPP 
=PP 
awaitPP 
_dealServicePP (
.PP( )

CreateDealPP) 3
(PP3 4
requestPP4 ;
,PP; <
GuidPP= A
.PPA B
ParsePPB G
(PPG H
	companyIdPPH Q
)PPQ R
)PPR S
;PPS T
returnQQ 
OkQQ 
(QQ 
newQQ 
{QQ 
idQQ 
=QQ  
idQQ! #
}QQ$ %
)QQ% &
;QQ& '
}SS 	
catchTT 
(TT 
	ExceptionTT 
exTT 
)TT 
{UU 	
_loggerVV 
.VV 
LogErrorVV 
(VV 
exVV 
,VV  
$strVV! N
)VVN O
;VVO P
returnWW 

BadRequestWW 
(WW 
$numWW !
)WW! "
;WW" #
}XX 	
}YY 
}ZZ Ëé
nC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.Api\Controllers\CompanyController.cs
	namespace 	
CompanyService
 
. 
Api 
. 
Controllers (
{ 
[ 
	Authorize 
( 
Roles 
= 
$str  
,  !!
AuthenticationSchemes" 7
=8 9
JwtBearerDefaults: K
.K L 
AuthenticationSchemeL `
)` a
]a b
[ 
ApiController 
] 
[ 
Route 

(
 
$str  
)  !
]! "
public 

class 
CompanyController "
:# $

Controller% /
{ 
private 
readonly 
ICompnayService (
_companyService) 8
;8 9
private 
readonly 
ICompanyRepository +
_companyRepository, >
;> ?
private 
readonly 
ILogger  
<  !
CompanyController! 2
>2 3
_logger4 ;
;; <
public 
CompanyController  
(  !
ICompnayService! 0
companyService1 ?
,? @
ILoggerA H
<H I
CompanyControllerI Z
>Z [
logger\ b
,b c
ICompanyRepositorye w
companyRepository	x â
)
â ä
{ 	
_companyService 
= 
companyService ,
;, -
_logger 
= 
logger 
; 
_companyRepository 
=  
companyRepository! 2
;2 3
} 	
[ 	
AllowAnonymous	 
] 
[   	
HttpGet  	 
(   
$str   !
)  ! "
]  " #
public!! 
async!! 
Task!! 
<!! 
IActionResult!! '
>!!' (
GetVacancyInfo!!) 7
(!!7 8
[!!8 9
	FromQuery!!9 B
]!!B C
Guid!!D H
	vacancyId!!I R
)!!R S
{"" 	
try## 
{$$ 
VacancyDtos%% 
vacancy%% #
=%%$ %
await%%& +
_companyService%%, ;
.%%; <
GetVacancyInfoAsync%%< O
(%%O P
	vacancyId%%P Y
)%%Y Z
;%%Z [
return&& 
Ok&& 
(&& 
new&& 
{&& 
vacancy&&  '
}&&( )
)&&) *
;&&* +
}'' 
catch(( 
((( 
	Exception(( 
ex(( 
)((  
{)) 
_logger** 
.** 
LogError**  
(**  !
ex**! #
,**# $
$str**% >
)**> ?
;**? @
return++ 

BadRequest++ !
(++! "
$num++" %
)++% &
;++& '
},, 
}-- 	
[00 	
HttpGet00	 
(00 
$str00 
)00 
]00 
public11 
async11 
Task11 
<11 
IActionResult11 '
>11' (
GetMyVacancy11) 5
(115 6
[116 7
	FromQuery117 @
]11@ A
Guid11B F
?11F G
	vacancyId11H Q
)11Q R
{22 	
try33 
{44 
string55 
	companyId55  
=55! "
User55# '
.55' (
FindFirstValue55( 6
(556 7

ClaimTypes557 A
.55A B
NameIdentifier55B P
)55P Q
;55Q R
IEnumerable66 
<66 
VacancyDtos66 '
>66' (
	vacancies66) 2
=663 4
await665 :
_companyService66; J
.66J K
GetMyVacanciesAsync66K ^
(66^ _
Guid66_ c
.66c d
Parse66d i
(66i j
	companyId66j s
)66s t
,66t u
	vacancyId66v 
)	66 Ä
;
66Ä Å
return77 
Ok77 
(77 
new77 
{77 
	vacancies77  )
}77* +
)77+ ,
;77, -
}88 
catch99 
(99 
	Exception99 
ex99 
)99  
{:: 
_logger;; 
.;; 
LogError;;  
(;;  !
ex;;! #
,;;# $
$str;;% <
);;< =
;;;= >
return<< 

BadRequest<< !
(<<! "
$num<<" %
)<<% &
;<<& '
}== 
}>> 	
[@@ 	
HttpPost@@	 
(@@ 
$str@@ !
)@@! "
]@@" #
publicAA 
asyncAA 
TaskAA 
<AA 
IActionResultAA '
>AA' (
CreateVacancyAA) 6
(AA6 7
[AA7 8
FromBodyAA8 @
]AA@ A
CreateVacancyAAB O

newVacancyAAP Z
)AAZ [
{BB 	
tryCC 
{DD 
stringEE 
	companyIdEE  
=EE! "
UserEE# '
.EE' (
FindFirstValueEE( 6
(EE6 7

ClaimTypesEE7 A
.EEA B
NameIdentifierEEB P
)EEP Q
;EEQ R
GuidFF 
idFF 
=FF 
awaitFF 
_companyServiceFF  /
.FF/ 0
CreateVacancyAsyncFF0 B
(FFB C

newVacancyFFC M
,FFM N
	companyIdFFO X
)FFX Y
;FFY Z
returnGG 
OkGG 
(GG 
newGG 
{GG 
idGG  "
}GG# $
)GG$ %
;GG% &
}HH 
catchII 
(II 
	ExceptionII 
exII 
)II  
{JJ 
_loggerKK 
.KK 
LogErrorKK  
(KK  !
exKK! #
,KK# $
$strKK% =
)KK= >
;KK> ?
returnLL 

BadRequestLL !
(LL! "
$numLL" %
)LL% &
;LL& '
}MM 
}NN 	
[PP 	
HttpPutPP	 
(PP 
$strPP  
)PP  !
]PP! "
publicQQ 
asyncQQ 
TaskQQ 
<QQ 
IActionResultQQ '
>QQ' (
UpdateVacancyQQ) 6
(QQ6 7
[QQ7 8
FromBodyQQ8 @
]QQ@ A
UpdateVacancyQQB O
updatedVacancyQQP ^
)QQ^ _
{RR 	
trySS 
{TT 
stringUU 
	companyIdUU  
=UU! "
UserUU# '
.UU' (
FindFirstValueUU( 6
(UU6 7

ClaimTypesUU7 A
.UUA B
NameIdentifierUUB P
)UUP Q
;UUQ R
awaitVV 
_companyServiceVV %
.VV% &
UpdateVacancyAsyncVV& 8
(VV8 9
updatedVacancyVV9 G
,VVG H
	companyIdVVI R
)VVR S
;VVS T
returnWW 
OkWW 
(WW 
newWW 
{WW 
messageWW  '
=WW( )
$strWW* ;
}WW< =
)WW= >
;WW> ?
}XX 
catchYY 
(YY 
	ExceptionYY 
exYY 
)YY  
{ZZ 
_logger[[ 
.[[ 
LogError[[  
([[  !
ex[[! #
,[[# $
$str[[% =
)[[= >
;[[> ?
return\\ 

BadRequest\\ !
(\\! "
$num\\" %
)\\% &
;\\& '
}]] 
}^^ 	
[`` 	

HttpDelete``	 
(`` 
$str`` #
)``# $
]``$ %
publicaa 
asyncaa 
Taskaa 
<aa 
IActionResultaa '
>aa' (
DeleteVacancyaa) 6
(aa6 7
[aa7 8
	FromQueryaa8 A
]aaA B
GuidaaC G
	vacancyIdaaH Q
)aaQ R
{bb 	
trycc 
{dd 
stringee 
	companyIdee  
=ee! "
Useree# '
.ee' (
FindFirstValueee( 6
(ee6 7

ClaimTypesee7 A
.eeA B
NameIdentifiereeB P
)eeP Q
;eeQ R
awaitff 
_companyServiceff %
.ff% &
DeleteVacancyAsyncff& 8
(ff8 9
	vacancyIdff9 B
,ffB C
	companyIdffD M
)ffM N
;ffN O
returngg 
Okgg 
(gg 
$strgg +
)gg+ ,
;gg, -
}hh 
catchii 
(ii 
	Exceptionii 
exii 
)ii  
{jj 
_loggerkk 
.kk 
LogErrorkk  
(kk  !
exkk! #
,kk# $
$strkk% =
)kk= >
;kk> ?
returnll 

BadRequestll !
(ll! "
$numll" %
)ll% &
;ll& '
}mm 
}nn 	
[pp 	
HttpPostpp	 
(pp 
$strpp $
)pp$ %
]pp% &
publicqq 
asyncqq 
Taskqq 
<qq 
IActionResultqq '
>qq' (
AddVacancyFilterqq) 9
(qq9 :
[qq: ;
FromBodyqq; C
]qqC D
	AddFilterqqE N
	newFilterqqO X
)qqX Y
{rr 	
tryss 
{tt 
stringuu 
	companyIduu  
=uu! "
Useruu# '
.uu' (
FindFirstValueuu( 6
(uu6 7

ClaimTypesuu7 A
.uuA B
NameIdentifieruuB P
)uuP Q
!uuQ R
;uuR S
varvv 
idsvv 
=vv 
awaitvv 
_companyServicevv  /
.vv/ 0!
AddVacancyFilterAsyncvv0 E
(vvE F
	newFiltervvF O
,vvO P
	companyIdvvQ Z
)vvZ [
;vv[ \
returnww 
Okww 
(ww 
newww 
{ww 
idww  "
=ww# $
idsww% (
}ww) *
)ww* +
;ww+ ,
}xx 
catchyy 
(yy 
	Exceptionyy 
exyy 
)yy  
{zz 
_logger{{ 
.{{ 
LogError{{  
({{  !
ex{{! #
,{{# $
$str{{% @
){{@ A
;{{A B
return|| 

BadRequest|| !
(||! "
$num||" %
)||% &
;||& '
}}} 
}~~ 	
[
ÄÄ 	

HttpDelete
ÄÄ	 
(
ÄÄ 
$str
ÄÄ )
)
ÄÄ) *
]
ÄÄ* +
public
ÅÅ 
async
ÅÅ 
Task
ÅÅ 
<
ÅÅ 
IActionResult
ÅÅ '
>
ÅÅ' (!
DeleteVacancyFilter
ÅÅ) <
(
ÅÅ< =
[
ÅÅ= >
	FromQuery
ÅÅ> G
]
ÅÅG H
Guid
ÅÅI M
filterId
ÅÅN V
)
ÅÅV W
{
ÇÇ 	
try
ÉÉ 
{
ÑÑ 
string
ÖÖ 
	companyId
ÖÖ  
=
ÖÖ! "
User
ÖÖ# '
.
ÖÖ' (
FindFirstValue
ÖÖ( 6
(
ÖÖ6 7

ClaimTypes
ÖÖ7 A
.
ÖÖA B
NameIdentifier
ÖÖB P
)
ÖÖP Q
;
ÖÖQ R
await
ÜÜ 
_companyService
ÜÜ %
.
ÜÜ% &&
DeleteVacancyFilterAsync
ÜÜ& >
(
ÜÜ> ?
filterId
ÜÜ? G
,
ÜÜG H
	companyId
ÜÜI R
)
ÜÜR S
;
ÜÜS T
return
áá 
Ok
áá 
(
áá 
$str
áá *
)
áá* +
;
áá+ ,
}
àà 
catch
ââ 
(
ââ "
KeyNotFoundException
ââ '
ex
ââ( *
)
ââ* +
{
ää 
_logger
ãã 
.
ãã 
LogError
ãã  
(
ãã  !
ex
ãã! #
,
ãã# $
$str
ãã% B
,
ããB C
filterId
ããD L
)
ããL M
;
ããM N
return
åå 
NotFound
åå 
(
åå  
ex
åå  "
.
åå" #
Message
åå# *
)
åå* +
;
åå+ ,
}
çç 
catch
éé 
(
éé 
	Exception
éé 
ex
éé 
)
éé  
{
èè 
_logger
êê 
.
êê 
LogError
êê  
(
êê  !
ex
êê! #
,
êê# $
$str
êê% C
)
êêC D
;
êêD E
return
ëë 

BadRequest
ëë !
(
ëë! "
$num
ëë" %
)
ëë% &
;
ëë& '
}
íí 
}
ìì 	
[
¥¥ 	
HttpGet
¥¥	 
(
¥¥ 
$str
¥¥ 
)
¥¥ 
]
¥¥ 
public
µµ 
async
µµ 
Task
µµ 
<
µµ 
IActionResult
µµ '
>
µµ' (
GetFlyer
µµ) 1
(
µµ1 2
[
µµ2 3
	FromQuery
µµ3 <
]
µµ< =
Guid
µµ> B
	vacancyId
µµC L
,
µµL M
string
µµN T
url
µµU X
)
µµX Y
{
∂∂ 	
try
∑∑ 
{
∏∏ 
var
ππ 
flyer
ππ 
=
ππ 
await
ππ !
_companyService
ππ" 1
.
ππ1 2
GetFlyerAsync
ππ2 ?
(
ππ? @
	vacancyId
ππ@ I
,
ππI J
url
ππJ M
)
ππM N
;
ππN O
return
∫∫ 
File
∫∫ 
(
∫∫ 
flyer
∫∫ !
,
∫∫! "
$str
∫∫# 4
,
∫∫4 5
$"
∫∫6 8
$str
∫∫8 >
{
∫∫> ?
	vacancyId
∫∫? H
}
∫∫H I
$str
∫∫I M
"
∫∫M N
)
∫∫N O
;
∫∫O P
}
ªª 
catch
ºº 
(
ºº 
	Exception
ºº 
ex
ºº 
)
ºº  
{
ΩΩ 
_logger
ææ 
.
ææ 
LogError
ææ  
(
ææ  !
ex
ææ! #
,
ææ# $
$str
ææ% 8
)
ææ8 9
;
ææ9 :
return
øø 

BadRequest
øø !
(
øø! "
$num
øø" %
)
øø% &
;
øø& '
}
¿¿ 
}
¡¡ 	
[
√√ 	
HttpGet
√√	 
(
√√ 
$str
√√ 
)
√√ 
]
√√ 
public
ƒƒ 
async
ƒƒ 
Task
ƒƒ 
<
ƒƒ 
IActionResult
ƒƒ '
>
ƒƒ' (

GetProfile
ƒƒ) 3
(
ƒƒ3 4
)
ƒƒ4 5
{
≈≈ 	
try
∆∆ 
{
«« 
string
»» 
	companyId
»»  
=
»»! "
User
»»# '
.
»»' (
FindFirstValue
»»( 6
(
»»6 7

ClaimTypes
»»7 A
.
»»A B
NameIdentifier
»»B P
)
»»P Q
!
»»Q R
;
»»R S
string
…… 

authHeader
…… !
=
……" #
Request
……$ +
.
……+ ,
Headers
……, 3
[
……3 4
$str
……4 C
]
……C D
.
……D E
First
……E J
(
……J K
)
……K L
!
……L M
;
……M N
string
   
token
   
=
   

authHeader
   )
.
  ) *
Replace
  * 1
(
  1 2
$str
  2 ;
,
  ; <
string
  = C
.
  C D
Empty
  D I
)
  I J
;
  J K 
CompanyProfileDtos
ÀÀ "
profile
ÀÀ# *
=
ÀÀ+ ,
await
ÀÀ- 2
_companyService
ÀÀ3 B
.
ÀÀB C
GetProfileAsync
ÀÀC R
(
ÀÀR S
	companyId
ÀÀS \
,
ÀÀ\ ]
token
ÀÀ^ c
)
ÀÀc d
;
ÀÀd e
return
ÃÃ 
Ok
ÃÃ 
(
ÃÃ 
profile
ÃÃ !
)
ÃÃ! "
;
ÃÃ" #
}
ÕÕ 
catch
ŒŒ 
(
ŒŒ 
	Exception
ŒŒ 
ex
ŒŒ 
)
ŒŒ  
{
œœ 
_logger
–– 
.
–– 
LogError
––  
(
––  !
ex
––! #
,
––# $
$str
––% :
)
––: ;
;
––; <
return
—— 

BadRequest
—— !
(
——! "
$num
——" %
)
——% &
;
——& '
}
““ 
}
”” 	
[
’’ 	
HttpPut
’’	 
(
’’ 
$str
’’ "
)
’’" #
]
’’# $
public
÷÷ 
async
÷÷ 
Task
÷÷ 
<
÷÷ 
IActionResult
÷÷ '
>
÷÷' (
UpdateCompany
÷÷) 6
(
÷÷6 7
Guid
÷÷7 ;
userId
÷÷< B
,
÷÷B C
[
÷÷D E
FromBody
÷÷E M
]
÷÷M N
UpdateCompanyDto
÷÷O _
dto
÷÷` c
)
÷÷c d
{
◊◊ 	
var
ÿÿ 
company
ÿÿ 
=
ÿÿ 
await
ÿÿ  
_companyRepository
ÿÿ  2
.
ÿÿ2 3!
GetCompanyByIdAsync
ÿÿ3 F
(
ÿÿF G
userId
ÿÿG M
)
ÿÿM N
;
ÿÿN O
if
⁄⁄ 
(
⁄⁄ 
company
⁄⁄ 
==
⁄⁄ 
null
⁄⁄ 
)
⁄⁄  
return
€€ 
NotFound
€€ 
(
€€  
$str
€€  3
)
€€3 4
;
€€4 5
company
›› 
.
›› 
name
›› 
=
›› 
dto
›› 
.
›› 
Name
›› #
;
››# $
company
ﬁﬁ 
.
ﬁﬁ 
email
ﬁﬁ 
=
ﬁﬁ 
dto
ﬁﬁ 
.
ﬁﬁ  
Email
ﬁﬁ  %
;
ﬁﬁ% &
company
ﬂﬂ 
.
ﬂﬂ 
phoneNumber
ﬂﬂ 
=
ﬂﬂ  !
dto
ﬂﬂ" %
.
ﬂﬂ% &
PhoneNumber
ﬂﬂ& 1
;
ﬂﬂ1 2
company
‡‡ 
.
‡‡ 
latitude
‡‡ 
=
‡‡ 
dto
‡‡ "
.
‡‡" #
Latitude
‡‡# +
;
‡‡+ ,
company
·· 
.
·· 
	longitude
·· 
=
·· 
dto
··  #
.
··# $
	Longitude
··$ -
;
··- .
company
‚‚ 
.
‚‚ 
website
‚‚ 
=
‚‚ 
dto
‚‚ !
.
‚‚! "
Website
‚‚" )
;
‚‚) *
await
‰‰  
_companyRepository
‰‰ $
.
‰‰$ % 
UpdateCompanyAsync
‰‰% 7
(
‰‰7 8
company
‰‰8 ?
)
‰‰? @
;
‰‰@ A
return
ÊÊ 
Ok
ÊÊ 
(
ÊÊ 
company
ÊÊ 
)
ÊÊ 
;
ÊÊ 
}
ÁÁ 	
}
ËË 
}ÈÈ „
dC:\Users\demde\Desktop\Worky\Worky\Services\CompanyService\CompanyService.Api\CompanyDIExtensions.cs
	namespace 	
CompanyService
 
. 
Api 
. 

Extentions '
;' (
public 
static 
class 
CompanyDIExtensions '
{ 
public 

static 
IServiceCollection $
AddCompanyService% 6
(6 7
this7 ;
IServiceCollection< N
servicesO W
)W X
{ 
services 
. 
AddSingleton 
<  
IDbConnectionFactory 2
,2 3$
NpsqlDbConnectionFactory4 L
>L M
(M N
)N O
;O P
services 
. 
	AddScoped 
< 
IRedisRepository +
,+ ,
RedisRepository- <
>< =
(= >
)> ?
;? @
services 
. 
	AddScoped 
< 
ICompanyRepository -
,- .
CompanyRepository/ @
>@ A
(A B
)B C
;C D
services 
. 
	AddScoped 
< 
IVacancyRepository -
,- .
VacancyRepository/ @
>@ A
(A B
)B C
;C D
services 
. 
	AddScoped 
< 
IFilterCacheService .
,. /
FilterCacheService0 B
>B C
(C D
)D E
;E F
services 
. 
	AddScoped 
< 
ICompnayService *
,* +
BLL, /
./ 0
Services0 8
.8 9
Implementations9 H
.H I
CompanyServiceI W
>W X
(X Y
)Y Z
;Z [
services 
. 
	AddScoped 
< 
ITariffRepository ,
,, -
TariffRepository. >
>> ?
(? @
)@ A
;A B
services 
. 
	AddScoped 
< 
IDealRepository *
,* +
DealRepository, :
>: ;
(; <
)< =
;= >
services 
. 
	AddScoped 
< 
IDealService '
,' (
DealService) 4
>4 5
(5 6
)6 7
;7 8
return 
services 
; 
} 
} 