/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as nls from 'vs/nls';

export namespace AccessibilityHelpNLS {
	export const accessibilityHelpTitle = nls.localize('accessibilityHelpTitle', "Accessibility Help");
	export const openingDocs = nls.localize("openingDocs", "Opening the Accessibility documentation page.");
	export const readonlyDiffEditor = nls.localize("readonlyDiffEditor", "You are in a read-only pane of a diff editor.");
	export const editableDiffEditor = nls.localize("editableDiffEditor", "You are in a pane of a diff editor.");
	export const readonlyEditor = nls.localize("readonlyEditor", "You are in a read-only code editor.");
	export const editableEditor = nls.localize("editableEditor", "You are in a code editor.");
	export const changeConfigToOnMac = nls.localize("changeConfigToOnMac", "Configure the application to be optimized for usage with a Screen Reader (Command+E).");
	export const changeConfigToOnWinLinux = nls.localize("changeConfigToOnWinLinux", "Configure the application to be optimized for usage with a Screen Reader (Control+E).");
	export const auto_on = nls.localize("auto_on", "The application is configured to be optimized for usage with a Screen Reader.");
	export const auto_off = nls.localize("auto_off", "The application is configured to never be optimized for usage with a Screen Reader.");
	export const screenReaderModeEnabled = nls.localize("screenReaderModeEnabled", "Screen Reader Optimized Mode enabled.");
	export const screenReaderModeDisabled = nls.localize("screenReaderModeDisabled", "Screen Reader Optimized Mode disabled.");
	export const tabFocusModeOnMsg = nls.localize("tabFocusModeOnMsg", "Pressing Tab in the current editor will move focus to the next focusable element. Toggle this behavior {0}.");
	export const tabFocusModeOnMsgNoKb = nls.localize("tabFocusModeOnMsgNoKb", "Pressing Tab in the current editor will move focus to the next focusable element. The command {0} is currently not triggerable by a keybinding.");
	export const stickScrollKb = nls.localize("stickScrollKb", "Focus Sticky Scroll ({0}) to focus the currently nested scopes.");
	export const stickScrollNoKb = nls.localize("stickScrollNoKb", "Focus Sticky Scroll to focus the currently nested scopes. It is currently not triggerable by a keybinding.");
	export const tabFocusModeOffMsg = nls.localize("tabFocusModeOffMsg", "Pressing Tab in the current editor will insert the tab character. Toggle this behavior {0}.");
	export const tabFocusModeOffMsgNoKb = nls.localize("tabFocusModeOffMsgNoKb", "Pressing Tab in the current editor will insert the tab character. The command {0} is currently not triggerable by a keybinding.");
	export const showAccessibilityHelpAction = nls.localize("showAccessibilityHelpAction", "Show Accessibility Help");
	export const saveAudioCueDisabled = nls.localize("saveAudioCueDisabled", "`audioCues.save` is disabled, so an alert will occur when a file is saved.");
	export const saveAudioCueAlways = nls.localize("saveAudioCueAlways", "`audioCues.save` is enabled, so will play whenever a file is saved.");
	export const saveAudioCueUserGesture = nls.localize("saveAudioCueUserGesture", "`audioCues.save` is enabled, so will play when a file is saved via user gesture.");
	export const formatAudioCueDisabled = nls.localize("formatAudioCueDisabled", "`audioCues.format` is disabled, so an alert will occur when a file is formatted.");
	export const formatAudioCueAlways = nls.localize("formatAudioCueAlways", "`audioCues.format` is enabled, so will play whenever a file is formatted.");
	export const formatAudioCueUserGesture = nls.localize("formatAudioCueUserGesture", "`audioCues.format` is enabled, so will play when a file is formatted via user gesture.");
}

export namespace InspectTokensNLS {
	export const inspectTokensAction = nls.localize('inspectTokens', "Developer: Inspect Tokens");
}

export namespace GoToLineNLS {
	export const gotoLineActionLabel = nls.localize('gotoLineActionLabel', "Go to Line/Column...");
}

export namespace QuickHelpNLS {
	export const helpQuickAccessActionLabel = nls.localize('helpQuickAccess', "Show all Quick Access Providers");
}

export namespace QuickCommandNLS {
	export const quickCommandActionLabel = nls.localize('quickCommandActionLabel', "Command Palette");
	export const quickCommandHelp = nls.localize('quickCommandActionHelp', "Show And Run Commands");
}

export namespace QuickOutlineNLS {
	export const quickOutlineActionLabel = nls.localize('quickOutlineActionLabel', "Go to Symbol...");
	export const quickOutlineByCategoryActionLabel = nls.localize('quickOutlineByCategoryActionLabel', "Go to Symbol by Category...");
}

export namespace StandaloneCodeEditorNLS {
	export const editorViewAccessibleLabel = nls.localize('editorViewAccessibleLabel', "Editor content");
	export const accessibilityHelpMessage = nls.localize('accessibilityHelpMessage', "Press Alt+F1 for Accessibility Options.");
}

export namespace ToggleHighContrastNLS {
	export const toggleHighContrast = nls.localize('toggleHighContrast', "Toggle High Contrast Theme");
}

export namespace StandaloneServicesNLS {
	export const bulkEditServiceSummary = nls.localize('bulkEditServiceSummary', "Made {0} edits in {1} files");
}
