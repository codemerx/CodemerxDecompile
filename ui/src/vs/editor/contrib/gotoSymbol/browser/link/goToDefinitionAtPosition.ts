/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { IKeyboardEvent } from 'vs/base/browser/keyboardEvent';
import { CancelablePromise, createCancelablePromise } from 'vs/base/common/async';
import { CancellationToken } from 'vs/base/common/cancellation';
import { onUnexpectedError } from 'vs/base/common/errors';
import { MarkdownString } from 'vs/base/common/htmlContent';
import { DisposableStore } from 'vs/base/common/lifecycle';
import 'vs/css!./goToDefinitionAtPosition';
import { CodeEditorStateFlag, EditorState } from 'vs/editor/contrib/editorState/browser/editorState';
import { ICodeEditor, MouseTargetType } from 'vs/editor/browser/editorBrowser';
import { EditorContributionInstantiation, registerEditorContribution } from 'vs/editor/browser/editorExtensions';
import { EditorOption } from 'vs/editor/common/config/editorOptions';
import { Position } from 'vs/editor/common/core/position';
import { IRange, Range } from 'vs/editor/common/core/range';
import { IEditorContribution, IEditorDecorationsCollection } from 'vs/editor/common/editorCommon';
import { IModelDeltaDecoration, ITextModel } from 'vs/editor/common/model';
import { LocationLink } from 'vs/editor/common/languages';
import { ILanguageService } from 'vs/editor/common/languages/language';
import { ITextModelService } from 'vs/editor/common/services/resolverService';
import { ClickLinkGesture, ClickLinkKeyboardEvent, ClickLinkMouseEvent } from 'vs/editor/contrib/gotoSymbol/browser/link/clickLinkGesture';
import { PeekContext } from 'vs/editor/contrib/peekView/browser/peekView';
import * as nls from 'vs/nls';
import { IContextKeyService } from 'vs/platform/contextkey/common/contextkey';
import { ServicesAccessor } from 'vs/platform/instantiation/common/instantiation';
import { DefinitionAction } from '../goToCommands';
import { getDefinitionsAtPosition } from '../goToSymbol';
import { IWordAtPosition } from 'vs/editor/common/core/wordHelper';
import { ILanguageFeaturesService } from 'vs/editor/common/services/languageFeatures';
import { ModelDecorationInjectedTextOptions } from 'vs/editor/common/model/textModel';
/* AGPL */
import { IDecompilationService } from 'vs/cd/workbench/DecompilationService';
import { ReferenceMetadata } from 'vs/cd/common/DecompilationTypes';
import { ICodeEditorService } from 'vs/editor/browser/services/codeEditorService';
import { URI } from 'vs/base/common/uri';
import { INotificationService, Severity, IPromptChoice } from 'vs/platform/notification/common/notification';
import { IProgressService, ProgressLocation } from 'vs/platform/progress/common/progress';
import { IDecompilationHelper } from 'vs/cd/workbench/DecompilationHelper';
import { IAnalyticsService } from 'vs/cd/workbench/AnalyticsService';
/* End AGPL */

export class GotoDefinitionAtPositionEditorContribution implements IEditorContribution {

	public static readonly ID = 'editor.contrib.gotodefinitionatposition';
	static readonly MAX_SOURCE_PREVIEW_LINES = 8;

	private readonly editor: ICodeEditor;
	private readonly toUnhook = new DisposableStore();
	private readonly toUnhookForKeyboard = new DisposableStore();
	private readonly linkDecorations: IEditorDecorationsCollection;
	private currentWordAtPosition: IWordAtPosition | null = null;
	/* AGPL */
	private previousPromise: Promise<ReferenceMetadata | null> | null = null;
	private lastMemberReferenceResult: { position: Position, metadata: ReferenceMetadata } | null = null;
	/* End AGPL */

