
#ifdef VERTEX

in vec2 position;

uniform vec2 uViewPort; //Width and Height of the viewport
uniform mat4 mvp; 
out vec2 vLineCenter;

void main(void)
{
  vec4 pp = mvp * vec4(vec3(position, 1.0), 1.0);
  gl_Position = pp;
  vec2 vp = uViewPort;
  vLineCenter = 0.5*(pp.xy + vec2(1, 1))*vp;
}

#endif
 
#ifdef FRAGMENT

uniform float uLineWidth;
uniform vec4 uColor;
uniform float uBlendFactor; //1.5..2.5
in vec2 vLineCenter;
out vec4 color;
void main(void)
{
      vec4 col = uColor;        
      double d = length(vLineCenter - gl_FragCoord.xy);
      double w = uLineWidth;
      if (d>w)
        col.w = 0;
      else
        col.w *= pow(float((w-d)/w), uBlendFactor);
      color = col;
}

#endif