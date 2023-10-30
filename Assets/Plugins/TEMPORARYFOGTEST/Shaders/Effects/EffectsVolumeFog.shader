Shader "Roystan/Effects/Volume Fog"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_FadeThreshold("Fade Threshold", Float) = 1
		[HideInInspector]
		_DisplacementTex("-", 2D) = "black" {}
		[HideInInspector]
		_Size("-", Float) = 0
		[HideInInspector]
		_MaxDisplacement("-", Float) = 1
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 localVertex : TEXCOORD1;
				float4 projPos : TEXCOORD2;
			};
			
			sampler2D _EdgeNoiseTex;
			float4 _EdgeNoiseTex_ST;

			sampler2D _DisplacementTex;
			float _Size;			
			float _MaxDisplacement;

			v2f vert (appdata v)
			{
				v2f o;

				// Convert the local space position of this vertex to 0...1 coordinates
				float2 uv = (v.vertex.xz / _Size) + 0.5;
				float height = tex2Dlod(_DisplacementTex, float4(uv, 0, 0));

				v.vertex.y -= height * _MaxDisplacement;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.localVertex = v.vertex;
				o.projPos = ComputeScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _EdgeNoiseTex);

				return o;
			}
			
			float4 _Color;
			float _FadeThreshold;
			float _EdgeNoiseScrollSpeed;

			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

			fixed4 frag (v2f i) : SV_Target
			{
				// Sample the depth of any pixels behind this one
				float occludedDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
				float occludedDepthLinear = LinearEyeDepth(occludedDepth);

				// Calculate the distance between this pixel and the one behind it
				float difference = occludedDepthLinear - i.projPos.w;
				float normalizedDifference = saturate(difference / _FadeThreshold);

				float threshold = smoothstep(0, 1, normalizedDifference);

				float4 output = _Color;
				output.a *= threshold;

				return output;
			}
			ENDCG
		}
	}
}
