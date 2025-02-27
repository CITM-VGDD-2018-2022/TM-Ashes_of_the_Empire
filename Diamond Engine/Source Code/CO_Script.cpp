#include "CO_Script.h"
#include "ImGui/imgui.h"

#include "MO_Editor.h"
#include"MO_Scene.h"

#include "GameObject.h"
#include "Application.h"
#include "DETime.h"
#include "CO_Transform.h"

#include"DEJsonSupport.h"
#include <mono/metadata/class.h>
#include <mono/metadata/object.h>
#include <mono/metadata/debug-helpers.h>

C_Script* C_Script::runningScript = nullptr;
C_Script::C_Script(GameObject* _gm, const char* scriptName) : Component(_gm), noGCobject(0), updateMethod(nullptr), onCollisionEnter(nullptr), onTriggerEnter(nullptr), onApplicationQuit(nullptr)
, onExecuteButton(nullptr), onExecuteCheckbox(nullptr), onAwake(nullptr), onStart(nullptr), onDestroyMethod(nullptr)
{
	name = scriptName;
	//strcpy(name, scriptName);

	EngineExternal->moduleScene->activeScriptsVector.push_back(this);

	//EngineExternal->moduleMono->DebugAllMethods(DE_SCRIPTS_NAMESPACE, "GameObject", methods);
	LoadScriptData(scriptName);
}

C_Script::~C_Script()
{
	if (C_Script::runningScript == this)
		C_Script::runningScript = nullptr;

	if ((DETime::state == GameState::PLAY || DETime::state == GameState::STEP) && onDestroyMethod != nullptr)
	{
		OnDestroy();
	}

	mono_gchandle_free(noGCobject);

	for (unsigned int i = 0; i < fields.size(); i++)
	{
		if (fields[i].type == MonoTypeEnum::MONO_TYPE_CLASS && fields[i].fiValue.goValue != nullptr && fields[i].fiValue.goValue->csReferences.size() != 0)
		{
			//fields[i].fiValue.goValue->csReferences.erase(std::find(fields[i].fiValue.goValue->csReferences.begin(), fields[i].fiValue.goValue->csReferences.end(), &fields[i]));
			std::vector<SerializedField*>::iterator ptr = std::find(fields[i].fiValue.goValue->csReferences.begin(), fields[i].fiValue.goValue->csReferences.end(), &fields[i]);
			if (ptr != fields[i].fiValue.goValue->csReferences.end())
				fields[i].fiValue.goValue->csReferences.erase(ptr);
			
		}
	}
	EngineExternal->moduleScene->activeScriptsVector.erase(std::find(EngineExternal->moduleScene->activeScriptsVector.begin(), EngineExternal->moduleScene->activeScriptsVector.end(), this));
	methods.clear();
	fields.clear();
	name.clear();
}

void C_Script::Update()
{
	if (DETime::state == GameState::STOP || DETime::state == GameState::PAUSE || updateMethod == nullptr)
		return;

	C_Script::runningScript = this; // I really think this is the peak of stupid code, but hey, it works, slow as hell but works.

	MonoObject* exec = nullptr;
	mono_runtime_invoke(updateMethod, mono_gchandle_get_target(noGCobject), NULL, &exec);

	if (exec != nullptr)
	{
		if (strcmp(mono_class_get_name(mono_object_get_class(exec)), "NullReferenceException") == 0)
		{
			LOG(LogType::L_ERROR, "Null reference exception detected");
		}
		else
		{
			LOG(LogType::L_ERROR, "Something went wrong with %s", mono_class_get_name(mono_object_get_class(exec)));
		}
	}
}

#ifndef STANDALONE
bool C_Script::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		//ImGui::Separator();

		for (int i = 0; i < fields.size(); i++)
		{
			DropField(fields[i], "_GAMEOBJECT");
		}
		ImGui::Separator();
		for (int i = 0; i < methods.size(); i++)
		{
			ImGui::Text(methods[i].c_str());
		}

		return true;
	}
	return false;
}

