Õ´
VC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.Api\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
string 
elasticsearchUrl 
= 
builder !
.! "
Configuration" /
./ 0
GetValue0 8
<8 9
string9 ?
>? @
(@ A
$strA T
)T U
?? 
$str 1
;1 2
var 
username 
= 
builder 
. 
Configuration $
.$ %
GetValue% -
<- .
string. 4
>4 5
(5 6
$str6 N
)N O
?? 
$str 
; 
var 
password 
= 
builder 
. 
Configuration $
.$ %
GetValue% -
<- .
string. 4
>4 5
(5 6
$str6 N
)N O
?? 
$str 
; 
var 
connectionString 
= 
builder 
. 
Configuration ,
., -
GetConnectionString- @
(@ A
$strA T
)T U
??V X
throw 
new  %
InvalidOperationException! :
(: ;
$str; m
)m n
;n o
builder 
. 
Services 
. 
AddDbContext 
< 
SearchDbContext -
>- .
(. /
options/ 6
=>7 9
options   
.   
	UseNpgsql   
(   
connectionString   &
)  & '
.!! 	&
EnableSensitiveDataLogging!!	 #
(!!# $
)!!$ %
."" 	 
EnableDetailedErrors""	 
("" 
)"" 
)""  
;""  !
var$$ 
settings$$ 
=$$ 
new$$ '
ElasticsearchClientSettings$$ .
($$. /
new$$/ 2
Uri$$3 6
($$6 7
elasticsearchUrl$$7 G
)$$G H
)$$H I
.%% 
Authentication%% 
(%% 
new%% 
BasicAuthentication%% +
(%%+ ,
username%%, 4
,%%4 5
password%%6 >
)%%> ?
)%%? @
.&& 
DefaultIndex&& 
(&& 
$str&& !
)&&! "
.'' 
EnableDebugMode'' 
('' 
)'' 
.(( 

PrettyJson(( 
((( 
)(( 
.)) $
DefaultFieldNameInferrer)) 
()) 
p)) 
=>))  "
p))# $
)))$ %
.** 
DefaultMappingFor** 
<** 
VacancyDocument** &
>**& '
(**' (
m**( )
=>*** ,
m**- .
.++ 	
	IndexName++	 
(++ 
$str++ 
)++ 
.,, 	

IdProperty,,	 
(,, 
p,, 
=>,, 
p,, 
.,, 
id,, 
),, 
)-- 
... 
DefaultMappingFor.. 
<.. 
ResumeDocument.. %
>..% &
(..& '
m..' (
=>..) +
m.., -
.// 	
	IndexName//	 
(// 
$str// 
)// 
.00 	

IdProperty00	 
(00 
p00 
=>00 
p00 
.00 
id00 
)00 
)11 
;11 
ElasticsearchClient55 
client55 
=55 
new55  
ElasticsearchClient55! 4
(554 5
settings555 =
)55= >
;55> ?
builder88 
.88 
Services88 
.88 
AddSingleton88 
(88 
client88 $
)88$ %
;88% &
builder:: 
.:: 
Services:: 
.:: 
AddControllers:: 
(::  
)::  !
;::! "
builder;; 
.;; 
Services;; 
.;; #
AddEndpointsApiExplorer;; (
(;;( )
);;) *
;;;* +
builder<< 
.<< 
Services<< 
.<< 
AddSwaggerGen<< 
(<< 
c<<  
=><<! #
{== 
try>> 
{?? 
c@@ 	
.@@	 


SwaggerDoc@@
 
(@@ 
$str@@ 
,@@ 
new@@ 
OpenApiInfo@@ *
{@@+ ,
Title@@- 2
=@@3 4
$str@@5 <
,@@< =
Version@@> E
=@@F G
$str@@H L
}@@M N
)@@N O
;@@O P
}AA 
catchBB 	
(BB
 
	ExceptionBB 
exBB 
)BB 
{CC 
ConsoleDD 
.DD 
	WriteLineDD 
(DD 
$strDD (
+DD) *
exDD+ -
.DD- .
MessageDD. 5
)DD5 6
;DD6 7
throwEE 
;EE 
}FF 
}GG 
)GG 
;GG 
builderII 
.II 
ServicesII 
.II "
AddElasticRepositoriesII '
(II' (
)II( )
;II) *
builderJJ 
.JJ 
ServicesJJ 
.JJ 
AddElasticServicesJJ #
(JJ# $
)JJ$ %
;JJ% &
builderLL 
.LL 
ServicesLL 
.LL 
	ConfigureLL 
