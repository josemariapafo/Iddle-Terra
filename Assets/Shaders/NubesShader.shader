Shader "Custom/Nubes"
{
    Properties
    {
        _MainTex ("Cloud Texture", 2D) = "white" {}
        _Opacity ("Opacity Day", Range(0,1)) = 0.8
        _NightOpacity ("Opacity Night", Range(0,1)) = 0.05
        _ShadowAngle ("Shadow Angle", Range(0,1)) = 0.0
        _Transition ("Transition", Range(0.01,0.3)) = 0.05
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Opacity;
            float _NightOpacity;
            float _ShadowAngle;
            float _Transition;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 cloud = tex2D(_MainTex, i.uv);

                // Misma logica exacta que PlanetaShader
                float longitude = i.uv.x;
                float diff = longitude - _ShadowAngle;
                diff = diff - floor(diff + 0.5);
                float nightZone = abs(diff) - 0.25;
                float blend = smoothstep(-_Transition, _Transition, nightZone);

                float opacity = lerp(_NightOpacity, _Opacity, blend);

                return fixed4(1.0, 1.0, 1.0, cloud.r * opacity);
            }
            ENDCG
        }
    }
}