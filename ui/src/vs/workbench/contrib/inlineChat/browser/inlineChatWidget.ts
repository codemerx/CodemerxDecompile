/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import 'vs/css!./inlineChat';
import { DisposableStore, MutableDisposable, toDisposable } from 'vs/base/common/lifecycle';
import { IActiveCodeEditor, ICodeEditor, IDiffEditorConstructionOptions } from 'vs/editor/browser/editorBrowser';
import { EditorLayoutInfo, EditorOption } from 'vs/editor/common/config/editorOptions';
import { IRange, Range } from 'vs/editor/common/core/range';
import { localize } from 'vs/nls';
import { IContextKey, IContextKeyService } from 'vs/platform/contextkey/common/contextkey';
import { IInstantiationService } from 'vs/platform/instantiation/common/instantiation';
import { ZoneWidget } from 'vs/editor/contrib/zoneWidget/browser/zoneWidget';
import { CTX_INLINE_CHAT_FOCUSED, CTX_INLINE_CHAT_EMPTY, CTX_INLINE_CHAT_OUTER_CURSOR_POSITION, CTX_INLINE_CHAT_VISIBLE, MENU_INLINE_CHAT_WIDGET, MENU_INLINE_CHAT_WIDGET_STATUS, MENU_INLINE_CHAT_WIDGET_MARKDOWN_MESSAGE, CTX_INLINE_CHAT_MESSAGE_CROP_STATE, IInlineChatSlashCommand, MENU_INLINE_CHAT_WIDGET_FEEDBACK, ACTION_REGENERATE_RESPONSE, ACTION_VIEW_IN_CHAT, MENU_INLINE_CHAT_WIDGET_TOGGLE, CTX_INLINE_CHAT_INNER_CURSOR_FIRST, CTX_INLINE_CHAT_INNER_CURSOR_LAST, CTX_INLINE_CHAT_INNER_CURSOR_START, CTX_INLINE_CHAT_INNER_CURSOR_END, CTX_INLINE_CHAT_RESPONSE_FOCUSED, ACTION_ACCEPT_CHANGES } from 'vs/workbench/contrib/inlineChat/common/inlineChat';
import { IModelDeltaDecoration, ITextModel } from 'vs/editor/common/model';
import { EventType, Dimension, addDisposableListener, getActiveElement, getTotalHeight, getTotalWidth, h, reset, getWindow } from 'vs/base/browser/dom';
import { Emitter, Event, MicrotaskEmitter } from 'vs/base/common/event';
import { IEditorConstructionOptions } from 'vs/editor/browser/config/editorConfiguration';
import { ICodeEditorWidgetOptions } from 'vs/editor/browser/widget/codeEditorWidget';
import { EditorExtensionsRegistry } from 'vs/editor/browser/editorExtensions';
import { SnippetController2 } from 'vs/editor/contrib/snippet/browser/snippetController2';
import { IModelService } from 'vs/editor/common/services/model';
import { URI } from 'vs/base/common/uri';
import { EmbeddedCodeEditorWidget, EmbeddedDiffEditorWidget } from 'vs/editor/browser/widget/embeddedCodeEditorWidget';
import { HiddenItemStrategy, MenuWorkbenchToolBar } from 'vs/platform/actions/browser/toolbar';
import { ProgressBar } from 'vs/base/browser/ui/progressbar/progressbar';
import { SuggestController } from 'vs/editor/contrib/suggest/browser/suggestController';
import { Position } from 'vs/editor/common/core/position';
import { DEFAULT_FONT_FAMILY } from 'vs/workbench/browser/style';
import { CompletionItem, CompletionItemInsertTextRule, CompletionItemKind, CompletionItemProvider, CompletionList, ProviderResult } from 'vs/editor/common/languages';
import { ISingleEditOperation } from 'vs/editor/common/core/editOperation';
import { ILanguageSelection } from 'vs/editor/common/languages/language';
import { ResourceLabel } from 'vs/workbench/browser/labels';
import { FileKind } from 'vs/platform/files/common/files';
import { ILanguageFeaturesService } from 'vs/editor/common/services/languageFeatures';
import { LanguageSelector } from 'vs/editor/common/languageSelector';
import { createTextBufferFactoryFromSnapshot } from 'vs/editor/common/model/textModel';
import { invertLineRange, lineRangeAsRange } from 'vs/workbench/contrib/inlineChat/browser/utils';
import { ICodeEditorViewState, ScrollType } from 'vs/editor/common/editorCommon';
import { LineRange } from 'vs/editor/common/core/lineRange';
import { IAccessibilityService } from 'vs/platform/accessibility/common/accessibility';
import { IConfigurationService } from 'vs/platform/configuration/common/configuration';
import { IKeybindingService } from 'vs/platform/keybinding/common/keybinding';
import { AccessibilityVerbositySettingId } from 'vs/workbench/contrib/accessibility/browser/accessibilityConfiguration';
import { renderLabelWithIcons } from 'vs/base/browser/ui/iconLabel/iconLabels';
import { ExpansionState } from 'vs/workbench/contrib/inlineChat/browser/inlineChatSession';
import * as aria from 'vs/base/browser/ui/aria/aria';
import { IWorkbenchButtonBarOptions, MenuWorkbenchButtonBar, WorkbenchButtonBar } from 'vs/platform/actions/browser/buttonbar';
import { SlashCommandContentWidget } from 'vs/workbench/contrib/chat/browser/chatSlashCommandContentWidget';
import { IContextMenuService } from 'vs/platform/contextview/browser/contextView';
import { IAccessibleViewService } from 'vs/workbench/contrib/accessibility/browser/accessibleView';
import { StandardMouseEvent } from 'vs/base/browser/mouseEvent';
import { AccessibilityCommandId } from 'vs/workbench/contrib/accessibility/common/accessibilityCommands';
import { assertType } from 'vs/base/common/types';
import { renderFormattedText } from 'vs/base/browser/formattedTextRenderer';
import { IMarkdownString, MarkdownString } from 'vs/base/common/htmlContent';
import { ChatEditorOptions } from 'vs/workbench/contrib/chat/browser/chatOptions';
import { MenuId } from 'vs/platform/actions/common/actions';
import { editorForeground, inputBackground, editorBackground } from 'vs/platform/theme/common/colorRegistry';
import { Lazy } from 'vs/base/common/lazy';
import { IEditorWorkerService } from 'vs/editor/common/services/editorWorker';
import { ChatResponseViewModel } from 'vs/workbench/contrib/chat/common/chatViewModel';
import { ChatModel, ChatResponseModel } from 'vs/workbench/contrib/chat/common/chatModel';
import { ILogService } from 'vs/platform/log/common/log';
import { ChatListItemRenderer, IChatListItemRendererOptions, IChatRendererDelegate } from 'vs/workbench/contrib/chat/browser/chatListRenderer';
import { IUntitledTextEditorModel } from 'vs/workbench/services/untitled/common/untitledTextEditorModel';
import { ITextModelService } from 'vs/editor/common/services/resolverService';
import { IChatReplyFollowup } from 'vs/workbench/contrib/chat/common/chatService';
import { IChatAgentService } from 'vs/workbench/contrib/chat/common/chatAgents';
import { ChatFollowups } from 'vs/workbench/contrib/chat/browser/chatFollowups';
import { IAction } from 'vs/base/common/actions';

