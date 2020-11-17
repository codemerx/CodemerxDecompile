/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as nls from 'vs/nls';
import { Registry } from 'vs/platform/registry/common/platform';
/* AGPL */
import { GlobalCompareResourcesAction, ShowActiveFileInExplorer, CompareWithClipboardAction, openFilePreserveFocusHandler, ShowOpenedFileInNewWindow } from 'vs/workbench/contrib/files/browser/fileActions';
/* End AGPL */
import { revertLocalChangesCommand, acceptLocalChangesCommand, CONFLICT_RESOLUTION_CONTEXT } from 'vs/workbench/contrib/files/browser/editors/textFileSaveErrorHandler';
import { SyncActionDescriptor, MenuId, MenuRegistry, ILocalizedString } from 'vs/platform/actions/common/actions';
import { IWorkbenchActionRegistry, Extensions as ActionExtensions } from 'vs/workbench/common/actions';
import { KeyMod, KeyChord, KeyCode } from 'vs/base/common/keyCodes';
/* AGPL */
import { openWindowCommand, REVEAL_IN_EXPLORER_COMMAND_ID, OPEN_TO_SIDE_COMMAND_ID, OpenEditorsGroupContext, /* COMPARE_RESOURCE_COMMAND_ID, SELECT_FOR_COMPARE_COMMAND_ID, ResourceSelectedForCompareContext, COMPARE_SELECTED_COMMAND_ID */ newWindowCommand, OPEN_WITH_EXPLORER_COMMAND_ID } from 'vs/workbench/contrib/files/browser/fileCommands';
/* End AGPL */
import { CommandsRegistry, ICommandHandler } from 'vs/platform/commands/common/commands';
import { ContextKeyExpr, ContextKeyExpression } from 'vs/platform/contextkey/common/contextkey';
import { KeybindingsRegistry, KeybindingWeight } from 'vs/platform/keybinding/common/keybindingsRegistry';
import { isMacintosh } from 'vs/base/common/platform';
/* AGPL */
import { FilesExplorerFocusCondition, ExplorerRootContext, ExplorerFolderContext, ExplorerResourceAvailableEditorIdsContext } from 'vs/workbench/contrib/files/common/files';
import { CLOSE_EDITORS_IN_GROUP_COMMAND_ID, CLOSE_EDITOR_COMMAND_ID, CLOSE_OTHER_EDITORS_IN_GROUP_COMMAND_ID } from 'vs/workbench/browser/parts/editor/editorCommands';
/* End AGPL */
import { ResourceContextKey } from 'vs/workbench/common/resources';
import { Schemas } from 'vs/base/common/network';
/* AGPL */
// import { WorkbenchListDoubleSelection } from 'vs/platform/list/browser/listService';
import { OpenFileFolderAction, OpenFileAction } from 'vs/workbench/browser/actions/workspaceActions';
/* End AGPL */
import { ThemeIcon } from 'vs/platform/theme/common/themeService';

// Contribute Global Actions
const category = { value: nls.localize('filesCategory', "File"), original: 'File' };

const registry = Registry.as<IWorkbenchActionRegistry>(ActionExtensions.WorkbenchActions);
/* AGPL */
// registry.registerWorkbenchAction(SyncActionDescriptor.from(SaveAllAction, { primary: undefined, mac: { primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.KEY_S }, win: { primary: KeyChord(KeyMod.CtrlCmd | KeyCode.KEY_K, KeyCode.KEY_S) } }), 'File: Save All', category.value);
/* End AGPL */
registry.registerWorkbenchAction(SyncActionDescriptor.from(GlobalCompareResourcesAction), 'File: Compare Active File With...', category.value);
/* AGPL */
// registry.registerWorkbenchAction(SyncActionDescriptor.from(FocusFilesExplorer), 'File: Focus on Files Explorer', category.value);
/* End AGPL */
registry.registerWorkbenchAction(SyncActionDescriptor.from(ShowActiveFileInExplorer), 'File: Reveal Active File in Side Bar', category.value);
/* AGPL */
// registry.registerWorkbenchAction(SyncActionDescriptor.from(CollapseExplorerView), 'File: Collapse Folders in Explorer', category.value);
// registry.registerWorkbenchAction(SyncActionDescriptor.from(RefreshExplorerView), 'File: Refresh Explorer', category.value);
/* End AGPL */
registry.registerWorkbenchAction(SyncActionDescriptor.from(CompareWithClipboardAction, { primary: KeyChord(KeyMod.CtrlCmd | KeyCode.KEY_K, KeyCode.KEY_C) }), 'File: Compare Active File with Clipboard', category.value);
/* AGPL */
// registry.registerWorkbenchAction(SyncActionDescriptor.from(ToggleAutoSaveAction), 'File: Toggle Auto Save', category.value);
/* End AGPL */
registry.registerWorkbenchAction(SyncActionDescriptor.from(ShowOpenedFileInNewWindow, { primary: KeyChord(KeyMod.CtrlCmd | KeyCode.KEY_K, KeyCode.KEY_O) }), 'File: Open Active File in New Window', category.value);

