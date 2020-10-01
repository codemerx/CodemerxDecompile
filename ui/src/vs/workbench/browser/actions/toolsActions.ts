//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

import { Action } from 'vs/base/common/actions';
import * as nls from 'vs/nls';
import { Registry } from 'vs/platform/registry/common/platform';
import { IWorkbenchActionRegistry, Extensions } from 'vs/workbench/common/actions';
import { SyncActionDescriptor, MenuRegistry, MenuId, ISubmenuItem, IMenuItem } from 'vs/platform/actions/common/actions';
import { KeyChord, KeyMod, KeyCode } from 'vs/base/common/keyCodes';
import { IFileDialogService } from 'vs/platform/dialogs/common/dialogs';
import { IDecompilationService } from 'vs/cd/workbench/DecompilationService';
import { INotificationService, Severity, IPromptChoice } from 'vs/platform/notification/common/notification';
import { URI } from 'vs/base/common/uri';
import { IProgressService, ProgressLocation } from 'vs/platform/progress/common/progress';
import { IWorkbenchContributionsRegistry, IWorkbenchContribution, Extensions as WorkbenchExtensions } from 'vs/workbench/common/contributions';
import { Disposable } from 'vs/base/common/lifecycle';
import { LifecyclePhase } from 'vs/platform/lifecycle/common/lifecycle';
import { IExplorerService } from 'vs/workbench/contrib/files/common/files';
import { RawContextKey } from 'vs/platform/contextkey/common/contextkey';

export const PROJECT_CREATION_ENABLED_STATE = new RawContextKey<boolean>('areProjectCreationCommandsEnabled', false);

const DANGEROUS_RESOURCES_MESSAGE = 'This assembly contains resources that may contain malicious code. Decompilation of such resources will result in execution of that malicious code. Do you want to decompile those resources during the project generation?';

abstract class BaseCreateProjectAction extends Action {

	constructor(
		id: string,
		label: string,
		private readonly fileDialogService: IFileDialogService,
		private readonly notificationService: INotificationService,
		private readonly decompilationService: IDecompilationService,
		private readonly progressService: IProgressService,
		private readonly explorerService: IExplorerService
	) {
		super(id, label);
	}

	protected abstract get projectVsStudioVersion(): string | undefined;

	async run(): Promise<void> {
		const selectedExplorerItems = this.explorerService.getContext(false);
		const currentOpenCodeEditorResourceUri = selectedExplorerItems.length ? selectedExplorerItems[0].resource : null;

		if (currentOpenCodeEditorResourceUri?.fsPath) {
			const { assemblyFilePath } = await this.decompilationService.getContextAssembly(currentOpenCodeEditorResourceUri.fsPath);
			const { containsDangerousResources, projectFileMetadata } = await this.decompilationService.getProjectCreationMetadata(assemblyFilePath, this.projectVsStudioVersion);

			if (!projectFileMetadata || !projectFileMetadata.isVSSupportedProjectType) {
				this.notificationService.error(projectFileMetadata?.projectTypeNotSupportedErrorMessage ?? 'Failed to generate project');
				return;
			}

			let decompileDangerousResources = false;

			if (containsDangerousResources) {
				decompileDangerousResources = await this.promptUserForDangerousResourcesDecompilation();
			}

			const defaultPath = this.fileDialogService.defaultFolderPath();
			if (defaultPath) {
				const { projectFileName, projectFileExtension, isDecompilerSupportedProjectType } = projectFileMetadata;

				const path = URI.joinPath(defaultPath, `${projectFileName}.${projectFileExtension}`);
				const projectFileExtensionName = isDecompilerSupportedProjectType ? 'Project file' : 'Error project file';
				const outputPath = await this.fileDialogService.showSaveDialog({ defaultUri: path, filters: [{ name: projectFileExtensionName, extensions: [projectFileExtension] }] });

				if (outputPath?.fsPath) {
					this.progressService.withProgress({ location: ProgressLocation.Dialog, nonClosable: true }, async progress => {
						progress.report({ message: 'Creating project...' });

						const { errorMessage } = await this.decompilationService.createProject(assemblyFilePath, outputPath.fsPath, decompileDangerousResources, this.projectVsStudioVersion);

						if (errorMessage) {
							this.notificationService.error(errorMessage);
						} else {
							this.notificationService.info('Project creation completed successfully.');
						}
					});
				}
			}
		}
	}

