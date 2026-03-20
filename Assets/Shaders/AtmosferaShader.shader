Shader "Custom/Atmosfera"
{
    Properties
    {
        _Color ("Color", Color) = (0.4, 0.6, 1.0, 1.0)
        _Intensidad ("Intensidad", Range(0,3)) = 1.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 pos : SV_POSITION; float3 normal : TEXCOORD0; float3 viewDir : TEXCOORD1; };

            fixed4 _Color;
            float _Intensidad;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float rim = 1.0 - saturate(dot(normalize(i.viewDir), normalize(i.normal)));
                float alpha = pow(rim, 3.0) * _Intensidad;
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}