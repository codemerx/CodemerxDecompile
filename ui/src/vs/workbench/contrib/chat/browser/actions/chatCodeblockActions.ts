/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { CancellationTokenSource } from 'vs/base/common/cancellation';
import { Codicon } from 'vs/base/common/codicons';
import { KeyCode, KeyMod } from 'vs/base/common/keyCodes';
import { ICodeEditor, isCodeEditor, isDiffEditor } from 'vs/editor/browser/editorBrowser';
import { ServicesAccessor } from 'vs/editor/browser/editorExtensions';
import { IBulkEditService, ResourceTextEdit } from 'vs/editor/browser/services/bulkEditService';
import { ICodeEditorService } from 'vs/editor/browser/services/codeEditorService';
import { Range } from 'vs/editor/common/core/range';
import { EditorContextKeys } from 'vs/editor/common/editorContextKeys';
import { DocumentContextItem, WorkspaceEdit } from 'vs/editor/common/languages';
import { ILanguageService } from 'vs/editor/common/languages/language';
import { ITextModel } from 'vs/editor/common/model';
import { ILanguageFeaturesService } from 'vs/editor/common/services/languageFeatures';
import { CopyAction } from 'vs/editor/contrib/clipboard/browser/clipboard';
import { localize } from 'vs/nls';
import { Action2, MenuId, registerAction2 } from 'vs/platform/actions/common/actions';
import { IClipboardService } from 'vs/platform/clipboard/common/clipboardService';
import { ContextKeyExpr } from 'vs/platform/contextkey/common/contextkey';
import { KeybindingWeight } from 'vs/platform/keybinding/common/keybindingsRegistry';
import { TerminalLocation } from 'vs/platform/terminal/common/terminal';
import { IUntitledTextResourceEditorInput } from 'vs/workbench/common/editor';
import { CHAT_CATEGORY } from 'vs/workbench/contrib/chat/browser/actions/chatActions';
import { IChatWidgetService } from 'vs/workbench/contrib/chat/browser/chat';
import { ICodeBlockActionContext } from 'vs/workbench/contrib/chat/browser/codeBlockPart';
import { CONTEXT_IN_CHAT_INPUT, CONTEXT_IN_CHAT_SESSION, CONTEXT_PROVIDER_EXISTS } from 'vs/workbench/contrib/chat/common/chatContextKeys';
import { IChatService, IDocumentContext, InteractiveSessionCopyKind } from 'vs/workbench/contrib/chat/common/chatService';
import { IChatResponseViewModel, isResponseVM } from 'vs/workbench/contrib/chat/common/chatViewModel';
import { CTX_INLINE_CHAT_VISIBLE } from 'vs/workbench/contrib/inlineChat/common/inlineChat';
import { insertCell } from 'vs/workbench/contrib/notebook/browser/controller/cellOperations';
import { INotebookEditor } from 'vs/workbench/contrib/notebook/browser/notebookBrowser';
import { CellKind, NOTEBOOK_EDITOR_ID } from 'vs/workbench/contrib/notebook/common/notebookCommon';
import { ITerminalEditorService, ITerminalGroupService, ITerminalService } from 'vs/workbench/contrib/terminal/browser/terminal';
import { IEditorService } from 'vs/workbench/services/editor/common/editorService';
import { ITextFileService } from 'vs/workbench/services/textfile/common/textfiles';

export interface IChatCodeBlockActionContext extends ICodeBlockActionContext {
	element: IChatResponseViewModel;
}

export function isCodeBlockActionContext(thing: unknown): thing is ICodeBlockActionContext {
	return typeof thing === 'object' && thing !== null && 'code' in thing && 'element' in thing;
}

function isResponseFiltered(context: ICodeBlockActionContext) {
	return isResponseVM(context.element) && context.element.errorDetails?.responseIsFiltered;
}

function getUsedDocuments(context: ICodeBlockActionContext): IDocumentContext[] | undefined {
	return isResponseVM(context.element) ? context.element.usedContext?.documents : undefined;
}

abstract class ChatCodeBlockAction extends Action2 {
	run(accessor: ServicesAccessor, ...args: any[]) {
		let context = args[0];
		if (!isCodeBlockActionContext(context)) {
			const codeEditorService = accessor.get(ICodeEditorService);
			const editor = codeEditorService.getFocusedCodeEditor() || codeEditorService.getActiveCodeEditor();
			if (!editor) {
				return;
			}

			context = getContextFromEditor(editor, accessor);
			if (!isCodeBlockActionContext(context)) {
				return;
			}
		}

		return this.runWithContext(accessor, context);
	}

