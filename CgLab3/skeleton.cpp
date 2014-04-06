#include <iostream>
#include <cmath>
#include <glm/glm.hpp>
#include <SDL.h>
#include "SDLauxiliary.h"
#include "TestModel.h"

using namespace std;
using glm::vec3;
using glm::ivec2;
using glm::vec2;
using glm::mat3;

// ----------------------------------------------------------------------------
// Internal Data Structures

struct Pixel {
	ivec2 location;
	float zinv;
	vec3 pos3d;
};

struct Vertex {
	vec3 position;
};

// ----------------------------------------------------------------------------
// GLOBAL VARIABLES

const int SCREEN_WIDTH = 500;
const int SCREEN_HEIGHT = SCREEN_WIDTH;
SDL_Surface* screen;
int t;
vector<Triangle> triangles;

mat3 R = glm::mat3(1.0f);
float yaw = 0;

const float FOCAL_RATIO = 3.0f / 2.0f;
const float focalLength = SCREEN_HEIGHT * FOCAL_RATIO;
vec3 cameraPos(0, 0, -((2 * focalLength / SCREEN_HEIGHT) + 1));

// Constant for camera view change.
const float delta = 0.01f;

// The current color of the object or surface.
//vec3 currentColor;

// The buffer of that contains the depth information of each pixel.
float depthBuffer[SCREEN_HEIGHT+1][SCREEN_WIDTH+1];

// Lighting variables
vec3 lightPos(0, -0.5, -0.7);
vec3 lightPower = 1.1f*vec3(1, 1, 1);
vec3 indirectLightPowerPerArea = 0.5f*vec3(1, 1, 1);

// Current color 
vec3 currentNormal;
vec3 currentReflectance;

// ----------------------------------------------------------------------------
// FUNCTIONS

void Update();
void Draw();

/*
Converts a point in 3D world space to the projected 2 dimension space 
relative to the current camera position.

Arguments:
vertex: 3D Position in world/real space
projectedPos: New project position.
*/
void VertexShader(Vertex vertex, Pixel& projectedPos);

/*
Per Pixel shading function that handles placing the pixel on the screen.

Arguments:
p : Pixel to draw.
*/
void PixelShader(const Pixel& p);

/*
Interpolates the 2 dimensional points returning the points in between a and b
*/
void Interpolate(ivec2 a, ivec2 b, vector<ivec2>& result);

/*
Interpolates the value of a pixel includings is inverted depth.

Arguments
a : Pixel to interpolate from.
b : Pixel to interpolate to.
result : the resulting pixels between a and b.
*/
void Interpolate(Pixel a, Pixel b, vector<Pixel>& result);

/*
Draws a line inbetween two points.
*/
void DrawLineSDL(SDL_Surface* surface, Pixel a, Pixel b, vec3 color);

// Draws line between vertices inputted
void DrawPolygonEdges(const vector<Vertex>& vertices);

/*
	Extract the left and right boundaries of all the rows within the polygon.
*/
void ComputePolygonRows(const vector<Pixel>& vertexPixels, vector<Pixel>& leftPixels, vector<Pixel>& rightPixels);

/*
Draws all the rows
*/
void DrawRows(const vector<Pixel>& leftPixels, const vector<Pixel>& rightPixels);

/*
Draws the polygon
*/
void DrawPolygon(const vector<Vertex>& vertices);

int main(int argc, char* argv[])
{
	//vector<Pixel> vertexPixels(3);
	//vertexPixels[0].location = ivec2(10, 5);
	//vertexPixels[1].location = ivec2(5, 10);
	//vertexPixels[2].location = ivec2(15, 15);
	//vector<Pixel> leftPixels;
	//vector<Pixel> rightPixels;
	//ComputePolygonRows(vertexPixels, leftPixels, rightPixels);
	//for (size_t row = 0; row < leftPixels.size(); ++row) {
	//	cout << "Start: ("
	//		<< leftPixels[row].location.x << ","
	//		<< leftPixels[row].location.y << "). "
	//		<< "End: ("
	//		<< rightPixels[row].location.x << ","
	//		<< rightPixels[row].location.y << "). " << endl;
	//}
	//return 0;

	LoadTestModel(triangles);
	screen = InitializeSDL(SCREEN_WIDTH, SCREEN_HEIGHT);
	t = SDL_GetTicks();	// Set start value for timer.

	while (NoQuitMessageSDL())
	{
		Update();
		Draw();
	}

	SDL_SaveBMP(screen, "screenshot.bmp");
	return 0;
}

