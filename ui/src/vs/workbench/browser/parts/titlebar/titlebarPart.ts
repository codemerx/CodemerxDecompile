/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import 'vs/css!./media/titlebarpart';
import { localize } from 'vs/nls';
import { MultiWindowParts, Part } from 'vs/workbench/browser/part';
import { ITitleService } from 'vs/workbench/services/title/browser/titleService';
import { getZoomFactor, isWCOEnabled } from 'vs/base/browser/browser';
import { MenuBarVisibility, getTitleBarStyle, getMenuBarVisibility } from 'vs/platform/window/common/window';
import { IContextMenuService } from 'vs/platform/contextview/browser/contextView';
import { StandardMouseEvent } from 'vs/base/browser/mouseEvent';
import { IConfigurationService, IConfigurationChangeEvent } from 'vs/platform/configuration/common/configuration';
import { DisposableStore, IDisposable } from 'vs/base/common/lifecycle';
import { IBrowserWorkbenchEnvironmentService } from 'vs/workbench/services/environment/browser/environmentService';
import { IThemeService } from 'vs/platform/theme/common/themeService';
import { ThemeIcon } from 'vs/base/common/themables';
import { TITLE_BAR_ACTIVE_BACKGROUND, TITLE_BAR_ACTIVE_FOREGROUND, TITLE_BAR_INACTIVE_FOREGROUND, TITLE_BAR_INACTIVE_BACKGROUND, TITLE_BAR_BORDER, WORKBENCH_BACKGROUND } from 'vs/workbench/common/theme';
import { isMacintosh, isWindows, isLinux, isWeb, isNative, platformLocale } from 'vs/base/common/platform';
import { Color } from 'vs/base/common/color';
import { EventType, EventHelper, Dimension, append, $, addDisposableListener, prepend, reset, getWindow, getWindowId, isAncestor } from 'vs/base/browser/dom';
import { CustomMenubarControl } from 'vs/workbench/browser/parts/titlebar/menubarControl';
import { IInstantiationService } from 'vs/platform/instantiation/common/instantiation';
import { Emitter, Event } from 'vs/base/common/event';
import { IStorageService } from 'vs/platform/storage/common/storage';
import { Parts, IWorkbenchLayoutService, ActivityBarPosition, LayoutSettings } from 'vs/workbench/services/layout/browser/layoutService';
import { createActionViewItem, createAndFillInActionBarActions } from 'vs/platform/actions/browser/menuEntryActionViewItem';
import { Action2, IMenu, IMenuService, MenuId, registerAction2 } from 'vs/platform/actions/common/actions';
import { IContextKeyService } from 'vs/platform/contextkey/common/contextkey';
import { IHostService } from 'vs/workbench/services/host/browser/host';
import { Codicon } from 'vs/base/common/codicons';
import { getIconRegistry } from 'vs/platform/theme/common/iconRegistry';
import { WindowTitle } from 'vs/workbench/browser/parts/titlebar/windowTitle';
import { CommandCenterControl } from 'vs/workbench/browser/parts/titlebar/commandCenterControl';
import { IHoverDelegate } from 'vs/base/browser/ui/iconLabel/iconHoverDelegate';
import { IHoverService } from 'vs/workbench/services/hover/browser/hover';
import { Categories } from 'vs/platform/action/common/actionCommonCategories';
import { WorkbenchToolBar } from 'vs/platform/actions/browser/toolbar';
import { ACCOUNTS_ACTIVITY_ID, GLOBAL_ACTIVITY_ID } from 'vs/workbench/common/activity';
import { SimpleAccountActivityActionViewItem, SimpleGlobalActivityActionViewItem } from 'vs/workbench/browser/parts/globalCompositeBar';
import { HoverPosition } from 'vs/base/browser/ui/hover/hoverWidget';
import { IEditorGroupsContainer, IEditorGroupsService } from 'vs/workbench/services/editor/common/editorGroupsService';
import { ActionRunner, IAction } from 'vs/base/common/actions';
import { IEditorService } from 'vs/workbench/services/editor/common/editorService';
import { ActionsOrientation, IActionViewItem, prepareActions } from 'vs/base/browser/ui/actionbar/actionbar';
import { EDITOR_CORE_NAVIGATION_COMMANDS } from 'vs/workbench/browser/parts/editor/editorCommands';
import { AnchorAlignment } from 'vs/base/browser/ui/contextview/contextview';
import { EditorPane } from 'vs/workbench/browser/parts/editor/editorPane';
import { IKeybindingService } from 'vs/platform/keybinding/common/keybinding';
import { ResolvedKeybinding } from 'vs/base/common/keybindings';
import { EditorCommandsContextActionRunner } from 'vs/workbench/browser/parts/editor/editorTabsControl';
import { IEditorCommandsContext, IToolbarActions } from 'vs/workbench/common/editor';
import { mainWindow } from 'vs/base/browser/window';
import { ACCOUNTS_ACTIVITY_TILE_ACTION, GLOBAL_ACTIVITY_TITLE_ACTION } from 'vs/workbench/browser/parts/titlebar/titlebarActions';
import { IView } from 'vs/base/browser/ui/grid/grid';

