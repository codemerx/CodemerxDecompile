/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as dom from 'vs/base/browser/dom';
import { ProgressBar } from 'vs/base/browser/ui/progressbar/progressbar';
import { dispose, DisposableStore, MutableDisposable } from 'vs/base/common/lifecycle';
import 'vs/css!./assemblyMetadataPane';
import { ICodeEditor, isCodeEditor, isDiffEditor } from 'vs/editor/browser/editorBrowser';
import { IModelContentChangedEvent } from 'vs/editor/common/model/textModelEvents';
import { DocumentSymbolProviderRegistry } from 'vs/editor/common/modes';
import { LanguageFeatureRegistry } from 'vs/editor/common/modes/languageFeatureRegistry';
import { IConfigurationService } from 'vs/platform/configuration/common/configuration';
import { IContextKey, IContextKeyService } from 'vs/platform/contextkey/common/contextkey';
import { IContextMenuService } from 'vs/platform/contextview/browser/contextView';
import { IInstantiationService } from 'vs/platform/instantiation/common/instantiation';
import { IKeybindingService } from 'vs/platform/keybinding/common/keybinding';
import { attachProgressBarStyler } from 'vs/platform/theme/common/styler';
import { IThemeService } from 'vs/platform/theme/common/themeService';
import { ViewPane } from 'vs/workbench/browser/parts/views/viewPaneContainer';
import { IViewletViewOptions } from 'vs/workbench/browser/parts/views/viewsViewlet';
import { IEditorService } from 'vs/workbench/services/editor/common/editorService';
import { OutlineViewFocused } from 'vs/editor/contrib/documentSymbols/outline';
import { IViewDescriptorService } from 'vs/workbench/common/views';
import { IOpenerService } from 'vs/platform/opener/common/opener';
import { ITelemetryService } from 'vs/platform/telemetry/common/telemetry';
import { IDecompilationService } from 'vs/cd/workbench/DecompilationService';
import { localize } from 'vs/nls';
import { IExplorerService } from '../../files/common/files';

class RequestState {

	constructor(
		private _editorId: string,
		private _modelId: string,
		private _modelVersion: number,
		private _providerCount: number
	) {
		//
	}

	equals(other: RequestState): boolean {
		return other
			&& this._editorId === other._editorId
			&& this._modelId === other._modelId
			&& this._modelVersion === other._modelVersion
			&& this._providerCount === other._providerCount;
	}
}

class RequestOracle {

	private readonly _disposables = new DisposableStore();
	private _sessionDisposable = new MutableDisposable();
	private _lastState?: RequestState;

	constructor(
		private readonly _callback: (editor: ICodeEditor | undefined, change: IModelContentChangedEvent | undefined, resourcePath: string | undefined) => any,
		private readonly _featureRegistry: LanguageFeatureRegistry<any>,
		@IEditorService private readonly _editorService: IEditorService,
		@IDecompilationService _decompilationService: IDecompilationService,
		@IExplorerService _explorerService: IExplorerService
	) {
		_explorerService.onDidExplorerResourceChange(resourcePath => {
			this._callback(undefined, undefined, resourcePath);
		}, this, this._disposables);
		_decompilationService.onDecompilationContextRestored(this._update, this, this._disposables);
		_featureRegistry.onDidChange(this._update, this, this._disposables);
		this._update();
	}

	dispose(): void {
		this._disposables.dispose();
		this._sessionDisposable.dispose();
	}

	private _update(): void {

		let control = this._editorService.activeTextEditorControl;
		let codeEditor: ICodeEditor | undefined = undefined;
		if (isCodeEditor(control)) {
			codeEditor = control;
		} else if (isDiffEditor(control)) {
			codeEditor = control.getModifiedEditor();
		}

		if (!codeEditor || !codeEditor.hasModel()) {
			this._lastState = undefined;
			this._callback(undefined, undefined, undefined);
			return;
		}

		let thisState = new RequestState(
			codeEditor.getId(),
			codeEditor.getModel().id,
			codeEditor.getModel().getVersionId(),
			this._featureRegistry.all(codeEditor.getModel()).length
		);

		if (this._lastState && thisState.equals(this._lastState)) {
			// prevent unnecessary changes...
			return;
		}
		this._lastState = thisState;
		this._callback(codeEditor, undefined, undefined);

		let disposeListener = codeEditor.onDidDispose(() => {
			this._callback(undefined, undefined, undefined);
		});
		this._sessionDisposable.value = {
			dispose() {
				disposeListener.dispose();
			}
		};
	}
}

export class AssemblyMetadataPane extends ViewPane {

	private _disposables = new DisposableStore();

