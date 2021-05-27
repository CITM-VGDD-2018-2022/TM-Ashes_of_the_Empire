#include "MO_Renderer3D.h"
#include "MaykMath.h"
#include "MMGui.h"

#include "MO_Window.h"
#include "MO_Camera3D.h"
#include "MO_Editor.h"
#include "MO_Scene.h"
#include "MO_Input.h"
#include "MO_GUI.h"
#include"MO_Window.h"
#include "MO_Pathfinding.h"

#include "RE_Mesh.h"
#include "RE_Texture.h"
#include "RE_Shader.h"
#include "RE_Material.h"
#include "mmgr/mmgr.h"

#include"WI_Game.h"

#include"GameObject.h"

#include "CO_MeshRenderer.h"
#include "CO_Camera.h"
#include "CO_Transform.h"
#include "CO_ParticleSystem.h"
#include "CO_DirectionalLight.h"
#include "CO_AreaLight.h"

#include"Primitive.h"
#include"MathGeoLib/include/Geometry/Triangle.h"

#include"IM_TextureImporter.h"			//Delete this

#ifdef _DEBUG
#pragma comment (lib, "MathGeoLib/libx86/MGDebug/MathGeoLib.lib")
#else
#pragma comment (lib, "MathGeoLib/libx86/MGRelease/MathGeoLib.lib")
#endif

#pragma comment (lib, "opengl32.lib") /* link Microsoft OpenGL lib   */
#pragma comment (lib, "Glew/libx86/glew32.lib") /* link Microsoft OpenGL lib   */


ModuleRenderer3D::ModuleRenderer3D(Application* app, bool start_enabled) : Module(app, start_enabled), str_CAPS(""),
vsync(false), wireframe(false), gameCamera(nullptr), resolution(2), debugDraw(true), clearColor(float4::one)
{
	GetCAPS(str_CAPS);
	/*depth =*/ cull = lightng = color_material = texture_2d = true;

	//framebuffer = texColorBuffer = rbo = 0;
}

// Destructor
ModuleRenderer3D::~ModuleRenderer3D()
{
}