const defaultAriaLabel = localize('aria-label', "Inline Chat Input");

export const _inputEditorOptions: IEditorConstructionOptions = {
	padding: { top: 2, bottom: 2 },
	overviewRulerLanes: 0,
	glyphMargin: false,
	lineNumbers: 'off',
	folding: false,
	hideCursorInOverviewRuler: true,
	selectOnLineNumbers: false,
	selectionHighlight: false,
	scrollbar: {
		useShadows: false,
		vertical: 'hidden',
		horizontal: 'auto',
		alwaysConsumeMouseWheel: false
	},
	lineDecorationsWidth: 0,
	overviewRulerBorder: false,
	scrollBeyondLastLine: false,
	renderLineHighlight: 'none',
	fixedOverflowWidgets: true,
	dragAndDrop: false,
	revealHorizontalRightPadding: 5,
	minimap: { enabled: false },
	guides: { indentation: false },
	rulers: [],
	cursorWidth: 1,
	cursorStyle: 'line',
	cursorBlinking: 'blink',
	wrappingStrategy: 'advanced',
	wrappingIndent: 'none',
	renderWhitespace: 'none',
	dropIntoEditor: { enabled: true },
	quickSuggestions: false,
	suggest: {
		showIcons: false,
		showSnippets: false,
		showStatusBar: false,
	},
	wordWrap: 'on',
	ariaLabel: defaultAriaLabel,
	fontFamily: DEFAULT_FONT_FAMILY,
	fontSize: 13,
	lineHeight: 20
};

const _previewEditorEditorOptions: IDiffEditorConstructionOptions = {
	scrollbar: { useShadows: false, alwaysConsumeMouseWheel: false, ignoreHorizontalScrollbarInContentHeight: true, },
	renderMarginRevertIcon: false,
	diffCodeLens: false,
	scrollBeyondLastLine: false,
	stickyScroll: { enabled: false },
	originalAriaLabel: localize('original', 'Original'),
	modifiedAriaLabel: localize('modified', 'Modified'),
	diffAlgorithm: 'advanced',
	readOnly: true,
	isInEmbeddedEditor: true
};

export interface InlineChatWidgetViewState {
	editorViewState: ICodeEditorViewState;
	input: string;
	placeholder: string;
}

export interface IInlineChatWidgetConstructionOptions {
	menuId: MenuId;
	statusMenuId: MenuId;
	feedbackMenuId: MenuId;
}

export interface IInlineChatMessage {
	message: IMarkdownString;
	requestId: string;
	providerId: string;
}

export interface IInlineChatMessageAppender {
	appendContent(fragment: string): void;
	cancel(): void;
	complete(): void;
}

export class InlineChatWidget {

	private static _modelPool: number = 1;

	private readonly _elements = h(
		'div.inline-chat@root',
		[
			h('div.body', [
				h('div.content@content', [
					h('div.input@input', [
						h('div.editor-placeholder@placeholder'),
						h('div.editor-container@editor'),
					]),
					h('div.toolbar@editorToolbar'),
				]),
			]),
			h('div.progress@progress'),
			h('div.previewDiff.hidden@previewDiff'),
			h('div.previewCreateTitle.show-file-icons@previewCreateTitle'),
			h('div.previewCreate.hidden@previewCreate'),
			h('div.chatMessage.hidden@chatMessage', [
				h('div.chatMessageContent@chatMessageContent'),
				h('div.messageActions@messageActions')
			]),
			h('div.followUps.hidden@followUps'),
			h('div.status@status', [
				h('div.label.info.hidden@infoLabel'),
				h('div.actions.hidden@extraToolbar'),
				h('div.actions.hidden@statusToolbar'),
				h('div.label.status.hidden@statusLabel'),
				h('div.actions.hidden@feedbackToolbar'),
			]),
		]
	);

	private readonly _store = new DisposableStore();
	private readonly _slashCommands = this._store.add(new DisposableStore());

	private readonly _inputEditor: IActiveCodeEditor;
	private readonly _inputModel: ITextModel;
	private readonly _ctxInputEmpty: IContextKey<boolean>;
	private readonly _ctxMessageCropState: IContextKey<'cropped' | 'not_cropped' | 'expanded'>;
	private readonly _ctxInnerCursorFirst: IContextKey<boolean>;
	private readonly _ctxInnerCursorLast: IContextKey<boolean>;
	private readonly _ctxInnerCursorStart: IContextKey<boolean>;
	private readonly _ctxInnerCursorEnd: IContextKey<boolean>;
	private readonly _ctxInputEditorFocused: IContextKey<boolean>;
	private readonly _ctxResponseFocused: IContextKey<boolean>;

	private readonly _progressBar: ProgressBar;

	private readonly _previewDiffEditor: Lazy<EmbeddedDiffEditorWidget>;
	private readonly _previewDiffModel = this._store.add(new MutableDisposable());

	private readonly _previewCreateTitle: ResourceLabel;
	private readonly _previewCreateEditor: Lazy<ICodeEditor>;
	private readonly _previewCreateDispoable = this._store.add(new MutableDisposable());

	private readonly _onDidChangeHeight = this._store.add(new MicrotaskEmitter<void>());
	readonly onDidChangeHeight: Event<void> = Event.filter(this._onDidChangeHeight.event, _ => !this._isLayouting);

	private readonly _onDidChangeLayout = this._store.add(new MicrotaskEmitter<void>());
	private readonly _onDidChangeInput = this._store.add(new Emitter<this>());
	readonly onDidChangeInput: Event<this> = this._onDidChangeInput.event;

	private _lastDim: Dimension | undefined;
	private _isLayouting: boolean = false;
	private _preferredExpansionState: ExpansionState | undefined;
	private _expansionState: ExpansionState = ExpansionState.NOT_CROPPED;
	private _slashCommandDetails: { command: string; detail: string }[] = [];

