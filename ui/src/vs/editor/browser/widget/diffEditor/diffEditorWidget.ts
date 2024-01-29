/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
import { $, getWindow, h } from 'vs/base/browser/dom';
import { IBoundarySashes } from 'vs/base/browser/ui/sash/sash';
import { findLast } from 'vs/base/common/arraysFind';
import { onUnexpectedError } from 'vs/base/common/errors';
import { Event } from 'vs/base/common/event';
import { toDisposable } from 'vs/base/common/lifecycle';
import { IObservable, ITransaction, autorun, autorunWithStore, derived, observableFromEvent, observableValue, recomputeInitiallyAndOnChange, subtransaction, transaction } from 'vs/base/common/observable';
import { derivedDisposable } from 'vs/base/common/observableInternal/derived';
import 'vs/css!./style';
import { IEditorConstructionOptions } from 'vs/editor/browser/config/editorConfiguration';
import { ICodeEditor, IDiffEditor, IDiffEditorConstructionOptions } from 'vs/editor/browser/editorBrowser';
import { EditorExtensionsRegistry, IDiffEditorContributionDescription } from 'vs/editor/browser/editorExtensions';
import { ICodeEditorService } from 'vs/editor/browser/services/codeEditorService';
import { StableEditorScrollState } from 'vs/editor/browser/stableEditorScroll';
import { CodeEditorWidget, ICodeEditorWidgetOptions } from 'vs/editor/browser/widget/codeEditorWidget';
import { AccessibleDiffViewer } from 'vs/editor/browser/widget/diffEditor/accessibleDiffViewer';
import { DiffEditorDecorations } from 'vs/editor/browser/widget/diffEditor/diffEditorDecorations';
import { DiffEditorSash } from 'vs/editor/browser/widget/diffEditor/diffEditorSash';
import { HideUnchangedRegionsFeature } from 'vs/editor/browser/widget/diffEditor/hideUnchangedRegionsFeature';
import { ViewZoneManager } from 'vs/editor/browser/widget/diffEditor/lineAlignment';
import { MovedBlocksLinesPart } from 'vs/editor/browser/widget/diffEditor/movedBlocksLines';
import { OverviewRulerPart } from 'vs/editor/browser/widget/diffEditor/overviewRulerPart';
import { CSSStyle, ObservableElementSizeObserver, applyStyle, applyViewZones, bindContextKey, readHotReloadableExport, translatePosition } from 'vs/editor/browser/widget/diffEditor/utils';
import { IDiffEditorOptions } from 'vs/editor/common/config/editorOptions';
import { IDimension } from 'vs/editor/common/core/dimension';
import { Position } from 'vs/editor/common/core/position';
import { Range } from 'vs/editor/common/core/range';
import { CursorChangeReason } from 'vs/editor/common/cursorEvents';
import { IDiffComputationResult, ILineChange } from 'vs/editor/common/diff/legacyLinesDiffComputer';
import { DetailedLineRangeMapping, RangeMapping } from 'vs/editor/common/diff/rangeMapping';
import { EditorType, IDiffEditorModel, IDiffEditorViewModel, IDiffEditorViewState } from 'vs/editor/common/editorCommon';
import { EditorContextKeys } from 'vs/editor/common/editorContextKeys';
import { IIdentifiedSingleEditOperation } from 'vs/editor/common/model';
import { AudioCue, IAudioCueService } from 'vs/platform/audioCues/browser/audioCueService';
import { IContextKeyService } from 'vs/platform/contextkey/common/contextkey';
import { IInstantiationService } from 'vs/platform/instantiation/common/instantiation';
import { ServiceCollection } from 'vs/platform/instantiation/common/serviceCollection';
import { IEditorProgressService } from 'vs/platform/progress/common/progress';
import './colors';
import { DelegatingEditor } from './delegatingEditorImpl';
import { DiffEditorEditors } from './diffEditorEditors';
import { DiffEditorOptions } from './diffEditorOptions';
import { DiffEditorViewModel, DiffMapping, DiffState } from './diffEditorViewModel';

export interface IDiffCodeEditorWidgetOptions {
	originalEditor?: ICodeEditorWidgetOptions;
	modifiedEditor?: ICodeEditorWidgetOptions;
}

