Shader "Custom/SimplePaintShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _PaintTex ("Paint Texture", 2D) = "white" {}
        _AgeTex ("Age Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade preserve_to_alpha

        sampler2D _MainTex, _PaintTex, _AgeTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 paint = tex2D(_PaintTex, IN.uv_MainTex);
            float age = clamp(tex2D(_AgeTex, IN.uv_MainTex).r, 0.0, 1.0); // Ensure age is clamped within the shader as well
            fixed3 colorTransition = lerp(fixed3(1, 0, 0), fixed3(0, 0, 1), age); // Red to blue transition
            o.Albedo = colorTransition * paint.a; // Apply color based on paint alpha
            o.Alpha = (1 - age) * paint.a; // Fade out based on age, making sure it goes to zero
        }
        ENDCG
    }
    FallBack "Transparent"
}