void Update()
{
	// Compute frame time:
	int t2 = SDL_GetTicks();
	float dt = float(t2 - t);
	t = t2;
	cout << "Render time: " << dt << " ms." << endl;

	Uint8* keystate = SDL_GetKeyState(0);

	if (keystate[SDLK_UP])
		cameraPos.z += delta * 2;

	if (keystate[SDLK_DOWN])
		cameraPos.z -= delta * 2;

	if (keystate[SDLK_RIGHT])
		yaw -= delta;

	if (keystate[SDLK_LEFT])
		yaw += delta;

	/*if( keystate[SDLK_RSHIFT] )
		;

		if( keystate[SDLK_RCTRL] )
		;

		if( keystate[SDLK_w] )
		;

		if( keystate[SDLK_s] )
		;

		if( keystate[SDLK_d] )
		;

		if( keystate[SDLK_a] )
		;

		if( keystate[SDLK_e] )
		;

		if( keystate[SDLK_q] )
		;*/

	R[0][0] = glm::cos(yaw);
	R[2][0] = glm::sin(yaw);
	R[0][2] = -glm::sin(yaw);
	R[2][2] = glm::cos(yaw);
}

//void setValues(vector<Vertex>& vertices, const Triangle& triangle) {
//	for (size_t i = 0; i < vertices.size(); ++i) {
//		vertices[i].normal = triangle.normal;
//		vertices[i].reflectance = triangle.color;
//	}
//}

void Draw()
{
	SDL_FillRect(screen, 0, 0);

	if (SDL_MUSTLOCK(screen))
		SDL_LockSurface(screen);

	// Clear the depth buffer.
	for (int y = 0; y < SCREEN_HEIGHT + 1; ++y) {
		for (int x = 0; x < SCREEN_WIDTH + 1; ++x) {
			depthBuffer[y][x] = 0;
		}
	}

	for (size_t i = 0; i < triangles.size(); ++i)
	{
		//currentColor = triangles[i].color;
		vector<Vertex> vertices(3);
		vertices[0].position = triangles[i].v0;
		vertices[1].position = triangles[i].v1;
		vertices[2].position = triangles[i].v2;

		// Set the normal
		//setValues(vertices, triangles[i]);
		currentNormal = triangles[i].normal;
		currentReflectance = triangles[i].color;
		DrawPolygon(vertices);
	}

	if (SDL_MUSTLOCK(screen))
		SDL_UnlockSurface(screen);

	SDL_UpdateRect(screen, 0, 0, 0, 0);
}

void VertexShader(Vertex vertex, Pixel& projectedPos) {
	vec3 projected3D = R * (vertex.position - cameraPos);
	projectedPos.location.x = (int)(focalLength * projected3D.x / projected3D.z + SCREEN_WIDTH / 2);
	projectedPos.location.y = (int)(focalLength * projected3D.y / projected3D.z + SCREEN_HEIGHT / 2);
	projectedPos.zinv = 1.0f / projected3D.z; // Set the inverted 

	projectedPos.pos3d = vertex.position;

	/*vec3 r = glm::normalize(lightPos - vertex.position);
	float distance = glm::distance(lightPos, vertex.position);
	vec3 B = lightPower / (4 * 3.1416f * distance*distance);
	float dotProduct = glm::dot(vertex.normal, r);
	if (dotProduct < 0)
		dotProduct = 0;
	vec3 D = B * dotProduct;
	projectedPos.illumination = vertex.reflectance * (D + indirectLightPowerPerArea);*/
}

void PixelShader(const Pixel& p) {
	int x = p.location.x;
	int y = p.location.y;
	if (p.zinv > depthBuffer[y][x]) {
		// If the current pixel is closer to the COP or smaller depth.
		// Then update the buffer and draw the camera.
		depthBuffer[y][x] = p.zinv;

		vec3 r = glm::normalize(lightPos - p.pos3d);
		float distance = glm::distance(lightPos, p.pos3d);
		float dotProduct = std::fmax(glm::dot(r, currentNormal), 0);
		vec3 D = (lightPower * dotProduct) / (4 * 3.1416f * distance * distance);

		PutPixelSDL(screen, x, y, currentReflectance * (D + indirectLightPowerPerArea));
	}
}

void Interpolate(ivec2 a, ivec2 b, vector<ivec2>& result) {
	int N = result.size();
	vec2 step = vec2(b - a) / float(glm::max(N - 1, 1));
	vec2 current(a);
	for (int i = 0; i < N; ++i) {
		result[i] = glm::round(current);
		current += step;
	}
}

void Interpolate(vec3 a, vec3 b, vector<vec3>& result) {
	int N = result.size();
	vec3 step = vec3(b - a) / float(glm::max(N - 1, 1));
	vec3 current(a);
	for (int i = 0; i < N; ++i) {
		result[i] = current;
		current += step;
	}
}

void Interpolate(Pixel a, Pixel b, vector<Pixel>& result) {
	int numPixels = result.size();
	vector<ivec2> locations(numPixels);
	vector<vec3> positions3D(numPixels);

	// Interpolate all the internal locations.
	Interpolate(a.location, b.location, locations);
	Interpolate(a.pos3d * a.zinv, b.pos3d * b.zinv, positions3D);

	float numSteps = float(glm::max(numPixels - 1, 1));
	float zStep = (b.zinv - a.zinv) / numSteps;
	//vec3 illumStep = (b.illumination - a.illumination) / numSteps;
	float zInvCurrent = a.zinv;
	//vec3 illumCurrent = a.illumination;
	for (int i = 0; i < numPixels; ++i) {
		// Update the inverted depth.
		result[i].zinv = zInvCurrent;
		zInvCurrent += zStep;

		// Update the illumination.
		/*result[i].illumination = illumCurrent;
		illumCurrent += illumStep;*/
		// Update the 3d position of this.
		result[i].pos3d = positions3D[i] / result[i].zinv;

		// Update the location.
		result[i].location = locations[i];
	}
}

