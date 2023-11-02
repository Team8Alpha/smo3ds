Shader "3DS/Rim-Lit Specular"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_SpecColor ("SpecColor", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_SpecularIntensityPower ("Specular (X: Intensity, Y: Power)", Vector) = (1, 64, 0, 0)
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range(1, 10)) = 5
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
			float4 _RimColor;
			float _RimPower;
			
			VertexOut vert(VertexIn i)
			{
				VertexOut o;

				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv_TexA = i.uv_TexA;

				float3 worldNormal = normalize(mul(unity_ObjectToWorld, i.normal));
				float3 worldVertex = mul(unity_ObjectToWorld, i.vertex);
				float3 viewDirection = normalize(WorldSpaceViewDir(i.vertex));

				// Calculate the light direction (from camera)
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz - worldVertex);

				// Calculate the dot product of the normal and light direction (cosine term)
				float rimDot = dot(worldNormal, lightDirection);

				// Calculate the specular highlight
				float3 light1_HalfVector = normalize(lightDirection + viewDirection);
				float specularStrength = pow(max(0, dot(light1_HalfVector, worldNormal)), _SpecularIntensityPower.y) * _SpecularIntensityPower.x;

				// Calculate the rim light effect
				float rimFactor = 1 - max(0, rimDot);
				rimFactor = pow(rimFactor, _RimPower);

				// Calculate diffuse intensity (rim lighting)
				float3 light1_Color = _LightColor0;
				float3 light1 = max(0, rimFactor) * light1_Color;

				// Finalize the light color
				float4 ambient = unity_AmbientSky;
				float4 diffuse = float4(light1 * _Color.rgb, 1);
				o.color = clamp(diffuse + ambient, 0, 1) + _SpecColor * specularStrength + _RimColor * rimFactor;

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
