Shader "Custom/skinshader" {
    Properties {
        _Color ("Main Color", Color) = (1,0.5,0.5,1)
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    SubShader {
        Pass {
            Material {
                Diffuse [_Color]
            }
            Lighting On
            SetTexture [_MainTex] {
                constantColor [_Color]
                Combine texture * primary DOUBLE, texture * constant
            }
        }
    }
} 