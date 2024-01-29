/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Registry } from 'vs/platform/registry/common/platform';
import { localize } from 'vs/nls';
import { IEditorPaneRegistry, EditorPaneDescriptor } from 'vs/workbench/browser/editor';
import { IEditorFactoryRegistry, EditorExtensions } from 'vs/workbench/common/editor';
import {
	TextCompareEditorActiveContext, ActiveEditorPinnedContext, EditorGroupEditorsCountContext, ActiveEditorStickyContext, ActiveEditorAvailableEditorIdsContext,
	EditorPartMultipleEditorGroupsContext, ActiveEditorDirtyContext, ActiveEditorGroupLockedContext, ActiveEditorCanSplitInGroupContext, SideBySideEditorActiveContext,
	EditorTabsVisibleContext, ActiveEditorLastInGroupContext, EditorPartMaximizedEditorGroupContext, MultipleEditorGroupsContext, InEditorZenModeContext, IsAuxiliaryEditorPartContext
} from 'vs/workbench/common/contextkeys';
import { SideBySideEditorInput, SideBySideEditorInputSerializer } from 'vs/workbench/common/editor/sideBySideEditorInput';
import { TextResourceEditor } from 'vs/workbench/browser/parts/editor/textResourceEditor';
import { SideBySideEditor } from 'vs/workbench/browser/parts/editor/sideBySideEditor';
import { DiffEditorInput, DiffEditorInputSerializer } from 'vs/workbench/common/editor/diffEditorInput';
import { UntitledTextEditorInput } from 'vs/workbench/services/untitled/common/untitledTextEditorInput';
import { TextResourceEditorInput } from 'vs/workbench/common/editor/textResourceEditorInput';
import { TextDiffEditor } from 'vs/workbench/browser/parts/editor/textDiffEditor';
import { BinaryResourceDiffEditor } from 'vs/workbench/browser/parts/editor/binaryDiffEditor';
import { ChangeEncodingAction, ChangeEOLAction, ChangeLanguageAction, EditorStatusContribution } from 'vs/workbench/browser/parts/editor/editorStatus';
import { Categories } from 'vs/platform/action/common/actionCommonCategories';
import { MenuRegistry, MenuId, IMenuItem, registerAction2 } from 'vs/platform/actions/common/actions';
import { SyncDescriptor } from 'vs/platform/instantiation/common/descriptors';
import { KeyMod, KeyCode } from 'vs/base/common/keyCodes';
import {
	CloseEditorsInOtherGroupsAction, CloseAllEditorsAction, MoveGroupLeftAction, MoveGroupRightAction, SplitEditorAction, JoinTwoGroupsAction, RevertAndCloseEditorAction,
	NavigateBetweenGroupsAction, FocusActiveGroupAction, FocusFirstGroupAction, ResetGroupSizesAction, MinimizeOtherGroupsAction, FocusPreviousGroup, FocusNextGroup,
	CloseLeftEditorsInGroupAction, OpenNextEditor, OpenPreviousEditor, NavigateBackwardsAction, NavigateForwardAction, NavigatePreviousAction, ReopenClosedEditorAction,
	QuickAccessPreviousRecentlyUsedEditorInGroupAction, QuickAccessPreviousEditorFromHistoryAction, ShowAllEditorsByAppearanceAction, ClearEditorHistoryAction, MoveEditorRightInGroupAction, OpenNextEditorInGroup,
	OpenPreviousEditorInGroup, OpenNextRecentlyUsedEditorAction, OpenPreviousRecentlyUsedEditorAction, MoveEditorToPreviousGroupAction,
	MoveEditorToNextGroupAction, MoveEditorToFirstGroupAction, MoveEditorLeftInGroupAction, ClearRecentFilesAction, OpenLastEditorInGroup,
	ShowEditorsInActiveGroupByMostRecentlyUsedAction, MoveEditorToLastGroupAction, OpenFirstEditorInGroup, MoveGroupUpAction, MoveGroupDownAction, FocusLastGroupAction, SplitEditorLeftAction, SplitEditorRightAction,
	SplitEditorUpAction, SplitEditorDownAction, MoveEditorToLeftGroupAction, MoveEditorToRightGroupAction, MoveEditorToAboveGroupAction, MoveEditorToBelowGroupAction, CloseAllEditorGroupsAction,
	JoinAllGroupsAction, FocusLeftGroup, FocusAboveGroup, FocusRightGroup, FocusBelowGroup, EditorLayoutSingleAction, EditorLayoutTwoColumnsAction, EditorLayoutThreeColumnsAction, EditorLayoutTwoByTwoGridAction,
	EditorLayoutTwoRowsAction, EditorLayoutThreeRowsAction, EditorLayoutTwoColumnsBottomAction, EditorLayoutTwoRowsRightAction, NewEditorGroupLeftAction, NewEditorGroupRightAction,
	NewEditorGroupAboveAction, NewEditorGroupBelowAction, SplitEditorOrthogonalAction, CloseEditorInAllGroupsAction, NavigateToLastEditLocationAction, ToggleGroupSizesAction, ShowAllEditorsByMostRecentlyUsedAction,
	QuickAccessPreviousRecentlyUsedEditorAction, OpenPreviousRecentlyUsedEditorInGroupAction, OpenNextRecentlyUsedEditorInGroupAction, QuickAccessLeastRecentlyUsedEditorAction, QuickAccessLeastRecentlyUsedEditorInGroupAction,
	ReOpenInTextEditorAction, DuplicateGroupDownAction, DuplicateGroupLeftAction, DuplicateGroupRightAction, DuplicateGroupUpAction, ToggleEditorTypeAction, SplitEditorToAboveGroupAction, SplitEditorToBelowGroupAction,
	SplitEditorToFirstGroupAction, SplitEditorToLastGroupAction, SplitEditorToLeftGroupAction, SplitEditorToNextGroupAction, SplitEditorToPreviousGroupAction, SplitEditorToRightGroupAction, NavigateForwardInEditsAction,
	NavigateBackwardsInEditsAction, NavigateForwardInNavigationsAction, NavigateBackwardsInNavigationsAction, NavigatePreviousInNavigationsAction, NavigatePreviousInEditsAction, NavigateToLastNavigationLocationAction,
	MaximizeGroupHideSidebarAction, MoveEditorToNewWindowAction, CopyEditorToNewindowAction, RestoreEditorsToMainWindowAction, ToggleMaximizeEditorGroupAction, MinimizeOtherGroupsHideSidebarAction, CopyEditorGroupToNewWindowAction, MoveEditorGroupToNewWindowAction, NewEmptyEditorWindowAction
} from 'vs/workbench/browser/parts/editor/editorActions';
import {
	CLOSE_EDITORS_AND_GROUP_COMMAND_ID, CLOSE_EDITORS_IN_GROUP_COMMAND_ID, CLOSE_EDITORS_TO_THE_RIGHT_COMMAND_ID, CLOSE_EDITOR_COMMAND_ID, CLOSE_EDITOR_GROUP_COMMAND_ID, CLOSE_OTHER_EDITORS_IN_GROUP_COMMAND_ID,
	CLOSE_PINNED_EDITOR_COMMAND_ID, CLOSE_SAVED_EDITORS_COMMAND_ID, GOTO_NEXT_CHANGE, GOTO_PREVIOUS_CHANGE, KEEP_EDITOR_COMMAND_ID, PIN_EDITOR_COMMAND_ID, SHOW_EDITORS_IN_GROUP, SPLIT_EDITOR_DOWN, SPLIT_EDITOR_LEFT,
	SPLIT_EDITOR_RIGHT, SPLIT_EDITOR_UP, TOGGLE_DIFF_IGNORE_TRIM_WHITESPACE, TOGGLE_DIFF_SIDE_BY_SIDE, TOGGLE_KEEP_EDITORS_COMMAND_ID, UNPIN_EDITOR_COMMAND_ID, setup as registerEditorCommands, REOPEN_WITH_COMMAND_ID,
	TOGGLE_LOCK_GROUP_COMMAND_ID, UNLOCK_GROUP_COMMAND_ID, SPLIT_EDITOR_IN_GROUP, JOIN_EDITOR_IN_GROUP, FOCUS_FIRST_SIDE_EDITOR, FOCUS_SECOND_SIDE_EDITOR, TOGGLE_SPLIT_EDITOR_IN_GROUP_LAYOUT, LOCK_GROUP_COMMAND_ID, SPLIT_EDITOR, TOGGLE_MAXIMIZE_EDITOR_GROUP, MOVE_EDITOR_INTO_NEW_WINDOW_COMMAND_ID, COPY_EDITOR_INTO_NEW_WINDOW_COMMAND_ID, MOVE_EDITOR_GROUP_INTO_NEW_WINDOW_COMMAND_ID, COPY_EDITOR_GROUP_INTO_NEW_WINDOW_COMMAND_ID, NEW_EMPTY_EDITOR_WINDOW_COMMAND_ID
} from 'vs/workbench/browser/parts/editor/editorCommands';
import { inQuickPickContext, getQuickNavigateHandler } from 'vs/workbench/browser/quickaccess';
import { KeybindingsRegistry, KeybindingWeight } from 'vs/platform/keybinding/common/keybindingsRegistry';
import { ContextKeyExpr, ContextKeyExpression } from 'vs/platform/contextkey/common/contextkey';
import { isMacintosh } from 'vs/base/common/platform';
import { EditorContributionInstantiation, registerEditorContribution } from 'vs/editor/browser/editorExtensions';
import { FloatingEditorClickMenu } from 'vs/workbench/browser/codeeditor';
import { Extensions as WorkbenchExtensions, IWorkbenchContributionsRegistry } from 'vs/workbench/common/contributions';
import { LifecyclePhase } from 'vs/workbench/services/lifecycle/common/lifecycle';
import { EditorAutoSave } from 'vs/workbench/browser/parts/editor/editorAutoSave';
import { IQuickAccessRegistry, Extensions as QuickAccessExtensions } from 'vs/platform/quickinput/common/quickAccess';
import { ActiveGroupEditorsByMostRecentlyUsedQuickAccess, AllEditorsByAppearanceQuickAccess, AllEditorsByMostRecentlyUsedQuickAccess } from 'vs/workbench/browser/parts/editor/editorQuickAccess';
import { FileAccess } from 'vs/base/common/network';
import { Codicon } from 'vs/base/common/codicons';
import { registerIcon } from 'vs/platform/theme/common/iconRegistry';
import { UntitledTextEditorInputSerializer, UntitledTextEditorWorkingCopyEditorHandler } from 'vs/workbench/services/untitled/common/untitledTextEditorHandler';
import { DynamicEditorConfigurations } from 'vs/workbench/browser/parts/editor/editorConfiguration';
import { EditorActionsDefaultAction, EditorActionsTitleBarAction, HideEditorActionsAction, HideEditorTabsAction, ShowMultipleEditorTabsAction, ShowSingleEditorTabAction } from 'vs/workbench/browser/actions/layoutActions';
import { ICommandAction } from 'vs/platform/action/common/action';
import { EditorContextKeys } from 'vs/editor/common/editorContextKeys';

