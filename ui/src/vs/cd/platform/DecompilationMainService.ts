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

import { RpcDecompilerClient } from './proto/main_grpc_pb';
import {
	GetAllTypeFilePathsRequest,
	DecompileTypeRequest,
	GetAssemblyRelatedFilePathsRequest,
	GetMemberReferenceMetadataRequest,
	GetMemberDefinitionPositionRequest,
	AddResolvedAssemblyRequest,
	CreateProjectRequest,
	GetProjectCreationMetadataFromTypeFilePathRequest
} from './proto/main_pb';
import * as grpc from "@grpc/grpc-js";
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { IGrpcService } from 'vs/cd/platform/GrpcService';
import { AssemblyRelatedFilePaths, ReferenceMetadata, Selection, ProjectCreationMetadata, CreateProjectResult } from 'vs/cd/common/DecompilationTypes';

export const IDecompilationMainService = createDecorator<IDecompilationMainService>('IDecompilationMainService');

export interface IDecompilationMainService {
	readonly _serviceBrand: undefined;

	getAssemblyRelatedFilePaths(assemblyPath: string) : Promise<AssemblyRelatedFilePaths>;
	getProjectCreationMetadataFromTypeFilePath(typeFilePath: string, projectVisualStudioVersion?: number): Promise<ProjectCreationMetadata>;
	getAllTypeFilePaths(assemblyPath: string) : Promise<string[]>;
	decompileType(filePath: string) : Promise<string>;
	getMemberReferenceMetadata(absoluteFilePath: string, lineNumber: number, column: number) : Promise<ReferenceMetadata>;
	getMemberDefinitionPosition(absoluteFilePath: string, memberFullName: string) : Promise<Selection>;
	addResolvedAssembly(filePath: string) : Promise<void>;
	createProject(assemblyFilePath: string, outputPath: string, decompileDangerousResources: boolean, projectVisualStudioVersion?: number): Promise<CreateProjectResult>;
	getLegacyVisualStudioVersions() : string[];
}

export class DecompilationMainService implements IDecompilationMainService {
	readonly _serviceBrand: undefined;

	private client: RpcDecompilerClient | undefined;

	constructor(@IGrpcService grpcService: IGrpcService) {
		grpcService.getServiceUrl().then(url => {
			this.client = new RpcDecompilerClient(url, grpc.credentials.createInsecure());
		});
	}

	getAssemblyRelatedFilePaths(assemblyPath: string) : Promise<AssemblyRelatedFilePaths> {
		const request = new GetAssemblyRelatedFilePathsRequest();
		request.setAssemblypath(assemblyPath);

		return new Promise<AssemblyRelatedFilePaths>((resolve, reject) => {
			this.client?.getAssemblyRelatedFilePaths(request, {}, (err, response) => {
				if (err) {
					reject(`getAssemblyRelatedFilePaths failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const assemblyRelatedFilePaths: AssemblyRelatedFilePaths = {
					decompiledAssemblyDirectory: response!.getDecompiledassemblydirectory(),
					decompiledAssemblyPath: response!.getDecompiledassemblypath()
				};

				resolve(assemblyRelatedFilePaths);
			});
		});
	}

	getAllTypeFilePaths(assemblyPath: string) : Promise<string[]> {
		const request = new GetAllTypeFilePathsRequest();
		request.setAssemblypath(assemblyPath);

		return new Promise<string[]>((resolve, reject) => {
			this.client?.getAllTypeFilePaths(request, {}, (err, response) => {
				if (err) {
					reject(`getAllTypeFilePaths failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve(response!.getTypefilepathsList());
			});
		});
	};

	decompileType(filePath: string) : Promise<string> {
		const request = new DecompileTypeRequest();
		request.setFilepath(filePath);

		return new Promise<string>((resolve, reject) => {
			this.client?.decompileType(request, {}, (err, response) => {
				if (err) {
					reject(err);
					return;
				}

				resolve(response!.getSourcecode());
			});
		});
	}

	addResolvedAssembly(filePath: string) : Promise<void> {
		const request = new AddResolvedAssemblyRequest();
		request.setFilepath(filePath);

		return new Promise<void>((resolve, reject) => {
			this.client?.addResolvedAssembly(request, {}, (err, response) => {
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
			this.client?.getMemberReferenceMetadata(request, {}, (err, response) => {
				if (err) {
					reject(err);
					return;
				}

				const memberReferenceMetadata: ReferenceMetadata = {
					memberFullName: response!.getMemberfullname(),
					definitionFilePath: response!.getDefinitionfilepath(),
					isCrossAssemblyReference: response!.getIscrossassemblyreference(),
					referencedAssemblyFullName: response!.getReferencedassemblyfullname(),
					referencedAssemblyFilePath: response!.getReferencedassemblyfilepath()
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
			this.client?.getMemberDefinitionPosition(request, {}, (err, response) => {
				if (err) {
					reject(`getMemberDefinitionPosition failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const selection: Selection = {
					startLineNumber: response!.getStartlinenumber(),
					endLineNumber: response!.getEndlinenumber(),
					startColumn: response!.getStartcolumn(),
					endColumn: response!.getEndcolumn()
				};

				resolve(selection);
			});
		})
	}

	getProjectCreationMetadataFromTypeFilePath(typeFilePath: string, projectVisualStudioVersion?: number): Promise<ProjectCreationMetadata> {
		const request = new GetProjectCreationMetadataFromTypeFilePathRequest();
		request.setTypefilepath(typeFilePath);
		request.setProjectvisualstudioversion(projectVisualStudioVersion ?? 0);

		return new Promise<ProjectCreationMetadata>((resolve, reject) => {
			this.client?.getProjectCreationMetadataFromTypeFilePath(request, null, (err, response) => {
				if (err) {
					reject(`getProjectCreationMetadataFromTypeFilePath failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const projectFileMetadata = response.getProjectfilemetadata();

				const assemblyMetadata: ProjectCreationMetadata = {
					assemblyFilePath: response.getAssemblyfilepath(),
					containsDangerousResources: response.getContainsdangerousresources(),
					projectFileMetadata: projectFileMetadata ? {
						isDecompilerSupportedProjectType: projectFileMetadata?.getIsdecompilersupportedprojecttype(),
						isVSSupportedProjectType: projectFileMetadata?.getIsvssupportedprojecttype(),
						projectTypeNotSupportedErrorMessage: projectFileMetadata?.getProjecttypenotsupportederrormessage(),
						projectFileName: projectFileMetadata?.getProjectfilename(),
						projectFileExtension: projectFileMetadata?.getProjectfileextension()
					} : null
				};

				resolve(assemblyMetadata);
			});
		});
	}

	createProject(assemblyFilePath: string, outputPath: string, decompileDangerousResources: boolean, projectVisualStudioVersion?: number): Promise<CreateProjectResult> {
		const request = new CreateProjectRequest();
		request.setAssemblyfilepath(assemblyFilePath);
		request.setOutputpath(outputPath);
		request.setDecompiledangerousresources(decompileDangerousResources);
		request.setProjectvisualstudioversion(projectVisualStudioVersion ?? 0);

		return new Promise<CreateProjectResult>((resolve, reject) => {
			this.client?.createProject(request, null, (err, response) => {
				if (err) {
					reject(`createProject failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const createProjectResult: CreateProjectResult = {
					errorMessage: response.getErrormessage()
				};

				resolve(createProjectResult);
			});
		});
	}

	getLegacyVisualStudioVersions = () => ['2010', '2012', '2013', '2015'];
}
