attribute vec3 aVertexPosition;
    attribute vec4 aVertexColor;
    varying vec4 vColor;
    varying float   timer;
    varying vec2    uv;
    uniform float time;
    void main(void) {
        gl_Position = vec4(aVertexPosition, 1.0);
        vColor = vec4(aVertexColor.xyz - aVertexPosition, 1.0);
        timer = time;
        uv = aVertexPosition.xy;
    }