	private _slashCommandContentWidget: SlashCommandContentWidget;

	private readonly _editorOptions: ChatEditorOptions;
	private _chatMessageDisposables = this._store.add(new DisposableStore());
	private _followUpDisposables = this._store.add(new DisposableStore());

	private _chatMessage: MarkdownString | undefined;

	constructor(
		private readonly parentEditor: ICodeEditor,
		_options: IInlineChatWidgetConstructionOptions,
		@IModelService private readonly _modelService: IModelService,
		@IContextKeyService private readonly _contextKeyService: IContextKeyService,
		@ILanguageFeaturesService private readonly _languageFeaturesService: ILanguageFeaturesService,
		@IKeybindingService private readonly _keybindingService: IKeybindingService,
		@IInstantiationService private readonly _instantiationService: IInstantiationService,
		@IAccessibilityService private readonly _accessibilityService: IAccessibilityService,
		@IConfigurationService private readonly _configurationService: IConfigurationService,
		@IContextMenuService private readonly _contextMenuService: IContextMenuService,
		@IAccessibleViewService private readonly _accessibleViewService: IAccessibleViewService,
		@IEditorWorkerService private readonly _editorWorkerService: IEditorWorkerService,
		@ILogService private readonly _logService: ILogService,
		@ITextModelService private readonly _textModelResolverService: ITextModelService,
		@IChatAgentService private readonly _chatAgentService: IChatAgentService,
	) {

		// input editor logic
		const codeEditorWidgetOptions: ICodeEditorWidgetOptions = {
			isSimpleWidget: true,
			contributions: EditorExtensionsRegistry.getSomeEditorContributions([
				SnippetController2.ID,
				SuggestController.ID
			])
		};

		this._inputEditor = <IActiveCodeEditor>this._instantiationService.createInstance(EmbeddedCodeEditorWidget, this._elements.editor, _inputEditorOptions, codeEditorWidgetOptions, this.parentEditor);
		this._updateAriaLabel();
		this._store.add(this._inputEditor);
		this._store.add(this._inputEditor.onDidChangeModelContent(() => this._onDidChangeInput.fire(this)));
		this._store.add(this._inputEditor.onDidLayoutChange(() => this._onDidChangeHeight.fire()));
		this._store.add(this._inputEditor.onDidContentSizeChange(() => this._onDidChangeHeight.fire()));
		this._store.add(addDisposableListener(this._elements.chatMessageContent, 'focus', () => this._ctxResponseFocused.set(true)));
		this._store.add(addDisposableListener(this._elements.chatMessageContent, 'blur', () => this._ctxResponseFocused.reset()));

		this._store.add(this._configurationService.onDidChangeConfiguration(e => {
			if (e.affectsConfiguration(AccessibilityVerbositySettingId.InlineChat)) {
				this._updateAriaLabel();
			}
		}));

		const uri = URI.from({ scheme: 'vscode', authority: 'inline-chat', path: `/inline-chat/model${InlineChatWidget._modelPool++}.txt` });
		this._inputModel = this._store.add(this._modelService.getModel(uri) ?? this._modelService.createModel('', null, uri));
		this._inputEditor.setModel(this._inputModel);

		this._editorOptions = this._store.add(_instantiationService.createInstance(ChatEditorOptions, undefined, editorForeground, inputBackground, editorBackground));


		// --- context keys

		this._ctxMessageCropState = CTX_INLINE_CHAT_MESSAGE_CROP_STATE.bindTo(this._contextKeyService);
		this._ctxInputEmpty = CTX_INLINE_CHAT_EMPTY.bindTo(this._contextKeyService);

		this._ctxInnerCursorFirst = CTX_INLINE_CHAT_INNER_CURSOR_FIRST.bindTo(this._contextKeyService);
		this._ctxInnerCursorLast = CTX_INLINE_CHAT_INNER_CURSOR_LAST.bindTo(this._contextKeyService);
		this._ctxInnerCursorStart = CTX_INLINE_CHAT_INNER_CURSOR_START.bindTo(this._contextKeyService);
		this._ctxInnerCursorEnd = CTX_INLINE_CHAT_INNER_CURSOR_END.bindTo(this._contextKeyService);
		this._ctxInputEditorFocused = CTX_INLINE_CHAT_FOCUSED.bindTo(this._contextKeyService);
		this._ctxResponseFocused = CTX_INLINE_CHAT_RESPONSE_FOCUSED.bindTo(this._contextKeyService);

		// (1) inner cursor position (last/first line selected)
		const updateInnerCursorFirstLast = () => {
			const selection = this._inputEditor.getSelection();
			const fullRange = this._inputModel.getFullModelRange();
			let onFirst = false;
			let onLast = false;
			if (selection.isEmpty()) {
				const selectionTop = this._inputEditor.getTopForPosition(selection.startLineNumber, selection.startColumn);
				const firstViewLineTop = this._inputEditor.getTopForPosition(fullRange.startLineNumber, fullRange.startColumn);
				const lastViewLineTop = this._inputEditor.getTopForPosition(fullRange.endLineNumber, fullRange.endColumn);

				if (selectionTop === firstViewLineTop) {
					onFirst = true;
				}
				if (selectionTop === lastViewLineTop) {
					onLast = true;
				}
			}
			this._ctxInnerCursorFirst.set(onFirst);
			this._ctxInnerCursorLast.set(onLast);
			this._ctxInnerCursorStart.set(fullRange.getStartPosition().equals(selection.getStartPosition()));
			this._ctxInnerCursorEnd.set(fullRange.getEndPosition().equals(selection.getEndPosition()));
		};
		this._store.add(this._inputEditor.onDidChangeCursorPosition(updateInnerCursorFirstLast));
		updateInnerCursorFirstLast();

		// (2) input editor focused or not
		const updateFocused = () => {
			const hasFocus = this._inputEditor.hasWidgetFocus();
			this._ctxInputEditorFocused.set(hasFocus);
			this._elements.content.classList.toggle('synthetic-focus', hasFocus);
			this.readPlaceholder();
		};
		this._store.add(this._inputEditor.onDidFocusEditorWidget(updateFocused));
		this._store.add(this._inputEditor.onDidBlurEditorWidget(updateFocused));
		this._store.add(toDisposable(() => {
			this._ctxInnerCursorFirst.reset();
			this._ctxInnerCursorLast.reset();
			this._ctxInputEditorFocused.reset();
		}));
		updateFocused();

		// placeholder

		this._elements.placeholder.style.fontSize = `${this._inputEditor.getOption(EditorOption.fontSize)}px`;
		this._elements.placeholder.style.lineHeight = `${this._inputEditor.getOption(EditorOption.lineHeight)}px`;
		this._store.add(addDisposableListener(this._elements.placeholder, 'click', () => this._inputEditor.focus()));

		// show/hide placeholder depending on text model being empty
		// content height

		const currentContentHeight = 0;

		const togglePlaceholder = () => {
			const hasText = this._inputModel.getValueLength() > 0;
			this._elements.placeholder.classList.toggle('hidden', hasText);
			this._ctxInputEmpty.set(!hasText);
			this.readPlaceholder();

			const contentHeight = this._inputEditor.getContentHeight();
			if (contentHeight !== currentContentHeight && this._lastDim) {
				this._lastDim = this._lastDim.with(undefined, contentHeight);
				this._inputEditor.layout(this._lastDim);
				this._onDidChangeHeight.fire();
			}
		};
		this._store.add(this._inputModel.onDidChangeContent(togglePlaceholder));
		togglePlaceholder();

		// slash command content widget

		this._slashCommandContentWidget = new SlashCommandContentWidget(this._inputEditor);
		this._store.add(this._slashCommandContentWidget);

		// toolbars

		const toolbar = this._instantiationService.createInstance(MenuWorkbenchToolBar, this._elements.editorToolbar, _options.menuId, {
			telemetrySource: 'interactiveEditorWidget-toolbar',
			toolbarOptions: { primaryGroup: 'main' }
		});
		this._store.add(toolbar);

		this._progressBar = new ProgressBar(this._elements.progress);
		this._store.add(this._progressBar);

		const workbenchMenubarOptions: IWorkbenchButtonBarOptions = {
			telemetrySource: 'interactiveEditorWidget-toolbar',
			buttonConfigProvider: action => {
				if (action.id === ACTION_REGENERATE_RESPONSE) {
					return { showIcon: true, showLabel: false, isSecondary: true };
				} else if (action.id === ACTION_VIEW_IN_CHAT || action.id === ACTION_ACCEPT_CHANGES) {
					return { isSecondary: false };
				} else {
					return { isSecondary: true };
				}
			}
		};
		const statusButtonBar = this._instantiationService.createInstance(MenuWorkbenchButtonBar, this._elements.statusToolbar, _options.statusMenuId, workbenchMenubarOptions);
		this._store.add(statusButtonBar.onDidChange(() => this._onDidChangeHeight.fire()));
		this._store.add(statusButtonBar);


		const workbenchToolbarOptions = {
			hiddenItemStrategy: HiddenItemStrategy.NoHide,
			toolbarOptions: {
				primaryGroup: () => true,
				useSeparatorsInPrimaryActions: true
			}
		};

		const feedbackToolbar = this._instantiationService.createInstance(MenuWorkbenchToolBar, this._elements.feedbackToolbar, _options.feedbackMenuId, { ...workbenchToolbarOptions, hiddenItemStrategy: HiddenItemStrategy.Ignore });
		this._store.add(feedbackToolbar.onDidChangeMenuItems(() => this._onDidChangeHeight.fire()));
		this._store.add(feedbackToolbar);

		// preview editors
		this._previewDiffEditor = new Lazy(() => this._store.add(_instantiationService.createInstance(EmbeddedDiffEditorWidget, this._elements.previewDiff, {
			..._previewEditorEditorOptions,
			onlyShowAccessibleDiffViewer: this._accessibilityService.isScreenReaderOptimized(),
		}, { modifiedEditor: codeEditorWidgetOptions, originalEditor: codeEditorWidgetOptions }, parentEditor)));

		this._previewCreateTitle = this._store.add(_instantiationService.createInstance(ResourceLabel, this._elements.previewCreateTitle, { supportIcons: true }));
		this._previewCreateEditor = new Lazy(() => this._store.add(_instantiationService.createInstance(EmbeddedCodeEditorWidget, this._elements.previewCreate, _previewEditorEditorOptions, codeEditorWidgetOptions, parentEditor)));

		this._elements.chatMessageContent.tabIndex = 0;
		this._elements.chatMessageContent.ariaLabel = this._accessibleViewService.getOpenAriaHint(AccessibilityVerbositySettingId.InlineChat);
		this._elements.followUps.tabIndex = 0;
		this._elements.followUps.ariaLabel = this._accessibleViewService.getOpenAriaHint(AccessibilityVerbositySettingId.InlineChat);

		this._elements.statusLabel.tabIndex = 0;
		const markdownMessageToolbar = this._instantiationService.createInstance(MenuWorkbenchToolBar, this._elements.messageActions, MENU_INLINE_CHAT_WIDGET_MARKDOWN_MESSAGE, workbenchToolbarOptions);
		this._store.add(markdownMessageToolbar.onDidChangeMenuItems(() => this._onDidChangeHeight.fire()));
		this._store.add(markdownMessageToolbar);

		this._store.add(addDisposableListener(this._elements.root, EventType.CONTEXT_MENU, async (event: MouseEvent) => {
			this._onContextMenu(event);
		}));
		this._store.add(this._configurationService.onDidChangeConfiguration(e => {
			if (e.affectsConfiguration(AccessibilityVerbositySettingId.InlineChat)) {
				this._elements.chatMessageContent.ariaLabel = this._accessibleViewService.getOpenAriaHint(AccessibilityVerbositySettingId.InlineChat);
				this._elements.followUps.ariaLabel = this._accessibleViewService.getOpenAriaHint(AccessibilityVerbositySettingId.InlineChat);
			}
		}));
	}

