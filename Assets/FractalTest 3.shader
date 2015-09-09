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
			#pragma enable_d3d11_debug_symbols
			
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
			
			float blendf (float x)
			{
				return 6*pow(x, 5) - 15*pow(x, 4) + 10*pow(x, 3);
			}
			
			float length (float2 g)
			{
				return sqrt(g.x*g.x + g.y*g.y);
			}
			
			float length (float3 g)
			{
				return sqrt(g.x*g.x + g.y*g.y + g.z*g.z);
			}
			
			float2 normalize (float2 g)
			{
				return g / length(g);
			}
			
			float3 normalize (float3 g)
			{
				return g / length(g);
			}
			
			float dot (float2 g1, float2 g2)
			{
				return g1.x*g2.x + g1.y*g2.y;
			}
			
			float dist (float2 g1, float2 g2)
			{
				float2 d = g2 - g1;
				return length (d);
			}
			
			// Project a onto b
			float2 project (float2 a, float2 b)
			{
				return dot (a, b)/length(b);
			}
			
			// 1d white noise
			float random (float x, int primeSeed)
			{
				// I stole this from somewhere (and then butchered it)
				// TODO: Remember where I found this and give credit or whatever
				int r = int (x);	// TODO: Refactor, this is really stupid and counterintuitive (one might expect to get different numbers in-between whole numbers. Fools :D)
				r = (r<<13) ^ r;
				return ( 1.0 - ( (r * (r * r * primeSeed + 789221) + 1376312589) & 2147483647) / 1073741824.0);
			}
			
			// 2d white noise
			float random2d (float x, float y, int primeSeed)
			{
				return random (x+y*101, primeSeed);
			}
			
			// 3d white noise
			float random3d (float x, float y, float z, int primeSeed)
			{
				return random (x+y*101+z*99991, primeSeed);
			}
			
			float2 randomGradient2d (float x, float y)
			{
				float val = x+y*101;
				// TODO: improve randomness
				float xg = (random(val, 78779));
				float yg = (random(val, 81439));
				
				float2 gradient = float2 (xg, yg);
				return normalize(gradient);
			}
			
			float2 randomGradient2d (float2 p)
			{
				return randomGradient2d (p.x, p.y);
			}
			
			float3 randomGradient3d (float x, float y, float z)
			{
				// TODO: improve randomness
				float xg = (random3d(x, y, z, 78779));
				float yg = (random3d(x, y, z, 81439));
				float zg = (random3d(x, y, z, 55901));
				
				float3 gradient = float3 (xg, yg, zg);
				return normalize(gradient);
			}
			
			float3 randomGradient3d (float3 p)
			{
				return randomGradient3d (p.x, p.y, p.z);
			}
			
			//--------------------------------------------------------------------------------
			//--------------------------- STUFF THAT DOES STUFF ------------------------------
			//--------------------------------------------------------------------------------

			float perlin2d (float x, float y)
			{
				// Get gradients of corners
				float xf = floor(x);
				float yf = floor(y);
				float2 g1 = randomGradient2d (xf, yf);
				float2 g2 = randomGradient2d (xf+1, yf);
				float2 g3 = randomGradient2d (xf, yf+1);
				float2 g4 = randomGradient2d (xf+1, yf+1);
				
				// Calculate heights from gradients 
				float2 a1 = float2 (x, y) - float2 (xf, yf);
				a1 -= g1 * 0.5; //< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b1 = g1;
				float h1 = project (a1, b1);
				h1 += 0.5;
				
				float2 a2 = float2 (x, y) - float2 (xf+1, yf);
				a2 -= g2 * 0.5; //< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b2 = g2;
				float h2 = project (a2, b2);
				h2 += 0.5;
				
				float2 a3 = float2 (x, y) - float2 (xf, yf+1);
				a3 -= g3 * 0.5; //< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b3 = g3;
				float h3 = project (a3, b3);
				h3 += 0.5;
				
				float2 a4 = float2 (x, y) - float2 (xf+1, yf+1);
				a4 -= g4 * 0.5;	//< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b4 = g4;
				float h4 = project (a4, b4);
				h4 += 0.5;
				
				// Calculate inverse distances to point
				float d1 = 1/dist (float2(xf, yf), float2(x, y));
				float d2 = 1/dist (float2(xf+1, yf), float2(x, y));
				float d3 = 1/dist (float2(xf, yf+1), float2(x, y));
				float d4 = 1/dist (float2(xf+1, yf+1), float2(x, y));
				
				// Calculate infuences
				float d1234 = d1+d2+d3+d4;
				float i1 = d1/d1234;
				float i2 = d2/d1234;
				float i3 = d3/d1234;
				float i4 = d4/d1234;
				
				// Smoothen influences
				i1 = blendf (i1);
				i2 = blendf (i2);
				i3 = blendf (i3);
				i4 = blendf (i4);
				
				// Blend heights
				float h = h1*i1 + h2*i2 + h3*i3 + h4*i4;
				
				h += 0.5;
				
				return h;
			}
			
			float perlin3d (float x, float y, float z)
			{
				// Get gradients of corners
				float xf = floor(x);
				float yf = floor(y);
				float zf = floor(z);
				// Their coordinates
				float3 c1 = float3 (xf, yf, zf);
				float3 c2 = float3 (xf+1, yf, zf);
				float3 c3 = float3 (xf+1, yf, zf+1);
				float3 c4 = float3 (xf, yf, zf+1);
				float3 c5 = float3 (xf, yf+1, zf);
				float3 c6 = float3 (xf+1, yf+1, zf);
				float3 c7 = float3 (xf+1, yf+1, zf+1);
				float3 c8 = float3 (xf, yf+1, zf+1);
				// The actual gradients
				float2 g1 = randomGradient3d (c1);
				float2 g2 = randomGradient3d (c2);
				float2 g3 = randomGradient3d (c3);
				float2 g4 = randomGradient3d (c4);
				float2 g5 = randomGradient3d (c5);
				float2 g6 = randomGradient3d (c6);
				float2 g7 = randomGradient3d (c7);
				float2 g8 = randomGradient3d (c8);
				
				// Calculate heights from gradients 
				float2 a1 = float2 (x, y) - float2 (xf, yf);
				a1 -= g1 * 0.5; //< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b1 = g1;
				float h1 = project (a1, b1);
				h1 += 0.5;
				
				float2 a2 = float2 (x, y) - float2 (xf+1, yf);
				a2 -= g2 * 0.5; //< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b2 = g2;
				float h2 = project (a2, b2);
				h2 += 0.5;
				
				float2 a3 = float2 (x, y) - float2 (xf, yf+1);
				a3 -= g3 * 0.5; //< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b3 = g3;
				float h3 = project (a3, b3);
				h3 += 0.5;
				
				float2 a4 = float2 (x, y) - float2 (xf+1, yf+1);
				a4 -= g4 * 0.5;	//< Offset gradient (or, actually, offset pixel position, but the outcome is the same), so it's center is on the corner point
				float2 b4 = g4;
				float h4 = project (a4, b4);
				h4 += 0.5;
				
				// Calculate inverse distances to point
				float d1 = 1/dist (float2(xf, yf), float2(x, y));
				float d2 = 1/dist (float2(xf+1, yf), float2(x, y));
				float d3 = 1/dist (float2(xf, yf+1), float2(x, y));
				float d4 = 1/dist (float2(xf+1, yf+1), float2(x, y));
				
				// Calculate infuences
				float d1234 = d1+d2+d3+d4;
				float i1 = d1/d1234;
				float i2 = d2/d1234;
				float i3 = d3/d1234;
				float i4 = d4/d1234;
				
				// Smoothen influences
				i1 = blendf (i1);
				i2 = blendf (i2);
				i3 = blendf (i3);
				i4 = blendf (i4);
				
				// Blend heights
				float h = h1*i1 + h2*i2 + h3*i3 + h4*i4;
				
				h += 0.5;
				
				return h;
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
				return perlin2d (i.oPos.x, i.oPos.y);
				//return perlin3d (i.oPos.x, i.oPos.y, i.oPos.z);
            }
			
            ENDCG

        }
	}
}