	private async promptUserForDangerousResourcesDecompilation(): Promise<boolean> {
		return new Promise(resolve => {
			const choices: IPromptChoice[] = [
				{
					label: 'Yes',
					run: () => resolve(true)
				},
				{
					label: 'No',
					run: () => resolve(false)
				}
			];

			this.notificationService.prompt(Severity.Warning, DANGEROUS_RESOURCES_MESSAGE, choices, {
				onCancel: () => resolve(false)
			});
		});
	}
}

class CreateProjectAction extends BaseCreateProjectAction {
	static readonly ID = 'workbench.action.decompilerCreateProject';
	static readonly LABEL = nls.localize('decompilerCreateProject', "Create Project...");

	constructor(
		id: string,
		label: string,
		@IFileDialogService fileDialogService: IFileDialogService,
		@INotificationService notificationService: INotificationService,
		@IDecompilationService decompilationService: IDecompilationService,
		@IProgressService progressService: IProgressService,
		@IExplorerService explorerService: IExplorerService
	) {
		super(id, label, fileDialogService, notificationService, decompilationService, progressService, explorerService);
	}

	protected get projectVsStudioVersion(): string | undefined {
		return undefined;
	}
}

abstract class CreateLegacyProjectAction extends BaseCreateProjectAction {
	static readonly BASE_ID = 'workbench.action.decompilerCreateLegacyProject';
	static readonly LABEL = nls.localize('decompilerCreateLegacyProject', "Create Legacy Project");

	constructor(
		id: string,
		label: string,
		@IFileDialogService fileDialogService: IFileDialogService,
		@INotificationService notificationService: INotificationService,
		@IDecompilationService decompilationService: IDecompilationService,
		@IProgressService progressService: IProgressService,
		@IExplorerService explorerService: IExplorerService
	) {
		super(id, label, fileDialogService, notificationService, decompilationService, progressService, explorerService);
	}

	protected get projectVsStudioVersion(): string | undefined {
		const idParts = this._id.split('.');
		return idParts[idParts.length - 1];
	}
}

class Create2010ProjectAction extends CreateLegacyProjectAction {
	static readonly ID = 'workbench.action.decompilerCreateLegacyProject.2010';

	constructor(
		id: string,
		label: string,
		@IFileDialogService fileDialogService: IFileDialogService,
		@INotificationService notificationService: INotificationService,
		@IDecompilationService decompilationService: IDecompilationService,
		@IProgressService progressService: IProgressService,
		@IExplorerService explorerService: IExplorerService
	) {
		super(id, label, fileDialogService, notificationService, decompilationService, progressService, explorerService);
	}
}

class Create2012ProjectAction extends CreateLegacyProjectAction {
	static readonly ID = 'workbench.action.decompilerCreateLegacyProject.2012';

	constructor(
		id: string,
		label: string,
		@IFileDialogService fileDialogService: IFileDialogService,
		@INotificationService notificationService: INotificationService,
		@IDecompilationService decompilationService: IDecompilationService,
		@IProgressService progressService: IProgressService,
		@IExplorerService explorerService: IExplorerService
	) {
		super(id, label, fileDialogService, notificationService, decompilationService, progressService, explorerService);
	}
}

class Create2013ProjectAction extends CreateLegacyProjectAction {
	static readonly ID = 'workbench.action.decompilerCreateLegacyProject.2013';

	constructor(
		id: string,
		label: string,
		@IFileDialogService fileDialogService: IFileDialogService,
		@INotificationService notificationService: INotificationService,
		@IDecompilationService decompilationService: IDecompilationService,
		@IProgressService progressService: IProgressService,
		@IExplorerService explorerService: IExplorerService
	) {
		super(id, label, fileDialogService, notificationService, decompilationService, progressService, explorerService);
	}
}