	constructor(
		editor: ICodeEditor,
		@ITextModelService private readonly textModelResolverService: ITextModelService,
		@ILanguageService private readonly languageService: ILanguageService,
		@ILanguageFeaturesService private readonly languageFeaturesService: ILanguageFeaturesService,
		/* AGPL */
		@IDecompilationService private readonly decompilationService: IDecompilationService,
		@ICodeEditorService private readonly codeEditorService: ICodeEditorService,
		@INotificationService private readonly notificationService: INotificationService,
		@IDecompilationHelper private readonly decompilationHelper: IDecompilationHelper,
		@IProgressService private readonly progressService: IProgressService,
		@IAnalyticsService private readonly analyticsService: IAnalyticsService
		/* End AGPL */
	) {
		this.editor = editor;
		this.linkDecorations = this.editor.createDecorationsCollection();

		const linkGesture = new ClickLinkGesture(editor);
		this.toUnhook.add(linkGesture);

		this.toUnhook.add(linkGesture.onMouseMoveOrRelevantKeyDown(([mouseEvent, keyboardEvent]) => {
			this.startFindDefinitionFromMouse(mouseEvent, keyboardEvent ?? undefined);
		}));

		this.toUnhook.add(linkGesture.onExecute((mouseEvent: ClickLinkMouseEvent) => {
			if (this.isEnabled(mouseEvent)) {
				/* AGPL */
				// this.gotoDefinition(mouseEvent.target.position!, mouseEvent.hasSideBySideModifier)
				// 	.catch((error: Error) => {
				// 		onUnexpectedError(error);
				// 	})
				// 	.finally(() => {
				// 		this.removeLinkDecorations();
				// 	});
				try {
					const referenceMetadata = this.lastMemberReferenceResult?.metadata;
					if (referenceMetadata?.isCrossAssemblyReference && referenceMetadata?.referencedAssemblyFullName && referenceMetadata?.referencedAssemblyFilePath) {
						const choices: IPromptChoice[] = [
							{
								label: 'Yes',
								run: async () => {
									if (referenceMetadata?.isCrossAssemblyReference && referenceMetadata?.referencedAssemblyFilePath) {
										const assemblyFilePath = referenceMetadata?.referencedAssemblyFilePath;

										await this.progressService.withProgress({ location: ProgressLocation.Dialog, sticky: true }, async progress => {
											progress.report({ message: 'Loading assembly...' });
											await this.decompilationHelper.createAssemblyFileHierarchy(URI.file(assemblyFilePath));
										});

										const openedEditorFilePath = this.editor.getModel()?.uri.fsPath;
										if (openedEditorFilePath && this.lastMemberReferenceResult?.position) {
											const { lineNumber, column } = this.lastMemberReferenceResult?.position;
											this.lastMemberReferenceResult.metadata = await this.decompilationService.getMemberReferenceMetadata(openedEditorFilePath, lineNumber, column);

											if (this.lastMemberReferenceResult.metadata?.definitionFilePath) {
												await this.goToDefinition(this.lastMemberReferenceResult.metadata);
											}
										}

										this.removeLinkDecorations();
									}
								}
							}, {
								label: 'No',
								run: () => {
									this.removeLinkDecorations();
								}
							}
						];

						this.notificationService.prompt(Severity.Info, `Would you like to load ${referenceMetadata.referencedAssemblyFullName}?`, choices, {
							onCancel: () => this.removeLinkDecorations()
						});
					} else if (referenceMetadata?.definitionFilePath) {
						this.goToDefinition(referenceMetadata);
					} else {
						this.removeLinkDecorations();
					}
				} catch(err) {
					this.removeLinkDecorations();
					onUnexpectedError(err);
				}
				/* End AGPL */
			}
		}));

		this.toUnhook.add(linkGesture.onCancel(() => {
			this.removeLinkDecorations();
			this.currentWordAtPosition = null;
		}));
	}

	static get(editor: ICodeEditor): GotoDefinitionAtPositionEditorContribution | null {
		return editor.getContribution<GotoDefinitionAtPositionEditorContribution>(GotoDefinitionAtPositionEditorContribution.ID);
	}