	abstract runWithContext(accessor: ServicesAccessor, context: ICodeBlockActionContext): any;
}

export function registerChatCodeBlockActions() {
	registerAction2(class CopyCodeBlockAction extends Action2 {
		constructor() {
			super({
				id: 'workbench.action.chat.copyCodeBlock',
				title: {
					value: localize('interactive.copyCodeBlock.label', "Copy"),
					original: 'Copy'
				},
				f1: false,
				category: CHAT_CATEGORY,
				icon: Codicon.copy,
				menu: {
					id: MenuId.ChatCodeBlock,
					group: 'navigation',
				}
			});
		}

		run(accessor: ServicesAccessor, ...args: any[]) {
			const context = args[0];
			if (!isCodeBlockActionContext(context) || isResponseFiltered(context)) {
				return;
			}

			const clipboardService = accessor.get(IClipboardService);
			clipboardService.writeText(context.code);

			if (isResponseVM(context.element)) {
				const chatService = accessor.get(IChatService);
				chatService.notifyUserAction({
					providerId: context.element.providerId,
					agentId: context.element.agent?.id,
					sessionId: context.element.sessionId,
					requestId: context.element.requestId,
					action: {
						kind: 'copy',
						codeBlockIndex: context.codeBlockIndex,
						copyType: InteractiveSessionCopyKind.Toolbar,
						copiedCharacters: context.code.length,
						totalCharacters: context.code.length,
						copiedText: context.code,
					}
				});
			}
		}
	});

	CopyAction?.addImplementation(50000, 'chat-codeblock', (accessor) => {
		// get active code editor
		const editor = accessor.get(ICodeEditorService).getFocusedCodeEditor();
		if (!editor) {
			return false;
		}

		const editorModel = editor.getModel();
		if (!editorModel) {
			return false;
		}

		const context = getContextFromEditor(editor, accessor);
		if (!context) {
			return false;
		}

		const noSelection = editor.getSelections()?.length === 1 && editor.getSelection()?.isEmpty();
		const copiedText = noSelection ?
			editorModel.getValue() :
			editor.getSelections()?.reduce((acc, selection) => acc + editorModel.getValueInRange(selection), '') ?? '';
		const totalCharacters = editorModel.getValueLength();

		// Report copy to extensions
		const chatService = accessor.get(IChatService);
		chatService.notifyUserAction({
			providerId: context.element.providerId,
			agentId: context.element.agent?.id,
			sessionId: context.element.sessionId,
			requestId: context.element.requestId,
			action: {
				kind: 'copy',
				codeBlockIndex: context.codeBlockIndex,
				copyType: InteractiveSessionCopyKind.Action,
				copiedText,
				copiedCharacters: copiedText.length,
				totalCharacters,
			}
		});

		// Copy full cell if no selection, otherwise fall back on normal editor implementation
		if (noSelection) {
			accessor.get(IClipboardService).writeText(context.code);
			return true;
		}

		return false;
	});

	registerAction2(class InsertCodeBlockAction extends ChatCodeBlockAction {
		constructor() {
			super({
				id: 'workbench.action.chat.insertCodeBlock',
				title: {
					value: localize('interactive.insertCodeBlock.label', "Insert at Cursor"),
					original: 'Insert at Cursor'
				},
				precondition: CONTEXT_PROVIDER_EXISTS,
				f1: true,
				category: CHAT_CATEGORY,
				icon: Codicon.insert,
				menu: {
					id: MenuId.ChatCodeBlock,
					group: 'navigation',
					when: CONTEXT_IN_CHAT_SESSION,
				},
				keybinding: {
					when: ContextKeyExpr.and(CONTEXT_IN_CHAT_SESSION, CONTEXT_IN_CHAT_INPUT.negate()),
					primary: KeyMod.CtrlCmd | KeyCode.Enter,
					mac: { primary: KeyMod.WinCtrl | KeyCode.Enter },
					weight: KeybindingWeight.WorkbenchContrib
				},
			});
		}

		override async runWithContext(accessor: ServicesAccessor, context: ICodeBlockActionContext) {
			const editorService = accessor.get(IEditorService);
			const textFileService = accessor.get(ITextFileService);

			if (isResponseFiltered(context)) {
				// When run from command palette
				return;
			}

			if (editorService.activeEditorPane?.getId() === NOTEBOOK_EDITOR_ID) {
				return this.handleNotebookEditor(accessor, editorService.activeEditorPane.getControl() as INotebookEditor, context);
			}

			let activeEditorControl = editorService.activeTextEditorControl;
			if (isDiffEditor(activeEditorControl)) {
				activeEditorControl = activeEditorControl.getOriginalEditor().hasTextFocus() ? activeEditorControl.getOriginalEditor() : activeEditorControl.getModifiedEditor();
			}

			if (!isCodeEditor(activeEditorControl)) {
				return;
			}

			const activeModel = activeEditorControl.getModel();
			if (!activeModel) {
				return;
			}

			// Check if model is editable, currently only support untitled and text file
			const activeTextModel = textFileService.files.get(activeModel.uri) ?? textFileService.untitled.get(activeModel.uri);
			if (!activeTextModel || activeTextModel.isReadonly()) {
				return;
			}

			await this.handleTextEditor(accessor, activeEditorControl, activeModel, context);
		}

		private async handleNotebookEditor(accessor: ServicesAccessor, notebookEditor: INotebookEditor, context: ICodeBlockActionContext) {
			if (!notebookEditor.hasModel()) {
				return;
			}

			if (notebookEditor.isReadOnly) {
				return;
			}

			if (notebookEditor.activeCodeEditor?.hasTextFocus()) {
				const codeEditor = notebookEditor.activeCodeEditor;
				const textModel = codeEditor.getModel();

				if (textModel) {
					return this.handleTextEditor(accessor, codeEditor, textModel, context);
				}
			}

			const languageService = accessor.get(ILanguageService);
			const focusRange = notebookEditor.getFocus();
			const next = Math.max(focusRange.end - 1, 0);
			insertCell(languageService, notebookEditor, next, CellKind.Code, 'below', context.code, true);
			this.notifyUserAction(accessor, context);
		}

		private async handleTextEditor(accessor: ServicesAccessor, codeEditor: ICodeEditor, activeModel: ITextModel, codeBlockActionContext: ICodeBlockActionContext) {
			this.notifyUserAction(accessor, codeBlockActionContext);

			const bulkEditService = accessor.get(IBulkEditService);
			const codeEditorService = accessor.get(ICodeEditorService);

			const mappedEditsProviders = accessor.get(ILanguageFeaturesService).mappedEditsProvider.ordered(activeModel);

			// try applying workspace edit that was returned by a MappedEditsProvider, else simply insert at selection

			let mappedEdits: WorkspaceEdit | null = null;

			if (mappedEditsProviders.length > 0) {
				const mostRelevantProvider = mappedEditsProviders[0]; // TODO@ulugbekna: should we try all providers?

				// 0th sub-array - editor selections array if there are any selections
				// 1st sub-array - array with documents used to get the chat reply
				const docRefs: DocumentContextItem[][] = [];

				if (codeEditor.hasModel()) {
					const model = codeEditor.getModel();
					const currentDocUri = model.uri;
					const currentDocVersion = model.getVersionId();
					const selections = codeEditor.getSelections();
					if (selections.length > 0) {
						docRefs.push([
							{
								uri: currentDocUri,
								version: currentDocVersion,
								ranges: selections,
							}
						]);
					}
				}

				const usedDocuments = getUsedDocuments(codeBlockActionContext);
				if (usedDocuments) {
					docRefs.push(usedDocuments);
				}

				const cancellationTokenSource = new CancellationTokenSource();

				mappedEdits = await mostRelevantProvider.provideMappedEdits(
					activeModel,
					[codeBlockActionContext.code],
					{ documents: docRefs },
					cancellationTokenSource.token);
			}

			if (mappedEdits) {
				await bulkEditService.apply(mappedEdits);
			} else {
				const activeSelection = codeEditor.getSelection() ?? new Range(activeModel.getLineCount(), 1, activeModel.getLineCount(), 1);
				await bulkEditService.apply([
					new ResourceTextEdit(activeModel.uri, {
						range: activeSelection,
						text: codeBlockActionContext.code,
					}),
				]);
			}
			codeEditorService.listCodeEditors().find(editor => editor.getModel()?.uri.toString() === activeModel.uri.toString())?.focus();
		}

		private notifyUserAction(accessor: ServicesAccessor, context: ICodeBlockActionContext) {
			if (isResponseVM(context.element)) {
				const chatService = accessor.get(IChatService);
				chatService.notifyUserAction({
					providerId: context.element.providerId,
					agentId: context.element.agent?.id,
					sessionId: context.element.sessionId,
					requestId: context.element.requestId,
					action: {
						kind: 'insert',
						codeBlockIndex: context.codeBlockIndex,
						totalCharacters: context.code.length,
					}
				});
			}
		}

	});

	registerAction2(class InsertIntoNewFileAction extends ChatCodeBlockAction {
		constructor() {
			super({
				id: 'workbench.action.chat.insertIntoNewFile',
				title: {
					value: localize('interactive.insertIntoNewFile.label', "Insert into New File"),
					original: 'Insert into New File'
				},
				precondition: CONTEXT_PROVIDER_EXISTS,
				f1: true,
				category: CHAT_CATEGORY,
				icon: Codicon.newFile,
				menu: {
					id: MenuId.ChatCodeBlock,
					group: 'navigation',
					isHiddenByDefault: true,
				}
			});
		}

		override async runWithContext(accessor: ServicesAccessor, context: ICodeBlockActionContext) {
			if (isResponseFiltered(context)) {
				// When run from command palette
				return;
			}

			const editorService = accessor.get(IEditorService);
			const chatService = accessor.get(IChatService);

			editorService.openEditor(<IUntitledTextResourceEditorInput>{ contents: context.code, languageId: context.languageId, resource: undefined });

			if (isResponseVM(context.element)) {
				chatService.notifyUserAction({
					providerId: context.element.providerId,
					agentId: context.element.agent?.id,
					sessionId: context.element.sessionId,
					requestId: context.element.requestId,
					action: {
						kind: 'insert',
						codeBlockIndex: context.codeBlockIndex,
						totalCharacters: context.code.length,
						newFile: true
					}
				});
			}
		}
	});

	const shellLangIds = [
		'powershell',
		'shellscript'
	];
	registerAction2(class RunInTerminalAction extends ChatCodeBlockAction {
		constructor() {
			super({
				id: 'workbench.action.chat.runInTerminal',
				title: {
					value: localize('interactive.runInTerminal.label', "Insert into Terminal"),
					original: 'Insert into Terminal'
				},
				precondition: CONTEXT_PROVIDER_EXISTS,
				f1: true,
				category: CHAT_CATEGORY,
				icon: Codicon.terminal,
				menu: [{
					id: MenuId.ChatCodeBlock,
					group: 'navigation',
					when: ContextKeyExpr.and(
						CONTEXT_IN_CHAT_SESSION,
						ContextKeyExpr.or(...shellLangIds.map(e => ContextKeyExpr.equals(EditorContextKeys.languageId.key, e)))
					),
				},
				{
					id: MenuId.ChatCodeBlock,
					group: 'navigation',
					isHiddenByDefault: true,
					when: ContextKeyExpr.and(
						CONTEXT_IN_CHAT_SESSION,
						...shellLangIds.map(e => ContextKeyExpr.notEquals(EditorContextKeys.languageId.key, e))
					)
				},
				{
					id: MenuId.ChatCodeBlock,
					group: 'navigation',
					when: CTX_INLINE_CHAT_VISIBLE,
				}],
				keybinding: [{
					primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.Enter,
					mac: {
						primary: KeyMod.WinCtrl | KeyMod.Alt | KeyCode.Enter
					},
					weight: KeybindingWeight.EditorContrib,
					when: CONTEXT_IN_CHAT_SESSION,
				}]
			});
		}

		override async runWithContext(accessor: ServicesAccessor, context: ICodeBlockActionContext) {
			if (isResponseFiltered(context)) {
				// When run from command palette
				return;
			}

			const chatService = accessor.get(IChatService);
			const terminalService = accessor.get(ITerminalService);
			const editorService = accessor.get(IEditorService);
			const terminalEditorService = accessor.get(ITerminalEditorService);
			const terminalGroupService = accessor.get(ITerminalGroupService);

			let terminal = await terminalService.getActiveOrCreateInstance();

			// isFeatureTerminal = debug terminal or task terminal
			const unusableTerminal = terminal.xterm?.isStdinDisabled || terminal.shellLaunchConfig.isFeatureTerminal;
			terminal = unusableTerminal ? await terminalService.createTerminal() : terminal;

			terminalService.setActiveInstance(terminal);
			await terminal.focusWhenReady(true);
			if (terminal.target === TerminalLocation.Editor) {
				const existingEditors = editorService.findEditors(terminal.resource);
				terminalEditorService.openEditor(terminal, { viewColumn: existingEditors?.[0].groupId });
			} else {
				terminalGroupService.showPanel(true);
			}

			terminal.sendText(context.code, false, true);

			if (isResponseVM(context.element)) {
				chatService.notifyUserAction({
					providerId: context.element.providerId,
					agentId: context.element.agent?.id,
					sessionId: context.element.sessionId,
					requestId: context.element.requestId,
					action: {
						kind: 'runInTerminal',
						codeBlockIndex: context.codeBlockIndex,
						languageId: context.languageId,
					}
				});
			}
		}
	});

	function navigateCodeBlocks(accessor: ServicesAccessor, reverse?: boolean): void {
		const codeEditorService = accessor.get(ICodeEditorService);
		const chatWidgetService = accessor.get(IChatWidgetService);
		const widget = chatWidgetService.lastFocusedWidget;
		if (!widget) {
			return;
		}

		const editor = codeEditorService.getFocusedCodeEditor();
		const editorUri = editor?.getModel()?.uri;
		const curCodeBlockInfo = editorUri ? widget.getCodeBlockInfoForEditor(editorUri) : undefined;
		const focused = !widget.inputEditor.hasWidgetFocus() && widget.getFocus();
		const focusedResponse = isResponseVM(focused) ? focused : undefined;

		const currentResponse = curCodeBlockInfo ?
			curCodeBlockInfo.element :
			(focusedResponse ?? widget.viewModel?.getItems().reverse().find((item): item is IChatResponseViewModel => isResponseVM(item)));
		if (!currentResponse) {
			return;
		}

		widget.reveal(currentResponse);
		const responseCodeblocks = widget.getCodeBlockInfosForResponse(currentResponse);
		const focusIdx = curCodeBlockInfo ?
			(curCodeBlockInfo.codeBlockIndex + (reverse ? -1 : 1) + responseCodeblocks.length) % responseCodeblocks.length :
			reverse ? responseCodeblocks.length - 1 : 0;

		responseCodeblocks[focusIdx]?.focus();
	}

	registerAction2(class NextCodeBlockAction extends Action2 {
		constructor() {
			super({
				id: 'workbench.action.chat.nextCodeBlock',
				title: {
					value: localize('interactive.nextCodeBlock.label', "Next Code Block"),
					original: 'Next Code Block'
				},
				keybinding: {
					primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.PageDown,
					mac: { primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.PageDown, },
					weight: KeybindingWeight.WorkbenchContrib,
					when: CONTEXT_IN_CHAT_SESSION,
				},
				precondition: CONTEXT_PROVIDER_EXISTS,
				f1: true,
				category: CHAT_CATEGORY,
			});
		}

		run(accessor: ServicesAccessor, ...args: any[]) {
			navigateCodeBlocks(accessor);
		}
	});

	registerAction2(class PreviousCodeBlockAction extends Action2 {
		constructor() {
			super({
				id: 'workbench.action.chat.previousCodeBlock',
				title: {
					value: localize('interactive.previousCodeBlock.label', "Previous Code Block"),
					original: 'Previous Code Block'
				},
				keybinding: {
					primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.PageUp,
					mac: { primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.PageUp, },
					weight: KeybindingWeight.WorkbenchContrib,
					when: CONTEXT_IN_CHAT_SESSION,
				},
				precondition: CONTEXT_PROVIDER_EXISTS,
				f1: true,
				category: CHAT_CATEGORY,
			});
		}

		run(accessor: ServicesAccessor, ...args: any[]) {
			navigateCodeBlocks(accessor, true);
		}
	});
}

function getContextFromEditor(editor: ICodeEditor, accessor: ServicesAccessor): IChatCodeBlockActionContext | undefined {
	const chatWidgetService = accessor.get(IChatWidgetService);
	const model = editor.getModel();
	if (!model) {
		return;
	}

	const widget = chatWidgetService.lastFocusedWidget;
	if (!widget) {
		return;
	}

	const codeBlockInfo = widget.getCodeBlockInfoForEditor(model.uri);
	if (!codeBlockInfo) {
		return;
	}

	return {
		element: codeBlockInfo.element,
		codeBlockIndex: codeBlockInfo.codeBlockIndex,
		code: editor.getValue(),
		languageId: editor.getModel()!.getLanguageId(),
	};
}
