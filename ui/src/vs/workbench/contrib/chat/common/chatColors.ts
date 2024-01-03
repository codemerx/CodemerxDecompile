/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Color, RGBA } from 'vs/base/common/color';
import { localize } from 'vs/nls';
import { badgeBackground, badgeForeground, contrastBorder, foreground, registerColor } from 'vs/platform/theme/common/colorRegistry';

export const chatRequestBorder = registerColor(
	'chat.requestBorder',
	{ dark: new Color(new RGBA(255, 255, 255, 0.10)), light: new Color(new RGBA(0, 0, 0, 0.10)), hcDark: contrastBorder, hcLight: contrastBorder, },
	localize('chat.requestBorder', 'The border color of a chat request.')
);

export const chatSlashCommandBackground = registerColor(
	'chat.slashCommandBackground',
	{ dark: '#34414B', light: '#D2ECFF', hcDark: Color.white, hcLight: badgeBackground },
	localize('chat.slashCommandBackground', 'The background color of a chat slash command.')
);

export const chatSlashCommandForeground = registerColor(
	'chat.slashCommandForeground',
	{ dark: '#40A6FF', light: '#306CA2', hcDark: Color.black, hcLight: badgeForeground },
	localize('chat.slashCommandForeground', 'The foreground color of a chat slash command.')
);

export const chatAvatarBackground = registerColor(
	'chat.avatarBackground',
	{ dark: '#1f1f1f', light: '#f2f2f2', hcDark: Color.black, hcLight: Color.white, },
	localize('chat.avatarBackground', 'The background color of a chat avatar.')
);

export const chatAvatarForeground = registerColor(
	'chat.avatarForeground',
	{ dark: foreground, light: foreground, hcDark: foreground, hcLight: foreground, },
	localize('chat.avatarForeground', 'The foreground color of a chat avatar.')
);