	async startFindDefinitionFromCursor(position: Position) {
		// For issue: https://github.com/microsoft/vscode/issues/46257
		// equivalent to mouse move with meta/ctrl key

		// First find the definition and add decorations
		// to the editor to be shown with the content hover widget
		await this.startFindDefinition(position);
		// Add listeners for editor cursor move and key down events
		// Dismiss the "extended" editor decorations when the user hides
		// the hover widget. There is no event for the widget itself so these
		// serve as a best effort. After removing the link decorations, the hover
		// widget is clean and will only show declarations per next request.
		this.toUnhookForKeyboard.add(this.editor.onDidChangeCursorPosition(() => {
			this.currentWordAtPosition = null;
			this.removeLinkDecorations();
			this.toUnhookForKeyboard.clear();
		}));
		this.toUnhookForKeyboard.add(this.editor.onKeyDown((e: IKeyboardEvent) => {
			if (e) {
				this.currentWordAtPosition = null;
				this.removeLinkDecorations();
				this.toUnhookForKeyboard.clear();
			}
		}));
	}

	/* AGPL */
	private async goToDefinition(memberReferenceMetadata: ReferenceMetadata) : Promise<void> {
		if (memberReferenceMetadata?.definitionFilePath) {

			await this.analyticsService.trackEvent('Editor', 'NavigateToDefinition');

			await this.codeEditorService.openCodeEditor({
				resource: URI.file(memberReferenceMetadata.definitionFilePath)
			}, null, undefined, memberReferenceMetadata);
		}

		this.lastMemberReferenceResult = null;
	}
	/* End AGPL */

	private startFindDefinitionFromMouse(mouseEvent: ClickLinkMouseEvent, withKey?: ClickLinkKeyboardEvent): void {

		// check if we are active and on a content widget
		if (mouseEvent.target.type === MouseTargetType.CONTENT_WIDGET && this.linkDecorations.length > 0) {
			return;
		}

		if (!this.editor.hasModel() || !this.isEnabled(mouseEvent, withKey)) {
			this.currentWordAtPosition = null;
			this.removeLinkDecorations();
			return;
		}

		const position = mouseEvent.target.position!;

		this.startFindDefinition(position);
	}