void C_Script::DropField(SerializedField& field, const char* dropType)
{
	const char* fieldName = mono_field_get_name(field.field);
	ImGui::PushID(fieldName);

	ImGui::Text(fieldName);
	ImGui::SameLine();

	switch (field.type)
	{
	case MonoTypeEnum::MONO_TYPE_BOOLEAN:
		mono_field_get_value(mono_gchandle_get_target(noGCobject), field.field, &field.fiValue.bValue);
		if (ImGui::Checkbox(field.displayName.c_str(), &field.fiValue.bValue))
			mono_field_set_value(mono_gchandle_get_target(noGCobject), field.field, &field.fiValue.bValue);
		break;

	case MonoTypeEnum::MONO_TYPE_I4:
		mono_field_get_value(mono_gchandle_get_target(noGCobject), field.field, &field.fiValue.iValue);
		if (ImGui::InputInt(field.displayName.c_str(), &field.fiValue.iValue, 1, 10))
			mono_field_set_value(mono_gchandle_get_target(noGCobject), field.field, &field.fiValue.iValue);
		break;

	case MonoTypeEnum::MONO_TYPE_CLASS:

		if (strcmp(mono_type_get_name(mono_field_get_type(field.field)), "DiamondEngine.GameObject") != 0)
		{
			ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "The class %s can't be serialized yet", mono_type_get_name(mono_field_get_type(field.field)));
			break;
		}

		ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), (field.fiValue.goValue != nullptr) ? field.fiValue.goValue->name.c_str() : "None");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload(dropType))
			{
				if (field.fiValue.goValue != nullptr)
					field.fiValue.goValue->RemoveCSReference(&field);

				GameObject* draggedObject = EngineExternal->moduleEditor->GetDraggingGO();
				field.fiValue.goValue = draggedObject;
				field.goUID = draggedObject->UID;

				SetField(field.field, field.fiValue.goValue);
			}
			ImGui::EndDragDropTarget();
		}

		//ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "UID: %d", field.goUID);

		break;

	case MonoTypeEnum::MONO_TYPE_R4: {
		//float test = 0.f;
		mono_field_get_value(mono_gchandle_get_target(noGCobject), field.field, &field.fiValue.fValue);
		if (ImGui::InputFloat(field.displayName.c_str(), &field.fiValue.fValue, 0.1f))
			mono_field_set_value(mono_gchandle_get_target(noGCobject), field.field, &field.fiValue.fValue);
		break;
	}

	case MonoTypeEnum::MONO_TYPE_STRING:
	{

		MonoString* str = nullptr;
		mono_field_get_value(mono_gchandle_get_target(noGCobject), field.field, &str);

		char* value = mono_string_to_utf8(str);
		strcpy(field.fiValue.strValue, value);
		mono_free(value);

		if (ImGui::InputText(field.displayName.c_str(), &field.fiValue.strValue[0], 50))
		{
			str = mono_string_new(EngineExternal->moduleMono->domain, field.fiValue.strValue);
			mono_field_set_value(mono_gchandle_get_target(noGCobject), field.field, str);
			//mono_free(str);
		}
		break;
	}

	default:
		ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), mono_type_get_name(mono_field_get_type(field.field)));
		break;
	}

	ImGui::PopID();
}
#endif

