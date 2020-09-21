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

import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { IDecompilationService } from 'vs/cd/workbench/DecompilationService';
import { URI, UriComponents } from 'vs/base/common/uri';
import { IFileService } from 'vs/platform/files/common/files';
import { VSBuffer } from 'vs/base/common/buffer';
import { IProgressService, ProgressLocation } from 'vs/platform/progress/common/progress';

const CODEMERX_FILE_IDENTIFICATOR = 'CodemerxDecompile';

export const IDecompilationHelper = createDecorator<IDecompilationHelper>('IDecompilationHelper');

export interface IDecompilationHelper {
	readonly _serviceBrand: undefined;

	createAssemblyFileHierarchy(assemblyPath: URI | UriComponents) : Promise<{ hierarchyDirectory: string } | null>;
	ensureTypeIsDecompiled(typeUri: URI) : Promise<void>
}

export class DecompilationHelper implements IDecompilationHelper {
	readonly _serviceBrand: undefined;

	constructor(@IFileService private readonly fileService: IFileService,
				@IDecompilationService private readonly decompilationService: IDecompilationService,
				@IProgressService private readonly progressService: IProgressService) {}

	async createAssemblyFileHierarchy(assemblyPath: URI | UriComponents) : Promise<{ hierarchyDirectory: string } | null> {
		try {
			const assemblyUri = URI.revive(assemblyPath);

			if (await this.fileService.exists(assemblyUri)) {
				const assemblyRelatedFilePaths = await this.decompilationService.getAssemblyRelatedFilePaths(assemblyUri.fsPath);
				const typeFilePaths = await this.decompilationService.getAllTypeFilePaths(assemblyUri.fsPath);
				if (assemblyRelatedFilePaths &&
					assemblyRelatedFilePaths.decompiledAssemblyPath &&
					await this.fileService.exists(URI.file(assemblyRelatedFilePaths.decompiledAssemblyPath))) {
					await this.fileService.del(URI.file(assemblyRelatedFilePaths.decompiledAssemblyPath), {
						useTrash: false,
						recursive: true
					});
				}

				for (const typeFilePath of typeFilePaths) {
					const content = VSBuffer.fromString(CODEMERX_FILE_IDENTIFICATOR);
					await this.fileService.createFile(URI.file(typeFilePath), content);
				}

				return {
					hierarchyDirectory: assemblyRelatedFilePaths.decompiledAssemblyDirectory
				}
			}

		} catch (err) { }

		return null;
	}

	async ensureTypeIsDecompiled(typeUri: URI) : Promise<void> {
		const fileContent = await this.fileService.readFile(typeUri);
		const str = fileContent.value.toString();

		if (str === CODEMERX_FILE_IDENTIFICATOR) {
			await this.progressService.withProgress({ location: ProgressLocation.Dialog, nonClosable: true }, async progress => {
				progress.report({ message: 'Loading type...'});

				const sourceCode = await this.decompilationService.decompileType(typeUri.fsPath);
				await this.fileService.writeFile(typeUri, VSBuffer.fromString(sourceCode));
			});
		}
	}
}
