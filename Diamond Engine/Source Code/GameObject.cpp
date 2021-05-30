#include "GameObject.h"
#include "Component.h"

#include "CO_Transform.h"
#include "CO_MeshRenderer.h"
#include "CO_Material.h"
#include "CO_Camera.h"
#include "CO_Script.h"
#include "CO_Animator.h"
#include "CO_RigidBody.h"
#include "CO_Collider.h"
#include "CO_BoxCollider.h"
#include "CO_SphereCollider.h"
#include "CO_MeshCollider.h"
#include "CO_CapsuleCollider.h"
#include "CO_AudioListener.h"
#include "CO_AudioSource.h"
#include "CO_Transform2D.h"
#include "CO_Button.h"
#include "CO_Text.h"
#include "CO_Canvas.h"
#include "CO_Image2D.h"
#include "CO_Checkbox.h"
#include "CO_ParticleSystem.h"
#include "CO_Billboard.h"
#include "CO_Navigation.h"
#include "CO_DirectionalLight.h"
#include "CO_NavMeshAgent.h"
#include "CO_StencilMaterial.h"
#include "CO_AreaLight.h"
#include "CO_ProceduralMesh.h"

#include"MO_Scene.h"

#include "IM_PrefabImporter.h"

#include"MaykMath.h"
#include"DEJsonSupport.h"
#include <algorithm>

#include"Application.h"
#include"MO_Editor.h"


GameObject::GameObject(const char* _name, GameObject* parent, int _uid) : parent(parent), name(_name), showChildren(false),
active(true), isStatic(false), toDelete(false), dontDestroy(false), UID(_uid), transform(nullptr), dumpComponent(nullptr),
prefabID(0u), prefabReference(0u), tag("Untagged"), layer("Default")
{
	if (parent != nullptr)
		parent->children.push_back(this);

	transform = dynamic_cast<C_Transform*>(AddComponent(Component::TYPE::TRANSFORM));

	//TODO: Should make sure there are not duplicated ID's
	if (UID == -1)
	{
		UID = EngineExternal->GetRandomInt();
	}
	//UID = MaykMath::Random(0, INT_MAX);
}


GameObject::~GameObject()
{
#ifndef STANDALONE
	if (EngineExternal->moduleEditor->GetSelectedGO() == this)
		EngineExternal->moduleEditor->SetSelectedGO(nullptr);
#endif // !STANDALONE

	for (size_t i = 0; i < components.size(); i++)
	{
		delete components[i];
		components[i] = nullptr;
	}
	components.clear();

	for (size_t i = 0; i < children.size(); i++)
	{
		delete children[i];
		children[i] = nullptr;
	}
	children.clear();

	for (size_t i = 0; i < csReferences.size(); i++)
	{
		mono_field_set_value(mono_gchandle_get_target(csReferences[i]->parentSC->noGCobject), csReferences[i]->field, NULL);
		csReferences[i]->fiValue.goValue = nullptr;
	}
	csReferences.clear();
}


void GameObject::Update()
{
	if (dumpComponent != nullptr)
	{
		components.erase(std::find(components.begin(), components.end(), dumpComponent));
		delete dumpComponent;
		dumpComponent = nullptr;
	}

	for (size_t i = 0; i < components.size(); i++)
	{
		if (components[i]->IsActive())
			components[i]->Update();
	}
}

void GameObject::PostUpdate()
{
	for (size_t i = 0; i < components.size(); i++)
	{
		if (components[i]->IsActive())
			components[i]->PostUpdate();
	}
}

