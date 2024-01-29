/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { isSafari, setFullscreen } from 'vs/base/browser/browser';
import { addDisposableListener, addDisposableThrottledListener, detectFullscreen, EventHelper, EventType, getWindows, getWindowsCount, windowOpenNoOpener, windowOpenPopup, windowOpenWithSuccess } from 'vs/base/browser/dom';
import { DomEmitter } from 'vs/base/browser/event';
import { HidDeviceData, requestHidDevice, requestSerialPort, requestUsbDevice, SerialPortData, UsbDeviceData } from 'vs/base/browser/deviceAccess';
import { timeout } from 'vs/base/common/async';
import { Event } from 'vs/base/common/event';
import { Disposable, IDisposable, dispose, toDisposable } from 'vs/base/common/lifecycle';
import { matchesScheme, Schemas } from 'vs/base/common/network';
import { isIOS, isMacintosh } from 'vs/base/common/platform';
import Severity from 'vs/base/common/severity';
import { URI } from 'vs/base/common/uri';
import { localize } from 'vs/nls';
import { CommandsRegistry } from 'vs/platform/commands/common/commands';
import { IDialogService, IPromptButton } from 'vs/platform/dialogs/common/dialogs';
import { IInstantiationService, ServicesAccessor } from 'vs/platform/instantiation/common/instantiation';
import { ILabelService } from 'vs/platform/label/common/label';
import { IOpenerService } from 'vs/platform/opener/common/opener';
import { IProductService } from 'vs/platform/product/common/productService';
import { IBrowserWorkbenchEnvironmentService } from 'vs/workbench/services/environment/browser/environmentService';
import { IWorkbenchLayoutService } from 'vs/workbench/services/layout/browser/layoutService';
import { BrowserLifecycleService } from 'vs/workbench/services/lifecycle/browser/lifecycleService';
import { ILifecycleService } from 'vs/workbench/services/lifecycle/common/lifecycle';
import { IHostService } from 'vs/workbench/services/host/browser/host';
import { registerWindowDriver } from 'vs/workbench/services/driver/browser/driver';
import { CodeWindow, isAuxiliaryWindow, mainWindow } from 'vs/base/browser/window';
import { createSingleCallFunction } from 'vs/base/common/functional';

export abstract class BaseWindow extends Disposable {

	private static TIMEOUT_HANDLES = Number.MIN_SAFE_INTEGER; // try to not compete with the IDs of native `setTimeout`
	private static readonly TIMEOUT_DISPOSABLES = new Map<number, Set<IDisposable>>();

	constructor(targetWindow: CodeWindow, dom = { getWindowsCount, getWindows } /* for testing */) {
		super();

		this.enableMultiWindowAwareTimeout(targetWindow, dom);
	}

	//#region timeout handling in multi-window applications

	/**
	 * Override `setTimeout` and `clearTimeout` on the provided window to make
	 * sure timeouts are dispatched to all opened windows. Some browsers may decide
	 * to throttle timeouts in minimized windows, so with this we can ensure the
	 * timeout is scheduled without being throttled (unless all windows are minimized).
	 */
	private enableMultiWindowAwareTimeout(targetWindow: Window, dom = { getWindowsCount, getWindows }): void {
		const originalSetTimeout = targetWindow.setTimeout;
		Object.defineProperty(targetWindow, 'vscodeOriginalSetTimeout', { get: () => originalSetTimeout });

		const originalClearTimeout = targetWindow.clearTimeout;
		Object.defineProperty(targetWindow, 'vscodeOriginalClearTimeout', { get: () => originalClearTimeout });

		targetWindow.setTimeout = function (this: unknown, handler: TimerHandler, timeout = 0, ...args: unknown[]): number {
			if (dom.getWindowsCount() === 1 || typeof handler === 'string' || timeout === 0 /* immediates are never throttled */) {
				return originalSetTimeout.apply(this, [handler, timeout, ...args]);
			}

			const timeoutDisposables = new Set<IDisposable>();
			const timeoutHandle = BaseWindow.TIMEOUT_HANDLES++;
			BaseWindow.TIMEOUT_DISPOSABLES.set(timeoutHandle, timeoutDisposables);

			const handlerFn = createSingleCallFunction(handler, () => {
				dispose(timeoutDisposables);
				BaseWindow.TIMEOUT_DISPOSABLES.delete(timeoutHandle);
			});

			for (const { window, disposables } of dom.getWindows()) {
				if (isAuxiliaryWindow(window) && window.document.visibilityState === 'hidden') {
					continue; // skip over hidden windows (but never over main window)
				}

				const handle = (window as any).vscodeOriginalSetTimeout.apply(this, [handlerFn, timeout, ...args]);

				const timeoutDisposable = toDisposable(() => {
					(window as any).vscodeOriginalClearTimeout(handle);
					timeoutDisposables.delete(timeoutDisposable);
				});

				disposables.add(timeoutDisposable);
				timeoutDisposables.add(timeoutDisposable);
			}

			return timeoutHandle;
		};

		targetWindow.clearTimeout = function (this: unknown, timeoutHandle: number | undefined): void {
			const timeoutDisposables = typeof timeoutHandle === 'number' ? BaseWindow.TIMEOUT_DISPOSABLES.get(timeoutHandle) : undefined;
			if (timeoutDisposables) {
				dispose(timeoutDisposables);
				BaseWindow.TIMEOUT_DISPOSABLES.delete(timeoutHandle!);
			} else {
				originalClearTimeout.apply(this, [timeoutHandle]);
			}
		};
	}

