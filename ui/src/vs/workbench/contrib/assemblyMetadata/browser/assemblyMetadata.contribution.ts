/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { localize } from "vs/nls";
import { SyncDescriptor } from "vs/platform/instantiation/common/descriptors";
import { Registry } from "vs/platform/registry/common/platform";
import { IViewDescriptor, IViewsRegistry, Extensions as ViewExtensions } from "vs/workbench/common/views";
import { AssemblyMetadataPane } from "./assemblyMetadataPane";
import { VIEW_CONTAINER } from 'vs/workbench/contrib/files/browser/explorerViewlet';
export const PANEL_ID = 'panel.view.assemblyMetadata';

/* AGPL */
const _outlineDesc = <IViewDescriptor>{
	id: 'assemblyMetadata',
	name: localize('name', "Assembly Metadata"),
	containerIcon: 'codicon-symbol-class',
	ctorDescriptor: new SyncDescriptor(AssemblyMetadataPane),
	canToggleVisibility: true,
	canMoveView: true,
	hideByDefault: false,
	collapsed: false,
	order: 2,
	weight: 30,
	focusCommand: { id: 'assemblyMetadata.focus' }
};

Registry.as<IViewsRegistry>(ViewExtensions.ViewsRegistry).registerViews([_outlineDesc], VIEW_CONTAINER);
/* End AGPL */
