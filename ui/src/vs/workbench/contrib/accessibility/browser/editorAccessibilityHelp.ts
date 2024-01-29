/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Disposable } from 'vs/base/common/lifecycle';
import { ICodeEditor } from 'vs/editor/browser/editorBrowser';
import { ICodeEditorService } from 'vs/editor/browser/services/codeEditorService';
import { EditorOption } from 'vs/editor/common/config/editorOptions';
import { EditorContextKeys } from 'vs/editor/common/editorContextKeys';
import { AccessibilityHelpNLS } from 'vs/editor/common/standaloneStrings';
import { ToggleTabFocusModeAction } from 'vs/editor/contrib/toggleTabFocusMode/browser/toggleTabFocusMode';
import { AudioCue } from 'vs/platform/audioCues/browser/audioCueService';
import { ICommandService } from 'vs/platform/commands/common/commands';
import { IConfigurationService } from 'vs/platform/configuration/common/configuration';
import { IContextKeyService } from 'vs/platform/contextkey/common/contextkey';
import { IInstantiationService } from 'vs/platform/instantiation/common/instantiation';
import { IKeybindingService } from 'vs/platform/keybinding/common/keybinding';
import { AccessibleViewProviderId, AccessibilityVerbositySettingId } from 'vs/workbench/contrib/accessibility/browser/accessibilityConfiguration';
import { descriptionForCommand } from 'vs/workbench/contrib/accessibility/browser/accessibilityContributions';
import { IAccessibleViewService, IAccessibleContentProvider, IAccessibleViewOptions, AccessibleViewType } from 'vs/workbench/contrib/accessibility/browser/accessibleView';
import { AccessibilityHelpAction } from 'vs/workbench/contrib/accessibility/browser/accessibleViewActions';
import { CommentAccessibilityHelpNLS } from 'vs/workbench/contrib/comments/browser/comments.contribution';
import { CommentCommandId } from 'vs/workbench/contrib/comments/common/commentCommandIds';
import { CommentContextKeys } from 'vs/workbench/contrib/comments/common/commentContextKeys';
import { NEW_UNTITLED_FILE_COMMAND_ID } from 'vs/workbench/contrib/files/browser/fileConstants';

export class EditorAccessibilityHelpContribution extends Disposable {
	static ID: 'editorAccessibilityHelpContribution';
	constructor() {
		super();
		this._register(AccessibilityHelpAction.addImplementation(95, 'editor', async accessor => {
			const codeEditorService = accessor.get(ICodeEditorService);
			const accessibleViewService = accessor.get(IAccessibleViewService);
			const instantiationService = accessor.get(IInstantiationService);
			const commandService = accessor.get(ICommandService);
			let codeEditor = codeEditorService.getActiveCodeEditor() || codeEditorService.getFocusedCodeEditor();
			if (!codeEditor) {
				await commandService.executeCommand(NEW_UNTITLED_FILE_COMMAND_ID);
				codeEditor = codeEditorService.getActiveCodeEditor()!;
			}
			accessibleViewService.show(instantiationService.createInstance(EditorAccessibilityHelpProvider, codeEditor));
		}, EditorContextKeys.focus));
	}
}

class EditorAccessibilityHelpProvider implements IAccessibleContentProvider {
	id = AccessibleViewProviderId.Editor;
	onClose() {
		this._editor.focus();
	}
	options: IAccessibleViewOptions = { type: AccessibleViewType.Help, readMoreUrl: 'https://go.microsoft.com/fwlink/?linkid=851010' };
	verbositySettingKey = AccessibilityVerbositySettingId.Editor;
	constructor(
		private readonly _editor: ICodeEditor,
		@IKeybindingService private readonly _keybindingService: IKeybindingService,
		@IContextKeyService private readonly _contextKeyService: IContextKeyService,
		@IConfigurationService private readonly _configurationService: IConfigurationService
	) {
	}