// Called before render is available
bool ModuleRenderer3D::Init()
{
	LOG(LogType::L_NORMAL, "Init: 3D Renderer context");
	bool ret = true;

	//ASK: Can i do this inside the MM namespace?
	MaykMath::Init();
	LOG(LogType::L_NORMAL, "Init: MaykMath");

#ifdef STANDALONE
	vsync = true;
	SDL_SetWindowFullscreen(App->moduleWindow->window, SDL_WINDOW_FULLSCREEN_DESKTOP);
	SDL_GetWindowSize(App->moduleWindow->window, &App->moduleWindow->s_width, &App->moduleWindow->s_height);
	App->moduleRenderer3D->OnResize(App->moduleWindow->s_width, App->moduleWindow->s_height);
#endif // STANDALONE

	//Create context
	context = SDL_GL_CreateContext(App->moduleWindow->window);
	if (context == NULL)
	{
		LOG(LogType::L_ERROR, "OpenGL context could not be created! SDL_Error: %s\n", SDL_GetError());
		ret = false;
	}

	GLenum error = glewInit();
	if (error != GL_NO_ERROR)
	{
		LOG(LogType::L_ERROR, "Error initializing glew library! %s", SDL_GetError());
		ret = false;
	}
	else
	{
		LOG(LogType::L_NORMAL, "Init: Glew %s", glewGetString(GLEW_VERSION));
	}

	if (ret == true)
	{
		//Use Vsync
		if (SDL_GL_SetSwapInterval(static_cast<int>(vsync)) < 0)
			LOG(LogType::L_WARNING, "Warning: Unable to set VSync! SDL Error: %s\n", SDL_GetError());

		//Initialize Projection Matrix
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();

		//Check for error
		GLenum error = glGetError();
		if (error != GL_NO_ERROR)
		{
			//gluErrorString
			LOG(LogType::L_ERROR, "Error initializing OpenGL! %s\n", glewGetErrorString(error));
			ret = false;
		}

		//Initialize Modelview Matrix
		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();

		//Check for error
		error = glGetError();
		if (error != GL_NO_ERROR)
		{
			//ASK: Maybe glewGetErrorString is not the same as glutGerErrorString, so errors could be wrong
			LOG(LogType::L_ERROR, "Error initializing OpenGL! %s\n", glewGetErrorString(error));
			ret = false;
		}

		glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);
		glClearDepth(1.0f);

		//Initialize clear color
		glClearColor(0.f, 0.f, 0.f, 1.f);
		glClear(GL_COLOR_BUFFER_BIT);

		//Check for error
		error = glGetError();
		if (error != GL_NO_ERROR)
		{
			//ASK: Maybe glewGetErrorString is not the same as glutGerErrorString, so errors could be wrong
			LOG(LogType::L_ERROR, "Error initializing OpenGL! %s\n", glewGetErrorString(error));
			ret = false;
		}

		// Blend for transparency
		glEnable(GL_ALPHA_TEST);
		glAlphaFunc(GL_GREATER, 0.1f);

		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		//glBlendEquation(GL_FUNC_ADD);

		GLfloat LightModelAmbient[] = { 0.0f, 0.0f, 0.0f, 0.0f };
		glLightModelfv(GL_LIGHT_MODEL_AMBIENT, LightModelAmbient);

		lights[0].ref = GL_LIGHT0;
		lights[0].ambient.Set(0.75f, 0.75f, 0.75f, 1.0f);
		lights[0].diffuse.Set(0.05f, 0.05f, 0.05f, 1.0f);
		lights[0].SetPos(0.0f, 2.0f, 2.5f);
		lights[0].Init();

		GLfloat MaterialAmbient[] = { 1.0f, 1.0f, 1.0f, 1.0f };
		glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, MaterialAmbient);

		GLfloat MaterialDiffuse[] = { 1.0f, 1.0f, 1.0f, 1.0f };
		glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, MaterialDiffuse);

		glEnable(GL_MULTISAMPLE);
		glEnable(GL_DEPTH_TEST);
		glEnable(GL_CULL_FACE);
		lights[0].Active(true);
		glEnable(GL_LIGHTING);
		glEnable(GL_COLOR_MATERIAL);
		glEnable(GL_TEXTURE_2D);
	}

	//Generate texture
	for (int i = 0; i < SQUARE_TEXTURE_W; i++) {
		for (int j = 0; j < SQUARE_TEXTURE_H; j++) {
			int c = ((((i & 0x8) == 0) ^ (((j & 0x8)) == 0))) * 255;
			checkerImage[i][j][0] = (GLubyte)c;
			checkerImage[i][j][1] = (GLubyte)c;
			checkerImage[i][j][2] = (GLubyte)c;
			checkerImage[i][j][3] = (GLubyte)255;
		}
	}

	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
	glGenTextures(1, &checkersTexture);
	glBindTexture(GL_TEXTURE_2D, checkersTexture);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, SQUARE_TEXTURE_W, SQUARE_TEXTURE_H, 0, GL_RGBA, GL_UNSIGNED_BYTE, checkerImage);
	glBindTexture(GL_TEXTURE_2D, 0);

	//Generate default normal map

	for (int i = 0; i < SQUARE_TEXTURE_W; i++) {
		for (int j = 0; j < SQUARE_TEXTURE_H; j++) {
			defaultNormalMapImage[i][j][0] = (GLubyte)127;
			defaultNormalMapImage[i][j][1] = (GLubyte)127;
			defaultNormalMapImage[i][j][2] = (GLubyte)255;
			defaultNormalMapImage[i][j][3] = (GLubyte)255;
		}
	}

	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
	glGenTextures(1, &defaultNormalMap);
	glBindTexture(GL_TEXTURE_2D, defaultNormalMap);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, SQUARE_TEXTURE_W, SQUARE_TEXTURE_H, 0, GL_RGBA, GL_UNSIGNED_BYTE, defaultNormalMapImage);
	glBindTexture(GL_TEXTURE_2D, 0);

	//Generate default specular map

	for (int i = 0; i < SQUARE_TEXTURE_W; i++) {
		for (int j = 0; j < SQUARE_TEXTURE_H; j++) {
			defaultSpecularMapImage[i][j][0] = (GLubyte)255;
			defaultSpecularMapImage[i][j][1] = (GLubyte)255;
			defaultSpecularMapImage[i][j][2] = (GLubyte)255;
			defaultSpecularMapImage[i][j][3] = (GLubyte)255;
		}
	}

	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
	glGenTextures(1, &defaultSpecularMap);
	glBindTexture(GL_TEXTURE_2D, defaultSpecularMap);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, SQUARE_TEXTURE_W, SQUARE_TEXTURE_H, 0, GL_RGBA, GL_UNSIGNED_BYTE, defaultSpecularMapImage);
	glBindTexture(GL_TEXTURE_2D, 0);

	// Projection matrix for
	OnResize(App->moduleWindow->s_width, App->moduleWindow->s_height);

	//std::vector<char*> faces = {
	//	"EngineIcons/Skybox/right.jpg",
	//	"EngineIcons/Skybox/left.jpg",
	//	"EngineIcons/Skybox/top.jpg",
	//	"EngineIcons/Skybox/bottom.jpg",
	//	"EngineIcons/Skybox/front.jpg",
	//	"EngineIcons/Skybox/back.jpg"
	//};

	//TextureImporter::LoadCubeMap(faces, skybox);
	skybox.CreateGLData();

	glEnable(GL_TEXTURE_CUBE_MAP_SEAMLESS);

	postProcessing.Init();
	return ret;
}

// PreUpdate: clear buffer
update_status ModuleRenderer3D::PreUpdate(float dt)
{
	return UPDATE_CONTINUE;
}

