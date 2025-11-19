shader_type canvas_item;

uniform float colorR : hint_range(0.0, 1.0, 0.1);
uniform float colorG : hint_range(0.0, 1.0, 0.1);
uniform float colorB : hint_range(0.0, 1.0, 0.1);

uniform float colorA : hint_range(0.0, 1.0, 0.1);

uniform float frequency;
uniform float speed;
uniform float amplitude;

uniform sampler2D screenTexture : hint_screen_texture;




void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
    
    float waveX = sin(UV.x * frequency + TIME * speed) * amplitude;
    float waveY = cos(UV.y* frequency + TIME * speed) * amplitude;
	// Called for every pixel the material is visible on.
    // COLOR = texture(screenTexture, sin(TIME * UV.x) * SCREEN_UV) + vec4(colorR, colorG, colorB, colorA) ;
    
    vec2 waveUV = UV + vec2(waveX, waveY);
    
    COLOR = texture(TEXTURE, waveUV) * vec4(colorR, colorG, colorB, colorA);
}   

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