//#region Editor Registrations

Registry.as<IEditorPaneRegistry>(EditorExtensions.EditorPane).registerEditorPane(
	EditorPaneDescriptor.create(
		TextResourceEditor,
		TextResourceEditor.ID,
		/* AGPL */
		localize('textEditor', "Text Code Viewer"),
		/* End AGPL */
	),
	[
		new SyncDescriptor(UntitledTextEditorInput),
		new SyncDescriptor(TextResourceEditorInput)
	]
);

Registry.as<IEditorPaneRegistry>(EditorExtensions.EditorPane).registerEditorPane(
	EditorPaneDescriptor.create(
		TextDiffEditor,
		TextDiffEditor.ID,
		/* AGPL */
		localize('textDiffEditor', "Text Diff Code Viewer")
		/* End AGPL */
	),
	[
		new SyncDescriptor(DiffEditorInput)
	]
);

Registry.as<IEditorPaneRegistry>(EditorExtensions.EditorPane).registerEditorPane(
	EditorPaneDescriptor.create(
		BinaryResourceDiffEditor,
		BinaryResourceDiffEditor.ID,
		/* AGPL */
		localize('binaryDiffEditor', "Binary Diff Code Viewer")
		/* End AGPL */
	),
	[
		new SyncDescriptor(DiffEditorInput)
	]
);

Registry.as<IEditorPaneRegistry>(EditorExtensions.EditorPane).registerEditorPane(
	EditorPaneDescriptor.create(
		SideBySideEditor,
		SideBySideEditor.ID,
		/* AGPL */
		localize('sideBySideEditor', "Side by Side Code Viewer")
		/* End AGPL */
	),
	[
		new SyncDescriptor(SideBySideEditorInput)
	]
);

Registry.as<IEditorFactoryRegistry>(EditorExtensions.EditorFactory).registerEditorSerializer(UntitledTextEditorInput.ID, UntitledTextEditorInputSerializer);
Registry.as<IEditorFactoryRegistry>(EditorExtensions.EditorFactory).registerEditorSerializer(SideBySideEditorInput.ID, SideBySideEditorInputSerializer);
Registry.as<IEditorFactoryRegistry>(EditorExtensions.EditorFactory).registerEditorSerializer(DiffEditorInput.ID, DiffEditorInputSerializer);

//#endregion

//#region Workbench Contributions

Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench).registerWorkbenchContribution(EditorAutoSave, LifecyclePhase.Ready);
Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench).registerWorkbenchContribution(EditorStatusContribution, LifecyclePhase.Ready);
Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench).registerWorkbenchContribution(UntitledTextEditorWorkingCopyEditorHandler, LifecyclePhase.Ready);
Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench).registerWorkbenchContribution(DynamicEditorConfigurations, LifecyclePhase.Ready);

registerEditorContribution(FloatingEditorClickMenu.ID, FloatingEditorClickMenu, EditorContributionInstantiation.AfterFirstRender);
//#endregion

//#region Quick Access

const quickAccessRegistry = Registry.as<IQuickAccessRegistry>(QuickAccessExtensions.Quickaccess);
const editorPickerContextKey = 'inEditorsPicker';
const editorPickerContext = ContextKeyExpr.and(inQuickPickContext, ContextKeyExpr.has(editorPickerContextKey));

quickAccessRegistry.registerQuickAccessProvider({
	ctor: ActiveGroupEditorsByMostRecentlyUsedQuickAccess,
	prefix: ActiveGroupEditorsByMostRecentlyUsedQuickAccess.PREFIX,
	contextKey: editorPickerContextKey,
	placeholder: localize('editorQuickAccessPlaceholder', "Type the name of an editor to open it."),
	helpEntries: [{ description: localize('activeGroupEditorsByMostRecentlyUsedQuickAccess', "Show Editors in Active Group by Most Recently Used"), commandId: ShowEditorsInActiveGroupByMostRecentlyUsedAction.ID }]
});