Component* GameObject::AddComponent(Component::TYPE _type, const char* params)
{
	assert(_type != Component::TYPE::NONE, "Can't create a NONE component");
	Component* ret = nullptr;

	//TODO: Make a way to add only 1 instance components like transform and camera
	switch (_type)
	{
	case Component::TYPE::TRANSFORM:
		if (transform == nullptr)
			ret = new C_Transform(this);
		break;
	case Component::TYPE::MESH_RENDERER:
		ret = new C_MeshRenderer(this);
		break;
	case Component::TYPE::MATERIAL:
		ret = new C_Material(this);
		break;
	case Component::TYPE::SCRIPT:
		assert(params != nullptr, "Script without name can't be created");
		ret = new C_Script(this, params);
		break;
	case Component::TYPE::CAMERA:
		ret = new C_Camera(this);
		EngineExternal->moduleScene->SetGameCamera(dynamic_cast<C_Camera*>(ret));
		break;
	case Component::TYPE::ANIMATOR:
		ret = new C_Animator(this);
		break;
	case Component::TYPE::RIGIDBODY:
		ret = new C_RigidBody(this);
		break;
	case Component::TYPE::COLLIDER:
		ret = new C_BoxCollider(this);
		break;
	case Component::TYPE::BOXCOLLIDER:
		ret = new C_BoxCollider(this);
		break;
	case Component::TYPE::SPHERECOLLIDER:
		ret = new C_SphereCollider(this);
		break;
	case Component::TYPE::CAPSULECOLLIDER:
		ret = new C_CapsuleCollider(this);
		break;
	case Component::TYPE::MESHCOLLIDER:
		ret = new C_MeshCollider(this);
		break;
	case Component::TYPE::AUDIO_LISTENER:
		ret = new C_AudioListener(this);
		break;
	case Component::TYPE::AUDIO_SOURCE:
		ret = new C_AudioSource(this);
		break;

	case Component::TYPE::TRANSFORM_2D:
		ret = new C_Transform2D(this);
		break;

	case Component::TYPE::BUTTON:
		ret = new C_Button(this);
		break;

	case Component::TYPE::CHECKBOX:
		ret = new C_Checkbox(this);
		break;

	case Component::TYPE::NAVIGATION:
		assert(params != nullptr, "You need to specify the type of ui");
		if ("Button" == params)
			ret = new C_Navigation(this, Component::TYPE::BUTTON);
		else if ("Checkbox" == params)
			ret = new C_Navigation(this, Component::TYPE::CHECKBOX);
		else
			LOG(LogType::L_WARNING, "The Navigation component hasn't been created because the type wasn't correct");
		break;

	case Component::TYPE::TEXT_UI:
		ret = new C_Text(this);
		break;

	case Component::TYPE::CANVAS:
		ret = new C_Canvas(this);
		break;

	case Component::TYPE::IMAGE_2D:
		ret = new C_Image2D(this);
		break;

	case Component::TYPE::PARTICLE_SYSTEM:
		ret = new C_ParticleSystem(this);
		break;
	case Component::TYPE::BILLBOARD:
		ret = new C_Billboard(this);
		break;
	case Component::TYPE::DIRECTIONAL_LIGHT:
		ret = new C_DirectionalLight(this);
		break;
	case Component::TYPE::NAVMESHAGENT:
		ret = new C_NavMeshAgent(this);
		break;
	case Component::TYPE::STENCIL_MATERIAL:
		ret = new C_StencilMaterial(this);
		break;

	case Component::TYPE::AREA_LIGHT:
		ret = new C_AreaLight(this);
		break;

	case Component::TYPE::PROCEDURAL_MESH:
		ret = new C_ProceduralMesh(this);
		break;
	}

	if (ret != nullptr)
	{
		ret->type = _type;
		components.push_back(ret);
	}

	return ret;
}


Component* GameObject::GetComponent(Component::TYPE _type, const char* scriptName)
{
	for (size_t i = 0; i < components.size(); i++)
	{
		if (components[i] && components[i]->type == _type)
		{
			if (_type == Component::TYPE::SCRIPT)
			{
				if (scriptName != nullptr && strcmp(components[i]->GetName().c_str(), scriptName) == 0)
					return components[i];
			}
			else
			{
				return components[i];
			}
		}
	}

	return nullptr;
}

std::vector<Component*> GameObject::GetComponentsOfType(Component::TYPE type)
{
	std::vector< Component*> ret;
	for (size_t i = 0; i < components.size(); ++i)
	{
		if (components[i]->type == type)
			ret.push_back(components[i]);
	}
	return ret;
}

