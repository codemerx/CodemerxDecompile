import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { IDecompilationService } from 'vs/cd/workbench/DecompilationService';
import { URI, UriComponents } from 'vs/base/common/uri';
import { IFileService } from 'vs/platform/files/common/files';
import { VSBuffer } from 'vs/base/common/buffer';

export const IDecompilationHelper = createDecorator<IDecompilationHelper>('IDecompilationHelper');

export interface IDecompilationHelper {
	readonly _serviceBrand: undefined;

	createAssemblyFileHierarchy(assemblyPath: URI | UriComponents) : Promise<{ hierarchyDirectory: string } | null>;
}

export class DecompilationHelper implements IDecompilationHelper {
	readonly _serviceBrand: undefined;

	constructor(@IFileService private readonly fileService: IFileService,
				@IDecompilationService private readonly decompilationService: IDecompilationService) {}

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
					const content = VSBuffer.fromString('CodemerxDecompile');
					await this.fileService.createFile(URI.file(typeFilePath), content);
				}

				return {
					hierarchyDirectory: assemblyRelatedFilePaths.decompiledAssemblyDirectory
				}
			}

		} catch (err) { }

		return null;
	}
}
