import { RpcDecompilerClient } from './proto/MainServiceClientPb';
import {
	GetAllTypeFilePathsRequest,
	DecompileTypeRequest,
	GetAssemblyMetadataRequest,
	GetMemberDefinitionRequest,
	GetMemberDefinitionPositionRequest
} from './proto/main_pb';
import { IGrpcService } from 'vs/cd/workbench/GrpcService';
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';

export const IDecompilationService = createDecorator<IDecompilationService>('IDecompilationService');

export interface IDecompilationService {
	readonly _serviceBrand: undefined;

	getAssemblyMetadata(assembllyPath: string) : Promise<AssemblyMetadata>;
	getAllTypeFilePaths(assemblyPath: string, targetPath: string) : Promise<TypeFilePath[]>;
	decompileType(assemblyPath: string, typeFullName: string) : Promise<string>;
	getMemberDefinition(relativeFilePath: string, rowNumber: number, columnIndex: number) : Promise<MemberNavigationData>;
	getMemberDefinitionPosition(memberFullName: string, filePath: string) : Promise<Selection>;
}

export class DecompilationService implements IDecompilationService {
	readonly _serviceBrand: undefined;

	private client: RpcDecompilerClient | undefined;

	constructor(@IGrpcService grpcService: IGrpcService) {
		grpcService.getServiceUrl().then(url => {
			this.client = new RpcDecompilerClient(url);
		});
	}

	getAssemblyMetadata(assembllyPath: string) : Promise<AssemblyMetadata> {
		const request = new GetAssemblyMetadataRequest();
		request.setAssemblypath(assembllyPath);

		return new Promise<AssemblyMetadata>((resolve, reject) => {
			this.client?.getAssemblyMetadata(request, null, (err, response) => {
				if (err) {
					reject(`getAssemblyMetadata failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve({
					strongName: response.getStrongname(),
					mainModuleName: response.getMainmodulename()
				});
			});
		});
	}

	getAllTypeFilePaths(assemblyPath: string, targetPath: string) : Promise<TypeFilePath[]> {
		const request = new GetAllTypeFilePathsRequest();
		request.setAssemblypath(assemblyPath);
		request.setTargetpath(targetPath);

		return new Promise<TypeFilePath[]>((resolve, reject) => {
			this.client?.getAllTypeFilePaths(request, null, (err, response) => {
				if (err) {
					reject(`getAllTypeFilePaths failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve(response.getTypefilepathsList().map(tfp => {
					const typeFilePath: TypeFilePath = {
						typeFullName: tfp.getTypefullname(),
						relativeFilePath: tfp.getRelativefilepath()
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

	getMemberDefinition(relativeFilePath: string, rowNumber: number, columnIndex: number) : Promise<MemberNavigationData> {
		const request = new GetMemberDefinitionRequest();
		request.setFilepath(relativeFilePath);
		request.setLinenumber(rowNumber);
		request.setColumnindex(columnIndex);

		return new Promise<MemberNavigationData>((resolve, reject) => {
			this.client?.getMemberDefinition(request, null, (err, response) => {
				if (err) {
					reject(`getMemberDefinition failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const memberDefinitionData: MemberNavigationData = {
					filePath: response.getFilepath(),
					memberFullName: response.getMemberfullname()
				};

				resolve(memberDefinitionData);
			});
		})
	}

	getMemberDefinitionPosition(memberFullName: string, filePath: string) : Promise<Selection> {
		const request = new GetMemberDefinitionPositionRequest();
		request.setMemberfullname(memberFullName);
		request.setFilepath(filePath);

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

export interface AssemblyMetadata {
	strongName: string;
	mainModuleName: string;
}

export interface TypeFilePath {
	typeFullName: string;
	relativeFilePath: string;
}

export interface Selection {
	startLineNumber: number;
	endLineNumber: number;
	startColumnIndex: number;
	endColumnIndex: number;
}

export interface MemberNavigationData {
	filePath: string;
	memberFullName: string;
}