/* AGPL */
// const workspacesCategory = nls.localize('workspaces', "Workspaces");
// registry.registerWorkbenchAction(SyncActionDescriptor.from(OpenWorkspaceAction), 'Workspaces: Open Workspace...', workspacesCategory);
/* End AGPL */

const fileCategory = nls.localize('file', "File");
if (isMacintosh) {
	registry.registerWorkbenchAction(SyncActionDescriptor.from(OpenFileFolderAction, { primary: KeyMod.CtrlCmd | KeyCode.KEY_O }), 'File: Open...', fileCategory);
} else {
	/* AGPL */
	registry.registerWorkbenchAction(SyncActionDescriptor.from(OpenFileAction, { primary: KeyMod.CtrlCmd | KeyCode.KEY_O }), 'File: Open Assembly...', fileCategory);
	// registry.registerWorkbenchAction(SyncActionDescriptor.from(OpenFolderAction, { primary: KeyChord(KeyMod.CtrlCmd | KeyCode.KEY_K, KeyMod.CtrlCmd | KeyCode.KEY_O) }), 'File: Open Folder...', fileCategory);
	/* End AGPL */
}

// Commands
CommandsRegistry.registerCommand('_files.windowOpen', openWindowCommand);
CommandsRegistry.registerCommand('_files.newWindow', newWindowCommand);

const explorerCommandsWeightBonus = 10; // give our commands a little bit more weight over other default list/tree commands

/* AGPL */
// const RENAME_ID = 'renameFile';
// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: RENAME_ID,
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerRootContext.toNegated(), ExplorerResourceNotReadonlyContext),
// 	primary: KeyCode.F2,
// 	mac: {
// 		primary: KeyCode.Enter
// 	},
// 	handler: renameHandler
// });

// const MOVE_FILE_TO_TRASH_ID = 'moveFileToTrash';
// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: MOVE_FILE_TO_TRASH_ID,
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerResourceNotReadonlyContext, ExplorerResourceMoveableToTrash),
// 	primary: KeyCode.Delete,
// 	mac: {
// 		primary: KeyMod.CtrlCmd | KeyCode.Backspace
// 	},
// 	handler: moveFileToTrashHandler
// });

// const DELETE_FILE_ID = 'deleteFile';
// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: DELETE_FILE_ID,
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerResourceNotReadonlyContext),
// 	primary: KeyMod.Shift | KeyCode.Delete,
// 	mac: {
// 		primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.Backspace
// 	},
// 	handler: deleteFileHandler
// });

// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: DELETE_FILE_ID,
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerResourceNotReadonlyContext, ExplorerResourceMoveableToTrash.toNegated()),
// 	primary: KeyCode.Delete,
// 	mac: {
// 		primary: KeyMod.CtrlCmd | KeyCode.Backspace
// 	},
// 	handler: deleteFileHandler
// });

// const CUT_FILE_ID = 'filesExplorer.cut';
// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: CUT_FILE_ID,
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerRootContext.toNegated()),
// 	primary: KeyMod.CtrlCmd | KeyCode.KEY_X,
// 	handler: cutFileHandler,
// });

// const COPY_FILE_ID = 'filesExplorer.copy';
// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: COPY_FILE_ID,
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerRootContext.toNegated()),
// 	primary: KeyMod.CtrlCmd | KeyCode.KEY_C,
// 	handler: copyFileHandler,
// });

// const PASTE_FILE_ID = 'filesExplorer.paste';

// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: PASTE_FILE_ID,
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerResourceNotReadonlyContext),
// 	primary: KeyMod.CtrlCmd | KeyCode.KEY_V,
// 	handler: pasteFileHandler
// });