quickAccessRegistry.registerQuickAccessProvider({
	ctor: AllEditorsByAppearanceQuickAccess,
	prefix: AllEditorsByAppearanceQuickAccess.PREFIX,
	contextKey: editorPickerContextKey,
	placeholder: localize('editorQuickAccessPlaceholder', "Type the name of an editor to open it."),
	helpEntries: [{ description: localize('allEditorsByAppearanceQuickAccess', "Show All Opened Editors By Appearance"), commandId: ShowAllEditorsByAppearanceAction.ID }]
});

quickAccessRegistry.registerQuickAccessProvider({
	ctor: AllEditorsByMostRecentlyUsedQuickAccess,
	prefix: AllEditorsByMostRecentlyUsedQuickAccess.PREFIX,
	contextKey: editorPickerContextKey,
	placeholder: localize('editorQuickAccessPlaceholder', "Type the name of an editor to open it."),
	helpEntries: [{ description: localize('allEditorsByMostRecentlyUsedQuickAccess', "Show All Opened Editors By Most Recently Used"), commandId: ShowAllEditorsByMostRecentlyUsedAction.ID }]
});

//#endregion

//#region Actions & Commands

registerAction2(ChangeLanguageAction);
registerAction2(ChangeEOLAction);
registerAction2(ChangeEncodingAction);

registerAction2(NavigateForwardAction);
registerAction2(NavigateBackwardsAction);

registerAction2(OpenNextEditor);
registerAction2(OpenPreviousEditor);
registerAction2(OpenNextEditorInGroup);
registerAction2(OpenPreviousEditorInGroup);
registerAction2(OpenFirstEditorInGroup);
registerAction2(OpenLastEditorInGroup);

registerAction2(OpenNextRecentlyUsedEditorAction);
registerAction2(OpenPreviousRecentlyUsedEditorAction);
registerAction2(OpenNextRecentlyUsedEditorInGroupAction);
registerAction2(OpenPreviousRecentlyUsedEditorInGroupAction);

registerAction2(ReopenClosedEditorAction);
registerAction2(ClearRecentFilesAction);

registerAction2(ShowAllEditorsByAppearanceAction);
registerAction2(ShowAllEditorsByMostRecentlyUsedAction);
registerAction2(ShowEditorsInActiveGroupByMostRecentlyUsedAction);

registerAction2(CloseAllEditorsAction);
registerAction2(CloseAllEditorGroupsAction);
registerAction2(CloseLeftEditorsInGroupAction);
registerAction2(CloseEditorsInOtherGroupsAction);
registerAction2(CloseEditorInAllGroupsAction);
/* AGPL */
// registerAction2(RevertAndCloseEditorAction);
/* End AGPL */

registerAction2(SplitEditorAction);
registerAction2(SplitEditorOrthogonalAction);

registerAction2(SplitEditorLeftAction);
registerAction2(SplitEditorRightAction);
registerAction2(SplitEditorUpAction);
registerAction2(SplitEditorDownAction);

registerAction2(JoinTwoGroupsAction);
registerAction2(JoinAllGroupsAction);

registerAction2(NavigateBetweenGroupsAction);

registerAction2(ResetGroupSizesAction);
registerAction2(ToggleGroupSizesAction);
registerAction2(MaximizeGroupHideSidebarAction);
registerAction2(ToggleMaximizeEditorGroupAction);
registerAction2(MinimizeOtherGroupsAction);
registerAction2(MinimizeOtherGroupsHideSidebarAction);

registerAction2(MoveEditorLeftInGroupAction);
registerAction2(MoveEditorRightInGroupAction);

registerAction2(MoveGroupLeftAction);
registerAction2(MoveGroupRightAction);
registerAction2(MoveGroupUpAction);
registerAction2(MoveGroupDownAction);

registerAction2(DuplicateGroupLeftAction);
registerAction2(DuplicateGroupRightAction);
registerAction2(DuplicateGroupUpAction);
registerAction2(DuplicateGroupDownAction);

registerAction2(MoveEditorToPreviousGroupAction);
registerAction2(MoveEditorToNextGroupAction);
registerAction2(MoveEditorToFirstGroupAction);
registerAction2(MoveEditorToLastGroupAction);
registerAction2(MoveEditorToLeftGroupAction);
registerAction2(MoveEditorToRightGroupAction);
registerAction2(MoveEditorToAboveGroupAction);
registerAction2(MoveEditorToBelowGroupAction);

registerAction2(SplitEditorToPreviousGroupAction);
registerAction2(SplitEditorToNextGroupAction);
registerAction2(SplitEditorToFirstGroupAction);
registerAction2(SplitEditorToLastGroupAction);
registerAction2(SplitEditorToLeftGroupAction);
registerAction2(SplitEditorToRightGroupAction);
registerAction2(SplitEditorToAboveGroupAction);
registerAction2(SplitEditorToBelowGroupAction);

registerAction2(FocusActiveGroupAction);
registerAction2(FocusFirstGroupAction);
registerAction2(FocusLastGroupAction);
registerAction2(FocusPreviousGroup);
registerAction2(FocusNextGroup);
registerAction2(FocusLeftGroup);
registerAction2(FocusRightGroup);
registerAction2(FocusAboveGroup);
registerAction2(FocusBelowGroup);

registerAction2(NewEditorGroupLeftAction);
registerAction2(NewEditorGroupRightAction);
registerAction2(NewEditorGroupAboveAction);
registerAction2(NewEditorGroupBelowAction);

registerAction2(NavigatePreviousAction);
registerAction2(NavigateForwardInEditsAction);
registerAction2(NavigateBackwardsInEditsAction);
registerAction2(NavigatePreviousInEditsAction);
registerAction2(NavigateToLastEditLocationAction);
registerAction2(NavigateForwardInNavigationsAction);
registerAction2(NavigateBackwardsInNavigationsAction);
registerAction2(NavigatePreviousInNavigationsAction);
registerAction2(NavigateToLastNavigationLocationAction);
/* AGPL */
//registerAction2(ClearEditorHistoryAction);
/* End AGPL */

registerAction2(EditorLayoutSingleAction);
registerAction2(EditorLayoutTwoColumnsAction);
registerAction2(EditorLayoutThreeColumnsAction);
registerAction2(EditorLayoutTwoRowsAction);
registerAction2(EditorLayoutThreeRowsAction);
registerAction2(EditorLayoutTwoByTwoGridAction);
registerAction2(EditorLayoutTwoRowsRightAction);
registerAction2(EditorLayoutTwoColumnsBottomAction);

registerAction2(ToggleEditorTypeAction);
registerAction2(ReOpenInTextEditorAction);

registerAction2(QuickAccessPreviousRecentlyUsedEditorAction);
registerAction2(QuickAccessLeastRecentlyUsedEditorAction);
registerAction2(QuickAccessPreviousRecentlyUsedEditorInGroupAction);
registerAction2(QuickAccessLeastRecentlyUsedEditorInGroupAction);
/* AGPL */
// registerAction2(QuickAccessPreviousEditorFromHistoryAction);
/* End AGPL */

registerAction2(MoveEditorToNewWindowAction);
registerAction2(CopyEditorToNewindowAction);
registerAction2(MoveEditorGroupToNewWindowAction);
registerAction2(CopyEditorGroupToNewWindowAction);
registerAction2(RestoreEditorsToMainWindowAction);
registerAction2(NewEmptyEditorWindowAction);

