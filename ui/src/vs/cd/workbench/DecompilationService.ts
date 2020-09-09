import { RpcDecompilerClient } from './proto/MainServiceClientPb';
import {
	GetAllTypeFilePathsRequest,
	DecompileTypeRequest,
	GetAssemblyRelatedFilePathsRequest,
	GetMemberDefinitionRequest,
	GetMemberDefinitionPositionRequest
} from './proto/main_pb';
import { IGrpcService } from 'vs/cd/workbench/GrpcService';
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';

export const IDecompilationService = createDecorator<IDecompilationService>('IDecompilationService');

export interface IDecompilationService {
	readonly _serviceBrand: undefined;

	getAssemblyRelatedFilePaths(assemblyPath: string) : Promise<AssemblyRelatedFilePaths>;
	getAllTypeFilePaths(assemblyPath: string) : Promise<TypeFilePath[]>;
	decompileType(assemblyPath: string, typeFullName: string) : Promise<string>;
	getMemberDefinition(absoluteFilePath: string, rowNumber: number, columnIndex: number) : Promise<MemberNavigationData>;
	getMemberDefinitionPosition(absoluteFilePath: string, memberFullName: string) : Promise<Selection>;
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

	getAllTypeFilePaths(assemblyPath: string) : Promise<TypeFilePath[]> {
		const request = new GetAllTypeFilePathsRequest();
		request.setAssemblypath(assemblyPath);

		return new Promise<TypeFilePath[]>((resolve, reject) => {
			this.client?.getAllTypeFilePaths(request, null, (err, response) => {
				if (err) {
					reject(`getAllTypeFilePaths failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve(response.getTypefilepathsList().map(tfp => {
					const typeFilePath: TypeFilePath = {
						typeFullName: tfp.getTypefullname(),
						absoluteFilePath: tfp.getAbsolutefilepath()
					};

					return typeFilePath;
				}));
			});
		});
	};

	decompileType(assemblyPath: string, typeFullName: string) : Promise<string> {
		const request = new DecompileTypeRequest();
		request.setAssemblypath(assemblyPath);
		request.setTypefullname(typeFullName);

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

	getMemberDefinition(absoluteFilePath: string, rowNumber: number, columnIndex: number) : Promise<MemberNavigationData> {
		const request = new GetMemberDefinitionRequest();
		request.setAbsolutefilepath(absoluteFilePath);
		request.setLinenumber(rowNumber);
		request.setColumnindex(columnIndex);

		return new Promise<MemberNavigationData>((resolve, reject) => {
			this.client?.getMemberDefinition(request, null, (err, response) => {
				if (err) {
					reject(`getMemberDefinition failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const memberDefinitionData: MemberNavigationData = {
					navigationFilePath: response.getNavigationfilepath(),
					memberFullName: response.getMemberfullname()
				};

				resolve(memberDefinitionData);
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
					startColumnIndex: response.getStartcolumnindex(),
					endColumnIndex: response.getEndcolumnindex()
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
	startColumnIndex: number;
	endColumnIndex: number;
}

export interface MemberNavigationData {
	navigationFilePath: string;
	memberFullName: string;
}
