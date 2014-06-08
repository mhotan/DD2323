﻿Shader "DD2323/custom_shaders/Base Shader" {
	Properties {
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 10
		_RimColor ("Rim Color", Color) = (1.0,1.0,1.0,1.0)
		_RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0
	}
	SubShader {	
		Pass {						
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// user defined variables
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _SpecColor;			
			uniform float4 _RimColor;
			uniform float _Shininess;
			uniform float _RimPower;
			
		
			// Unity defined variable
			uniform float4 _LightColor0;
		
			// base input structs
			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord: TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};
			
			// vertex function
			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				o.posWorld = mul(_Object2World, v.vertex);
				o.normalDir = normalize( mul( float4(v.normal, 0.0), _World2Object ).xyz); 
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tex = v.texcoord;			
				return o;								
			}
			
			// fragment function
			float4 frag(vertexOutput i) : COLOR
			{				
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz );
				float3 lightDirection;
				float atten;	
				
				if (_WorldSpaceLightPos0.w == 0.0) { // Directional Lights
					atten = 1.0;	
					lightDirection = normalize( _WorldSpaceLightPos0.xyz );
				} else {
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length( fragmentToLightSource );
					atten = 1.0 / distance;
					lightDirection = normalize( fragmentToLightSource );
				}
				
				// Lighting
				float3 diffuseReflection = atten * _LightColor0.rgb * saturate(dot(normalDirection, lightDirection));
				float3 specularReflection = diffuseReflection * pow(saturate( dot( reflect( -lightDirection, normalDirection), viewDirection) ), _Shininess);
				
				// Rim lighting
				float rim = 1 - saturate(dot(viewDirection, normalDirection));	
				float3 rimLighting = atten * _LightColor0.rgb * _RimColor.rgb * saturate(dot(normalDirection, lightDirection)) * pow(rim, _RimPower);
				float3 lightFinal = rimLighting + diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT.rgb;
					
				// Texture Maps.
				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy * _MainTex_ST.zw);																										
																																																																				
				return float4(tex.rgb * lightFinal, 1.0);
			}
			
			ENDCG
		}	
		
		Pass {						
			Tags {"LightMode" = "ForwardAdd"}
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// user defined variables
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _SpecColor;			
			uniform float4 _RimColor;
			uniform float _Shininess;
			uniform float _RimPower;
			
		
			// Unity defined variable
			uniform float4 _LightColor0;
		
			// base input structs
			struct vertexInput{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord: TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};
			
			// vertex function
			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				o.posWorld = mul(_Object2World, v.vertex);
				o.normalDir = normalize( mul( float4(v.normal, 0.0), _World2Object ).xyz); 
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tex = v.texcoord;			
				return o;								
			}
			
			// fragment function
			float4 frag(vertexOutput i) : COLOR
			{				
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz );
				float3 lightDirection;
				float atten;	
				
				if (_WorldSpaceLightPos0.w == 0.0) { // Directional Lights
					atten = 1.0;	
					lightDirection = normalize( _WorldSpaceLightPos0.xyz );
				} else {
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length( fragmentToLightSource );
					atten = 1.0 / distance;
					lightDirection = normalize( fragmentToLightSource );
				}
				
				// Lighting
				float3 diffuseReflection = atten * _LightColor0.rgb * saturate(dot(normalDirection, lightDirection));
				float3 specularReflection = diffuseReflection * pow(saturate( dot( reflect( -lightDirection, normalDirection), viewDirection) ), _Shininess);
				
				// Rim lighting
				float rim = 1 - saturate(dot(viewDirection, normalDirection));	
				float3 rimLighting = atten * _LightColor0.rgb * _RimColor.rgb * saturate(dot(normalDirection, lightDirection)) * pow(rim, _RimPower);
				float3 lightFinal = rimLighting + diffuseReflection + specularReflection;
					
				// Texture Maps.
				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy * _MainTex_ST.zw);																										
																																																																				
				return float4(tex.rgb * lightFinal, 1.0);
			}
			
			ENDCG
		}	
	}
	// Uncomment for deployment. 
//	FallBack "Diffuse"
}
