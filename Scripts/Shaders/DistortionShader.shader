Shader "Hidden/DistortionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
				_NoiseTex ("Noise Texture", 2D) = "white" {}
				_SpeedX ("Speed X", float) = 1.0
				_SpeedY ("Speed Y", float) = 1.0
    }
    SubShader
    {
			Tags {"RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
								float2 noiseUV : TEXCOORD1;
								float4 vertex : SV_POSITION;
            };

						sampler2D _MainTex;
						sampler2D _NoiseTex;
						float4 _NoiseTex_ST;
						float _SpeedX, _SpeedY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
								o.noiseUV = TRANSFORM_TEX(v.uv, _NoiseTex);
								return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
								float2 noiseSampLoc = float2(_Time.y * _SpeedX, _Time.y * _SpeedY);
								fixed4 noiseSamp = tex2D(_NoiseTex, i.noiseUV + noiseSampLoc) *.1;
								fixed4 r = tex2D(_MainTex, i.uv + noiseSamp.x);
								r = r * r.a;
								fixed4 c = tex2D(_MainTex, i.uv);
								r *= 1 - c.a;
                return c + r;
            }
            ENDCG
        }
    }
}
