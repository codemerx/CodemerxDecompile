/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { IAction, Action, Separator } from 'vs/base/common/actions';
import { localize } from 'vs/nls';
import { IWorkbenchLayoutService } from 'vs/workbench/services/layout/browser/layoutService';
import { IContextMenuService } from 'vs/platform/contextview/browser/contextView';
import { Disposable } from 'vs/base/common/lifecycle';
import { EventHelper, addDisposableListener, getActiveDocument, getWindow } from 'vs/base/browser/dom';
import { IWorkbenchContribution, IWorkbenchContributionsRegistry, Extensions as WorkbenchExtensions } from 'vs/workbench/common/contributions';
import { Registry } from 'vs/platform/registry/common/platform';
import { LifecyclePhase } from 'vs/workbench/services/lifecycle/common/lifecycle';
import { isNative } from 'vs/base/common/platform';
import { IClipboardService } from 'vs/platform/clipboard/common/clipboardService';
import { StandardMouseEvent } from 'vs/base/browser/mouseEvent';
import { Event } from 'vs/base/common/event';

export class TextInputActionsProvider extends Disposable implements IWorkbenchContribution {

	private textInputActions: IAction[] = [];

	constructor(
		@IWorkbenchLayoutService private readonly layoutService: IWorkbenchLayoutService,
		@IContextMenuService private readonly contextMenuService: IContextMenuService,
		@IClipboardService private readonly clipboardService: IClipboardService
	) {
		super();

		this.createActions();

		this.registerListeners();
	}

	private createActions(): void {
		this.textInputActions.push(

			// Undo/Redo
			new Action('undo', localize('undo', "Undo"), undefined, true, async () => getActiveDocument().execCommand('undo')),
			new Action('redo', localize('redo', "Redo"), undefined, true, async () => getActiveDocument().execCommand('redo')),
			new Separator(),

			// Cut / Copy / Paste
			new Action('editor.action.clipboardCutAction', localize('cut', "Cut"), undefined, true, async () => getActiveDocument().execCommand('cut')),
			new Action('editor.action.clipboardCopyAction', localize('copy', "Copy"), undefined, true, async () => getActiveDocument().execCommand('copy')),
			new Action('editor.action.clipboardPasteAction', localize('paste', "Paste"), undefined, true, async element => {

				// Native: paste is supported
				if (isNative) {
					getActiveDocument().execCommand('paste');
				}

				// Web: paste is not supported due to security reasons
				else {
					const clipboardText = await this.clipboardService.readText();
					if (
						element instanceof HTMLTextAreaElement ||
						element instanceof HTMLInputElement
					) {
						const selectionStart = element.selectionStart || 0;
						const selectionEnd = element.selectionEnd || 0;

						element.value = `${element.value.substring(0, selectionStart)}${clipboardText}${element.value.substring(selectionEnd, element.value.length)}`;
						element.selectionStart = selectionStart + clipboardText.length;
						element.selectionEnd = element.selectionStart;
					}
				}
			}),
			new Separator(),

			// Select All
			new Action('editor.action.selectAll', localize('selectAll', "Select All"), undefined, true, async () => getActiveDocument().execCommand('selectAll'))
		);
	}

	private registerListeners(): void {

		// Context menu support in input/textarea
		this._register(Event.runAndSubscribe(this.layoutService.onDidAddContainer, ({ container, disposables }) => {
			disposables.add(addDisposableListener(container, 'contextmenu', e => this.onContextMenu(getWindow(container), e)));
		}, { container: this.layoutService.mainContainer, disposables: this._store }));
	}

	private onContextMenu(targetWindow: Window, e: MouseEvent): void {
		if (e.defaultPrevented) {
			return; // make sure to not show these actions by accident if component indicated to prevent
		}

		const target = e.target;
		if (!(target instanceof HTMLElement) || (target.nodeName.toLowerCase() !== 'input' && target.nodeName.toLowerCase() !== 'textarea')) {
			return; // only for inputs or textareas
		}

		EventHelper.stop(e, true);

		const event = new StandardMouseEvent(targetWindow, e);

		this.contextMenuService.showContextMenu({
			getAnchor: () => event,
			getActions: () => this.textInputActions,
			getActionsContext: () => target,
		});
	}
}

Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench).registerWorkbenchContribution(TextInputActionsProvider, LifecyclePhase.Ready);
