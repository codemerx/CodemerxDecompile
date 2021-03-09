//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

import { Event } from 'vs/base/common/event';
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { IMainProcessService } from 'vs/platform/ipc/electron-sandbox/mainProcessService';
import { createChannelSender } from 'vs/base/parts/ipc/common/ipc';
import { AssemblyMetadata, AssemblyRelatedFilePaths, ReferenceMetadata, Selection, ProjectCreationMetadata, CreateProjectResult } from 'vs/cd/common/DecompilationTypes';

export const IDecompilationService = createDecorator<IDecompilationService>('IDecompilationService');

export interface IDecompilationService {
	readonly _serviceBrand: undefined;

	readonly onDecompilationContextRestored: Event<void>;

	restoreDecompilationContext() : Promise<void>;
	shouldDecompileFile(filePath: string) : Promise<boolean>;
	getContextAssembly(contextUri: string) : Promise<AssemblyMetadata>;
	getWorkspaceDirectory() : Promise<string>
	getAssemblyRelatedFilePaths(assemblyPath: string): Promise<AssemblyRelatedFilePaths>;
	getProjectCreationMetadata(assemblyFilePath: string, projectVisualStudioVersion?: string): Promise<ProjectCreationMetadata>;
	getAllTypeFilePaths(assemblyPath: string): Promise<string[]>;
	decompileType(filePath: string): Promise<string>;
	getMemberReferenceMetadata(absoluteFilePath: string, lineNumber: number, column: number): Promise<ReferenceMetadata>;
	getMemberDefinitionPosition(absoluteFilePath: string, memberName: string, declaringTypeName?: string): Promise<Selection>;
	addResolvedAssembly(filePath: string): Promise<void>;
	createProject(assemblyFilePath: string, outputPath: string, decompileDangerousResources: boolean, projectVisualStudioVersion?: string): Promise<CreateProjectResult>;
	getLegacyVisualStudioVersions() : Promise<string[]>;
	getSearchResultPosition(searchResultId: number) : Promise<Selection>;
	clearAssemblyList() : Promise<void>;
}

export class DecompilationService {
	readonly _serviceBrand: undefined;

	constructor(@IMainProcessService mainProcessService: IMainProcessService) {
		return createChannelSender<IDecompilationService>(mainProcessService.getChannel('decompilationService'));
	}
}