	//#endregion

}

export class BrowserWindow extends BaseWindow {

	constructor(
		@IOpenerService private readonly openerService: IOpenerService,
		@ILifecycleService private readonly lifecycleService: BrowserLifecycleService,
		@IDialogService private readonly dialogService: IDialogService,
		@ILabelService private readonly labelService: ILabelService,
		@IProductService private readonly productService: IProductService,
		@IBrowserWorkbenchEnvironmentService private readonly environmentService: IBrowserWorkbenchEnvironmentService,
		@IWorkbenchLayoutService private readonly layoutService: IWorkbenchLayoutService,
		@IInstantiationService private readonly instantiationService: IInstantiationService,
		@IHostService private readonly hostService: IHostService
	) {
		super(mainWindow);

		this.registerListeners();
		this.create();
	}

	private registerListeners(): void {

		// Lifecycle
		this._register(this.lifecycleService.onWillShutdown(() => this.onWillShutdown()));

		// Layout
		const viewport = isIOS && mainWindow.visualViewport ? mainWindow.visualViewport /** Visual viewport */ : mainWindow /** Layout viewport */;
		this._register(addDisposableListener(viewport, EventType.RESIZE, () => {
			this.layoutService.layout();

			// Sometimes the keyboard appearing scrolls the whole workbench out of view, as a workaround scroll back into view #121206
			if (isIOS) {
				mainWindow.scrollTo(0, 0);
			}
		}));

		// Prevent the back/forward gestures in macOS
		this._register(addDisposableListener(this.layoutService.mainContainer, EventType.WHEEL, e => e.preventDefault(), { passive: false }));

		// Prevent native context menus in web
		this._register(addDisposableListener(this.layoutService.mainContainer, EventType.CONTEXT_MENU, e => EventHelper.stop(e, true)));

		// Prevent default navigation on drop
		this._register(addDisposableListener(this.layoutService.mainContainer, EventType.DROP, e => EventHelper.stop(e, true)));

		// Fullscreen (Browser)
		for (const event of [EventType.FULLSCREEN_CHANGE, EventType.WK_FULLSCREEN_CHANGE]) {
			this._register(addDisposableListener(mainWindow.document, event, () => setFullscreen(!!detectFullscreen(mainWindow))));
		}

		// Fullscreen (Native)
		this._register(addDisposableThrottledListener(viewport, EventType.RESIZE, () => {
			setFullscreen(!!detectFullscreen(mainWindow));
		}, undefined, isMacintosh ? 2000 /* adjust for macOS animation */ : 800 /* can be throttled */));
	}

	private onWillShutdown(): void {

		// Try to detect some user interaction with the workbench
		// when shutdown has happened to not show the dialog e.g.
		// when navigation takes a longer time.
		Event.toPromise(Event.any(
			Event.once(new DomEmitter(mainWindow.document.body, EventType.KEY_DOWN, true).event),
			Event.once(new DomEmitter(mainWindow.document.body, EventType.MOUSE_DOWN, true).event)
		)).then(async () => {

			// Delay the dialog in case the user interacted
			// with the page before it transitioned away
			await timeout(3000);

			// This should normally not happen, but if for some reason
			// the workbench was shutdown while the page is still there,
			// inform the user that only a reload can bring back a working
			// state.
			await this.dialogService.prompt({
				type: Severity.Error,
				message: localize('shutdownError', "An unexpected error occurred that requires a reload of this page."),
				detail: localize('shutdownErrorDetail', "The workbench was unexpectedly disposed while running."),
				buttons: [
					{
						label: localize({ key: 'reload', comment: ['&& denotes a mnemonic'] }, "&&Reload"),
						run: () => mainWindow.location.reload() // do not use any services at this point since they are likely not functional at this point
					}
				]
			});
		});
	}

	private create(): void {

		// Handle open calls
		this.setupOpenHandlers();

		// Label formatting
		this.registerLabelFormatters();

		// Commands
		this.registerCommands();

		// Smoke Test Driver
		this.setupDriver();
	}

	private setupDriver(): void {
		if (this.environmentService.enableSmokeTestDriver) {
			registerWindowDriver(this.instantiationService);
		}
	}

