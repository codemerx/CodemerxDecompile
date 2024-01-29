/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { TokenizationRegistry } from 'vs/editor/common/languages';
import { generateTokensCSSForColorMap } from 'vs/editor/common/languages/supports/tokenization';
import { ILanguageService } from 'vs/editor/common/languages/language';
import * as nls from 'vs/nls';
import { IWebviewWorkbenchService } from 'vs/workbench/contrib/webviewPanel/browser/webviewWorkbenchService';
import { IEditorService, ACTIVE_GROUP } from 'vs/workbench/services/editor/common/editorService';
import { WebviewInput } from 'vs/workbench/contrib/webviewPanel/browser/webviewEditorInput';
import { IExtensionService } from 'vs/workbench/services/extensions/common/extensions';
import { IEditorGroupsService } from 'vs/workbench/services/editor/common/editorGroupsService';
import { generateUuid } from 'vs/base/common/uuid';
import { renderMarkdownDocument } from 'vs/workbench/contrib/markdown/browser/markdownDocumentRenderer';

export class MarkdownPreviewEditor {

	private _currentWebview: WebviewInput | undefined = undefined;

	public constructor(
		@ILanguageService private readonly _languageService: ILanguageService,
		@IEditorService private readonly _editorService: IEditorService,
		@IEditorGroupsService private readonly _editorGroupService: IEditorGroupsService,
		@IWebviewWorkbenchService private readonly _webviewWorkbenchService: IWebviewWorkbenchService,
		@IExtensionService private readonly _extensionService: IExtensionService,
	) { }

	public async show(
		markdownResourceContents: string,
		webviewTitle: string
	): Promise<boolean> {
		const html = await this.renderBody(markdownResourceContents);
		const title = nls.localize('webviewTitle', "{0}", webviewTitle);

		const activeEditorPane = this._editorService.activeEditorPane;
		if (this._currentWebview) {
			this._currentWebview.setName(title);
			this._currentWebview.webview.setHtml(html);
			this._webviewWorkbenchService.revealWebview(this._currentWebview, activeEditorPane ? activeEditorPane.group : this._editorGroupService.activeGroup, false);
		} else {
			this._currentWebview = this._webviewWorkbenchService.openWebview(
				{
					title,
					options: {
						tryRestoreScrollPosition: true,
						enableFindWidget: true
					},
					contentOptions: {
						localResourceRoots: []
					},
					extension: undefined
				},
				'markdownPreview',
				title,
				{ group: ACTIVE_GROUP, preserveFocus: false });

			this._currentWebview.onWillDispose(() => { this._currentWebview = undefined; });

			this._currentWebview.webview.setHtml(html);
		}

		return true;
	}

	private async renderBody(text: string) {
		const nonce = generateUuid();
		const content = await renderMarkdownDocument(text, this._extensionService, this._languageService);
		const colorMap = TokenizationRegistry.getColorMap();
		const css = colorMap ? generateTokensCSSForColorMap(colorMap) : '';
		return `<!DOCTYPE html>
		<html>
			<head>
				<meta http-equiv="Content-type" content="text/html;charset=UTF-8">
				<meta http-equiv="Content-Security-Policy" content="default-src 'none'; img-src https: data:; media-src https:; style-src 'nonce-${nonce}'">
				<style nonce="${nonce}">
					body {
						padding: 10px 20px;
						line-height: 22px;
						max-width: 882px;
						margin: 0 auto;
					}

					img {
						max-width: 100%;
						max-height: 100%;
					}

					a {
						text-decoration: none;
					}

					a:hover {
						text-decoration: underline;
					}

					a:focus,
					input:focus,
					select:focus,
					textarea:focus {
						outline: 1px solid -webkit-focus-ring-color;
						outline-offset: -1px;
					}

					hr {
						border: 0;
						height: 2px;
						border-bottom: 2px solid;
					}

					h1 {
						padding-bottom: 0.3em;
						line-height: 1.2;
						border-bottom-width: 1px;
						border-bottom-style: solid;
					}

					h1, h2, h3 {
						font-weight: normal;
					}

					table {
						border-collapse: collapse;
					}

					table > thead > tr > th {
						text-align: left;
						border-bottom: 1px solid;
					}

					table > thead > tr > th,
					table > thead > tr > td,
					table > tbody > tr > th,
					table > tbody > tr > td {
						padding: 5px 10px;
					}

					table > tbody > tr + tr > td {
						border-top-width: 1px;
						border-top-style: solid;
					}

					blockquote {
						margin: 0 7px 0 5px;
						padding: 0 16px 0 10px;
						border-left-width: 5px;
						border-left-style: solid;
					}

					code {
						font-family: var(--vscode-editor-font-family);
						font-weight: var(--vscode-editor-font-weight);
						font-size: var(--vscode-editor-font-size);
						line-height: 19px;
					}

					code > div {
						padding: 16px;
						border-radius: 3px;
						overflow: auto;
					}

					.monaco-tokenized-source {
						white-space: pre;
					}

					/** Theming */

					.vscode-light code > div {
						background-color: rgba(220, 220, 220, 0.4);
					}

					.vscode-dark code > div {
						background-color: rgba(10, 10, 10, 0.4);
					}

					.vscode-high-contrast code > div {
						background-color: rgb(0, 0, 0);
					}

					.vscode-high-contrast h1 {
						border-color: rgb(0, 0, 0);
					}

					.vscode-light table > thead > tr > th {
						border-color: rgba(0, 0, 0, 0.69);
					}

					.vscode-dark table > thead > tr > th {
						border-color: rgba(255, 255, 255, 0.69);
					}

					.vscode-light h1,
					.vscode-light hr,
					.vscode-light table > tbody > tr + tr > td {
						border-color: rgba(0, 0, 0, 0.18);
					}

					.vscode-dark h1,
					.vscode-dark hr,
					.vscode-dark table > tbody > tr + tr > td {
						border-color: rgba(255, 255, 255, 0.18);
					}

					${css}
				</style>
			</head>
			<body>${content}</body>
		</html>`;
	}
}
