/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Codicon } from 'vs/base/common/codicons';
import { KeyCode, KeyMod } from 'vs/base/common/keyCodes';
import { Disposable } from 'vs/base/common/lifecycle';
import { ThemeIcon } from 'vs/base/common/themables';
import { ICodeEditor } from 'vs/editor/browser/editorBrowser';
import { EditorAction2, ServicesAccessor } from 'vs/editor/browser/editorExtensions';
import { ICodeEditorService } from 'vs/editor/browser/services/codeEditorService';
import { EditorContextKeys } from 'vs/editor/common/editorContextKeys';
import { localize, localize2 } from 'vs/nls';
import { Action2, IAction2Options, MenuId, registerAction2 } from 'vs/platform/actions/common/actions';
import { ContextKeyExpr } from 'vs/platform/contextkey/common/contextkey';
import { KeybindingWeight } from 'vs/platform/keybinding/common/keybindingsRegistry';
import { IQuickInputService, IQuickPickItem } from 'vs/platform/quickinput/common/quickInput';
import { Registry } from 'vs/platform/registry/common/platform';
import { ViewAction } from 'vs/workbench/browser/parts/views/viewPane';
import { IWorkbenchContributionsRegistry, Extensions as WorkbenchExtensions } from 'vs/workbench/common/contributions';
import { IViewsService } from 'vs/workbench/common/views';
import { AccessibilityHelpAction } from 'vs/workbench/contrib/accessibility/browser/accessibleViewActions';
import { runAccessibilityHelpAction } from 'vs/workbench/contrib/chat/browser/actions/chatAccessibilityHelp';
import { IChatWidgetService } from 'vs/workbench/contrib/chat/browser/chat';
import { IChatEditorOptions } from 'vs/workbench/contrib/chat/browser/chatEditor';
import { ChatEditorInput } from 'vs/workbench/contrib/chat/browser/chatEditorInput';
import { ChatViewPane } from 'vs/workbench/contrib/chat/browser/chatViewPane';
import { IChatAgentService } from 'vs/workbench/contrib/chat/common/chatAgents';
import { CONTEXT_IN_CHAT_INPUT, CONTEXT_IN_CHAT_SESSION, CONTEXT_PROVIDER_EXISTS, CONTEXT_REQUEST, CONTEXT_RESPONSE } from 'vs/workbench/contrib/chat/common/chatContextKeys';
import { IChatContributionService } from 'vs/workbench/contrib/chat/common/chatContributionService';
import { chatAgentLeader } from 'vs/workbench/contrib/chat/common/chatParserTypes';
import { IChatDetail, IChatService } from 'vs/workbench/contrib/chat/common/chatService';
import { IChatWidgetHistoryService } from 'vs/workbench/contrib/chat/common/chatWidgetHistoryService';
import { IEditorService } from 'vs/workbench/services/editor/common/editorService';
import { LifecyclePhase } from 'vs/workbench/services/lifecycle/common/lifecycle';

export const CHAT_CATEGORY = { value: localize('chat.category', "Chat"), original: 'Chat' };
export const CHAT_OPEN_ACTION_ID = 'workbench.action.chat.open';

class QuickChatGlobalAction extends Action2 {
	constructor() {
		super({
			id: CHAT_OPEN_ACTION_ID,
			title: { value: localize('quickChat', "Quick Chat"), original: 'Quick Chat' },
			precondition: CONTEXT_PROVIDER_EXISTS,
			icon: Codicon.commentDiscussion,
			f1: false,
			category: CHAT_CATEGORY,
			keybinding: {
				weight: KeybindingWeight.WorkbenchContrib,
				primary: KeyMod.CtrlCmd | KeyMod.Alt | KeyCode.KeyI,
				mac: {
					primary: KeyMod.CtrlCmd | KeyMod.WinCtrl | KeyCode.KeyI
				}
			}
		});
	}

	override async run(accessor: ServicesAccessor, query?: string): Promise<void> {
		const chatService = accessor.get(IChatService);
		const chatWidgetService = accessor.get(IChatWidgetService);
		const providers = chatService.getProviderInfos();
		if (!providers.length) {
			return;
		}
		const chatWidget = await chatWidgetService.revealViewForProvider(providers[0].id);
		if (!chatWidget) {
			return;
		}
		if (query) {
			chatWidget.acceptInput(query);
		}
		chatWidget.focusInput();
	}
}