	private _onContextMenu(e: MouseEvent) {
		const event = new StandardMouseEvent(getWindow(this._elements.root), e);
		this._contextMenuService.showContextMenu({
			menuId: MENU_INLINE_CHAT_WIDGET_TOGGLE,
			getAnchor: () => event,
		});
	}

	private _updateAriaLabel(): void {
		if (!this._accessibilityService.isScreenReaderOptimized()) {
			return;
		}
		let label = defaultAriaLabel;
		if (this._configurationService.getValue<boolean>(AccessibilityVerbositySettingId.InlineChat)) {
			const kbLabel = this._keybindingService.lookupKeybinding(AccessibilityCommandId.OpenAccessibilityHelp)?.getLabel();
			label = kbLabel ? localize('inlineChat.accessibilityHelp', "Inline Chat Input, Use {0} for Inline Chat Accessibility Help.", kbLabel) : localize('inlineChat.accessibilityHelpNoKb', "Inline Chat Input, Run the Inline Chat Accessibility Help command for more information.");
		}
		_inputEditorOptions.ariaLabel = label;
		this._inputEditor.updateOptions({ ariaLabel: label });
	}

	dispose(): void {
		this._store.dispose();
		this._ctxInputEmpty.reset();
		this._ctxMessageCropState.reset();
	}

