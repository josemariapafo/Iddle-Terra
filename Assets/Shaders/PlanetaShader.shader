Shader "Custom/Planeta"
{
    Properties
    {
        _DayTex ("Textura Día", 2D) = "white" {}
        _NightTex ("Textura Noche", 2D) = "black" {}
        _CloudTex ("Textura Nubes", 2D) = "white" {}
        _CloudOpacity ("Opacidad Nubes", Range(0,1)) = 0.8
        _NightIntensity ("Intensidad Noche", Range(0,2)) = 1.2
        _Smoothness ("Suavidad", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _DayTex;
        sampler2D _NightTex;
        sampler2D _CloudTex;
        float _CloudOpacity;
        float _NightIntensity;
        float _Smoothness;

        struct Input
        {
            float2 uv_DayTex;
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Dirección de la luz solar
            float3 lightDir = normalize(float3(1, 0.5, 0));
            
            // Cuánto mira esta parte del planeta hacia el sol
            float dotProduct = dot(IN.worldNormal, lightDir);
            
            // Zona de transición suave entre día y noche
            float blend = smoothstep(-0.25, 0.25, dotProduct);            
            // Texturas
            fixed4 dayColor   = tex2D(_DayTex,   IN.uv_DayTex);
            fixed4 nightColor = tex2D(_NightTex, IN.uv_DayTex) * _NightIntensity;
            fixed4 cloudColor = tex2D(_CloudTex, IN.uv_DayTex);
            
            // Mezclar día y noche
            fixed4 surface = lerp(nightColor, dayColor, blend);
            
            o.Albedo = surface.rgb;
            o.Gloss  = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}