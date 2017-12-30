precision mediump float;
varying vec4 vColor;
varying float   timer;
varying vec2    uv;
float   t;

#define I_MAX   100
#define E       0.002

/*
* License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
*/

void    init_vars();
float   letters(vec3 p);
vec3    give_color(float inter);
float   de(vec3 pos);
float   de(vec3 pos, vec4 rad);
float   sdTorus( vec3 p, vec2 t );
vec4    march(vec3 pos, vec3 dir);
vec3    camera(vec2 u);
vec3    calcNormal(in vec3 pos, float e, vec3 dir);
float   smin(float a, float b, float k);
vec2    rot(vec2 p, vec2 ang);
vec2    u;
vec4    peanut_butter_jelly_time(vec3 pos, vec3 dir);

/*
* Distances
*/

    float   rigt_eye;
    float   left_eye;
    float   rigt_pup;
    float   left_pup;
    float   riga_leg;
    float   lefa_leg;
    float   rigb_leg;
    float   lefb_leg;
    float   rigt_arm;
    float   rigt_brm;
    float   left_arm;
    float   left_brm;
    float   left_fot;
    float   rigt_fot;
    float   left_had;
    float   rigt_had;
    float   up_cap;
    float   bt_cap;
    float   mouth;

/*
* Skeleton
*/

    vec3    a_arm_rigt;
    vec3    a_arm_left;
    vec3    b_arm_rigt;
    vec3    b_arm_left;

    vec3    a_leg_rigt;
    vec3    a_leg_left;
    vec3    b_leg_left;
    vec3    b_leg_rigt;

/*
* Distance functions taken from iq
*/