export class DiffEditorWidget extends DelegatingEditor implements IDiffEditor {
	public static ENTIRE_DIFF_OVERVIEW_WIDTH = OverviewRulerPart.ENTIRE_DIFF_OVERVIEW_WIDTH;

	private readonly elements = h('div.monaco-diff-editor.side-by-side', { style: { position: 'relative', height: '100%' } }, [
		h('div.noModificationsOverlay@overlay', { style: { position: 'absolute', height: '100%', visibility: 'hidden', } }, [$('span', {}, 'No Changes')]),
		h('div.editor.original@original', { style: { position: 'absolute', height: '100%' } }),
		h('div.editor.modified@modified', { style: { position: 'absolute', height: '100%' } }),
		h('div.accessibleDiffViewer@accessibleDiffViewer', { style: { position: 'absolute', height: '100%' } }),
	]);
	private readonly _diffModel = observableValue<DiffEditorViewModel | undefined>(this, undefined);
	private _shouldDisposeDiffModel = false;
	public readonly onDidChangeModel = Event.fromObservableLight(this._diffModel);

	public get onDidContentSizeChange() { return this._editors.onDidContentSizeChange; }

	private readonly _contextKeyService = this._register(this._parentContextKeyService.createScoped(this._domElement));
	private readonly _instantiationService = this._parentInstantiationService.createChild(
		new ServiceCollection([IContextKeyService, this._contextKeyService])
	);
	private readonly _rootSizeObserver: ObservableElementSizeObserver;

	private readonly _sash: IObservable<DiffEditorSash | undefined>;
	private readonly _boundarySashes = observableValue<IBoundarySashes | undefined>(this, undefined);

	private _accessibleDiffViewerShouldBeVisible = observableValue(this, false);
	private _accessibleDiffViewerVisible = derived(this, reader =>
		this._options.onlyShowAccessibleDiffViewer.read(reader)
			? true
			: this._accessibleDiffViewerShouldBeVisible.read(reader)
	);
	private readonly _accessibleDiffViewer: IObservable<AccessibleDiffViewer>;
	private readonly _options: DiffEditorOptions;
	private readonly _editors: DiffEditorEditors;

	private readonly _overviewRulerPart: IObservable<OverviewRulerPart | undefined>;
	private readonly _movedBlocksLinesPart = observableValue<MovedBlocksLinesPart | undefined>(this, undefined);

	public get collapseUnchangedRegions() { return this._options.hideUnchangedRegions.get(); }