	private setupOpenHandlers(): void {

		// We need to ignore the `beforeunload` event while
		// we handle external links to open specifically for
		// the case of application protocols that e.g. invoke
		// vscode itself. We do not want to open these links
		// in a new window because that would leave a blank
		// window to the user, but using `window.location.href`
		// will trigger the `beforeunload`.
		this.openerService.setDefaultExternalOpener({
			openExternal: async (href: string) => {
				let isAllowedOpener = false;
				if (this.environmentService.options?.openerAllowedExternalUrlPrefixes) {
					for (const trustedPopupPrefix of this.environmentService.options.openerAllowedExternalUrlPrefixes) {
						if (href.startsWith(trustedPopupPrefix)) {
							isAllowedOpener = true;
							break;
						}
					}
				}

				// HTTP(s): open in new window and deal with potential popup blockers
				if (matchesScheme(href, Schemas.http) || matchesScheme(href, Schemas.https)) {
					if (isSafari) {
						const opened = windowOpenWithSuccess(href, !isAllowedOpener);
						if (!opened) {
							await this.dialogService.prompt({
								type: Severity.Warning,
								message: localize('unableToOpenExternal', "The browser interrupted the opening of a new tab or window. Press 'Open' to open it anyway."),
								detail: href,
								buttons: [
									{
										label: localize({ key: 'open', comment: ['&& denotes a mnemonic'] }, "&&Open"),
										run: () => isAllowedOpener ? windowOpenPopup(href) : windowOpenNoOpener(href)
									},
									{
										label: localize({ key: 'learnMore', comment: ['&& denotes a mnemonic'] }, "&&Learn More"),
										run: () => this.openerService.open(URI.parse('https://aka.ms/allow-vscode-popup'))
									}
								],
								cancelButton: true
							});
						}
					} else {
						isAllowedOpener
							? windowOpenPopup(href)
							: windowOpenNoOpener(href);
					}
				}

				// Anything else: set location to trigger protocol handler in the browser
				// but make sure to signal this as an expected unload and disable unload
				// handling explicitly to prevent the workbench from going down.
				else {
					const invokeProtocolHandler = () => {
						this.lifecycleService.withExpectedShutdown({ disableShutdownHandling: true }, () => mainWindow.location.href = href);
					};

					invokeProtocolHandler();

					const showProtocolUrlOpenedDialog = async () => {
						const { downloadUrl } = this.productService;
						let detail: string;

						const buttons: IPromptButton<void>[] = [
							{
								label: localize({ key: 'openExternalDialogButtonRetry.v2', comment: ['&& denotes a mnemonic'] }, "&&Try Again"),
								run: () => invokeProtocolHandler()
							}
						];

						if (downloadUrl !== undefined) {
							detail = localize(
								'openExternalDialogDetail.v2',
								"We launched {0} on your computer.\n\nIf {1} did not launch, try again or install it below.",
								this.productService.nameLong,
								this.productService.nameLong
							);

							buttons.push({
								label: localize({ key: 'openExternalDialogButtonInstall.v3', comment: ['&& denotes a mnemonic'] }, "&&Install"),
								run: async () => {
									await this.openerService.open(URI.parse(downloadUrl));

									// Re-show the dialog so that the user can come back after installing and try again
									showProtocolUrlOpenedDialog();
								}
							});
						} else {
							detail = localize(
								'openExternalDialogDetailNoInstall',
								"We launched {0} on your computer.\n\nIf {1} did not launch, try again below.",
								this.productService.nameLong,
								this.productService.nameLong
							);
						}

						// While this dialog shows, closing the tab will not display a confirmation dialog
						// to avoid showing the user two dialogs at once
						await this.hostService.withExpectedShutdown(() => this.dialogService.prompt({
							type: Severity.Info,
							message: localize('openExternalDialogTitle', "All done. You can close this tab now."),
							detail,
							buttons,
							cancelButton: true
						}));
					};

					// We cannot know whether the protocol handler succeeded.
					// Display guidance in case it did not, e.g. the app is not installed locally.
					if (matchesScheme(href, this.productService.urlProtocol)) {
						await showProtocolUrlOpenedDialog();
					}
				}

				return true;
			}
		});
	}

	private registerLabelFormatters(): void {
		this._register(this.labelService.registerFormatter({
			scheme: Schemas.vscodeUserData,
			priority: true,
			formatting: {
				label: '(Settings) ${path}',
				separator: '/',
			}
		}));
	}

	private registerCommands(): void {

		// Allow extensions to request USB devices in Web
		CommandsRegistry.registerCommand('workbench.experimental.requestUsbDevice', async (_accessor: ServicesAccessor, options?: { filters?: unknown[] }): Promise<UsbDeviceData | undefined> => {
			return requestUsbDevice(options);
		});

		// Allow extensions to request Serial devices in Web
		CommandsRegistry.registerCommand('workbench.experimental.requestSerialPort', async (_accessor: ServicesAccessor, options?: { filters?: unknown[] }): Promise<SerialPortData | undefined> => {
			return requestSerialPort(options);
		});

		// Allow extensions to request HID devices in Web
		CommandsRegistry.registerCommand('workbench.experimental.requestHidDevice', async (_accessor: ServicesAccessor, options?: { filters?: unknown[] }): Promise<HidDeviceData | undefined> => {
			return requestHidDevice(options);
		});
	}
}
