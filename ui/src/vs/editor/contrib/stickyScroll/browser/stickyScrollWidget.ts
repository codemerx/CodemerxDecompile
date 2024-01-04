/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as dom from 'vs/base/browser/dom';
import { createTrustedTypesPolicy } from 'vs/base/browser/trustedTypes';
import { equals } from 'vs/base/common/arrays';
import { Disposable, DisposableStore } from 'vs/base/common/lifecycle';
import { ThemeIcon } from 'vs/base/common/themables';
import 'vs/css!./stickyScroll';
import { ICodeEditor, IOverlayWidget, IOverlayWidgetPosition } from 'vs/editor/browser/editorBrowser';
import { getColumnOfNodeOffset } from 'vs/editor/browser/viewParts/lines/viewLine';
import { EmbeddedCodeEditorWidget } from 'vs/editor/browser/widget/embeddedCodeEditorWidget';
import { EditorLayoutInfo, EditorOption, RenderLineNumbersType } from 'vs/editor/common/config/editorOptions';
import { Position } from 'vs/editor/common/core/position';
import { StringBuilder } from 'vs/editor/common/core/stringBuilder';
import { LineDecoration } from 'vs/editor/common/viewLayout/lineDecorations';
import { CharacterMapping, RenderLineInput, renderViewLine } from 'vs/editor/common/viewLayout/viewLineRenderer';
import { foldingCollapsedIcon, foldingExpandedIcon } from 'vs/editor/contrib/folding/browser/foldingDecorations';
import { FoldingModel } from 'vs/editor/contrib/folding/browser/foldingModel';

export class StickyScrollWidgetState {
	constructor(
		readonly startLineNumbers: number[],
		readonly endLineNumbers: number[],
		readonly lastLineRelativePosition: number,
		readonly showEndForLine: number | null = null
	) { }

	equals(other: StickyScrollWidgetState | undefined): boolean {
		return !!other
			&& this.lastLineRelativePosition === other.lastLineRelativePosition
			&& this.showEndForLine === other.showEndForLine
			&& equals(this.startLineNumbers, other.startLineNumbers)
			&& equals(this.endLineNumbers, other.endLineNumbers);
	}
}

const _ttPolicy = createTrustedTypesPolicy('stickyScrollViewLayer', { createHTML: value => value });
const STICKY_INDEX_ATTR = 'data-sticky-line-index';
const STICKY_IS_LINE_ATTR = 'data-sticky-is-line';
const STICKY_IS_LINE_NUMBER_ATTR = 'data-sticky-is-line-number';
const STICKY_IS_FOLDING_ICON_ATTR = 'data-sticky-is-folding-icon';

export class StickyScrollWidget extends Disposable implements IOverlayWidget {

	private readonly _foldingIconStore = new DisposableStore();
	private readonly _rootDomNode: HTMLElement = document.createElement('div');
	private readonly _lineNumbersDomNode: HTMLElement = document.createElement('div');
	private readonly _linesDomNodeScrollable: HTMLElement = document.createElement('div');
	private readonly _linesDomNode: HTMLElement = document.createElement('div');

	private _previousState: StickyScrollWidgetState | undefined;
	private _lineHeight: number = this._editor.getOption(EditorOption.lineHeight);
	private _stickyLines: RenderedStickyLine[] = [];
	private _lineNumbers: number[] = [];
	private _lastLineRelativePosition: number = 0;
	private _minContentWidthInPx: number = 0;
	private _isOnGlyphMargin: boolean = false;

	constructor(
		private readonly _editor: ICodeEditor
	) {
		super();

		this._lineNumbersDomNode.className = 'sticky-widget-line-numbers';
		this._lineNumbersDomNode.setAttribute('role', 'none');

		this._linesDomNode.className = 'sticky-widget-lines';
		this._linesDomNode.setAttribute('role', 'list');

		this._linesDomNodeScrollable.className = 'sticky-widget-lines-scrollable';
		this._linesDomNodeScrollable.appendChild(this._linesDomNode);

		this._rootDomNode.className = 'sticky-widget';
		this._rootDomNode.classList.toggle('peek', _editor instanceof EmbeddedCodeEditorWidget);
		this._rootDomNode.appendChild(this._lineNumbersDomNode);
		this._rootDomNode.appendChild(this._linesDomNodeScrollable);

		const updateScrollLeftPosition = () => {
			this._linesDomNode.style.left = this._editor.getOption(EditorOption.stickyScroll).scrollWithEditor ? `-${this._editor.getScrollLeft()}px` : '0px';
		};
		this._register(this._editor.onDidChangeConfiguration((e) => {
			if (e.hasChanged(EditorOption.stickyScroll)) {
				updateScrollLeftPosition();
			}
			if (e.hasChanged(EditorOption.lineHeight)) {
				this._lineHeight = this._editor.getOption(EditorOption.lineHeight);
			}
		}));
		this._register(this._editor.onDidScrollChange((e) => {
			if (e.scrollLeftChanged) {
				updateScrollLeftPosition();
			}
			if (e.scrollWidthChanged) {
				this._updateWidgetWidth();
			}
		}));
		this._register(this._editor.onDidChangeModel(() => {
			updateScrollLeftPosition();
			this._updateWidgetWidth();
		}));
		this._register(this._foldingIconStore);
		updateScrollLeftPosition();

		this._register(this._editor.onDidLayoutChange((e) => {
			this._updateWidgetWidth();
		}));
		this._updateWidgetWidth();
	}