	get domNode(): HTMLElement {
		return this._elements.root;
	}

	layout(dim: Dimension) {
		this._isLayouting = true;
		try {
			const innerEditorWidth = dim.width - (getTotalWidth(this._elements.editorToolbar) + 8 /* L/R-padding */);
			dim = new Dimension(innerEditorWidth, dim.height);
			if (!this._lastDim || !Dimension.equals(this._lastDim, dim)) {
				this._lastDim = dim;
				this._inputEditor.layout(new Dimension(innerEditorWidth, this._inputEditor.getContentHeight()));
				this._elements.placeholder.style.width = `${innerEditorWidth  /* input-padding*/}px`;

				if (this._previewDiffEditor.hasValue) {
					const previewDiffDim = new Dimension(dim.width, Math.min(300, Math.max(0, this._previewDiffEditor.value.getContentHeight())));
					this._previewDiffEditor.value.layout(previewDiffDim);
					this._elements.previewDiff.style.height = `${previewDiffDim.height}px`;
				}

				if (this._previewCreateEditor.hasValue) {
					const previewCreateDim = new Dimension(dim.width, Math.min(300, Math.max(0, this._previewCreateEditor.value.getContentHeight())));
					this._previewCreateEditor.value.layout(previewCreateDim);
					this._elements.previewCreate.style.height = `${previewCreateDim.height}px`;
				}

				const lineHeight = this.parentEditor.getOption(EditorOption.lineHeight);
				const editorHeight = this.parentEditor.getLayoutInfo().height;
				const editorHeightInLines = Math.floor(editorHeight / lineHeight);
				this._elements.root.style.setProperty('--vscode-inline-chat-cropped', String(Math.floor(editorHeightInLines / 5)));
				this._elements.root.style.setProperty('--vscode-inline-chat-expanded', String(Math.floor(editorHeightInLines / 3)));
				this._onDidChangeLayout.fire();
			}
		} finally {
			this._isLayouting = false;
		}
	}

	getHeight(): number {
		const base = getTotalHeight(this._elements.progress) + getTotalHeight(this._elements.status);
		const editorHeight = this._inputEditor.getContentHeight() + 12 /* padding and border */;
		const followUpsHeight = getTotalHeight(this._elements.followUps);
		const chatResponseHeight = getTotalHeight(this._elements.chatMessage);
		const previewDiffHeight = this._previewDiffEditor.hasValue && this._previewDiffEditor.value.getModel() ? 12 + Math.min(300, Math.max(0, this._previewDiffEditor.value.getContentHeight())) : 0;
		const previewCreateTitleHeight = getTotalHeight(this._elements.previewCreateTitle);
		const previewCreateHeight = this._previewCreateEditor.hasValue && this._previewCreateEditor.value.getModel() ? 18 + Math.min(300, Math.max(0, this._previewCreateEditor.value.getContentHeight())) : 0;
		return base + editorHeight + followUpsHeight + chatResponseHeight + previewDiffHeight + previewCreateTitleHeight + previewCreateHeight + 18 /* padding */ + 8 /*shadow*/;
	}

	updateProgress(show: boolean) {
		if (show) {
			this._progressBar.infinite();
		} else {
			this._progressBar.stop();
		}
	}

	get value(): string {
		return this._inputModel.getValue();
	}

	set value(value: string) {
		this._inputModel.setValue(value);
		this._inputEditor.setPosition(this._inputModel.getFullModelRange().getEndPosition());
	}

	selectAll(includeSlashCommand: boolean = true) {
		let selection = this._inputModel.getFullModelRange();

		if (!includeSlashCommand) {
			const firstLine = this._inputModel.getLineContent(1);
			const slashCommand = this._slashCommandDetails.find(c => firstLine.startsWith(`/${c.command} `));
			selection = slashCommand ? new Range(1, slashCommand.command.length + 3, selection.endLineNumber, selection.endColumn) : selection;
		}

		this._inputEditor.setSelection(selection);
	}

	set placeholder(value: string) {
		this._elements.placeholder.innerText = value;
	}

	readPlaceholder(): void {
		const slashCommand = this._slashCommandDetails.find(c => `${c.command} ` === this._inputModel.getValue().substring(1));
		const hasText = this._inputModel.getValueLength() > 0;
		if (!hasText) {
			aria.status(this._elements.placeholder.innerText);
		} else if (slashCommand) {
			aria.status(slashCommand.detail);
		}
	}

	updateToolbar(show: boolean) {
		this._elements.statusToolbar.classList.toggle('hidden', !show);
		this._elements.feedbackToolbar.classList.toggle('hidden', !show);
		this._elements.status.classList.toggle('actions', show);
		this._elements.infoLabel.classList.toggle('hidden', show);
		this._onDidChangeHeight.fire();
	}

	private _extraButtonsCleanup = this._store.add(new MutableDisposable());

	setExtraButtons(buttons: IAction[]) {
		const bar = this._instantiationService.createInstance(WorkbenchButtonBar, this._elements.extraToolbar, { telemetrySource: 'inlineChat' });
		bar.update(buttons);
		this._elements.extraToolbar.classList.toggle('hidden', buttons.length === 0);
		this._extraButtonsCleanup.value = bar;
	}

	get expansionState(): ExpansionState {
		return this._expansionState;
	}

	set preferredExpansionState(expansionState: ExpansionState | undefined) {
		this._preferredExpansionState = expansionState;
	}

	get responseContent(): string | undefined {
		return this._chatMessage?.value;
	}

