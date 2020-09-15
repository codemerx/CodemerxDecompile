import { RpcDecompilerClient } from './proto/MainServiceClientPb';
import {
	GetAllTypeFilePathsRequest,
	DecompileTypeRequest,
	GetAssemblyRelatedFilePathsRequest,
	GetMemberReferenceMetadataRequest,
	GetMemberDefinitionPositionRequest,
	AddResolvedAssemblyRequest
} from './proto/main_pb';
import { IGrpcService } from 'vs/cd/workbench/GrpcService';
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';

export const IDecompilationService = createDecorator<IDecompilationService>('IDecompilationService');

export interface IDecompilationService {
	readonly _serviceBrand: undefined;

	getAssemblyRelatedFilePaths(assemblyPath: string) : Promise<AssemblyRelatedFilePaths>;
	getAllTypeFilePaths(assemblyPath: string) : Promise<string[]>;
	decompileType(filePath: string) : Promise<string>;
	getMemberReferenceMetadata(absoluteFilePath: string, lineNumber: number, column: number) : Promise<ReferenceMetadata>;
	getMemberDefinitionPosition(absoluteFilePath: string, memberFullName: string) : Promise<Selection>;
	addResolvedAssembly(filePath: string) : Promise<void>;
}

export class DecompilationService implements IDecompilationService {
	readonly _serviceBrand: undefined;

	private client: RpcDecompilerClient | undefined;

	constructor(@IGrpcService grpcService: IGrpcService) {
		grpcService.getServiceUrl().then(url => {
			this.client = new RpcDecompilerClient(url);
		});
	}

	getAssemblyRelatedFilePaths(assemblyPath: string) : Promise<AssemblyRelatedFilePaths> {
		const request = new GetAssemblyRelatedFilePathsRequest();
		request.setAssemblypath(assemblyPath);

		return new Promise<AssemblyRelatedFilePaths>((resolve, reject) => {
			this.client?.getAssemblyRelatedFilePaths(request, null, (err, response) => {
				if (err) {
					reject(`getAssemblyRelatedFilePaths failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const assemblyRelatedFilePaths: AssemblyRelatedFilePaths = {
					decompiledAssemblyDirectory: response.getDecompiledassemblydirectory(),
					decompiledAssemblyPath: response.getDecompiledassemblypath()
				};

				resolve(assemblyRelatedFilePaths);
			});
		});
	}

	getAllTypeFilePaths(assemblyPath: string) : Promise<string[]> {
		const request = new GetAllTypeFilePathsRequest();
		request.setAssemblypath(assemblyPath);

		return new Promise<string[]>((resolve, reject) => {
			this.client?.getAllTypeFilePaths(request, null, (err, response) => {
				if (err) {
					reject(`getAllTypeFilePaths failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve(response.getTypefilepathsList());
			});
		});
	};

	decompileType(filePath: string) : Promise<string> {
		const request = new DecompileTypeRequest();
		request.setFilepath(filePath);

		return new Promise<string>((resolve, reject) => {
			this.client?.decompileType(request, null, (err, response) => {
				if (err) {
					reject(err);
					return;
				}

				resolve(response.getSourcecode());
			});
		});
	}

	addResolvedAssembly(filePath: string) : Promise<void> {
		const request = new AddResolvedAssemblyRequest();
		request.setFilepath(filePath);

		return new Promise<void>((resolve, reject) => {
			this.client?.addResolvedAssembly(request, null, (err, response) => {
				if (err) {
					reject(`addResolvedAssembly failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve();
			});
		})
	}

	getMemberReferenceMetadata(absoluteFilePath: string, lineNumber: number, column: number) : Promise<ReferenceMetadata> {
		const request = new GetMemberReferenceMetadataRequest();
		request.setAbsolutefilepath(absoluteFilePath);
		request.setLinenumber(lineNumber);
		request.setColumn(column);

		return new Promise<ReferenceMetadata>((resolve, reject) => {
			this.client?.getMemberReferenceMetadata(request, null, (err, response) => {
				if (err) {
					reject(err);
					return;
				}

				const memberReferenceMetadata: ReferenceMetadata = {
					memberFullName: response.getMemberfullname(),
					definitionFilePath: response.getDefinitionfilepath(),
					isCrossAssemblyReference: response.getIscrossassemblyreference(),
					referencedAssemblyFullName: response.getReferencedassemblyfullname(),
					referencedAssemblyFilePath: response.getReferencedassemblyfilepath()
				};

				resolve(memberReferenceMetadata);
			});
		})
	}

	getMemberDefinitionPosition(absoluteFilePath: string, memberFullName: string) : Promise<Selection> {
		const request = new GetMemberDefinitionPositionRequest();
		request.setMemberfullname(memberFullName);
		request.setAbsolutefilepath(absoluteFilePath);

		return new Promise<Selection>((resolve, reject) => {
			this.client?.getMemberDefinitionPosition(request, null, (err, response) => {
				if (err) {
					reject(`getMemberDefinitionPosition failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const selection: Selection = {
					startLineNumber: response.getStartlinenumber(),
					endLineNumber: response.getEndlinenumber(),
					startColumn: response.getStartcolumn(),
					endColumn: response.getEndcolumn()
				};

				resolve(selection);
			});
		})
	}
}

export interface AssemblyRelatedFilePaths {
	decompiledAssemblyDirectory: string;
	decompiledAssemblyPath: string;
}

export interface TypeFilePath {
	typeFullName: string;
	absoluteFilePath: string;
}

export interface Selection {
	startLineNumber: number;
	endLineNumber: number;
	startColumn: number;
	endColumn: number;
}

export interface ReferenceMetadata {
	memberFullName: string;
	definitionFilePath?: string;
	isCrossAssemblyReference: boolean;
    referencedAssemblyFullName?: string;
    referencedAssemblyFilePath?: string;
}
