#version 330 core
out vec4 FragColor;

struct Light {
    vec3 direction;

    vec3 specular;
    vec3 diffuse;
    vec3 ambient;

    float constant;
    float linear;
    float quadratic;
};

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_specular1;

    float shininess;
};
in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;

uniform Light sun;
uniform Material material;

uniform vec3 viewPosition;
// calculates the color when using a point light.
vec3 CalcSunLight(Light sun, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(sun.direction - FragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(sun.direction - fragPos);
    float attenuation = 1.0 / (sun.constant + sun.linear * distance + sun.quadratic * (distance * distance));
    // combine results
    vec3 ambient = sun.ambient * vec3(texture(material.texture_diffuse1, TexCoords));
    vec3 diffuse = sun.diffuse * diff * vec3(texture(material.texture_diffuse1, TexCoords));
    vec3 specular = sun.specular * spec * vec3(texture(material.texture_specular1, TexCoords).xxx);
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

void main()
{
    vec3 normal = normalize(Normal);
    vec3 viewDir = normalize(viewPosition - FragPos);
    vec3 result = CalcSunLight(sun, normal, FragPos, viewDir);
    FragColor = vec4(result, 1.0);
}