// KeybindingsRegistry.registerCommandAndKeybindingRule({
// 	id: 'filesExplorer.cancelCut',
// 	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
// 	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerResourceCut),
// 	primary: KeyCode.Escape,
// 	handler: async (accessor: ServicesAccessor) => {
// 		const explorerService = accessor.get(IExplorerService);
// 		await explorerService.setToCopy([], true);
// 	}
// });
/* End AGPL */

KeybindingsRegistry.registerCommandAndKeybindingRule({
	id: 'filesExplorer.openFilePreserveFocus',
	weight: KeybindingWeight.WorkbenchContrib + explorerCommandsWeightBonus,
	when: ContextKeyExpr.and(FilesExplorerFocusCondition, ExplorerFolderContext.toNegated()),
	primary: KeyCode.Space,
	handler: openFilePreserveFocusHandler
});

/* AGPL */
// const copyPathCommand = {
// 	id: COPY_PATH_COMMAND_ID,
// 	title: nls.localize('copyPath', "Copy Path")
// };

// const copyRelativePathCommand = {
// 	id: COPY_RELATIVE_PATH_COMMAND_ID,
// 	title: nls.localize('copyRelativePath', "Copy Relative Path")
// };
/* End AGPL */

// Editor Title Context Menu
/* AGPL */
// appendEditorTitleContextMenuItem(COPY_PATH_COMMAND_ID, copyPathCommand.title, ResourceContextKey.IsFileSystemResource, '1_cutcopypaste');
// appendEditorTitleContextMenuItem(COPY_RELATIVE_PATH_COMMAND_ID, copyRelativePathCommand.title, ResourceContextKey.IsFileSystemResource, '1_cutcopypaste');
/* End AGPL */
appendEditorTitleContextMenuItem(REVEAL_IN_EXPLORER_COMMAND_ID, nls.localize('revealInSideBar', "Reveal in Side Bar"), ResourceContextKey.IsFileSystemResource);

export function appendEditorTitleContextMenuItem(id: string, title: string, when: ContextKeyExpression | undefined, group?: string): void {

	// Menu
	MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, {
		command: { id, title },
		when,
		group: group || '2_files'
	});
}

// Editor Title Menu for Conflict Resolution
appendSaveConflictEditorTitleAction('workbench.files.action.acceptLocalChanges', nls.localize('acceptLocalChanges', "Use your changes and overwrite file contents"), { id: 'codicon/check' }, -10, acceptLocalChangesCommand);
appendSaveConflictEditorTitleAction('workbench.files.action.revertLocalChanges', nls.localize('revertLocalChanges', "Discard your changes and revert to file contents"), { id: 'codicon/discard' }, -9, revertLocalChangesCommand);

function appendSaveConflictEditorTitleAction(id: string, title: string, icon: ThemeIcon, order: number, command: ICommandHandler): void {

	// Command
	CommandsRegistry.registerCommand(id, command);

	// Action
	MenuRegistry.appendMenuItem(MenuId.EditorTitle, {
		command: { id, title, icon },
		when: ContextKeyExpr.equals(CONFLICT_RESOLUTION_CONTEXT, true),
		group: 'navigation',
		order
	});
}

// Menu registration - command palette

export function appendToCommandPalette(id: string, title: ILocalizedString, category: ILocalizedString, when?: ContextKeyExpression): void {
	MenuRegistry.appendMenuItem(MenuId.CommandPalette, {
		command: {
			id,
			title,
			category
		},
		when
	});
}

