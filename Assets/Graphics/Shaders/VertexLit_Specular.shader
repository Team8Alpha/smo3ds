Shader "3DS/Shiny Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Shininess ("Shininess", Range(0, 10)) = 1.2
        _Light ("Light Direction", Vector) = (1, -1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "Vertex" }
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
            float _Shininess;
            float3 _Light;

            VertexOut vert(VertexIn i)
            {
                VertexOut o;

                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv_TexA = i.uv_TexA;

                float3 worldNormal = normalize(mul(unity_ObjectToWorld, i.normal));

                // Calculate diffuse intensity
                float3 light = max(0, dot(worldNormal, -normalize(_Light)));

                // Calculate specular intensity
                float3 halfwayDir = normalize(normalize(_Light) + normalize(i.vertex.xyz));
                float specular = pow(max(0, dot(worldNormal, halfwayDir)), 1);

                // Finalize light color with shininess
                float4 diffuse = float4(light * _Color.rgb, 1);
                float4 specularColor = float4(specular, specular, specular, 1);
                o.color = clamp(diffuse + specularColor, 0, 1);

                return o;
            }

            fixed4 frag(VertexOut i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv_TexA) * i.color * _Shininess;
                return color;
            }

            ENDCG
        }
    }
}
