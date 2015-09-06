Shader "Custom/FractalTest 3" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
		_Time ("Time", Float) = 1
		_Color1 ("Color 1", Color) = (1, 1, 1, 1)
		_Color2 ("Color 1", Color) = (0, 0, 0, 1)
		_Seed ("Seed", Int) = 1
		// TODO: think about naming
		_Frequency ("Frequency", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {

            CGPROGRAM
			
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			
			fixed4 _Color1;
			fixed4 _Color2;
			//float _Time;

            struct v2f {
                float4 pos : SV_POSITION;
				float4 oPos : TEXCOORD0;
                fixed3 color : COLOR0;
            };
			
			float saw (float x, float a)
			{
				x = abs(x);
				return (x%a)/a;	
			}
			
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.oPos = v.vertex;
                o.color = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float value = saw (i.oPos.x * i.oPos.y * i.oPos.z, 0.2);
				return lerp (_Color1, _Color2, value);
            }
			
            ENDCG

        }
	}
}