	constructor(
		private readonly _domElement: HTMLElement,
		options: Readonly<IDiffEditorConstructionOptions>,
		codeEditorWidgetOptions: IDiffCodeEditorWidgetOptions,
		@IContextKeyService private readonly _parentContextKeyService: IContextKeyService,
		@IInstantiationService private readonly _parentInstantiationService: IInstantiationService,
		@ICodeEditorService codeEditorService: ICodeEditorService,
		@IAudioCueService private readonly _audioCueService: IAudioCueService,
		@IEditorProgressService private readonly _editorProgressService: IEditorProgressService,
	) {
		super();
		codeEditorService.willCreateDiffEditor();

		this._contextKeyService.createKey('isInDiffEditor', true);

		this._domElement.appendChild(this.elements.root);
		this._register(toDisposable(() => this._domElement.removeChild(this.elements.root)));

		this._rootSizeObserver = this._register(new ObservableElementSizeObserver(this.elements.root, options.dimension));
		this._rootSizeObserver.setAutomaticLayout(options.automaticLayout ?? false);

		this._options = new DiffEditorOptions(options);
		this._register(autorun(reader => {
			this._options.setWidth(this._rootSizeObserver.width.read(reader));
		}));

		this._contextKeyService.createKey(EditorContextKeys.isEmbeddedDiffEditor.key, false);
		this._register(bindContextKey(EditorContextKeys.isEmbeddedDiffEditor, this._contextKeyService,
			reader => this._options.isInEmbeddedEditor.read(reader)
		));
		this._register(bindContextKey(EditorContextKeys.comparingMovedCode, this._contextKeyService,
			reader => !!this._diffModel.read(reader)?.movedTextToCompare.read(reader)
		));
		this._register(bindContextKey(EditorContextKeys.diffEditorRenderSideBySideInlineBreakpointReached, this._contextKeyService,
			reader => this._options.couldShowInlineViewBecauseOfSize.read(reader)
		));

		this._register(bindContextKey(EditorContextKeys.hasChanges, this._contextKeyService,
			reader => (this._diffModel.read(reader)?.diff.read(reader)?.mappings.length ?? 0) > 0
		));

		this._editors = this._register(this._instantiationService.createInstance(
			DiffEditorEditors,
			this.elements.original,
			this.elements.modified,
			this._options,
			codeEditorWidgetOptions,
			(i, c, o, o2) => this._createInnerEditor(i, c, o, o2),
		));

		this._overviewRulerPart = derivedDisposable(this, reader =>
			!this._options.renderOverviewRuler.read(reader)
				? undefined
				: this._instantiationService.createInstance(
					readHotReloadableExport(OverviewRulerPart, reader),
					this._editors,
					this.elements.root,
					this._diffModel,
					this._rootSizeObserver.width,
					this._rootSizeObserver.height,
					this._layoutInfo.map(i => i.modifiedEditor),
				)
		).recomputeInitiallyAndOnChange(this._store);

		this._sash = derivedDisposable(this, reader => {
			const showSash = this._options.renderSideBySide.read(reader);
			this.elements.root.classList.toggle('side-by-side', showSash);
			return !showSash ? undefined : new DiffEditorSash(
				this._options,
				this.elements.root,
				{
					height: this._rootSizeObserver.height,
					width: this._rootSizeObserver.width.map((w, reader) => w - (this._overviewRulerPart.read(reader)?.width ?? 0)),
				},
				this._boundarySashes,
			);
		}).recomputeInitiallyAndOnChange(this._store);

		const unchangedRangesFeature = derivedDisposable(this, reader => /** @description UnchangedRangesFeature */
			this._instantiationService.createInstance(
				readHotReloadableExport(HideUnchangedRegionsFeature, reader),
				this._editors, this._diffModel, this._options
			)
		).recomputeInitiallyAndOnChange(this._store);

		derivedDisposable(this, reader => /** @description DiffEditorDecorations */
			this._instantiationService.createInstance(
				readHotReloadableExport(DiffEditorDecorations, reader),
				this._editors, this._diffModel, this._options, this,
			)
		).recomputeInitiallyAndOnChange(this._store);

		const origViewZoneIdsToIgnore = new Set<string>();
		const modViewZoneIdsToIgnore = new Set<string>();
		let isUpdatingViewZones = false;
		const viewZoneManager = derivedDisposable(this, reader => /** @description ViewZoneManager */
			this._instantiationService.createInstance(
				readHotReloadableExport(ViewZoneManager, reader),
				getWindow(this._domElement),
				this._editors,
				this._diffModel,
				this._options,
				this,
				() => isUpdatingViewZones || unchangedRangesFeature.get().isUpdatingHiddenAreas,
				origViewZoneIdsToIgnore,
				modViewZoneIdsToIgnore
			)
		).recomputeInitiallyAndOnChange(this._store);

		const originalViewZones = derived(this, (reader) => { /** @description originalViewZones */
			const orig = viewZoneManager.read(reader).viewZones.read(reader).orig;
			const orig2 = unchangedRangesFeature.read(reader).viewZones.read(reader).origViewZones;
			return orig.concat(orig2);
		});
		const modifiedViewZones = derived(this, (reader) => { /** @description modifiedViewZones */
			const mod = viewZoneManager.read(reader).viewZones.read(reader).mod;
			const mod2 = unchangedRangesFeature.read(reader).viewZones.read(reader).modViewZones;
			return mod.concat(mod2);
		});
		this._register(applyViewZones(this._editors.original, originalViewZones, isUpdatingOrigViewZones => {
			isUpdatingViewZones = isUpdatingOrigViewZones;
		}, origViewZoneIdsToIgnore));
		let scrollState: StableEditorScrollState | undefined;
		this._register(applyViewZones(this._editors.modified, modifiedViewZones, isUpdatingModViewZones => {
			isUpdatingViewZones = isUpdatingModViewZones;
			if (isUpdatingViewZones) {
				scrollState = StableEditorScrollState.capture(this._editors.modified);
			} else {
				scrollState?.restore(this._editors.modified);
				scrollState = undefined;
			}
		}, modViewZoneIdsToIgnore));

		this._accessibleDiffViewer = derivedDisposable(this, reader =>
			this._instantiationService.createInstance(
				readHotReloadableExport(AccessibleDiffViewer, reader),
				this.elements.accessibleDiffViewer,
				this._accessibleDiffViewerVisible,
				(visible, tx) => this._accessibleDiffViewerShouldBeVisible.set(visible, tx),
				this._options.onlyShowAccessibleDiffViewer.map(v => !v),
				this._rootSizeObserver.width,
				this._rootSizeObserver.height,
				this._diffModel.map((m, r) => m?.diff.read(r)?.mappings.map(m => m.lineRangeMapping)),
				this._editors,
			)
		).recomputeInitiallyAndOnChange(this._store);

		const visibility = this._accessibleDiffViewerVisible.map<CSSStyle['visibility']>(v => v ? 'hidden' : 'visible');
		this._register(applyStyle(this.elements.modified, { visibility }));
		this._register(applyStyle(this.elements.original, { visibility }));

		this._createDiffEditorContributions();

		codeEditorService.addDiffEditor(this);

		this._register(recomputeInitiallyAndOnChange(this._layoutInfo));

		derivedDisposable(this, reader => /** @description MovedBlocksLinesPart */
			new (readHotReloadableExport(MovedBlocksLinesPart, reader))(
				this.elements.root,
				this._diffModel,
				this._layoutInfo.map(i => i.originalEditor),
				this._layoutInfo.map(i => i.modifiedEditor),
				this._editors,
			)
		).recomputeInitiallyAndOnChange(this._store, value => {
			// This is to break the layout info <-> moved blocks lines part dependency cycle.
			this._movedBlocksLinesPart.set(value, undefined);
		});

		this._register(applyStyle(this.elements.overlay, {
			width: this._layoutInfo.map((i, r) => i.originalEditor.width + (this._options.renderSideBySide.read(r) ? 0 : i.modifiedEditor.width)),
			visibility: derived(reader => /** @description visibility */(this._options.hideUnchangedRegions.read(reader) && this._diffModel.read(reader)?.diff.read(reader)?.mappings.length === 0)
				? 'visible' : 'hidden'
			),
		}));

		this._register(Event.runAndSubscribe(this._editors.modified.onDidChangeCursorPosition, (e) => {
			if (e?.reason === CursorChangeReason.Explicit) {
				const diff = this._diffModel.get()?.diff.get()?.mappings.find(m => m.lineRangeMapping.modified.contains(e.position.lineNumber));
				if (diff?.lineRangeMapping.modified.isEmpty) {
					this._audioCueService.playAudioCue(AudioCue.diffLineDeleted, { source: 'diffEditor.cursorPositionChanged' });
				} else if (diff?.lineRangeMapping.original.isEmpty) {
					this._audioCueService.playAudioCue(AudioCue.diffLineInserted, { source: 'diffEditor.cursorPositionChanged' });
				} else if (diff) {
					this._audioCueService.playAudioCue(AudioCue.diffLineModified, { source: 'diffEditor.cursorPositionChanged' });
				}
			}
		}));

		const isInitializingDiff = this._diffModel.map(this, (m, reader) => {
			/** @isInitializingDiff isDiffUpToDate */
			if (!m) { return undefined; }
			return m.diff.read(reader) === undefined && !m.isDiffUpToDate.read(reader);
		});
		this._register(autorunWithStore((reader, store) => {
			/** @description DiffEditorWidgetHelper.ShowProgress */
			if (isInitializingDiff.read(reader) === true) {
				const r = this._editorProgressService.show(true, 1000);
				store.add(toDisposable(() => r.done()));
			}
		}));

		this._register(toDisposable(() => {
			if (this._shouldDisposeDiffModel) {
				this._diffModel.get()?.dispose();
			}
		}));
	}