const quickAccessNavigateNextInEditorPickerId = 'workbench.action.quickOpenNavigateNextInEditorPicker';
KeybindingsRegistry.registerCommandAndKeybindingRule({
	id: quickAccessNavigateNextInEditorPickerId,
	weight: KeybindingWeight.WorkbenchContrib + 50,
	handler: getQuickNavigateHandler(quickAccessNavigateNextInEditorPickerId, true),
	when: editorPickerContext,
	primary: KeyMod.CtrlCmd | KeyCode.Tab,
	mac: { primary: KeyMod.WinCtrl | KeyCode.Tab }
});

const quickAccessNavigatePreviousInEditorPickerId = 'workbench.action.quickOpenNavigatePreviousInEditorPicker';
KeybindingsRegistry.registerCommandAndKeybindingRule({
	id: quickAccessNavigatePreviousInEditorPickerId,
	weight: KeybindingWeight.WorkbenchContrib + 50,
	handler: getQuickNavigateHandler(quickAccessNavigatePreviousInEditorPickerId, false),
	when: editorPickerContext,
	primary: KeyMod.CtrlCmd | KeyMod.Shift | KeyCode.Tab,
	mac: { primary: KeyMod.WinCtrl | KeyMod.Shift | KeyCode.Tab }
});

registerEditorCommands();

//#endregion

//#region Menus

// macOS: Touchbar
if (isMacintosh) {
	MenuRegistry.appendMenuItem(MenuId.TouchBarContext, {
		command: { id: NavigateBackwardsAction.ID, title: NavigateBackwardsAction.LABEL, icon: { dark: FileAccess.asFileUri('vs/workbench/browser/parts/editor/media/back-tb.png') } },
		group: 'navigation',
		order: 0
	});

	MenuRegistry.appendMenuItem(MenuId.TouchBarContext, {
		command: { id: NavigateForwardAction.ID, title: NavigateForwardAction.LABEL, icon: { dark: FileAccess.asFileUri('vs/workbench/browser/parts/editor/media/forward-tb.png') } },
		group: 'navigation',
		order: 1
	});
}

// Empty Editor Group Toolbar
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroup, { command: { id: LOCK_GROUP_COMMAND_ID, title: localize('lockGroupAction', "Lock Group"), icon: Codicon.unlock }, group: 'navigation', order: 10, when: ContextKeyExpr.and(IsAuxiliaryEditorPartContext, ActiveEditorGroupLockedContext.toNegated()) });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroup, { command: { id: UNLOCK_GROUP_COMMAND_ID, title: localize('unlockGroupAction', "Unlock Group"), icon: Codicon.lock, toggled: ContextKeyExpr.true() }, group: 'navigation', order: 10, when: ActiveEditorGroupLockedContext });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroup, { command: { id: CLOSE_EDITOR_GROUP_COMMAND_ID, title: localize('closeGroupAction', "Close Group"), icon: Codicon.close }, group: 'navigation', order: 20 });

// Empty Editor Group Context Menu
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: SPLIT_EDITOR_UP, title: localize('splitUp', "Split Up") }, group: '2_split', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: SPLIT_EDITOR_DOWN, title: localize('splitDown', "Split Down") }, group: '2_split', order: 20 });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: SPLIT_EDITOR_LEFT, title: localize('splitLeft', "Split Left") }, group: '2_split', order: 30 });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: SPLIT_EDITOR_RIGHT, title: localize('splitRight', "Split Right") }, group: '2_split', order: 40 });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: NEW_EMPTY_EDITOR_WINDOW_COMMAND_ID, title: localize('newWindow', "New Window") }, group: '3_window', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: TOGGLE_LOCK_GROUP_COMMAND_ID, title: localize('toggleLockGroup', "Lock Group"), toggled: ActiveEditorGroupLockedContext }, group: '4_lock', order: 10, when: ContextKeyExpr.and(MultipleEditorGroupsContext, IsAuxiliaryEditorPartContext.toNegated()) });
MenuRegistry.appendMenuItem(MenuId.EmptyEditorGroupContext, { command: { id: CLOSE_EDITOR_GROUP_COMMAND_ID, title: localize('close', "Close") }, group: '5_close', order: 10, when: MultipleEditorGroupsContext });

// Editor Tab Container Context Menu
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { command: { id: SPLIT_EDITOR_UP, title: localize('splitUp', "Split Up") }, group: '2_split', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { command: { id: SPLIT_EDITOR_DOWN, title: localize('splitDown', "Split Down") }, group: '2_split', order: 20 });
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { command: { id: SPLIT_EDITOR_LEFT, title: localize('splitLeft', "Split Left") }, group: '2_split', order: 30 });
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { command: { id: SPLIT_EDITOR_RIGHT, title: localize('splitRight', "Split Right") }, group: '2_split', order: 40 });

MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { command: { id: MOVE_EDITOR_GROUP_INTO_NEW_WINDOW_COMMAND_ID, title: localize('moveEditorGroupToNewWindow', "Move into New Window") }, group: '3_window', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { command: { id: COPY_EDITOR_GROUP_INTO_NEW_WINDOW_COMMAND_ID, title: localize('copyEditorGroupToNewWindow', "Copy into New Window") }, group: '3_window', order: 20 });

MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { submenu: MenuId.EditorTabsBarShowTabsSubmenu, title: localize('tabBar', "Tab Bar"), group: '4_config', order: 10, when: InEditorZenModeContext.negate() });
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarShowTabsSubmenu, { command: { id: ShowMultipleEditorTabsAction.ID, title: localize('multipleTabs', "Multiple Tabs"), toggled: ContextKeyExpr.equals('config.workbench.editor.showTabs', 'multiple') }, group: '1_config', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarShowTabsSubmenu, { command: { id: ShowSingleEditorTabAction.ID, title: localize('singleTab', "Single Tab"), toggled: ContextKeyExpr.equals('config.workbench.editor.showTabs', 'single') }, group: '1_config', order: 20 });
MenuRegistry.appendMenuItem(MenuId.EditorTabsBarShowTabsSubmenu, { command: { id: HideEditorTabsAction.ID, title: localize('hideTabs', "Hidden"), toggled: ContextKeyExpr.equals('config.workbench.editor.showTabs', 'none') }, group: '1_config', order: 30 });

MenuRegistry.appendMenuItem(MenuId.EditorTabsBarContext, { submenu: MenuId.EditorActionsPositionSubmenu, title: localize('editorActionsPosition', "Editor Actions Position"), group: '4_config', order: 20 });
MenuRegistry.appendMenuItem(MenuId.EditorActionsPositionSubmenu, { command: { id: EditorActionsDefaultAction.ID, title: localize('tabBar', "Tab Bar"), toggled: ContextKeyExpr.equals('config.workbench.editor.editorActionsLocation', 'default') }, group: '1_config', order: 10, when: ContextKeyExpr.equals('config.workbench.editor.showTabs', 'none').negate() });
MenuRegistry.appendMenuItem(MenuId.EditorActionsPositionSubmenu, { command: { id: EditorActionsTitleBarAction.ID, title: localize('titleBar', "Title Bar"), toggled: ContextKeyExpr.or(ContextKeyExpr.equals('config.workbench.editor.editorActionsLocation', 'titleBar'), ContextKeyExpr.and(ContextKeyExpr.equals('config.workbench.editor.showTabs', 'none'), ContextKeyExpr.equals('config.workbench.editor.editorActionsLocation', 'default'))) }, group: '1_config', order: 20, when: ContextKeyExpr.equals('config.window.titleBarStyle', 'native').negate() });
MenuRegistry.appendMenuItem(MenuId.EditorActionsPositionSubmenu, { command: { id: HideEditorActionsAction.ID, title: localize('hidden', "Hidden"), toggled: ContextKeyExpr.or(ContextKeyExpr.equals('config.workbench.editor.editorActionsLocation', 'hidden'), ContextKeyExpr.and(ContextKeyExpr.equals('config.window.titleBarStyle', 'native'), ContextKeyExpr.equals('config.workbench.editor.editorActionsLocation', 'titleBar'))) }, group: '1_config', order: 30 });

