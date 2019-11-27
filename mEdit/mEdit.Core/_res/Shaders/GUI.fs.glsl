#version 130

in vec2 UV;
uniform sampler2D texture;
uniform vec4 color;

void main()
{    
    gl_FragColor = color;
} 