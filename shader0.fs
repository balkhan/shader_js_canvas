precision mediump float;
varying vec4 vColor;
varying float   timer;
varying vec2    uv;
float   t;

#define I_MAX   100
#define E       0.0000001
#define PI      3.14
// change this from 0 to 2
#define MOBIUS  0
// change this from 0 to 4
#define SHAPE   0
// abs(POWER) > 1.0 and an integer
#define POWER   9.0

vec3    camera(vec2 uv);
void    rotate(inout vec2 v, float angle);
float   DE(vec3 p);
vec4    march(vec3 pos, vec3 dir);

void main(void)
{
    t = timer/20000.;
    vec2    u = uv;
    vec3    col = vec3(0.0);
    u.y += 1.5;
    vec3    dir = camera(u);
    vec3    pos = vec3(2.0*cos(2.0+t), -6.0, 5.0);

    vec4 inter = (march(pos, dir));
    col.xyz = inter.xyz;
    gl_FragColor = vec4(col, 1.0);
}

float DE(vec3 p)
{
    float mobius;
    if (0 == MOBIUS || 2 < MOBIUS)
        mobius = (1.0-1.0/(POWER)) * atan(PI*(p.y*p.x)*sin(t), PI*(-p.y*p.x)*cos(t));
    else if (1 == MOBIUS)
        mobius = (1.0-1.0/(POWER)) * atan(PI*(p.z*p.x)*sin(t)- PI*(p.z/p.x)*cos(t));
    else if (2 == MOBIUS)
        mobius = (1.0-1.0/(POWER)) * atan(PI*(p.y*p.x)*sin(t)* PI*(p.y*p.x)*cos(t));
    p.x = length(p.xy) - 1.2;
    rotate(p.xz, mobius);
    float m = (1.0 - ((1.0+POWER)))/(1.0*PI);
    float angle = floor(0.5 + m * (PI/2.0-atan(p.x,p.z)))/m;
    rotate(p.xz, angle);
    p.x -= 0.8+abs(cos(t)/2.4);
    if (0 == SHAPE || 4 < SHAPE)
        return length(p.xz)-0.2+abs(sin(t*2.0))*0.1;
    else if (1 == SHAPE)
        return length(p.xy)-0.4-abs(sin(t*3.0))*0.2; // 2-torus
    else if (2 == SHAPE)
        return length(p.yx)-length(p.xz)-0.72+abs(sin(t*3.0))*0.4; // surface
    else if (3 == SHAPE)
        return length(p.yx)-length(p.yz)+0.2-abs(sin(t*2.0))*0.2; // torus construction
    else if (4 == SHAPE)
        return length(p.x+p.y/2.0)-0.2+abs(sin(t*2.0))*0.1; // torus interior from exterior
}

vec4    march(vec3 pos, vec3 dir)
{
    vec2    dist = vec2(0.0);
    vec3    p = vec3(0.0);
    vec4    step = vec4(0.0);

    for (int i = -1; i < I_MAX; ++i)
    {
        p = pos + dir * dist.y;
        dist.x = DE(p);
        dist.y += dist.x;
        if (dist.y < E)
           break;
        step.w++;
    }
    // cheap colors
    step.x = ((abs(sin(t)*10.0)+10.0)/dist.y)/10.0;
    step.y = ((abs(sin(6.0/PI+t)*10.0)+10.0)/dist.y)/10.0;
    step.z = ((abs(sin(12.0/PI+t)*10.0)+10.0)/dist.y)/10.0;
    return (step);
}

vec3    camera(vec2 uv)
{
    float   fov = 1.;
    vec3    forw  = vec3(0.0, 0.0, -1.0);
    vec3    right = vec3(1.0, 0.0, 0.0);
    vec3    up    = vec3(0.0, 1.0, 0.0);

    return (normalize((uv.x) * right + (uv.y) * up + fov * forw));
}

void rotate(inout vec2 v, float angle)
{
    v = vec2(cos(angle)*v.x+sin(angle)*v.y,-sin(angle)*v.x+cos(angle)*v.y);
}