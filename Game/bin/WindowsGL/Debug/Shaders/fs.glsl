// Author: Alex DS
#version 330 core
#define MAX_POINT_LIGHTS 2
#define MAX_DIRECTION_LIGHTS 2
#define MAX_LIGHTS 55

in vec3 fragPos;
in vec2 textCoord;
in vec3 normal;
in vec3 viewDirection;
in vec3 lightDirection;

uniform vec3 lightPosition;
uniform vec3 eyePosition;

uniform sampler2D texture;

out vec4 Color;

float fSpecularPower = 1;
vec3 fvAmbient = 2.0*normalize(vec3(94,94,94)) -1.0;
vec3 fvDiffuse = 2.0*normalize(vec3(226,226,226)) -1.0;
vec3 fvSpecular = 2.0*normalize(vec3(125,125,125)) -1.0;

// lightposition = vec3
// viewdirection = eyeposition - objectposition
// lightdirection = lightposition - objectposition

//vec3 lightAmbient = vec3(51,51,51);
//vec3 lightDiffuse = vec3(125,125,125);
//vec3 lightSpecular = vec3(185,185,185);

float pointLight_constant = 1;
float pointLight_linear = 0.32f;
float pointLight_quadratic = 0.032f;
float intensity = 20;

void main()
{	
   vec3  fvLightDirection = normalize( lightDirection );
   vec3  fvNormal         = normalize( normal );
   float fNDotL           = dot( fvNormal, fvLightDirection ); 
   
   vec3  fvReflection     = normalize( ( ( 2.0 * fvNormal ) * fNDotL ) - fvLightDirection ); 
   vec3  fvViewDirection  = normalize( viewDirection );
   float fRDotV           = max( 0.0, dot( fvReflection, fvViewDirection ) );
   
   vec3  fvBaseColor      = texture2D( texture, textCoord ).xyz;
   
   vec3  fvTotalAmbient   = fvAmbient * fvBaseColor; 
   vec3  fvTotalDiffuse   = fvDiffuse * fNDotL * fvBaseColor; 
   vec3  fvTotalSpecular  = fvSpecular * ( pow( fRDotV, fSpecularPower ) );
  
  // Point Light Calculations
  float distance = length(lightPosition - fragPos);
  float attentuation = (1f / (pointLight_constant + pointLight_linear + distance + pointLight_quadratic * (distance * distance))) * intensity;
  fvTotalAmbient *= attentuation;
  fvTotalDiffuse *= attentuation;
  fvTotalSpecular *= attentuation;

   Color = vec4( (fvTotalAmbient + fvTotalDiffuse + fvTotalSpecular ),1);
}

/* DEPRECATED 
Source: https://learnopengl.com/book/offline%20learnopengl.pdf

struct DirectionLight{
	vec3 direction;
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};
uniform DirectionLight dirLights[MAX_DIRECTION_LIGHTS];

struct PointLight{
	vec3 position;
	
	float constant;
	float linear;
	float quadratic;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};
uniform PointLight pointLights[MAX_POINT_LIGHTS];

// Prototypes 
vec3 CalculateDirectionLight(DirectionLight light, vec3 normal, vec3 viewDir);
vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalculateAllLights();

vec3 CalculateAllLights(){
	vec3 finalColor = vec3(0,0,0);

	for(int i = 0; i < MAX_DIRECTION_LIGHTS; i++){ // for each direction light
		finalColor += CalculateDirectionLight(dirLights[i], normal, ViewDirection);
	}

	for(int i = 0; i < MAX_POINT_LIGHTS; i++){ // for each point light
		finalColor += CalculatePointLight(pointLights[i], normal, fragPos, ViewDirection);
	}

	return finalColor;
}

vec3 CalculateDirectionLight(DirectionLight light, vec3 normal, vec3 viewDir){
	vec3 lightDir = normalize(-light.direction);
	// Diffuse shading
	float diff = max(dot(normal, lightDir), 0.0);
	// Specular shading
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 1);
	// Combine results
	vec3 ambient = lightAmbient
	vec3 diffuse = lightDiffuse*diff;
	vec3 specular = lightSpecular*spec;
	return (ambient + diffuse + specular);
}

vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
	vec3 lightDir = normalize(light.position - fragPos);
	// Diffuse shading
	float diff = max(dot(normal, lightDir), 0.0);
	// Specular shading
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 1);
	// Attenuation
	float distance    = length(light.position - fragPos);
	float attenuation = 1.0f / (light.constant + light.linear*distance +light.quadratic*(distance*distance));
	// Combine results
	vec3 ambient = lightAmbient;
	vec3 diffuse = lightDiffuse*diff;
	vec3 specular = lightSpecular*spec;
	ambient*= attenuation;diffuse*= attenuation;specular*= attenuation;
	return (ambient + diffuse + specular);
}
*/