void C_Script::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);
	DEJson::WriteString(nObj, "ScriptName", name.c_str());

	for (int i = 0; i < fields.size(); i++)
	{
		switch (fields[i].type)
		{
		case MonoTypeEnum::MONO_TYPE_BOOLEAN:
			mono_field_get_value(mono_gchandle_get_target(noGCobject), fields[i].field, &fields[i].fiValue.bValue);
			DEJson::WriteBool(nObj, mono_field_get_name(fields[i].field), fields[i].fiValue.bValue);
			break;

		case MonoTypeEnum::MONO_TYPE_I4:
			mono_field_get_value(mono_gchandle_get_target(noGCobject), fields[i].field, &fields[i].fiValue.iValue);
			DEJson::WriteInt(nObj, mono_field_get_name(fields[i].field), fields[i].fiValue.iValue);
			break;

		case MonoTypeEnum::MONO_TYPE_CLASS:
			if (fields[i].fiValue.goValue != nullptr)
			{
				if (fields[i].fiValue.goValue->prefabReference != 0u && gameObject->prefabReference != 0u) {
					DEJson::WriteInt(nObj, mono_field_get_name(fields[i].field), fields[i].fiValue.goValue->prefabReference);}
				else {
					DEJson::WriteInt(nObj, mono_field_get_name(fields[i].field), fields[i].fiValue.goValue->UID); }
			}
			break;

		case MonoTypeEnum::MONO_TYPE_R4:
			mono_field_get_value(mono_gchandle_get_target(noGCobject), fields[i].field, &fields[i].fiValue.fValue);
			DEJson::WriteFloat(nObj, mono_field_get_name(fields[i].field), fields[i].fiValue.fValue);
			break;

		case MonoTypeEnum::MONO_TYPE_STRING:
			DEJson::WriteString(nObj, mono_field_get_name(fields[i].field), fields[i].fiValue.strValue);
			break;

		default:
			DEJson::WriteInt(nObj, mono_field_get_name(fields[i].field), fields[i].fiValue.iValue);
			break;
		}
	}
}

void C_Script::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	SerializedField* _field = nullptr;
	for (int i = 0; i < fields.size(); i++) //TODO IMPORTANT ASK: There must be a better way to do this... too much use of switches with this stuff, look at MONOMANAGER
	{
		_field = &fields[i];

		//if (_field->displayName == "##pointer")
			//continue;

		switch (_field->type)
		{
		case MonoTypeEnum::MONO_TYPE_BOOLEAN:
			_field->fiValue.bValue = nObj.ReadBool(mono_field_get_name(_field->field));
			mono_field_set_value(mono_gchandle_get_target(noGCobject), _field->field, &_field->fiValue.bValue);
			break;

		case MonoTypeEnum::MONO_TYPE_I4:
			_field->fiValue.iValue = nObj.ReadInt(mono_field_get_name(_field->field));
			mono_field_set_value(mono_gchandle_get_target(noGCobject), _field->field, &_field->fiValue.iValue);
			break;

		case MonoTypeEnum::MONO_TYPE_CLASS:
		{
			if (strcmp(mono_type_get_name(mono_field_get_type(_field->field)), "DiamondEngine.GameObject") == 0)
			{
				int uid = nObj.ReadInt(mono_field_get_name(_field->field));
				_field->goUID = uid;

				EngineExternal->moduleScene->AddToReferenceMap(uid, _field);
			}

			break;
		}
		case MonoTypeEnum::MONO_TYPE_R4:
			_field->fiValue.fValue = nObj.ReadFloat(mono_field_get_name(_field->field));
			mono_field_set_value(mono_gchandle_get_target(noGCobject), _field->field, &_field->fiValue.fValue);
			break;

		case MonoTypeEnum::MONO_TYPE_STRING:
		{
			const char* ret = nObj.ReadString(mono_field_get_name(_field->field));

			if (ret == NULL)
				ret = "\0";

			strcpy(&_field->fiValue.strValue[0], ret);

			MonoString* str = mono_string_new(EngineExternal->moduleMono->domain, _field->fiValue.strValue);
			mono_field_set_value(mono_gchandle_get_target(noGCobject), _field->field, str);
			break;
		}

		default:
			_field->fiValue.iValue = nObj.ReadInt(mono_field_get_name(_field->field));
			mono_field_set_value(mono_gchandle_get_target(noGCobject), _field->field, &_field->fiValue.iValue);
			break;
		}
	}
}