// PostUpdate present buffer to screen
update_status ModuleRenderer3D::PostUpdate(float dt)
{
	//Render light depth pass
	DirectionalLightShadowPass();
	AreaLightShadowPass();

	//-------- CAMERA CULLING PROCESS -----------//
	if (GetGameRenderTarget() != nullptr && GetGameRenderTarget()->cullingState == true)
	{
		std::vector<C_MeshRenderer*> copy = renderQueue;
		renderQueue.clear();
		for (size_t i = 0; i < copy.size(); i++)
		{
			if (GetGameRenderTarget()->IsInsideFrustum(copy[i]->globalAABB))
				renderQueue.push_back(copy[i]);
		}
		copy.clear();
	}

#ifndef STANDALONE

	App->moduleCamera->editorCamera.StartDraw();

	//TODO: This should not be here
	if (!renderQueue.empty())
	{
		for (size_t i = 0; i < renderQueue.size(); i++)
		{
			float distance = App->moduleCamera->editorCamera.camFrustrum.pos.DistanceSq(renderQueue[i]->globalOBB.pos);
			renderQueueMap.emplace(distance, renderQueue[i]);
		}
		//TODO: Make wireframe only affect scene window
		(wireframe) ? glPolygonMode(GL_FRONT_AND_BACK, GL_LINE) : glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
		RenderWithOrdering();
		(wireframe) ? glPolygonMode(GL_FRONT_AND_BACK, GL_FILL) : glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	}

	glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);

	for (int i = 0; i < areaLightVector.size(); ++i)
	{
		//glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

		areaLightVector[i]->DebugDraw();
	}

	glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

	DrawRays();

	//DrawParticleSystems();//THIS IS LOCATED INSIDE RENDER STENCIL FOR NOW (UNTIL WE HAVE A POST PROCESSING SYSTEM)

	if (App->moduleCamera->editorCamera.drawSkybox)
		skybox.DrawAsSkybox(&App->moduleCamera->editorCamera);

	if (debugDraw)
	{
		Grid p(0, 0, 0, 0);
		p.axis = true;

		p.Render();

		DebugLine(pickingDebug);
		DrawDebugLines();
	}

	for (size_t i = 0; i < renderQueueStencil.size(); i++)
	{
		float distance = App->moduleCamera->editorCamera.camFrustrum.pos.DistanceSq(renderQueueStencil[i]->globalOBB.pos);
		renderQueueMapStencil.emplace(distance, renderQueueStencil[i]);
	}
	for (size_t i = 0; i < renderQueuePostStencil.size(); i++)
	{
		float distance = App->moduleCamera->editorCamera.camFrustrum.pos.DistanceSq(renderQueuePostStencil[i]->globalOBB.pos);
		renderQueueMapPostStencil.emplace(distance, renderQueuePostStencil[i]);
	}

	RenderStencilWithOrdering(true);

	App->moduleCamera->editorCamera.EndDraw();

#endif // !STANDALONE

	//Draw game camera
	if (gameCamera != nullptr)
	{
		gameCamera->StartDraw();

		if (!renderQueue.empty())
		{
			for (size_t i = 0; i < renderQueue.size(); i++)
			{
				float distance = gameCamera->camFrustrum.pos.DistanceSq(renderQueue[i]->globalOBB.pos);
				renderQueueMap.emplace(distance, renderQueue[i]);
			}

			RenderWithOrdering(true);
		}

		DrawRays();
		//DrawParticleSystems();//THIS IS LOCATED INSIDE RENDER STENCIL FOR NOW (UNTIL WE HAVE A POST PROCESSING SYSTEM)

		if (gameCamera->drawSkybox)
			skybox.DrawAsSkybox(gameCamera);


		for (size_t i = 0; i < renderQueueStencil.size(); ++i)
		{
			float distance = gameCamera->camFrustrum.pos.DistanceSq(renderQueueStencil[i]->globalOBB.pos);
			renderQueueMapStencil.emplace(distance, renderQueueStencil[i]);
		}
		for (size_t i = 0; i < renderQueuePostStencil.size(); ++i)
		{
			float distance = gameCamera->camFrustrum.pos.DistanceSq(renderQueuePostStencil[i]->globalOBB.pos);
			renderQueueMapPostStencil.emplace(distance, renderQueuePostStencil[i]);
		}

		RenderStencilWithOrdering(true);

		gameCamera->EndDraw();
		if (gameCamera->postProcessProfile != nullptr)
		{
			postProcessing.DoPostProcessing(gameCamera->resolvedFBO.texBufferSize.x, gameCamera->resolvedFBO.texBufferSize.y, gameCamera->resolvedFBO, gameCamera->resolvedFBO.GetColorTexture(), gameCamera->resolvedFBO.GetDepthTexture(), gameCamera);
		}

		gameCamera->resolvedFBO.BindFrameBuffer();
		glClear(GL_DEPTH_BUFFER_BIT);
		App->moduleGui->RenderCanvas2D();
		glEnable(GL_DEPTH_TEST);
		glClear(GL_DEPTH_BUFFER_BIT);
		gameCamera->resolvedFBO.UnbindFrameBuffer();

#ifdef STANDALONE
		gameCamera->resolvedFBO.ResolveToScreen();
#endif

	}


#ifndef STANDALONE
	App->moduleEditor->Draw();
#endif // !STANDALONE

	//TEMPORAL: Delete here so you can call mouse picking from scene window, should not be here in the future
	ClearAllRenderData();

	App->moduleScene->LoadHoldScene();

	SDL_GL_SwapWindow(App->moduleWindow->window);

	return UPDATE_CONTINUE;
}

// Called before quitting
bool ModuleRenderer3D::CleanUp()
{
	directLightVector.clear();
	areaLightVector.clear();

	LOG(LogType::L_NORMAL, "Destroying 3D Renderer");
	skybox.ClearMemory();

	glDeleteTextures(1, &checkersTexture);
	glDeleteTextures(1, &defaultNormalMap);
	glDeleteTextures(1, &defaultSpecularMap);

	postProcessing.CleanUp();

	SDL_GL_DeleteContext(context);
	ClearAllRenderData();

	return true;
}


