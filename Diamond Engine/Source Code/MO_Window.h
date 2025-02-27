#ifndef __ModuleWindow_H__
#define __ModuleWindow_H__

#include "Module.h"
#include "SDL/include/SDL.h"

class Application;

class ModuleWindow : public Module
{
public:

	ModuleWindow(Application* app, bool start_enabled = true);

	// Destructor
	virtual ~ModuleWindow();

	bool Init() override;
	bool CleanUp() override;

#ifndef STANDALONE
	void OnGUI() override;
#endif // !STANDALONE

	void SetTitle(const char* title);

public:
	//The window we'll be rendering to
	SDL_Window* window;

	//The surface contained by the window
	SDL_Surface* screen_surface;

	int s_width;
	int s_height;
	float brightness;

	bool fullScreen;
	bool borderless;
	bool resizable;
	bool fullScreenDesktop;

	int windowMode;

};

#endif // __ModuleWindow_H__