	public getViewWidth(): number {
		return this._rootSizeObserver.width.get();
	}

	public getContentHeight() {
		return this._editors.modified.getContentHeight();
	}

	protected _createInnerEditor(instantiationService: IInstantiationService, container: HTMLElement, options: Readonly<IEditorConstructionOptions>, editorWidgetOptions: ICodeEditorWidgetOptions): CodeEditorWidget {
		const editor = instantiationService.createInstance(CodeEditorWidget, container, options, editorWidgetOptions);
		return editor;
	}

	private readonly _layoutInfo = derived(this, reader => {
		const width = this._rootSizeObserver.width.read(reader);
		const height = this._rootSizeObserver.height.read(reader);
		const sashLeft = this._sash.read(reader)?.sashLeft.read(reader);

		const originalWidth = sashLeft ?? Math.max(5, this._editors.original.getLayoutInfo().decorationsLeft);
		const modifiedWidth = width - originalWidth - (this._overviewRulerPart.read(reader)?.width ?? 0);

		const movedBlocksLinesWidth = this._movedBlocksLinesPart.read(reader)?.width.read(reader) ?? 0;
		const originalWidthWithoutMovedBlockLines = originalWidth - movedBlocksLinesWidth;
		this.elements.original.style.width = originalWidthWithoutMovedBlockLines + 'px';
		this.elements.original.style.left = '0px';

		this.elements.modified.style.width = modifiedWidth + 'px';
		this.elements.modified.style.left = originalWidth + 'px';

		this._editors.original.layout({ width: originalWidthWithoutMovedBlockLines, height }, true);
		this._editors.modified.layout({ width: modifiedWidth, height }, true);

		return {
			modifiedEditor: this._editors.modified.getLayoutInfo(),
			originalEditor: this._editors.original.getLayoutInfo(),
		};
	});