	private async startFindDefinition(position: Position): Promise<void> {

		// Dispose listeners for updating decorations when using keyboard to show definition hover
		this.toUnhookForKeyboard.clear();

		// Find word at mouse position
		const word = position ? this.editor.getModel()?.getWordAtPosition(position) : null;
		if (!word) {
			this.currentWordAtPosition = null;
			this.removeLinkDecorations();
			return;
		}

		// Return early if word at position is still the same
		if (this.currentWordAtPosition && this.currentWordAtPosition.startColumn === word.startColumn && this.currentWordAtPosition.endColumn === word.endColumn && this.currentWordAtPosition.word === word.word) {
			return;
		}

		this.currentWordAtPosition = word;

		// Find definition and decorate word if found
		const state = new EditorState(this.editor, CodeEditorStateFlag.Position | CodeEditorStateFlag.Value | CodeEditorStateFlag.Selection | CodeEditorStateFlag.Scroll);

		/* AGPL */
		const openedEditorFilePath = this.editor.getModel()?.uri.fsPath;

		if (openedEditorFilePath) {
			this.previousPromise = this.decompilationService.getMemberReferenceMetadata(openedEditorFilePath, position.lineNumber, position.column);
		} else {
			this.previousPromise = Promise.resolve(null);
		}
		/* End AGPL */

		/* AGPL */
		// this.previousPromise = createCancelablePromise(token => this.findDefinition(position, token));

		// let results: LocationLink[] | null;
		// try {
		// 	results = await this.previousPromise;

		// } catch (error) {
		// 	onUnexpectedError(error);
		// 	return;
		// }

		// if (!results || !results.length || !state.validate(this.editor)) {
		// 	this.removeLinkDecorations();
		// 	return;
		// }

		// const linkRange = results[0].originSelectionRange
		// 	? Range.lift(results[0].originSelectionRange)
		// 	: new Range(position.lineNumber, word.startColumn, position.lineNumber, word.endColumn);

		// // Multiple results
		// if (results.length > 1) {

		// 	let combinedRange = linkRange;
		// 	for (const { originSelectionRange } of results) {
		// 		if (originSelectionRange) {
		// 			combinedRange = Range.plusRange(combinedRange, originSelectionRange);
		// 		}
		// 	}

		// 	this.addDecoration(
		// 		combinedRange,
		// 		new MarkdownString().appendText(nls.localize('multipleResults', "Click to show {0} definitions.", results.length))
		// 	);
		// } else {
		// 	// Single result
		// 	const result = results[0];

		// 	if (!result.uri) {
		// 		return;
		// 	}

		// 	this.textModelResolverService.createModelReference(result.uri).then(ref => {

		// 		if (!ref.object || !ref.object.textEditorModel) {
		// 			ref.dispose();
		// 			return;
		// 		}

		// 		const { object: { textEditorModel } } = ref;
		// 		const { startLineNumber } = result.range;

		// 		if (startLineNumber < 1 || startLineNumber > textEditorModel.getLineCount()) {
		// 			// invalid range
		// 			ref.dispose();
		// 			return;
		// 		}

		// 		const previewValue = this.getPreviewValue(textEditorModel, startLineNumber, result);
		// 		const languageId = this.languageService.guessLanguageIdByFilepathOrFirstLine(textEditorModel.uri);
		// 		this.addDecoration(
		// 			linkRange,
		// 			previewValue ? new MarkdownString().appendCodeblock(languageId ? languageId : '', previewValue) : undefined
		// 		);
		// 		ref.dispose();
		// 	});
		// }
		/* End AGPL */

		return this.previousPromise.then(result => {
			if (!result || !state.validate(this.editor)) {
				/* AGPL */
				this.lastMemberReferenceResult = null;
				/* End AGPL */
				this.removeLinkDecorations();
				return;
			}

			/* AGPL */
			this.lastMemberReferenceResult = {
				position,
				metadata: result
			};

			this.addDecoration(new Range(position.lineNumber, word.startColumn, position.lineNumber, word.endColumn));
			/* End AGPL */
		})
		.catch(err => {
			/* AGPL */
			this.lastMemberReferenceResult = null;

			if (err && err.metadata && err.metadata.unresolvedAssemblyName) {
				const tooltipContent: MarkdownString = new MarkdownString().appendMarkdown(`Ambiguous type reference. Please locate the assembly where the type is defined.\nAssemblyName: **${err.metadata.unresolvedAssemblyName}**`);
				this.addDecoration(new Range(position.lineNumber, word.startColumn, position.lineNumber, word.endColumn), tooltipContent, true);
			} else {
				throw err;
			}
			/* End AGPL */
		})
		.then(undefined, onUnexpectedError);
	}

	/* AGPL */
	// private getPreviewValue(textEditorModel: ITextModel, startLineNumber: number, result: LocationLink) {
	// 	let rangeToUse = result.range;
	// 	const numberOfLinesInRange = rangeToUse.endLineNumber - rangeToUse.startLineNumber;
	// 	if (numberOfLinesInRange >= GotoDefinitionAtPositionEditorContribution.MAX_SOURCE_PREVIEW_LINES) {
	// 		rangeToUse = this.getPreviewRangeBasedOnIndentation(textEditorModel, startLineNumber);
	// 	}

	// 	const previewValue = this.stripIndentationFromPreviewRange(textEditorModel, startLineNumber, rangeToUse);
	// 	return previewValue;
	// }

	// private stripIndentationFromPreviewRange(textEditorModel: ITextModel, startLineNumber: number, previewRange: IRange) {
	// 	const startIndent = textEditorModel.getLineFirstNonWhitespaceColumn(startLineNumber);
	// 	let minIndent = startIndent;

	// 	for (let endLineNumber = startLineNumber + 1; endLineNumber < previewRange.endLineNumber; endLineNumber++) {
	// 		const endIndent = textEditorModel.getLineFirstNonWhitespaceColumn(endLineNumber);
	// 		minIndent = Math.min(minIndent, endIndent);
	// 	}