void GameObject::RecursivePrefabReferenceGeneration()
{
	if (prefabReference == 0u)
		prefabReference = UID;

	for (size_t i = 0; i < children.size(); i++)
	{
		children[i]->RecursivePrefabReferenceGeneration();
	}
}

//When we load models from model trees the UID should get regenerated
//because the .model UID are not unique.
void GameObject::RecursiveUIDRegeneration()
{
	this->UID = EngineExternal->GetRandomInt();

	for (size_t i = 0; i < this->children.size(); i++)
	{
		this->children[i]->RecursiveUIDRegeneration();
	}
}


void GameObject::RecursiveUIDRegenerationSavingReferences(std::map<uint, GameObject*>& gameObjects)
{
	gameObjects[UID] = this;
	UID = EngineExternal->GetRandomInt();

	for (size_t i = 0; i < this->children.size(); i++)
	{
		this->children[i]->RecursiveUIDRegenerationSavingReferences(gameObjects);
	}
}

void GameObject::UnlinkFromPrefab()
{
	prefabID = 0;
	prefabReference = 0;

	for (size_t i = 0; i < children.size(); i++)
	{
		children[i]->UnlinkFromPrefab();
	}
}

void GameObject::OverrideGameObject(uint _prefabID, bool prefabChild)
{
	if (prefabID == _prefabID || prefabChild == true)
	{
		for (size_t i = components.size() - 1; i > 0; --i)
		{
			delete components[i];
			components[i] = nullptr;
			components.erase(components.end() - 1);
		}

		for (size_t i = 0; i < children.size(); i++)
		{
			children[i]->OverrideGameObject(_prefabID, true);
		}

		if (prefabID == _prefabID)
			PrefabImporter::OverrideGameObject(prefabID, this);
	}
	else
	{
		for (size_t i = 0; i < children.size(); i++)
		{
			children[i]->OverrideGameObject(_prefabID, false);
		}
	}

}

bool GameObject::isActive() const
{
	return active;
}


//void GameObject::ChangeActiveState()
//{
//	(active == true) ? Disable() : Enable();
//}


void GameObject::Enable()
{
	active = true;

	if (parent != nullptr)
		parent->Enable();
}


void GameObject::Disable()
{
	active = false;
	//for (size_t i = 0; i < children.size(); i++)
	//{
	//	children[i]->Disable();
	//}
}

void GameObject::EnableTopDown()
{
	for (int i = 0; i < children.size(); i++) {
		children[i]->EnableTopDown();
	}
	Component* nav = GetComponent(Component::TYPE::NAVIGATION);
	if (nav != nullptr) {
		nav->Enable();
	}
}

void GameObject::DisableTopDown()
{
	for (int i = 0; i < children.size(); i++) {
		children[i]->DisableTopDown();
	}
	Component* nav = GetComponent(Component::TYPE::NAVIGATION);
	if (nav != nullptr) {
		nav->Disable();
	}
}


bool GameObject::IsRoot()
{
	return (parent == nullptr) ? true : false;
}


void GameObject::Destroy()
{
	toDelete = true;
}