// Editor Title Context Menu
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: CLOSE_EDITOR_COMMAND_ID, title: localize('close', "Close") }, group: '1_close', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: CLOSE_OTHER_EDITORS_IN_GROUP_COMMAND_ID, title: localize('closeOthers', "Close Others"), precondition: EditorGroupEditorsCountContext.notEqualsTo('1') }, group: '1_close', order: 20 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: CLOSE_EDITORS_TO_THE_RIGHT_COMMAND_ID, title: localize('closeRight', "Close to the Right"), precondition: ActiveEditorLastInGroupContext.toNegated() }, group: '1_close', order: 30, when: EditorTabsVisibleContext });
/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: CLOSE_SAVED_EDITORS_COMMAND_ID, title: localize('closeAllSaved', "Close Saved") }, group: '1_close', order: 40 });
/* End AGPL */
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: CLOSE_EDITORS_IN_GROUP_COMMAND_ID, title: localize('closeAll', "Close All") }, group: '1_close', order: 50 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: REOPEN_WITH_COMMAND_ID, title: localize('reopenWith', "Reopen Editor With...") }, group: '1_open', order: 10, when: ActiveEditorAvailableEditorIdsContext });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: KEEP_EDITOR_COMMAND_ID, title: localize('keepOpen', "Keep Open"), precondition: ActiveEditorPinnedContext.toNegated() }, group: '3_preview', order: 10, when: ContextKeyExpr.has('config.workbench.editor.enablePreview') });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: PIN_EDITOR_COMMAND_ID, title: localize('pin', "Pin") }, group: '3_preview', order: 20, when: ActiveEditorStickyContext.toNegated() });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: UNPIN_EDITOR_COMMAND_ID, title: localize('unpin', "Unpin") }, group: '3_preview', order: 20, when: ActiveEditorStickyContext });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: SPLIT_EDITOR_UP, title: localize('splitUp', "Split Up") }, group: '5_split', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: SPLIT_EDITOR_DOWN, title: localize('splitDown', "Split Down") }, group: '5_split', order: 20 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: SPLIT_EDITOR_LEFT, title: localize('splitLeft', "Split Left") }, group: '5_split', order: 30 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: SPLIT_EDITOR_RIGHT, title: localize('splitRight', "Split Right") }, group: '5_split', order: 40 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: SPLIT_EDITOR_IN_GROUP, title: localize('splitInGroup', "Split in Group") }, group: '5_split', order: 50, when: ActiveEditorCanSplitInGroupContext });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: JOIN_EDITOR_IN_GROUP, title: localize('joinInGroup', "Join in Group") }, group: '5_split', order: 50, when: SideBySideEditorActiveContext });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: MOVE_EDITOR_INTO_NEW_WINDOW_COMMAND_ID, title: localize('moveToNewWindow', "Move into New Window") }, group: '6_new_window', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EditorTitleContext, { command: { id: COPY_EDITOR_INTO_NEW_WINDOW_COMMAND_ID, title: localize('copyToNewWindow', "Copy into New Window") }, group: '6_new_window', order: 20 });

// Editor Title Menu
MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: TOGGLE_DIFF_SIDE_BY_SIDE, title: localize('inlineView', "Inline View"), toggled: ContextKeyExpr.equals('config.diffEditor.renderSideBySide', false) }, group: '1_diff', order: 10, when: ContextKeyExpr.has('isInDiffEditor') });
/* AGPL */
MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: SHOW_EDITORS_IN_GROUP, title: localize('showOpenedEditors', "Show Opened Types") }, group: '3_open', order: 10 });
/* End AGPL */
MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: CLOSE_EDITORS_IN_GROUP_COMMAND_ID, title: localize('closeAll', "Close All") }, group: '5_close', order: 10 });
/* AGPL */
// MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: CLOSE_SAVED_EDITORS_COMMAND_ID, title: localize('closeAllSaved', "Close Saved") }, group: '5_close', order: 20 });
/* End AGPL */
MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: TOGGLE_KEEP_EDITORS_COMMAND_ID, title: localize('togglePreviewMode', "Enable Preview Editors"), toggled: ContextKeyExpr.has('config.workbench.editor.enablePreview') }, group: '7_settings', order: 10 });
MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: TOGGLE_MAXIMIZE_EDITOR_GROUP, title: localize('maximizeGroup', "Maximize Group") }, group: '8_group_operations', order: 5, when: ContextKeyExpr.and(EditorPartMaximizedEditorGroupContext.negate(), EditorPartMultipleEditorGroupsContext) });
MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: TOGGLE_MAXIMIZE_EDITOR_GROUP, title: localize('unmaximizeGroup', "Unmaximize Group") }, group: '8_group_operations', order: 5, when: EditorPartMaximizedEditorGroupContext });
MenuRegistry.appendMenuItem(MenuId.EditorTitle, { command: { id: TOGGLE_LOCK_GROUP_COMMAND_ID, title: localize('lockGroup', "Lock Group"), toggled: ActiveEditorGroupLockedContext }, group: '8_group_operations', order: 10, when: ContextKeyExpr.and(MultipleEditorGroupsContext, IsAuxiliaryEditorPartContext.toNegated()) });

function appendEditorToolItem(primary: ICommandAction, when: ContextKeyExpression | undefined, order: number, alternative?: ICommandAction, precondition?: ContextKeyExpression | undefined): void {
	const item: IMenuItem = {
		command: {
			id: primary.id,
			title: primary.title,
			icon: primary.icon,
			toggled: primary.toggled,
			precondition
		},
		group: 'navigation',
		when,
		order
	};

	if (alternative) {
		item.alt = {
			id: alternative.id,
			title: alternative.title,
			icon: alternative.icon
		};
	}

	MenuRegistry.appendMenuItem(MenuId.EditorTitle, item);
}

const SPLIT_ORDER = 100000;  // towards the end
const CLOSE_ORDER = 1000000; // towards the far end

// Editor Title Menu: Split Editor
appendEditorToolItem(
	{
		id: SPLIT_EDITOR,
		/* AGPL */
		title: localize('splitEditorRight', "Split Right"),
		/* End AGPL */
		icon: Codicon.splitHorizontal
	},
	ContextKeyExpr.not('splitEditorsVertically'),
	SPLIT_ORDER,
	{
		id: SPLIT_EDITOR_DOWN,
		/* AGPL */
		title: localize('splitEditorDown', "Split Down"),
		/* End AGPL */
		icon: Codicon.splitVertical
	}
);

appendEditorToolItem(
	{
		id: SPLIT_EDITOR,
		/* AGPL */
		title: localize('splitEditorDown', "Split Down"),
		/* End AGPL */
		icon: Codicon.splitVertical
	},
	ContextKeyExpr.has('splitEditorsVertically'),
	SPLIT_ORDER,
	{
		id: SPLIT_EDITOR_RIGHT,
		/* AGPL */
		title: localize('splitEditorRight', "Split Right"),
		/* End AGPL */
		icon: Codicon.splitHorizontal
	}
);