	get lineNumbers(): number[] {
		return this._lineNumbers;
	}

	get lineNumberCount(): number {
		return this._lineNumbers.length;
	}

	getStickyLineForLine(lineNumber: number): RenderedStickyLine | undefined {
		return this._stickyLines.find(stickyLine => stickyLine.lineNumber === lineNumber);
	}

	getCurrentLines(): readonly number[] {
		return this._lineNumbers;
	}

	setState(state: StickyScrollWidgetState | undefined, foldingModel: FoldingModel | null, rebuildFromLine: number = Infinity): void {
		if (((!this._previousState && !state) || (this._previousState && this._previousState.equals(state)))
			&& rebuildFromLine === Infinity) {
			return;
		}
		this._previousState = state;
		const previousStickyLines = this._stickyLines;
		this._clearStickyWidget();
		if (!state || !this._editor._getViewModel()) {
			return;
		}
		const futureWidgetHeight = state.startLineNumbers.length * this._lineHeight + state.lastLineRelativePosition;

		if (futureWidgetHeight > 0) {
			this._lastLineRelativePosition = state.lastLineRelativePosition;
			const lineNumbers = [...state.startLineNumbers];
			if (state.showEndForLine !== null) {
				lineNumbers[state.showEndForLine] = state.endLineNumbers[state.showEndForLine];
			}
			this._lineNumbers = lineNumbers;
		} else {
			this._lastLineRelativePosition = 0;
			this._lineNumbers = [];
		}
		this._renderRootNode(previousStickyLines, foldingModel, rebuildFromLine);
	}

	private _updateWidgetWidth(): void {
		const layoutInfo = this._editor.getLayoutInfo();
		const lineNumbersWidth = layoutInfo.contentLeft;
		this._lineNumbersDomNode.style.width = `${lineNumbersWidth}px`;
		this._linesDomNodeScrollable.style.setProperty('--vscode-editorStickyScroll-scrollableWidth', `${this._editor.getScrollWidth() - layoutInfo.verticalScrollbarWidth}px`);
		this._rootDomNode.style.width = `${layoutInfo.width - layoutInfo.verticalScrollbarWidth}px`;
	}

	private _clearStickyWidget() {
		this._stickyLines = [];
		this._foldingIconStore.clear();
		dom.clearNode(this._lineNumbersDomNode);
		dom.clearNode(this._linesDomNode);
		this._rootDomNode.style.display = 'none';
	}

	private _useFoldingOpacityTransition(requireTransitions: boolean) {
		this._lineNumbersDomNode.style.setProperty('--vscode-editorStickyScroll-foldingOpacityTransition', `opacity ${requireTransitions ? 0.5 : 0}s`);
	}

	private _setFoldingIconsVisibility(allVisible: boolean) {
		for (const line of this._stickyLines) {
			const foldingIcon = line.foldingIcon;
			if (!foldingIcon) {
				continue;
			}
			foldingIcon.setVisible(allVisible ? true : foldingIcon.isCollapsed);
		}
	}