void ModuleRenderer3D::OnResize(int width, int height)
{
	glViewport(0, 0, width, height);
	//ProjectionMatrix = perspective(60.0f, (float)width / (float)height, 0.125f, 512.0f);

	App->moduleWindow->s_width = width;
	App->moduleWindow->s_height = height;

#ifndef STANDALONE
	App->moduleCamera->editorCamera.ReGenerateBuffer(width, height);
#endif // !STANDALONE

	if (gameCamera != nullptr)
	{
		gameCamera->ReGenerateBuffer(width, height);
	}
}

#ifndef STANDALONE
void ModuleRenderer3D::OnGUI()
{
	if (ImGui::CollapsingHeader("Rendering", ImGuiTreeNodeFlags_DefaultOpen))
	{
		//TODO: Store all this info as const char* and dont call the functions every frame
		SDL_version ver;
		SDL_GetVersion(&ver);
		ImGui::Text("SDL Version: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%d.%d.%d", ver.major, ver.minor, ver.patch);
		ImGui::Text("OpenGL Version: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%s", glGetString(GL_VERSION));
		ImGui::TextWrapped("All external library versions can be found in the 'About' window with links to their pages.");

		ImGui::GreySeparator();
		ImGui::Text("CPUs: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%d (Cache: %dkb)", SDL_GetCPUCount(), SDL_GetCPUCacheLineSize());
		ImGui::Text("System RAM: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%.1fGb", SDL_GetSystemRAM() / 1000.f);

#pragma region Caps
		ImGui::Text("Caps:");
		ImGui::SameLine();

		ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), str_CAPS.c_str());

