Shader "Custom/FractalTest" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
		_Time ("Time", Float) = 1
		_Color1 ("Color 1", Color) = (1, 1, 1, 1)
		_Color2 ("Color 1", Color) = (0, 0, 0, 1)
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
				// Magic value?
                float4 pos : SV_POSITION;
				// Because semantics
				float4 wPos : TEXCOORD0;
                fixed3 color : COLOR0;
            };
			
			// 3D simplex noise. Not
			float noise(float xin, float yin, float zin) {
				return (sin(xin*yin*zin*500)/2+0.5);
			}
			
			// Perlin stuff
			/*float noise3(float vec[3])
			{
				int bx0, bx1, by0, by1, bz0, bz1, b00, b10, b01, b11;
				float rx0, rx1, ry0, ry1, rz0, rz1, *q, sy, sz, a, b, c, d, t, u, v;
				register i, j;
				if (start) 
				{
					start = 0;
					init();
				}
				setup(0, bx0,bx1, rx0,rx1);
				setup(1, by0,by1, ry0,ry1);
				setup(2, bz0,bz1, rz0,rz1);
				i = p[ bx0 ];
				j = p[ bx1 ];
				b00 = p[ i + by0 ];
				b10 = p[ j + by0 ];
				b01 = p[ i + by1 ];
				b11 = p[ j + by1 ];
				t = s_curve(rx0);
				sy = s_curve(ry0);
				sz = s_curve(rz0);
				#define at3(rx,ry,rz) ( rx * q[0] + ry * q[1] + rz * q[2] )
				2 - 20
				q = g3[ b00 + bz0 ] ; u = at3(rx0,ry0,rz0);
				q = g3[ b10 + bz0 ] ; v = at3(rx1,ry0,rz0);
				a = lerp(t, u, v);
				q = g3[ b01 + bz0 ] ; u = at3(rx0,ry1,rz0);
				q = g3[ b11 + bz0 ] ; v = at3(rx1,ry1,rz0);
				b = lerp(t, u, v);
				c = lerp(sy, a, b);
				q = g3[ b00 + bz1 ] ; u = at3(rx0,ry0,rz1);
				q = g3[ b10 + bz1 ] ; v = at3(rx1,ry0,rz1);
				a = lerp(t, u, v);
				q = g3[ b01 + bz1 ] ; u = at3(rx0,ry1,rz1);
				q = g3[ b11 + bz1 ] ; v = at3(rx1,ry1,rz1);
				b = lerp(t, u, v);
				d = lerp(sy, a, b);
				return lerp(sz, c, d);
			}*/


            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.wPos = v.vertex;
                o.color = v.normal * 0.5 + 0.5;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float grayscale = noise (i.wPos.x, i.wPos.y, i.wPos.z);
                return lerp (_Color1, _Color2, grayscale);
            }
			
            ENDCG

        }
	} 
	FallBack "Diffuse"
}
