Shader "Unlit/Appear"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FromColor ("Color", Color) = (.27,.66,1,1)
        _SwirlAngle ("swirl", Range(0,2)) = 2 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
               
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 vertex2: TEXCOORD2;
            };
            fixed4 _FromColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SwirlAngle;
            float3 rotation(float3 vertex, float angle)
            {
                float c = cos(angle);
	            float s = sin(angle);
                float4x4 m =  
                    float4x4 (
                    c,-s,0,0,
	   		        s,c,0,0,
			        0,0,1,0,
			        0,0,0,1);
                 
                return mul(m, vertex);
            }

            float2 rotateuv(float2 uv, float angle)
            {
                uv -= 0.5;
                 float c = cos(angle);
                
	            float s =  sin(angle);
               
                float2x2 m = float2x2(c,-s,
                                      s,c);
                m *= 0.5;
                m += 0.5;
                m = m * 2 - 1;
               float2 temp = mul(uv,m);
               temp += 0.5;
                return temp;
                   

            }

            fixed4 swirl(sampler2D tex, inout float2 uv, float angle) 
            {
			    float radius = .5;//_ScreenParams.x;
			    float2 center = float2(.5,.5);//(_ScreenParams.x, _ScreenParams.y);
			    float2 texSize = float2(.5/.5,.5/.5);//(_ScreenParams.x / 0.5, _ScreenParams.y / 0.5);
			    float2 tc = uv * texSize;
			    tc -= center;
			    float dist = length(tc);
			   // float angle = sin(time );
			    if (dist < radius)
			    {
				    float percent = (radius - dist) / radius;
				    float theta = percent * percent * angle * 28.0;
				    float s = sin(theta);
				    float c = cos(theta);
				    tc = float2(dot(tc, float2(c, -s)), dot(tc, float2(s, c)));
			    }
			    tc += center;
			    float4 color = tex2D(tex, tc / texSize).rgba;
			    //color.r = 1.0;
			    return color;
		    }


            v2f vert (appdata v)
            {
                v2f o;
                //v.vertex.z += 0.01* _SinTime.w;
               // float3 rotVertex = rotation( v.vertex, _Time.y);
               // float3 rotVertex2 = rotation( v.vertex,_Time.y + .5);
                //rotVertex -= 0.5;
                //rotVertex = lerp(rotVertex,0.5,_SinTime.w);
                //rotVertex += _SinTime.w;
                //rotVertex.xy -= _SinTime.w;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
               // o.vertex2 = UnityObjectToClipPos(rotVertex2);
              
                 // v.uv.y 
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               // o.uv2 = TRANSFORM_TEX(v.uv, _MainTex);
              
                //o.uv.x +=  _SinTime.w;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                    //i.uv.x += _SinTime.w/10;
                // sample the texture
               
                fixed4 col = swirl(_MainTex,i.uv,_SwirlAngle);//tex2D(_MainTex, skew(rotateuv(i.uv,_Time.y)) );
                //fixed4 col2 = tex2D(_MainTex, rotateuv(i.uv,_CosTime.w));
               
               // fixed4 col2 = tex2D(_MainTex, rotateuv(i.uv)+_SinTime.w);
                //fixed4 col2 = tex2D(_MainTex, i.uv + i.vertex2);
             
               //col.rgb = rotateuv(col);
               // fixed4 col2 = tex2D(_MainTex, i.uv + _SinTime.w);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
          // rotate(i.uv,45)
               // col.rgb = _FromColor.rgb * col;
                     //col -= 0.03;
                //col.a -= col.r;
                //col.r += .27;
                //col.g += .67;
                //col.b = 1;
                //col.a =  //_SinTime.w;
               // col.a = _SinTime.x;
              // col.rgb = lerp(_FromColor.rgb,fixed3(.20,.44,1),_SinTime.w);
             // col.a *= .5;
             float3 backcolor = float3(.27,.67,1); 
             col.rgb = lerp(col.rgb, backcolor, _SwirlAngle/2);
                return col;
            }
            ENDCG
        }
    }
}