void GameObject::SaveToJson(JSON_Array* _goArray, bool saveAllData)
{
	JSON_Value* goValue = json_value_init_object();
	JSON_Object* goData = json_value_get_object(goValue);

	json_object_set_string(goData, "name", name.c_str());
	json_object_set_string(goData, "tag", tag);
	json_object_set_string(goData, "layer", layer);

	//Save all gameObject data
	DEJson::WriteBool(goData, "Active", active);

	//Saving Scene
	//if (!saveAllData)
	//{
		DEJson::WriteInt(goData, "UID", UID);
		if (parent)
			DEJson::WriteInt(goData, "ParentUID", parent->UID);

		DEJson::WriteInt(goData, "PrefabReference", prefabReference);
	//}

	DEJson::WriteInt(goData, "PrefabID", prefabID);

	DEJson::WriteBool(goData, "DontDestroy", dontDestroy);
	DEJson::WriteBool(goData, "Static", isStatic);

	DEJson::WriteVector3(goData, "Position", &transform->position[0]);
	DEJson::WriteQuat(goData, "Rotation", &transform->rotation.x);
	DEJson::WriteVector3(goData, "Scale", &transform->localScale[0]);

	json_array_append_value(_goArray, goValue);

	if (prefabID == 0u)
	{
		//Save components
		JSON_Value* goArray = json_value_init_array();
		JSON_Array* jsArray = json_value_get_array(goArray);
		for (size_t i = 0; i < components.size(); i++)
		{
			JSON_Value* nVal = json_value_init_object();
			JSON_Object* nObj = json_value_get_object(nVal);

			components[i]->SaveData(nObj);
			json_array_append_value(jsArray, nVal);
		}
		json_object_set_value(goData, "Components", goArray);
	}

	if (prefabID != 0 /*&& !saveAllData*/)
	{
		SaveAsPrefabRoot(goData, false);
		return;
	}

	for (size_t i = 0; i < children.size(); i++)
	{
		children[i]->SaveToJson(_goArray, children[i]->prefabID == 0u);
	}
}

void GameObject::SavePrefab(JSON_Array* _goArray, bool saveAllData)
{
	JSON_Value* goValue = json_value_init_object();
	JSON_Object* goData = json_value_get_object(goValue);

	json_object_set_string(goData, "name", name.c_str());
	json_object_set_string(goData, "tag", tag);
	json_object_set_string(goData, "layer", layer);

	//Save all gameObject data
	DEJson::WriteBool(goData, "Active", active);

	DEJson::WriteInt(goData, "UID", prefabReference);
	if (parent)
		DEJson::WriteInt(goData, "ParentUID", parent->prefabReference);

	DEJson::WriteInt(goData, "PrefabReference", 0);

	DEJson::WriteInt(goData, "PrefabID", prefabID);

	DEJson::WriteBool(goData, "DontDestroy", dontDestroy);
	DEJson::WriteBool(goData, "Static", isStatic);

	DEJson::WriteVector3(goData, "Position", &transform->position[0]);
	DEJson::WriteQuat(goData, "Rotation", &transform->rotation.x);
	DEJson::WriteVector3(goData, "Scale", &transform->localScale[0]);

	json_array_append_value(_goArray, goValue);

	if (saveAllData)
	{
		//Save components
		JSON_Value* goArray = json_value_init_array();
		JSON_Array* jsArray = json_value_get_array(goArray);
		for (size_t i = 0; i < components.size(); i++)
		{
			JSON_Value* nVal = json_value_init_object();
			JSON_Object* nObj = json_value_get_object(nVal);

			components[i]->SaveData(nObj);
			json_array_append_value(jsArray, nVal);
		}
		json_object_set_value(goData, "Components", goArray);
	}

	if (prefabID != 0u && !saveAllData)
	{
		SaveAsPrefabRoot(goData, true);
		return;
	}

	for (size_t i = 0; i < children.size(); i++)
	{
		children[i]->SavePrefab(_goArray, children[i]->prefabID == 0u);
	}
}

void GameObject::SaveAsPrefabRoot(JSON_Object* goData, bool prefabInsidePrefab)
{
	JSON_Value* childrenValue = json_value_init_array();
	JSON_Array* childrenArray = json_value_get_array(childrenValue);

	for (size_t i = 0; i < children.size(); i++)
	{
		if (children[i]->prefabID != 0u)
		{
			children[i]->SaveToJson(childrenArray, false);
		}
		else
		{
			children[i]->SaveReducedData(childrenArray, prefabInsidePrefab);
		}
	}

	json_object_set_value(goData, "PrefabObjects", childrenValue);
}