<LL 
EmbeddingOptionsLL +
>LL+ ,
(LL, -
builderMM 
.MM 
ConfigurationMM 
.MM 

GetSectionMM $
(MM$ %
$strMM% 1
)MM1 2
)MM2 3
;MM3 4
builderOO 
.OO 
ServicesOO 
.OO 
AddHttpClientOO 
<OO 
IEmbeddingServiceOO 0
,OO0 1 
EmbeddingHttpServiceOO2 F
>OOF G
(OOG H
(PP 
spPP 
,PP 
clientPP	 
)PP 
=>PP 
{QQ 
varRR 
optionsRR 
=RR 
spRR 
.RR 
GetRequiredServiceRR +
<RR+ ,
	MicrosoftSS 
.SS 

ExtensionsSS  
.SS  !
OptionsSS! (
.SS( )
IOptionsSS) 1
<SS1 2
EmbeddingOptionsSS2 B
>SSB C
>SSC D
(SSD E
)SSE F
.SSF G
ValueSSG L
;SSL M
clientUU 
.UU 
BaseAddressUU 
=UU 
newUU  
UriUU! $
(UU$ %
optionsUU% ,
.UU, -
BaseUrlUU- 4
)UU4 5
;UU5 6
clientVV 
.VV 
TimeoutVV 
=VV 
TimeSpanVV !
.VV! "
FromSecondsVV" -
(VV- .
optionsVV. 5
.VV5 6
TimeoutSecondsVV6 D
)VVD E
;VVE F
}WW 
)WW 
;WW 
builderYY 
.YY 
ServicesYY 
.YY 
AddMassTransitYY 
(YY  
configYY  &
=>YY' )
{ZZ 
config[[ 

.[[
 "
AddInMemoryInboxOutbox[[ !
([[! "
)[[" #
;[[# $
config]] 

.]]
 
UsingInMemory]] 
(]] 
(]] 
context]] !
,]]! "
cfg]]# &
)]]& '
=>]]( *
{^^ 
cfg__ 
.__ 
ConfigureEndpoints__ 
(__ 
context__ &
)__& '
;__' (
}`` 
)`` 
;`` 
configbb 

.bb
 