export class ChatSubmitSecondaryAgentEditorAction extends EditorAction2 {
	static readonly ID = 'workbench.action.chat.submitSecondaryAgent';

	constructor() {
		super({
			id: ChatSubmitSecondaryAgentEditorAction.ID,
			title: localize2({ key: 'actions.chat.submitSecondaryAgent', comment: ['Send input from the chat input box to the secondary agent'] }, "Submit to Secondary Agent"),
			precondition: CONTEXT_IN_CHAT_INPUT,
			keybinding: {
				when: EditorContextKeys.textInputFocus,
				primary: KeyMod.CtrlCmd | KeyCode.Enter,
				weight: KeybindingWeight.EditorContrib
			}
		});
	}

	runEditorCommand(accessor: ServicesAccessor, editor: ICodeEditor): void | Promise<void> {
		const editorUri = editor.getModel()?.uri;
		if (editorUri) {
			const agentService = accessor.get(IChatAgentService);
			const secondaryAgent = agentService.getSecondaryAgent();
			if (!secondaryAgent) {
				return;
			}

			const widgetService = accessor.get(IChatWidgetService);
			widgetService.getWidgetByInputUri(editorUri)?.acceptInputWithPrefix(`${chatAgentLeader}${secondaryAgent.id}`);
		}
	}
}

export class ChatSubmitEditorAction extends EditorAction2 {
	static readonly ID = 'workbench.action.chat.acceptInput';

	constructor() {
		super({
			id: ChatSubmitEditorAction.ID,
			title: localize2({ key: 'actions.chat.submit', comment: ['Apply input from the chat input box'] }, "Submit"),
			precondition: CONTEXT_IN_CHAT_INPUT,
			keybinding: {
				when: EditorContextKeys.textInputFocus,
				primary: KeyCode.Enter,
				weight: KeybindingWeight.EditorContrib
			}
		});
	}

	runEditorCommand(accessor: ServicesAccessor, editor: ICodeEditor): void | Promise<void> {
		const editorUri = editor.getModel()?.uri;
		if (editorUri) {
			const widgetService = accessor.get(IChatWidgetService);
			widgetService.getWidgetByInputUri(editorUri)?.acceptInput();
		}
	}
}

export function registerChatActions() {
	registerAction2(QuickChatGlobalAction);
	registerAction2(ChatSubmitEditorAction);

	registerAction2(ChatSubmitSecondaryAgentEditorAction);

	registerAction2(class ClearChatHistoryAction extends Action2 {
		constructor() {
			super({
				id: 'workbench.action.chatEditor.clearHistory',
				title: {
					value: localize('interactiveSession.clearHistory.label', "Clear Input History"),
					original: 'Clear Input History'
				},
				precondition: CONTEXT_PROVIDER_EXISTS,
				category: CHAT_CATEGORY,
				f1: true,
			});
		}
		async run(accessor: ServicesAccessor, ...args: any[]) {
			const historyService = accessor.get(IChatWidgetHistoryService);
			historyService.clearHistory();
		}
	});

	registerAction2(class FocusChatAction extends EditorAction2 {
		constructor() {
			super({
				id: 'chat.action.focus',
				title: { value: localize('actions.interactiveSession.focus', "Focus Chat List"), original: 'Focus Chat List' },
				precondition: CONTEXT_IN_CHAT_INPUT,
				category: CHAT_CATEGORY,
				keybinding: {
					when: EditorContextKeys.textInputFocus,
					primary: KeyMod.CtrlCmd | KeyCode.UpArrow,
					weight: KeybindingWeight.EditorContrib
				}
			});
		}

		runEditorCommand(accessor: ServicesAccessor, editor: ICodeEditor): void | Promise<void> {
			const editorUri = editor.getModel()?.uri;
			if (editorUri) {
				const widgetService = accessor.get(IChatWidgetService);
				widgetService.getWidgetByInputUri(editorUri)?.focusLastMessage();
			}
		}
	});

	class ChatAccessibilityHelpContribution extends Disposable {
		static ID: 'chatAccessibilityHelpContribution';
		constructor() {
			super();
			this._register(AccessibilityHelpAction.addImplementation(105, 'panelChat', async accessor => {
				const codeEditor = accessor.get(ICodeEditorService).getActiveCodeEditor() || accessor.get(ICodeEditorService).getFocusedCodeEditor();
				runAccessibilityHelpAction(accessor, codeEditor ?? undefined, 'panelChat');
			}, ContextKeyExpr.or(CONTEXT_IN_CHAT_SESSION, CONTEXT_RESPONSE, CONTEXT_REQUEST)));
		}
	}

	const workbenchRegistry = Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench);
	workbenchRegistry.registerWorkbenchContribution(ChatAccessibilityHelpContribution, LifecyclePhase.Eventually);

	registerAction2(class FocusChatInputAction extends Action2 {
		constructor() {
			super({
				id: 'workbench.action.chat.focusInput',
				title: {
					value: localize('interactiveSession.focusInput.label', "Focus Chat Input"),
					original: 'Focus Chat Input'
				},
				f1: false,
				keybinding: {
					primary: KeyMod.CtrlCmd | KeyCode.DownArrow,
					weight: KeybindingWeight.WorkbenchContrib,
					when: ContextKeyExpr.and(CONTEXT_IN_CHAT_SESSION, ContextKeyExpr.not(EditorContextKeys.focus.key))
				}
			});
		}
		run(accessor: ServicesAccessor, ...args: any[]) {
			const widgetService = accessor.get(IChatWidgetService);
			widgetService.lastFocusedWidget?.focusInput();
		}
	});
}

