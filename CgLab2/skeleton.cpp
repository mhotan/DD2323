#include <iostream>
#include <glm/glm.hpp>
#include <SDL.h>
#include "SDLauxiliary.h"
#include "TestModel.h"

using namespace std;
using glm::vec3;
using glm::mat3;

// ----------------------------------------------------------------------------
// Data Structures

struct Intersection
{
	vec3 position;
	float distance;
	int triangleIndex;
};

// ----------------------------------------------------------------------------
// GLOBAL VARIABLES

const int SCREEN_WIDTH = 100;
const int SCREEN_HEIGHT = SCREEN_WIDTH;
SDL_Surface* screen;
int t;
vector<Triangle> triangles;

float focalLength = SCREEN_HEIGHT * 3 / 2;
vec3 cameraPos(0, 0, - ((2 * focalLength / SCREEN_HEIGHT) + 1));

// ----------------------------------------------------------------------------
// FUNCTIONS

void Update();
void Draw();

/*
Checks to see if there is a triangle plane that intersects with the
vector with the vector.  If it doesn't find a triangle then false is returned. 
If it does find a triangle returns true and sets the Intersection reference.

Arguments:
IN
start : The start location of the light source 
dir : Direction of the light from start
triangles : All the triangles surfaces
closestIntersection : The intersection of the light source and surface.

*/
bool ClosestIntersection(
	vec3 start,
	vec3 dir,
	const vector<Triangle>& triangles,
	Intersection& closestIntersection
	);

int main( int argc, char* argv[] )
{
	screen = InitializeSDL( SCREEN_WIDTH, SCREEN_HEIGHT );
	t = SDL_GetTicks();	// Set start value for timer.
	LoadTestModel(triangles);

	while( NoQuitMessageSDL() )
	{
		Update();
		Draw();
	}

	SDL_SaveBMP( screen, "screenshot.bmp" );
	return 0;
}

void Update()
{
	// Compute frame time:
	int t2 = SDL_GetTicks();
	float dt = float(t2-t);
	t = t2;
	cout << "Render time: " << dt << " ms." << endl;

	Uint8* keystate = SDL_GetKeyState(0);
	if (keystate[SDLK_UP])
	{
		// Move camera forward
	}
	if (keystate[SDLK_DOWN])
	{
		// Move camera backward
	}
	if (keystate[SDLK_LEFT])
	{
		// Move camera to the left
	}
	if (keystate[SDLK_RIGHT])
	{
		// Move camera to the right
	}
}

void Draw()
{
	if( SDL_MUSTLOCK(screen) )
		SDL_LockSurface(screen);

	vec3 black(0, 0, 0);
	for( int y=0; y<SCREEN_HEIGHT; ++y )
	{
		for( int x=0; x<SCREEN_WIDTH; ++x )
		{
			vec3 dir(x - SCREEN_WIDTH / 2, y - SCREEN_HEIGHT / 2, focalLength);
			Intersection inter;
			if (ClosestIntersection(cameraPos, dir, triangles, inter)) {
				PutPixelSDL(screen, x, y, triangles[inter.triangleIndex].color);
			}
			else {
				PutPixelSDL(screen, x, y, black);
			}
		}
	}

	if( SDL_MUSTLOCK(screen) )
		SDL_UnlockSurface(screen);

	SDL_UpdateRect( screen, 0, 0, 0, 0 );
}

bool ClosestIntersection(
	vec3 start,
	vec3 dir,
	const vector<Triangle>& triangles,
	Intersection& closestIntersection
	) {

	// Initially set the closest value to be max.
	closestIntersection.distance = std::numeric_limits<float>::max();
	bool foundIntersection = false;

	// Iterate through all the triangles
	for (size_t i = 0; i < triangles.size(); ++i) {
		
		Triangle triangle = triangles[i];
		// Calculate the value of x, (t, u, v)
		vec3 v0 = triangle.v0 ;
		vec3 v1 = triangle.v1 ;
		vec3 v2 = triangle.v2 ;
		vec3 e1 = v1 - v0;
		vec3 e2 = v2 - v0;
		vec3 b = start - v0;
		mat3 A(-dir, e1, e2);
		vec3 x = glm::inverse(A) * b;

		// Check if there is point that is in the triangle pane
		// Map to mathematical variables.
		float t = x.x;
		float u = x.y;
		float v = x.z;

		// If intersection is found.
		if (0 <= u && 0 <= v && u + v <= 1 && t >= 0) {
			// If the distance is closer then the current minimum.;
			if (t < closestIntersection.distance) {
				closestIntersection.distance = t;
				closestIntersection.position = x;
				closestIntersection.triangleIndex = i;
			}
			foundIntersection = true;
		}
	}
	return foundIntersection;
}