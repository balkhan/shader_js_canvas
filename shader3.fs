precision mediump float;
varying vec4 vColor;
varying float   timer;
varying vec2    uv;
float   t;

/*
* License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
*/

#define PI		3.14
#define I_MAX	50
#define E_BULB	0.001
//#define BULB // comment this line to see something else

float	bulb(vec3 pos);
float	de(vec3 pos);
mat4	lookat(vec3 eye, vec3 target, vec3 up);
vec2 	cmult(vec2 a, vec2 b);
vec4	march(vec3 pos, vec3 dir);

/*
* Taken from an iq's shader
*/

vec3 calcNormal( in vec3 pos, float e, vec3 dir)
{
    vec3 eps = vec3(e,0.0,0.0);

	return normalize(vec3(
           march(pos+eps.xyy, dir).w - march(pos-eps.xyy, dir).w,
           march(pos+eps.yxy, dir).w - march(pos-eps.yxy, dir).w,
           march(pos+eps.yyx, dir).w - march(pos-eps.yyx, dir).w ));
}

vec3	camera(vec2 u)
{
    float   fov = 1.;
	vec3    forw  = vec3(0.0, 0.0, -1.0);
	vec3    right = vec3(1.0, 0.0, 0.0);
	vec3    up    = vec3(0.0, 1.0, 0.0);

    return (normalize((u.x-0.5) * right + (u.y-0.5) * up + fov * forw));
}

void main(void)
{
	t = timer/20000.;
    vec4	col = vec4(0.0, 0.0, 0.0, 1.0);
    vec3	pos = vec3(0.0, 0.0, 0.0);
    vec3	light = vec3(-30.0, 30.0, -20.0);
	vec2	u = uv;
	u.xy += 1.;
    vec3	dir = camera(u);

    vec4	inter = (march(pos, dir));
    // phong shading
    if (inter.x < float(I_MAX))
    {
	    vec3	n = calcNormal(pos, E_BULB, dir);
        vec3 vd = normalize(light - (pos+inter.w*dir));
        float vdn = dot(vd, n);
	    col.xyz = vec3(1.0, 0.50, .25) * vd + dot(dir, n) * vdn;
	    col.xyz += vec3(dot(-dir, 2.0 * vdn * n - vd));
    }
   	gl_FragColor = col;
}

vec4	march(vec3 pos, vec3 dir)
{
    vec2	dist = vec2(0.0);
    vec3	p = vec3(0.0);
    vec4	step = vec4(0.0);

    for (int i = -1; i < I_MAX; ++i)
    {
    	p = pos + dir * dist.y;
        dist.x = de(p);
        dist.y += dist.x;
        if (dist.x < E_BULB)
           break;
        step.x++;
    }
    step.w = dist.y;
    return (step);
}

/*
*	Mandelbulb DE taken from Syntopia
*/

float	de(vec3 pos)
{
#ifdef	BULB
    pos.x = 2.0*sin(pos.x+t/2.0);
    pos.z = 2.0*sin(pos.z+t/2.0);
    pos.y = sin(pos.y+t/2.0)+cos(pos.y+t/2.0);

    vec3 z = pos;
    float theta;
    float phi;
    float zr;
	float dr = 1.0;
	float r = 0.0;
	for (int i = 0; i < 10 ; i++)
    {
		r = length(z);
		if (r > 2.0)
            break;
		theta = acos(z.z/r);
		phi = atan(z.y, z.x);
        zr = r * r;
        zr *= zr;
        zr *= zr;
		dr =  zr * 8.0 * dr + .50;
		zr *= r;
		theta = theta * 8.0;
		phi = phi * 8.0 + 30.0 * cos(t/3.0);
		z = zr*vec3(sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta));
		z += pos;
	}
	return (0.5*log(r)*r/dr);
#endif
#ifndef	BULB
    pos.x = cos(pos.x-pos.y+t/4.0)*cos(pos.x+pos.y-t/4.0);
    pos.z = cos(pos.z+t/4.0)*cos(pos.z-t/4.0);
    pos.y = cos(pos.y-pos.x+t/4.0)*cos(pos.y+pos.x-t/4.0);
    return (length(pos)-0.125);
#endif
}
