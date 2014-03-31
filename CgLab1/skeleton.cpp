// Introduction lab that covers:
// * C++
// * SDL
// * 2D graphics
// * Plotting pixels
// * Video memory
// * Color representation
// * Linear interpolation
// * glm::vec3 and std::vector

#include "SDL.h"
#include <iostream>
#include <glm/glm.hpp>
#include <vector>
#include "SDLauxiliary.h"

using namespace std;
using glm::vec3;

// --------------------------------------------------------
// GLOBAL VARIABLES

const int SCREEN_WIDTH = 640;
const int SCREEN_HEIGHT = 480;
vector<vec3> stars(1000);
SDL_Surface* screen;
int t;
const float VELOCITY = 1;

// --------------------------------------------------------
// FUNCTION DECLARATIONS

void Draw();

void Interpolate(float a, float b, vector<float>& result);

void Interpolate(vec3 a, vec3 b, vector<vec3>& result);

void Update();

// --------------------------------------------------------
// FUNCTION DEFINITIONS

int main( int argc, char* argv[] )
{
	for (int i = 0; i < stars.size(); ++i) {
		stars[i].x = ((float(rand()) / float(RAND_MAX)) * 2) - 1;
		stars[i].y = ((float(rand()) / float(RAND_MAX)) * 2) - 1;
		stars[i].z = (float(rand()) / float(RAND_MAX));
	}

	screen = InitializeSDL( SCREEN_WIDTH, SCREEN_HEIGHT );
	while( NoQuitMessageSDL() )
	{
		Update();
		Draw();
	}
	SDL_SaveBMP( screen, "screenshot.bmp" );
	return 0;
}

void Draw()
{
	//vec3 topLeft(1, 0, 0); // red
	//vec3 topRight(0, 0, 1); // blue
	//vec3 bottomLeft(0, .5, .5); // green
	//vec3 bottomRight(1, 1, 0); // yellow

	//vector<vec3> leftSide(SCREEN_HEIGHT);
	//vector<vec3> rightSide(SCREEN_HEIGHT);
	//Interpolate(topLeft, bottomLeft, leftSide);
	//Interpolate(topRight, bottomRight, rightSide);

	//for( int y=0; y<SCREEN_HEIGHT; ++y )
	//{
	//	vector<vec3> row(SCREEN_WIDTH);
	//	Interpolate(rightSide[y], leftSide[y], row);
	//	for( int x=0; x<SCREEN_WIDTH; ++x )
	//	{
	//		PutPixelSDL( screen, x, y, row[x]);
	//	}
	//}

	SDL_FillRect(screen, 0, 0);
	if (SDL_MUSTLOCK(screen))
		SDL_LockSurface(screen);
	float f = SCREEN_HEIGHT / 2;
	for (size_t s = 0; s<stars.size(); ++s) {
		// Add code for projecting and drawing each star
		// The 3D coordinate is on the same side as the 2D projection (PP) from the COP.
		// Offset the center point to be the center of the screen.
		float u = f * (stars[s].x / stars[s].z) + SCREEN_WIDTH / 2;
		float v = f * (stars[s].y / stars[s].z) + SCREEN_HEIGHT / 2;
		vec3 color = 0.2f * vec3(1, 1, 1) / (stars[s].z*stars[s].z);
		PutPixelSDL(screen, u, v, color);
	}
	if( SDL_MUSTLOCK(screen) )
		SDL_UnlockSurface(screen);

	SDL_UpdateRect( screen, 0, 0, 0, 0 );
}

void Interpolate(float a, float b, vector<float>& result) {
	if (result.empty()) return;

	// Set the value of the first element
	result[0] = a;

	// Do a single element check
	if (result.size() == 1) return;

	float step = (b - a) / (result.size() - 1);
	for (size_t i = 1; i < result.size(); ++i) {
		result[i] = result[i -1] + step;
	}
}

void Interpolate(vec3 a, vec3 b, vector<vec3>& result) {
	if (result.empty()) return;

	// Set all the initial values
	result[0].x = a.x;
	result[0].y = a.y;
	result[0].z = a.z;

	// Check for a result of one
	if (result.size() == 1) return;

	// Fill in the following steps
	float xStep = (b.x - a.x) / (result.size() - 1);
	float yStep = (b.y - a.y) / (result.size() - 1);
	float zStep = (b.z - a.z) / (result.size() - 1);
	for (size_t i = 1; i < result.size(); ++i) {
		result[i].x = result[i - 1].x + xStep;
		result[i].y = result[i - 1].y + yStep;
		result[i].z = result[i - 1].z + zStep;
	}
}

void Update() {
	int t2 = SDL_GetTicks();
	float dt = float(t2 - t) / 1000; // in seconds
	t = t2;

	for (int s = 0; s<stars.size(); ++s)
	{
		// Update the new position of the star.
		stars[s].z = stars[s].z - (VELOCITY * dt);

		if (stars[s].z <= 0)
			stars[s].z += 1;
		if (stars[s].z > 1)
			stars[s].z -= 1;
	}
}