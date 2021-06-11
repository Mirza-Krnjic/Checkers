Shader "Custom/Reflective Shader" {
	Properties {
		[Header(Main Parameters)] 
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_Atten ("Incoming Light Intensity", Range(1,2)) = 1

		[Space(15)]
		_SpecIntensity ("Specular Intensity", Float) = 2
		_Shininess ("Specular Shininess", Float) = 10

		[Space(15)]
		_CubeIntensity ("Cube Map Intensity", Range(0,2)) = 0.5
		_Cube ("Cube Map", Cube) = "" {}

		[Header(Brilliance Parameters)]
		[Toggle] _Iridiscent("Brilliance", Float) = 0
		[Space(10)]
		_FresnelColor1("Illumination Color 1", Color) = (1.0,1.0,1.0,1.0)
		_FresnelPower1("Illumination Power 1", Range(1,15)) = 3.0
		_FresnelIntensity1("Illumination Intensity 1", Range(0,1)) = 1
		[Space(10)]
		_FresnelColor2("Illumination Color 2", Color) = (1.0,1.0,1.0,1.0)
		_FresnelPower2("Illumination Power 2", Range(1, 10)) = 3.0
		_FresnelIntensity2("Illumination Intensity 2", Range(0,1)) = 1

		[Header(Metal Options)]
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MetalSpecIntensity ("Metallic Specular Intensity", Float ) = 3
		_MetallicShininess ("Metallic Specular Shininess", Float) = 10
		
	}
	SubShader {
		Pass {
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			//user defined variables
			uniform float _Atten;
			uniform float _AOIntensity;
			uniform float4 _Color;
			uniform float _SpecIntensity;
			uniform float _Shininess;
			uniform samplerCUBE _Cube;
			uniform float _CubeIntensity;


			uniform float _Metallic;
			uniform float _MetalSpecIntensity;
			uniform float _MetallicShininess;


			uniform fixed _Iridiscent;
			uniform float4 _FresnelColor1;
			uniform float4 _FresnelColor2;
			uniform float _FresnelPower1;
			uniform float _FresnelPower2;
			uniform float _FresnelIntensity1;
			uniform float _FresnelIntensity2;
			
			//unity defined variables;
			uniform float4 _LightColor0;
			
			//structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 posWorld : uv0;
				float3 normalDir : TEXCOORD1;
				float3 viewDir : normalDir;
				float3 metallicsp : metallicsp;
				float metallicdf : metallicdf;
				float4 color : color;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v){
				vertexOutput o;				
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = normalize( mul( float4(v.normal, 0.0), unity_WorldToObject ).xyz );
				o.viewDir = float3(mul(unity_ObjectToWorld, v.vertex) - _WorldSpaceCameraPos).xyz;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.color = v.color;

				o.uv0 = (v.texcoord);

				o.metallicsp = lerp(1, _Color.xyz, _Metallic);
				o.metallicdf = lerp(1, 0.5, _Metallic);

				return o;
				
			}
			
			//fragment function
			float4 frag(vertexOutput i) : COLOR
			{
				//vectors
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );
				float3 lightDirection;				

				//lighting
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuseReflection = _Atten * _LightColor0.xyz * saturate(dot( normalDirection, lightDirection ) );
				float3 specularReflection = (_Atten * _LightColor0.xyz * _SpecIntensity * saturate( dot( normalDirection, lightDirection ) ) * 
											pow( saturate(dot( reflect( -lightDirection, normalDirection ), viewDirection ) ), _Shininess ));

				float3 metallicSpecularReflection = (_Atten * _LightColor0.xyz * _MetalSpecIntensity * saturate( dot( normalDirection, lightDirection ) ) * 
													pow( saturate(dot( reflect( -lightDirection, normalDirection ), viewDirection ) ), _MetallicShininess )) * _Metallic * _Color;

				//cubemap
				float3 reflectDir = reflect(i.viewDir, i.normalDir);
				float3 texC = texCUBE(_Cube, reflectDir) * _CubeIntensity * _Atten * i.color * i.metallicsp;

				//Iridiscent
				float3 fresnel = saturate(dot(normalDirection, viewDirection));
				float3 fresnel1 = pow(fresnel, _FresnelPower1) * _FresnelIntensity1 * _Iridiscent;
				float3 fresnel2 = pow(abs(1 - fresnel), _FresnelPower2) * _FresnelIntensity2 * _Iridiscent;
				float3 colorfinal = lerp (_Color, _FresnelColor1, fresnel1);
				colorfinal = lerp(colorfinal, _FresnelColor2, fresnel2);

				float3 lightFinal = (diffuseReflection + UNITY_LIGHTMODEL_AMBIENT) * colorfinal *  i.metallicdf;

				return float4 (specularReflection + metallicSpecularReflection + lightFinal + texC, 1.0) * i.color;
			}
			
			ENDCG
		}
	}
}