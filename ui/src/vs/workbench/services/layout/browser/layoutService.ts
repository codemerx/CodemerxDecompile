/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { refineServiceDecorator } from 'vs/platform/instantiation/common/instantiation';
import { Event } from 'vs/base/common/event';
import { ILayoutService } from 'vs/platform/layout/browser/layoutService';
import { Part } from 'vs/workbench/browser/part';
import { IDimension } from 'vs/base/browser/dom';
import { Direction } from 'vs/base/browser/ui/grid/grid';

export const IWorkbenchLayoutService = refineServiceDecorator<ILayoutService, IWorkbenchLayoutService>(ILayoutService);

export const enum Parts {
	TITLEBAR_PART = 'workbench.parts.titlebar',
	BANNER_PART = 'workbench.parts.banner',
	ACTIVITYBAR_PART = 'workbench.parts.activitybar',
	SIDEBAR_PART = 'workbench.parts.sidebar',
	PANEL_PART = 'workbench.parts.panel',
	AUXILIARYBAR_PART = 'workbench.parts.auxiliarybar',
	EDITOR_PART = 'workbench.parts.editor',
	STATUSBAR_PART = 'workbench.parts.statusbar'
}

export const enum LayoutSettings {
	ACTIVITY_BAR_LOCATION = 'workbench.activityBar.location',
	EDITOR_TABS_MODE = 'workbench.editor.showTabs',
	COMMAND_CENTER = 'window.commandCenter',
}

export const enum ActivityBarPosition {
	SIDE = 'side',
	TOP = 'top',
	HIDDEN = 'hidden'
}

export const enum EditorTabsMode {
	MULTIPLE = 'multiple',
	SINGLE = 'single',
	NONE = 'none'
}

export const enum Position {
	LEFT,
	RIGHT,
	BOTTOM
}

export const enum PanelOpensMaximizedOptions {
	ALWAYS,
	NEVER,
	REMEMBER_LAST
}

export type PanelAlignment = 'left' | 'center' | 'right' | 'justify';

export function positionToString(position: Position): string {
	switch (position) {
		case Position.LEFT: return 'left';
		case Position.RIGHT: return 'right';
		case Position.BOTTOM: return 'bottom';
		default: return 'bottom';
	}
}

const positionsByString: { [key: string]: Position } = {
	[positionToString(Position.LEFT)]: Position.LEFT,
	[positionToString(Position.RIGHT)]: Position.RIGHT,
	[positionToString(Position.BOTTOM)]: Position.BOTTOM
};

export function positionFromString(str: string): Position {
	return positionsByString[str];
}

function panelOpensMaximizedSettingToString(setting: PanelOpensMaximizedOptions): string {
	switch (setting) {
		case PanelOpensMaximizedOptions.ALWAYS: return 'always';
		case PanelOpensMaximizedOptions.NEVER: return 'never';
		case PanelOpensMaximizedOptions.REMEMBER_LAST: return 'preserve';
		default: return 'preserve';
	}
}

const panelOpensMaximizedByString: { [key: string]: PanelOpensMaximizedOptions } = {
	[panelOpensMaximizedSettingToString(PanelOpensMaximizedOptions.ALWAYS)]: PanelOpensMaximizedOptions.ALWAYS,
	[panelOpensMaximizedSettingToString(PanelOpensMaximizedOptions.NEVER)]: PanelOpensMaximizedOptions.NEVER,
	[panelOpensMaximizedSettingToString(PanelOpensMaximizedOptions.REMEMBER_LAST)]: PanelOpensMaximizedOptions.REMEMBER_LAST
};

export function panelOpensMaximizedFromString(str: string): PanelOpensMaximizedOptions {
	return panelOpensMaximizedByString[str];
}

export type MULTI_WINDOW_PARTS = Parts.EDITOR_PART | Parts.STATUSBAR_PART | Parts.TITLEBAR_PART;
export type SINGLE_WINDOW_PARTS = Exclude<Parts, MULTI_WINDOW_PARTS>;

export interface IWorkbenchLayoutService extends ILayoutService {

	readonly _serviceBrand: undefined;

	/**
	 * Emits when the zen mode is enabled or disabled.
	 */
	readonly onDidChangeZenMode: Event<boolean>;

	/**
	 * Emits when fullscreen is enabled or disabled.
	 */
	readonly onDidChangeFullscreen: Event<boolean>;

	/**
	 * Emits when the target window is maximized or unmaximized.
	 */
	readonly onDidChangeWindowMaximized: Event<{ readonly windowId: number; readonly maximized: boolean }>;

	/**
	 * Emits when centered layout is enabled or disabled.
	 */
	readonly onDidChangeCenteredLayout: Event<boolean>;

	/*
	 * Emit when panel position changes.
	 */
	readonly onDidChangePanelPosition: Event<string>;

