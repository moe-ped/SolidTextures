Shader "Custom/FractalTest 2" {
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
			float _Seed;
			float _Frequency;

            struct v2f {
                float4 pos : SV_POSITION;
				float4 oPos : TEXCOORD0;
                fixed3 color : COLOR0;
            };
			
			// TODO: find better noise algorithm
			float random (float x)
			{
				return abs(((sin(x*99907) * 77723 + _Seed + (x*23417)) % 832) / 832);
			}
			
			// TODO: fix
			float random2 (float x, float y)
			{
				return random(x + (y*83));
			}
			
			float random3 (float x, float y, float z)
			{
				return random(x + (y*83) + (z*83*83));
			}
			
			// returns 2d vector noise for certain grid points
			float2 grid2 (float x, float y)
			{
				return float2 (random(x), random(y));
			}
			
			float3 grid3 (float x, float y, float z)
			{
				return float3 (random(x), random(y), random(z));
			}
			
			float noise2(float x, float y) {
				
			}
			
			float noise3 (float x, float y, float z)
			{
				// Round -> grid thingy
				float x0 = floor(x/_Frequency)*_Frequency;
				float y0 = floor(y/_Frequency)*_Frequency;
				float z0 = floor(z/_Frequency)*_Frequency;
				float h0 = random2(x0, z0);
				
				float x1 = floor(x/_Frequency)*_Frequency;
				float y1 = floor(y/_Frequency)*_Frequency;
				float z1 = floor((z-_Frequency)/_Frequency)*_Frequency;
				float h1 = random2(x1, z1);
				
				float x2 = floor((x+_Frequency)/_Frequency)*_Frequency;
				float y2 = floor(y/_Frequency)*_Frequency;
				float z2 = floor((z-_Frequency)/_Frequency)*_Frequency;
				float h2 = random2(x2, z2);
				
				float x3 = floor((x+_Frequency)/_Frequency)*_Frequency;
				float y3 = floor(y/_Frequency)*_Frequency;
				float z3 = floor(z/_Frequency)*_Frequency;
				float h3 = random2(x3, z3);
				
				// Smooth
				float d0 = sqrt (pow((x0-x),2) + pow ((y0-y),2) + pow ((z0-z),2));
				float d1 = sqrt (pow((x1-x),2) + pow ((y1-y),2) + pow ((z1-z),2));
				float d2 = sqrt (pow((x2-x),2) + pow ((y2-y),2) + pow ((z2-z),2));
				float d3 = abs(sqrt (pow((x3-x),2) + pow ((y3-y),2) + pow ((z3-z),2)));
				
				return h0 * d0 + h1 * d1 + h2 * d2 + h3 * d3;
				//return d3;
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
				float x = i.oPos.x;
				float y = i.oPos.y;
				float z = i.oPos.z;
				
				float h = noise3 (x, y, z);
				return fixed4 (h, h, h, 1);
            }
			
            ENDCG

        }
	} 
}