/* AGPL */
// appendToCommandPalette(COPY_PATH_COMMAND_ID, { value: nls.localize('copyPathOfActive', "Copy Path of Active File"), original: 'Copy Path of Active File' }, category);
// appendToCommandPalette(COPY_RELATIVE_PATH_COMMAND_ID, { value: nls.localize('copyRelativePathOfActive', "Copy Relative Path of Active File"), original: 'Copy Relative Path of Active File' }, category);
// appendToCommandPalette(SAVE_FILE_COMMAND_ID, { value: SAVE_FILE_LABEL, original: 'Save' }, category);
// appendToCommandPalette(SAVE_FILE_WITHOUT_FORMATTING_COMMAND_ID, { value: SAVE_FILE_WITHOUT_FORMATTING_LABEL, original: 'Save without Formatting' }, category);
// appendToCommandPalette(SAVE_ALL_IN_GROUP_COMMAND_ID, { value: nls.localize('saveAllInGroup', "Save All in Group"), original: 'Save All in Group' }, category);
// appendToCommandPalette(SAVE_FILES_COMMAND_ID, { value: nls.localize('saveFiles', "Save All Files"), original: 'Save All Files' }, category);
// appendToCommandPalette(REVERT_FILE_COMMAND_ID, { value: nls.localize('revert', "Revert File"), original: 'Revert File' }, category);
// appendToCommandPalette(COMPARE_WITH_SAVED_COMMAND_ID, { value: nls.localize('compareActiveWithSaved', "Compare Active File with Saved"), original: 'Compare Active File with Saved' }, category);
// appendToCommandPalette(SAVE_FILE_AS_COMMAND_ID, { value: SAVE_FILE_AS_LABEL, original: 'Save As...' }, category);
appendToCommandPalette(CLOSE_EDITOR_COMMAND_ID, { value: nls.localize('closeEditor', "Close Code Viewer"), original: 'Close Code Viewer' }, { value: nls.localize('view', "View"), original: 'View' });
// appendToCommandPalette(NEW_FILE_COMMAND_ID, { value: NEW_FILE_LABEL, original: 'New File' }, category, WorkspaceFolderCountContext.notEqualsTo('0'));
// appendToCommandPalette(NEW_FOLDER_COMMAND_ID, { value: NEW_FOLDER_LABEL, original: 'New Folder' }, category, WorkspaceFolderCountContext.notEqualsTo('0'));
// appendToCommandPalette(DOWNLOAD_COMMAND_ID, { value: DOWNLOAD_LABEL, original: 'Download' }, category, ContextKeyExpr.and(ResourceContextKey.Scheme.notEqualsTo(Schemas.file)));
// appendToCommandPalette(NEW_UNTITLED_FILE_COMMAND_ID, { value: NEW_UNTITLED_FILE_LABEL, original: 'New Untitled File' }, category);
/* End AGPL */

// Menu registration - open editors

const openToSideCommand = {
	id: OPEN_TO_SIDE_COMMAND_ID,
	title: nls.localize('openToSide', "Open to the Side")
};
MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
	group: 'navigation',
	order: 10,
	command: openToSideCommand,
	when: ContextKeyExpr.or(ResourceContextKey.IsFileSystemResource, ResourceContextKey.Scheme.isEqualTo(Schemas.untitled))
});

/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '1_cutcopypaste',
// 	order: 10,
// 	command: copyPathCommand,
// 	when: ResourceContextKey.IsFileSystemResource
// });

// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '1_cutcopypaste',
// 	order: 20,
// 	command: copyRelativePathCommand,
// 	when: ResourceContextKey.IsFileSystemResource
// });

// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '2_save',
// 	order: 10,
// 	command: {
// 		id: SAVE_FILE_COMMAND_ID,
// 		title: SAVE_FILE_LABEL,
// 		precondition: DirtyEditorContext
// 	},
// 	when: ContextKeyExpr.or(
// 		// Untitled Editors
// 		ResourceContextKey.Scheme.isEqualTo(Schemas.untitled),
// 		// Or:
// 		ContextKeyExpr.and(
// 			// Not: editor groups
// 			OpenEditorsGroupContext.toNegated(),
// 			// Not: readonly editors
// 			ReadonlyEditorContext.toNegated(),
// 			// Not: auto save after short delay
// 			AutoSaveAfterShortDelayContext.toNegated()
// 		)
// 	)
// });

// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '2_save',
// 	order: 20,
// 	command: {
// 		id: REVERT_FILE_COMMAND_ID,
// 		title: nls.localize('revert', "Revert File"),
// 		precondition: DirtyEditorContext
// 	},
// 	when: ContextKeyExpr.and(
// 		// Not: editor groups
// 		OpenEditorsGroupContext.toNegated(),
// 		// Not: readonly editors
// 		ReadonlyEditorContext.toNegated(),
// 		// Not: untitled editors (revert closes them)
// 		ResourceContextKey.Scheme.notEqualsTo(Schemas.untitled),
// 		// Not: auto save after short delay
// 		AutoSaveAfterShortDelayContext.toNegated()
// 	)
// });

// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '2_save',
// 	order: 30,
// 	command: {
// 		id: SAVE_ALL_IN_GROUP_COMMAND_ID,
// 		title: nls.localize('saveAll', "Save All"),
// 		precondition: DirtyWorkingCopiesContext
// 	},
// 	// Editor Group
// 	when: OpenEditorsGroupContext
// });

// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '3_compare',
// 	order: 10,
// 	command: {
// 		id: COMPARE_WITH_SAVED_COMMAND_ID,
// 		title: nls.localize('compareWithSaved', "Compare with Saved"),
// 		precondition: DirtyEditorContext
// 	},
// 	when: ContextKeyExpr.and(ResourceContextKey.IsFileSystemResource, AutoSaveAfterShortDelayContext.toNegated(), WorkbenchListDoubleSelection.toNegated())
// });

// const compareResourceCommand = {
// 	id: COMPARE_RESOURCE_COMMAND_ID,
// 	title: nls.localize('compareWithSelected', "Compare with Selected")
// };
// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '3_compare',
// 	order: 20,
// 	command: compareResourceCommand,
// 	when: ContextKeyExpr.and(ResourceContextKey.HasResource, ResourceSelectedForCompareContext, WorkbenchListDoubleSelection.toNegated())
// });

// const selectForCompareCommand = {
// 	id: SELECT_FOR_COMPARE_COMMAND_ID,
// 	title: nls.localize('compareSource', "Select for Compare")
// };
// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '3_compare',
// 	order: 30,
// 	command: selectForCompareCommand,
// 	when: ContextKeyExpr.and(ResourceContextKey.HasResource, WorkbenchListDoubleSelection.toNegated())
// });

// const compareSelectedCommand = {
// 	id: COMPARE_SELECTED_COMMAND_ID,
// 	title: nls.localize('compareSelected', "Compare Selected")
// };
// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '3_compare',
// 	order: 30,
// 	command: compareSelectedCommand,
// 	when: ContextKeyExpr.and(ResourceContextKey.HasResource, WorkbenchListDoubleSelection)
// });
/* End AGPL */

MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
	group: '4_close',
	order: 10,
	command: {
		id: CLOSE_EDITOR_COMMAND_ID,
		title: nls.localize('close', "Close")
	},
	when: OpenEditorsGroupContext.toNegated()
});

MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
	group: '4_close',
	order: 20,
	command: {
		id: CLOSE_OTHER_EDITORS_IN_GROUP_COMMAND_ID,
		title: nls.localize('closeOthers', "Close Others")
	},
	when: OpenEditorsGroupContext.toNegated()
});

/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
// 	group: '4_close',
// 	order: 30,
// 	command: {
// 		id: CLOSE_SAVED_EDITORS_COMMAND_ID,
// 		title: nls.localize('closeSaved', "Close Saved")
// 	}
// });
/* End AGPL */

MenuRegistry.appendMenuItem(MenuId.OpenEditorsContext, {
	group: '4_close',
	order: 40,
	command: {
		id: CLOSE_EDITORS_IN_GROUP_COMMAND_ID,
		title: nls.localize('closeAll', "Close All")
	}
});

// Menu registration - explorer

/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: 'navigation',
// 	order: 4,
// 	command: {
// 		id: NEW_FILE_COMMAND_ID,
// 		title: NEW_FILE_LABEL,
// 		precondition: ExplorerResourceNotReadonlyContext
// 	},
// 	when: ExplorerFolderContext
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: 'navigation',
// 	order: 6,
// 	command: {
// 		id: NEW_FOLDER_COMMAND_ID,
// 		title: NEW_FOLDER_LABEL,
// 		precondition: ExplorerResourceNotReadonlyContext
// 	},
// 	when: ExplorerFolderContext
// });
/* End AGPL */

MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
	group: 'navigation',
	order: 10,
	command: openToSideCommand,
	when: ContextKeyExpr.and(ExplorerFolderContext.toNegated(), ResourceContextKey.HasResource)
});

MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
	group: 'navigation',
	order: 20,
	command: {
		id: OPEN_WITH_EXPLORER_COMMAND_ID,
		title: nls.localize('explorerOpenWith', "Open With..."),
	},
	when: ContextKeyExpr.and(ExplorerRootContext.toNegated(), ExplorerResourceAvailableEditorIdsContext),
});