	provideContent(): string {
		const options = this._editor.getOptions();
		const content = [];

		if (options.get(EditorOption.inDiffEditor)) {
			if (options.get(EditorOption.readOnly)) {
				content.push(AccessibilityHelpNLS.readonlyDiffEditor);
			} else {
				content.push(AccessibilityHelpNLS.editableDiffEditor);
			}
		} else {
			if (options.get(EditorOption.readOnly)) {
				content.push(AccessibilityHelpNLS.readonlyEditor);
			} else {
				content.push(AccessibilityHelpNLS.editableEditor);
			}
		}
		const saveAudioCue = this._configurationService.getValue(AudioCue.save.settingsKey);
		switch (saveAudioCue) {
			case 'never':
				content.push(AccessibilityHelpNLS.saveAudioCueDisabled);
				break;
			case 'always':
				content.push(AccessibilityHelpNLS.saveAudioCueAlways);
				break;
			case 'userGesture':
				content.push(AccessibilityHelpNLS.saveAudioCueUserGesture);
				break;
		}
		const formatAudioCue = this._configurationService.getValue(AudioCue.format.settingsKey);
		switch (formatAudioCue) {
			case 'never':
				content.push(AccessibilityHelpNLS.formatAudioCueDisabled);
				break;
			case 'always':
				content.push(AccessibilityHelpNLS.formatAudioCueAlways);
				break;
			case 'userGesture':
				content.push(AccessibilityHelpNLS.formatAudioCueUserGesture);
				break;
		}

		const commentCommandInfo = getCommentCommandInfo(this._keybindingService, this._contextKeyService, this._editor);
		if (commentCommandInfo) {
			content.push(commentCommandInfo);
		}

		if (options.get(EditorOption.stickyScroll).enabled) {
			content.push(descriptionForCommand('editor.action.focusStickyScroll', AccessibilityHelpNLS.stickScrollKb, AccessibilityHelpNLS.stickScrollNoKb, this._keybindingService));
		}

		if (options.get(EditorOption.tabFocusMode)) {
			content.push(descriptionForCommand(ToggleTabFocusModeAction.ID, AccessibilityHelpNLS.tabFocusModeOnMsg, AccessibilityHelpNLS.tabFocusModeOnMsgNoKb, this._keybindingService));
		} else {
			content.push(descriptionForCommand(ToggleTabFocusModeAction.ID, AccessibilityHelpNLS.tabFocusModeOffMsg, AccessibilityHelpNLS.tabFocusModeOffMsgNoKb, this._keybindingService));
		}
		return content.join('\n\n');
	}
}

export function getCommentCommandInfo(keybindingService: IKeybindingService, contextKeyService: IContextKeyService, editor: ICodeEditor): string | undefined {
	const editorContext = contextKeyService.getContext(editor.getDomNode()!);
	if (editorContext.getValue<boolean>(CommentContextKeys.activeEditorHasCommentingRange.key)) {
		const commentCommandInfo: string[] = [];
		commentCommandInfo.push(CommentAccessibilityHelpNLS.intro);
		commentCommandInfo.push(descriptionForCommand(CommentCommandId.Add, CommentAccessibilityHelpNLS.addComment, CommentAccessibilityHelpNLS.addCommentNoKb, keybindingService));
		commentCommandInfo.push(descriptionForCommand(CommentCommandId.NextThread, CommentAccessibilityHelpNLS.nextCommentThreadKb, CommentAccessibilityHelpNLS.nextCommentThreadNoKb, keybindingService));
		commentCommandInfo.push(descriptionForCommand(CommentCommandId.PreviousThread, CommentAccessibilityHelpNLS.previousCommentThreadKb, CommentAccessibilityHelpNLS.previousCommentThreadNoKb, keybindingService));
		commentCommandInfo.push(descriptionForCommand(CommentCommandId.NextRange, CommentAccessibilityHelpNLS.nextRange, CommentAccessibilityHelpNLS.nextRangeNoKb, keybindingService));
		commentCommandInfo.push(descriptionForCommand(CommentCommandId.PreviousRange, CommentAccessibilityHelpNLS.previousRange, CommentAccessibilityHelpNLS.previousRangeNoKb, keybindingService));
		return commentCommandInfo.join('\n');
	}
	return;
}