	/**
	 * Emit when panel alignment changes.
	 */
	readonly onDidChangePanelAlignment: Event<PanelAlignment>;

	/**
	 * Emit when part visibility changes
	 */
	readonly onDidChangePartVisibility: Event<void>;

	/**
	 * Emit when notifications (toasts or center) visibility changes.
	 */
	readonly onDidChangeNotificationsVisibility: Event<boolean>;

	/**
	 * True if a default layout with default editors was applied at startup
	 */
	readonly openedDefaultEditors: boolean;

	/**
	 * Run a layout of the workbench.
	 */
	layout(): void;

	/**
	 * Asks the part service if all parts have been fully restored. For editor part
	 * this means that the contents of visible editors have loaded.
	 */
	isRestored(): boolean;

	/**
	 * A promise for to await the `isRestored()` condition to be `true`.
	 */
	readonly whenRestored: Promise<void>;

	/**
	 * Returns whether the given part has the keyboard focus or not.
	 */
	hasFocus(part: Parts): boolean;

	/**
	 * Focuses the part in the target window. If the part is not visible this is a noop.
	 */
	focusPart(part: SINGLE_WINDOW_PARTS): void;
	focusPart(part: MULTI_WINDOW_PARTS, targetWindow: Window): void;
	focusPart(part: Parts, targetWindow: Window): void;

	/**
	 * Returns the target window container or parts HTML element within, if there is one.
	 */
	getContainer(targetWindow: Window): HTMLElement;
	getContainer(targetWindow: Window, part: Parts): HTMLElement | undefined;

	/**
	 * Returns if the part is visible in the target window.
	 */
	isVisible(part: SINGLE_WINDOW_PARTS): boolean;
	isVisible(part: MULTI_WINDOW_PARTS, targetWindow: Window): boolean;
	isVisible(part: Parts, targetWindow: Window): boolean;

	/**
	 * Set part hidden or not in the target window.
	 */
	setPartHidden(hidden: boolean, part: Exclude<SINGLE_WINDOW_PARTS, Parts.STATUSBAR_PART | Parts.TITLEBAR_PART>): void;
	setPartHidden(hidden: boolean, part: Exclude<MULTI_WINDOW_PARTS, Parts.STATUSBAR_PART | Parts.TITLEBAR_PART>, targetWindow: Window): void;
	setPartHidden(hidden: boolean, part: Exclude<Parts, Parts.STATUSBAR_PART | Parts.TITLEBAR_PART>, targetWindow: Window): void;

	/**
	 * Maximizes the panel height if the panel is not already maximized.
	 * Shrinks the panel to the default starting size if the panel is maximized.
	 */
	toggleMaximizedPanel(): void;

	/**
	 * Returns true if the main window has a border.
	 */
	hasMainWindowBorder(): boolean;

	/**
	 * Returns the main window border radius if any.
	 */
	getMainWindowBorderRadius(): string | undefined;

	/**
	 * Returns true if the panel is maximized.
	 */
	isPanelMaximized(): boolean;

	/**
	 * Gets the current side bar position. Note that the sidebar can be hidden too.
	 */
	getSideBarPosition(): Position;

	/**
	 * Toggles the menu bar visibility.
	 */
	toggleMenuBar(): void;

	/*
	 * Gets the current panel position. Note that the panel can be hidden too.
	 */
	getPanelPosition(): Position;

	/**
	 * Sets the panel position.
	 */
	setPanelPosition(position: Position): void;

	/**
	 * Gets the panel alignement.
	 */
	getPanelAlignment(): PanelAlignment;

	/**
	 * Sets the panel alignment.
	 */
	setPanelAlignment(alignment: PanelAlignment): void;

	/**
	 * Gets the maximum possible size for editor in the given container.
	 */
	getMaximumEditorDimensions(container: HTMLElement): IDimension;

	/**
	 * Toggles the workbench in and out of zen mode - parts get hidden and window goes fullscreen.
	 */
	toggleZenMode(): void;

	/**
	 * Returns whether the centered editor layout is active on the main editor part.
	 */
	isMainEditorLayoutCentered(): boolean;

	/**
	 * Sets the main editor part in and out of centered layout.
	 */
	centerMainEditorLayout(active: boolean): void;

	/**
	 * Resize the provided part in the main window.
	 */
	resizePart(part: Parts, sizeChangeWidth: number, sizeChangeHeight: number): void;

	/**
	 * Register a part to participate in the layout.
	 */
	registerPart(part: Part): void;

	/**
	 * Returns whether the target window is maximized.
	 */
	isWindowMaximized(targetWindow: Window): boolean;

	/**
	 * Updates the maximized state of the target window.
	 */
	updateWindowMaximizedState(targetWindow: Window, maximized: boolean): void;

	/**
	 * Returns the next visible view part in a given direction in the main window.
	 */
	getVisibleNeighborPart(part: Parts, direction: Direction): Parts | undefined;
}