void C_Script::OnRecursiveUIDChange(std::map<uint, GameObject*> gameObjects)
{
	for (size_t i = 0; i < fields.size(); i++)
	{
		if (fields[i].type == MonoTypeEnum::MONO_TYPE_CLASS && strcmp(mono_type_get_name(mono_field_get_type(fields[i].field)), "DiamondEngine.GameObject") == 0)
		{
			std::map<uint, GameObject*>::iterator gameObjectIt = gameObjects.find(fields[i].goUID);

			if (gameObjectIt != gameObjects.end())
			{
				if(EngineExternal->moduleScene->referenceMap.size() > 0)
					EngineExternal->moduleScene->referenceMap.erase(gameObjectIt->first);

				EngineExternal->moduleScene->AddToReferenceMap((uint)gameObjectIt->second->UID, &fields[i]);

				fields[i].fiValue.goValue = gameObjectIt->second;
				fields[i].goUID = (uint)gameObjectIt->second->UID;
			}
		}
	}
}

void C_Script::LoadScriptData(const char* scriptName)
{
	methods.clear();
	fields.clear();

	MonoClass* klass = mono_class_from_name(EngineExternal->moduleMono->image, USER_SCRIPTS_NAMESPACE, scriptName);

	if (klass == nullptr)
	{
		LOG(LogType::L_ERROR, "Script %s was deleted and can't be loaded", scriptName);
		name = "Missing script reference";
		return;
	}

	EngineExternal->moduleMono->DebugAllMethods(USER_SCRIPTS_NAMESPACE, scriptName, methods);

	noGCobject = mono_gchandle_new(mono_object_new(EngineExternal->moduleMono->domain, klass), false);
	mono_runtime_object_init(mono_gchandle_get_target(noGCobject));


	MonoClass* goClass = mono_object_get_class(mono_gchandle_get_target(noGCobject));
	uintptr_t ptr = reinterpret_cast<uintptr_t>(this);
	mono_field_set_value(mono_gchandle_get_target(noGCobject), mono_class_get_field_from_name(goClass, "pointer"), &ptr);

	MonoMethodDesc* mdesc = mono_method_desc_new(":Update", false);
	updateMethod = mono_method_desc_search_in_class(mdesc, klass);
	mono_method_desc_free(mdesc);

	MonoMethodDesc* oncDesc = mono_method_desc_new(":OnCollisionEnter", false);
	onCollisionEnter = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);

#pragma region InitCSMethods
	oncDesc = mono_method_desc_new(":Awake", false);
	onAwake = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);

	oncDesc = mono_method_desc_new(":Start", false);
	onStart = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);


#pragma endregion

#pragma region DestroyCSMethods

	oncDesc = mono_method_desc_new(":OnDestroy", false);
	onDestroyMethod = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);

#pragma endregion

	oncDesc = mono_method_desc_new(":OnTriggerEnter", false);
	onTriggerEnter = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);

	oncDesc = mono_method_desc_new(":OnCollisionStay", false);
	onCollisionStay = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);

	oncDesc = mono_method_desc_new(":OnCollisionExit", false);
	onCollisionExit = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);

	oncDesc = mono_method_desc_new(":OnTriggerExit", false);
	onTriggerExit = mono_method_desc_search_in_class(oncDesc, klass);
	mono_method_desc_free(oncDesc);

	MonoMethodDesc* onaQuit = mono_method_desc_new(":OnApplicationQuit", false);
	onApplicationQuit = mono_method_desc_search_in_class(onaQuit, klass);
	mono_method_desc_free(onaQuit);

	MonoMethodDesc* oncBut = mono_method_desc_new(":OnExecuteButton", false);
	onExecuteButton = mono_method_desc_search_in_class(oncBut, klass);
	mono_method_desc_free(oncBut);

	MonoMethodDesc* oncChck = mono_method_desc_new(":OnExecuteCheckbox", false);
	onExecuteCheckbox = mono_method_desc_search_in_class(oncChck, klass);
	mono_method_desc_free(oncChck);

	MonoClass* baseClass = mono_class_get_parent(klass);
	if (baseClass != nullptr)
		EngineExternal->moduleMono->DebugAllFields(mono_class_get_name(baseClass), fields, mono_gchandle_get_target(noGCobject), this, mono_class_get_namespace(baseClass));

	EngineExternal->moduleMono->DebugAllFields(scriptName, fields, mono_gchandle_get_target(noGCobject), this, mono_class_get_namespace(goClass));
}