/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '3_compare',
// 	order: 20,
// 	command: compareResourceCommand,
// 	when: ContextKeyExpr.and(ExplorerFolderContext.toNegated(), ResourceContextKey.HasResource, ResourceSelectedForCompareContext, WorkbenchListDoubleSelection.toNegated())
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '3_compare',
// 	order: 30,
// 	command: selectForCompareCommand,
// 	when: ContextKeyExpr.and(ExplorerFolderContext.toNegated(), ResourceContextKey.HasResource, WorkbenchListDoubleSelection.toNegated())
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '3_compare',
// 	order: 30,
// 	command: compareSelectedCommand,
// 	when: ContextKeyExpr.and(ExplorerFolderContext.toNegated(), ResourceContextKey.HasResource, WorkbenchListDoubleSelection)
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '5_cutcopypaste',
// 	order: 8,
// 	command: {
// 		id: CUT_FILE_ID,
// 		title: nls.localize('cut', "Cut")
// 	},
// 	when: ExplorerRootContext.toNegated()
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '5_cutcopypaste',
// 	order: 10,
// 	command: {
// 		id: COPY_FILE_ID,
// 		title: COPY_FILE_LABEL
// 	},
// 	when: ExplorerRootContext.toNegated()
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '5_cutcopypaste',
// 	order: 20,
// 	command: {
// 		id: PASTE_FILE_ID,
// 		title: PASTE_FILE_LABEL,
// 		precondition: ContextKeyExpr.and(ExplorerResourceNotReadonlyContext, FileCopiedContext)
// 	},
// 	when: ExplorerFolderContext
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, ({
// 	group: '5_cutcopypaste',
// 	order: 30,
// 	command: {
// 		id: DOWNLOAD_COMMAND_ID,
// 		title: DOWNLOAD_LABEL,
// 	},
// 	when: ContextKeyExpr.or(ContextKeyExpr.and(ResourceContextKey.Scheme.notEqualsTo(Schemas.file), IsWebContext.toNegated()), ContextKeyExpr.and(ResourceContextKey.Scheme.notEqualsTo(Schemas.file), ExplorerFolderContext.toNegated(), ExplorerRootContext.toNegated()))
// }));

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '6_copypath',
// 	order: 30,
// 	command: copyPathCommand,
// 	when: ResourceContextKey.IsFileSystemResource
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '6_copypath',
// 	order: 30,
// 	command: copyRelativePathCommand,
// 	when: ResourceContextKey.IsFileSystemResource
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '2_workspace',
// 	order: 10,
// 	command: {
// 		id: ADD_ROOT_FOLDER_COMMAND_ID,
// 		title: ADD_ROOT_FOLDER_LABEL
// 	},
// 	when: ExplorerRootContext
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '2_workspace',
// 	order: 30,
// 	command: {
// 		id: REMOVE_ROOT_FOLDER_COMMAND_ID,
// 		title: REMOVE_ROOT_FOLDER_LABEL
// 	},
// 	when: ContextKeyExpr.and(ExplorerRootContext, ExplorerFolderContext)
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '7_modification',
// 	order: 10,
// 	command: {
// 		id: RENAME_ID,
// 		title: TRIGGER_RENAME_LABEL,
// 		precondition: ExplorerResourceNotReadonlyContext
// 	},
// 	when: ExplorerRootContext.toNegated()
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '7_modification',
// 	order: 20,
// 	command: {
// 		id: MOVE_FILE_TO_TRASH_ID,
// 		title: MOVE_FILE_TO_TRASH_LABEL,
// 		precondition: ExplorerResourceNotReadonlyContext
// 	},
// 	alt: {
// 		id: DELETE_FILE_ID,
// 		title: nls.localize('deleteFile', "Delete Permanently"),
// 		precondition: ExplorerResourceNotReadonlyContext
// 	},
// 	when: ContextKeyExpr.and(ExplorerRootContext.toNegated(), ExplorerResourceMoveableToTrash)
// });

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, {
// 	group: '7_modification',
// 	order: 20,
// 	command: {
// 		id: DELETE_FILE_ID,
// 		title: nls.localize('deleteFile', "Delete Permanently"),
// 		precondition: ExplorerResourceNotReadonlyContext
// 	},
// 	when: ContextKeyExpr.and(ExplorerRootContext.toNegated(), ExplorerResourceMoveableToTrash.toNegated())
// });

// Empty Editor Group Context Menu
// MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: NEW_UNTITLED_FILE_COMMAND_ID, title: nls.localize('newFile', "New File") }, group: '1_file', order: 10 });
// MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: 'workbench.action.quickOpen', title: nls.localize('openFile', "Open File...") }, group: '1_file', order: 20 });

// File menu

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '1_new',
// 	command: {
// 		id: NEW_UNTITLED_FILE_COMMAND_ID,
// 		title: nls.localize({ key: 'miNewFile', comment: ['&& denotes a mnemonic'] }, "&&New File")
// 	},
// 	order: 1
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '4_save',
// 	command: {
// 		id: SAVE_FILE_COMMAND_ID,
// 		title: nls.localize({ key: 'miSave', comment: ['&& denotes a mnemonic'] }, "&&Save"),
// 		precondition: ContextKeyExpr.or(ActiveEditorIsReadonlyContext.toNegated(), ContextKeyExpr.and(ExplorerViewletVisibleContext, SidebarFocusContext))
// 	},
// 	order: 1
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '4_save',
// 	command: {
// 		id: SAVE_FILE_AS_COMMAND_ID,
// 		title: nls.localize({ key: 'miSaveAs', comment: ['&& denotes a mnemonic'] }, "Save &&As..."),
// 		// ActiveEditorContext is not 100% correct, but we lack a context for indicating "Save As..." support
// 		precondition: ContextKeyExpr.or(ActiveEditorContext, ContextKeyExpr.and(ExplorerViewletVisibleContext, SidebarFocusContext))
// 	},
// 	order: 2
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '4_save',
// 	command: {
// 		id: SaveAllAction.ID,
// 		title: nls.localize({ key: 'miSaveAll', comment: ['&& denotes a mnemonic'] }, "Save A&&ll"),
// 		precondition: DirtyWorkingCopiesContext
// 	},
// 	order: 3
// });
/* End AGPL */

if (isMacintosh) {
	MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
		group: '2_open',
		command: {
			id: OpenFileFolderAction.ID,
			title: nls.localize({ key: 'miOpen', comment: ['&& denotes a mnemonic'] }, "&&Open...")
		},
		order: 1
	});
} else {
	MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
		group: '2_open',
		command: {
			id: OpenFileAction.ID,
			title: nls.localize({ key: 'miOpenFile', comment: ['&& denotes a mnemonic'] }, /* AGPL */"&&Open Assembly..."/* End AGPL */)
		},
		order: 1
	});

	/* AGPL */
	// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
	// 	group: '2_open',
	// 	command: {
	// 		id: OpenFolderAction.ID,
	// 		title: nls.localize({ key: 'miOpenFolder', comment: ['&& denotes a mnemonic'] }, "Open &&Folder...")
	// 	},
	// 	order: 2
	// });
	/* End AGPL */
}

