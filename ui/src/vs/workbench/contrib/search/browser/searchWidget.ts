/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as dom from 'vs/base/browser/dom';
import { IKeyboardEvent } from 'vs/base/browser/keyboardEvent';
import { FindInput, IFindInputOptions } from 'vs/base/browser/ui/findinput/findInput';
import { IMessage, InputBox } from 'vs/base/browser/ui/inputbox/inputBox';
import { Widget } from 'vs/base/browser/ui/widget';
import { Action } from 'vs/base/common/actions';
import { Emitter, Event } from 'vs/base/common/event';
import { KeyCode, KeyMod } from 'vs/base/common/keyCodes';
import { CONTEXT_FIND_WIDGET_NOT_VISIBLE } from 'vs/editor/contrib/find/findModel';
import * as nls from 'vs/nls';
import { IClipboardService } from 'vs/platform/clipboard/common/clipboardService';
import { IConfigurationService } from 'vs/platform/configuration/common/configuration';
import { ContextKeyExpr, IContextKey, IContextKeyService } from 'vs/platform/contextkey/common/contextkey';
import { IContextViewService } from 'vs/platform/contextview/browser/contextView';
import { IKeybindingService } from 'vs/platform/keybinding/common/keybinding';
import { KeybindingsRegistry, KeybindingWeight } from 'vs/platform/keybinding/common/keybindingsRegistry';
import { ISearchConfigurationProperties } from 'vs/workbench/services/search/common/search';
import { attachFindReplaceInputBoxStyler, attachInputBoxStyler } from 'vs/platform/theme/common/styler';
import { IThemeService } from 'vs/platform/theme/common/themeService';
import { ContextScopedFindInput } from 'vs/platform/browser/contextScopedHistoryWidget';
import { appendKeyBindingLabel, isSearchViewFocused, getSearchView } from 'vs/workbench/contrib/search/browser/searchActions';
import * as Constants from 'vs/workbench/contrib/search/common/constants';
import { IAccessibilityService } from 'vs/platform/accessibility/common/accessibility';
import { isMacintosh } from 'vs/base/common/platform';
import { Checkbox } from 'vs/base/browser/ui/checkbox/checkbox';
import { IViewsService } from 'vs/workbench/common/views';
import { searchReplaceAllIcon, searchShowContextIcon } from 'vs/workbench/contrib/search/browser/searchIcons';

/** Specified in searchview.css */
export const SingleLineInputHeight = 24;

export interface ISearchWidgetOptions {
	value?: string;
	replaceValue?: string;
	isRegex?: boolean;
	isCaseSensitive?: boolean;
	isWholeWords?: boolean;
	searchHistory?: string[];
	replaceHistory?: string[];
	preserveCase?: boolean;
	_hideReplaceToggle?: boolean; // TODO: Search Editor's replace experience
	showContextToggle?: boolean;
}

class ReplaceAllAction extends Action {

	static readonly ID: string = 'search.action.replaceAll';

	constructor(private _searchWidget: SearchWidget) {
		super(ReplaceAllAction.ID, '', searchReplaceAllIcon.classNames, false);
	}

	set searchWidget(searchWidget: SearchWidget) {
		this._searchWidget = searchWidget;
	}

	run(): Promise<any> {
		if (this._searchWidget) {
			/* AGPL */
			// return this._searchWidget.triggerReplaceAll();
			/* End AGPL */
		}
		return Promise.resolve(null);
	}
}

const ctrlKeyMod = (isMacintosh ? KeyMod.WinCtrl : KeyMod.CtrlCmd);

function stopPropagationForMultiLineUpwards(event: IKeyboardEvent, value: string, textarea: HTMLTextAreaElement | null) {
	const isMultiline = !!value.match(/\n/);
	if (textarea && (isMultiline || textarea.clientHeight > SingleLineInputHeight) && textarea.selectionStart > 0) {
		event.stopPropagation();
		return;
	}
}

function stopPropagationForMultiLineDownwards(event: IKeyboardEvent, value: string, textarea: HTMLTextAreaElement | null) {
	const isMultiline = !!value.match(/\n/);
	if (textarea && (isMultiline || textarea.clientHeight > SingleLineInputHeight) && textarea.selectionEnd < textarea.value.length) {
		event.stopPropagation();
		return;
	}
}

export class SearchWidget extends Widget {

	domNode!: HTMLElement;