	private _createDiffEditorContributions() {
		const contributions: IDiffEditorContributionDescription[] = EditorExtensionsRegistry.getDiffEditorContributions();
		for (const desc of contributions) {
			try {
				this._register(this._instantiationService.createInstance(desc.ctor, this));
			} catch (err) {
				onUnexpectedError(err);
			}
		}
	}

	protected override get _targetEditor(): CodeEditorWidget { return this._editors.modified; }

	override getEditorType(): string { return EditorType.IDiffEditor; }

	override onVisible(): void {
		// TODO: Only compute diffs when diff editor is visible
		this._editors.original.onVisible();
		this._editors.modified.onVisible();
	}

	override onHide(): void {
		this._editors.original.onHide();
		this._editors.modified.onHide();
	}

	override layout(dimension?: IDimension | undefined): void {
		this._rootSizeObserver.observe(dimension);
	}

	override hasTextFocus(): boolean { return this._editors.original.hasTextFocus() || this._editors.modified.hasTextFocus(); }

	public override saveViewState(): IDiffEditorViewState {
		const originalViewState = this._editors.original.saveViewState();
		const modifiedViewState = this._editors.modified.saveViewState();
		return {
			original: originalViewState,
			modified: modifiedViewState,
			modelState: this._diffModel.get()?.serializeState(),
		};
	}

	public override restoreViewState(s: IDiffEditorViewState): void {
		if (s && s.original && s.modified) {
			const diffEditorState = s as IDiffEditorViewState;
			this._editors.original.restoreViewState(diffEditorState.original);
			this._editors.modified.restoreViewState(diffEditorState.modified);
			if (diffEditorState.modelState) {
				this._diffModel.get()?.restoreSerializedState(diffEditorState.modelState as any);
			}
		}
	}

	public handleInitialized(): void {
		this._editors.original.handleInitialized();
		this._editors.modified.handleInitialized();
	}

	public createViewModel(model: IDiffEditorModel): IDiffEditorViewModel {
		return this._instantiationService.createInstance(DiffEditorViewModel, model, this._options);
	}

	override getModel(): IDiffEditorModel | null { return this._diffModel.get()?.model ?? null; }

	override setModel(model: IDiffEditorModel | null | IDiffEditorViewModel, tx?: ITransaction): void {
		if (!model && this._diffModel.get()) {
			// Transitioning from a model to no-model
			this._accessibleDiffViewer.get().close();
		}

		const vm = model ? ('model' in model) ? { model, shouldDispose: false } : { model: this.createViewModel(model), shouldDispose: true } : undefined;

		if (this._diffModel.get() !== vm?.model) {
			subtransaction(tx, tx => {
				/** @description DiffEditorWidget.setModel */
				observableFromEvent.batchEventsGlobally(tx, () => {
					this._editors.original.setModel(vm ? vm.model.model.original : null);
					this._editors.modified.setModel(vm ? vm.model.model.modified : null);
				});
				const prevValue = this._diffModel.get();
				const shouldDispose = this._shouldDisposeDiffModel;

				this._shouldDisposeDiffModel = vm?.shouldDispose ?? false;
				this._diffModel.set(vm?.model as (DiffEditorViewModel | undefined), tx);

				if (shouldDispose) {
					prevValue?.dispose();
				}
			});
		}
	}

