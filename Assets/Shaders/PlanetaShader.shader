Shader "Custom/Planeta"
{
    Properties
    {
        _DayTex ("Day Texture", 2D) = "white" {}
        _NightTex ("Night Texture", 2D) = "black" {}
        _NightIntensity ("Night Intensity", Range(0,2)) = 1.5
        _ShadowAngle ("Shadow Angle", Range(0,1)) = 0.0
        _Transition ("Transition", Range(0.01, 0.3)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf NoLight noambient nolightmap nodirlightmap

        sampler2D _DayTex;
        sampler2D _NightTex;
        float _NightIntensity;
        float _ShadowAngle;
        float _Transition;

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
        float longitude = IN.uv_DayTex.x;

        // Distancia a la linea de sombra
        float diff = longitude - _ShadowAngle;
        diff = diff - floor(diff + 0.5);

        // Noche ocupa la mitad del planeta (0.5 en UV)
        // Suavizar los DOS bordes
        float nightZone = abs(diff) - 0.25;
        float blend = smoothstep(-_Transition, _Transition, nightZone);

        fixed4 dayColor   = tex2D(_DayTex,   IN.uv_DayTex);
        fixed4 nightColor = tex2D(_NightTex, IN.uv_DayTex) * _NightIntensity;

        o.Albedo = lerp(dayColor.rgb, nightColor.rgb, 1.0 - blend);
        }
        ENDCG
    }
    FallBack "Diffuse"
}