/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { localize } from 'vs/nls';
import { ConfigurationScope, Extensions, IConfigurationNode, IConfigurationRegistry } from 'vs/platform/configuration/common/configurationRegistry';
import { Registry } from 'vs/platform/registry/common/platform';
import { RawContextKey } from 'vs/platform/contextkey/common/contextkey';
import { workbenchConfigurationNodeBase } from 'vs/workbench/common/configuration';

export const accessibilityHelpIsShown = new RawContextKey<boolean>('accessibilityHelpIsShown', false, true);
export const accessibleViewIsShown = new RawContextKey<boolean>('accessibleViewIsShown', false, true);
export const accessibleViewSupportsNavigation = new RawContextKey<boolean>('accessibleViewSupportsNavigation', false, true);
export const accessibleViewVerbosityEnabled = new RawContextKey<boolean>('accessibleViewVerbosityEnabled', false, true);
export const accessibleViewGoToSymbolSupported = new RawContextKey<boolean>('accessibleViewGoToSymbolSupported', false, true);
export const accessibleViewOnLastLine = new RawContextKey<boolean>('accessibleViewOnLastLine', false, true);
export const accessibleViewCurrentProviderId = new RawContextKey<string>('accessibleViewCurrentProviderId', undefined, undefined);

/**
 * Miscellaneous settings tagged with accessibility and implemented in the accessibility contrib but
 * were better to live under workbench for discoverability.
 */
export const enum AccessibilityWorkbenchSettingId {
	DimUnfocusedEnabled = 'accessibility.dimUnfocused.enabled',
	DimUnfocusedOpacity = 'accessibility.dimUnfocused.opacity',
	HideAccessibleView = 'accessibility.hideAccessibleView',
	AccessibleViewCloseOnKeyPress = 'accessibility.accessibleView.closeOnKeyPress'
}

export const enum ViewDimUnfocusedOpacityProperties {
	Default = 0.75,
	Minimum = 0.2,
	Maximum = 1
}

export const enum AccessibilityVoiceSettingId {
	SpeechTimeout = 'accessibility.voice.speechTimeout',
}
export const SpeechTimeoutDefault = 1200;

export const enum AccessibilityVerbositySettingId {
	Terminal = 'accessibility.verbosity.terminal',
	DiffEditor = 'accessibility.verbosity.diffEditor',
	Chat = 'accessibility.verbosity.panelChat',
	InlineChat = 'accessibility.verbosity.inlineChat',
	InlineCompletions = 'accessibility.verbosity.inlineCompletions',
	KeybindingsEditor = 'accessibility.verbosity.keybindingsEditor',
	Notebook = 'accessibility.verbosity.notebook',
	Editor = 'accessibility.verbosity.editor',
	Hover = 'accessibility.verbosity.hover',
	Notification = 'accessibility.verbosity.notification',
	EmptyEditorHint = 'accessibility.verbosity.emptyEditorHint',
	Comments = 'accessibility.verbosity.comments'
}

export const enum AccessibilityAlertSettingId {
	Save = 'accessibility.alert.save',
	Format = 'accessibility.alert.format'
}

export const enum AccessibleViewProviderId {
	Terminal = 'terminal',
	TerminalHelp = 'terminal-help',
	DiffEditor = 'diffEditor',
	Chat = 'panelChat',
	InlineChat = 'inlineChat',
	InlineCompletions = 'inlineCompletions',
	KeybindingsEditor = 'keybindingsEditor',
	Notebook = 'notebook',
	Editor = 'editor',
	Hover = 'hover',
	Notification = 'notification',
	EmptyEditorHint = 'emptyEditorHint',
	Comments = 'comments'
}

const baseProperty: object = {
	type: 'boolean',
	default: true,
	tags: ['accessibility']
};

