precision mediump float;
varying vec4 vColor;
varying float   timer;
varying vec2    uv;
float   t;

#define I_MAX   50
#define E       0.0001

/*
* License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
*/

float   de(vec3 pos);
float   de(vec3 pos, vec4 rad);
float   sdTorus( vec3 p, vec2 t );
vec4    march(vec3 pos, vec3 dir);
vec3    camera(vec2 u);
vec3    calcNormal(in vec3 pos, float e, vec3 dir);
float   smin(float a, float b, float k);
vec2    rot(vec2 p, vec2 ang);
vec2    u;

// must be >= 2
#define NUM_BALLS 20

vec4    balls[NUM_BALLS];

float hash( float n ) {
    return fract(sin(n)*43758.5453123);
}

float noise( in vec2 x ) {
    vec2 p = floor(x);
    vec2 f = fract(x);

    f = f*f*(3.0-2.0*f);

    float n = p.x + p.y*57.0;

    return mix(mix( hash(n+  0.0), hash(n+  1.0),f.x), mix( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y);
}
//3.4*noise(vec2(float(i), float(i)+0.005*sin(t)))
void    init_balls(inout vec4 balls[NUM_BALLS])
{    
    for (int i = 0; i < NUM_BALLS; i++)
    {
        float angle = radians(45.*float(i)/float(NUM_BALLS)*t/3.+(45.*float(i))*t/3.);
        //float angle = radians(45.*(float(i))*t/3.);
        mat2 matRotate = mat2(2.*cos(angle/1.), -2.*sin(angle/1.),
                        2.*sin(angle/1.),  2.*cos(angle/1.));
        balls[i] = vec4(1.,//+1.2*noise(vec2(1.1, sin(1.+3.*float(i)+t/5.))),
                        1.,//+1.3*noise(vec2(.1, cos(1.+3.*float(i)+t/5.))),
                        1.,
                        .125);
        
        //balls[i].xyz *= sin(t*float(i)/3.);
        balls[i].x += -5.+10.*float(i)/float(NUM_BALLS);
        balls[i].y += -1.5+2.5*sin(t*5.*float(i)/float(NUM_BALLS));
        //balls[i].x *= matRotate[0].y;
        //balls[i].xz *= matRotate;
        //balls[i].zy *= 1.5*cos(float(i)+t)*sin(angle/float(i));
        //balls[i].z *= matRotate[1].x;
        //balls[i].x +=     2.*sin(float(i)*cos(t/30.))*2.*cos(angle/3.);
        //balls[i].y += -1.+2.*cos(float(i)*cos(t/30.))*2.*cos(angle/3.);
        //balls[i].x += -1.*sin(t*float(i)/float(NUM_BALLS));
        //balls[i].xy *= matRotate;
        //balls[i].zx *= matRotate;
        //balls[i].x += balls[i].y;
        //balls[i].z += balls[i].y;
        //balls[i].y += balls[i].x;
     }
}

void main(void)
{
    t = timer/20000.;

    // giving balls position
    init_balls(balls);    
    
    u  = uv;
    u.x += 1.;
    u.y += .5;
    vec3    dir = camera(u);
    vec4    col = vec4(0.0, 0.0, 0.0, 1.0);
    vec3    pos = vec3(.0, .0, 10.0);

    vec4    inter = (march(pos, dir));

    if (inter.w <= 20.)
    {
        /*
        * taken from here : https://www.shadertoy.com/view/XsB3Rm
        */
        vec3    n = calcNormal(pos, E, dir);
        vec3    v = pos+inter.w*dir;
        vec3    ev = normalize(v - pos);
        vec3    ref_ev = reflect(ev, n);
        vec3    light_pos   = vec3(10.0, 10.0, 50.0);
        //light_pos.zx = rot(light_pos.xz, vec2(sin(t)));
        vec3    light_color = vec3(.40, .5, 0.2);
        light_color.zx = rot(vec2(1.), vec2(sin(t/3.),cos(t/3.)));
        vec3    vl = normalize(light_pos - v);
        float   diffuse  = max(0.0, dot(vl, n));
        float   specular = pow(max(0.0, dot(vl, ref_ev)), 42.);
        float angle = radians(65.*(+1.+(3.4*noise(vec2(v.x, v.y+t)))/500.)*t/3.);
        mat2 matRotate = mat2(cos(angle), -sin(angle),
                        sin(angle),  cos(angle));
        col.xyz = light_color * (specular)+ vec3(0.6, .3, 0.6) * diffuse;


    }
    gl_FragColor = col;
}

float   scene(vec3 p, vec4 balls[NUM_BALLS])
{
    float   mind = 0.;

    for (int i = 0; i < NUM_BALLS; i++)
        mind += (exp(-2. * de(p, balls[i]) ));
    return(-log(mind)/2.);
}

vec4    march(vec3 pos, vec3 dir)
{
    vec2    dist = vec2(0.0);
    vec3    p = vec3(0.0);
    vec4    step = vec4(0.0);

    for (int i = -1; i < I_MAX; ++i)
    {
        p = pos + dir * dist.y;
        dist.x = scene(p, balls);
        dist.y += dist.x;
        if (dist.x < E || dist.y > 20.)
           break;
        step.x++;
    }
    step.w = (step.x > 0.) ? dist.y : -1.;
    return (step);
}

float sdTorus( vec3 p, vec2 t )
{
    vec2 q = vec2(length(p.xy)-t.x,p.z);

    q.y *= rot(q.xy, vec2(-1., 0.)).x;
    return length(q)-t.y;
}


float de(vec3 pos, vec4 rad)
{
    float   r = length(pos+rad.xyz)-rad.w;

    return (r);
}

vec2    rot(vec2 p, vec2 ang)
{
    float   c = cos(ang.x);
    float   s = sin(ang.y);
    mat2    m = mat2(c, -s, s, c);
    
    return (p * m);
}

//taken from an iq shader
vec3 calcNormal( in vec3 pos, float e, vec3 dir)
{
    vec3 eps = vec3(e,0.0,0.0);

    return normalize(vec3(
           march(pos+eps.xyy, dir).w - march(pos-eps.xyy, dir).w,
           march(pos+eps.yxy, dir).w - march(pos-eps.yxy, dir).w,
           march(pos+eps.yyx, dir).w - march(pos-eps.yyx, dir).w ));
}

vec3    camera(vec2 u)
{
    float   fov = 1.;
    vec3    forw  = vec3(0.0, 0.0, -1.0);
    vec3    right = vec3(1.0, 0.0, 0.0);
    vec3    up    = vec3(0.0, 1.0, 0.0);

    return (normalize((u.x-1.) * right + (u.y-0.5) * up + fov * forw));
}

//http://iquilezles.org/www/articles/smin/smin.htm
float smin( float a, float b, float k )
{
    float res = exp( -k*a ) + exp( -k*b );
    return -log( res )/k;
}
