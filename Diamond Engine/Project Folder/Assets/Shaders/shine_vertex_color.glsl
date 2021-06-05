#ifdef vertex
#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;
layout (location = 2) in vec3 normals;
layout (location = 3) in vec3 tangents;
layout (location = 6) in vec3 colors;

out VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec4 FragPosLightSpace[2];
    
    mat3 TBN;
    vec3 TangentLightPos[2];
    vec3 TangentViewPos;
    vec3 TangentFragPos;
    
    vec3 vertexColor;
} vs_out;



uniform mat4 model_matrix;
uniform mat4 view;
uniform mat4 projection;

struct LightInfo 
{
	vec3 lightPosition;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	bool calculateShadows;
	bool activeLight;

};

uniform LightInfo lightInfo[2];

uniform vec3 cameraPosition;

void main()
{
    vs_out.vertexColor = colors;
    
    vs_out.FragPos = vec3(model_matrix * vec4(position, 1.0));
    vs_out.TexCoords = texCoord;
    vs_out.FragPosLightSpace[0] = lightInfo[0].lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);
    vs_out.FragPosLightSpace[1] = lightInfo[1].lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);

	
	//Calculate tangent vars
	vec3 T = normalize(vec3(model_matrix * vec4(tangents, 0.0)));
    vec3 N = normalize(vec3(model_matrix * vec4(normals,  0.0)));
    vec3 B = cross(N, T);
	
	vs_out.TBN = transpose(mat3(T, B, N));
    vs_out.TangentLightPos[0] = vs_out.TBN * lightInfo[0].lightPosition;
    vs_out.TangentLightPos[1] = vs_out.TBN * lightInfo[1].lightPosition;
    vs_out.TangentViewPos  = vs_out.TBN * cameraPosition;
    vs_out.TangentFragPos  = vs_out.TBN * vec3(model_matrix * vec4(position, 1.0));

    gl_Position = projection * view * model_matrix * vec4(position, 1.0);

}
#endif

#ifdef fragment
#version 330 core

in VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec4 FragPosLightSpace[2];
    
    mat3 TBN;
    vec3 TangentLightPos[2];
    vec3 TangentViewPos;
    vec3 TangentFragPos;
    
    vec3 vertexColor;
} fs_in;

in vec3 vertexColor;  

struct LightInfo 
{
	vec3 lightPosition;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	bool calculateShadows;
	bool activeLight;
};

struct AreaLightInfo 
{
	vec3 lightPosition;
	vec3 lightForward;
	mat4 lightSpaceMatrix;
	
	vec3 lightColor;
	vec3 ambientLightColor;
	float lightIntensity;
	float specularValue;
	
	float lFadeDistance;
	float lMaxDistance;
	
	bool activeLight;
	bool calculateShadows;
};


uniform LightInfo lightInfo[2];
uniform AreaLightInfo areaLightInfo[5];

uniform float farPlaneDistance;

uniform sampler2D shadowMap;
uniform sampler2D normalMap;
uniform sampler2D specularMap;
uniform float bumpDepth;

uniform vec3 cameraPosition;
uniform vec3 altColor;

out vec4 color;


//global vars used to calculate area light
vec4 upRight = vec4(1.0, 1.0, 0.0, 1.0);
vec4 downRight = vec4(1.0, -1.0, 0.0, 1.0);
vec4 upLeft = vec4(-1.0, 1.0, 0.0, 1.0);
vec4 downLeft = vec4(-1.0, -1.0, 0.0, 1.0);

vec3 rectVertex[4];

uniform samplerCube cubeShadowMap[5];

uniform vec3 shineColor;
uniform float shineColorValue;


float ShadowCalculation(vec4 fragPosLightSpace, vec3 lightDir, vec3 normal, int iterator)
{

	if (lightInfo[iterator].calculateShadows == false)
		return 0.0;
	
// perform perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
   
    if(projCoords.z > 1.0)
    	return 0.0;
    
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // check whether current frag pos is in shadow
	float bias = max(0.0005 * (1.0 - dot(normal, lightDir)), 0.0005);
	float shadow = 0.0;
	
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
    shadow /= 9.0;
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        shadow = 0.0;
        
    return shadow;  
}


float GetShadowValue()
{
	vec3 normal = texture(normalMap, fs_in.TexCoords).rgb;
    normal = normalize(normal * 2.0 - 1.0);
    
	for	(int i = 0; i < 2; i++)
    {
    	if(lightInfo[i].calculateShadows == true)
    	{
    		vec3 lightDir = normalize(fs_in.TangentLightPos[i] - vec3(0, 0, 0));
    		return ShadowCalculation(fs_in.FragPosLightSpace[i], lightDir, normal, i);
    	}
    }

}