// Side by side: layout
appendEditorToolItem(
	{
		id: TOGGLE_SPLIT_EDITOR_IN_GROUP_LAYOUT,
		title: localize('toggleSplitEditorInGroupLayout', "Toggle Layout"),
		icon: Codicon.editorLayout
	},
	SideBySideEditorActiveContext,
	SPLIT_ORDER - 1, // left to split actions
);

// Editor Title Menu: Close (tabs disabled, normal editor)
appendEditorToolItem(
	{
		id: CLOSE_EDITOR_COMMAND_ID,
		title: localize('close', "Close"),
		icon: Codicon.close
	},
	ContextKeyExpr.and(EditorTabsVisibleContext.toNegated(), ActiveEditorDirtyContext.toNegated(), ActiveEditorStickyContext.toNegated()),
	CLOSE_ORDER,
	{
		id: CLOSE_EDITORS_IN_GROUP_COMMAND_ID,
		title: localize('closeAll', "Close All"),
		icon: Codicon.closeAll
	}
);

// Editor Title Menu: Close (tabs disabled, dirty editor)
appendEditorToolItem(
	{
		id: CLOSE_EDITOR_COMMAND_ID,
		title: localize('close', "Close"),
		icon: Codicon.closeDirty
	},
	ContextKeyExpr.and(EditorTabsVisibleContext.toNegated(), ActiveEditorDirtyContext, ActiveEditorStickyContext.toNegated()),
	CLOSE_ORDER,
	{
		id: CLOSE_EDITORS_IN_GROUP_COMMAND_ID,
		title: localize('closeAll', "Close All"),
		icon: Codicon.closeAll
	}
);

// Editor Title Menu: Close (tabs disabled, sticky editor)
appendEditorToolItem(
	{
		id: UNPIN_EDITOR_COMMAND_ID,
		title: localize('unpin', "Unpin"),
		icon: Codicon.pinned
	},
	ContextKeyExpr.and(EditorTabsVisibleContext.toNegated(), ActiveEditorDirtyContext.toNegated(), ActiveEditorStickyContext),
	CLOSE_ORDER,
	{
		id: CLOSE_EDITOR_COMMAND_ID,
		title: localize('close', "Close"),
		icon: Codicon.close
	}
);

// Editor Title Menu: Close (tabs disabled, dirty & sticky editor)
appendEditorToolItem(
	{
		id: UNPIN_EDITOR_COMMAND_ID,
		title: localize('unpin', "Unpin"),
		icon: Codicon.pinnedDirty
	},
	ContextKeyExpr.and(EditorTabsVisibleContext.toNegated(), ActiveEditorDirtyContext, ActiveEditorStickyContext),
	CLOSE_ORDER,
	{
		id: CLOSE_EDITOR_COMMAND_ID,
		title: localize('close', "Close"),
		icon: Codicon.close
	}
);

// Lock Group: only on auxiliary window and when group is unlocked
appendEditorToolItem(
	{
		id: LOCK_GROUP_COMMAND_ID,
		title: localize('lockEditorGroup', "Lock Group"),
		icon: Codicon.unlock
	},
	ContextKeyExpr.and(IsAuxiliaryEditorPartContext, ActiveEditorGroupLockedContext.toNegated()),
	CLOSE_ORDER - 1, // immediately to the left of close action
);

// Unlock Group: only when group is locked
appendEditorToolItem(
	{
		id: UNLOCK_GROUP_COMMAND_ID,
		title: localize('unlockEditorGroup', "Unlock Group"),
		icon: Codicon.lock,
		toggled: ContextKeyExpr.true()
	},
	ActiveEditorGroupLockedContext,
	CLOSE_ORDER - 1, // immediately to the left of close action
);

const previousChangeIcon = registerIcon('diff-editor-previous-change', Codicon.arrowUp, localize('previousChangeIcon', 'Icon for the previous change action in the diff editor.'));
const nextChangeIcon = registerIcon('diff-editor-next-change', Codicon.arrowDown, localize('nextChangeIcon', 'Icon for the next change action in the diff editor.'));
const toggleWhitespace = registerIcon('diff-editor-toggle-whitespace', Codicon.whitespace, localize('toggleWhitespace', 'Icon for the toggle whitespace action in the diff editor.'));

// Diff Editor Title Menu: Previous Change
appendEditorToolItem(
	{
		id: GOTO_PREVIOUS_CHANGE,
		title: localize('navigate.prev.label', "Previous Change"),
		icon: previousChangeIcon
	},
	TextCompareEditorActiveContext,
	10,
	undefined,
	EditorContextKeys.hasChanges,
);

// Diff Editor Title Menu: Next Change
appendEditorToolItem(
	{
		id: GOTO_NEXT_CHANGE,
		title: localize('navigate.next.label', "Next Change"),
		icon: nextChangeIcon
	},
	TextCompareEditorActiveContext,
	11,
	undefined,
	EditorContextKeys.hasChanges,
);

MenuRegistry.appendMenuItem(MenuId.EditorTitle, {
	command: {
		id: TOGGLE_DIFF_IGNORE_TRIM_WHITESPACE,
		title: localize('ignoreTrimWhitespace.label', "Show Leading/Trailing Whitespace Differences"),
		icon: toggleWhitespace,
		precondition: TextCompareEditorActiveContext,
		toggled: ContextKeyExpr.equals('config.diffEditor.ignoreTrimWhitespace', false),
	},
	group: 'navigation',
	when: TextCompareEditorActiveContext,
	order: 20,
});

// Editor Commands for Command Palette
/* AGPL */
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: KEEP_EDITOR_COMMAND_ID, title: { value: localize('keepEditor', "Keep"), original: 'Keep' }, category: Categories.View }, when: ContextKeyExpr.has('config.workbench.editor.enablePreview') });
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: PIN_EDITOR_COMMAND_ID, title: { value: localize('pinEditor', "Pin"), original: 'Pin' }, category: Categories.View } });
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: UNPIN_EDITOR_COMMAND_ID, title: { value: localize('unpinEditor', "Unpin"), original: 'Unpin' }, category: Categories.View } });
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: CLOSE_EDITOR_COMMAND_ID, title: { value: localize('closeEditor', "Close"), original: 'Close' }, category: Categories.View } });
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: CLOSE_PINNED_EDITOR_COMMAND_ID, title: { value: localize('closePinnedEditor', "Close Pinned"), original: 'Close Pinned' }, category: Categories.View } });
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: CLOSE_EDITORS_IN_GROUP_COMMAND_ID, title: { value: localize('closeEditorsInGroup', "Close All in Group"), original: 'Close All in Group' }, category: Categories.View } });
// MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: CLOSE_SAVED_EDITORS_COMMAND_ID, title: { value: localize('closeSavedEditors', "Close Saved Editors in Group"), original: 'Close Saved Editors in Group' }, category: Categories.View } });

MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: CLOSE_OTHER_EDITORS_IN_GROUP_COMMAND_ID, title: { value: localize('closeOtherEditors', "Close Other in Group"), original: 'Close Other in Group' }, category: Categories.View } });
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: CLOSE_EDITORS_TO_THE_RIGHT_COMMAND_ID, title: { value: localize('closeRightEditors', "Close to the Right in Group"), original: 'Close to the Right in Group' }, category: Categories.View }, when: ActiveEditorLastInGroupContext.toNegated() });
MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: CLOSE_EDITORS_AND_GROUP_COMMAND_ID, title: { value: localize('closeEditorGroup', "Close Group"), original: 'Close Group' }, category: Categories.View }, when: MultipleEditorGroupsContext });
// MenuRegistry.appendMenuItem(MenuId.CommandPalette, { command: { id: REOPEN_WITH_COMMAND_ID, title: { value: localize('reopenWith', "Reopen Editor With..."), original: 'Reopen Editor With...' }, category: Categories.View }, when: ActiveEditorAvailableEditorIdsContext });
/* End AGPL */