void GameObject::SaveReducedData(JSON_Array* goArray, bool prefabInsidePrefab)
{
	JSON_Value* goValue = json_value_init_object();
	JSON_Object* goData = json_value_get_object(goValue);

	json_object_set_string(goData, "name", name.c_str());
	json_object_set_string(goData, "tag", tag);
	json_object_set_string(goData, "layer", layer);

	//Save all gameObject data
	DEJson::WriteBool(goData, "Active", active);

	DEJson::WriteInt(goData, "UID", UID);
	if (parent)
		DEJson::WriteInt(goData, "ParentUID", prefabInsidePrefab == false ? parent->UID : parent->prefabReference);

	DEJson::WriteInt(goData, "PrefabID", prefabID);
	DEJson::WriteInt(goData, "PrefabReference", prefabReference);

	DEJson::WriteBool(goData, "DontDestroy", dontDestroy);
	DEJson::WriteBool(goData, "Static", isStatic);

	DEJson::WriteVector3(goData, "Position", &transform->position[0]);
	DEJson::WriteQuat(goData, "Rotation", &transform->rotation.x);
	DEJson::WriteVector3(goData, "Scale", &transform->localScale[0]);

	json_array_append_value(goArray, goValue);

	for (size_t i = 0; i < children.size(); i++)
	{
		children[i]->SaveReducedData(goArray);
	}
}

void GameObject::LoadFromJson(JSON_Object* _obj)
{
	active = DEJson::ReadBool(_obj, "Active");
	transform->SetTransformMatrix(DEJson::ReadVector3(_obj, "Position"), DEJson::ReadQuat(_obj, "Rotation"), DEJson::ReadVector3(_obj, "Scale"));
	prefabID = DEJson::ReadInt(_obj, "PrefabID");
	prefabReference = DEJson::ReadInt(_obj, "PrefabReference");
	LoadComponents(json_object_get_array(_obj, "Components"));
	dontDestroy = DEJson::ReadBool(_obj, "DontDestroy");
	isStatic = DEJson::ReadBool(_obj, "Static");

	const char* json_tag = DEJson::ReadString(_obj, "tag");

	if (json_tag == nullptr) sprintf_s(tag, "Untagged");
	else sprintf_s(tag, json_tag);

	const char* json_layer = DEJson::ReadString(_obj, "layer");

	if (json_layer == nullptr) sprintf_s(layer, "Default");
	else sprintf_s(layer, json_layer);
}

void GameObject::LoadForPrefab(JSON_Object* _obj)
{
	UID = DEJson::ReadInt(_obj, "UID");
	active = DEJson::ReadBool(_obj, "Active");
	transform->SetTransformMatrix(DEJson::ReadVector3(_obj, "Position"), DEJson::ReadQuat(_obj, "Rotation"), DEJson::ReadVector3(_obj, "Scale"));
	prefabID = DEJson::ReadInt(_obj, "PrefabID");
	prefabReference = DEJson::ReadInt(_obj, "PrefabReference");
	dontDestroy = DEJson::ReadBool(_obj, "DontDestroy");
	isStatic = DEJson::ReadBool(_obj, "Static");

	//Comment this lines if you want to load the GameObject's tag instead of the prefab tag
	const char* json_tag = DEJson::ReadString(_obj, "tag");
	if (json_tag == nullptr) sprintf_s(tag, "Untagged");
	else sprintf_s(tag, json_tag);

	//Comment this lines if you want to load the GameObject's layer instead of the prefab layer
	const char* json_layer = DEJson::ReadString(_obj, "layer");
	if (json_layer == nullptr) sprintf_s(layer, "Default");
	else sprintf_s(layer, json_layer);
}

void GameObject::CopyObjectData(JSON_Object* jsonObject)
{
	UID = DEJson::ReadInt(jsonObject, "UID");
	active = DEJson::ReadBool(jsonObject, "Active");
	transform->SetTransformMatrix(DEJson::ReadVector3(jsonObject, "Position"), DEJson::ReadQuat(jsonObject, "Rotation"), DEJson::ReadVector3(jsonObject, "Scale"));
	dontDestroy = DEJson::ReadBool(jsonObject, "DontDestroy");
	isStatic = DEJson::ReadBool(jsonObject, "Static");

	const char* json_tag = DEJson::ReadString(jsonObject, "tag");

	if (json_tag == nullptr) sprintf_s(tag, "Untagged");
	else sprintf_s(tag, json_tag);

	const char* json_layer = DEJson::ReadString(jsonObject, "layer");

	if (json_layer == nullptr) sprintf_s(layer, "Default");
	else sprintf_s(layer, json_layer);
}