export interface ITitleProperties {
	isPure?: boolean;
	isAdmin?: boolean;
	prefix?: string;
}

export interface ITitlebarPart extends IDisposable {

	/**
	 * An event when the menubar visibility changes.
	 */
	readonly onMenubarVisibilityChange: Event<boolean>;

	/**
	 * Update some environmental title properties.
	 */
	updateProperties(properties: ITitleProperties): void;
}

export class BrowserTitleService extends MultiWindowParts<BrowserTitlebarPart> implements ITitleService {

	declare _serviceBrand: undefined;

	readonly mainPart = this._register(this.createMainTitlebarPart());

	constructor(
		@IInstantiationService protected readonly instantiationService: IInstantiationService
	) {
		super();

		this._register(this.registerPart(this.mainPart));
	}

	protected createMainTitlebarPart(): BrowserTitlebarPart {
		return this.instantiationService.createInstance(MainBrowserTitlebarPart);
	}

	//#region Auxiliary Titlebar Parts

	createAuxiliaryTitlebarPart(container: HTMLElement, editorGroupsContainer: IEditorGroupsContainer): IAuxiliaryTitlebarPart {
		const titlebarPartContainer = document.createElement('div');
		titlebarPartContainer.classList.add('part', 'titlebar');
		titlebarPartContainer.setAttribute('role', 'none');
		titlebarPartContainer.style.position = 'relative';
		container.insertBefore(titlebarPartContainer, container.firstChild); // ensure we are first element

		const disposables = new DisposableStore();

		const titlebarPart = this.doCreateAuxiliaryTitlebarPart(titlebarPartContainer, editorGroupsContainer);
		disposables.add(this.registerPart(titlebarPart));

		disposables.add(Event.runAndSubscribe(titlebarPart.onDidChange, () => titlebarPartContainer.style.height = `${titlebarPart.height}px`));
		titlebarPart.create(titlebarPartContainer);

		Event.once(titlebarPart.onWillDispose)(() => disposables.dispose());

		return titlebarPart;
	}

	protected doCreateAuxiliaryTitlebarPart(container: HTMLElement, editorGroupsContainer: IEditorGroupsContainer): BrowserTitlebarPart & IAuxiliaryTitlebarPart {
		return this.instantiationService.createInstance(AuxiliaryBrowserTitlebarPart, container, editorGroupsContainer);
	}

	//#endregion


	//#region Service Implementation

	readonly onMenubarVisibilityChange = this.mainPart.onMenubarVisibilityChange;

	updateProperties(properties: ITitleProperties): void {
		for (const part of this.parts) {
			part.updateProperties(properties);
		}
	}

	//#endregion
}

class TitlebarPartHoverDelegate implements IHoverDelegate {

	readonly showHover = this.hoverService.showHover.bind(this.hoverService);
	readonly placement = 'element';

	private lastHoverHideTime: number = 0;
	get delay(): number {
		return Date.now() - this.lastHoverHideTime < 200
			? 0  // show instantly when a hover was recently shown
			: this.configurationService.getValue<number>('workbench.hover.delay');
	}

	constructor(
		@IHoverService private readonly hoverService: IHoverService,
		@IConfigurationService private readonly configurationService: IConfigurationService
	) { }

	onDidHideHover() {
		this.lastHoverHideTime = Date.now();
	}
}

export class BrowserTitlebarPart extends Part implements ITitlebarPart {

