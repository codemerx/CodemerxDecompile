/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

/* eslint-disable @typescript-eslint/naming-convention */

import { IBufferCell } from '@xterm/xterm';

export type XtermAttributes = Omit<IBufferCell, 'getWidth' | 'getChars' | 'getCode'> & { clone?(): XtermAttributes };

export interface IXtermCore {
	viewport?: {
		readonly scrollBarWidth: number;
		_innerRefresh(): void;
	};
	_onData: IEventEmitter<string>;
	_onKey: IEventEmitter<{ key: string }>;

	_charSizeService: {
		width: number;
		height: number;
	};

	coreService: {
		triggerDataEvent(data: string, wasUserInput?: boolean): void;
	};

	_inputHandler: {
		_curAttrData: XtermAttributes;
	};

	_renderService: {
		dimensions: {
			css: {
				cell: {
					width: number;
					height: number;
				}
			}
		},
		_renderer: {
			value?: {
				_renderLayers?: any[];
			}
		};
		_handleIntersectionChange: any;
	};
}

export interface IEventEmitter<T> {
	fire(e: T): void;
}