vec3 CalculateDirectionalLight(float specMapValue)
{
	vec3 normal = texture(normalMap, fs_in.TexCoords).rgb + bumpDepth;
    normal = normalize(normal * 2.0 - 1.0);
    
	vec3 lighting = vec3(0.0, 0.0, 0.0);
    
    float shadow = GetShadowValue();
    
    
    for (int i = 0; i < 2; i++)
    {
    	if (lightInfo[i].activeLight == true)
    	{
    		vec3 lightDir = normalize(fs_in.TangentLightPos[i] - vec3(0, 0, 0));
    
   	 	// diffusesssaasw
    		float diff = max(dot(lightDir, normal), 0.0);
    		vec3 diffuse = diff * lightInfo[i].lightColor;
   	 	// specular
   	 		vec3 viewDir = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos);
    		float spec = 0.0;
    		vec3 halfwayDir = normalize(lightDir + viewDir);  
    		spec = pow(max(dot(normal, halfwayDir), 0.0), lightInfo[i].specularValue);
    		vec3 specular = spec * lightInfo[i].lightColor * specMapValue;
        	
    	 // calculate shadow
    		lighting += (lightInfo[i].ambientLightColor + (1.0 - shadow) * (diffuse + specular)) * lightInfo[i].lightIntensity;
    	}
    }
    
    return lighting;

}


float SignedDistancePlanePoint(vec3 planePoint, vec3 lForward)
{
	return dot(lForward, (fs_in.FragPos - planePoint));
}


vec3 SetVectorLenght(vec3 vector, float dst)
{
	vec3 normVector = normalize(vector);
	
	return (normVector * dst);
}


vec3 ProjectPointOnPlane(vec3 planePoint, vec3 lForward)
{
	float dst = SignedDistancePlanePoint(planePoint, lForward);
	
	//Reverse the sign
	dst *= -1;
	
	
	vec3 translationVector = SetVectorLenght(lForward, dst);

	
	return vec3(fs_in.FragPos + translationVector);
}

float LengthScuared(vec3 line)
{
	return (line.x * line.x + line.y * line.y + line.z * line.z);
}

bool IsBetween(vec3 value, vec3 b, vec3 c)
{
	float dst = LengthScuared(c - b);
	
	if (LengthScuared(b - value) <= dst && LengthScuared(c - value) <= dst)
		return true;
		
	return false;
}


vec3 NearestPointOnLine(vec3 point, vec3 lineP1, vec3 lineP2)
{
	vec3 lineDir = normalize(lineP2 - lineP1);
	
	vec3 v = point - lineP1;
	float d = dot(v, lineDir);
	
	return vec3(lineP1 + lineDir * d);
}





vec3 NearestPointOnFiniteLine(vec3 point, vec3 lineP1, vec3 lineP2)
{
	float dst = LengthScuared(lineP2 - lineP1);
	
	if (dst == 0.0)
		return lineP1;
		
	float t = max(0, min(1, dot(point - lineP1, lineP2 - lineP1) / dst));
	
	return (lineP1 + t * (lineP2 - lineP1));
}


vec3 CalculateClosestPoint(vec3 projectedPoint)
{
	float closestDistance = 1000000000.0;
	int closestPointIndex;
	
	for (int i = 0; i < 4; ++i)
	{
		float dst = LengthScuared(rectVertex[i] - projectedPoint);
		
		if (dst < closestDistance)
		{
			closestDistance = dst;
			closestPointIndex = i;
		}
	
	}
	
	vec3 closestVertex = rectVertex[closestPointIndex];
	
	vec3 nextVertex;
	vec3 prevVertex;
	
	if (closestPointIndex == 0)
	{
		nextVertex = rectVertex[1];
		prevVertex = rectVertex[3];
	}
	
	else if (closestPointIndex == 3)
	{
		nextVertex = rectVertex[0];
		prevVertex = rectVertex[2];
	}
	
	else
	{
		nextVertex = rectVertex[closestPointIndex + 1];
		prevVertex = rectVertex[closestPointIndex - 1];
	}
	
	vec3 projectedPoint1 = NearestPointOnLine(projectedPoint, nextVertex, closestVertex);
	vec3 projectedPoint2 = NearestPointOnLine(projectedPoint, prevVertex, closestVertex);
	
	if (IsBetween(projectedPoint1, closestVertex, nextVertex) && IsBetween(projectedPoint2, closestVertex, prevVertex))
		return projectedPoint;
		
	else
	{
		vec3 posiblePoint1 = NearestPointOnFiniteLine(projectedPoint, nextVertex, closestVertex);
		vec3 posiblePoint2 = NearestPointOnFiniteLine(projectedPoint, prevVertex, closestVertex);
			
			
		if (LengthScuared(posiblePoint1 - projectedPoint) < LengthScuared(posiblePoint2 - projectedPoint))
			return posiblePoint1;
			
				
		else
			return posiblePoint2;
	}

}