	declare readonly _serviceBrand: undefined;

	//#region IView

	readonly minimumWidth: number = 0;
	readonly maximumWidth: number = Number.POSITIVE_INFINITY;

	get minimumHeight(): number {
		const value = this.isCommandCenterVisible || (isWeb && isWCOEnabled()) ? 35 : 30;

		return value / (this.useCounterZoom ? getZoomFactor() : 1);
	}

	get maximumHeight(): number { return this.minimumHeight; }

	//#endregion

	//#region Events

	private _onMenubarVisibilityChange = this._register(new Emitter<boolean>());
	readonly onMenubarVisibilityChange = this._onMenubarVisibilityChange.event;

	private readonly _onWillDispose = this._register(new Emitter<void>());
	readonly onWillDispose = this._onWillDispose.event;

	//#endregion

	protected rootContainer!: HTMLElement;
	protected primaryWindowControls: HTMLElement | undefined;
	protected dragRegion: HTMLElement | undefined;
	private title!: HTMLElement;

	private leftContent!: HTMLElement;
	private centerContent!: HTMLElement;
	private rightContent!: HTMLElement;

	protected customMenubar: CustomMenubarControl | undefined;
	protected appIcon: HTMLElement | undefined;
	private appIconBadge: HTMLElement | undefined;
	protected menubar?: HTMLElement;
	private lastLayoutDimensions: Dimension | undefined;

	private actionToolBar!: WorkbenchToolBar;
	private actionToolBarDisposable = this._register(new DisposableStore());
	private editorActionsChangeDisposable = this._register(new DisposableStore());
	private actionToolBarElement!: HTMLElement;

	private layoutToolbarMenu: IMenu | undefined;
	private readonly editorToolbarMenuDisposables = this._register(new DisposableStore());
	private readonly layoutToolbarMenuDisposables = this._register(new DisposableStore());

	private readonly hoverDelegate = new TitlebarPartHoverDelegate(this.hoverService, this.configurationService);

	private readonly titleDisposables = this._register(new DisposableStore());
	private titleBarStyle: 'native' | 'custom' = getTitleBarStyle(this.configurationService);

	private isInactive: boolean = false;
	private readonly isAuxiliary: boolean;

	private readonly windowTitle: WindowTitle;

	private readonly editorService: IEditorService;
	private readonly editorGroupsContainer: IEditorGroupsContainer;

	constructor(
		id: string,
		targetWindow: Window,
		editorGroupsContainer: IEditorGroupsContainer | 'main',
		@IContextMenuService private readonly contextMenuService: IContextMenuService,
		@IConfigurationService protected readonly configurationService: IConfigurationService,
		@IBrowserWorkbenchEnvironmentService protected readonly environmentService: IBrowserWorkbenchEnvironmentService,
		@IInstantiationService protected readonly instantiationService: IInstantiationService,
		@IThemeService themeService: IThemeService,
		@IStorageService storageService: IStorageService,
		@IWorkbenchLayoutService layoutService: IWorkbenchLayoutService,
		@IContextKeyService private readonly contextKeyService: IContextKeyService,
		@IHostService private readonly hostService: IHostService,
		@IHoverService private readonly hoverService: IHoverService,
		@IEditorGroupsService editorGroupService: IEditorGroupsService,
		@IEditorService editorService: IEditorService,
		@IMenuService private readonly menuService: IMenuService,
		@IKeybindingService private readonly keybindingService: IKeybindingService
	) {
		super(id, { hasTitle: false }, themeService, storageService, layoutService);

		this.isAuxiliary = editorGroupsContainer !== 'main';
		this.editorService = editorService.createScoped(editorGroupsContainer, this._store);
		this.editorGroupsContainer = editorGroupsContainer === 'main' ? editorGroupService.mainPart : editorGroupsContainer;

		this.windowTitle = this._register(instantiationService.createInstance(WindowTitle, targetWindow, editorGroupsContainer));

		this.registerListeners(getWindowId(targetWindow));
	}

	private registerListeners(targetWindowId: number): void {
		this._register(this.hostService.onDidChangeFocus(focused => focused ? this.onFocus() : this.onBlur()));
		this._register(this.hostService.onDidChangeActiveWindow(windowId => windowId === targetWindowId ? this.onFocus() : this.onBlur()));
		this._register(this.configurationService.onDidChangeConfiguration(e => this.onConfigurationChanged(e)));
	}

