Shader "Custom/OutlineGlow"
{
    Properties
    {
        _Color ("Base Color", Color) = (0.1,0,0.2,1)
        _EmissionColor ("Emission Color", Color) = (0.5,0.2,1,1)
        _EmissionStrength ("Emission Strength", Float) = 3
        _EdgeSoftness ("Edge Softness", Float) = 1.5
    }

    SubShader
    {
        Tags { "Queue"="Geometry+1" }

        Pass
        {
            Cull Front
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _EmissionColor;
            float _EmissionStrength;
            float _EdgeSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float fresnel = pow(1 - saturate(dot(normalize(i.viewDir), normalize(i.normal))), _EdgeSoftness);
                float3 baseCol = _Color.rgb;
                float3 emission = _EmissionColor.rgb * fresnel * _EmissionStrength;
                return float4(baseCol + emission, 1);
            }
            ENDCG
        }
    }
}