void DrawLineSDL(SDL_Surface* surface, Pixel a, Pixel b, vec3 color) {
	ivec2 delta = glm::abs(a.location - b.location);
	int pixels = glm::max(delta.x, delta.y) + 1;
	vector<Pixel> line(pixels);
	Interpolate(a, b, line);

	for (int i = 0; i < pixels; ++i) {
		PixelShader(line[i]);
	}
}

void DrawPolygonEdges(const vector<Vertex>& vertices) {

	int numVertices = vertices.size();
	// Transform each vertex from 3D world position to 2D image position.

	vector<Pixel> projectedVertices(numVertices);
	for (int i = 0; i < numVertices; ++i) {
		VertexShader(vertices[i], projectedVertices[i]);
	}

	// Loop through all the vertices and draw the edgees from it to the next vertex
	for (int i = 0; i < numVertices; ++i) {
		int j = (i + 1) % numVertices;
		vec3 color(1, 1, 1);
		DrawLineSDL(screen, projectedVertices[i], projectedVertices[j], color);
	}
}

void ComputePolygonRows(const vector<Pixel>& vertexPixels, vector<Pixel>& leftPixels, vector<Pixel>& rightPixels)
{
	// 1. Find max and min y-value of the polygon
	// and compute the number of rows it occupies.
	int min = numeric_limits<int>::max();
	int max = numeric_limits<int>::min();
	for (size_t i = 0; i < vertexPixels.size(); ++i) {
		min = glm::min(vertexPixels[i].location.y, min);
		max = glm::max(vertexPixels[i].location.y, max);
	}
	size_t rows = max - min + 1;

	// 2. Resize leftPixels and rightPixels
	// so that they have an element for each row.
	leftPixels.resize(rows);
	rightPixels.resize(rows);

	// 3. Initialize the x-coordinates in leftPixels
	// to some really large value and the x-coordinates
	// in rightPixels to some really small value.
	for (size_t i = 0; i < rows; ++i) {
		leftPixels[i].location.x = numeric_limits<int>::max();
		rightPixels[i].location.x = numeric_limits<int>::min();
		leftPixels[i].location.y = min + i;
		rightPixels[i].location.y = min + i;
	}

	// 4. Loop through all edges of the polygon and use
	// linear interpolation to find the x-coordinate for
	// each row it occupies. Update the corresponding
	// values in rightPixels and leftPixels.
	for (size_t i = 0; i < vertexPixels.size(); ++i) {
		// Reference the endpoint of the polygon side.
		int j = (i + 1) % vertexPixels.size();
		Pixel start = vertexPixels[i];
		Pixel end = vertexPixels[j];

		// Find the edge between the two points on the line 
		ivec2 delta = glm::abs(start.location - end.location);
		int pixels = glm::max(delta.x, delta.y) + 1;
		vector<Pixel> line(pixels);
		Interpolate(start, end, line);

		// For each point in the line update the corresponding row.
		for (size_t k = 0; k < line.size(); ++k) {
			Pixel edgePoint = line[k];
			int rowOffSet = edgePoint.location.y - min;
			if (rowOffSet < 0 || rowOffSet > max) {
				cout << "The row index is outside the bounds of the polygon: " << edgePoint.location.y << endl;
			}

			// Check if we have a new left bound.
			if (edgePoint.location.x < leftPixels[rowOffSet].location.x) {
				// Update the current left bound.
				leftPixels[rowOffSet].location.x = edgePoint.location.x;
				// Make sure we set the inverted depth.
				leftPixels[rowOffSet].zinv = edgePoint.zinv;
				leftPixels[rowOffSet].pos3d = edgePoint.pos3d;
			}

			// Check if we have a new right bound.
			if (edgePoint.location.x > rightPixels[rowOffSet].location.x) {
				// Update the current right bound.
				rightPixels[rowOffSet].location.x = edgePoint.location.x;
				rightPixels[rowOffSet].zinv = edgePoint.zinv;
				rightPixels[rowOffSet].pos3d = edgePoint.pos3d;
			}
		}
	}
}

void DrawRows(const vector<Pixel>& leftPixels, const vector<Pixel>& rightPixels) {
	for (size_t i = 0; i < leftPixels.size(); ++i) {
		DrawLineSDL(screen, leftPixels[i], rightPixels[i], currentReflectance);
	}
}

void DrawPolygon(const vector<Vertex>& vertices) {
	size_t V = vertices.size();
	vector<Pixel> vertexPixels(V);
	for (size_t i = 0; i < V; ++i)
		VertexShader(vertices[i], vertexPixels[i]);
	vector<Pixel> leftPixels;
	vector<Pixel> rightPixels;
	ComputePolygonRows(vertexPixels, leftPixels, rightPixels);
	DrawRows(leftPixels, rightPixels);
}