#pragma endregion


		ImGui::GreySeparator();
		ImGui::Text("GPU: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), (const char*)glGetString(GL_VENDOR));
		ImGui::Text("Brand: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), (const char*)glGetString(GL_RENDERER));

		// Memory --------------------
		sMStats stats = m_getMemoryStatistics();

		ImGui::Text("Total Reported Mem: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%u bytes", stats.totalReportedMemory);
		ImGui::Text("Total Actual Mem: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%u bytes", stats.totalActualMemory);
		ImGui::Text("Peak Reported Mem: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%u bytes", stats.peakReportedMemory);
		ImGui::Text("Peak Actual Mem: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "%u bytes", stats.peakActualMemory);

		ImGui::Checkbox("Enable V-Sync", &vsync);
		////Use Vsync
		//if (SDL_GL_SetSwapInterval(static_cast<int>(vsync)) < 0)
		//	LOG(LogType::L_WARNING, "Warning: Unable to set VSync! SDL Error: %s\n", SDL_GetError());

		ImGui::SameLine();
		ImGui::Checkbox("Wireframe Mode", &wireframe);

		//if (ImGui::Checkbox("Depth Test", &depth))
		//	(depth) ? glEnable(GL_DEPTH_TEST) : glDisable(GL_DEPTH_TEST);
		ImGui::SameLine();
		if (ImGui::Checkbox("Cull face", &cull))
			(cull) ? glEnable(GL_CULL_FACE) : glDisable(GL_CULL_FACE);

		if (ImGui::Checkbox("Lighting", &lightng))
			(lightng) ? glEnable(GL_LIGHTING) : glDisable(GL_LIGHTING);
		ImGui::SameLine();
		if (ImGui::Checkbox("Color material", &color_material))
			(color_material) ? glEnable(GL_COLOR_MATERIAL) : glDisable(GL_COLOR_MATERIAL);

		ImGui::SameLine();
		if (ImGui::Checkbox("Texture 2D", &texture_2d))
			(texture_2d) ? glEnable(GL_TEXTURE_2D) : glDisable(GL_TEXTURE_2D);

		ImGui::Checkbox("DebugDraw", &debugDraw);

		ImGui::Separator();

		if (ImGui::DragFloat4("Ambient: ", &lights[0].ambient, 0.01f, 0.f, 1.f))
			lights[0].Init();
		if (ImGui::DragFloat4("Diffuse: ", &lights[0].diffuse, 0.01f, 0.f, 1.f))
			lights[0].Init();

	}
}


void ModuleRenderer3D::DrawDebugLines()
{
	glBegin(GL_LINES);
	for (size_t i = 0; i < lines.size(); i++)
	{
		glColor3fv(lines[i].color.ptr());
		glVertex3fv(lines[i].a.ptr());
		glVertex3fv(lines[i].b.ptr());

		glColor3f(255.f, 255.f, 255.f);
	}
	glEnd();

	lines.clear();

	glBegin(GL_TRIANGLES);
	for (size_t i = 0; i < triangles.size(); i++)
	{
		glColor3fv(triangles[i].color.ptr());

		glVertex3fv(triangles[i].a.ptr());
		glVertex3fv(triangles[i].b.ptr());
		glVertex3fv(triangles[i].c.ptr());
	}

	glColor3f(255.f, 255.f, 255.f);
	glEnd();

	triangles.clear();

	glPointSize(20.0f);
	glBegin(GL_POINTS);
	for (size_t i = 0; i < points.size(); i++)
	{
		glColor3fv(points[i].color.ptr());
		glVertex3fv(points[i].position.ptr());
		glColor3f(255.f, 255.f, 255.f);
	}
	glEnd();
	glPointSize(1.0f);

	points.clear();

	EngineExternal->modulePathfinding->DebugDraw();
}
void ModuleRenderer3D::AddDebugLines(float3& a, float3& b, float3& color)
{
	lines.push_back(LineRender(a, b, color));
}

void ModuleRenderer3D::AddDebugTriangles(float3& a, float3& b, float3& c, float3& color)
{
	triangles.push_back(DebugTriangle(a, b, c, color));
}
void ModuleRenderer3D::AddDebugPoints(float3& position, float3& color)
{
	points.push_back(DebugPoint(position, color));
}

void ModuleRenderer3D::DrawBox(float3* points, float3 color)
{
	glColor3fv(&color.x);
	glLineWidth(2.f);

	//Draw plane
	EngineExternal->moduleRenderer3D->AddDebugLines(points[0], points[2], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[2], points[6], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[6], points[4], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[4], points[0], color);

	EngineExternal->moduleRenderer3D->AddDebugLines(points[0], points[1], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[1], points[3], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[3], points[2], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[4], points[5], color);

	EngineExternal->moduleRenderer3D->AddDebugLines(points[6], points[7], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[5], points[7], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[3], points[7], color);
	EngineExternal->moduleRenderer3D->AddDebugLines(points[1], points[5], color);

	glLineWidth(1.f);
	glColor3f(1.f, 1.f, 1.f);
}

void ModuleRenderer3D::DebugLine(LineSegment& line)
{
	glLineWidth(2.f);
	this->AddDebugLines(pickingDebug.a, pickingDebug.b, float3(1.f, 0.f, 0.f));
	glLineWidth(1.f);
}
#endif // !STANDALONE

void ModuleRenderer3D::AddRay(float3& a, float3& b, float3& color, float& rayWidth)
{
	rays.push_back(LineRender(a, b, color, rayWidth));
}

void ModuleRenderer3D::DrawRays()
{
	for (size_t i = 0; i < rays.size(); i++)
	{
		glLineWidth(rays[i].width);
		glBegin(GL_LINES);
		glColor3fv(rays[i].color.ptr());
		glVertex3fv(rays[i].a.ptr());
		glVertex3fv(rays[i].b.ptr());

		glColor3f(255.f, 255.f, 255.f);
		glEnd();
	}
	//rays.clear();
	glLineWidth(1.0f);
}

void ModuleRenderer3D::RayToMeshQueueIntersection(LineSegment& ray)
{
	pickingDebug = ray;

	std::map<float, C_MeshRenderer*> canSelect;
	float nHit = 0;
	float fHit = 0;

	bool selected = false;
	for (std::vector<C_MeshRenderer*>::iterator i = renderQueue.begin(); i != renderQueue.end(); ++i)
	{
		if (ray.Intersects((*i)->globalAABB, nHit, fHit))
			canSelect[nHit] = (*i);
	}
	for (std::vector<C_MeshRenderer*>::iterator i = renderQueuePostStencil.begin(); i != renderQueuePostStencil.end(); ++i)
	{
		if (ray.Intersects((*i)->globalAABB, nHit, fHit))
			canSelect[nHit] = (*i);
	}

	//Add all meshes with a triangle hit and store the distance from the ray to the triangle, then pick the closest one
	std::map<float, C_MeshRenderer*> distMap;
	for (auto i = canSelect.begin(); i != canSelect.end(); ++i)
	{
		const ResourceMesh* _mesh = (*i).second->GetRenderMesh();
		if (_mesh)
		{
			LineSegment local = ray;
			local.Transform((*i).second->GetGO()->transform->globalTransform.Inverted());

			if (_mesh->vertices_count >= 9) //TODO: Had to do this to avoid squared meshes crash
			{
				for (uint index = 0; index < _mesh->indices_count; index += 3)
				{
					float3 pA(&_mesh->vertices[_mesh->indices[index] * VERTEX_ATTRIBUTES]);
					float3 pB(&_mesh->vertices[_mesh->indices[index + 1] * VERTEX_ATTRIBUTES]);
					float3 pC(&_mesh->vertices[_mesh->indices[index + 2] * VERTEX_ATTRIBUTES]);

					Triangle triangle(pA, pB, pC);

					float dist = 0;
					if (local.Intersects(triangle, &dist, nullptr))
						distMap[dist] = (*i).second;

				}
			}
		}
	}
	canSelect.clear();


#ifndef STANDALONE

	GameObject* gameobject_to_return = nullptr;
	if (distMap.begin() != distMap.end())
	{
		App->moduleEditor->SetSelectedGO((*distMap.begin()).second->GetGO());
		selected = true;

		/*if (EngineExternal->moduleInput->GetKey(SDL_SCANCODE_LSHIFT) == KEY_REPEAT)
			EngineExternal->moduleEditor->AddSelectedGameObject((*distMap.begin()).second->GetGO());
		else
			App->moduleEditor->SetSelectedGO((*distMap.begin()).second->GetGO());*/

	}

	//If nothing is selected, set selected GO to null
	if (!selected)
		App->moduleEditor->SetSelectedGO(nullptr);

#endif // !STANDALONE

	distMap.clear();

}

void ModuleRenderer3D::RenderWithOrdering(bool rTex)
{
	if (renderQueueMap.empty())
		return;

	for (auto i = renderQueueMap.rbegin(); i != renderQueueMap.rend(); ++i)
	{
		// Get the range of the current key
		auto range = renderQueueMap.equal_range(i->first);

		// Now render out that whole range
		for (auto d = range.first; d != range.second; ++d)
			d->second->RenderMesh(rTex);
	}
	renderQueueMap.clear();
}

void ModuleRenderer3D::RenderStencilWithOrdering(bool rTex)
{
	if (renderQueueMapStencil.empty())
	{
		DrawParticleSystems();
		return;
	}


	glEnable(GL_STENCIL_TEST);
	glClear(GL_STENCIL_BUFFER_BIT);
	//1. First we mask the silouhettes of the objects that will be drawn as stencil

	glStencilFunc(GL_ALWAYS, 1, 0xFF);
	glStencilOp(GL_KEEP, GL_REPLACE, GL_KEEP); //only write into the stencil mask as 1 the fragments that do not pass the depth test
	glDepthFunc(GL_LESS);
	//Only enable writting to the stencil buffer, no depth or color
	glStencilMask(0xFF);//enable writting to the stencil buffer
	glDepthMask(GL_FALSE);
	glColorMask(GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);


	for (auto i = renderQueueMapStencil.rbegin(); i != renderQueueMapStencil.rend(); ++i)
	{
		// Get the range of the current key
		auto range = renderQueueMapStencil.equal_range(i->first);

		// Now render out that whole range
		for (auto d = range.first; d != range.second; ++d)
			d->second->RenderMeshStencil(rTex);
	}

	glStencilFunc(GL_EQUAL, 0, 0xFF);
	glStencilOp(GL_KEEP, GL_KEEP, GL_REPLACE);//TODO original is keep keep replace //only write into the color & depth masks fragments that do pass the depth test and pass the stencil
	glDepthMask(GL_TRUE);
	glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
	glStencilMask(0x00);//disable writting to the stencil buffer
	//Draw meshes that haven't been rendered
	for (auto i = renderQueueMapPostStencil.rbegin(); i != renderQueueMapPostStencil.rend(); ++i)
	{
		// Get the range of the current key
		auto range = renderQueueMapPostStencil.equal_range(i->first);

		// Now render out that whole range
		for (auto d = range.first; d != range.second; ++d)
			d->second->RenderMesh(rTex);
	}
	DrawParticleSystems();
	//==================================================================
	//2. We then draw the stencil objects in front of the mask

	glStencilFunc(GL_NOTEQUAL, 0, 0xFF);
	//glDepthMask(GL_FALSE);
	glDisable(GL_DEPTH_TEST);
	//We now want to draw on the color & depth buffers takin into account the stencil (without changing it)
	//glClear(GL_DEPTH_BUFFER_BIT);

	for (auto i = renderQueueMapStencil.rbegin(); i != renderQueueMapStencil.rend(); ++i)
	{
		// Get the range of the current key
		auto range = renderQueueMapStencil.equal_range(i->first);

		// Now render out that whole range
		for (auto d = range.first; d != range.second; ++d)
			d->second->RenderMeshStencil(rTex);
	}

	//==================================================================
	//Clear(only the needed ones) & reset buffers to their normal state
	renderQueueMapStencil.clear();
	glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
	glEnable(GL_DEPTH_TEST);
	glDepthMask(GL_TRUE);
	glDepthFunc(GL_LESS);
	glStencilMask(0xFF);//enable writting to the stencil buffer
	glDisable(GL_STENCIL_TEST);
	glClear(GL_STENCIL_BUFFER_BIT);
}


void ModuleRenderer3D::DirectionalLightShadowPass()
{
	if (directLightVector.size() > 0)
	{
		for (int i = 0; i < directLightVector.size(); ++i)
		{
			if (directLightVector[i]->calculateShadows == true)
			{
				directLightVector[i]->StartPass();
				if (!renderQueue.empty())
				{
					for (size_t j = 0; j < renderQueue.size(); ++j)
					{
						if (renderQueue[j]->GetDrawShadows())
						{
							float distance = directLightVector[i]->orthoFrustum.pos.DistanceSq(renderQueue[j]->globalOBB.pos);
							renderQueueMap.emplace(distance, renderQueue[j]);
						}
					}

					for (size_t j = 0; j < renderQueuePostStencil.size(); ++j)
					{
						if (renderQueuePostStencil[j]->GetDrawShadows())
						{
							float distance = directLightVector[i]->orthoFrustum.pos.DistanceSq(renderQueuePostStencil[j]->globalOBB.pos);
							renderQueueMap.emplace(distance, renderQueuePostStencil[j]);
						}
					}

					if (!renderQueueMap.empty())
					{
						for (auto j = renderQueueMap.rbegin(); j != renderQueueMap.rend(); ++j)
						{
							// Get the range of the current key
							auto range = renderQueueMap.equal_range(j->first);

							// Now render out that whole range
							for (auto d = range.first; d != range.second; ++d)
							{
								GLint modelLoc = glGetUniformLocation(directLightVector[i]->depthShader->shaderProgramID, "model");
								glUniformMatrix4fv(modelLoc, 1, GL_FALSE, d->second->GetGO()->transform->GetGlobalTransposed());

								modelLoc = glGetUniformLocation(directLightVector[i]->depthShader->shaderProgramID, "lightSpaceMatrix");
								glUniformMatrix4fv(modelLoc, 1, GL_FALSE, directLightVector[i]->spaceMatrixOpenGL.ptr());

								d->second->TryCalculateBones();
								d->second->GetRenderMesh()->PushDefaultMeshUniforms(directLightVector[i]->depthShader->shaderProgramID, 0, d->second->GetGO()->transform, float3::one);
								d->second->GetRenderMesh()->OGL_GPU_Render();
							}
						}

						renderQueueMap.clear();
					}
				}
				directLightVector[i]->EndPass();
			}
		}
	}
}


void ModuleRenderer3D::AreaLightShadowPass()
{
	if (areaLightVector.size() > 0)
	{
		for (int i = 0; i < areaLightVector.size(); ++i)
		{
			if (areaLightVector[i]->calculateShadows == true)
			{
				areaLightVector[i]->StartPass();
				if (!renderQueue.empty())
				{
					for (size_t j = 0; j < renderQueue.size(); ++j)
					{
						if (renderQueue[j]->GetDrawShadows())
						{
							float distance = areaLightVector[i]->shadowTransforms[0].pos.DistanceSq(renderQueue[j]->globalOBB.pos);
							renderQueueMap.emplace(distance, renderQueue[j]);
						}
					}

					for (size_t j = 0; j < renderQueuePostStencil.size(); ++j)
					{
						if (renderQueuePostStencil[j]->GetDrawShadows())
						{
							float distance = areaLightVector[i]->shadowTransforms[0].pos.DistanceSq(renderQueuePostStencil[j]->globalOBB.pos);
							renderQueueMap.emplace(distance, renderQueuePostStencil[j]);
						}
					}

					if (!renderQueueMap.empty())
					{
						for (auto j = renderQueueMap.rbegin(); j != renderQueueMap.rend(); ++j)
						{
							// Get the range of the current key
							auto range = renderQueueMap.equal_range(j->first);

							// Now render out that whole range
							for (auto d = range.first; d != range.second; ++d)
							{
								GLint modelLoc = glGetUniformLocation(areaLightVector[i]->depthShader->shaderProgramID, "model_matrix");
								glUniformMatrix4fv(modelLoc, 1, GL_FALSE, d->second->GetGO()->transform->GetGlobalTransposed());

								char buffer[64];
								for (int k = 0; k < 6; ++k)
								{
									sprintf(buffer, "shadowMatrices[%i]", k);
									modelLoc = glGetUniformLocation(areaLightVector[i]->depthShader->shaderProgramID, buffer);
									glUniformMatrix4fv(modelLoc, 1, GL_FALSE, (areaLightVector[i]->shadowTransforms[k].ProjectionMatrix() * areaLightVector[i]->shadowTransforms[k].ViewMatrix()).Transposed().ptr());
								}

								modelLoc = glGetUniformLocation(areaLightVector[i]->depthShader->shaderProgramID, "lightPosition");
								glUniform3fv(modelLoc, 1, &areaLightVector[i]->GetGO()->transform->position.x);

								modelLoc = glGetUniformLocation(areaLightVector[i]->depthShader->shaderProgramID, "farPlane");
								glUniform1f(modelLoc, ALIGHT_FAR_PLANE_DISTANCE);

								d->second->TryCalculateBones();
								d->second->GetRenderMesh()->PushDefaultMeshUniforms(areaLightVector[i]->depthShader->shaderProgramID, 0, d->second->GetGO()->transform, float3::one);
								d->second->GetRenderMesh()->OGL_GPU_Render();
							}
						}

						renderQueueMap.clear();
					}
				}
				areaLightVector[i]->EndPass();
			}
		}
	}
}


/*Get SDL caps*/
void ModuleRenderer3D::GetCAPS(std::string& caps)
{
	caps += (SDL_HasRDTSC()) ? "RDTSC," : "";
	caps += (SDL_HasMMX()) ? "MMX, " : "";
	caps += (SDL_HasSSE()) ? "SSE, " : "";
	caps += (SDL_HasSSE2()) ? "SSE2, " : "";
	caps += (SDL_HasSSE3()) ? "SSE3, " : "";
	caps += "\n";
	caps += (SDL_HasSSE41()) ? "SSE41, " : "";
	caps += (SDL_HasSSE42()) ? "SSE42, " : "";
	caps += (SDL_HasAVX()) ? "AVX, " : "";
	caps += (SDL_HasAltiVec()) ? "AltiVec, " : "";
	caps += (SDL_Has3DNow()) ? "3DNow, " : "";
}

C_Camera* ModuleRenderer3D::GetGameRenderTarget() const
{
	return gameCamera;
}

void ModuleRenderer3D::SetGameRenderTarget(C_Camera* cam)
{
	gameCamera = cam;

#ifndef STANDALONE
	W_Game* gWindow = dynamic_cast<W_Game*>(App->moduleEditor->GetEditorWindow(EditorWindow::GAME));
	if (gWindow != nullptr && gameCamera != nullptr)
		gWindow->SetTargetCamera(gameCamera);
#endif // !STANDALONE

	if (gameCamera != nullptr)
		gameCamera->ReGenerateBuffer(App->moduleWindow->s_width, App->moduleWindow->s_height);
}

void ModuleRenderer3D::ClearAllRenderData()
{
	renderQueue.clear();
	renderQueueStencil.clear();
	renderQueuePostStencil.clear();

	renderQueueMap.clear();
	renderQueueMapStencil.clear();
	renderQueueMapPostStencil.clear();

	particleSystemQueue.clear();
	rays.clear();

	lines.clear();
	triangles.clear();
	points.clear();
}

bool ModuleRenderer3D::IsWalkable(float3 pointToCheck)
{
	LineSegment walkablePoint = LineSegment(float3(pointToCheck.x, -20.0, pointToCheck.z), float3(pointToCheck.x, 20.0, pointToCheck.z));

	float nHit = 0;
	float fHit = 0;

	for (std::vector<C_MeshRenderer*>::iterator i = renderQueue.begin(); i != renderQueue.end(); ++i)
	{
		if (walkablePoint.Intersects((*i)->globalAABB, nHit, fHit))
		{
			//walkablePoints.push_back(walkablePoint);
			return true;
		}
	}

	for (std::vector<C_MeshRenderer*>::iterator i = renderQueuePostStencil.begin(); i != renderQueuePostStencil.end(); ++i)
	{
		if (walkablePoint.Intersects((*i)->globalAABB, nHit, fHit))
		{
			//walkablePoints.push_back(walkablePoint);
			return true;
		}
	}

	/*if (walkable)
	{
		glColor3f(0.f, 1.f, 0.f);
		glLineWidth(2.f);
		glBegin(GL_LINES);
		glVertex3fv(&walkablePoint.a.x);
		glVertex3fv(&walkablePoint.b.x);
		glEnd();
		glLineWidth(1.f);
		glColor3f(1.f, 1.f, 1.f);
	}
	else
	{
		glColor3f(1.f, 0.f, 0.f);
		glLineWidth(2.f);
		glBegin(GL_LINES);
		glVertex3fv(&walkablePoint.a.x);
		glVertex3fv(&walkablePoint.b.x);
		glEnd();
		glLineWidth(1.f);
		glColor3f(1.f, 1.f, 1.f);
	}*/

	return false;
}


void ModuleRenderer3D::AddDirectionalLight(C_DirectionalLight* light)
{
	for (int i = 0; i < directLightVector.size(); ++i)
	{
		if (directLightVector[i] == light)
			return;
	}

	directLightVector.push_back(light);

	if (directLightVector.size() > MAX_DIRECTIONAL_LIGHTS)
		directLightVector.erase(directLightVector.begin());
}


void ModuleRenderer3D::RemoveDirectionalLight(C_DirectionalLight* light)
{
	for (int i = 0; i < directLightVector.size(); ++i)
	{
		if (directLightVector[i] == light)
		{
			directLightVector.erase(directLightVector.begin() + i);
			return;
		}
	}
}


void ModuleRenderer3D::AddAreaLight(C_AreaLight* light)
{
	for (int i = 0; i < areaLightVector.size(); ++i)
	{
		if (areaLightVector[i] == light)
			return;
	}

	areaLightVector.push_back(light);

	if (areaLightVector.size() > MAX_AREA_LIGHTS)
		areaLightVector.erase(areaLightVector.begin());
}


void ModuleRenderer3D::RemoveAreaLight(C_AreaLight* light)
{
	for (int i = 0; i < areaLightVector.size(); ++i)
	{
		if (areaLightVector[i] == light)
		{
			areaLightVector.erase(areaLightVector.begin() + i);
			return;
		}
	}
}


void ModuleRenderer3D::PushLightUniforms(ResourceMaterial* material)
{
	char buffer[64];

	for (int i = 0; i < directLightVector.size(); ++i)
	{
		sprintf(buffer, "lightInfo[%i].activeLight", i);
		GLint modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
		glUniform1i(modelLoc, true);

		directLightVector[i]->PushLightUniforms(material, i);
	}

	for (int i = directLightVector.size(); i < MAX_DIRECTIONAL_LIGHTS; ++i)
	{
		sprintf(buffer, "lightInfo[%i].activeLight", i);
		GLint modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
		glUniform1i(modelLoc, false);
	}

	for (int i = 0; i < areaLightVector.size(); ++i)
	{
		sprintf(buffer, "areaLightInfo[%i].activeLight", i);
		GLint modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
		glUniform1i(modelLoc, true);

		areaLightVector[i]->PushLightUniforms(material, i);
	}

	for (int i = areaLightVector.size(); i < MAX_AREA_LIGHTS; ++i)
	{
		sprintf(buffer, "areaLightInfo[%i].activeLight", i);
		GLint modelLoc = glGetUniformLocation(material->shader->shaderProgramID, buffer);
		glUniform1i(modelLoc, false);
	}
}


void ModuleRenderer3D::DrawParticleSystems()
{
	int systemCount = particleSystemQueue.size();

	for (int i = 0; i < systemCount; ++i)
	{
		Component* partSy = particleSystemQueue[i]->GetComponent(Component::TYPE::PARTICLE_SYSTEM);

		if (partSy != nullptr)
			static_cast<C_ParticleSystem*>(partSy)->Draw();
	}
}
