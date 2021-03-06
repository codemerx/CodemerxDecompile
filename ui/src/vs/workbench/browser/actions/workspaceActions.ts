/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Action } from 'vs/base/common/actions';
import * as nls from 'vs/nls';
import { ITelemetryData } from 'vs/platform/telemetry/common/telemetry';
import { IWorkspaceContextService, WorkbenchState, IWorkspaceFolder } from 'vs/platform/workspace/common/workspace';
import { IWorkspaceEditingService } from 'vs/workbench/services/workspaces/common/workspaceEditing';
/* AGPL */
import { IEditorService } from 'vs/workbench/services/editor/common/editorService';
import { ICommandService } from 'vs/platform/commands/common/commands';
/* End AGPL */
import { ADD_ROOT_FOLDER_COMMAND_ID, ADD_ROOT_FOLDER_LABEL, PICK_WORKSPACE_FOLDER_COMMAND_ID } from 'vs/workbench/browser/actions/workspaceCommands';
import { IFileDialogService } from 'vs/platform/dialogs/common/dialogs';
import { IHostService } from 'vs/workbench/services/host/browser/host';
import { IWorkbenchEnvironmentService } from 'vs/workbench/services/environment/common/environmentService';
import { IWorkspacesService, hasWorkspaceFileExtension } from 'vs/platform/workspaces/common/workspaces';
/* AGPL */
import { MenuRegistry, MenuId, SyncActionDescriptor } from 'vs/platform/actions/common/actions';
import { WorkspaceFolderCountContext, WorkbenchStateContext } from 'vs/workbench/browser/contextkeys';
import { Registry } from 'vs/platform/registry/common/platform';
import { IWorkbenchActionRegistry, Extensions } from 'vs/workbench/common/actions';
import { IProgressService, ProgressLocation } from 'vs/platform/progress/common/progress';
import { IDecompilationService } from 'vs/cd/workbench/DecompilationService';
import { IFileService } from 'vs/platform/files/common/files';
import { INotificationService } from 'vs/platform/notification/common/notification';
import { URI } from 'vs/base/common/uri';
import { IAnalyticsService } from 'vs/cd/workbench/AnalyticsService';
/* End AGPL */

export class OpenFileAction extends Action {

	static readonly ID = 'workbench.action.files.openFile';
	/* AGPL */
	static readonly LABEL = nls.localize('openFile', "Open Assembly...");
	/* End AGPL */

	constructor(
		id: string,
		label: string,
		@IFileDialogService private readonly dialogService: IFileDialogService
	) {
		super(id, label);
	}

	run(event?: unknown, data?: ITelemetryData): Promise<void> {
		return this.dialogService.pickFileAndOpen({ forceNewWindow: false, telemetryExtraData: data });
	}
}

export class OpenFolderAction extends Action {

	static readonly ID = 'workbench.action.files.openFolder';
	static readonly LABEL = nls.localize('openFolder', "Open Folder...");

	constructor(
		id: string,
		label: string,
		@IFileDialogService private readonly dialogService: IFileDialogService
	) {
		super(id, label);
	}

	run(event?: unknown, data?: ITelemetryData): Promise<void> {
		return this.dialogService.pickFolderAndOpen({ forceNewWindow: false, telemetryExtraData: data });
	}
}

export class OpenFileFolderAction extends Action {

	static readonly ID = 'workbench.action.files.openFileFolder';
	static readonly LABEL = nls.localize('openFileFolder', "Open...");

	constructor(
		id: string,
		label: string,
		@IFileDialogService private readonly dialogService: IFileDialogService
	) {
		super(id, label);
	}

	run(event?: unknown, data?: ITelemetryData): Promise<void> {
		return this.dialogService.pickFileFolderAndOpen({ forceNewWindow: false, telemetryExtraData: data });
	}
}

export class OpenWorkspaceAction extends Action {

	static readonly ID = 'workbench.action.openWorkspace';
	static readonly LABEL = nls.localize('openWorkspaceAction', "Open Workspace...");

	constructor(
		id: string,
		label: string,
		@IFileDialogService private readonly dialogService: IFileDialogService
	) {
		super(id, label);
	}

	run(event?: unknown, data?: ITelemetryData): Promise<void> {
		return this.dialogService.pickWorkspaceAndOpen({ telemetryExtraData: data });
	}
}

/* AGPL */
export class ClearAssemblyListAction extends Action {

	static readonly ID = 'workbench.action.clearAssemblyList';
	static readonly LABEL = nls.localize('clearAssemblyList', "Clear Assembly List");