	private onBlur(): void {
		this.isInactive = true;

		this.updateStyles();
	}

	private onFocus(): void {
		this.isInactive = false;

		this.updateStyles();
	}

	protected onConfigurationChanged(event: IConfigurationChangeEvent): void {

		// Custom menu bar (disabled if auxiliary)
		if (!this.isAuxiliary && this.titleBarStyle !== 'native' && (!isMacintosh || isWeb)) {
			if (event.affectsConfiguration('window.menuBarVisibility')) {
				if (this.currentMenubarVisibility === 'compact') {
					this.uninstallMenubar();
				} else {
					this.installMenubar();
				}
			}
		}

		// Actions
		if (this.titleBarStyle !== 'native' && this.actionToolBar) {
			const affectsEditorActions = event.affectsConfiguration('workbench.editor.editorActionsLocation') || event.affectsConfiguration('workbench.editor.showTabs');
			const affectsLayoutControl = event.affectsConfiguration('workbench.layoutControl.enabled');
			const affectsActivityControl = event.affectsConfiguration(LayoutSettings.ACTIVITY_BAR_LOCATION);
			if (affectsEditorActions) {
				this.createActionToolBar();
			}

			if (affectsEditorActions || affectsLayoutControl || affectsActivityControl) {
				this.createActionToolBarMenus({ editorActions: affectsEditorActions, layoutActions: affectsLayoutControl, activityActions: affectsActivityControl });

				this._onDidChange.fire(undefined);
			}
		}

		// Command Center
		if (event.affectsConfiguration(LayoutSettings.COMMAND_CENTER)) {
			this.createTitle();

			this._onDidChange.fire(undefined);
		}
	}

	protected installMenubar(): void {
		if (this.menubar) {
			return; // If the menubar is already installed, skip
		}

		this.customMenubar = this._register(this.instantiationService.createInstance(CustomMenubarControl));

		this.menubar = append(this.leftContent, $('div.menubar'));
		this.menubar.setAttribute('role', 'menubar');

		this._register(this.customMenubar.onVisibilityChange(e => this.onMenubarVisibilityChanged(e)));

		this.customMenubar.create(this.menubar);
	}

	private uninstallMenubar(): void {
		this.customMenubar?.dispose();
		this.customMenubar = undefined;

		this.menubar?.remove();
		this.menubar = undefined;

		this.onMenubarVisibilityChanged(false);
	}

	protected onMenubarVisibilityChanged(visible: boolean): void {
		if (isWeb || isWindows || isLinux) {
			if (this.lastLayoutDimensions) {
				this.layout(this.lastLayoutDimensions.width, this.lastLayoutDimensions.height);
			}

			this._onMenubarVisibilityChange.fire(visible);
		}
	}

	updateProperties(properties: ITitleProperties): void {
		this.windowTitle.updateProperties(properties);
	}

