Shader "Custom/SimplePaintShader"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _PaintTex("Paint Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On // Ensure depth is written to avoid rendering issues

        CGPROGRAM
        #pragma surface surf Standard alpha:fade preserve_to_alpha

        sampler2D _MainTex, _PaintTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 p = tex2D(_PaintTex, IN.uv_MainTex);
            o.Albedo = p.rgb;
            o.Alpha = p.a; // Use the alpha of the paint texture to control visibility
        }
        ENDCG
    }
        FallBack "Transparent"
}
