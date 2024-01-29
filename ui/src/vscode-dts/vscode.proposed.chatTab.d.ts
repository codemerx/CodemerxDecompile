/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

declare module 'vscode' {
	/**
	 * The tab represents an interactive window.
	 */
	export class TabInputChat {
		/**
		 * The uri of the history notebook in the interactive window.
		 */
		readonly providerId: string;
		constructor(providerId: string);
	}

	export interface Tab {
		readonly input: TabInputText | TabInputTextDiff | TabInputCustom | TabInputWebview | TabInputNotebook | TabInputNotebookDiff | TabInputTerminal | TabInputChat | unknown;
	}
}