	private async _renderRootNode(previousStickyLines: RenderedStickyLine[], foldingModel: FoldingModel | null, rebuildFromLine: number = Infinity): Promise<void> {

		const layoutInfo = this._editor.getLayoutInfo();
		for (const [index, line] of this._lineNumbers.entries()) {
			const previousStickyLine = previousStickyLines[index];
			const stickyLine = (line >= rebuildFromLine || previousStickyLine?.lineNumber !== line)
				? this._renderChildNode(index, line, foldingModel, layoutInfo)
				: this._updateTopAndZIndexOfStickyLine(previousStickyLine);
			if (!stickyLine) {
				continue;
			}
			this._linesDomNode.appendChild(stickyLine.lineDomNode);
			this._lineNumbersDomNode.appendChild(stickyLine.lineNumberDomNode);
			this._stickyLines.push(stickyLine);
		}
		if (foldingModel) {
			this._setFoldingHoverListeners();
			this._useFoldingOpacityTransition(!this._isOnGlyphMargin);
		}

		const widgetHeight: number = this._lineNumbers.length * this._lineHeight + this._lastLineRelativePosition;
		if (widgetHeight === 0) {
			this._clearStickyWidget();
			return;
		}
		this._rootDomNode.style.display = 'block';
		this._lineNumbersDomNode.style.height = `${widgetHeight}px`;
		this._linesDomNodeScrollable.style.height = `${widgetHeight}px`;
		this._rootDomNode.style.height = `${widgetHeight}px`;

		this._rootDomNode.style.marginLeft = '0px';
		this._updateMinContentWidth();
		this._editor.layoutOverlayWidget(this);
	}

	private _setFoldingHoverListeners(): void {
		const showFoldingControls: 'mouseover' | 'always' | 'never' = this._editor.getOption(EditorOption.showFoldingControls);
		if (showFoldingControls !== 'mouseover') {
			return;
		}
		this._foldingIconStore.add(dom.addDisposableListener(this._lineNumbersDomNode, dom.EventType.MOUSE_ENTER, (e) => {
			this._isOnGlyphMargin = true;
			this._setFoldingIconsVisibility(true);
		}));
		this._foldingIconStore.add(dom.addDisposableListener(this._lineNumbersDomNode, dom.EventType.MOUSE_LEAVE, () => {
			this._isOnGlyphMargin = false;
			this._useFoldingOpacityTransition(true);
			this._setFoldingIconsVisibility(false);

		}));
	}

	private _renderChildNode(index: number, line: number, foldingModel: FoldingModel | null, layoutInfo: EditorLayoutInfo): RenderedStickyLine | undefined {
		const viewModel = this._editor._getViewModel();
		if (!viewModel) {
			return;
		}
		const viewLineNumber = viewModel.coordinatesConverter.convertModelPositionToViewPosition(new Position(line, 1)).lineNumber;
		const lineRenderingData = viewModel!.getViewLineRenderingData(viewLineNumber);
		const lineNumberOption = this._editor.getOption(EditorOption.lineNumbers);

		let actualInlineDecorations: LineDecoration[];
		try {
			actualInlineDecorations = LineDecoration.filter(lineRenderingData.inlineDecorations, viewLineNumber, lineRenderingData.minColumn, lineRenderingData.maxColumn);
		} catch (err) {
			actualInlineDecorations = [];
		}

		const renderLineInput: RenderLineInput = new RenderLineInput(true, true, lineRenderingData.content,
			lineRenderingData.continuesWithWrappedLine,
			lineRenderingData.isBasicASCII, lineRenderingData.containsRTL, 0,
			lineRenderingData.tokens, actualInlineDecorations,
			lineRenderingData.tabSize, lineRenderingData.startVisibleColumn,
			1, 1, 1, 500, 'none', true, true, null
		);

		const sb = new StringBuilder(2000);
		const renderOutput = renderViewLine(renderLineInput, sb);

		let newLine;
		if (_ttPolicy) {
			newLine = _ttPolicy.createHTML(sb.build() as string);
		} else {
			newLine = sb.build();
		}

		const lineHTMLNode = document.createElement('span');
		lineHTMLNode.setAttribute(STICKY_INDEX_ATTR, String(index));
		lineHTMLNode.setAttribute(STICKY_IS_LINE_ATTR, '');
		lineHTMLNode.setAttribute('role', 'listitem');
		lineHTMLNode.tabIndex = 0;
		lineHTMLNode.className = 'sticky-line-content';
		lineHTMLNode.classList.add(`stickyLine${line}`);
		lineHTMLNode.style.lineHeight = `${this._lineHeight}px`;
		lineHTMLNode.innerHTML = newLine as string;

		const lineNumberHTMLNode = document.createElement('span');
		lineNumberHTMLNode.setAttribute(STICKY_INDEX_ATTR, String(index));
		lineNumberHTMLNode.setAttribute(STICKY_IS_LINE_NUMBER_ATTR, '');
		lineNumberHTMLNode.className = 'sticky-line-number';
		lineNumberHTMLNode.style.lineHeight = `${this._lineHeight}px`;
		const lineNumbersWidth = layoutInfo.contentLeft;
		lineNumberHTMLNode.style.width = `${lineNumbersWidth}px`;

		const innerLineNumberHTML = document.createElement('span');
		if (lineNumberOption.renderType === RenderLineNumbersType.On || lineNumberOption.renderType === RenderLineNumbersType.Interval && line % 10 === 0) {
			innerLineNumberHTML.innerText = line.toString();
		} else if (lineNumberOption.renderType === RenderLineNumbersType.Relative) {
			innerLineNumberHTML.innerText = Math.abs(line - this._editor.getPosition()!.lineNumber).toString();
		}
		innerLineNumberHTML.className = 'sticky-line-number-inner';
		innerLineNumberHTML.style.lineHeight = `${this._lineHeight}px`;
		innerLineNumberHTML.style.width = `${layoutInfo.lineNumbersWidth}px`;
		innerLineNumberHTML.style.paddingLeft = `${layoutInfo.lineNumbersLeft}px`;

		lineNumberHTMLNode.appendChild(innerLineNumberHTML);
		const foldingIcon = this._renderFoldingIconForLine(foldingModel, line);
		if (foldingIcon) {
			lineNumberHTMLNode.appendChild(foldingIcon.domNode);
		}

		this._editor.applyFontInfo(lineHTMLNode);
		this._editor.applyFontInfo(innerLineNumberHTML);


		lineNumberHTMLNode.style.lineHeight = `${this._lineHeight}px`;
		lineHTMLNode.style.lineHeight = `${this._lineHeight}px`;
		lineNumberHTMLNode.style.height = `${this._lineHeight}px`;
		lineHTMLNode.style.height = `${this._lineHeight}px`;

		const renderedLine = new RenderedStickyLine(index, line, lineHTMLNode, lineNumberHTMLNode, foldingIcon, renderOutput.characterMapping);
		return this._updateTopAndZIndexOfStickyLine(renderedLine);
	}

