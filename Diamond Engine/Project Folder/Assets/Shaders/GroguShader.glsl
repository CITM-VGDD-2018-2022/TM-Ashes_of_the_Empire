#ifdef vertex
#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normals;
layout (location = 6) in vec3 colors;

out vec3 influenceColor;
out vec3 Normal;
out vec3 fPosition;
out vec3 vertexColor;
out vec3 diffuseColor;

uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

uniform float time;

uniform vec3 lightPosition;
vec3 lightColor;

void main()
{
	lightColor = vec3(0.225, 0.150, 0.120);
	
	
	mat4 viewModel = view * model_matrix;
	gl_Position = projection * viewModel * vec4(position, 1.0);
	
	//influenceColor = vec3(weights.x, weights.y, weights.z);
	//ourColor = vec3(boneIDs.x / 30, boneIDs.y / 30, boneIDs.z / 30);
	vertexColor = colors;
	
	lightColor = vec3(0.225, 0.150, 0.120);
	vec3 lightDirection = vec3(lightPosition - normalize(position));
	diffuseColor = vec3(max(dot(lightDirection, -normals), 0.0) * lightColor);
    
}
#endif

#ifdef fragment
#version 330 core

in vec3 influenceColor;
in vec3 vertexColor; 
in vec3 diffuseColor; 

out vec4 color;

void main()
{
 	//color = vec4(influenceColor, 1.0);
 	color = vec4(vertexColor + diffuseColor, 1.0);
}
#endif

f);
