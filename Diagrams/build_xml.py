#!/usr/bin/env python3
# Script to complete the EchoRift XMI file
import os

tail = """
			<!-- SceneManagement Package -->
			<uml:Package xmi.id="PKG_Scene" name="SceneManagement" visibility="public" namespace="MX_MODEL_1">
				<uml:Namespace.ownedElement>
					<uml:Class xmi.id="CLS_GlobalLoader" name="GlobalLoader" visibility="public" namespace="PKG_Scene">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="GL_instance" name="Instance" visibility="public" ownerScope="classifier" type="GlobalLoader"/>
							<uml:Attribute xmi.id="GL_player" name="playerInstance" visibility="public" type="Player"/>
							<uml:Attribute xmi.id="GL_mainUI" name="mainUI" visibility="public" type="MainUI"/>
							<uml:Attribute xmi.id="GL_gameSettings" name="gameSettings" visibility="private" type="GameSettings"/>
							<uml:Attribute xmi.id="GL_sceneLoader" name="fightSceneLoader" visibility="public" type="SceneLoader"/>
							<uml:Operation xmi.id="GL_SavePlayer" name="SavePlayer" visibility="public"/>
							<uml:Operation xmi.id="GL_LoadPlayer" name="LoadPlayerData" visibility="public"/>
							<uml:Operation xmi.id="GL_SaveGlobal" name="SaveGlobal" visibility="public"/>
							<uml:Operation xmi.id="GL_LoadGlobal" name="LoadGlobal" visibility="public"/>
							<uml:Operation xmi.id="GL_LoadToScene" name="LoadToScene" visibility="public"/>
							<uml:Operation xmi.id="GL_Show" name="Show" visibility="public"/>
							<uml:Operation xmi.id="GL_Hide" name="Hide" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_SceneLoader" name="SceneLoader" visibility="public" namespace="PKG_Scene">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="SL_GlobalSpeed" name="GlobalLoadingSpeed" visibility="public" ownerScope="classifier" type="float"/>
							<uml:Operation xmi.id="SL_Load" name="LoadScene" visibility="public"/>
							<uml:Operation xmi.id="SL_LoadAsync" name="LoadSceneAsync" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_ScreenFader" name="ScreenFader" visibility="public" namespace="PKG_Scene">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="SF_fadeDuration" name="fadeDuration" visibility="private" type="float"/>
							<uml:Operation xmi.id="SF_FadeIn" name="FadeInAsync" visibility="public"/>
							<uml:Operation xmi.id="SF_FadeOut" name="FadeOutAsync" visibility="public"/>
							<uml:Operation xmi.id="SF_SetAlpha" name="SetAlpha" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_SceneLoaderTrigger" name="SceneLoaderTrigger" visibility="public" namespace="PKG_Scene">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="SLT_OnTrigger" name="OnTriggerEnter2D" visibility="private"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_TimeManager" name="TimeManager" visibility="public" namespace="PKG_Scene">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="TM_Pause" name="Pause" visibility="public"/>
							<uml:Operation xmi.id="TM_Resume" name="Resume" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_GameSettings" name="GameSettings" visibility="public" namespace="PKG_Scene">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="GS_loadSpeed" name="loadingSceneSpeed" visibility="public" type="float"/>
							<uml:Attribute xmi.id="GS_enemyDelay" name="enemyTurnDelay" visibility="public" type="float"/>
							<uml:Attribute xmi.id="GS_enemySpeed" name="enemyTurnSpeed" visibility="public" type="float"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_MainUI" name="MainUI" visibility="public" namespace="PKG_Scene">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="MUI_invMgr" name="inventoryManager" visibility="public" type="InventoryManager"/>
							<uml:Attribute xmi.id="MUI_canOpenUI" name="canOpenUI" visibility="public" type="bool"/>
							<uml:Operation xmi.id="MUI_Show" name="Show" visibility="public"/>
							<uml:Operation xmi.id="MUI_Hide" name="Hide" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Dependency xmi.id="DEP_GL_SLS" name="uses" client="CLS_GlobalLoader" supplier="CLS_SaveLoadSystem"/>
					<uml:Dependency xmi.id="DEP_SLT_GL" name="uses" client="CLS_SceneLoaderTrigger" supplier="CLS_GlobalLoader"/>
				</uml:Namespace.ownedElement>
			</uml:Package>

			<!-- SaveLoadSystem Package -->
			<uml:Package xmi.id="PKG_Save" name="SaveLoadSystem" visibility="public" namespace="MX_MODEL_1">
				<uml:Namespace.ownedElement>
					<uml:Class xmi.id="CLS_SaveLoadSystem" name="SaveLoadSystem" visibility="public" namespace="PKG_Save">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="SLS_GetPath" name="GetPath" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="SLS_Save" name="Save" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="SLS_Load" name="Load" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="SLS_Exists" name="Exists" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="SLS_Delete" name="Delete" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="SLS_ClearAll" name="ClearAllSaves" visibility="public" ownerScope="classifier"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_PersistentObject" name="PersistentObject" visibility="public" namespace="PKG_Save">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="PO_persistentId" name="persistentId" visibility="private" type="String"/>
							<uml:Operation xmi.id="PO_Save" name="Save" visibility="public"/>
							<uml:Operation xmi.id="PO_Load" name="Load" visibility="public"/>
							<uml:Operation xmi.id="PO_SaveAll" name="SaveAll" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="PO_LoadAll" name="LoadAll" visibility="public" ownerScope="classifier"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_SceneObjectSaver" name="SceneObjectSaver" visibility="public" namespace="PKG_Save">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="SOS_dialogueVar" name="dialogueVariable" visibility="private" type="String"/>
							<uml:Attribute xmi.id="SOS_hideWhenTrue" name="hideWhenTrue" visibility="private" type="bool"/>
							<uml:Attribute xmi.id="SOS_disableGO" name="disableGameObject" visibility="private" type="bool"/>
							<uml:Operation xmi.id="SOS_ApplyState" name="ApplyState" visibility="public"/>
							<uml:Operation xmi.id="SOS_SetAndApply" name="SetVariableAndApply" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_SaveFileNames" name="SaveFileNames" visibility="public" namespace="PKG_Save">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="SFN_PlayerData" name="PLAYER_DATA" visibility="public" ownerScope="classifier" type="String"/>
							<uml:Attribute xmi.id="SFN_TeamData" name="TEAM_DATA" visibility="public" ownerScope="classifier" type="String"/>
							<uml:Attribute xmi.id="SFN_DialogState" name="DIALOGUE_STATE" visibility="public" ownerScope="classifier" type="String"/>
							<uml:Attribute xmi.id="SFN_GlobalSave" name="GLOBAL_SAVE" visibility="public" ownerScope="classifier" type="String"/>
							<uml:Attribute xmi.id="SFN_GameDir" name="GAME_DIRECTORY" visibility="public" ownerScope="classifier" type="String"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Dependency xmi.id="DEP_PO_SLS" name="uses" client="CLS_PersistentObject" supplier="CLS_SaveLoadSystem"/>
					<uml:Dependency xmi.id="DEP_SOS_SLS" name="uses" client="CLS_SceneObjectSaver" supplier="CLS_SaveLoadSystem"/>
				</uml:Namespace.ownedElement>
			</uml:Package>

			<!-- Audio / Notification Package -->
			<uml:Package xmi.id="PKG_Audio" name="Audio_Notification" visibility="public" namespace="MX_MODEL_1">
				<uml:Namespace.ownedElement>
					<uml:Interface xmi.id="IF_IAudioManager" name="IAudioManager" visibility="public" namespace="PKG_Audio" isAbstract="true">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="IAM_Play" name="Play" visibility="public"/>
							<uml:Operation xmi.id="IAM_AddSound" name="AddSoundFromPath" visibility="public"/>
							<uml:Operation xmi.id="IAM_TryGetSource" name="TryGetSource" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Interface>
					<uml:Interface xmi.id="IF_IAudioLogger" name="IAudioLogger" visibility="public" namespace="PKG_Audio" isAbstract="true">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="IAL_Log" name="Log" visibility="public"/>
							<uml:Operation xmi.id="IAL_LogFormat" name="LogFormat" visibility="public"/>
							<uml:Operation xmi.id="IAL_LogException" name="LogException" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Interface>
					<uml:Class xmi.id="CLS_UIAudioLogger" name="UIAudioLogger" visibility="public" namespace="PKG_Audio">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="UAL_logOutput" name="m_logOutput" visibility="private" type="Text"/>
							<uml:Attribute xmi.id="UAL_logLevel" name="m_logLevel" visibility="private" type="LoggingLevel"/>
							<uml:Operation xmi.id="UAL_Log" name="Log" visibility="public"/>
							<uml:Operation xmi.id="UAL_LogFormat" name="LogFormat" visibility="public"/>
							<uml:Operation xmi.id="UAL_CanLog" name="CanLog" visibility="private"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Abstraction xmi.id="REAL_UAL_IAL" name="" client="CLS_UIAudioLogger" supplier="IF_IAudioLogger"/>
					<uml:Class xmi.id="CLS_UIAudioAutoInstaller" name="UIAudioAutoInstaller" visibility="public" namespace="PKG_Audio">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="UAAI_clickSound" name="clickSoundName" visibility="private" type="String"/>
							<uml:Operation xmi.id="UAAI_InitDelay" name="InitializeWithDelay" visibility="private"/>
							<uml:Operation xmi.id="UAAI_PlaySound" name="PlaySound" visibility="private"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_AreaAmbientSound" name="AreaAmbientSound" visibility="public" namespace="PKG_Audio">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="AAS_soundName" name="soundName" visibility="private" type="String"/>
							<uml:Operation xmi.id="AAS_OnEnter" name="OnTriggerEnter2D" visibility="private"/>
							<uml:Operation xmi.id="AAS_OnExit" name="OnTriggerExit2D" visibility="private"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Enumeration xmi.id="ENUM_LoggingLevel" name="LoggingLevel" visibility="public" namespace="PKG_Audio">
						<uml:EnumerationLiteral xmi.id="LL_None" name="NONE"/>
						<uml:EnumerationLiteral xmi.id="LL_Error" name="ERROR"/>
						<uml:EnumerationLiteral xmi.id="LL_Warning" name="WARNING"/>
						<uml:EnumerationLiteral xmi.id="LL_Info" name="INFO"/>
					</uml:Enumeration>
					<uml:Dependency xmi.id="DEP_UAAI_IAM" name="uses" client="CLS_UIAudioAutoInstaller" supplier="IF_IAudioManager"/>
					<uml:Dependency xmi.id="DEP_AAS_IAM" name="uses" client="CLS_AreaAmbientSound" supplier="IF_IAudioManager"/>
				</uml:Namespace.ownedElement>
			</uml:Package>

			<!-- Debug / Editor Package -->
			<uml:Package xmi.id="PKG_Debug" name="Debug_Editor" visibility="public" namespace="MX_MODEL_1">
				<uml:Namespace.ownedElement>
					<uml:Class xmi.id="CLS_DebugCommands" name="DebugCommands" visibility="public" namespace="PKG_Debug">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="DC_SpawnConsole" name="SpawnConsole" visibility="private" ownerScope="classifier"/>
							<uml:Operation xmi.id="DC_RegisterCmds" name="RegisterCommands" visibility="private" ownerScope="classifier"/>
							<uml:Operation xmi.id="DC_LoadScene" name="LoadScene" visibility="private" ownerScope="classifier"/>
							<uml:Operation xmi.id="DC_GiveItem" name="GiveItem" visibility="private" ownerScope="classifier"/>
							<uml:Operation xmi.id="DC_AddCoins" name="AddCoins" visibility="private" ownerScope="classifier"/>
							<uml:Operation xmi.id="DC_SetLevel" name="SetLevel" visibility="private" ownerScope="classifier"/>
							<uml:Operation xmi.id="DC_SaveGame" name="SaveGame" visibility="private" ownerScope="classifier"/>
							<uml:Operation xmi.id="DC_LoadGame" name="LoadGame" visibility="private" ownerScope="classifier"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_GameSettingsEditor" name="GameSettingsEditor" visibility="public" namespace="PKG_Debug">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="GSE_OnInspectorGUI" name="OnInspectorGUI" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Interface xmi.id="IF_IColliderDebug" name="IColliderDebugDrawable2D" visibility="public" namespace="PKG_Debug" isAbstract="true">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="ICD_GetCollider" name="GetCollider2D" visibility="public"/>
							<uml:Operation xmi.id="ICD_ShouldDraw" name="ShouldDrawGizmos" visibility="public"/>
							<uml:Operation xmi.id="ICD_OnDraw" name="OnDrawColliderGizmos2D" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Interface>
					<uml:Dependency xmi.id="DEP_DC_GL" name="uses" client="CLS_DebugCommands" supplier="CLS_GlobalLoader"/>
					<uml:Dependency xmi.id="DEP_GSE_GS" name="edits" client="CLS_GameSettingsEditor" supplier="CLS_GameSettings"/>
				</uml:Namespace.ownedElement>
			</uml:Package>

			<!-- NPC_Dialogue Package -->
			<uml:Package xmi.id="PKG_NPC" name="NPC_Dialogue" visibility="public" namespace="MX_MODEL_1">
				<uml:Namespace.ownedElement>
					<uml:Interface xmi.id="IF_ITalkable" name="ITalkable" visibility="public" namespace="PKG_NPC" isAbstract="true">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="IT_Talk" name="Talk" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Interface>
					<uml:Class xmi.id="CLS_NPC" name="NPC" visibility="public" namespace="PKG_NPC">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="NPC_isTalkable" name="isTalkable" visibility="private" type="bool"/>
							<uml:Attribute xmi.id="NPC_DSTrigger" name="DSTrigger" visibility="private" type="DialogueSystemTrigger"/>
							<uml:Operation xmi.id="NPC_Talk" name="Talk" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Abstraction xmi.id="REAL_NPC_ITalkable" name="" client="CLS_NPC" supplier="IF_ITalkable"/>
					<uml:Class xmi.id="CLS_LuaFunctions" name="LuaFunctions" visibility="public" namespace="PKG_NPC">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="LF_invMgr" name="inventoryManager" visibility="private" type="InventoryManager"/>
							<uml:Operation xmi.id="LF_HasItem" name="HasItem" visibility="public"/>
							<uml:Operation xmi.id="LF_AddItem" name="AddItem" visibility="public"/>
							<uml:Operation xmi.id="LF_RemoveItem" name="RemoveItem" visibility="public"/>
							<uml:Operation xmi.id="LF_HasCoins" name="HasCoins" visibility="public"/>
							<uml:Operation xmi.id="LF_AddCoins" name="AddCoins" visibility="public"/>
							<uml:Operation xmi.id="LF_GetStat" name="GetStat" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_DialogueSaveMgr" name="DialogueSaveManager" visibility="public" namespace="PKG_NPC">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="DSM_Save" name="Save" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="DSM_Load" name="Load" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="DSM_Delete" name="Delete" visibility="public" ownerScope="classifier"/>
							<uml:Operation xmi.id="DSM_Exists" name="Exists" visibility="public" ownerScope="classifier"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_SequencerCmd" name="SequencerCommand" visibility="public" namespace="PKG_NPC" isAbstract="true">
						<uml:Classifier.feature>
							<uml:Operation xmi.id="SC_OnStart" name="OnStart" visibility="public" isAbstract="true"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_SC_GiveItem" name="SequencerCommandGiveItem" visibility="public" namespace="PKG_NPC"/>
					<uml:Generalization xmi.id="GEN_SC_GiveItem" child="CLS_SC_GiveItem" parent="CLS_SequencerCmd"/>
					<uml:Class xmi.id="CLS_SC_PlaySound" name="SequencerCommandPlaySound" visibility="public" namespace="PKG_NPC"/>
					<uml:Generalization xmi.id="GEN_SC_PlaySound" child="CLS_SC_PlaySound" parent="CLS_SequencerCmd"/>
					<uml:Class xmi.id="CLS_SC_RecruitComp" name="SequencerCommandRecruitCompanion" visibility="public" namespace="PKG_NPC"/>
					<uml:Generalization xmi.id="GEN_SC_RecruitComp" child="CLS_SC_RecruitComp" parent="CLS_SequencerCmd"/>
					<uml:Class xmi.id="CLS_SC_Shop" name="SequencerCommandShop" visibility="public" namespace="PKG_NPC"/>
					<uml:Generalization xmi.id="GEN_SC_Shop" child="CLS_SC_Shop" parent="CLS_SequencerCmd"/>
					<uml:Class xmi.id="CLS_InventoryManager" name="InventoryManager" visibility="public" namespace="PKG_NPC">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="IM_Wallet" name="Wallet" visibility="public" type="PlayerWallet"/>
							<uml:Operation xmi.id="IM_HasItem" name="HasItem" visibility="public"/>
							<uml:Operation xmi.id="IM_AddItem" name="AddItem" visibility="public"/>
							<uml:Operation xmi.id="IM_SyncFromUI" name="SyncFromUI" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Class xmi.id="CLS_PlayerWallet" name="PlayerWallet" visibility="public" namespace="PKG_NPC">
						<uml:Classifier.feature>
							<uml:Attribute xmi.id="PW_coins" name="coins" visibility="public" type="int"/>
							<uml:Operation xmi.id="PW_AddCoins" name="AddCoins" visibility="public"/>
						</uml:Classifier.feature>
					</uml:Class>
					<uml:Dependency xmi.id="DEP_LF_IM" name="uses" client="CLS_LuaFunctions" supplier="CLS_InventoryManager"/>
					<uml:Dependency xmi.id="DEP_DSM_SLS" name="uses" client="CLS_DialogueSaveMgr" supplier="CLS_SaveLoadSystem"/>
				</uml:Namespace.ownedElement>
			</uml:Package>

		</uml:Namespace.ownedElement>
	</uml:Model>
</xmi:XMI>
"""

# Read current file
with open('EchoRift_ClassDiagram.xml', 'r', encoding='utf-8') as f:
    content = f.read()

# Append closing content
with open('EchoRift_ClassDiagram.xml', 'a', encoding='utf-8') as f:
    f.write(tail)

print("Done! File size:", os.path.getsize('EchoRift_ClassDiagram.xml'), "bytes")