	/**
	 * @param changedOptions Only has values for top-level options that have actually changed.
	 */
	override updateOptions(changedOptions: IDiffEditorOptions): void {
		this._options.updateOptions(changedOptions);
	}

	getContainerDomNode(): HTMLElement { return this._domElement; }
	getOriginalEditor(): ICodeEditor { return this._editors.original; }
	getModifiedEditor(): ICodeEditor { return this._editors.modified; }

	setBoundarySashes(sashes: IBoundarySashes): void {
		this._boundarySashes.set(sashes, undefined);
	}

	private readonly _diffValue = this._diffModel.map((m, r) => m?.diff.read(r));
	readonly onDidUpdateDiff: Event<void> = Event.fromObservableLight(this._diffValue);

	get ignoreTrimWhitespace(): boolean { return this._options.ignoreTrimWhitespace.get(); }

	get maxComputationTime(): number { return this._options.maxComputationTimeMs.get(); }

	get renderSideBySide(): boolean { return this._options.renderSideBySide.get(); }

	/**
	 * @deprecated Use `this.getDiffComputationResult().changes2` instead.
	 */
	getLineChanges(): ILineChange[] | null {
		const diffState = this._diffModel.get()?.diff.get();
		if (!diffState) { return null; }
		return toLineChanges(diffState);
	}

	getDiffComputationResult(): IDiffComputationResult | null {
		const diffState = this._diffModel.get()?.diff.get();
		if (!diffState) { return null; }

		return {
			changes: this.getLineChanges()!,
			changes2: diffState.mappings.map(m => m.lineRangeMapping),
			identical: diffState.identical,
			quitEarly: diffState.quitEarly,
		};
	}

	revert(diff: DetailedLineRangeMapping): void {
		if (diff.innerChanges) {
			this.revertRangeMappings(diff.innerChanges);
			return;
		}

		const model = this._diffModel.get()?.model;
		if (!model) { return; }

		this._editors.modified.executeEdits('diffEditor', [
			{
				range: diff.modified.toExclusiveRange(),
				text: model.original.getValueInRange(diff.original.toExclusiveRange())
			}
		]);
	}

	revertRangeMappings(diffs: RangeMapping[]): void {
		const model = this._diffModel.get();
		if (!model || !model.isDiffUpToDate.get()) { return; }

		const changes: IIdentifiedSingleEditOperation[] = diffs.map<IIdentifiedSingleEditOperation>(c => ({
			range: c.modifiedRange,
			text: model.model.original.getValueInRange(c.originalRange)
		}));

		this._editors.modified.executeEdits('diffEditor', changes);
	}

	private _goTo(diff: DiffMapping): void {
		this._editors.modified.setPosition(new Position(diff.lineRangeMapping.modified.startLineNumber, 1));
		this._editors.modified.revealRangeInCenter(diff.lineRangeMapping.modified.toExclusiveRange());
	}

	goToDiff(target: 'previous' | 'next'): void {
		const diffs = this._diffModel.get()?.diff.get()?.mappings;
		if (!diffs || diffs.length === 0) {
			return;
		}

		const curLineNumber = this._editors.modified.getPosition()!.lineNumber;

		let diff: DiffMapping | undefined;
		if (target === 'next') {
			diff = diffs.find(d => d.lineRangeMapping.modified.startLineNumber > curLineNumber) ?? diffs[0];
		} else {
			diff = findLast(diffs, d => d.lineRangeMapping.modified.startLineNumber < curLineNumber) ?? diffs[diffs.length - 1];
		}
		this._goTo(diff);

		if (diff.lineRangeMapping.modified.isEmpty) {
			this._audioCueService.playAudioCue(AudioCue.diffLineDeleted, { source: 'diffEditor.goToDiff' });
		} else if (diff.lineRangeMapping.original.isEmpty) {
			this._audioCueService.playAudioCue(AudioCue.diffLineInserted, { source: 'diffEditor.goToDiff' });
		} else if (diff) {
			this._audioCueService.playAudioCue(AudioCue.diffLineModified, { source: 'diffEditor.goToDiff' });
		}
	}