	protected override createContentArea(parent: HTMLElement): HTMLElement {
		this.element = parent;
		this.rootContainer = append(parent, $('.titlebar-container'));

		this.leftContent = append(this.rootContainer, $('.titlebar-left'));
		this.centerContent = append(this.rootContainer, $('.titlebar-center'));
		this.rightContent = append(this.rootContainer, $('.titlebar-right'));

		// App Icon (Native Windows/Linux and Web)
		if (!isMacintosh && !isWeb) {
			this.appIcon = prepend(this.leftContent, $('a.window-appicon'));

			// Web-only home indicator and menu (not for auxiliary windows)
			if (!this.isAuxiliary && isWeb) {
				const homeIndicator = this.environmentService.options?.homeIndicator;
				if (homeIndicator) {
					const icon: ThemeIcon = getIconRegistry().getIcon(homeIndicator.icon) ? { id: homeIndicator.icon } : Codicon.code;

					this.appIcon.setAttribute('href', homeIndicator.href);
					this.appIcon.classList.add(...ThemeIcon.asClassNameArray(icon));
					this.appIconBadge = document.createElement('div');
					this.appIconBadge.classList.add('home-bar-icon-badge');
					this.appIcon.appendChild(this.appIconBadge);
				}
			}
		}

		// Draggable region that we can manipulate for #52522
		this.dragRegion = prepend(this.rootContainer, $('div.titlebar-drag-region'));

		// Menubar: install a custom menu bar depending on configuration
		if (
			!this.isAuxiliary &&
			this.titleBarStyle !== 'native' &&
			(!isMacintosh || isWeb) &&
			this.currentMenubarVisibility !== 'compact'
		) {
			this.installMenubar();
		}

		// Title
		this.title = append(this.centerContent, $('div.window-title'));
		this.createTitle();

		// Create Toolbar Actions
		if (this.titleBarStyle !== 'native') {
			this.actionToolBarElement = append(this.rightContent, $('div.action-toolbar-container'));
			this.createActionToolBar();
			this.createActionToolBarMenus();
		}

		let primaryControlLocation = isMacintosh ? 'left' : 'right';
		if (isMacintosh && isNative) {

			// Check if the locale is RTL, macOS will move traffic lights in RTL locales
			// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/Locale/textInfo

			const localeInfo = new Intl.Locale(platformLocale) as any;
			if (localeInfo?.textInfo?.direction === 'rtl') {
				primaryControlLocation = 'right';
			}
		}

		this.primaryWindowControls = append(primaryControlLocation === 'left' ? this.leftContent : this.rightContent, $('div.window-controls-container.primary'));
		append(primaryControlLocation === 'left' ? this.rightContent : this.leftContent, $('div.window-controls-container.secondary'));

		// Context menu over title bar: depending on the OS and the location of the click this will either be
		// the overall context menu for the entire title bar or a specific title context menu.
		// Windows / Linux: we only support the overall context menu on the title bar
		// macOS: we support both the overall context menu and the title context menu.
		//        in addition, we allow Cmd+click to bring up the title context menu.
		{
			this._register(addDisposableListener(this.rootContainer, EventType.CONTEXT_MENU, e => {
				EventHelper.stop(e);

				let targetMenu: MenuId;
				if (isMacintosh && e.target instanceof HTMLElement && isAncestor(e.target, this.title)) {
					targetMenu = MenuId.TitleBarTitleContext;
				} else {
					targetMenu = MenuId.TitleBarContext;
				}

				this.onContextMenu(e, targetMenu);
			}));

			if (isMacintosh) {
				this._register(addDisposableListener(this.title, EventType.MOUSE_DOWN, e => {
					if (e.metaKey) {
						EventHelper.stop(e, true /* stop bubbling to prevent command center from opening */);

						this.onContextMenu(e, MenuId.TitleBarTitleContext);
					}
				}, true /* capture phase to prevent command center from opening */));
			}
		}

		// Focus action
		const that = this;
		registerAction2(class FocusTitleBar extends Action2 {

			constructor() {
				super({
					id: `workbench.action.focusTitleBar`,
					title: { value: localize('focusTitleBar', "Focus Title Bar"), original: 'Focus Title Bar' },
					category: Categories.View,
					f1: true,
				});
			}

			run(): void {
				if (that.customMenubar) {
					that.customMenubar.toggleFocus();
				} else {
					(that.element.querySelector('[tabindex]:not([tabindex="-1"])') as HTMLElement).focus();
				}
			}
		});

		this.updateStyles();

		return this.element;
	}

	private createTitle(): void {
		this.titleDisposables.clear();

		// Text Title
		if (!this.isCommandCenterVisible) {
			this.title.innerText = this.windowTitle.value;
			this.titleDisposables.add(this.windowTitle.onDidChange(() => {
				this.title.innerText = this.windowTitle.value;
			}));
		}

		// Menu Title
		else {
			const commandCenter = this.instantiationService.createInstance(CommandCenterControl, this.windowTitle, this.hoverDelegate);
			reset(this.title, commandCenter.element);
			this.titleDisposables.add(commandCenter);
		}
	}