export function getOpenChatEditorAction(id: string, label: string, when?: string) {
	return class OpenChatEditor extends Action2 {
		constructor() {
			super({
				id: `workbench.action.openChat.${id}`,
				title: { value: localize('interactiveSession.open', "Open Editor ({0})", label), original: `Open Editor (${label})` },
				f1: true,
				category: CHAT_CATEGORY,
				precondition: ContextKeyExpr.deserialize(when)
			});
		}

		async run(accessor: ServicesAccessor) {
			const editorService = accessor.get(IEditorService);
			await editorService.openEditor({ resource: ChatEditorInput.getNewEditorUri(), options: <IChatEditorOptions>{ target: { providerId: id }, pinned: true } });
		}
	};
}

const getHistoryChatActionDescriptorForViewTitle = (viewId: string, providerId: string): Readonly<IAction2Options> & { viewId: string } => ({
	viewId,
	id: `workbench.action.chat.${providerId}.history`,
	title: {
		value: localize('interactiveSession.history.label', "Show History"),
		original: 'Show History'
	},
	menu: {
		id: MenuId.ViewTitle,
		when: ContextKeyExpr.equals('view', viewId),
		group: 'navigation',
		order: -1
	},
	category: CHAT_CATEGORY,
	icon: Codicon.history,
	f1: false,
	precondition: CONTEXT_PROVIDER_EXISTS
});

export function getHistoryAction(viewId: string, providerId: string) {
	return class HistoryAction extends ViewAction<ChatViewPane> {
		constructor() {
			super(getHistoryChatActionDescriptorForViewTitle(viewId, providerId));
		}

		async runInView(accessor: ServicesAccessor, view: ChatViewPane) {
			const chatService = accessor.get(IChatService);
			const quickInputService = accessor.get(IQuickInputService);
			const chatContribService = accessor.get(IChatContributionService);
			const viewsService = accessor.get(IViewsService);
			const items = chatService.getHistory();
			const picks = items.map(i => (<IQuickPickItem & { chat: IChatDetail }>{
				label: i.title,
				chat: i,
				buttons: [{
					iconClass: ThemeIcon.asClassName(Codicon.x),
					tooltip: localize('interactiveSession.history.delete', "Delete"),
				}]
			}));
			const selection = await quickInputService.pick(picks,
				{
					placeHolder: localize('interactiveSession.history.pick', "Switch to chat session"),
					onDidTriggerItemButton: context => {
						chatService.removeHistoryEntry(context.item.chat.sessionId);
						context.removeItem();
					}
				});
			if (selection) {
				const sessionId = selection.chat.sessionId;
				const provider = chatContribService.registeredProviders[0]?.id;
				if (provider) {
					const viewId = chatContribService.getViewIdForProvider(provider);
					const view = await viewsService.openView(viewId) as ChatViewPane;
					view.loadSession(sessionId);
				}
			}
		}
	};
}