const configuration: IConfigurationNode = {
	id: 'accessibility',
	title: localize('accessibilityConfigurationTitle', "Accessibility"),
	type: 'object',
	properties: {
		[AccessibilityVerbositySettingId.Terminal]: {
			description: localize('verbosity.terminal.description', 'Provide information about how to access the terminal accessibility help menu when the terminal is focused.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.DiffEditor]: {
			description: localize('verbosity.diffEditor.description', 'Provide information about how to navigate changes in the diff editor when it is focused.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.Chat]: {
			description: localize('verbosity.chat.description', 'Provide information about how to access the chat help menu when the chat input is focused.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.InlineChat]: {
			description: localize('verbosity.interactiveEditor.description', 'Provide information about how to access the inline editor chat accessibility help menu and alert with hints that describe how to use the feature when the input is focused.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.InlineCompletions]: {
			description: localize('verbosity.inlineCompletions.description', 'Provide information about how to access the inline completions hover and accessible view.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.KeybindingsEditor]: {
			description: localize('verbosity.keybindingsEditor.description', 'Provide information about how to change a keybinding in the keybindings editor when a row is focused.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.Notebook]: {
			description: localize('verbosity.notebook', 'Provide information about how to focus the cell container or inner editor when a notebook cell is focused.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.Hover]: {
			description: localize('verbosity.hover', 'Provide information about how to open the hover in an accessible view.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.Notification]: {
			description: localize('verbosity.notification', 'Provide information about how to open the notification in an accessible view.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.EmptyEditorHint]: {
			description: localize('verbosity.emptyEditorHint', 'Provide information about relevant actions in an empty text editor.'),
			...baseProperty
		},
		[AccessibilityVerbositySettingId.Comments]: {
			description: localize('verbosity.comments', 'Provide information about actions that can be taken in the comment widget or in a file which contains comments.'),
			...baseProperty
		},
		[AccessibilityAlertSettingId.Save]: {
			'markdownDescription': localize('alert.save', "When in screen reader mode, alerts when a file is saved. Note that this will be ignored when {0} is enabled.", '`#audioCues.save#`'),
			'type': 'string',
			'enum': ['userGesture', 'always', 'never'],
			'default': 'always',
			'enumDescriptions': [
				localize('alert.save.userGesture', "Alerts when a file is saved via user gesture."),
				localize('alert.save.always', "Alerts whenever is a file is saved, including auto save."),
				localize('alert.save.never', "Never alerts.")
			],
			tags: ['accessibility']
		},
		[AccessibilityAlertSettingId.Format]: {
			'markdownDescription': localize('alert.format', "When in screen reader mode, alerts when a file or notebook cell is formatted. Note that this will be ignored when {0} is enabled.", '`#audioCues.format#`'),
			'type': 'string',
			'enum': ['userGesture', 'always', 'never'],
			'default': 'always',
			'enumDescriptions': [
				localize('alert.format.userGesture', "Alerts when a file is formatted via user gesture."),
				localize('alert.format.always', "Alerts whenever is a file is formatted, including auto save, on cell execution, and more."),
				localize('alert.format.never', "Never alerts.")
			],
			tags: ['accessibility']
		},
		[AccessibilityVoiceSettingId.SpeechTimeout]: {
			'markdownDescription': localize('voice.speechTimeout', "Define the duration for which the voice speech recognition remains active after you stop speaking. For example in a chat session the transcribed text is submitted automatically after the timeout is met. Set to `0` to disable this feature."),
			'type': 'number',
			'default': SpeechTimeoutDefault,
			'minimum': 0,
			'tags': ['accessibility']
		},
		[AccessibilityWorkbenchSettingId.AccessibleViewCloseOnKeyPress]: {
			markdownDescription: localize('terminal.integrated.accessibleView.closeOnKeyPress', "On keypress, close the accessible view and focus the element from which it was invoked."),
			type: 'boolean',
			default: true
		},
	}
};

export function registerAccessibilityConfiguration() {
	const registry = Registry.as<IConfigurationRegistry>(Extensions.Configuration);
	registry.registerConfiguration(configuration);

	registry.registerConfiguration({
		...workbenchConfigurationNodeBase,
		properties: {
			[AccessibilityWorkbenchSettingId.DimUnfocusedEnabled]: {
				description: localize('dimUnfocusedEnabled', 'Whether to dim unfocused editors and terminals, which makes it more clear where typed input will go to. This works with the majority of editors with the notable exceptions of those that utilize iframes like notebooks and extension webview editors.'),
				type: 'boolean',
				default: false,
				tags: ['accessibility'],
				scope: ConfigurationScope.APPLICATION,
			},
			[AccessibilityWorkbenchSettingId.DimUnfocusedOpacity]: {
				markdownDescription: localize('dimUnfocusedOpacity', 'The opacity fraction (0.2 to 1.0) to use for unfocused editors and terminals. This will only take effect when {0} is enabled.', `\`#${AccessibilityWorkbenchSettingId.DimUnfocusedEnabled}#\``),
				type: 'number',
				minimum: ViewDimUnfocusedOpacityProperties.Minimum,
				maximum: ViewDimUnfocusedOpacityProperties.Maximum,
				default: ViewDimUnfocusedOpacityProperties.Default,
				tags: ['accessibility'],
				scope: ConfigurationScope.APPLICATION,
			},
			[AccessibilityWorkbenchSettingId.HideAccessibleView]: {
				description: localize('accessibility.hideAccessibleView', "Controls whether the accessible view is hidden."),
				type: 'boolean',
				default: false,
				tags: ['accessibility']
			}
		}
	});
}
