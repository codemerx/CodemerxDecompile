/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import 'vs/css!./goToDefinitionAtPosition';
import { onUnexpectedError } from 'vs/base/common/errors';
import { MarkdownString } from 'vs/base/common/htmlContent';
import { Range } from 'vs/editor/common/core/range';
import { IEditorContribution } from 'vs/editor/common/editorCommon';
import { ICodeEditor, MouseTargetType } from 'vs/editor/browser/editorBrowser';
import { registerEditorContribution } from 'vs/editor/browser/editorExtensions';
import { DisposableStore } from 'vs/base/common/lifecycle';
import { registerThemingParticipant } from 'vs/platform/theme/common/themeService';
import { editorActiveLinkForeground } from 'vs/platform/theme/common/colorRegistry';
import { EditorState, CodeEditorStateFlag } from 'vs/editor/browser/core/editorState';
import { ClickLinkGesture, ClickLinkMouseEvent, ClickLinkKeyboardEvent } from 'vs/editor/contrib/gotoSymbol/link/clickLinkGesture';
import { IWordAtPosition, IModelDeltaDecoration } from 'vs/editor/common/model';
import { Position } from 'vs/editor/common/core/position';
import { withNullAsUndefined } from 'vs/base/common/types';
import { IKeyboardEvent } from 'vs/base/browser/keyboardEvent';
/* AGPL */
import { IDecompilationService, ReferenceMetadata } from 'vs/cd/workbench/DecompilationService';
import { ICodeEditorService } from 'vs/editor/browser/services/codeEditorService';
import { URI } from 'vs/base/common/uri';
import { INotificationService, Severity, IPromptChoice } from 'vs/platform/notification/common/notification';
import { IProgressService, ProgressLocation } from 'vs/platform/progress/common/progress';
import { IDecompilationHelper } from 'vs/cd/workbench/DecompilationHelper';
/* End AGPL */

export class GotoDefinitionAtPositionEditorContribution implements IEditorContribution {

	public static readonly ID = 'editor.contrib.gotodefinitionatposition';
	static readonly MAX_SOURCE_PREVIEW_LINES = 8;

	private readonly editor: ICodeEditor;
	private readonly toUnhook = new DisposableStore();
	private readonly toUnhookForKeyboard = new DisposableStore();
	private linkDecorations: string[] = [];
	private currentWordAtPosition: IWordAtPosition | null = null;
	/* AGPL */
	private previousPromise: Promise<ReferenceMetadata | null> | null = null;
	private lastMemberReferenceResult: { position: Position, metadata: ReferenceMetadata } | null = null;
	/* End AGPL */