	// 	const previewValue = textEditorModel.getValueInRange(previewRange).replace(new RegExp(`^\\s{${minIndent - 1}}`, 'gm'), '').trim();
	// 	return previewValue;
	// }

	// private getPreviewRangeBasedOnIndentation(textEditorModel: ITextModel, startLineNumber: number) {
	// 	const startIndent = textEditorModel.getLineFirstNonWhitespaceColumn(startLineNumber);
	// 	const maxLineNumber = Math.min(textEditorModel.getLineCount(), startLineNumber + GotoDefinitionAtPositionEditorContribution.MAX_SOURCE_PREVIEW_LINES);
	// 	let endLineNumber = startLineNumber + 1;

	// 	for (; endLineNumber < maxLineNumber; endLineNumber++) {
	// 		const endIndent = textEditorModel.getLineFirstNonWhitespaceColumn(endLineNumber);

	// 		if (startIndent === endIndent) {
	// 			break;
	// 		}
	// 	}

	// 	return new Range(startLineNumber, 1, endLineNumber + 1, 1);
	// }
	/* End AGPL */

	/* AGPL */
	// private addDecoration(range: Range, hoverMessage: MarkdownString | undefined): void {
	private addDecoration(range: Range, hoverMessage?: MarkdownString, isErrorDecoration?: boolean): void {
	/* End AGPL */

		const newDecorations: IModelDeltaDecoration = {
			range: range,
			options: {
				/* AGPL */
				description: isErrorDecoration ? 'goto-error-definition-link' : 'goto-definition-link',
				inlineClassName: isErrorDecoration ? 'goto-error-definition-link' : 'goto-definition-link',
				isLocateAssemblyHover: isErrorDecoration,
				/* End AGPL */
				hoverMessage
			}
		};

		this.linkDecorations.set([newDecorations]);
	}

	private removeLinkDecorations(): void {
		this.linkDecorations.clear();
	}

	private isEnabled(mouseEvent: ClickLinkMouseEvent, withKey?: ClickLinkKeyboardEvent): boolean {
		return this.editor.hasModel()
			&& mouseEvent.isLeftClick
			&& mouseEvent.isNoneOrSingleMouseDown
			&& mouseEvent.target.type === MouseTargetType.CONTENT_TEXT
			&& !(mouseEvent.target.detail.injectedText?.options instanceof ModelDecorationInjectedTextOptions)
			&& (mouseEvent.hasTriggerModifier || (withKey ? withKey.keyCodeIsTriggerKey : false));
			/* AGPL */
			// && this.languageFeaturesService.definitionProvider.has(this.editor.getModel());
			/* End AGPL */
	}

	/* AGPL */
	// private findDefinition(position: Position, token: CancellationToken): Promise<LocationLink[] | null> {
	// 	const model = this.editor.getModel();
	// 	if (!model) {
	// 		return Promise.resolve(null);
	// 	}

	// 	return getDefinitionsAtPosition(this.languageFeaturesService.definitionProvider, model, position, token);
	// }

	// private gotoDefinition(position: Position, openToSide: boolean): Promise<any> {
	// 	this.editor.setPosition(position);
	// 	return this.editor.invokeWithinContext((accessor) => {
	// 		const canPeek = !openToSide && this.editor.getOption(EditorOption.definitionLinkOpensInPeek) && !this.isInPeekEditor(accessor);
	// 		const action = new DefinitionAction({ openToSide, openInPeek: canPeek, muteMessage: true }, { title: { value: '', original: '' }, id: '', precondition: undefined });
	// 		return action.run(accessor);
	// 	});
	// }

	// private isInPeekEditor(accessor: ServicesAccessor): boolean | undefined {
	// 	const contextKeyService = accessor.get(IContextKeyService);
	// 	return PeekContext.inPeekEditor.getValue(contextKeyService);
	// }
	/* End AGPL */

	public dispose(): void {
		this.toUnhook.dispose();
		this.toUnhookForKeyboard.dispose();
	}
}

registerEditorContribution(GotoDefinitionAtPositionEditorContribution.ID, GotoDefinitionAtPositionEditorContribution, EditorContributionInstantiation.BeforeFirstInteraction);