	searchInput!: FindInput;
	searchInputFocusTracker!: dom.IFocusTracker;
	private searchInputBoxFocused: IContextKey<boolean>;

	private ignoreGlobalFindBufferOnNextFocus = false;
	private previousGlobalFindBufferValue: string | null = null;

	private _onSearchSubmit = this._register(new Emitter<{ triggeredOnType: boolean, delay: number }>());
	readonly onSearchSubmit: Event<{ triggeredOnType: boolean, delay: number }> = this._onSearchSubmit.event;

	private _onSearchCancel = this._register(new Emitter<{ focus: boolean }>());
	readonly onSearchCancel: Event<{ focus: boolean }> = this._onSearchCancel.event;

	private _onPreserveCaseChange = this._register(new Emitter<boolean>());
	readonly onPreserveCaseChange: Event<boolean> = this._onPreserveCaseChange.event;

	private _onBlur = this._register(new Emitter<void>());
	readonly onBlur: Event<void> = this._onBlur.event;

	private _onDidHeightChange = this._register(new Emitter<void>());
	readonly onDidHeightChange: Event<void> = this._onDidHeightChange.event;

	private readonly _onDidToggleContext = new Emitter<void>();
	readonly onDidToggleContext: Event<void> = this._onDidToggleContext.event;

	private showContextCheckbox!: Checkbox;
	private contextLinesInput!: InputBox;

	constructor(
		container: HTMLElement,
		options: ISearchWidgetOptions,
		@IContextViewService private readonly contextViewService: IContextViewService,
		@IThemeService private readonly themeService: IThemeService,
		@IContextKeyService private readonly contextKeyService: IContextKeyService,
		@IKeybindingService private readonly keyBindingService: IKeybindingService,
		@IClipboardService private readonly clipboardServce: IClipboardService,
		@IConfigurationService private readonly configurationService: IConfigurationService,
		@IAccessibilityService private readonly accessibilityService: IAccessibilityService
	) {
		super();
		this.searchInputBoxFocused = Constants.SearchInputBoxFocusedKey.bindTo(this.contextKeyService);

		this.render(container, options);

		this.configurationService.onDidChangeConfiguration(e => {
			if (e.affectsConfiguration('editor.accessibilitySupport')) {
				this.updateAccessibilitySupport();
			}
		});
		this.accessibilityService.onDidChangeScreenReaderOptimized(() => this.updateAccessibilitySupport());
		this.updateAccessibilitySupport();
	}

	focus(select: boolean = true, focusReplace: boolean = false, suppressGlobalSearchBuffer = false): void {
		this.ignoreGlobalFindBufferOnNextFocus = suppressGlobalSearchBuffer;

		/* AGPL */
		this.searchInput.focus();
		if (select) {
			this.searchInput.select();
		}
		/* End AGPL */
	}

	setWidth(width: number) {
		this.searchInput.inputBox.layout();
	}

	clear() {
		this.searchInput.clear();
	}

	getSearchHistory(): string[] {
		return this.searchInput.inputBox.getHistory();
	}

	clearHistory(): void {
		this.searchInput.inputBox.clearHistory();
	}

	showNextSearchTerm() {
		this.searchInput.inputBox.showNextValue();
	}

	showPreviousSearchTerm() {
		this.searchInput.inputBox.showPreviousValue();
	}

	searchInputHasFocus(): boolean {
		return !!this.searchInputBoxFocused.get();
	}

	focusRegexAction(): void {
		this.searchInput.focusOnRegex();
	}

	private render(container: HTMLElement, options: ISearchWidgetOptions): void {
		this.domNode = dom.append(container, dom.$('.search-widget'));
		this.domNode.style.position = 'relative';

		this.renderSearchInput(this.domNode, options);
	}

	private updateAccessibilitySupport(): void {
		this.searchInput.setFocusInputOnOptionClick(!this.accessibilityService.isScreenReaderOptimized());
	}