	private actionViewItemProvider(action: IAction): IActionViewItem | undefined {

		// --- Activity Actions
		if (!this.isAuxiliary) {
			if (action.id === GLOBAL_ACTIVITY_ID) {
				return this.instantiationService.createInstance(SimpleGlobalActivityActionViewItem, { position: () => HoverPosition.BELOW });
			}
			if (action.id === ACCOUNTS_ACTIVITY_ID) {
				return this.instantiationService.createInstance(SimpleAccountActivityActionViewItem, { position: () => HoverPosition.BELOW });
			}
		}

		// --- Editor Actions
		const activeEditorPane = this.editorGroupsContainer.activeGroup?.activeEditorPane;
		if (activeEditorPane && activeEditorPane instanceof EditorPane) {
			const result = activeEditorPane.getActionViewItem(action);

			if (result) {
				return result;
			}
		}

		// Check extensions
		return createActionViewItem(this.instantiationService, action, { hoverDelegate: this.hoverDelegate, menuAsChild: false });
	}

	private getKeybinding(action: IAction): ResolvedKeybinding | undefined {
		const editorPaneAwareContextKeyService = this.editorGroupsContainer.activeGroup?.activeEditorPane?.scopedContextKeyService ?? this.contextKeyService;

		return this.keybindingService.lookupKeybinding(action.id, editorPaneAwareContextKeyService);
	}

	private createActionToolBar() {

		// Creates the action tool bar. Depends on the configuration of the title bar menus
		// Requires to be recreated whenever editor actions enablement changes

		this.actionToolBarDisposable.clear();

		this.actionToolBar = this.actionToolBarDisposable.add(this.instantiationService.createInstance(WorkbenchToolBar, this.actionToolBarElement, {
			contextMenu: MenuId.TitleBarContext,
			orientation: ActionsOrientation.HORIZONTAL,
			ariaLabel: localize('ariaLabelTitleActions', "Title actions"),
			getKeyBinding: action => this.getKeybinding(action),
			overflowBehavior: { maxItems: 9, exempted: [ACCOUNTS_ACTIVITY_ID, GLOBAL_ACTIVITY_ID, ...EDITOR_CORE_NAVIGATION_COMMANDS] },
			anchorAlignmentProvider: () => AnchorAlignment.RIGHT,
			telemetrySource: 'titlePart',
			highlightToggledItems: this.editorActionsEnabled, // Only show toggled state for editor actions (Layout actions are not shown as toggled)
			actionViewItemProvider: action => this.actionViewItemProvider(action)
		}));

		if (this.editorActionsEnabled) {
			this.actionToolBarDisposable.add(this.editorGroupsContainer.onDidChangeActiveGroup(() => this.createActionToolBarMenus({ editorActions: true })));
		}
	}

	private createActionToolBarMenus(update: true | { editorActions?: boolean; layoutActions?: boolean; activityActions?: boolean } = true) {
		if (update === true) {
			update = { editorActions: true, layoutActions: true, activityActions: true };
		}

		const updateToolBarActions = () => {
			const actions: IToolbarActions = { primary: [], secondary: [] };

			// --- Editor Actions
			if (this.editorActionsEnabled) {
				this.editorActionsChangeDisposable.clear();

				const activeGroup = this.editorGroupsContainer.activeGroup;
				if (activeGroup) {
					const editorActions = activeGroup.createEditorActions(this.editorActionsChangeDisposable);

					actions.primary.push(...editorActions.actions.primary);
					actions.secondary.push(...editorActions.actions.secondary);

					this.editorActionsChangeDisposable.add(editorActions.onDidChange(() => updateToolBarActions()));
				}
			}

			// --- Layout Actions
			if (this.layoutToolbarMenu) {
				createAndFillInActionBarActions(
					this.layoutToolbarMenu,
					{},
					actions,
					() => !this.editorActionsEnabled // Layout Actions in overflow menu when editor actions enabled in title bar
				);
			}

			// --- Activity Actions
			if (this.activityActionsEnabled) {
				actions.primary.push(ACCOUNTS_ACTIVITY_TILE_ACTION);
				actions.primary.push(GLOBAL_ACTIVITY_TITLE_ACTION);
			}

			this.actionToolBar.setActions(prepareActions(actions.primary), prepareActions(actions.secondary));
		};

		// Create/Update the menus which should be in the title tool bar

		if (update.editorActions) {
			this.editorToolbarMenuDisposables.clear();

			// The editor toolbar menu is handled by the editor group so we do not need to manage it here.
			// However, depending on the active editor, we need to update the context and action runner of the toolbar menu.
			if (this.editorActionsEnabled && this.editorService.activeEditor !== undefined) {
				const context: IEditorCommandsContext = { groupId: this.editorGroupsContainer.activeGroup.id };

				this.actionToolBar.actionRunner = new EditorCommandsContextActionRunner(context);
				this.actionToolBar.context = context;
				this.editorToolbarMenuDisposables.add(this.actionToolBar.actionRunner);
			} else {
				this.actionToolBar.actionRunner = new ActionRunner();
				this.actionToolBar.context = {};

				this.editorToolbarMenuDisposables.add(this.actionToolBar.actionRunner);
			}
		}

		if (update.layoutActions) {
			this.layoutToolbarMenuDisposables.clear();

			if (this.layoutControlEnabled) {
				this.layoutToolbarMenu = this.menuService.createMenu(MenuId.LayoutControlMenu, this.contextKeyService);

				this.layoutToolbarMenuDisposables.add(this.layoutToolbarMenu);
				this.layoutToolbarMenuDisposables.add(this.layoutToolbarMenu.onDidChange(() => updateToolBarActions()));
			} else {
				this.layoutToolbarMenu = undefined;
			}
		}

		updateToolBarActions();
	}

