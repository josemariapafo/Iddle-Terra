Shader "Custom/Planeta"
{
    Properties
    {
        _DayTex ("Day Texture", 2D) = "white" {}
        _NightTex ("Night Texture", 2D) = "black" {}
        _CloudTex ("Cloud Texture", 2D) = "white" {}
        _NightIntensity ("Night Intensity", Range(0,2)) = 1.5
        _CloudOpacity ("Cloud Opacity", Range(0,1)) = 0.8
        _ShadowAngle ("Shadow Angle", Range(0,1)) = 0.0
        _Transition ("Transition", Range(0.01,0.3)) = 0.05
        _CloudSpeed ("Cloud Speed", Range(0,0.1)) = 0.005
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf NoLight noambient nolightmap nodirlightmap

        sampler2D _DayTex;
        sampler2D _NightTex;
        sampler2D _CloudTex;
        float _NightIntensity;
        float _CloudOpacity;
        float _ShadowAngle;
        float _Transition;
        float _CloudSpeed;

        struct Input
        {
            float2 uv_DayTex;
        };

        half4 LightingNoLight(SurfaceOutput s, half3 lightDir, half atten)
        {
            return half4(s.Albedo, s.Alpha);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Calcular blend dia/noche
            float diff = IN.uv_DayTex.x - _ShadowAngle;
            diff = diff - floor(diff + 0.5);
            float nightZone = abs(diff) - 0.25;
            float blend = smoothstep(-_Transition, _Transition, nightZone);

            // Texturas
            fixed4 dayColor   = tex2D(_DayTex,   IN.uv_DayTex);
            fixed4 nightColor = tex2D(_NightTex, IN.uv_DayTex) * _NightIntensity;

            // Nubes con movimiento propio
            float2 cloudUV = IN.uv_DayTex + float2(_Time.y * _CloudSpeed, 0);
            fixed4 cloudColor = tex2D(_CloudTex, cloudUV);

            // Mezclar dia y noche
            fixed3 surface = lerp(dayColor.rgb, nightColor.rgb, 1.0 - blend);

            // Nubes visibles de dia, invisibles de noche
            float cloudVisibility = cloudColor.r * _CloudOpacity * blend;
            surface = lerp(surface, fixed3(1,1,1), cloudVisibility);

            o.Albedo = surface;
        }
        ENDCG
    }
    FallBack "Diffuse"
}