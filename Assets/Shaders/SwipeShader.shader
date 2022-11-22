Shader "DLIFR/SwipShader" 
{
    Properties 
    {
        _Color ("Color", Color) = (0, 0, 0, 0)
        _Radius ("Radius", Range(0, 500)) = 5
        [ShowAsVector2] _Position ("Position", Vector) = (0, 0, 0, 0)
    }
    SubShader 
    {
        Blend SrcAlpha OneMinusSrcAlpha
        
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 100

        Lighting Off

        Pass 
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed _Radius;
                float4 _Color;
                float4 _Position;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 col = _Color; /*tex2D(_MainTex, i.texcoord);*/

                    if(_Radius > 0)
                    {
                        float2 center = float2(_Position.x, _Position.y); 
                        //float2 center = float2(_ScreenParams.x * _Position.x, _ScreenParams.y * _Position.y); 
                        float2 pos = float2(i.vertex.x, i.vertex.y);

                        float a = round(distance(pos, center) / _Radius);
                        col.a = a == 0 ? 0 : col.a;
                    }
                    
                    //col.a = _Progress == 0 ? col.a : round(distance(pos, center) / (m * _Progress));
                    return col;
                }
            ENDCG
        }
    }
}