	updateChatMessage(message: IInlineChatMessage, isIncomplete: true): IInlineChatMessageAppender;
	updateChatMessage(message: IInlineChatMessage | undefined): void;
	updateChatMessage(message: IInlineChatMessage | undefined, isIncomplete?: boolean): IInlineChatMessageAppender | undefined {
		let expansionState: ExpansionState;
		this._chatMessageDisposables.clear();
		this._chatMessage = message ? new MarkdownString(message.message.value) : undefined;
		const hasMessage = message?.message.value;
		this._elements.chatMessage.classList.toggle('hidden', !hasMessage);
		reset(this._elements.chatMessageContent);
		let resultingAppender: IInlineChatMessageAppender | undefined;
		if (!hasMessage) {
			this._ctxMessageCropState.reset();
			expansionState = ExpansionState.NOT_CROPPED;
		} else {
			const sessionModel = this._chatMessageDisposables.add(new ChatModel(message.providerId, undefined, this._logService, this._chatAgentService));
			const responseModel = this._chatMessageDisposables.add(new ChatResponseModel(message.message, sessionModel, undefined, message.requestId, !isIncomplete, false, undefined));
			const viewModel = this._chatMessageDisposables.add(new ChatResponseViewModel(responseModel, this._logService));
			const renderOptions: IChatListItemRendererOptions = { renderStyle: 'compact', noHeader: true, noPadding: true };
			const chatRendererDelegate: IChatRendererDelegate = { getListLength() { return 1; } };
			const renderer = this._chatMessageDisposables.add(this._instantiationService.createInstance(ChatListItemRenderer, this._editorOptions, renderOptions, chatRendererDelegate));
			renderer.layout(this._elements.chatMessageContent.clientWidth - 4); // 2 for the padding used for the tab index border
			this._chatMessageDisposables.add(this._onDidChangeLayout.event(() => {
				renderer.layout(this._elements.chatMessageContent.clientWidth - 4);
			}));
			const template = renderer.renderTemplate(this._elements.chatMessageContent);
			this._chatMessageDisposables.add(template.elementDisposables);
			this._chatMessageDisposables.add(template.templateDisposables);
			renderer.renderChatTreeItem(viewModel, 0, template);
			this._chatMessageDisposables.add(renderer.onDidChangeItemHeight(() => this._onDidChangeHeight.fire()));

			if (this._preferredExpansionState) {
				expansionState = this._preferredExpansionState;
				this._preferredExpansionState = undefined;
			} else {
				this._updateLineClamp(ExpansionState.CROPPED);
				expansionState = template.value.scrollHeight > template.value.clientHeight ? ExpansionState.CROPPED : ExpansionState.NOT_CROPPED;
			}
			this._ctxMessageCropState.set(expansionState);
			this._updateLineClamp(expansionState);
			resultingAppender = isIncomplete ? {
				cancel: () => responseModel.cancel(),
				complete: () => responseModel.complete(),
				appendContent: (fragment: string) => {
					responseModel.updateContent({ kind: 'markdownContent', content: new MarkdownString(fragment) });
					this._chatMessage?.appendMarkdown(fragment);
				}
			} : undefined;
		}
		this._expansionState = expansionState;
		this._onDidChangeHeight.fire();
		return resultingAppender;
	}

	updateFollowUps(items: IChatReplyFollowup[], onFollowup: (followup: IChatReplyFollowup) => void): void;
	updateFollowUps(items: undefined): void;
	updateFollowUps(items: IChatReplyFollowup[] | undefined, onFollowup?: ((followup: IChatReplyFollowup) => void)) {
		this._followUpDisposables.clear();
		this._elements.followUps.classList.toggle('hidden', !items || items.length === 0);
		reset(this._elements.followUps);
		if (items && items.length > 0 && onFollowup) {
			this._followUpDisposables.add(new ChatFollowups(this._elements.followUps, items, undefined, onFollowup, this._contextKeyService));
		}
		this._onDidChangeHeight.fire();
	}

	updateChatMessageExpansionState(expansionState: ExpansionState) {
		this._ctxMessageCropState.set(expansionState);
		const heightBefore = this._elements.chatMessageContent.scrollHeight;
		this._updateLineClamp(expansionState);
		const heightAfter = this._elements.chatMessageContent.scrollHeight;
		if (heightBefore === heightAfter) {
			this._ctxMessageCropState.set(ExpansionState.NOT_CROPPED);
		}
		this._onDidChangeHeight.fire();
	}

	private _updateLineClamp(expansionState: ExpansionState) {
		this._elements.chatMessageContent.setAttribute('state', expansionState);
	}

	updateSlashCommandUsed(command: string): void {
		const details = this._slashCommandDetails.find(candidate => candidate.command === command);
		if (!details) {
			return;
		}

		this._elements.infoLabel.classList.toggle('hidden', false);
		const label = localize('slashCommandUsed', "Using {0} to generate response...", `\`\`/${details.command}\`\``);

		const e = renderFormattedText(label, { inline: true, renderCodeSegments: true, className: 'slash-command-pill' });
		reset(this._elements.infoLabel, e);
		this._onDidChangeHeight.fire();
	}

	updateInfo(message: string): void {
		this._elements.infoLabel.classList.toggle('hidden', !message);
		const renderedMessage = renderLabelWithIcons(message);
		reset(this._elements.infoLabel, ...renderedMessage);
		this._onDidChangeHeight.fire();
	}

	updateStatus(message: string, ops: { classes?: string[]; resetAfter?: number; keepMessage?: boolean } = {}) {
		const isTempMessage = typeof ops.resetAfter === 'number';
		if (isTempMessage && !this._elements.statusLabel.dataset['state']) {
			const statusLabel = this._elements.statusLabel.innerText;
			const classes = Array.from(this._elements.statusLabel.classList.values());
			setTimeout(() => {
				this.updateStatus(statusLabel, { classes, keepMessage: true });
			}, ops.resetAfter);
		}
		reset(this._elements.statusLabel, message);
		this._elements.statusLabel.className = `label status ${(ops.classes ?? []).join(' ')}`;
		this._elements.statusLabel.classList.toggle('hidden', !message);
		if (isTempMessage) {
			this._elements.statusLabel.dataset['state'] = 'temp';
		} else {
			delete this._elements.statusLabel.dataset['state'];
		}
		this._onDidChangeHeight.fire();
	}

	reset() {
		this._ctxInputEmpty.reset();
		this._ctxInnerCursorFirst.reset();
		this._ctxInnerCursorLast.reset();
		this._ctxInputEditorFocused.reset();

		this.value = '';
		this.updateChatMessage(undefined);
		this.updateFollowUps(undefined);

		reset(this._elements.statusLabel);
		this._elements.statusLabel.classList.toggle('hidden', true);
		this._elements.extraToolbar.classList.add('hidden');
		this._elements.statusToolbar.classList.add('hidden');
		this._elements.feedbackToolbar.classList.add('hidden');
		this.hideCreatePreview();
		this.hideEditsPreview();
		this._onDidChangeHeight.fire();
	}

	focus() {
		this._inputEditor.focus();
	}

	hasFocus() {
		return this.domNode.contains(getActiveElement());
	}

	// --- preview