	private _updateTopAndZIndexOfStickyLine(stickyLine: RenderedStickyLine): RenderedStickyLine {
		const index = stickyLine.index;
		const lineHTMLNode = stickyLine.lineDomNode;
		const lineNumberHTMLNode = stickyLine.lineNumberDomNode;
		const isLastLine = index === this._lineNumbers.length - 1;

		const lastLineZIndex = '0';
		const intermediateLineZIndex = '1';
		lineHTMLNode.style.zIndex = isLastLine ? lastLineZIndex : intermediateLineZIndex;
		lineNumberHTMLNode.style.zIndex = isLastLine ? lastLineZIndex : intermediateLineZIndex;

		const lastLineTop = `${index * this._lineHeight + this._lastLineRelativePosition + (stickyLine.foldingIcon?.isCollapsed ? 1 : 0)}px`;
		const intermediateLineTop = `${index * this._lineHeight}px`;
		lineHTMLNode.style.top = isLastLine ? lastLineTop : intermediateLineTop;
		lineNumberHTMLNode.style.top = isLastLine ? lastLineTop : intermediateLineTop;
		return stickyLine;
	}

	private _renderFoldingIconForLine(foldingModel: FoldingModel | null, line: number): StickyFoldingIcon | undefined {
		const showFoldingControls: 'mouseover' | 'always' | 'never' = this._editor.getOption(EditorOption.showFoldingControls);
		if (!foldingModel || showFoldingControls === 'never') {
			return;
		}
		const foldingRegions = foldingModel.regions;
		const indexOfFoldingRegion = foldingRegions.findRange(line);
		const startLineNumber = foldingRegions.getStartLineNumber(indexOfFoldingRegion);
		const isFoldingScope = line === startLineNumber;
		if (!isFoldingScope) {
			return;
		}
		const isCollapsed = foldingRegions.isCollapsed(indexOfFoldingRegion);
		const foldingIcon = new StickyFoldingIcon(isCollapsed, startLineNumber, foldingRegions.getEndLineNumber(indexOfFoldingRegion), this._lineHeight);
		foldingIcon.setVisible(this._isOnGlyphMargin ? true : (isCollapsed || showFoldingControls === 'always'));
		foldingIcon.domNode.setAttribute(STICKY_IS_FOLDING_ICON_ATTR, '');
		return foldingIcon;
	}