	revealFirstDiff(): void {
		const diffModel = this._diffModel.get();
		if (!diffModel) {
			return;
		}
		// wait for the diff computation to finish
		this.waitForDiff().then(() => {
			const diffs = diffModel.diff.get()?.mappings;
			if (!diffs || diffs.length === 0) {
				return;
			}
			this._goTo(diffs[0]);
		});
	}

	accessibleDiffViewerNext(): void { this._accessibleDiffViewer.get().next(); }

	accessibleDiffViewerPrev(): void { this._accessibleDiffViewer.get().prev(); }

	async waitForDiff(): Promise<void> {
		const diffModel = this._diffModel.get();
		if (!diffModel) { return; }
		await diffModel.waitForDiff();
	}

	mapToOtherSide(): { destination: CodeEditorWidget; destinationSelection: Range | undefined } {
		const isModifiedFocus = this._editors.modified.hasWidgetFocus();
		const source = isModifiedFocus ? this._editors.modified : this._editors.original;
		const destination = isModifiedFocus ? this._editors.original : this._editors.modified;

		let destinationSelection: Range | undefined;

		const sourceSelection = source.getSelection();
		if (sourceSelection) {
			const mappings = this._diffModel.get()?.diff.get()?.mappings.map(m => isModifiedFocus ? m.lineRangeMapping.flip() : m.lineRangeMapping);
			if (mappings) {
				const newRange1 = translatePosition(sourceSelection.getStartPosition(), mappings);
				const newRange2 = translatePosition(sourceSelection.getEndPosition(), mappings);
				destinationSelection = Range.plusRange(newRange1, newRange2);
			}
		}
		return { destination, destinationSelection };
	}

	switchSide(): void {
		const { destination, destinationSelection } = this.mapToOtherSide();
		destination.focus();
		if (destinationSelection) {
			destination.setSelection(destinationSelection);
		}
	}

	exitCompareMove(): void {
		const model = this._diffModel.get();
		if (!model) { return; }
		model.movedTextToCompare.set(undefined, undefined);
	}

	collapseAllUnchangedRegions(): void {
		const unchangedRegions = this._diffModel.get()?.unchangedRegions.get();
		if (!unchangedRegions) { return; }
		transaction(tx => {
			for (const region of unchangedRegions) {
				region.collapseAll(tx);
			}
		});
	}

	showAllUnchangedRegions(): void {
		const unchangedRegions = this._diffModel.get()?.unchangedRegions.get();
		if (!unchangedRegions) { return; }
		transaction(tx => {
			for (const region of unchangedRegions) {
				region.showAll(tx);
			}
		});
	}
}

function toLineChanges(state: DiffState): ILineChange[] {
	return state.mappings.map(x => {
		const m = x.lineRangeMapping;
		let originalStartLineNumber: number;
		let originalEndLineNumber: number;
		let modifiedStartLineNumber: number;
		let modifiedEndLineNumber: number;
		let innerChanges = m.innerChanges;

		if (m.original.isEmpty) {
			// Insertion
			originalStartLineNumber = m.original.startLineNumber - 1;
			originalEndLineNumber = 0;
			innerChanges = undefined;
		} else {
			originalStartLineNumber = m.original.startLineNumber;
			originalEndLineNumber = m.original.endLineNumberExclusive - 1;
		}

		if (m.modified.isEmpty) {
			// Deletion
			modifiedStartLineNumber = m.modified.startLineNumber - 1;
			modifiedEndLineNumber = 0;
			innerChanges = undefined;
		} else {
			modifiedStartLineNumber = m.modified.startLineNumber;
			modifiedEndLineNumber = m.modified.endLineNumberExclusive - 1;
		}

		return {
			originalStartLineNumber,
			originalEndLineNumber,
			modifiedStartLineNumber,
			modifiedEndLineNumber,
			charChanges: innerChanges?.map(m => ({
				originalStartLineNumber: m.originalRange.startLineNumber,
				originalStartColumn: m.originalRange.startColumn,
				originalEndLineNumber: m.originalRange.endLineNumber,
				originalEndColumn: m.originalRange.endColumn,
				modifiedStartLineNumber: m.modifiedRange.startLineNumber,
				modifiedStartColumn: m.modifiedRange.startColumn,
				modifiedEndLineNumber: m.modifiedRange.endLineNumber,
				modifiedEndColumn: m.modifiedRange.endColumn,
			}))
		};
	});
}
