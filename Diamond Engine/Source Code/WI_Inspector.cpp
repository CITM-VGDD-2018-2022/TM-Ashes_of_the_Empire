#ifndef STANDALONE

#include "WI_Inspector.h"
#include"WI_Assets.h"

#include "MMGui.h"
#include "GameObject.h"
#include "Application.h"

#include "MO_Scene.h"
#include"MO_ResourceManager.h"
#include"MO_MonoManager.h"
#include "MO_Editor.h"

#include "CO_NavMeshAgent.h"
#include "CO_Material.h"
#include"CO_Script.h"
#include"RE_Material.h"

//todelete
#include "Application.h"
#include "MO_Physics.h"
#include "IM_PrefabImporter.h"

#include <algorithm>

W_Inspector::W_Inspector() : Window(), selectedGO(nullptr), editingRes(nullptr)
{
	name = "Inspector";
}

W_Inspector::~W_Inspector()
{
	if (editingRes)
		EngineExternal->moduleResources->UnloadResource(editingRes->GetUID());
}

void W_Inspector::Draw()
{

	//ImGui::PushStyleVar(ImGuiStyleVar_WindowMinSize, ImVec2(1000, 1000));

	if (ImGui::Begin(name.c_str(), NULL /*| ImGuiWindowFlags_NoResize*/))
	{
		if (editingRes != nullptr && editingRes->GetType() == Resource::Type::MATERIAL)
		{
			std::string matSuffix = "##Mat";
			dynamic_cast<ResourceMaterial*>(editingRes)->DrawEditor(matSuffix);
		}
		else
		{
			if (selectedGO != nullptr && !selectedGO->IsRoot())
			{
				if (ImGui::Checkbox("##Active", &selectedGO->active))
				{
					//The bool has changed on the checkbox call at this point
					(selectedGO->active == true) ? selectedGO->Enable() : selectedGO->Disable();
				}

				ImGui::SameLine();

				strcpy(inputName, selectedGO->name.c_str());
				if (ImGui::InputText("##Name", &inputName[0], sizeof(inputName)))
				{
					if (inputName[0] != '\0')
						selectedGO->name = inputName;
				}
				ImGui::SameLine();


				ImGui::Checkbox("Static", &selectedGO->isStatic);

				ImGui::Text("Tag"); ImGui::SameLine();

				ImGuiStyle& style = ImGui::GetStyle();
				float w = ImGui::CalcItemWidth();
				float spacing = style.ItemInnerSpacing.x;
				float button_sz = ImGui::GetFrameHeight();
				ImGui::PushItemWidth((w - spacing * 2.0f - button_sz * 2.0f) * 0.5f);

				std::vector<std::string> tags = EngineExternal->moduleScene->tags;

				if (ImGui::BeginCombo("##tags", selectedGO->tag))
				{
					for (int t = 0; t < tags.size(); t++)
					{
						bool is_selected = strcmp(selectedGO->tag, tags[t].c_str()) == 0;
						if (ImGui::Selectable(tags[t].c_str(), is_selected)) {
							strcpy(selectedGO->tag, tags[t].c_str());
						}

						if (is_selected)
							ImGui::SetItemDefaultFocus();
					}
					if (ImGui::BeginMenu("Add Tag"))
					{
						static char newTag[32];
						ImGui::InputText("", newTag, IM_ARRAYSIZE(newTag));

						if (ImGui::Button("Save Tag")) {
							char* tagToAdd = new char[IM_ARRAYSIZE(newTag)];
							strcpy(tagToAdd, newTag);
							EngineExternal->moduleScene->tags.push_back(tagToAdd);
							newTag[0] = '\0';
						}
						ImGui::EndMenu();
					}

					int tag_to_remove = -1;
					if (ImGui::BeginMenu("Remove Tag"))
					{
						for (int t = 0; t < tags.size(); t++)
						{
							if (ImGui::Selectable(tags[t].c_str(), false)) {
								tag_to_remove = t;
								//Remove Tag
							}
						}
						ImGui::EndMenu();
					}

					if (tag_to_remove != -1)
						EngineExternal->moduleScene->tags.erase(EngineExternal->moduleScene->tags.begin() + tag_to_remove);

					ImGui::EndCombo();
				}
				ImGui::PopItemWidth();

				ImGui::SameLine();

				std::vector<std::string> layers = EngineExternal->moduleScene->layers;

				ImGui::Text("Layer"); ImGui::SameLine();
				style = ImGui::GetStyle();
				w = ImGui::CalcItemWidth();
				spacing = style.ItemInnerSpacing.x;
				button_sz = ImGui::GetFrameHeight();
				ImGui::PushItemWidth((w - spacing * 2.0f - button_sz * 2.0f) * 0.5f);
				if (ImGui::BeginCombo("##layers", selectedGO->layer))
				{
					for (int n = 0; n < layers.size(); n++)
					{
						bool is_selected = strcmp(selectedGO->layer, layers[n].c_str()) == 0;
						if (ImGui::Selectable(layers[n].c_str(), is_selected)) {
							strcpy(selectedGO->layer, layers[n].c_str());
						}

						if (is_selected)
							ImGui::SetItemDefaultFocus();
					}
					ImGui::EndCombo();
				}
				ImGui::PopItemWidth();

				if (ImGui::Button("Delete")) {
					selectedGO->Destroy();
				}

				ImGui::SameLine();
				ImGui::Text("UID: %d", selectedGO->UID);

				ImGui::Spacing();
				ImGui::Checkbox("Dont Destroy", &selectedGO->dontDestroy);

				ImGui::Text("Prefab Reference: %i", selectedGO->prefabReference);

				if (selectedGO->prefabID != 0u)
				{
					ImGui::Text("Prefab ID: %d", selectedGO->prefabID);
					if (ImGui::Button("Override Prefab")) {
						PrefabImporter::OverridePrefab(selectedGO->prefabID, selectedGO);
					}

					//ImGui::SameLine();
					//if (ImGui::Button("Revert Changes")) {
					//	PrefabImporter::OverrideGameObject(selectedGO->prefabID, selectedGO);
					//}

					ImGui::SameLine();
					if (ImGui::Button("Unlink Prefab")) {
						selectedGO->UnlinkFromPrefab();
					}
				}

				ImGui::GreySeparator();

				for (size_t i = 0; i < selectedGO->components.size(); i++)
				{
					selectedGO->components[i]->OnEditor();
				}

				ImGui::Separator();

				ImGui::CenterNextElement(ImGui::GetWindowSize(), 0.5f);
				if (ImGui::BeginCombo("##addComponent", "Add Component"))
				{
					if (ImGui::Selectable("Mesh Renderer"))
					{
						if (selectedGO->GetComponent(Component::TYPE::MESH_RENDERER) == nullptr)
							selectedGO->AddComponent(Component::TYPE::MESH_RENDERER);
					}
					if (ImGui::Selectable("Material"))
					{
						if (selectedGO->GetComponent(Component::TYPE::MATERIAL) == nullptr)
							selectedGO->AddComponent(Component::TYPE::MATERIAL);
					}
					if (ImGui::Selectable("Stencil Material"))
					{
						if (selectedGO->GetComponent(Component::TYPE::STENCIL_MATERIAL) == nullptr)
							selectedGO->AddComponent(Component::TYPE::STENCIL_MATERIAL);
					}
					if (ImGui::Selectable("Camera"))
					{
						if (selectedGO->GetComponent(Component::TYPE::CAMERA) == nullptr)
							selectedGO->AddComponent(Component::TYPE::CAMERA);
					}
					if (ImGui::Selectable("RigidBody3D"))
					{
						if (selectedGO->GetComponent(Component::TYPE::RIGIDBODY) == nullptr)
							selectedGO->AddComponent(Component::TYPE::RIGIDBODY);
					}
					if (ImGui::Selectable("Box Collider"))
					{
						//if (selectedGO->GetComponent(Component::TYPE::BOXCOLLIDER) == nullptr)
						selectedGO->AddComponent(Component::TYPE::BOXCOLLIDER);
					}
					if (ImGui::Selectable("Mesh Collider"))
					{
						if (selectedGO->GetComponent(Component::TYPE::RIGIDBODY) != nullptr)
							selectedGO->AddComponent(Component::TYPE::MESHCOLLIDER);

					}
					if (ImGui::Selectable("Sphere Collider"))
					{
						selectedGO->AddComponent(Component::TYPE::SPHERECOLLIDER);

					}
					if (ImGui::Selectable("Capsule Collider"))
					{
						if (selectedGO->GetComponent(Component::TYPE::RIGIDBODY) != nullptr)
						selectedGO->AddComponent(Component::TYPE::CAPSULECOLLIDER);

					}
					if (ImGui::Selectable("AudioListener"))
					{
						if (selectedGO->GetComponent(Component::TYPE::AUDIO_LISTENER) == nullptr)
							selectedGO->AddComponent(Component::TYPE::AUDIO_LISTENER);
					}
					if (ImGui::Selectable("AudioSource"))
					{
						if (selectedGO->GetComponent(Component::TYPE::AUDIO_SOURCE) == nullptr)
							selectedGO->AddComponent(Component::TYPE::AUDIO_SOURCE);
					}
					if (ImGui::Selectable("Animator"))
					{
						if (selectedGO->GetComponent(Component::TYPE::ANIMATOR) == nullptr)
							selectedGO->AddComponent(Component::TYPE::ANIMATOR);
					}

					if (ImGui::Selectable("Particle System"))
					{
						if (selectedGO->GetComponent(Component::TYPE::PARTICLE_SYSTEM) == nullptr)
							selectedGO->AddComponent(Component::TYPE::PARTICLE_SYSTEM);
					}
					if (ImGui::Selectable("Billboard"))
					{
						if (selectedGO->GetComponent(Component::TYPE::BILLBOARD) == nullptr)
							selectedGO->AddComponent(Component::TYPE::BILLBOARD);
					}
					if (ImGui::Selectable("Directional Light"))
					{
						if (selectedGO->GetComponent(Component::TYPE::DIRECTIONAL_LIGHT) == nullptr)
							selectedGO->AddComponent(Component::TYPE::DIRECTIONAL_LIGHT);
					}
					if (ImGui::Selectable("Area Light"))
					{
						if (selectedGO->GetComponent(Component::TYPE::AREA_LIGHT) == nullptr)
							selectedGO->AddComponent(Component::TYPE::AREA_LIGHT);
					}
					if (ImGui::Selectable("Nav Mesh Agent"))
					{
						if (selectedGO->GetComponent(Component::TYPE::NAVMESHAGENT) == nullptr)
							selectedGO->AddComponent(Component::TYPE::NAVMESHAGENT);
					}

					if (ImGui::Selectable("Procedural mesh"))
					{
						if (selectedGO->GetComponent(Component::TYPE::PROCEDURAL_MESH) == nullptr)
							selectedGO->AddComponent(Component::TYPE::PROCEDURAL_MESH);
					}

					if (ImGui::BeginMenu("Scripts"))
					{
						std::vector<std::string> scripts;
						for (int i = 0; i < EngineExternal->moduleMono->userScripts.size(); i++)
						{
							scripts.push_back(std::string(mono_class_get_name(EngineExternal->moduleMono->userScripts[i])));
						}

						std::sort(scripts.begin(), scripts.end());
							
						for (size_t i = 0; i < scripts.size(); i++)
						{
							if (ImGui::MenuItem(scripts[i].c_str()))
							{
								selectedGO->AddComponent(Component::TYPE::SCRIPT, scripts[i].c_str());
							}
						}

						ImGui::EndMenu();
					}

					


					ImGui::EndCombo();
				}

				ImGui::BeginChild("##dragDrop", ImGui::GetContentRegionAvail());
				ImGui::EndChild();
				ComponentDragAndDrop();

			}
		}
	}


	ImGui::End();
}

void W_Inspector::SetEditingResource(Resource* res)
{
	if (editingRes != nullptr)
		EngineExternal->moduleResources->UnloadResource(editingRes->GetUID());

	editingRes = res;

}

void W_Inspector::ComponentDragAndDrop()
{
	if (ImGui::BeginDragDropTarget())
	{
		if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_MATERIAL"))
		{
			std::string* assetsPath = (std::string*)payload->Data;

			if (selectedGO->GetComponent(Component::TYPE::MATERIAL) == nullptr)
			{
				C_Material* componentMaterial = dynamic_cast<C_Material*>(selectedGO->AddComponent(Component::TYPE::MATERIAL));
				ResourceMaterial* resourceMaterial = dynamic_cast<ResourceMaterial*>(EngineExternal->moduleResources->RequestFromAssets(assetsPath->c_str()));

				componentMaterial->SetMaterial(resourceMaterial);
			}
		}
		ImGui::EndDragDropTarget();
	}
}

#endif // !STANDALONE