	private _updateMinContentWidth() {
		this._minContentWidthInPx = 0;
		for (const stickyLine of this._stickyLines) {
			if (stickyLine.lineDomNode.scrollWidth > this._minContentWidthInPx) {
				this._minContentWidthInPx = stickyLine.lineDomNode.scrollWidth;
			}
		}
		this._minContentWidthInPx += this._editor.getLayoutInfo().verticalScrollbarWidth;
	}

	getId(): string {
		return 'editor.contrib.stickyScrollWidget';
	}

	getDomNode(): HTMLElement {
		return this._rootDomNode;
	}

	getPosition(): IOverlayWidgetPosition | null {
		return {
			preference: null
		};
	}

	getMinContentWidthInPx(): number {
		return this._minContentWidthInPx;
	}

	focusLineWithIndex(index: number) {
		if (0 <= index && index < this._stickyLines.length) {
			this._stickyLines[index].lineDomNode.focus();
		}
	}

	/**
	 * Given a leaf dom node, tries to find the editor position.
	 */
	getEditorPositionFromNode(spanDomNode: HTMLElement | null): Position | null {
		if (!spanDomNode || spanDomNode.children.length > 0) {
			// This is not a leaf node
			return null;
		}
		const renderedStickyLine = this._getRenderedStickyLineFromChildDomNode(spanDomNode);
		if (!renderedStickyLine) {
			return null;
		}
		const column = getColumnOfNodeOffset(renderedStickyLine.characterMapping, spanDomNode, 0);
		return new Position(renderedStickyLine.lineNumber, column);
	}

	getLineNumberFromChildDomNode(domNode: HTMLElement | null): number | null {
		return this._getRenderedStickyLineFromChildDomNode(domNode)?.lineNumber ?? null;
	}

	private _getRenderedStickyLineFromChildDomNode(domNode: HTMLElement | null): RenderedStickyLine | null {
		const index = this.getLineIndexFromChildDomNode(domNode);
		if (index === null || index < 0 || index >= this._stickyLines.length) {
			return null;
		}
		return this._stickyLines[index];
	}

	/**
	 * Given a child dom node, tries to find the line number attribute that was stored in the node.
	 * @returns the attribute value or null if none is found.
	 */
	getLineIndexFromChildDomNode(domNode: HTMLElement | null): number | null {
		const lineIndex = this._getAttributeValue(domNode, STICKY_INDEX_ATTR);
		return lineIndex ? parseInt(lineIndex, 10) : null;
	}

	/**
	 * Given a child dom node, tries to find if it is (contained in) a sticky line.
	 * @returns a boolean.
	 */
	isInStickyLine(domNode: HTMLElement | null): boolean {
		const isInLine = this._getAttributeValue(domNode, STICKY_IS_LINE_ATTR);
		return isInLine !== undefined;
	}

	/**
	 * Given a child dom node, tries to find if this dom node is (contained in) a sticky folding icon.
	 * @returns a boolean.
	 */
	isInFoldingIconDomNode(domNode: HTMLElement | null): boolean {
		const isInFoldingIcon = this._getAttributeValue(domNode, STICKY_IS_FOLDING_ICON_ATTR);
		return isInFoldingIcon !== undefined;
	}

	/**
	 * Given the dom node, finds if it or its parent sequence contains the given attribute.
	 * @returns the attribute value or undefined.
	 */
	private _getAttributeValue(domNode: HTMLElement | null, attribute: string): string | undefined {
		while (domNode && domNode !== this._rootDomNode) {
			const line = domNode.getAttribute(attribute);
			if (line !== null) {
				return line;
			}
			domNode = domNode.parentElement;
		}
		return;
	}
}

class RenderedStickyLine {
	constructor(
		public readonly index: number,
		public readonly lineNumber: number,
		public readonly lineDomNode: HTMLElement,
		public readonly lineNumberDomNode: HTMLElement,
		public readonly foldingIcon: StickyFoldingIcon | undefined,
		public readonly characterMapping: CharacterMapping
	) { }
}

class StickyFoldingIcon {

	public domNode: HTMLElement;

	constructor(
		public isCollapsed: boolean,
		public foldingStartLine: number,
		public foldingEndLine: number,
		public dimension: number
	) {
		this.domNode = document.createElement('div');
		this.domNode.style.width = `${dimension}px`;
		this.domNode.style.height = `${dimension}px`;
		this.domNode.className = ThemeIcon.asClassName(isCollapsed ? foldingCollapsedIcon : foldingExpandedIcon);
	}

	public setVisible(visible: boolean) {
		this.domNode.style.cursor = visible ? 'pointer' : 'default';
		this.domNode.style.opacity = visible ? '1' : '0';
	}
}
