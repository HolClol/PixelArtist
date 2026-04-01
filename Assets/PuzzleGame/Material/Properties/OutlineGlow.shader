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
            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _EmissionColor;
            float _EmissionStrength;
            float _EdgeSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Fresnel for soft edge glow
                float fresnel = pow(1 - saturate(dot(i.viewDir, i.normal)), _EdgeSoftness);

                // Base color (very subtle)
                float3 baseCol = _Color.rgb;

                // Emission (this is what bloom reacts to)
                float3 emission = _EmissionColor.rgb * fresnel * _EmissionStrength;

                return float4(baseCol + emission, 1);
            }
            ENDCG
        }
    }
}