// File menu
MenuRegistry.appendMenuItem(MenuId.MenubarRecentMenu, {
	group: '1_editor',
	command: {
		id: ReopenClosedEditorAction.ID,
		/* AGPL */
		title: localize({ key: 'miReopenClosedEditor', comment: ['&& denotes a mnemonic'] }, "&&Reopen Closed Type"),
		/* End AGPL */
		precondition: ContextKeyExpr.has('canReopenClosedEditor')
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarRecentMenu, {
	group: 'z_clear',
	command: {
		id: ClearRecentFilesAction.ID,
		title: localize({ key: 'miClearRecentOpen', comment: ['&& denotes a mnemonic'] }, "&&Clear Recently Opened...")
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarFileMenu, {
	title: localize('miShare', "Share"),
	submenu: MenuId.MenubarShare,
	group: '45_share',
	order: 1,
});

// Layout menu
MenuRegistry.appendMenuItem(MenuId.MenubarViewMenu, {
	group: '2_appearance',
	/* AGPL */
	title: localize({ key: 'miEditorLayout', comment: ['&& denotes a mnemonic'] }, "Code Viewer &&Layout"),
	/* End AGPL */
	submenu: MenuId.MenubarLayoutMenu,
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '1_split',
	command: {
		id: SPLIT_EDITOR_UP,
		title: {
			original: 'Split Up',
			value: localize('miSplitEditorUpWithoutMnemonic', "Split Up"),
			mnemonicTitle: localize({ key: 'miSplitEditorUp', comment: ['&& denotes a mnemonic'] }, "Split &&Up"),
		}
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '1_split',
	command: {
		id: SPLIT_EDITOR_DOWN,
		title: {
			original: 'Split Down',
			value: localize('miSplitEditorDownWithoutMnemonic', "Split Down"),
			mnemonicTitle: localize({ key: 'miSplitEditorDown', comment: ['&& denotes a mnemonic'] }, "Split &&Down")
		}
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '1_split',
	command: {
		id: SPLIT_EDITOR_LEFT,
		title: {
			original: 'Split Left',
			value: localize('miSplitEditorLeftWithoutMnemonic', "Split Left"),
			mnemonicTitle: localize({ key: 'miSplitEditorLeft', comment: ['&& denotes a mnemonic'] }, "Split &&Left")
		}
	},
	order: 3
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '1_split',
	command: {
		id: SPLIT_EDITOR_RIGHT,
		title: {
			original: 'Split Right',
			value: localize('miSplitEditorRightWithoutMnemonic', "Split Right"),
			mnemonicTitle: localize({ key: 'miSplitEditorRight', comment: ['&& denotes a mnemonic'] }, "Split &&Right")
		}
	},
	order: 4
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '2_split_in_group',
	command: {
		id: SPLIT_EDITOR_IN_GROUP,
		title: {
			original: 'Split in Group',
			value: localize('miSplitEditorInGroupWithoutMnemonic', "Split in Group"),
			mnemonicTitle: localize({ key: 'miSplitEditorInGroup', comment: ['&& denotes a mnemonic'] }, "Split in &&Group")
		}
	},
	when: ActiveEditorCanSplitInGroupContext,
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '2_split_in_group',
	command: {
		id: JOIN_EDITOR_IN_GROUP,
		title: {
			original: 'Join in Group',
			value: localize('miJoinEditorInGroupWithoutMnemonic', "Join in Group"),
			mnemonicTitle: localize({ key: 'miJoinEditorInGroup', comment: ['&& denotes a mnemonic'] }, "Join in &&Group")
		}
	},
	when: SideBySideEditorActiveContext,
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '3_new_window',
	command: {
		id: MOVE_EDITOR_INTO_NEW_WINDOW_COMMAND_ID,
		title: {
			original: 'Move Editor into New Window',
			value: localize('moveEditorToNewWindow', "Move Editor into New Window"),
			mnemonicTitle: localize({ key: 'miMoveEditorToNewWindow', comment: ['&& denotes a mnemonic'] }, "&&Move Editor into New Window")
		}
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '3_new_window',
	command: {
		id: COPY_EDITOR_INTO_NEW_WINDOW_COMMAND_ID,
		title: {
			original: 'Copy Editor into New Window',
			value: localize('copyEditorToNewWindow', "Copy Editor into New Window"),
			mnemonicTitle: localize({ key: 'miCopyEditorToNewWindow', comment: ['&& denotes a mnemonic'] }, "&&Copy Editor into New Window")
		}
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutSingleAction.ID,
		title: {
			original: 'Single',
			value: localize('miSingleColumnEditorLayoutWithoutMnemonic', "Single"),
			mnemonicTitle: localize({ key: 'miSingleColumnEditorLayout', comment: ['&& denotes a mnemonic'] }, "&&Single")
		}
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutTwoColumnsAction.ID,
		title: {
			original: 'Two Columns',
			value: localize('miTwoColumnsEditorLayoutWithoutMnemonic', "Two Columns"),
			mnemonicTitle: localize({ key: 'miTwoColumnsEditorLayout', comment: ['&& denotes a mnemonic'] }, "&&Two Columns")
		}
	},
	order: 3
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutThreeColumnsAction.ID,
		title: {
			original: 'Three Columns',
			value: localize('miThreeColumnsEditorLayoutWithoutMnemonic', "Three Columns"),
			mnemonicTitle: localize({ key: 'miThreeColumnsEditorLayout', comment: ['&& denotes a mnemonic'] }, "T&&hree Columns")
		}
	},
	order: 4
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutTwoRowsAction.ID,
		title: {
			original: 'Two Rows',
			value: localize('miTwoRowsEditorLayoutWithoutMnemonic', "Two Rows"),
			mnemonicTitle: localize({ key: 'miTwoRowsEditorLayout', comment: ['&& denotes a mnemonic'] }, "T&&wo Rows")
		}
	},
	order: 5
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutThreeRowsAction.ID,
		title: {
			original: 'Three Rows',
			value: localize('miThreeRowsEditorLayoutWithoutMnemonic', "Three Rows"),
			mnemonicTitle: localize({ key: 'miThreeRowsEditorLayout', comment: ['&& denotes a mnemonic'] }, "Three &&Rows")
		}
	},
	order: 6
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutTwoByTwoGridAction.ID,
		title: {
			original: 'Grid (2x2)',
			value: localize('miTwoByTwoGridEditorLayoutWithoutMnemonic', "Grid (2x2)"),
			mnemonicTitle: localize({ key: 'miTwoByTwoGridEditorLayout', comment: ['&& denotes a mnemonic'] }, "&&Grid (2x2)")
		}
	},
	order: 7
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutTwoRowsRightAction.ID,
		title: {
			original: 'Two Rows Right',
			value: localize('miTwoRowsRightEditorLayoutWithoutMnemonic', "Two Rows Right"),
			mnemonicTitle: localize({ key: 'miTwoRowsRightEditorLayout', comment: ['&& denotes a mnemonic'] }, "Two R&&ows Right")
		}
	},
	order: 8
});

MenuRegistry.appendMenuItem(MenuId.MenubarLayoutMenu, {
	group: '4_layouts',
	command: {
		id: EditorLayoutTwoColumnsBottomAction.ID,
		title: {
			original: 'Two Columns Bottom',
			value: localize('miTwoColumnsBottomEditorLayoutWithoutMnemonic', "Two Columns Bottom"),
			mnemonicTitle: localize({ key: 'miTwoColumnsBottomEditorLayout', comment: ['&& denotes a mnemonic'] }, "Two &&Columns Bottom")
		}
	},
	order: 9
});

// Main Menu Bar Contributions:

MenuRegistry.appendMenuItem(MenuId.MenubarGoMenu, {
	group: '1_history_nav',
	command: {
		id: 'workbench.action.navigateToLastEditLocation',
		title: localize({ key: 'miLastEditLocation', comment: ['&& denotes a mnemonic'] }, "&&Last Edit Location"),
		precondition: ContextKeyExpr.has('canNavigateToLastEditLocation')
	},
	order: 3
});

// Switch Editor

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '1_sideBySide',
	command: {
		id: FOCUS_FIRST_SIDE_EDITOR,
		title: localize({ key: 'miFirstSideEditor', comment: ['&& denotes a mnemonic'] }, "&&First Side in Editor")
	},
	when: ContextKeyExpr.or(SideBySideEditorActiveContext, TextCompareEditorActiveContext),
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '1_sideBySide',
	command: {
		id: FOCUS_SECOND_SIDE_EDITOR,
		title: localize({ key: 'miSecondSideEditor', comment: ['&& denotes a mnemonic'] }, "&&Second Side in Editor")
	},
	when: ContextKeyExpr.or(SideBySideEditorActiveContext, TextCompareEditorActiveContext),
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '2_any',
	command: {
		id: 'workbench.action.nextEditor',
		/* AGPL */
		title: localize({ key: 'miNextEditor', comment: ['&& denotes a mnemonic'] }, "&&Next")
		/* End AGPL */
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '2_any',
	command: {
		id: 'workbench.action.previousEditor',
		/* AGPL */
		title: localize({ key: 'miPreviousEditor', comment: ['&& denotes a mnemonic'] }, "&&Previous")
		/* End AGPL */
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '3_any_used',
	command: {
		id: 'workbench.action.openNextRecentlyUsedEditor',
		/* AGPL */
		title: localize({ key: 'miNextRecentlyUsedEditor', comment: ['&& denotes a mnemonic'] }, "&&Next Used")
		/* End AGPL */
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '3_any_used',
	command: {
		id: 'workbench.action.openPreviousRecentlyUsedEditor',
		/* AGPL */
		title: localize({ key: 'miPreviousRecentlyUsedEditor', comment: ['&& denotes a mnemonic'] }, "&&Previous Used")
		/* End AGPL */
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '4_group',
	command: {
		id: 'workbench.action.nextEditorInGroup',
		/* AGPL */
		title: localize({ key: 'miNextEditorInGroup', comment: ['&& denotes a mnemonic'] }, "&&Next in Group")
		/* End AGPL */
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '4_group',
	command: {
		id: 'workbench.action.previousEditorInGroup',
		/* AGPL */
		title: localize({ key: 'miPreviousEditorInGroup', comment: ['&& denotes a mnemonic'] }, "&&Previous in Group")
		/* End AGPL */
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '5_group_used',
	command: {
		id: 'workbench.action.openNextRecentlyUsedEditorInGroup',
		/* AGPL */
		title: localize({ key: 'miNextUsedEditorInGroup', comment: ['&& denotes a mnemonic'] }, "&&Next Used in Group")
		/* End AGPL */
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchEditorMenu, {
	group: '5_group_used',
	command: {
		id: 'workbench.action.openPreviousRecentlyUsedEditorInGroup',
		/* AGPL */
		title: localize({ key: 'miPreviousUsedEditorInGroup', comment: ['&& denotes a mnemonic'] }, "&&Previous Used in Group")
		/* End AGPL */
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarGoMenu, {
	group: '2_editor_nav',
	/* AGPL */
	title: localize({ key: 'miSwitchEditor', comment: ['&& denotes a mnemonic'] }, "Switch"),
	/* End AGPL */
	submenu: MenuId.MenubarSwitchEditorMenu,
	order: 1
});

// Switch Group
MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '1_focus_index',
	command: {
		id: 'workbench.action.focusFirstEditorGroup',
		title: localize({ key: 'miFocusFirstGroup', comment: ['&& denotes a mnemonic'] }, "Group &&1")
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '1_focus_index',
	command: {
		id: 'workbench.action.focusSecondEditorGroup',
		title: localize({ key: 'miFocusSecondGroup', comment: ['&& denotes a mnemonic'] }, "Group &&2")
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '1_focus_index',
	command: {
		id: 'workbench.action.focusThirdEditorGroup',
		title: localize({ key: 'miFocusThirdGroup', comment: ['&& denotes a mnemonic'] }, "Group &&3"),
		precondition: MultipleEditorGroupsContext
	},
	order: 3
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '1_focus_index',
	command: {
		id: 'workbench.action.focusFourthEditorGroup',
		title: localize({ key: 'miFocusFourthGroup', comment: ['&& denotes a mnemonic'] }, "Group &&4"),
		precondition: MultipleEditorGroupsContext
	},
	order: 4
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '1_focus_index',
	command: {
		id: 'workbench.action.focusFifthEditorGroup',
		title: localize({ key: 'miFocusFifthGroup', comment: ['&& denotes a mnemonic'] }, "Group &&5"),
		precondition: MultipleEditorGroupsContext
	},
	order: 5
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '2_next_prev',
	command: {
		id: 'workbench.action.focusNextGroup',
		title: localize({ key: 'miNextGroup', comment: ['&& denotes a mnemonic'] }, "&&Next Group"),
		precondition: MultipleEditorGroupsContext
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '2_next_prev',
	command: {
		id: 'workbench.action.focusPreviousGroup',
		title: localize({ key: 'miPreviousGroup', comment: ['&& denotes a mnemonic'] }, "&&Previous Group"),
		precondition: MultipleEditorGroupsContext
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '3_directional',
	command: {
		id: 'workbench.action.focusLeftGroup',
		title: localize({ key: 'miFocusLeftGroup', comment: ['&& denotes a mnemonic'] }, "Group &&Left"),
		precondition: MultipleEditorGroupsContext
	},
	order: 1
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '3_directional',
	command: {
		id: 'workbench.action.focusRightGroup',
		title: localize({ key: 'miFocusRightGroup', comment: ['&& denotes a mnemonic'] }, "Group &&Right"),
		precondition: MultipleEditorGroupsContext
	},
	order: 2
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '3_directional',
	command: {
		id: 'workbench.action.focusAboveGroup',
		title: localize({ key: 'miFocusAboveGroup', comment: ['&& denotes a mnemonic'] }, "Group &&Above"),
		precondition: MultipleEditorGroupsContext
	},
	order: 3
});

MenuRegistry.appendMenuItem(MenuId.MenubarSwitchGroupMenu, {
	group: '3_directional',
	command: {
		id: 'workbench.action.focusBelowGroup',
		title: localize({ key: 'miFocusBelowGroup', comment: ['&& denotes a mnemonic'] }, "Group &&Below"),
		precondition: MultipleEditorGroupsContext
	},
	order: 4
});

MenuRegistry.appendMenuItem(MenuId.MenubarGoMenu, {
	group: '2_editor_nav',
	title: localize({ key: 'miSwitchGroup', comment: ['&& denotes a mnemonic'] }, "Switch &&Group"),
	submenu: MenuId.MenubarSwitchGroupMenu,
	order: 2
});

//#endregion