	async showEditsPreview(textModel0: ITextModel, textModelN: ITextModel, allEdits: ISingleEditOperation[][]) {

		this._elements.previewDiff.classList.remove('hidden');

		const languageSelection: ILanguageSelection = { languageId: textModel0.getLanguageId(), onDidChange: Event.None };
		const modified = this._modelService.createModel(createTextBufferFactoryFromSnapshot(textModel0.createSnapshot()), languageSelection, undefined, true);
		for (const edits of allEdits) {
			modified.applyEdits(edits, false);
		}

		const diff = await this._editorWorkerService.computeDiff(textModel0.uri, modified.uri, { ignoreTrimWhitespace: false, maxComputationTimeMs: 5000, computeMoves: false }, 'advanced');
		if (!diff || diff.changes.length === 0) {
			this.hideEditsPreview();
			return;
		}

		this._previewDiffEditor.value.setModel({ original: textModel0, modified });

		// joined ranges
		let originalLineRange = diff.changes[0].original;
		let modifiedLineRange = diff.changes[0].modified;
		for (let i = 1; i < diff.changes.length; i++) {
			originalLineRange = originalLineRange.join(diff.changes[i].original);
			modifiedLineRange = modifiedLineRange.join(diff.changes[i].modified);
		}

		// apply extra padding
		const pad = 3;
		const newStartLine = Math.max(1, originalLineRange.startLineNumber - pad);
		modifiedLineRange = new LineRange(newStartLine, modifiedLineRange.endLineNumberExclusive);
		originalLineRange = new LineRange(newStartLine, originalLineRange.endLineNumberExclusive);

		const newEndLineModified = Math.min(modifiedLineRange.endLineNumberExclusive + pad, modified.getLineCount());
		modifiedLineRange = new LineRange(modifiedLineRange.startLineNumber, newEndLineModified);
		const newEndLineOriginal = Math.min(originalLineRange.endLineNumberExclusive + pad, textModel0.getLineCount());
		originalLineRange = new LineRange(originalLineRange.startLineNumber, newEndLineOriginal);

		const hiddenOriginal = invertLineRange(originalLineRange, textModel0);
		const hiddenModified = invertLineRange(modifiedLineRange, modified);
		this._previewDiffEditor.value.getOriginalEditor().setHiddenAreas(hiddenOriginal.map(lineRangeAsRange), 'diff-hidden');
		this._previewDiffEditor.value.getModifiedEditor().setHiddenAreas(hiddenModified.map(lineRangeAsRange), 'diff-hidden');
		this._previewDiffEditor.value.revealLine(modifiedLineRange.startLineNumber, ScrollType.Immediate);

		this._onDidChangeHeight.fire();
	}

	hideEditsPreview() {
		this._elements.previewDiff.classList.add('hidden');
		if (this._previewDiffEditor.hasValue) {
			this._previewDiffEditor.value.setModel(null);
		}
		this._previewDiffModel.clear();
		this._onDidChangeHeight.fire();
	}

	async showCreatePreview(model: IUntitledTextEditorModel): Promise<void> {
		this._elements.previewCreateTitle.classList.remove('hidden');
		this._elements.previewCreate.classList.remove('hidden');

		const ref = await this._textModelResolverService.createModelReference(model.resource);
		this._previewCreateDispoable.value = ref;
		this._previewCreateTitle.element.setFile(model.resource, { fileKind: FileKind.FILE });

		this._previewCreateEditor.value.setModel(ref.object.textEditorModel);
		this._onDidChangeHeight.fire();
	}

	hideCreatePreview() {
		this._elements.previewCreateTitle.classList.add('hidden');
		this._elements.previewCreate.classList.add('hidden');
		this._previewCreateEditor.rawValue?.setModel(null);
		this._previewCreateDispoable.clear();
		this._previewCreateTitle.element.clear();
		this._onDidChangeHeight.fire();
	}

	showsAnyPreview() {
		return !this._elements.previewDiff.classList.contains('hidden') ||
			!this._elements.previewCreate.classList.contains('hidden');
	}

	// --- slash commands

	updateSlashCommands(commands: IInlineChatSlashCommand[]) {

		this._slashCommands.clear();

		if (commands.length === 0) {
			return;
		}
		this._slashCommandDetails = commands.filter(c => c.command && c.detail).map(c => { return { command: c.command!, detail: c.detail! }; });

		const selector: LanguageSelector = { scheme: this._inputModel.uri.scheme, pattern: this._inputModel.uri.path, language: this._inputModel.getLanguageId() };
		this._slashCommands.add(this._languageFeaturesService.completionProvider.register(selector, new class implements CompletionItemProvider {

			_debugDisplayName: string = 'InlineChatSlashCommandProvider';

			readonly triggerCharacters?: string[] = ['/'];

			provideCompletionItems(_model: ITextModel, position: Position): ProviderResult<CompletionList> {
				if (position.lineNumber !== 1 && position.column !== 1) {
					return undefined;
				}

				const suggestions: CompletionItem[] = commands.map(command => {

					const withSlash = `/${command.command}`;

					return {
						label: { label: withSlash, description: command.detail },
						insertText: `${withSlash} $0`,
						insertTextRules: CompletionItemInsertTextRule.InsertAsSnippet,
						kind: CompletionItemKind.Text,
						range: new Range(1, 1, 1, 1),
						command: command.executeImmediately ? { id: 'inlineChat.accept', title: withSlash } : undefined
					};
				});

				return { suggestions };
			}
		}));

		const decorations = this._inputEditor.createDecorationsCollection();

		const updateSlashDecorations = () => {
			this._slashCommandContentWidget.hide();

			const newDecorations: IModelDeltaDecoration[] = [];
			for (const command of commands) {
				const withSlash = `/${command.command}`;
				const firstLine = this._inputModel.getLineContent(1);
				if (firstLine.startsWith(withSlash)) {
					newDecorations.push({
						range: new Range(1, 1, 1, withSlash.length + 1),
						options: {
							description: 'inline-chat-slash-command',
							inlineClassName: 'inline-chat-slash-command',
							after: {
								// Force some space between slash command and placeholder
								content: ' '
							}
						}
					});

					this._slashCommandContentWidget.setCommandText(command.command);
					this._slashCommandContentWidget.show();

					// inject detail when otherwise empty
					if (firstLine === `/${command.command}`) {
						newDecorations.push({
							range: new Range(1, withSlash.length + 1, 1, withSlash.length + 2),
							options: {
								description: 'inline-chat-slash-command-detail',
								after: {
									content: `${command.detail}`,
									inlineClassName: 'inline-chat-slash-command-detail'
								}
							}
						});
					}
					break;
				}
			}
			decorations.set(newDecorations);
		};

		this._slashCommands.add(this._inputEditor.onDidChangeModelContent(updateSlashDecorations));
		updateSlashDecorations();
	}
}

