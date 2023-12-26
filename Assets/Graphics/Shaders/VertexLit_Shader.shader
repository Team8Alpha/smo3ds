//Vertex-lit shader with support for specular and ambient lighting. Fully compatible with the 3DS family.

//Shader by Kevin Foley, 2021. Based loosely on the "Gourad Shader" from Bumpy Trail Games 
//(https://developer.nintendo.com/group/development/wtc6ppr2/forums/english/-/gts_message_boards/thread/294653147)

//You are free to use this shader in any game for the Nintendo 3DS/2DS family without restrictions.

Shader "3DS/Vertex-Lit Specular"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_SpecColor ("SpecColor", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_SpecularIntensityPower ("Specular (X: Intensity, Y: Power)", Vector) = (1, 64, 0, 0)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "Lighting.cginc"
			#include "UnityCG.cginc"
			
			struct VertexIn
			{
				float4 vertex : POSITION;
				float2 uv_TexA : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct VertexOut
			{
				float4 vertex : POSITION;
				float2 uv_TexA : TEXCOORD0;
				float4 color : COLOR;
			};

			float4 _Color;
			sampler2D _MainTex;
			float2 _SpecularIntensityPower;
			
			VertexOut vert(VertexIn i)
			{
				VertexOut o;

				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv_TexA = i.uv_TexA;

				float3 worldNormal = normalize(mul(unity_ObjectToWorld, i.normal));
				float3 worldVertex = mul(unity_ObjectToWorld, i.vertex);
				float3 viewDirection = normalize(WorldSpaceViewDir(i.vertex));
				
				float3 light1_Direction;
				if (_WorldSpaceLightPos0.a == 0) { //directional light
					light1_Direction = _WorldSpaceLightPos0.xyz;
				} else { //point light
					float3 lightPosition = _WorldSpaceLightPos0.xyz;
					light1_Direction = normalize(worldVertex - lightPosition);
				}

				//calculate diffuse intensity
				float3 light1_Color = _LightColor0;
				float3 light1 = max(0, dot(worldNormal, light1_Direction)) * light1_Color;
				
				//calculate specular intensity
				float3 light1_HalfVector = normalize(light1_Direction + viewDirection);
				float light1_Specular = max(0, dot(light1_HalfVector, worldNormal));

				//finalize light color
				float4 ambient = unity_AmbientSky;
				float4 diffuse = float4(light1 * _Color.rgb, 1);
				float specularStrength = pow(light1_Specular, _SpecularIntensityPower.y) * _SpecularIntensityPower.x;
				o.color = clamp(diffuse + ambient, 0, 1) + _SpecColor * specularStrength;

				return o;
			}
			
			fixed4 frag(VertexOut i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv_TexA) * i.color;
				return color;
			}

			ENDCG
		}
	}
}