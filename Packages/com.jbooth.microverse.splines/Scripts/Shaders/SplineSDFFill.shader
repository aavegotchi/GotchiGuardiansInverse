
Shader "Hidden/MicroVerse/SplineSDFFill"
{
    Properties
    {
        _NumSegments("Number of Segments", Int) = 120
        _MainTex("Main", 2D) = "white" {}
        _Prev("Prev", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local_fragment _ _EDGES
            #pragma shader_feature_local_fragment _ _WIDTHSMOOTHSTEP _WIDTHEASEIN _WIDTHEASEOUT _WIDTHEASEINOUT

            #include "UnityCG.cginc"
            #include "Packages/com.unity.splines/Shader/Spline.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 _RealSize;
            float4x4 _Transform;
            sampler2D _Prev;
            sampler2D _MainTex;
            int _IsArea;

            SplineInfo _Info = float4(0, 0, 0, 0);
            float4 _WidthInfo;
            float _WidthBoost;
            StructuredBuffer<BezierCurve> _Curves;
            StructuredBuffer<float> _CurveLengths;
            StructuredBuffer<float2> _Widths;
            uint _NumSegments;

            v2f vert(vertexInput v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            int solve_cubic(float3 coeffs, inout float3 r)
            {

	            float a = coeffs.z;
	            float b = coeffs.y;
	            float c = coeffs.x;

	            float p = b - a*a / 3.0;
	            float q = a * (2.0*a*a - 9.0*b) / 27.0 + c;
	            float p3 = p*p*p;
	            float d = q*q + 4.0*p3 / 27.0;
	            float offset = -a / 3.0;
	            if(d >= 0.0) { // Single solution
		            float z = sqrt(d);
		            float u = (-q + z) / 2.0;
		            float v = (-q - z) / 2.0;
		            u = sign(u)*pow(abs(u),1.0/3.0);
		            v = sign(v)*pow(abs(v),1.0/3.0);
		            r.x = offset + u + v;	

		            //Single newton iteration to account for cancellation
		            float f = ((r.x + a) * r.x + b) * r.x + c;
		            float f1 = (3. * r.x + 2. * a) * r.x + b;

		            r.x -= f / f1;

		            return 1;
	            }
	            float u = sqrt(-p / 3.0);
	            float v = acos(-sqrt( -27.0 / p3) * q / 2.0) / 3.0;
	            float m = cos(v), n = sin(v)*1.732050808;

	            //Single newton iteration to account for cancellation
	            //(once for every root)
	            r.x = offset + u * (m + m);
                r.y = offset - u * (n + m);
                r.z = offset + u * (n - m);

	            float3 f = ((r + a) * r + b) * r + c;
	            float3 f1 = (3. * r + 2. * a) * r + b;

	            r -= f / f1;

	            return 3;
            }

            int cubic_bezier_sign(float2 uv, float2 p0, float2 p1, float2 p2, float2 p3){

	            float cu = (-p0.y + 3. * p1.y - 3. * p2.y + p3.y);
	            float qu = (3. * p0.y - 6. * p1.y + 3. * p2.y);
	            float li = (-3. * p0.y + 3. * p1.y);
	            float co = p0.y - uv.y;

	            float3 roots = 1e38;
	            int n_roots = solve_cubic(float3(co/cu,li/cu,qu/cu),roots);

	            int n_ints = 0;

	            for(int i=0;i<3;i++){
		            if(i < n_roots){
			            if(roots[i] >= 0. && roots[i] <= 1.){
				            float x_pos = -p0.x + 3. * p1.x - 3. * p2.x + p3.x;
				            x_pos = x_pos * roots[i] + 3. * p0.x - 6. * p1.x + 3. * p2.x;
				            x_pos = x_pos * roots[i] + -3. * p0.x + 3. * p1.x;
				            x_pos = x_pos * roots[i] + p0.x;

				            if(x_pos < uv.x){
					            n_ints++;
				            }
			            }
		            }
	            }
                return n_ints;
            }

            float length2( float2 v ) { return dot(v,v); }

            float segment_dis_sq( float2 p, float2 a, float2 b ){
	            float2 pa = p-a, ba = b-a;
	            float h = saturate( dot(pa,ba)/dot(ba,ba));
	            return length2( pa - ba*h );
            }

            float4 cubic_bezier_segments_dis_sq(float2 uv, float3 p0, float3 p1, float3 p2, float3 p3, out float _t)
            {
                int numSeg = max(_NumSegments, 2);
                float d0 = 1e38;
                float3 a = p0;
                float3 minPos = 99999;
                _t = 0;
                for( int i=1; i<numSeg; i++ )
                {
                    float t = float(i)/float(numSeg-1);
                    float s = 1.0-t;
                    float3 b = p0*s*s*s + p1*3.0*s*s*t + p2*3.0*s*t*t + p3*t*t*t;
                    float nd = segment_dis_sq(uv, a.xz, b.xz);
                    if (nd < d0)
                    {
                        d0 = nd;
                        minPos = b;
                        _t = t;
                    }
                    a = b;
                }
    
                return float4(d0, minPos);
            }



            float4 frag(v2f i) : SV_Target
            {
                float4 last = tex2D(_MainTex, i.uv);
                #if _EDGES
                    UNITY_BRANCH
                    if (i.uv.x > 0.01 && i.uv.y > 0.01 && i.uv.y < 0.99 && i.uv.y < 0.99)
                    {
                        return tex2D(_Prev, i.uv);
                    }
                #endif

                float2 position = i.uv * _RealSize.xz;
                position = mul(_Transform, float4(position.x, 0, position.y, 1)).xz;

                float4 d = 99999;

                uint numIntersections = 0;
                float finalT = 0;
                for (int x = 0; x < _Info.x; ++x)
                {
                    BezierCurve bc = _Curves[x];
                    float t = 0;
                    float4 nd = cubic_bezier_segments_dis_sq(position, bc.P0, bc.P1, bc.P2, bc.P3, t);
                    if (nd.x < d.x)
                    {
                        d = nd;
                        finalT = x + t;
                    }
                    numIntersections += cubic_bezier_sign(position, bc.P0.xz, bc.P1.xz, bc.P2.xz, bc.P3.xz);
                }


                float width = 0;
                if (_WidthInfo.x == 1)
                {
                    width = _Widths[0].y;
                }
                else if (_WidthInfo.x >= 1)
                {
                    width = _Widths[0].y;
                    if (finalT >= _Widths[_WidthInfo.x-1].x)
                    {
                        width = _Widths[_WidthInfo.x-1].y;
                    }
                    else
                    {
                        for (x = 1; x < _WidthInfo.x; ++x)
                        {
                            float2 pw = _Widths[x-1];
                            float2 cw = _Widths[x];
                            if (finalT >= pw.x && finalT < cw.x)
                            {
                                float fr = max(0.001,(cw.x - pw.x));
                                float r = frac((finalT-cw.x)/fr);
                            
                                width = lerp(pw.y, cw.y, r);
                                float maxW = max(0.001, max(pw.y, cw.y));
                                width /= maxW;
                                #if _WIDTHSMOOTHSTEP
                                    width = smoothstep(0,1,width);
                                #elif _WIDTHEASEIN
                                    width *= width;
                                #elif _WIDTHEASEOUT
                                    width = 1 - (1 - width) * (1 - width);
                                #elif _WIDTHEASEINOUT
                                    width = width < 0.5 ? 2 * width * width : 1 - pow(-2 * width + 2, 2) / 2;
                                #endif

                                width *= maxW;
                            }
                        }
                    }

                }
                

                d.x = sqrt(d.x);
                d.x -= _WidthBoost;
                
                if (last.y < d.x)
                {
                    return last;
                }

                float sn =  (frac(numIntersections/2.0) > 0 ) ? -1 : 1;
                float dw = max(0, d.x - width);
                float sdf = sn * dw;

                if (_Info.y > 0 && _IsArea > 0.5)
                {
                    return float4(sdf, dw, d.z, width);
                }
                else
                {
                    return float4(dw, dw, d.z, width);
                }
            }
            ENDCG
        }
    }
}