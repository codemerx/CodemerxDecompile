/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { ICodeEditor } from 'vs/editor/browser/editorBrowser';
import { EditorAction, registerEditorAction, ServicesAccessor } from 'vs/editor/browser/editorExtensions';
import { EditorZoom } from 'vs/editor/common/config/editorZoom';
import * as nls from 'vs/nls';

class EditorFontZoomIn extends EditorAction {

	constructor() {
		super({
			id: 'editor.action.fontZoomIn',
			/* AGPL */
			label: nls.localize('EditorFontZoomIn.label', "Code Viewer Font Zoom In"),
			alias: 'Code Viewer Font Zoom In',
			/* End AGPL */
			precondition: undefined
		});
	}

	public run(accessor: ServicesAccessor, editor: ICodeEditor): void {
		EditorZoom.setZoomLevel(EditorZoom.getZoomLevel() + 1);
	}
}

class EditorFontZoomOut extends EditorAction {

	constructor() {
		super({
			id: 'editor.action.fontZoomOut',
			/* AGPL */
			label: nls.localize('EditorFontZoomOut.label', "Code Viewer Font Zoom Out"),
			alias: 'Code Viewer Font Zoom Out',
			/* End AGPL */
			precondition: undefined
		});
	}

	public run(accessor: ServicesAccessor, editor: ICodeEditor): void {
		EditorZoom.setZoomLevel(EditorZoom.getZoomLevel() - 1);
	}
}

class EditorFontZoomReset extends EditorAction {

	constructor() {
		super({
			id: 'editor.action.fontZoomReset',
			/* AGPL */
			label: nls.localize('EditorFontZoomReset.label', "Code Viewer Font Zoom Reset"),
			alias: 'Code Viewer Font Zoom Reset',
			/* End AGPL */
			precondition: undefined
		});
	}

	public run(accessor: ServicesAccessor, editor: ICodeEditor): void {
		EditorZoom.setZoomLevel(0);
	}
}

registerEditorAction(EditorFontZoomIn);
registerEditorAction(EditorFontZoomOut);
registerEditorAction(EditorFontZoomReset);