/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '2_open',
// 	command: {
// 		id: OpenWorkspaceAction.ID,
// 		title: nls.localize({ key: 'miOpenWorkspace', comment: ['&& denotes a mnemonic'] }, "Open Wor&&kspace...")
// 	},
// 	order: 3
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '5_autosave',
// 	command: {
// 		id: ToggleAutoSaveAction.ID,
// 		title: nls.localize({ key: 'miAutoSave', comment: ['&& denotes a mnemonic'] }, "A&&uto Save"),
// 		toggled: ContextKeyExpr.notEquals('config.files.autoSave', 'off')
// 	},
// 	order: 1
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '6_close',
// 	command: {
// 		id: REVERT_FILE_COMMAND_ID,
// 		title: nls.localize({ key: 'miRevert', comment: ['&& denotes a mnemonic'] }, "Re&&vert File"),
// 		precondition: ContextKeyExpr.or(ActiveEditorIsReadonlyContext.toNegated(), ContextKeyExpr.and(ExplorerViewletVisibleContext, SidebarFocusContext))
// 	},
// 	order: 1
// });

// MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
// 	group: '6_close',
// 	command: {
// 		id: CLOSE_EDITOR_COMMAND_ID,
// 		/* AGPL */
// 		title: nls.localize({ key: 'miCloseEditor', comment: ['&& denotes a mnemonic'] }, "&&Close Code Viewer")
// 		/* End AGPL */
// 	},
// 	order: 2
// });
/* End AGPL */

// Go to menu

MenuRegistry.appendMenuItem(MenuId.MenubarGoMenu, {
	group: '3_global_nav',
	command: {
		id: 'workbench.action.quickOpen',
		title: nls.localize({ key: 'miGotoFile', comment: ['&& denotes a mnemonic'] }, "Go to &&File...")
	},
	order: 1
});