void C_Script::CollisionCallback(bool isTrigger, GameObject* collidedGameObject)
{
	void* params[1];

	if (collidedGameObject != nullptr)
	{
		params[0] = EngineExternal->moduleMono->GoToCSGO(collidedGameObject);

		if (onCollisionEnter != nullptr)
			mono_runtime_invoke(onCollisionEnter, mono_gchandle_get_target(noGCobject), params, NULL);

		if (isTrigger)
		{
			if (onTriggerEnter != nullptr)
				mono_runtime_invoke(onTriggerEnter, mono_gchandle_get_target(noGCobject), params, NULL);
		}
	}
}

void C_Script::CollisionPersistCallback(GameObject* collidedGameObject)
{
	void* params[1];
	//LOG(LogType::L_WARNING, "Collided object: %s, Collider object: %s", gameObject->tag, collidedGameObject->tag);

	params[0] = EngineExternal->moduleMono->GoToCSGO(collidedGameObject);

	if (onCollisionStay != nullptr)
		mono_runtime_invoke(onCollisionStay, mono_gchandle_get_target(noGCobject), params, NULL);

}

void C_Script::CollisionExitCallback(bool isTrigger, GameObject* collidedGameObject)
{
	void* params[1];

	params[0] = EngineExternal->moduleMono->GoToCSGO(collidedGameObject);

	if (onCollisionExit != nullptr)
		mono_runtime_invoke(onCollisionExit, mono_gchandle_get_target(noGCobject), params, NULL);

	if (isTrigger)
	{
		if (onTriggerExit != nullptr)
			mono_runtime_invoke(onTriggerExit, mono_gchandle_get_target(noGCobject), params, NULL);
	}

}
void C_Script::ExecuteButton()
{
	if (onExecuteButton != nullptr)
		mono_runtime_invoke(onExecuteButton, mono_gchandle_get_target(noGCobject), NULL, NULL);
}

void C_Script::OnApplicationQuit()
{
	if (onApplicationQuit != nullptr)
		mono_runtime_invoke(onApplicationQuit, mono_gchandle_get_target(noGCobject), NULL, NULL);
}

void C_Script::ExecuteCheckbox(bool checkbox_active)
{
	if (onExecuteCheckbox == nullptr)
		return;

	void* args[1];
	args[0] = &checkbox_active;

	mono_runtime_invoke(onExecuteCheckbox, mono_gchandle_get_target(noGCobject), args, NULL);
}

void C_Script::OnStart()
{
	if (onStart != nullptr)
		mono_runtime_invoke(onStart, mono_gchandle_get_target(noGCobject), NULL, NULL);
}

void C_Script::OnAwake()
{
	if (onAwake != nullptr)
		mono_runtime_invoke(onAwake, mono_gchandle_get_target(noGCobject), NULL, NULL);
}

void C_Script::OnDestroy()
{
	if (onDestroyMethod != nullptr)
		mono_runtime_invoke(onDestroyMethod, mono_gchandle_get_target(noGCobject), NULL, NULL);
}

void C_Script::SetField(MonoClassField* field, GameObject* value)
{
	mono_field_set_value(mono_gchandle_get_target(noGCobject), field, EngineExternal->moduleMono->GoToCSGO(value));
}