vec3 CalculateAreaLightDirection(int iterator)
{
	rectVertex[0] = vec4(areaLightInfo[iterator].lightSpaceMatrix * upRight).xyz;
	rectVertex[1] = vec4(areaLightInfo[iterator].lightSpaceMatrix * downRight).xyz;
	rectVertex[2] = vec4(areaLightInfo[iterator].lightSpaceMatrix * downLeft).xyz;
	rectVertex[3] = vec4(areaLightInfo[iterator].lightSpaceMatrix * upLeft).xyz;
	
	vec3 projectedPoint = ProjectPointOnPlane(rectVertex[0], areaLightInfo[iterator].lightForward);
	
	vec3 lightPos = CalculateClosestPoint(projectedPoint);

	return (lightPos - fs_in.FragPos);
}


vec3 sampleOffsetDirections[9] = vec3[]
(
   vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1,  1, -1),
   vec3( 1,  1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), 
   vec3(-1,  1,  0), vec3( 1,  0,  1), vec3(-1,  0,  1)
);   


float GetAreaShadowValue(int lightNum, vec3 normal, vec3 lightDir)
{
	if(areaLightInfo[lightNum].calculateShadows == false)
		return 0.0;
		
	vec3 fragToLight = fs_in.FragPos - areaLightInfo[lightNum].lightPosition;
	float currDepth = length(fragToLight);

	float bias = max(0.9 * (1.0 - dot(normal, lightDir)), 0.9);

	float diskRadius = 0.05;
	float shadowValue = 0.0;

	for (int i = 0; i < 9; ++i)
	{
		float closestDepth = texture(cubeShadowMap[lightNum], fragToLight + sampleOffsetDirections[i] * diskRadius).r;
		closestDepth *= farPlaneDistance;
		
		shadowValue += (currDepth - bias > closestDepth  ? 1.0 : 0.0);
	}
	
	return shadowValue / 9.0;

}

vec3 CalculateAreaLight( float specMapValue)
{
	vec3 lighting = vec3(0.0, 0.0, 0.0);
	vec3 normal = texture(normalMap, fs_in.TexCoords).rgb + bumpDepth;
	normal = normal * 2.0 - 1.0;
	normal = normalize(transpose(fs_in.TBN) * normal);
    
    for (int i = 0; i < 5; i++)
    {
		if (areaLightInfo[i].activeLight == true)
		{
    		vec3 lightDir = CalculateAreaLightDirection(i);
    		float fade = 1.0;
    		float dst = LengthScuared(lightDir);
    		
    		if (dst > areaLightInfo[i].lMaxDistance)
    			continue;
    		
    		else if (dst > areaLightInfo[i].lFadeDistance && dst < areaLightInfo[i].lMaxDistance)
    			fade = 1.0 - ((dst - areaLightInfo[i].lFadeDistance) / (areaLightInfo[i].lMaxDistance - areaLightInfo[i].lFadeDistance));
			
			lightDir = normalize(lightDir);
    		
    		if (dot(lightDir, areaLightInfo[i].lightForward) <= 0.0)
    		{
   	 			// diffuse
    			float diff = max(dot(lightDir, normal), 0.0);
    			vec3 diffuse = diff * areaLightInfo[i].lightColor;
    			
   	 			// specular
   				vec3 viewDir = normalize(cameraPosition - fs_in.FragPos);
    			float spec = 0.0;
    			vec3 halfwayDir = normalize(lightDir + viewDir);  
    			spec = pow(max(dot(normal, halfwayDir), 0.0), areaLightInfo[i].specularValue);
    			vec3 specular = spec * areaLightInfo[i].lightColor * specMapValue;
    			
    			float shadow = GetAreaShadowValue(i, normal, lightDir);        	
    	 		// calculate light value
    			lighting += (areaLightInfo[i].ambientLightColor + (1 - shadow) * (diffuse + specular)) * areaLightInfo[i].lightIntensity * fade;
    		}
    	}
    }

	return lighting;
}


void main()
{
	float specMapValue = texture(specularMap, fs_in.TexCoords).r;
    vec3 directionalLight = CalculateDirectionalLight(specMapValue);
    vec3 areaLight = CalculateAreaLight(specMapValue);
    
	vec3 col = (directionalLight + areaLight) * (fs_in.vertexColor * altColor);
	
	color = vec4(mix(col, shineColor, shineColorValue), 1.0);
}
#endif