	private _editorDisposables = new DisposableStore();
	private _requestOracle?: RequestOracle;
	private _domNode!: HTMLElement;
	private _message!: HTMLDivElement;
	private _inputContainer!: HTMLDivElement;
	private _progressBar!: ProgressBar;
	private readonly _contextKeyFocused: IContextKey<boolean>;

	constructor(
		options: IViewletViewOptions,
		@IInstantiationService private readonly _instantiationService: IInstantiationService,
		@IViewDescriptorService viewDescriptorService: IViewDescriptorService,
		@IThemeService private readonly _themeService: IThemeService,
		@IConfigurationService _configurationService: IConfigurationService,
		@IKeybindingService keybindingService: IKeybindingService,
		@IContextKeyService contextKeyService: IContextKeyService,
		@IContextMenuService contextMenuService: IContextMenuService,
		@IOpenerService openerService: IOpenerService,
		@IThemeService themeService: IThemeService,
		@ITelemetryService telemetryService: ITelemetryService,
		@IDecompilationService private readonly decompilationService: IDecompilationService
	) {
		super(options, keybindingService, contextMenuService, _configurationService, contextKeyService, viewDescriptorService, _instantiationService, openerService, themeService, telemetryService);
		this._contextKeyFocused = OutlineViewFocused.bindTo(contextKeyService);
		this._disposables.add(this.onDidFocus(_ => this._contextKeyFocused.set(true)));
		this._disposables.add(this.onDidBlur(_ => this._contextKeyFocused.set(false)));
	}

	dispose(): void {
		dispose(this._disposables);
		dispose(this._requestOracle);
		dispose(this._editorDisposables);
		super.dispose();
	}

	protected renderBody(container: HTMLElement): void {
		super.renderBody(container);

		this._domNode = container;
		this._domNode.tabIndex = 0;
		container.classList.add('assemblyMetadata-pane');

		let progressContainer = dom.$('.assemblyMetadata-progress');
		this._message = dom.$('.assemblyMetadata-message');
		this._inputContainer = dom.$('.assemblyMetadata-input');

		this._progressBar = new ProgressBar(progressContainer);
		this._register(attachProgressBarStyler(this._progressBar, this._themeService));

		dom.append(
			container,
			progressContainer, this._message, this._inputContainer
		);

		this._register(this.onDidChangeBodyVisibility(visible => {
			if (visible && !this._requestOracle) {
				this._requestOracle = this._instantiationService.createInstance(RequestOracle, (editor, event, resourcePath) => this._doUpdate(editor, event, resourcePath), DocumentSymbolProviderRegistry);
			} else if (!visible) {
				dispose(this._requestOracle);
				this._requestOracle = undefined;
				this._doUpdate(undefined, undefined, undefined);
			}
		}));
	}

	private _showMessage(message: string) {
		this._domNode.classList.add('message');
		this._progressBar.stop().hide();
		this._message.innerText = message;
	}

	private _showAssemblyInfo(info: Map<string, string>) {
		this._domNode.classList.add('message');
		this._message.innerHTML = '';

		for(const [key, value] of info) {
			let container = dom.$('.metadata-container');
			let keyElement = dom.$('.metadata-key');
			keyElement.innerText = key;
			let valueElement = dom.$('.metadata-value');
			valueElement.innerText = value;
			dom.append(
				container,
				keyElement, valueElement
			);
			dom.append(
				this._message,
				container
			)
		}
	}

	private async _doUpdate(editor: ICodeEditor | undefined, event: IModelContentChangedEvent | undefined, resourcePath: string | undefined): Promise<void> {
		this._editorDisposables.clear();
		
		if ((!resourcePath || !resourcePath.length) && (!editor || !editor.hasModel() || !editor.getModel()?.uri)) {
			return this._showMessage(localize('no-editor', "No assembly metadata information."));
		}

		this._progressBar.infinite().show();

		const uri = editor?.getModel()?.uri?.fsPath ?? resourcePath ?? '';
		const assemblyMetadata = await this.decompilationService.getContextAssembly(uri);

		if (!assemblyMetadata) {
			return this._showMessage(localize('no-editor', "No assembly metadata information."));
		}

		const metadata = new Map();
		metadata.set('Assembly Name', assemblyMetadata.assemblyFullName);
		metadata.set('Platform Target', assemblyMetadata.targetPlatform);
		metadata.set('Platform Architecture', assemblyMetadata.targetArchitecture);
		metadata.set('Assembly Manifest', assemblyMetadata.assemblyFilePath);
		
		this._progressBar.stop().hide();
		this._showAssemblyInfo(metadata);
	}
}