	private renderSearchInput(parent: HTMLElement, options: ISearchWidgetOptions): void {
		const inputOptions: IFindInputOptions = {
			label: nls.localize('label.Search', 'Search: Type Search Term and press Enter to search or Escape to cancel'),
			validation: (value: string) => this.validateSearchInput(value),
			placeholder: nls.localize('search.placeHolder', "Search"),
			appendCaseSensitiveLabel: appendKeyBindingLabel('', this.keyBindingService.lookupKeybinding(Constants.ToggleCaseSensitiveCommandId), this.keyBindingService),
			appendWholeWordsLabel: appendKeyBindingLabel('', this.keyBindingService.lookupKeybinding(Constants.ToggleWholeWordCommandId), this.keyBindingService),
			appendRegexLabel: appendKeyBindingLabel('', this.keyBindingService.lookupKeybinding(Constants.ToggleRegexCommandId), this.keyBindingService),
			history: options.searchHistory,
			flexibleHeight: true
		};

		const searchInputContainer = dom.append(parent, dom.$('.search-container.input-box'));
		this.searchInput = this._register(new ContextScopedFindInput(searchInputContainer, this.contextViewService, inputOptions, this.contextKeyService, true));
		this._register(attachFindReplaceInputBoxStyler(this.searchInput, this.themeService));
		this.searchInput.onKeyDown((keyboardEvent: IKeyboardEvent) => this.onSearchInputKeyDown(keyboardEvent));
		this.searchInput.setValue(options.value || '');
		this.searchInput.setRegex(!!options.isRegex);
		this.searchInput.setCaseSensitive(!!options.isCaseSensitive);
		this.searchInput.setWholeWords(!!options.isWholeWords);
		this._register(this.searchInput.inputBox.onDidChange(() => this.onSearchInputChanged()));
		this._register(this.searchInput.inputBox.onDidHeightChange(() => this._onDidHeightChange.fire()));

		this.searchInputFocusTracker = this._register(dom.trackFocus(this.searchInput.inputBox.inputElement));
		this._register(this.searchInputFocusTracker.onDidFocus(async () => {
			this.searchInputBoxFocused.set(true);

			const useGlobalFindBuffer = this.searchConfiguration.globalFindClipboard;
			if (!this.ignoreGlobalFindBufferOnNextFocus && useGlobalFindBuffer) {
				const globalBufferText = await this.clipboardServce.readFindText();
				if (this.previousGlobalFindBufferValue !== globalBufferText) {
					this.searchInput.inputBox.addToHistory();
					this.searchInput.setValue(globalBufferText);
					this.searchInput.select();
				}

				this.previousGlobalFindBufferValue = globalBufferText;
			}

			this.ignoreGlobalFindBufferOnNextFocus = false;
		}));
		this._register(this.searchInputFocusTracker.onDidBlur(() => this.searchInputBoxFocused.set(false)));


		this.showContextCheckbox = new Checkbox({ isChecked: false, title: nls.localize('showContext', "Show Context"), icon: searchShowContextIcon });
		this._register(this.showContextCheckbox.onChange(() => this.onContextLinesChanged()));

		if (options.showContextToggle) {
			this.contextLinesInput = new InputBox(searchInputContainer, this.contextViewService, { type: 'number' });
			dom.addClass(this.contextLinesInput.element, 'context-lines-input');
			this.contextLinesInput.value = '' + (this.configurationService.getValue<ISearchConfigurationProperties>('search').searchEditor.defaultNumberOfContextLines ?? 1);
			this._register(this.contextLinesInput.onDidChange(() => this.onContextLinesChanged()));
			this._register(attachInputBoxStyler(this.contextLinesInput, this.themeService));
			dom.append(searchInputContainer, this.showContextCheckbox.domNode);
		}
	}

	private onContextLinesChanged() {
		dom.toggleClass(this.domNode, 'show-context', this.showContextCheckbox.checked);
		this._onDidToggleContext.fire();

		if (this.contextLinesInput.value.includes('-')) {
			this.contextLinesInput.value = '0';
		}

		this._onDidToggleContext.fire();
	}

	public setContextLines(lines: number) {
		if (!this.contextLinesInput) { return; }
		if (lines === 0) {
			this.showContextCheckbox.checked = false;
		} else {
			this.showContextCheckbox.checked = true;
			this.contextLinesInput.value = '' + lines;
		}
		dom.toggleClass(this.domNode, 'show-context', this.showContextCheckbox.checked);
	}

	setValue(value: string) {
		this.searchInput.setValue(value);
	}

	private validateSearchInput(value: string): IMessage | null {
		if (value.length === 0) {
			return null;
		}
		if (!this.searchInput.getRegex()) {
			return null;
		}
		try {
			new RegExp(value, 'u');
		} catch (e) {
			return { content: e.message };
		}

		return null;
	}

