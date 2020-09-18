/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { SyncDescriptor } from 'vs/platform/instantiation/common/descriptors';
import { registerSingleton } from 'vs/platform/instantiation/common/extensions';
/* AGPL */
import { IViewDescriptor } from 'vs/workbench/common/views';
/* End AGPL */
import { ITimelineService, TimelinePaneId } from 'vs/workbench/contrib/timeline/common/timeline';
import { TimelineService } from 'vs/workbench/contrib/timeline/common/timelineService';
import { TimelinePane } from './timelinePane';

export class TimelinePaneDescriptor implements IViewDescriptor {
	readonly id = TimelinePaneId;
	readonly name = TimelinePane.TITLE;
	readonly containerIcon = 'codicon-history';
	readonly ctorDescriptor = new SyncDescriptor(TimelinePane);
	readonly order = 2;
	readonly weight = 30;
	readonly collapsed = true;
	readonly canToggleVisibility = true;
	readonly hideByDefault = false;
	readonly canMoveView = true;

	focusCommand = { id: 'timeline.focus' };
}

// Configuration
/* AGPL */
// const configurationRegistry = Registry.as<IConfigurationRegistry>(ConfigurationExtensions.Configuration);
// configurationRegistry.registerConfiguration({
// 	id: 'timeline',
// 	order: 1001,
// 	title: localize('timelineConfigurationTitle', "Timeline"),
// 	type: 'object',
// 	properties: {
// 		'timeline.excludeSources': {
// 			type: [
// 				'array',
// 				'null'
// 			],
// 			default: null,
// 			description: localize('timeline.excludeSources', "An array of Timeline sources that should be excluded from the Timeline view"),
// 		},
// 		'timeline.pageSize': {
// 			type: ['number', 'null'],
// 			default: null,
// 			markdownDescription: localize('timeline.pageSize', "The number of items to show in the Timeline view by default and when loading more items. Setting to `null` (the default) will automatically choose a page size based on the visible area of the Timeline view"),
// 		},
// 		'timeline.pageOnScroll': {
// 			type: 'boolean',
// 			default: false,
// 			description: localize('timeline.pageOnScroll', "Experimental. Controls whether the Timeline view will load the next page of items when you scroll to the end of the list"),
// 		},
// 	}
// });

// Registry.as<IViewsRegistry>(ViewExtensions.ViewsRegistry).registerViews([new TimelinePaneDescriptor()], VIEW_CONTAINER);

// namespace OpenTimelineAction {

// 	export const ID = 'files.openTimeline';
// 	export const LABEL = localize('files.openTimeline', "Open Timeline");

// 	export function handler(): ICommandHandler {
// 		return (accessor, arg) => {
// 			const service = accessor.get(ITimelineService);
// 			return service.setUri(arg);
// 		};
// 	}
// }

// CommandsRegistry.registerCommand(OpenTimelineAction.ID, OpenTimelineAction.handler());

// MenuRegistry.appendMenuItem(MenuId.ExplorerContext, ({
// 	group: '4_timeline',
// 	order: 1,
// 	command: {
// 		id: OpenTimelineAction.ID,
// 		title: OpenTimelineAction.LABEL,
// 		icon: { id: 'codicon/history' }
// 	},
// 	when: ContextKeyExpr.and(ExplorerFolderContext.toNegated(), ResourceContextKey.HasResource)
// }));
/* End AGPL */

registerSingleton(ITimelineService, TimelineService, true);