	override updateStyles(): void {
		super.updateStyles();

		// Part container
		if (this.element) {
			if (this.isInactive) {
				this.element.classList.add('inactive');
			} else {
				this.element.classList.remove('inactive');
			}

			const titleBackground = this.getColor(this.isInactive ? TITLE_BAR_INACTIVE_BACKGROUND : TITLE_BAR_ACTIVE_BACKGROUND, (color, theme) => {
				// LCD Rendering Support: the title bar part is a defining its own GPU layer.
				// To benefit from LCD font rendering, we must ensure that we always set an
				// opaque background color. As such, we compute an opaque color given we know
				// the background color is the workbench background.
				return color.isOpaque() ? color : color.makeOpaque(WORKBENCH_BACKGROUND(theme));
			}) || '';
			this.element.style.backgroundColor = titleBackground;

			if (this.appIconBadge) {
				this.appIconBadge.style.backgroundColor = titleBackground;
			}

			if (titleBackground && Color.fromHex(titleBackground).isLighter()) {
				this.element.classList.add('light');
			} else {
				this.element.classList.remove('light');
			}

			const titleForeground = this.getColor(this.isInactive ? TITLE_BAR_INACTIVE_FOREGROUND : TITLE_BAR_ACTIVE_FOREGROUND);
			this.element.style.color = titleForeground || '';

			const titleBorder = this.getColor(TITLE_BAR_BORDER);
			this.element.style.borderBottom = titleBorder ? `1px solid ${titleBorder}` : '';
		}
	}

	protected onContextMenu(e: MouseEvent, menuId: MenuId): void {
		const event = new StandardMouseEvent(getWindow(this.rootContainer), e);

		// Show it
		this.contextMenuService.showContextMenu({
			getAnchor: () => event,
			menuId,
			contextKeyService: this.contextKeyService,
			domForShadowRoot: isMacintosh && isNative ? event.target : undefined
		});
	}

	protected get currentMenubarVisibility(): MenuBarVisibility {
		if (this.isAuxiliary) {
			return 'hidden';
		}

		return getMenuBarVisibility(this.configurationService);
	}

	private get layoutControlEnabled(): boolean {
		return !this.isAuxiliary && this.configurationService.getValue<boolean>('workbench.layoutControl.enabled') !== false;
	}

	protected get isCommandCenterVisible() {
		return this.configurationService.getValue<boolean>(LayoutSettings.COMMAND_CENTER) !== false;
	}

	private get editorActionsEnabled(): boolean {
		return this.editorGroupsContainer.partOptions.editorActionsLocation === 'titleBar' ||
			(
				this.editorGroupsContainer.partOptions.editorActionsLocation === 'default' &&
				this.editorGroupsContainer.partOptions.showTabs === 'none'
			);
	}

	private get activityActionsEnabled(): boolean {
		return !this.isAuxiliary && this.configurationService.getValue(LayoutSettings.ACTIVITY_BAR_LOCATION) === ActivityBarPosition.TOP;
	}