float sdCapsule( vec3 p, vec3 a, vec3 b, float r )
{
    vec3 pa = p-a, ba = b-a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

float sdTorus( vec3 p, vec2 t )
{
    vec2 q = vec2(length(p.xy)-t.x,p.z);

    return length(q)-t.y;
}

void main(void)
{
    t = timer/20000.;
    init_vars(); // init skeleton and distances
    u  = uv;
    u.x += 1.;
    vec3    dir = camera(u);
    vec4    col = vec4(0.0,0.0,0.0,1.0);
    vec3    pos = vec3(.0, .0, -10.0);

    vec4    inter = (march(pos, dir));

    vec3    v = pos+inter.w*dir;
    vec3    obj_color;
    if (inter.w <= 17.)
    {
        obj_color = give_color(inter.y);
        /*
        * taken from here : https://www.shadertoy.com/view/XsB3Rm
        */
        vec3    n = calcNormal(pos, E, dir);
        vec3    ev = normalize(v - pos);
        vec3    ref_ev = reflect(ev, n);
        vec3    light_pos   = vec3(-20.0, 10.0, -25.0);

        vec3    vl = normalize(light_pos - v);
        float   diffuse  = max(0.0, dot(vl, n));
        float   specular = pow(max(0.0, dot(vl, ref_ev)), 42.);
        col.xyz = obj_color * (diffuse + specular);
    }
    // letters
    else
    {
        col = peanut_butter_jelly_time(pos, dir);
    }
    gl_FragColor = col;
}

float   scene(vec3 p)
{
    float   t = sin(t);
    float   ct = cos(1.3+ .75*t);
    float   st = sin(1.3+ .75*t);
    float   mind = 1e5;

    mat2    rotmat = mat2(ct, st, -st, ct);
    p.xz *= rotmat;
    p.x += .5*sin(15.*t);
    p.y += .75*sin(.7+20.*t);

    rigt_eye = length(p+vec3(-1.5, -0.4, .4))-.3;
    left_eye = length(p+vec3(-1.5, -0.4, -.4))-.3;
    rigt_pup = length(p+vec3(-1.25,-0.4,0.4))-.1;
    left_pup = length(p+vec3(-1.25,-0.4,-0.4))-.1;

    mouth = sdTorus(vec3((p.y+.3),p.z-.05,(p.x-1.2)-cos(0.4-p.y))+vec3(0.), vec2(.5,.35));

    riga_leg = sdCapsule(vec3(p.x-1.,p.y+1.,p.z+1.), a_leg_rigt, vec3(.8,-.25,.6) , .15);
    lefa_leg = sdCapsule(vec3(p.x-1.,p.y+1.,p.z-1.), a_leg_left, vec3(.8,-.25,-.6) , .15);
    rigb_leg = sdCapsule(vec3(p.x-1.,p.y+1.,p.z+1.), a_leg_rigt, b_leg_rigt , .15);
    lefb_leg = sdCapsule(vec3(p.x-1.,p.y+1.,p.z-1.), a_leg_left, b_leg_left , .15);

    rigt_arm = sdCapsule(vec3(p.x-1.,p.y+1.,p.z+1.), a_arm_rigt, vec3(1.5,.8,.46) , .15);
    rigt_brm = sdCapsule(vec3(p.x-1.,p.y+1.,p.z+1.), a_arm_rigt, b_arm_rigt , .15);
    left_arm = sdCapsule(vec3(p.x-1.,p.y+1.,p.z-1.), a_arm_left, vec3(1.5,.8,-.46) , .15);
    left_brm = sdCapsule(vec3(p.x-1.,p.y+1.,p.z-1.), a_arm_left, b_arm_left , .15);

    left_had = length(vec3(p.x-1.,p.y+1.,p.z-1.)-b_arm_left) - 0.4;
    rigt_had = length(vec3(p.x-1.,p.y+1.,p.z+1.)-b_arm_rigt) - 0.4;
    left_fot = length(vec3(p.x-1.,p.y*1.5+2.5,p.z-1.)-b_leg_left) - 0.7;
    rigt_fot = length(vec3(p.x-1.,p.y*1.5+2.5,p.z+1.)-b_leg_rigt) - 0.7;

    up_cap = sdCapsule(vec3(p.y-2.4, p.z, p.x-.6), vec3(-.3, -.0, .33), vec3(.1, .0, -.31), .15);
    bt_cap = sdCapsule(vec3(p.y+2.4, p.z, p.x-.6), vec3(.3, -.0, .33), vec3(.1, .0, .0), .15);

    // 3 lanes above are the banana corpse, comment the if to reveal the cheat
    mind = sdTorus(p, vec2(2.4, cos(-(p.y)/1.50)));
    if (p.x < -.45)
        mind = 2.;
    // assembling shapes together
    mind = min(mind, rigt_eye);
    mind = min(mind, left_eye);
    mind = min(mind, rigt_pup);
    mind = min(mind, left_pup);

    mind = min(mind, riga_leg);
    mind = min(mind, lefa_leg);
    mind = min(mind, rigb_leg);
    mind = min(mind, lefb_leg);

    mind = min(mind, rigt_arm);
    mind = min(mind, rigt_brm);
    mind = min(mind, left_arm);
    mind = min(mind, left_brm);
    mind = min(mind, left_fot);
    mind = min(mind, rigt_fot);
    mind = min(mind, left_had);
    mind = min(mind, rigt_had);
    mind = min(mind, up_cap);
    mind = min(mind, bt_cap);

    //carving comes last
    mind = max(mind, -mouth);

    return(mind);
}

vec4    march(vec3 pos, vec3 dir)
{
    vec2    dist = vec2(0.0);
    vec3    p = vec3(0.0);
    vec4    step = vec4(0.0);

    for (int i = -1; i < I_MAX; ++i)
    {
        p = pos + dir * dist.y;
        dist.x = scene(p);
        dist.y += dist.x;
        if (dist.x < E || dist.y > 20.)
           break;
        step.x++;
    }
    step.y = dist.x;
    step.w = dist.y;
    return (step);
}

vec3    give_color(float inter)
{
    vec3    ret = vec3(1.,1.,0.3);
    
    // magic
    if (mouth / inter < E)
        ret = vec3(1.,0.,0.);
    if (rigt_eye < 2.*E || left_eye < 2.*E)
        ret = vec3(1.,1.,1.);
    if (rigt_pup < 2.*E || left_pup < 2.*E)
        ret = vec3(0.,0.,0.);
    if (left_arm < 2.*E || left_brm < 2.*E || rigt_arm < 2.*E || rigt_brm < 2.*E 
        || lefb_leg < 2.*E || rigb_leg < 2.*E || lefa_leg < 2.*E || riga_leg < 2.*E)
        ret = vec3(0.2,0.2,0.2);
    if (left_fot < 2.*E || rigt_fot < 2.*E || left_had < 2.*E || rigt_had < 2.*E)
        ret = vec3(1.,1.,1.);
    
    return (ret);
}

vec4    peanut_butter_jelly_time(vec3 pos, vec3 dir) // march letters
{
    vec4    ret = vec4(0.,0.,0.,1.);

    vec2    dist = vec2(0.0);
    vec3    p = vec3(0.0);
    vec4    step = vec4(0.0);

    dir.x += 0.054*cos(2.8+t/5.); // makes text moove
    for (int i = -1; i < 2; ++i) // low iteration give nice blurr
    {
        p = pos + dir * dist.y;
        p.x -=.7;
        dist.x = letters(p);
        dist.y += dist.x;
        if (dist.x < E || dist.y > 17.)
           break;
        step.x++;
    }
    step.y = dist.x;
    step.w = dist.y;
    
    ret.x = 1.-step.y;
    ret.y = 1.-step.y;
    ret.z = 1. -step.w/float(15.);
    return (ret);
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
    vec3    forw  = vec3(0.0, 0.0, 1.0);
    vec3    right = vec3(1.0, 0.0, 0.0);
    vec3    up    = vec3(0.0, 1.0, 0.0);

    return (normalize((u.x-1.) * right + (u.y-0.5) * up + fov * forw));
}

void    init_vars()
{
    rigt_eye = 1e5;
    left_eye = 1e5;
    mouth = 1e5;
    rigt_pup = 1e5;
    left_pup = 1e5;
    riga_leg = 1e5;
    lefa_leg = 1e5;
    rigb_leg = 1e5;
    lefb_leg = 1e5;
    rigt_arm = 1e5;
    rigt_brm = 1e5;
    left_arm = 1e5;
    left_brm = 1e5;
    left_fot = 1e5;
    rigt_fot = 1e5;
    left_had = 1e5;
    rigt_had = 1e5;
    up_cap = 1e5;
    bt_cap = 1e5;
    a_arm_rigt = vec3(1.+.25*sin(2.*t), -.25+.125*sin(3.*t), -.9+.5*sin(2.*t));
    a_arm_left = vec3(1.+.25*sin(2.*t), -.25+.125*sin(3.*t), .9-.5*sin(2.*t));
    b_arm_rigt = vec3(.2,.3+.5+.5*sin(17.*t),-1.5+.5*sin(5.*t));
    b_arm_left = vec3(.2,.3+.5+.5*sin(17.*t),1.5-.5*sin(5.*t));
    a_leg_rigt = vec3(.5+0.25*sin(0.3+4.*t), -.70, -.1275+.25*cos(20.*t));
    a_leg_left = vec3(.5-0.25*sin(0.3+4.*t), -.70, .1275-.25*cos(20.*t));
    b_leg_left = vec3(.4,-2.25,.6+0.35*sin(0.3+40.*t));
    b_leg_rigt = vec3(.4,-2.25,-.6+0.35*sin(0.3+40.*t));
}

//http://iquilezles.org/www/articles/smin/smin.htm
float smin( float a, float b, float k )
{
    float res = exp( -k*a ) + exp( -k*b );
    return -log( res )/k;
}

/*
* trying to hide this at the end
*/

float   letters(vec3 p) // this was painfull (and ugly)
{
    float   mind = 1e5;

    mind = min(mind, sdCapsule(p, vec3(-9.2, 4., 0.), vec3(-9.2, 3., 0.), .15));
    mind = smin(mind, sdTorus(p-vec3(-8.8, 3.7, 0.), vec2(0.25, 0.15)), 40.3);         // p

    mind = min(mind, sdCapsule(p, vec3(-8., 4., 0.), vec3(-8., 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-8., 3.5, 0.), vec3(-7.2, 3.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-8., 4., 0.), vec3(-7.2, 4., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-8., 3., 0.), vec3(-7.2, 3., 0.), .1)); // E
    
    mind = min(mind, sdCapsule(p, vec3(-6.4, 4., 0.), vec3(-6.8, 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-6.4, 4., 0.), vec3(-6., 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-6.3, 3.3, 0.), vec3(-6.5, 3.3, 0.), .1)); // A
    
    mind = min(mind, sdCapsule(p, vec3(-5.6, 4., 0.), vec3(-5.6, 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-5.6, 4., 0.), vec3(-5., 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-5., 4., 0.), vec3(-5., 3., 0.), .1)); // N
    
    mind = min(mind, sdCapsule(p, vec3(-4.4, 4., 0.), vec3(-4.4, 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-4.4, 3., 0.), vec3(-3.8, 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-3.8, 4., 0.), vec3(-3.8, 3., 0.), .1)); // U
    
    mind = min(mind, sdCapsule(p, vec3(-3., 4., 0.), vec3(-3., 3., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-3.4, 4., 0.), vec3(-2.6, 4., 0.), .1)); // T

    mind = min(mind, sdCapsule(p, vec3(-9.4, 1., 0.), vec3(-9.4, 2., 0.), .1));
    mind = min(mind, sdTorus(p - vec3(-9.2, 1.8, 0.), vec2(0.25, 0.051)));
    mind = min(mind, sdTorus(p - vec3(-9.2, 1.2, 0.), vec2(0.25, 0.061)));  // B
    
    mind = min(mind, sdCapsule(p, vec3(-8.4, 2., 0.), vec3(-8.4, 1., 0.), .05));
    mind = min(mind, sdCapsule(p, vec3(-8.4, 1., 0.), vec3(-7.8, 1., 0.), .05));
    mind = min(mind, sdCapsule(p, vec3(-7.8, 2., 0.), vec3(-7.8, 1., 0.), .05)); // U
    
    mind = min(mind, sdCapsule(p, vec3(-7., 2., 0.), vec3(-7., 1., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-7.4, 2., 0.), vec3(-6.6, 2., 0.), .1)); // T
    
    mind = min(mind, sdCapsule(p, vec3(-6., 2., 0.), vec3(-6., 1., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-6.4, 2., 0.), vec3(-5.6, 2., 0.), .1)); // T
    
    mind = min(mind, sdCapsule(p, vec3(-5., 2., 0.), vec3(-5., 1., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-5., 1.5, 0.), vec3(-4.2, 1.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-5., 2., 0.), vec3(-4.2, 2., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-5., 1., 0.), vec3(-4.2, 1., 0.), .1)); // E
    
    mind = min(mind, sdCapsule(p, vec3(-3.8, 1., 0.), vec3(-3.8, 2., 0.), .1));
    mind = min(mind, sdTorus(p - vec3(-3.5, 1.8, 0.), vec2(0.3, 0.1)));
    mind = min(mind, sdCapsule(p, vec3(-3.8, 1.6, 0.), vec3(-3.2, 1., 0.), .1)); // R

    mind = min(mind, sdCapsule(p, vec3(-7.8, 0.5, 0.), vec3(-7.2, 0.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-7.5, 0.5, 0.), vec3(-7.5, -0.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-7.9, -0.5, 0.), vec3(-7.5, -0.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-7.9, -0.5, 0.), vec3(-7.9, -0.3, 0.), .1)); // J
    
    mind = min(mind, sdCapsule(p, vec3(-7., -0.5, 0.), vec3(-7., 0.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-7., 0.5, 0.), vec3(-6.2, 0.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-7., -0.5, 0.), vec3(-6.2, -0.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-7., 0., 0.), vec3(-6.2, 0., 0.), .1)); // E
    
    mind = min(mind, sdCapsule(p, vec3(-5.8, 0.5, 0.), vec3(-5.8, -.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-5.8, -0.5, 0.), vec3(-5.4, -.5, 0.), .1)); // L

    mind = min(mind, sdCapsule(p, vec3(-5., 0.5, 0.), vec3(-5., -.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-5., -0.5, 0.), vec3(-4.6, -.5, 0.), .1)); // L
    
    mind = min(mind, sdCapsule(p, vec3(-3.4, 0.5, 0.), vec3(-3.8, -.0, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-4.2, 0.5, 0.), vec3(-3.8, -.0, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(-3.8, 0.0, 0.), vec3(-3.8, -.5, 0.), .1)); // Y
    
    mind = min(mind, sdCapsule(p, vec3(2., 3.5, 0.), vec3(2., 2.0, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(1.3, 3.5, 0.), vec3(2.6, 3.5, 0.), .1)); // T
    
    mind = min(mind, sdCapsule(p, vec3(2.6, 2.5, 0.), vec3(2.6, 2.0, 0.), .1));
    mind = min(mind, length(p-vec3(2.6, 2.9, 0.)) - .1); // I
    
    mind = min(mind, sdCapsule(p, vec3(3., 3.1, 0.), vec3(3., 2.3, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(3., 3.1, 0.), vec3(3.4, 2.7, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(3.8, 3.1, 0.), vec3(3.4, 2.7, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(3.8, 3.1, 0.), vec3(3.8, 2.3, 0.), .1)); // M

    mind = min(mind, sdCapsule(p, vec3(4.2, 2., 0.), vec3(4.2, 3.5, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(4.2, 2., 0.), vec3(5.2, 2., 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(4.2, 2.7, 0.), vec3(5.2, 2.7, 0.), .1));
    mind = min(mind, sdCapsule(p, vec3(4.2, 3.5, 0.), vec3(5.2, 3.5, 0.), .1)); // E

    return (mind);
}