void GameObject::GetChildrenUIDs(std::vector<uint>& UIDs)
{
	for (size_t i = 0; i < children.size(); i++)
	{
		UIDs.push_back(children[i]->UID);
		children[i]->GetChildrenUIDs(UIDs);
	}
}

void GameObject::LoadComponents(JSON_Array* componentArray)
{
	DEConfig conf(nullptr);
	for (size_t i = 1; i < json_array_get_count(componentArray); i++)
	{
		conf.nObj = json_array_get_object(componentArray, i);

		const char* scName = conf.ReadString("ScriptName");
		int num_type = conf.ReadInt("Type Of UI");
		if (num_type != 0) {
			switch (static_cast<Component::TYPE>(num_type)) {
			case Component::TYPE::BUTTON:
				scName = "Button";
				break;
			case Component::TYPE::CHECKBOX:
				scName = "Checkbox";
				break;
			}

		}
		Component* comp = AddComponent((Component::TYPE)conf.ReadInt("Type"), scName);

		if (comp != nullptr)
			comp->LoadData(conf);
	}
}

void GameObject::RemoveComponent(Component* ptr)
{
	dumpComponent = ptr;
}


//TODO: WTF IS GOING ON WITH THE ARNAU BUG FFS
//Deparenting objects with deformations grows transforms
void GameObject::ChangeParent(GameObject* newParent)
{
	//GameObject* ret = nullptr;
	//ret = IsChild(newParent, ret);
	if (IsChild(newParent))
		return;

	if (parent != nullptr)
		parent->RemoveChild(this);

	parent = newParent;
	parent->children.push_back(this);

	if (parent->prefabID != 0u || parent->prefabReference != 0u)
	{
		if (prefabReference == 0u)
			prefabReference = UID;
	}
	else if(prefabID == 0u)
		prefabReference = 0u;

	//TODO: This could be improved, you are setting up the local matrix 2 times
	transform->localTransform = parent->transform->globalTransform.Inverted() * transform->globalTransform;

	Quat _rot;
	float3 scale, pos;
	transform->localTransform.RotatePart().Decompose(_rot, scale);

	transform->SetTransformMatrix(transform->localTransform.TranslatePart(), _rot, scale);
	transform->updateTransform = true;
}


bool GameObject::IsChild(GameObject* _toFind)
{
	if (_toFind == nullptr)
		return false;

	if (_toFind == this)
	{
		return true;
	}
	else
	{
		return IsChild(_toFind->parent);
	}
}


void GameObject::RemoveChild(GameObject* child)
{
	child->parent = nullptr;
	children.erase(std::find(children.begin(), children.end(), child));
}

void GameObject::CollectChilds(std::vector<GameObject*>& vector)
{
	vector.push_back(this);
	for (uint i = 0; i < children.size(); i++)
		children[i]->CollectChilds(vector);
}

void GameObject::RemoveCSReference(SerializedField* fieldToRemove)
{
	for (size_t i = 0; i < csReferences.size(); ++i)
	{
		if (csReferences[i]->fiValue.goValue == fieldToRemove->fiValue.goValue)
		{
			mono_field_set_value(mono_gchandle_get_target(csReferences[i]->parentSC->noGCobject), csReferences[i]->field, NULL);
			csReferences.erase(csReferences.begin() + i);
		}
	}
}

bool GameObject::CompareTag(const char* _tag)
{
	return strcmp(tag, _tag) == 0;
}

GameObject* GameObject::GetChild(std::string& childName)
{
	for (size_t i = 0; i < children.size(); i++)
	{
		if (children[i]->name == childName)
			return children[i];

		GameObject* child = children[i]->GetChild(childName);
		
		if (child != nullptr)
			return child;
	}

	return nullptr;
}