	protected get useCounterZoom(): boolean {

		// Prevent zooming behavior if any of the following conditions are met:
		// 1. Shrinking below the window control size (zoom < 1)
		// 2. No custom items are present in the title bar

		const zoomFactor = getZoomFactor();

		const noMenubar = this.currentMenubarVisibility === 'hidden' || (!isWeb && isMacintosh);
		const noCommandCenter = !this.isCommandCenterVisible;
		const noToolBarActions = !this.layoutControlEnabled && !this.editorActionsEnabled && !this.activityActionsEnabled;

		return zoomFactor < 1 || (noMenubar && noCommandCenter && noToolBarActions);
	}

	override layout(width: number, height: number): void {
		this.updateLayout(new Dimension(width, height));

		super.layoutContents(width, height);
	}

	private updateLayout(dimension: Dimension): void {
		this.lastLayoutDimensions = dimension;

		if (getTitleBarStyle(this.configurationService) === 'custom') {
			const zoomFactor = getZoomFactor();

			this.element.style.setProperty('--zoom-factor', zoomFactor.toString());
			this.rootContainer.classList.toggle('counter-zoom', this.useCounterZoom);

			if (this.customMenubar) {
				const menubarDimension = new Dimension(0, dimension.height);
				this.customMenubar.layout(menubarDimension);
			}
		}
	}

	toJSON(): object {
		return {
			type: Parts.TITLEBAR_PART
		};
	}

	override dispose(): void {
		this._onWillDispose.fire();

		super.dispose();
	}
}

export class MainBrowserTitlebarPart extends BrowserTitlebarPart {

	constructor(
		@IContextMenuService contextMenuService: IContextMenuService,
		@IConfigurationService configurationService: IConfigurationService,
		@IBrowserWorkbenchEnvironmentService environmentService: IBrowserWorkbenchEnvironmentService,
		@IInstantiationService instantiationService: IInstantiationService,
		@IThemeService themeService: IThemeService,
		@IStorageService storageService: IStorageService,
		@IWorkbenchLayoutService layoutService: IWorkbenchLayoutService,
		@IContextKeyService contextKeyService: IContextKeyService,
		@IHostService hostService: IHostService,
		@IHoverService hoverService: IHoverService,
		@IEditorGroupsService editorGroupService: IEditorGroupsService,
		@IEditorService editorService: IEditorService,
		@IMenuService menuService: IMenuService,
		@IKeybindingService keybindingService: IKeybindingService,
	) {
		super(Parts.TITLEBAR_PART, mainWindow, 'main', contextMenuService, configurationService, environmentService, instantiationService, themeService, storageService, layoutService, contextKeyService, hostService, hoverService, editorGroupService, editorService, menuService, keybindingService);
	}
}

export interface IAuxiliaryTitlebarPart extends ITitlebarPart, IView {
	readonly container: HTMLElement;
	readonly height: number;
}

export class AuxiliaryBrowserTitlebarPart extends BrowserTitlebarPart implements IAuxiliaryTitlebarPart {

	private static COUNTER = 1;

	get height() { return this.minimumHeight; }

	constructor(
		readonly container: HTMLElement,
		editorGroupsContainer: IEditorGroupsContainer,
		@IContextMenuService contextMenuService: IContextMenuService,
		@IConfigurationService configurationService: IConfigurationService,
		@IBrowserWorkbenchEnvironmentService environmentService: IBrowserWorkbenchEnvironmentService,
		@IInstantiationService instantiationService: IInstantiationService,
		@IThemeService themeService: IThemeService,
		@IStorageService storageService: IStorageService,
		@IWorkbenchLayoutService layoutService: IWorkbenchLayoutService,
		@IContextKeyService contextKeyService: IContextKeyService,
		@IHostService hostService: IHostService,
		@IHoverService hoverService: IHoverService,
		@IEditorGroupsService editorGroupService: IEditorGroupsService,
		@IEditorService editorService: IEditorService,
		@IMenuService menuService: IMenuService,
		@IKeybindingService keybindingService: IKeybindingService,
	) {
		const id = AuxiliaryBrowserTitlebarPart.COUNTER++;
		super(`workbench.parts.auxiliaryTitle.${id}`, getWindow(container), editorGroupsContainer, contextMenuService, configurationService, environmentService, instantiationService, themeService, storageService, layoutService, contextKeyService, hostService, hoverService, editorGroupService, editorService, menuService, keybindingService);
	}
}