	constructor(
		editor: ICodeEditor,
		/* AGPL */
		@IDecompilationService private readonly decompilationService: IDecompilationService,
		@ICodeEditorService private readonly codeEditorService: ICodeEditorService,
		@INotificationService private readonly notificationService: INotificationService,
		@IDecompilationHelper private readonly decompilationHelper: IDecompilationHelper,
		@IProgressService private readonly progressService: IProgressService
		/* End AGPL */
	) {
		this.editor = editor;

		let linkGesture = new ClickLinkGesture(editor);
		this.toUnhook.add(linkGesture);

		this.toUnhook.add(linkGesture.onMouseMoveOrRelevantKeyDown(([mouseEvent, keyboardEvent]) => {
			this.startFindDefinitionFromMouse(mouseEvent, withNullAsUndefined(keyboardEvent));
		}));

		this.toUnhook.add(linkGesture.onExecute((mouseEvent: ClickLinkMouseEvent) => {
			if (this.isEnabled(mouseEvent)) {
				/* AGPL */
				try {
					const referenceMetadata = this.lastMemberReferenceResult?.metadata;
					if (referenceMetadata?.isCrossAssemblyReference && referenceMetadata?.referencedAssemblyFullName && referenceMetadata?.referencedAssemblyFilePath) {
						const choices: IPromptChoice[] = [
							{
								label: 'Yes',
								run: async () => {
									if (referenceMetadata?.isCrossAssemblyReference && referenceMetadata?.referencedAssemblyFilePath) {
										const assemblyFilePath = referenceMetadata?.referencedAssemblyFilePath;

										await this.progressService.withProgress({ location: ProgressLocation.Explorer }, async () => {
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

	static get(editor: ICodeEditor): GotoDefinitionAtPositionEditorContribution {
		return editor.getContribution<GotoDefinitionAtPositionEditorContribution>(GotoDefinitionAtPositionEditorContribution.ID);
	}

	startFindDefinitionFromCursor(position: Position) {
		// For issue: https://github.com/microsoft/vscode/issues/46257
		// equivalent to mouse move with meta/ctrl key

		// First find the definition and add decorations
		// to the editor to be shown with the content hover widget
		return this.startFindDefinition(position).then(() => {

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
		});
	}

	/* AGPL */
	private async goToDefinition(memberReferenceMetadata: ReferenceMetadata) : Promise<void> {
		if (memberReferenceMetadata?.definitionFilePath) {
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

	private startFindDefinition(position: Position): Promise<number | undefined> {

		// Dispose listeners for updating decorations when using keyboard to show definition hover
		this.toUnhookForKeyboard.clear();

		// Find word at mouse position
		const word = position ? this.editor.getModel()?.getWordAtPosition(position) : null;
		if (!word) {
			this.currentWordAtPosition = null;
			this.removeLinkDecorations();
			return Promise.resolve(0);
		}

		// Return early if word at position is still the same
		if (this.currentWordAtPosition && this.currentWordAtPosition.startColumn === word.startColumn && this.currentWordAtPosition.endColumn === word.endColumn && this.currentWordAtPosition.word === word.word) {
			return Promise.resolve(0);
		}

		this.currentWordAtPosition = word;

		// Find definition and decorate word if found
		let state = new EditorState(this.editor, CodeEditorStateFlag.Position | CodeEditorStateFlag.Value | CodeEditorStateFlag.Selection | CodeEditorStateFlag.Scroll);

		/* AGPL */
		const openedEditorFilePath = this.editor.getModel()?.uri.fsPath;

		if (openedEditorFilePath) {
			this.previousPromise = this.decompilationService.getMemberReferenceMetadata(openedEditorFilePath, position.lineNumber, position.column);
		} else {
			this.previousPromise = Promise.resolve(null);
		}
		/* End AGPL */

		return this.previousPromise.then(result => {
			if (!result || !state.validate(this.editor)) {
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
			if (err && err.metadata && err.metadata.unresolvedassemblyname) {
				const tooltipContent: MarkdownString = new MarkdownString().appendMarkdown(`Ambiguous type reference. Please locate the assembly where the type is defined.\nAssemblyName: **${err.metadata.unresolvedassemblyname}**`);
				this.addDecoration(new Range(position.lineNumber, word.startColumn, position.lineNumber, word.endColumn), tooltipContent, true);
			} else {
				throw err;
			}
			/* End AGPL */
		})
		.then(undefined, onUnexpectedError);
	}

	/* AGPL */
	private addDecoration(range: Range, hoverMessage?: MarkdownString, isErrorDecoration?: boolean): void {
	/* End AGPL */

		const newDecorations: IModelDeltaDecoration = {
			range: range,
			options: {
				/* AGPL */
				inlineClassName: isErrorDecoration ? 'goto-error-definition-link' : 'goto-definition-link',
				isLocateAssemblyHover: isErrorDecoration,
				/* End AGPL */
				hoverMessage
			}
		};

		this.linkDecorations = this.editor.deltaDecorations(this.linkDecorations, [newDecorations]);
	}

	private removeLinkDecorations(): void {
		if (this.linkDecorations.length > 0) {
			this.linkDecorations = this.editor.deltaDecorations(this.linkDecorations, []);
		}
	}

	private isEnabled(mouseEvent: ClickLinkMouseEvent, withKey?: ClickLinkKeyboardEvent): boolean {
		return this.editor.hasModel() &&
			mouseEvent.isNoneOrSingleMouseDown &&
			(mouseEvent.target.type === MouseTargetType.CONTENT_TEXT) &&
			(mouseEvent.hasTriggerModifier || (withKey ? withKey.keyCodeIsTriggerKey : false));
	}

	public dispose(): void {
		this.toUnhook.dispose();
	}
}

registerEditorContribution(GotoDefinitionAtPositionEditorContribution.ID, GotoDefinitionAtPositionEditorContribution);

registerThemingParticipant((theme, collector) => {
	const activeLinkForeground = theme.getColor(editorActiveLinkForeground);
	if (activeLinkForeground) {
		collector.addRule(`.monaco-editor .goto-definition-link { color: ${activeLinkForeground} !important; }`);
	}
});