AddRiderbb 
(bb 
riderbb 
=>bb 
{cc 
riderdd 
.dd 
AddConsumerdd 
<dd !
ResumeCreatedConsumerdd /
>dd/ 0
(dd0 1
)dd1 2
;dd2 3
rideree 
.ee 
AddConsumeree 
<ee !
ResumeDeletedConsumeree /
>ee/ 0
(ee0 1
)ee1 2
;ee2 3
riderff 
.ff 
AddConsumerff 
<ff #
ResumeFilterAddConsumerff 1
>ff1 2
(ff2 3
)ff3 4
;ff4 5
ridergg 
.gg 
AddConsumergg 
<gg &
ResumeFilterDeleteConsumergg 4
>gg4 5
(gg5 6
)gg6 7
;gg7 8
riderhh 
.hh 
AddConsumerhh 
<hh !
ResumeUpdatedConsumerhh /
>hh/ 0
(hh0 1
)hh1 2
;hh2 3
riderjj 
.jj 
AddConsumerjj 
<jj "
VacancyCreatedConsumerjj 0
>jj0 1
(jj1 2
)jj2 3
;jj3 4
riderkk 
.kk 
AddConsumerkk 
<kk "
VacancyDeletedConsumerkk 0
>kk0 1
(kk1 2
)kk2 3
;kk3 4
riderll 
.ll 
AddConsumerll 
<ll $
VacancyFilterAddConsumerll 2
>ll2 3
(ll3 4
)ll4 5
;ll5 6
ridermm 
.mm 
AddConsumermm 
<mm '
VacancyFilterDeleteConsumermm 5
>mm5 6
(mm6 7
)mm7 8
;mm8 9
ridernn 
.nn 
AddConsumernn 
<nn "
VacancyUpdatedConsumernn 0
>nn0 1
(nn1 2
)nn2 3
;nn3 4
riderqq 
.qq 

UsingKafkaqq 
(qq 
(qq 
contextqq !
,qq! "
kqq# $
)qq$ %
=>qq& (
{rr 	!
IConfigurationSectionss !
kafkaSettingsss" /
=ss0 1
builderss2 9
.ss9 :
Configurationss: G
.ssG H

GetSectionssH R
(ssR S
$strssS Z
)ssZ [
;ss[ \
stringtt 
bootstrapServerstt #
=tt$ %
kafkaSettingstt& 3
[tt3 4
$strtt4 F
]ttF G
;ttG H
stringuu 
groupIduu 
=uu 
kafkaSettingsuu *
[uu* +
$struu+ 4
]uu4 5
;uu5 6
kvv 
.vv 
Hostvv 
(vv 
bootstrapServersvv #
)vv# $
;vv$ %
kyy 
.yy 
TopicEndpointyy 
<yy 
ResumeCreatedEventyy .
>yy. /
(yy/ 0
$stryy0 @
,yy@ A
groupIdyyB I
,yyI J
eyyK L
=>yyM O
{zz 
e{{ 
.{{ 
AutoOffsetReset{{ !
={{" #
	Confluent{{$ -
.{{- .
Kafka{{. 3
.{{3 4
AutoOffsetReset{{4 C
.{{C D
Earliest{{D L
;{{L M
e|| 
.|| 
ConfigureConsumer|| #
<||# $!
ResumeCreatedConsumer||$ 9
>||9 :
(||: ;
context||; B
)||B C
;||C D
e}} 
.}} 
CreateIfMissing}} !
(}}! "
)}}" #
;}}# $
}~~ 
)~~ 
;~~ 
k 
. 
TopicEndpoint 
< 
ResumeDeletedEvent .
>. /
(/ 0
$str0 @
,@ A
groupIdB I
,I J
eK L
=>M O
{
ÄÄ 
e
ÅÅ 
.
ÅÅ 
AutoOffsetReset
ÅÅ !
=
ÅÅ" #
	Confluent
ÅÅ$ -
.
ÅÅ- .
Kafka
ÅÅ. 3
.
ÅÅ3 4
AutoOffsetReset
ÅÅ4 C
.
ÅÅC D
Earliest
ÅÅD L
;
ÅÅL M
e
ÇÇ 
.
ÇÇ 
ConfigureConsumer
ÇÇ #
<
ÇÇ# $#
ResumeDeletedConsumer
ÇÇ$ 9
>
ÇÇ9 :
(
ÇÇ: ;
context
ÇÇ; B
)
ÇÇB C
;
ÇÇC D
e
ÉÉ 
.
ÉÉ 
CreateIfMissing
ÉÉ !
(
ÉÉ! "
)
ÉÉ" #
;
ÉÉ# $
}
ÑÑ 
)
ÑÑ 
;
ÑÑ 
k
ÖÖ 
.
ÖÖ 
TopicEndpoint
ÖÖ 
<
ÖÖ "
ResumeFilterAddEvent
ÖÖ 0
>
ÖÖ0 1
(
ÖÖ1 2
$str
ÖÖ2 E
,
ÖÖE F
groupId
ÖÖG N
,
ÖÖN O
e
ÖÖP Q
=>
ÖÖR T
{
ÜÜ 
e
áá 
.
áá 
AutoOffsetReset
áá !
=
áá" #
	Confluent
áá$ -
.
áá- .
Kafka
áá. 3
.
áá3 4
AutoOffsetReset
áá4 C
.
ááC D
Earliest
ááD L
;
ááL M
e
àà 
.
àà 
ConfigureConsumer
àà #
<
àà# $%
ResumeFilterAddConsumer
àà$ ;
>
àà; <
(
àà< =
context
àà= D
)
ààD E
;
ààE F
e
ââ 
.
ââ 
CreateIfMissing
ââ !
(
ââ! "
)
ââ" #
;
ââ# $
}
ää 
)
ää 
;
ää 
k
ãã 
.
ãã 
TopicEndpoint
ãã 
<
ãã %
ResumeFilterDeleteEvent
ãã 3
>
ãã3 4
(
ãã4 5
$str
ãã5 K
,
ããK L
groupId
ããM T
,
ããT U
e
ããV W
=>
ããX Z
{
åå 
e
çç 
.
çç 
AutoOffsetReset
çç !
=
çç" #
	Confluent
çç$ -
.
çç- .
Kafka
çç. 3
.
çç3 4
AutoOffsetReset
çç4 C
.
ççC D
Earliest
ççD L
;
ççL M
e
éé 
.
éé 
ConfigureConsumer
éé #
<
éé# $(
ResumeFilterDeleteConsumer
éé$ >
>
éé> ?
(
éé? @
context
éé@ G
)
ééG H
;
ééH I
e
èè 
.
èè 
CreateIfMissing
èè !
(
èè! "
)
èè" #
;
èè# $
}
êê 
)
êê 
;
êê 
k
ëë 
.
ëë 
TopicEndpoint
ëë 
<
ëë  
ResumeUpdatedEvent
ëë .
>
ëë. /
(
ëë/ 0
$str
ëë0 @
,
ëë@ A
groupId
ëëB I
,
ëëI J
e
ëëK L
=>
ëëM O
{
íí 
e
ìì 
.
ìì 
AutoOffsetReset
ìì !
=
ìì" #
	Confluent
ìì$ -
.
ìì- .
Kafka
ìì. 3
.
ìì3 4
AutoOffsetReset
ìì4 C
.
ììC D
Earliest
ììD L
;
ììL M
e
îî 
.
îî 
ConfigureConsumer
îî #
<
îî# $#
ResumeUpdatedConsumer
îî$ 9
>
îî9 :
(
îî: ;
context
îî; B
)
îîB C
;
îîC D
e
ïï 
.
ïï 
CreateIfMissing
ïï !
(
ïï! "
)
ïï" #
;
ïï# $
}
ññ 
)
ññ 
;
ññ 
k
öö 
.
öö 
TopicEndpoint
öö 
<
öö !
VacancyCreatedEvent
öö /
>
öö/ 0
(
öö0 1
$str
öö1 B
,
ööB C
groupId
ööD K
,
ööK L
e
ööM N
=>
ööO Q
{
õõ 
e
úú 
.
úú 
AutoOffsetReset
úú !
=
úú" #
	Confluent
úú$ -
.
úú- .
Kafka
úú. 3
.
úú3 4
AutoOffsetReset
úú4 C
.
úúC D
Earliest
úúD L
;
úúL M
e
ùù 
.
ùù 
ConfigureConsumer
ùù #
<
ùù# $$
VacancyCreatedConsumer
ùù$ :
>
ùù: ;
(
ùù; <
context
ùù< C
)
ùùC D
;
ùùD E
e
ûû 
.
ûû 
CreateIfMissing
ûû !
(
ûû! "
)
ûû" #
;
ûû# $
}
üü 
)
üü 
;
üü 
k
†† 
.
†† 
TopicEndpoint
†† 
<
†† !
VacancyDeletedEvent
†† /
>
††/ 0
(
††0 1
$str
††1 B
,
††B C
groupId
††D K
,
††K L
e
††M N
=>
††O Q
{
°° 
e
¢¢ 
.
¢¢ 
AutoOffsetReset
¢¢ !
=
¢¢" #
	Confluent
¢¢$ -
.
¢¢- .
Kafka
¢¢. 3
.
¢¢3 4
AutoOffsetReset
¢¢4 C
.
¢¢C D
Earliest
¢¢D L
;
¢¢L M
e
££ 
.
££ 
ConfigureConsumer
££ #
<
££# $$
VacancyDeletedConsumer
££$ :
>
££: ;
(
££; <
context
££< C
)
££C D
;
££D E
e
§§ 
.
§§ 
CreateIfMissing
§§ !
(
§§! "
)
§§" #
;
§§# $
}
•• 
)
•• 
;
•• 
k
¶¶ 
.
¶¶ 
TopicEndpoint
¶¶ 
<
¶¶ #
VacancyFilterAddEvent
¶¶ 1
>
¶¶1 2
(
¶¶2 3
$str
¶¶3 G
,
¶¶G H
groupId
¶¶I P
,
¶¶P Q
e
¶¶R S
=>
¶¶T V
{
ßß 
e
®® 
.
®® 
AutoOffsetReset
®® !
=
®®" #
	Confluent
®®$ -
.
®®- .
Kafka
®®. 3
.
®®3 4
AutoOffsetReset
®®4 C
.
®®C D
Earliest
®®D L
;
®®L M
e
©© 
.
©© 
ConfigureConsumer
©© #
<
©©# $&
VacancyFilterAddConsumer
©©$ <
>
©©< =
(
©©= >
context
©©> E
)
©©E F
;
©©F G
e
™™ 
.
™™ 
CreateIfMissing
™™ !
(
™™! "
)
™™" #
;
™™# $
}
´´ 
)
´´ 
;
´´ 
k
¨¨ 
.
¨¨ 
TopicEndpoint
¨¨ 
<
¨¨ &
VacancyFilterDeleteEvent
¨¨ 4
>
¨¨4 5
(
¨¨5 6
$str
¨¨6 M
,
¨¨M N
groupId
¨¨O V
,
¨¨V W
e
¨¨X Y
=>
¨¨Z \
{
≠≠ 
e
ÆÆ 
.
ÆÆ 
AutoOffsetReset
ÆÆ !
=
ÆÆ" #
	Confluent
ÆÆ$ -
.
ÆÆ- .
Kafka
ÆÆ. 3
.
ÆÆ3 4
AutoOffsetReset
ÆÆ4 C
.
ÆÆC D
Earliest
ÆÆD L
;
ÆÆL M
e
ØØ 
.
ØØ 
ConfigureConsumer
ØØ #
<
ØØ# $)
VacancyFilterDeleteConsumer
ØØ$ ?
>
ØØ? @
(
ØØ@ A
context
ØØA H
)
ØØH I
;
ØØI J
e
∞∞ 
.
∞∞ 
CreateIfMissing
∞∞ !
(
∞∞! "
)
∞∞" #
;
∞∞# $
}
±± 
)
±± 
;
±± 
k
≤≤ 
.
≤≤ 
TopicEndpoint
≤≤ 
<
≤≤ !
VacancyUpdatedEvent
≤≤ /
>
≤≤/ 0
(
≤≤0 1
$str
≤≤1 B
,
≤≤B C
groupId
≤≤D K
,
≤≤K L
e
≤≤M N
=>
≤≤O Q
{
≥≥ 
e
¥¥ 
.
¥¥ 
AutoOffsetReset
¥¥ !
=
¥¥" #
	Confluent
¥¥$ -
.
¥¥- .
Kafka
¥¥. 3
.
¥¥3 4
AutoOffsetReset
¥¥4 C
.
¥¥C D
Earliest
¥¥D L
;
¥¥L M
e
µµ 
.
µµ 
ConfigureConsumer
µµ #
<
µµ# $$
VacancyUpdatedConsumer
µµ$ :
>
µµ: ;
(
µµ; <
context
µµ< C
)
µµC D
;
µµD E
e
∂∂ 
.
∂∂ 
CreateIfMissing
∂∂ !
(
∂∂! "
)
∂∂" #
;
∂∂# $
}
∑∑ 
)
∑∑ 
;
∑∑ 
}
∏∏ 	
)
∏∏	 

;
∏∏
 
}
ππ 
)
ππ 
;
ππ 
}∫∫ 
)
∫∫ 
;
∫∫ 
varææ 
app
ææ 
=
ææ 	
builder
ææ
 
.
ææ 
Build
ææ 
(
ææ 
)
ææ 
;
ææ 
app¿¿ 
.
¿¿ 
UseCors
¿¿ 
(
¿¿ 
)
¿¿ 
;
¿¿ 
app¡¡ 
.
¡¡ 

UseRouting
¡¡ 
(
¡¡ 
)
¡¡ 
;
¡¡ 
app¬¬ 
.
¬¬ 
MapControllers
¬¬ 
(
¬¬ 
)
¬¬ 
;
¬¬ 
app√√ 
.
√√ !
UseHttpsRedirection
√√ 
(
√√ 
)
√√ 
;
√√ 
appƒƒ 
.
ƒƒ 
UseAuthentication
ƒƒ 
(
ƒƒ 
)
ƒƒ 
;
ƒƒ 
app≈≈ 
.
≈≈ 
UseAuthorization
≈≈ 
(
≈≈ 
)
≈≈ 
;
≈≈ 
app∆∆ 
.
∆∆ 

UseSwagger
∆∆ 
(
∆∆ 
)
∆∆ 
;
∆∆ 
app«« 
.
«« 
UseSwaggerUI
«« 
(
«« 
c
«« 
=>
«« 
{»» 
c
…… 
.
…… 
SwaggerEndpoint
…… 
(
…… 
$str
…… 0
,
……0 1
$str
……2 H
)
……H I
;
……I J
c
   
.
   
RoutePrefix
   
=
   
string
   
.
   
Empty
    
;
    !
}ÀÀ 
)
ÀÀ 
;
ÀÀ 
usingÕÕ 
(
ÕÕ 
var
ÕÕ 

serviceScope
ÕÕ 
=
ÕÕ 
app
ÕÕ 
.
ÕÕ 
Services
ÕÕ &
.
ÕÕ& '
CreateScope
ÕÕ' 2
(
ÕÕ2 3
)
ÕÕ3 4
)
ÕÕ4 5
{ŒŒ 
var
œœ 
context
œœ 
=
œœ 
serviceScope
œœ 
.
œœ 
ServiceProvider
œœ .
.
œœ. / 
GetRequiredService
œœ/ A
<
œœA B
SearchDbContext
œœB Q
>
œœQ R
(
œœR S
)
œœS T
;
œœT U
context
–– 
.
–– 
Database
–– 
.
–– 
Migrate
–– 
(
–– 
)
–– 
;
–– 
}—— 
app”” 
.
”” 
Run
”” 
(
”” 
)
”” 	
;
””	 
“
uC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.Api\Extensions\AddElasticServicesExtension.cs
	namespace 	
SearchService
 
. 
Api 
. 

Extensions &
;& '
public 
static 
class '
AddElasticServicesExtension /
{ 
public		 

static		 
IServiceCollection		 $
AddElasticServices		% 7
(		7 8
this		8 <
IServiceCollection		= O
services		P X
)		X Y
{

 
services 
. 
	AddScoped 
< !
IVacancySearchService 0
,0 1 
VacancySearchService2 F
>F G
(G H
)H I
;I J
services 
. 
	AddScoped 
<  
IResumeSearchService /
,/ 0
ResumeSearchService1 D
>D E
(E F
)F G
;G H
return 
services 
; 
} 
} Ó
yC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.Api\Extensions\AddElasticRepositoriesExtension.cs
	namespace 	
SearchService
 
. 
Api 
. 

Extensions &
;& '
public 
static 
class +
AddElasticRepositoriesExtension 3
{ 
public 

static 
IServiceCollection $"
AddElasticRepositories% ;
(; <
this< @
IServiceCollectionA S
servicesT \
)\ ]
{		 
services

 
.

 
	AddScoped

 
<

 $
IResumeElasticRepository

 3
,

3 4#
ResumeElasticRepository

5 L
>

L M
(

M N
)

N O
;

O P
services 
. 
	AddScoped 
< %
IVacancyElasticRepository 4
,4 5$
VacancyElasticRepository6 N
>N O
(O P
)P Q
;Q R
return 
services 
; 
} 
} °
lC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.Api\Controllers\VacancyController.cs
	namespace 	
SearchService
 
. 
Api 
. 
Controllers '
;' (
[

 
ApiController

 
]

 
[ 
Route 
( 
$str 
) 
] 
public 
class 
VacancyController 
:  
ControllerBase! /
{ 
private 
readonly !
IVacancySearchService *
_searchService+ 9
;9 :
public 

VacancyController 
( !
IVacancySearchService 2
searchService3 @
)@ A
{ 
_searchService 
= 
searchService &
;& '
} 
[ 
HttpGet 
] 
public 

async 
Task 
< 
IActionResult #
># $
Search% +
(+ ,
[, -
	FromQuery- 6
]6 7
GetVacanciesRequest8 K
requestL S
)S T
{ 
var 
result 
= 
await 
_searchService )
.) *
SearchAsync* 5
(5 6
request6 =
)= >
;> ?
return 
Ok 
( 
result 
) 
; 
} 
} x
vC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.Api\Controllers\TestSearchVacancyController.csø
sC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.Api\Controllers\SearchFeedbackController.cs
	namespace 	
SearchService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[		 
Route		 
(		 
$str		 
)		 
]		 
public

 
class

 $
SearchFeedbackController

 %
:

& '
ControllerBase

( 6
{ 
private 
readonly 
SearchDbContext $
_context% -
;- .
public 
$
SearchFeedbackController #
(# $
SearchDbContext$ 3
context4 ;
); <
{ 
_context 
= 
context 
; 
} 
[ 
HttpPost 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
Click% *
(* +
[+ ,
FromBody, 4
]4 5
ClickRequest6 B
requestC J
)J K
{ 
var 

impression 
= 
await 
_context '
.' (
SearchImpressions( 9
. 
FirstOrDefaultAsync  
(  !
x! "
=># %
x 
. 
	SessionId 
== 
request &
.& '
	SessionId' 0
&&1 3
x 
. 

DocumentId 
== 
request  '
.' (

DocumentId( 2
)2 3
;3 4
if 

( 

impression 
== 
null 
) 
return 
NotFound 
( 
) 
; 

impression 
. 
Clicked 
= 
true !
;! "

impression 
. 
DwellTimeMs 
=  
request! (
.( )
DwellTimeMs) 4
;4 5
await!! 
_context!! 
.!! 
SaveChangesAsync!! '
(!!' (
)!!( )
;!!) *
return## 
Ok## 
(## 
)## 
;## 
}$$ 
}%% ö
kC:\Users\demde\Desktop\Worky\Worky\Services\SearchService\SearchService.Api\Controllers\ResumeController.cs
	namespace 	
SearchService
 
. 
Api 
. 
Controllers '
;' (
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
 
)

 
]

 
public 
class 
ResumeController 
: 
ControllerBase  .
{ 
private 
readonly  
IResumeSearchService )
_searchService* 8
;8 9
public 

ResumeController 
(  
IResumeSearchService 0
searchService1 >
)> ?
{ 
_searchService 
= 
searchService &
;& '
} 
[ 
HttpGet 
] 
public 

async 
Task 
< 
IActionResult #
># $
Search% +
(+ ,
[, -
	FromQuery- 6
]6 7
GetResumesRequest8 I
requestJ Q
)Q R
{ 
var 
result 
= 
await 
_searchService )
.) *
SearchAsync* 5
(5 6
request6 =
)= >
;> ?
return 
Ok 
( 
result 
) 
; 
} 
} 