	constructor(
		id: string,
		label: string,
		@IFileService private readonly fileService: IFileService,
		@IProgressService private readonly progressService: IProgressService,
		@IDecompilationService private readonly decompilationService: IDecompilationService,
		@INotificationService private readonly notificationService: INotificationService,
		@IAnalyticsService private readonly analyticsService: IAnalyticsService,
		@IHostService private readonly hostService: IHostService,
		@IWorkbenchEnvironmentService private readonly environmentService: IWorkbenchEnvironmentService
	) {
		super(id, label);
	}

	async run(): Promise<void> {
		await this.progressService.withProgress({ location: ProgressLocation.Dialog, nonClosable: true }, async progress => {
			await this.analyticsService.trackEvent('AssemblyList', 'Clear');

			progress.report({ message: 'Clearing assembly list...' });

			try {
				await this.decompilationService.clearAssemblyList();
				await this.clearWorkspaceDirectory();

				return this.hostService.openWindow({ forceReuseWindow: true, remoteAuthority: this.environmentService.configuration.remoteAuthority });
			} catch (err) {
				this.notificationService.error(err.message || 'Failed to clear assembly list');
			}
		});
	}

	private async clearWorkspaceDirectory() : Promise<void> {
		const directoryPath = await this.decompilationService.getWorkspaceDirectory();
		const resolvedDirectory = await this.fileService.resolve(URI.file(directoryPath));

		if (resolvedDirectory.children) {
			for(const entry of resolvedDirectory.children) {
				if (await this.fileService.exists(entry.resource)) {
					await this.fileService.del(entry.resource, { useTrash: false, recursive: true });
				}
			}
		}
	}
}
/* End AGPL */

export class OpenWorkspaceConfigFileAction extends Action {

	static readonly ID = 'workbench.action.openWorkspaceConfigFile';
	static readonly LABEL = nls.localize('openWorkspaceConfigFile', "Open Workspace Configuration File");

	constructor(
		id: string,
		label: string,
		@IWorkspaceContextService private readonly workspaceContextService: IWorkspaceContextService,
		@IEditorService private readonly editorService: IEditorService
	) {
		super(id, label);

		this.enabled = !!this.workspaceContextService.getWorkspace().configuration;
	}

	async run(): Promise<void> {
		const configuration = this.workspaceContextService.getWorkspace().configuration;
		if (configuration) {
			await this.editorService.openEditor({ resource: configuration });
		}
	}
}

export class AddRootFolderAction extends Action {

	static readonly ID = 'workbench.action.addRootFolder';
	static readonly LABEL = ADD_ROOT_FOLDER_LABEL;

	constructor(
		id: string,
		label: string,
		@ICommandService private readonly commandService: ICommandService
	) {
		super(id, label);
	}

	run(): Promise<void> {
		return this.commandService.executeCommand(ADD_ROOT_FOLDER_COMMAND_ID);
	}
}

export class GlobalRemoveRootFolderAction extends Action {

	static readonly ID = 'workbench.action.removeRootFolder';
	static readonly LABEL = nls.localize('globalRemoveFolderFromWorkspace', "Remove Folder from Workspace...");

	constructor(
		id: string,
		label: string,
		@IWorkspaceEditingService private readonly workspaceEditingService: IWorkspaceEditingService,
		@IWorkspaceContextService private readonly contextService: IWorkspaceContextService,
		@ICommandService private readonly commandService: ICommandService
	) {
		super(id, label);
	}

	async run(): Promise<void> {
		const state = this.contextService.getWorkbenchState();

		// Workspace / Folder
		if (state === WorkbenchState.WORKSPACE || state === WorkbenchState.FOLDER) {
			const folder = await this.commandService.executeCommand<IWorkspaceFolder>(PICK_WORKSPACE_FOLDER_COMMAND_ID);
			if (folder) {
				await this.workspaceEditingService.removeFolders([folder.uri]);
			}
		}
	}
}

export class SaveWorkspaceAsAction extends Action {

	static readonly ID = 'workbench.action.saveWorkspaceAs';
	static readonly LABEL = nls.localize('saveWorkspaceAsAction', "Save Workspace As...");

	constructor(
		id: string,
		label: string,
		@IWorkspaceContextService private readonly contextService: IWorkspaceContextService,
		@IWorkspaceEditingService private readonly workspaceEditingService: IWorkspaceEditingService

	) {
		super(id, label);
	}

	async run(): Promise<void> {
		const configPathUri = await this.workspaceEditingService.pickNewWorkspacePath();
		if (configPathUri && hasWorkspaceFileExtension(configPathUri)) {
			switch (this.contextService.getWorkbenchState()) {
				case WorkbenchState.EMPTY:
				case WorkbenchState.FOLDER:
					const folders = this.contextService.getWorkspace().folders.map(folder => ({ uri: folder.uri }));
					return this.workspaceEditingService.createAndEnterWorkspace(folders, configPathUri);
				case WorkbenchState.WORKSPACE:
					return this.workspaceEditingService.saveAndEnterWorkspace(configPathUri);
			}
		}
	}
}