	private onSearchInputChanged(): void {
		this.searchInput.clearMessage();

		if (this.searchConfiguration.searchOnType) {
			if (this.searchInput.getRegex()) {
				try {
					const regex = new RegExp(this.searchInput.getValue(), 'ug');
					const matchienessHeuristic = `
								~!@#$%^&*()_+
								\`1234567890-=
								qwertyuiop[]\\
								QWERTYUIOP{}|
								asdfghjkl;'
								ASDFGHJKL:"
								zxcvbnm,./
								ZXCVBNM<>? `.match(regex)?.length ?? 0;

					const delayMultiplier =
						matchienessHeuristic < 50 ? 1 :
							matchienessHeuristic < 100 ? 5 : // expressions like `.` or `\w`
								10; // only things matching empty string

					this.submitSearch(true, this.searchConfiguration.searchOnTypeDebouncePeriod * delayMultiplier);
				} catch {
					// pass
				}
			} else {
				this.submitSearch(true, this.searchConfiguration.searchOnTypeDebouncePeriod);
			}
		}
	}

	private onSearchInputKeyDown(keyboardEvent: IKeyboardEvent) {
		if (keyboardEvent.equals(ctrlKeyMod | KeyCode.Enter)) {
			this.searchInput.inputBox.insertAtCursor('\n');
			keyboardEvent.preventDefault();
		}

		if (keyboardEvent.equals(KeyCode.Enter)) {
			this.searchInput.onSearchSubmit();
			this.submitSearch();
			keyboardEvent.preventDefault();
		}

		else if (keyboardEvent.equals(KeyCode.Escape)) {
			this._onSearchCancel.fire({ focus: true });
			keyboardEvent.preventDefault();
		}

		else if (keyboardEvent.equals(KeyCode.Tab)) {
			/* AGPL */
			this.searchInput.focusOnCaseSensitive();
			/* End AGPL */

			keyboardEvent.preventDefault();
		}

		else if (keyboardEvent.equals(KeyCode.UpArrow)) {
			stopPropagationForMultiLineUpwards(keyboardEvent, this.searchInput.getValue(), this.searchInput.domNode.querySelector('textarea'));
		}

		else if (keyboardEvent.equals(KeyCode.DownArrow)) {
			stopPropagationForMultiLineDownwards(keyboardEvent, this.searchInput.getValue(), this.searchInput.domNode.querySelector('textarea'));
		}
	}



	private async submitSearch(triggeredOnType = false, delay: number = 0): Promise<void> {
		this.searchInput.validate();
		if (!this.searchInput.inputBox.isInputValid()) {
			return;
		}

		const value = this.searchInput.getValue();
		const useGlobalFindBuffer = this.searchConfiguration.globalFindClipboard;
		if (value && useGlobalFindBuffer) {
			await this.clipboardServce.writeFindText(value);
		}
		this._onSearchSubmit.fire({ triggeredOnType, delay });
	}

	getContextLines() {
		return this.showContextCheckbox.checked ? +this.contextLinesInput.value : 0;
	}

	modifyContextLines(increase: boolean) {
		const current = +this.contextLinesInput.value;
		const modified = current + (increase ? 1 : -1);
		this.showContextCheckbox.checked = modified !== 0;
		this.contextLinesInput.value = '' + modified;
	}

	toggleContextLines() {
		this.showContextCheckbox.checked = !this.showContextCheckbox.checked;
		this.onContextLinesChanged();
	}

	dispose(): void {
		super.dispose();
	}

	private get searchConfiguration(): ISearchConfigurationProperties {
		return this.configurationService.getValue<ISearchConfigurationProperties>('search');
	}
}

export function registerContributions() {
	KeybindingsRegistry.registerCommandAndKeybindingRule({
		id: ReplaceAllAction.ID,
		weight: KeybindingWeight.WorkbenchContrib,
		when: ContextKeyExpr.and(Constants.SearchViewVisibleKey, Constants.ReplaceActiveKey, CONTEXT_FIND_WIDGET_NOT_VISIBLE),
		primary: KeyMod.Alt | KeyMod.CtrlCmd | KeyCode.Enter,
		handler: accessor => {
			const viewsService = accessor.get(IViewsService);
			if (isSearchViewFocused(viewsService)) {
				const searchView = getSearchView(viewsService);
				if (searchView) {
					new ReplaceAllAction(searchView.searchAndReplaceWidget).run();
				}
			}
		}
	});
}
