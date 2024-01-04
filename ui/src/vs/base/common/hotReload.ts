/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { IDisposable } from 'vs/base/common/lifecycle';
import { env } from 'vs/base/common/process';

export function isHotReloadEnabled(): boolean {
	return env && !!env['VSCODE_DEV'];
}
export function registerHotReloadHandler(handler: HotReloadHandler): IDisposable {
	if (!isHotReloadEnabled()) {
		return { dispose() { } };
	} else {
		const handlers = registerGlobalHotReloadHandler();
		handlers.add(handler);
		return {
			dispose() { handlers.delete(handler); }
		};
	}
}

/**
 * Takes the old exports of the module to reload and returns a function to apply the new exports.
 * If `undefined` is returned, this handler is not able to handle the module.
 *
 * If no handler can apply the new exports, the module will not be reloaded.
 */
export type HotReloadHandler = (args: { oldExports: Record<string, unknown>; newSrc: string }) => AcceptNewExportsHandler | undefined;
export type AcceptNewExportsHandler = (newExports: Record<string, unknown>) => boolean;

function registerGlobalHotReloadHandler() {
	if (!hotReloadHandlers) {
		hotReloadHandlers = new Set();
	}

	const g = globalThis as unknown as GlobalThisAddition;
	if (!g.$hotReload_applyNewExports) {
		g.$hotReload_applyNewExports = oldExports => {
			for (const h of hotReloadHandlers!) {
				const result = h(oldExports);
				if (result) { return result; }
			}
			return undefined;
		};
	}

	return hotReloadHandlers;
}

let hotReloadHandlers: Set<(args: { oldExports: Record<string, unknown>; newSrc: string }) => AcceptNewExportsFn | undefined> | undefined = undefined;

interface GlobalThisAddition {
	$hotReload_applyNewExports?(args: { oldExports: Record<string, unknown>; newSrc: string }): AcceptNewExportsFn | undefined;
}


type AcceptNewExportsFn = (newExports: Record<string, unknown>) => boolean;

if (isHotReloadEnabled()) {
	// This code does not run in production.
	registerHotReloadHandler(({ oldExports, newSrc }) => {
		// Don't match its own source code
		if (newSrc.indexOf('/* ' + 'hot-reload:patch-prototype-methods */') === -1) {
			return undefined;
		}
		return newExports => {
			for (const key in newExports) {
				const exportedItem = newExports[key];
				console.log(`[hot-reload] Patching prototype methods of '${key}'`, { exportedItem });
				if (typeof exportedItem === 'function' && exportedItem.prototype) {
					const oldExportedItem = oldExports[key];
					if (oldExportedItem) {
						for (const prop of Object.getOwnPropertyNames(exportedItem.prototype)) {
							const descriptor = Object.getOwnPropertyDescriptor(exportedItem.prototype, prop)!;
							const oldDescriptor = Object.getOwnPropertyDescriptor((oldExportedItem as any).prototype, prop);

							if (descriptor?.value?.toString() !== oldDescriptor?.value?.toString()) {
								console.log(`[hot-reload] Patching prototype method '${key}.${prop}'`);
							}

							Object.defineProperty((oldExportedItem as any).prototype, prop, descriptor);
						}
						newExports[key] = oldExportedItem;
					}
				}
			}
			return true;
		};
	});
}