export class InlineChatZoneWidget extends ZoneWidget {

	readonly widget: InlineChatWidget;

	private readonly _ctxVisible: IContextKey<boolean>;
	private readonly _ctxCursorPosition: IContextKey<'above' | 'below' | ''>;
	private _dimension?: Dimension;
	private _indentationWidth: number | undefined;

	constructor(
		editor: ICodeEditor,
		@IInstantiationService private readonly _instaService: IInstantiationService,
		@IContextKeyService contextKeyService: IContextKeyService,
	) {
		super(editor, { showFrame: false, showArrow: false, isAccessible: true, className: 'inline-chat-widget', keepEditorSelection: true, showInHiddenAreas: true, ordinal: 10000 });

		this._ctxVisible = CTX_INLINE_CHAT_VISIBLE.bindTo(contextKeyService);
		this._ctxCursorPosition = CTX_INLINE_CHAT_OUTER_CURSOR_POSITION.bindTo(contextKeyService);

		this._disposables.add(toDisposable(() => {
			this._ctxVisible.reset();
			this._ctxCursorPosition.reset();
		}));

		this.widget = this._instaService.createInstance(InlineChatWidget, this.editor, {
			menuId: MENU_INLINE_CHAT_WIDGET,
			statusMenuId: MENU_INLINE_CHAT_WIDGET_STATUS,
			feedbackMenuId: MENU_INLINE_CHAT_WIDGET_FEEDBACK
		});
		this._disposables.add(this.widget.onDidChangeHeight(() => this._relayout()));
		this._disposables.add(this.widget);
		this.create();


		this._disposables.add(addDisposableListener(this.domNode, 'click', e => {
			if (!this.widget.hasFocus()) {
				this.widget.focus();
			}
		}, true));

		// todo@jrieken listen ONLY when showing
		const updateCursorIsAboveContextKey = () => {
			if (!this.position || !this.editor.hasModel()) {
				this._ctxCursorPosition.reset();
			} else if (this.position.lineNumber === this.editor.getPosition().lineNumber) {
				this._ctxCursorPosition.set('above');
			} else if (this.position.lineNumber + 1 === this.editor.getPosition().lineNumber) {
				this._ctxCursorPosition.set('below');
			} else {
				this._ctxCursorPosition.reset();
			}
		};
		this._disposables.add(this.editor.onDidChangeCursorPosition(e => updateCursorIsAboveContextKey()));
		this._disposables.add(this.editor.onDidFocusEditorText(e => updateCursorIsAboveContextKey()));
		updateCursorIsAboveContextKey();
	}

	protected override _fillContainer(container: HTMLElement): void {
		container.appendChild(this.widget.domNode);
	}


	protected override _doLayout(heightInPixel: number): void {

		const maxWidth = !this.widget.showsAnyPreview() ? 640 : Number.MAX_SAFE_INTEGER;
		const width = Math.min(maxWidth, this._availableSpaceGivenIndentation(this._indentationWidth));
		this._dimension = new Dimension(width, heightInPixel);
		this.widget.domNode.style.width = `${width}px`;
		this.widget.layout(this._dimension);
	}

	private _availableSpaceGivenIndentation(indentationWidth: number | undefined): number {
		const info = this.editor.getLayoutInfo();
		return info.contentWidth - (info.glyphMarginWidth + info.decorationsWidth + (indentationWidth ?? 0));
	}

	private _computeHeightInLines(): number {
		const lineHeight = this.editor.getOption(EditorOption.lineHeight);
		return this.widget.getHeight() / lineHeight;
	}

	protected override _relayout() {
		if (this._dimension) {
			this._doLayout(this._dimension.height);
		}
		super._relayout(this._computeHeightInLines());
	}

	override show(position: Position): void {
		super.show(position, this._computeHeightInLines());
		this.widget.focus();
		this._ctxVisible.set(true);
	}

	protected override _getWidth(info: EditorLayoutInfo): number {
		return info.width - info.minimap.minimapWidth;
	}

	updateBackgroundColor(newPosition: Position, wholeRange: IRange) {
		assertType(this.container);
		const widgetLineNumber = newPosition.lineNumber;
		this.container.classList.toggle('inside-selection', widgetLineNumber > wholeRange.startLineNumber && widgetLineNumber < wholeRange.endLineNumber);
	}

	private _calculateIndentationWidth(position: Position): number {
		const viewModel = this.editor._getViewModel();
		if (!viewModel) {
			return 0;
		}
		const visibleRange = viewModel.getCompletelyVisibleViewRange();
		const startLineVisibleRange = visibleRange.startLineNumber;
		const positionLine = position.lineNumber;
		let indentationLineNumber: number | undefined;
		let indentationLevel: number | undefined;
		for (let lineNumber = positionLine; lineNumber >= startLineVisibleRange; lineNumber--) {
			const currentIndentationLevel = viewModel.getLineFirstNonWhitespaceColumn(lineNumber);
			if (currentIndentationLevel !== 0) {
				indentationLineNumber = lineNumber;
				indentationLevel = currentIndentationLevel;
				break;
			}
		}
		return this.editor.getOffsetForColumn(indentationLineNumber ?? positionLine, indentationLevel ?? viewModel.getLineFirstNonWhitespaceColumn(positionLine));
	}

	setContainerMargins(): void {
		assertType(this.container);

		const info = this.editor.getLayoutInfo();
		const marginWithoutIndentation = info.glyphMarginWidth + info.decorationsWidth + info.lineNumbersWidth;
		this.container.style.marginLeft = `${marginWithoutIndentation}px`;
	}

	setWidgetMargins(position: Position, indentationWidth?: number): void {
		if (indentationWidth === undefined) {
			indentationWidth = this._calculateIndentationWidth(position);
		}
		if (this._indentationWidth === indentationWidth) {
			return;
		}
		this._indentationWidth = this._availableSpaceGivenIndentation(indentationWidth) > 400 ? indentationWidth : 0;
		this.widget.domNode.style.marginLeft = `${this._indentationWidth}px`;
		this.widget.domNode.style.marginRight = `${this.editor.getLayoutInfo().minimap.minimapWidth}px`;
	}

	override hide(): void {
		this.container!.classList.remove('inside-selection');
		this._ctxVisible.reset();
		this._ctxCursorPosition.reset();
		this.widget.reset();
		super.hide();
		aria.status(localize('inlineChatClosed', 'Closed inline chat widget'));
	}
}
