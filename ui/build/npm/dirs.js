/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

const fs = require('fs');

// Complete list of directories where yarn should be executed to install node modules
const dirs = [
	'',
	'build',
	'extensions',
	'extensions/configuration-editing',
	'extensions/css-language-features',
	'extensions/css-language-features/server',
	'extensions/debug-auto-launch',
	'extensions/debug-server-ready',
	'extensions/emmet',
	'extensions/extension-editing',
	'extensions/git',
	'extensions/git-base',
	'extensions/github',
	'extensions/github-authentication',
	/* AGPL */
	// 'extensions/grunt',
	// 'extensions/gulp',
	/* End AGPL */
	'extensions/html-language-features',
	'extensions/html-language-features/server',
	'extensions/ipynb',
	/* AGPL */
	// 'extensions/jake',
	/* End AGPL */
	'extensions/json-language-features',
	'extensions/json-language-features/server',
	'extensions/markdown-language-features/server',
	'extensions/markdown-language-features',
	'extensions/markdown-math',
	'extensions/media-preview',
	'extensions/merge-conflict',
	'extensions/microsoft-authentication',
	'extensions/notebook-renderers',
	'extensions/npm',
	/* AGPL */
	// 'extensions/php-language-features',
	/* End AGPL */
	'extensions/references-view',
	'extensions/search-result',
	'extensions/simple-browser',
	'extensions/tunnel-forwarding',
	'extensions/typescript-language-features',
	'extensions/vscode-api-tests',
	'extensions/vscode-colorize-tests',
	'extensions/vscode-test-resolver',
	/* AGPL */
	// 'remote',
	// 'remote/web',
	// 'test/automation',
	// 'test/integration/browser',
	// 'test/monaco',
	// 'test/smoke',
	/* End AGPL */
];

if (fs.existsSync(`${__dirname}/../../.build/distro/npm`)) {
	dirs.push('.build/distro/npm');
	dirs.push('.build/distro/npm/remote');
	dirs.push('.build/distro/npm/remote/web');
}

exports.dirs = dirs;
