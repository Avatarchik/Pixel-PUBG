Shader "RenderFX/Skybox Blended" {

Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    _FrontTex ("Front (+Z)", 2D) = "white" {}
    _BackTex ("Back (-Z)", 2D) = "white" {}
    _LeftTex ("Left (+X)", 2D) = "white" {}
    _RightTex ("Right (-X)", 2D) = "white" {}
    _UpTex ("Up (+Y)", 2D) = "white" {}
    _DownTex ("Down (-Y)", 2D) = "white" {}
}

SubShader {
    Tags { "Queue" = "Background" }
    Cull Off
    Fog { Mode Off }
    Lighting Off        
    Color [_Tint]
    Pass {
        SetTexture [_FrontTex] { combine texture }
        SetTexture [_FrontTex] { combine previous * primary, previous * primary }
    }
    Pass {
        SetTexture [_BackTex] { combine texture }
        SetTexture [_BackTex] { combine previous * primary, previous * primary }
    }
    Pass {
        SetTexture [_LeftTex] { combine texture }
        SetTexture [_LeftTex] { combine previous * primary, previous * primary }
    }
    Pass {
        SetTexture [_RightTex] { combine texture }
        SetTexture [_RightTex] { combine previous * primary, previous * primary }
    }
    Pass {
        SetTexture [_UpTex] { combine texture }
        SetTexture [_UpTex] { combine previous * primary, previous * primary }
    }
    Pass {
        SetTexture [_DownTex] { combine texture }
        SetTexture [_DownTex] { combine previous * primary, previous * primary }
    }
}

Fallback "RenderFX/Skybox", 1
}