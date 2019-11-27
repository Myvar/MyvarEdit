#version 130


attribute vec3 position;
attribute vec2 textCoord;
uniform mat4 Model;
uniform mat4 Proj;

out vec2 UV;

void main()
{
    UV = textCoord;
    gl_Position = Proj * Model * vec4(position, 1.0);
}