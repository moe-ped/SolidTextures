Shader "Custom/FractalTest 3" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
		_Time ("Time", Float) = 1
		_Color1 ("Color 1", Color) = (1, 1, 1, 1)
		_Color2 ("Color 1", Color) = (0, 0, 0, 1)
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
			
			//--------------------------------------------------------------------------------
			//--------------------------- HELPER FUNCTIONS -----------------------------------
			//--------------------------------------------------------------------------------
			
			float2 normalize (float2 gradient)
			{
				float length = sqrt(gradient.x*gradient.x + gradient.y*gradient.y);
				return gradient / length;
			}
			
			float dot (float2 g1, float2 g2)
			{
				return g1.x*g2.x+g1.y*g2.x;
			}
			
			float dist (float2 g1, float2 g2)
			{
				float2 d = g2 - g1;
				return sqrt(d.x*d.x+d.y*d.y);
			}
			
			float random (float number, int primeSeed)
			{
				int x = int (number);
				x = (x<<13) ^ x;
				return ( 1.0 - ( (x * (x * x * primeSeed + 789221) + 1376312589) & 2147483647) / 1073741824.0);
			}
			
			float2 randomGradient (float x, float y)
			{
				float val = x+y*1000;
				// TODO: improve randomness
				float xg = (random(val, 78779));
				float yg = (random(val, 81439));
				
				float2 gradient = float2 (xg, yg);
				return normalize(gradient);
			}
			
			//--------------------------------------------------------------------------------
			//--------------------------- STUFF THAT DOES STUFF ------------------------------
			//--------------------------------------------------------------------------------
			
			// failed perlin attempt
			float noise2d (float x, float y)
			{
				// Get gradients of corners
				float xf = floor(x);
				float yf = floor(y);
				float2 g1 = randomGradient (xf, yf);
				float2 g2 = randomGradient (xf+1, yf);
				float2 g3 = randomGradient (xf, yf+1);
				float2 g4 = randomGradient (xf+1, yf+1);
				
				// Get vectors from corners to point
				float2 d1 = float2(x, y) - float2(xf, yf);
				float2 d2 = float2(x, y) - float2(xf+1, yf);
				float2 d3 = float2(x, y) - float2(xf, yf+1);
				float2 d4 = float2(x, y) - float2(xf+1, yf+1);
				
				// Calculate influences
				float i1 = dot (g1, d1);
				float i2 = dot (g2, d2);
				float i3 = dot (g3, d3);
				float i4 = dot (g4, d4);
				
				// Calculate average influence i guess ...
				// TODO: make s-shape thingy
				float avg = (i1+i2+i3+i4)/4;
				
				return avg;
			}
			
			float noise2d2 (float x, float y)
			{
				// Get heights at corners
				float xf = floor(x);
				float yf = floor(y);
				float g1 = random (xf+yf*1000, 81619);
				float g2 = random (xf+1+yf*1000, 81619);
				float g3 = random (xf+(yf+1)*1000, 81619);
				float g4 = random (xf+1+(yf+1)*1000+1, 81619);
				
				// Get inverse length of distances
				float l1 = 1/dist(float2(x, y), float2(xf, yf));
				float l2 = 1/dist(float2(x, y), float2(xf+1, yf));
				float l3 = 1/dist(float2(x, y), float2(xf, yf+1));
				float l4 = 1/dist(float2(x, y), float2(xf+1, yf+1));
				
				// get average
				float avg = (g1*l1+g2*l2+g3*l3+g4*l4)/(l1+l2+l3+l4);
				
				return avg;
			}
			
			//--------------------------------------------------------------------------------
			//--------------------------- ACTUAL SHADER STUFF --------------------------------
			//--------------------------------------------------------------------------------
			
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
				return noise2d2 (i.oPos.x, i.oPos.y);
				//return float4(random(floor(i.oPos.x), 81619), -random(floor(i.oPos.x), 81619), random(floor(i.oPos.y), 83443), 1);
            }
			
            ENDCG

        }
	}
}