export class DuplicateWorkspaceInNewWindowAction extends Action {

	static readonly ID = 'workbench.action.duplicateWorkspaceInNewWindow';
	static readonly LABEL = nls.localize('duplicateWorkspaceInNewWindow', "Duplicate Workspace in New Window");

	constructor(
		id: string,
		label: string,
		@IWorkspaceContextService private readonly workspaceContextService: IWorkspaceContextService,
		@IWorkspaceEditingService private readonly workspaceEditingService: IWorkspaceEditingService,
		@IHostService private readonly hostService: IHostService,
		@IWorkspacesService private readonly workspacesService: IWorkspacesService,
		@IWorkbenchEnvironmentService private readonly environmentService: IWorkbenchEnvironmentService
	) {
		super(id, label);
	}

	async run(): Promise<void> {
		const folders = this.workspaceContextService.getWorkspace().folders;
		const remoteAuthority = this.environmentService.configuration.remoteAuthority;

		const newWorkspace = await this.workspacesService.createUntitledWorkspace(folders, remoteAuthority);
		await this.workspaceEditingService.copyWorkspaceSettings(newWorkspace);

		return this.hostService.openWindow([{ workspaceUri: newWorkspace.configPath }], { forceNewWindow: true });
	}
}

// --- Actions Registration

const registry = Registry.as<IWorkbenchActionRegistry>(Extensions.WorkbenchActions);
const workspacesCategory = nls.localize('workspaces', "Workspaces");

/* AGPL */
// registry.registerWorkbenchAction(SyncActionDescriptor.from(AddRootFolderAction), 'Workspaces: Add Folder to Workspace...', workspacesCategory);
// registry.registerWorkbenchAction(SyncActionDescriptor.from(GlobalRemoveRootFolderAction), 'Workspaces: Remove Folder from Workspace...', workspacesCategory);
registry.registerWorkbenchAction(SyncActionDescriptor.from(ClearAssemblyListAction), 'Workspaces: Clear Assembly List', workspacesCategory);
// registry.registerWorkbenchAction(SyncActionDescriptor.from(SaveWorkspaceAsAction), 'Workspaces: Save Workspace As...', workspacesCategory);
// registry.registerWorkbenchAction(SyncActionDescriptor.from(DuplicateWorkspaceInNewWindowAction), 'Workspaces: Duplicate Workspace in New Window', workspacesCategory);
/* End AGPL */

// --- Menu Registration

/* AGPL */
// CommandsRegistry.registerCommand(OpenWorkspaceConfigFileAction.ID, serviceAccessor => {
// 	serviceAccessor.get(IInstantiationService).createInstance(OpenWorkspaceConfigFileAction, OpenWorkspaceConfigFileAction.ID, OpenWorkspaceConfigFileAction.LABEL).run();
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '3_workspace',
// 	command: {
// 		id: ADD_ROOT_FOLDER_COMMAND_ID,
// 		title: nls.localize({ key: 'miAddFolderToWorkspace', comment: ['&& denotes a mnemonic'] }, "A&&dd Folder to Workspace...")
// 	},
// 	order: 1
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '3_workspace',
// 	command: {
// 		id: SaveWorkspaceAsAction.ID,
// 		title: nls.localize('miSaveWorkspaceAs', "Save Workspace As...")
// 	},
// 	order: 2
// });

// MenuRegistry.appendMenuItem(MenuId.CommandPalette, {
// 	command: {
// 		id: OpenWorkspaceConfigFileAction.ID,
// 		title: { value: `${workspacesCategory}: ${OpenWorkspaceConfigFileAction.LABEL}`, original: 'Workspaces: Open Workspace Configuration File' },
// 	},
// 	when: WorkbenchStateContext.isEqualTo('workspace')
// });
/* End AGPL */

MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
	group: '6_close',
	command: {
		/* AGPL */
		id: ClearAssemblyListAction.ID,
		title: nls.localize({ key: 'miClearAssemblyList', comment: ['&& denotes a mnemonic'] }, "Clear &&Assembly List"),
		/* End AGPL */
		precondition: WorkspaceFolderCountContext.notEqualsTo('0')
	},
	order: 3,
	when: WorkbenchStateContext.notEqualsTo('workspace')
});

/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '6_close',
// 	command: {
// 		id: CloseWorkspaceAction.ID,
// 		title: nls.localize({ key: 'miCloseWorkspace', comment: ['&& denotes a mnemonic'] }, "Close &&Workspace")
// 	},
// 	order: 3,
// 	when: ContextKeyExpr.and(WorkbenchStateContext.isEqualTo('workspace'))
// });
/* End AGPL */