class Create2015ProjectAction extends CreateLegacyProjectAction {
	static readonly ID = 'workbench.action.decompilerCreateLegacyProject.2015';

	constructor(
		id: string,
		label: string,
		@IFileDialogService fileDialogService: IFileDialogService,
		@INotificationService notificationService: INotificationService,
		@IDecompilationService decompilationService: IDecompilationService,
		@IProgressService progressService: IProgressService,
		@IExplorerService explorerService: IExplorerService
	) {
		super(id, label, fileDialogService, notificationService, decompilationService, progressService, explorerService);
	}
}

class ToolsActionsProvider extends Disposable implements IWorkbenchContribution {

	constructor(@IDecompilationService private readonly decompilationService: IDecompilationService) {
		super();

		this.registerActions();
		this.appendMenuItems();
	}

	private registerActions() : void {
		const registry = Registry.as<IWorkbenchActionRegistry>(Extensions.WorkbenchActions);

		registry.registerWorkbenchAction(SyncActionDescriptor.from(CreateProjectAction, { primary: KeyChord(KeyMod.CtrlCmd, KeyMod.Shift & KeyCode.KEY_G) }), 'Tools: Create Project...', nls.localize('tools', "Tools"));
		registry.registerWorkbenchAction(SyncActionDescriptor.from(Create2010ProjectAction), 'Legacy Project Visual Studio 2010', nls.localize('tools', "Tools"));
		registry.registerWorkbenchAction(SyncActionDescriptor.from(Create2012ProjectAction), 'Legacy Project Visual Studio 2012', nls.localize('tools', "Tools"));
		registry.registerWorkbenchAction(SyncActionDescriptor.from(Create2013ProjectAction), 'Legacy Project Visual Studio 2013', nls.localize('tools', "Tools"));
		registry.registerWorkbenchAction(SyncActionDescriptor.from(Create2015ProjectAction), 'Legacy Project Visual Studio 2015', nls.localize('tools', "Tools"));
	}

	private async appendMenuItems() : Promise<void> {
		const legacyVsStudioVersions: string[] = await this.decompilationService.getLegacyVisualStudioVersions();

		const menuItems: { id: MenuId, item: IMenuItem | ISubmenuItem }[] = [];

		for (let i = 0; i < legacyVsStudioVersions.length; i += 1) {
			menuItems.push({
				id: MenuId.MenubarCreateLegacyProjectMenu,
				item: {
					command: {
						id: `${CreateLegacyProjectAction.BASE_ID}.${legacyVsStudioVersions[i]}`,
						precondition: PROJECT_CREATION_ENABLED_STATE,
						title: nls.localize({ key: `miDecompilerCreateLegacyProject${legacyVsStudioVersions[i]}`, comment: ['&& denotes a mnemonic'] }, `&&Visual Studio ${legacyVsStudioVersions[i]}`)
					},
					order: i + 1
				}
			});
		}

		menuItems.push({
			id: MenuId.MenubarToolsMenu,
			item: {
				title: nls.localize({ key: 'miDecompilerCreateLegacyProject', comment: ['&& denotes a mnemonic'] }, "&&Create Legacy Project"),
				submenu: MenuId.MenubarCreateLegacyProjectMenu,
				order: 2
			}
		});

		menuItems.push({
			id: MenuId.MenubarToolsMenu,
			item: {
				command: {
					id: CreateProjectAction.ID,
					precondition: PROJECT_CREATION_ENABLED_STATE,
					title: nls.localize({ key: 'miDecompilerCreateProject', comment: ['&& denotes a mnemonic'] }, "&&Crete Project...")
				},
				order: 1
			}
		});

		MenuRegistry.appendMenuItems(menuItems);
	}
}

Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench).registerWorkbenchContribution(ToolsActionsProvider, LifecyclePhase.Starting);
