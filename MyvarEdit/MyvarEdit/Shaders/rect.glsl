
#ifdef VERTEX

in vec2 position;
uniform mat4 mvp; 


void main(void)
{
    gl_Position = mvp * vec4(vec3(position, 1.0), 1.0);
}

#endif
 
#ifdef FRAGMENT

uniform vec4 uColor;

out vec4 color;

void main(void)
{    
      color = uColor;
}

#endif