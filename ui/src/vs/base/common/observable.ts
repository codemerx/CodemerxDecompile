/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

// This is a facade for the observable implementation. Only import from here!

export {
	IObservable,
	IObserver,
	IReader,
	ISettable,
	ISettableObservable,
	ITransaction,
	IChangeContext,
	IChangeTracker,
	observableValue,
	disposableObservableValue,
	transaction,
	subtransaction,
} from 'vs/base/common/observableInternal/base';
export {
	derived,
	derivedOpts,
	derivedHandleChanges,
	derivedWithStore,
} from 'vs/base/common/observableInternal/derived';
export {
	autorun,
	autorunDelta,
	autorunHandleChanges,
	autorunWithStore,
	autorunOpts,
	autorunWithStoreHandleChanges,
} from 'vs/base/common/observableInternal/autorun';
export {
	IObservableSignal,
	constObservable,
	debouncedObservable,
	derivedObservableWithCache,
	derivedObservableWithWritableCache,
	keepObserved,
	recomputeInitiallyAndOnChange,
	observableFromEvent,
	observableFromPromise,
	observableSignal,
	observableSignalFromEvent,
	waitForState,
	wasEventTriggeredRecently,
} from 'vs/base/common/observableInternal/utils';

import { ConsoleObservableLogger, setLogger } from 'vs/base/common/observableInternal/logging';

const enableLogging = false;
if (enableLogging) {
	setLogger(new ConsoleObservableLogger());
}
