import { RpcDecompilerClient } from './proto/MainServiceClientPb';
import {
	GetAllTypeFilePathsRequest,
	DecompileTypeRequest,
	GetAssemblyMetadataRequest
} from './proto/main_pb';
import { IGrpcService } from 'vs/cd/workbench/GrpcService';
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';

export const IDecompilationService = createDecorator<IDecompilationService>('IDecompilationService');

export interface IDecompilationService {
	readonly _serviceBrand: undefined;

	getAssemblyMetadata(assembllyPath: string) : Promise<AssemblyMetadata>;
	getAllTypeFilePaths(assemblyPath: string, targetPath: string) : Promise<TypeFilePath[]>;
	decompileType(assemblyPath: string, typeFullName: string) : Promise<string>;
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
					reject(`getAssemblyMetadata failed. Error: ${err}`);
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
					reject(`getAllTypeFilePaths failed. Error: ${err}`);
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
}

export interface AssemblyMetadata {
	strongName: string;
	mainModuleName: string;
}

export interface TypeFilePath {
	typeFullName: string;
	relativeFilePath: string;
}
