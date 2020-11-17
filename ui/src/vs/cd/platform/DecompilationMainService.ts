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
	GetProjectCreationMetadataRequest, GetContextAssemblyRequest, GetSearchResultPositionRequest, ShouldDecompileFileRequest
} from './proto/main_pb';
import * as grpc from "@grpc/grpc-js";
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { IGrpcMainService } from 'vs/cd/platform/GrpcMainService';
import { AssemblyMetadata, AssemblyRelatedFilePaths, ReferenceMetadata, Selection, ProjectCreationMetadata, CreateProjectResult } from 'vs/cd/common/DecompilationTypes';
import { Empty } from 'vs/cd/platform/proto/common_pb';

export const IDecompilationMainService = createDecorator<IDecompilationMainService>('IDecompilationMainService');

export interface IDecompilationMainService {
	readonly _serviceBrand: undefined;

	restoreDecompilationContext() : Promise<void>;
	shouldDecompileFile(filePath: string) : Promise<boolean>;
	getContextAssembly(contextUri: string) : Promise<AssemblyMetadata>;
	getAssemblyRelatedFilePaths(assemblyPath: string) : Promise<AssemblyRelatedFilePaths>;
	getProjectCreationMetadata(assemblyFilePath: string, projectVisualStudioVersion?: string): Promise<ProjectCreationMetadata>;
	getAllTypeFilePaths(assemblyPath: string) : Promise<string[]>;
	decompileType(filePath: string) : Promise<string>;
	getMemberReferenceMetadata(absoluteFilePath: string, lineNumber: number, column: number) : Promise<ReferenceMetadata>;
	getMemberDefinitionPosition(absoluteFilePath: string, memberName: string, declaringTypeName?: string) : Promise<Selection>;
	addResolvedAssembly(filePath: string) : Promise<void>;
	createProject(assemblyFilePath: string, outputPath: string, decompileDangerousResources: boolean, projectVisualStudioVersion?: string): Promise<CreateProjectResult>;
	getLegacyVisualStudioVersions() : Promise<string[]>;
	getSearchResultPosition(searchResultId: number) : Promise<Selection>;
	getWorkspaceDirectory() : Promise<string>;
	clearAssemblyList() : Promise<void>;
}

export class DecompilationMainService implements IDecompilationMainService {
	readonly _serviceBrand: undefined;

	private client: RpcDecompilerClient | undefined;

	constructor(@IGrpcMainService grpcService: IGrpcMainService) {
		grpcService.getServiceUrl().then(url => {
			this.client = new RpcDecompilerClient(url, grpc.credentials.createInsecure());
		});
	}

	restoreDecompilationContext() : Promise<void> {
		return new Promise<void>((resolve, reject) => {
			this.client?.restoreDecompilationContext(new Empty(), {}, (err) => {
				if (err) {
					reject(`initialize failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve();
			});
		});
	}

	shouldDecompileFile(filePath: string) : Promise<boolean> {
		const request = new ShouldDecompileFileRequest();
		request.setFilepath(filePath);

		return new Promise<boolean>((resolve, reject) => {
			this.client?.shouldDecompileFile(request, {}, (err, response) => {
				if (err) {
					reject(`shouldDecompileFile failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve(response!.getShoulddecompilefile());
			});
		});
	}

	getContextAssembly(contextUri: string) : Promise<AssemblyMetadata> {
		const request = new GetContextAssemblyRequest();
		request.setContexturi(contextUri);

		return new Promise<AssemblyMetadata>((resolve, reject) => {
			this.client?.getContextAssembly(request, {}, (err, response) => {
				if (err) {
					reject(`getContextAssembly failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const assemblyMetadata: AssemblyMetadata = {
					assemblyFullName: response!.getAssemblyname(),
					assemblyFilePath: response!.getAssemblyfilepath()
				};

				resolve(assemblyMetadata);
			});
		});
	}

	getWorkspaceDirectory() : Promise<string> {
		return new Promise<string>((resolve, reject) => {
			this.client?.getWorkspaceDirectory(new Empty(), {}, (err, response) => {
				if (err) {
					reject(`getWorkspaceDirectory failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve(response!.getDirectorypath());
			});
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
					reject({
						...err,
						metadata: {
							unresolvedAssemblyName: err.metadata.get('unresolvedassemblyname')[0]
						}
					});
					return;
				}

				const memberReferenceMetadata: ReferenceMetadata = {
					memberName: response!.getMembername(),
					declaringTypeName: response!.getDeclaringtypename(),
					definitionFilePath: response!.getDefinitionfilepath(),
					isCrossAssemblyReference: response!.getIscrossassemblyreference(),
					referencedAssemblyFullName: response!.getReferencedassemblyfullname(),
					referencedAssemblyFilePath: response!.getReferencedassemblyfilepath()
				};

				resolve(memberReferenceMetadata);
			});
		})
	}

	getMemberDefinitionPosition(absoluteFilePath: string, memberName: string, declaringTypeName?: string) : Promise<Selection> {
		const request = new GetMemberDefinitionPositionRequest();
		request.setMembername(memberName);
		if (declaringTypeName) {
			request.setDeclaringtypename(declaringTypeName);
		}
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

	getProjectCreationMetadata(assemblyFilePath: string, projectVisualStudioVersion?: string): Promise<ProjectCreationMetadata> {
		const request = new GetProjectCreationMetadataRequest();
		request.setAssemblyfilepath(assemblyFilePath);
		request.setProjectvisualstudioversion(projectVisualStudioVersion ?? '');

		return new Promise<ProjectCreationMetadata>((resolve, reject) => {
			this.client?.getProjectCreationMetadata(request, {}, (err, response) => {
				if (err) {
					reject(`getProjectCreationMetadata failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const projectFileMetadata = response!.getProjectfilemetadata();

				const projectCreationMetadata: ProjectCreationMetadata = {
					containsDangerousResources: response!.getContainsdangerousresources(),
					projectFileMetadata: projectFileMetadata ? {
						isDecompilerSupportedProjectType: projectFileMetadata?.getIsdecompilersupportedprojecttype(),
						isVSSupportedProjectType: projectFileMetadata?.getIsvssupportedprojecttype(),
						projectTypeNotSupportedErrorMessage: projectFileMetadata?.getProjecttypenotsupportederrormessage(),
						projectFileName: projectFileMetadata?.getProjectfilename(),
						projectFileExtension: projectFileMetadata?.getProjectfileextension()
					} : null
				};

				resolve(projectCreationMetadata);
			});
		});
	}

	createProject(assemblyFilePath: string, outputPath: string, decompileDangerousResources: boolean, projectVisualStudioVersion?: string): Promise<CreateProjectResult> {
		const request = new CreateProjectRequest();
		request.setAssemblyfilepath(assemblyFilePath);
		request.setOutputpath(outputPath);
		request.setDecompiledangerousresources(decompileDangerousResources);
		request.setProjectvisualstudioversion(projectVisualStudioVersion ?? '');

		return new Promise<CreateProjectResult>((resolve, reject) => {
			this.client?.createProject(request, {}, (err, response) => {
				if (err) {
					reject(`createProject failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				const createProjectResult: CreateProjectResult = {
					errorMessage: response!.getErrormessage()
				};

				resolve(createProjectResult);
			});
		});
	}

	getLegacyVisualStudioVersions = async () => ['2010', '2012', '2013', '2015', '2017'];

	getSearchResultPosition(searchResultId: number) : Promise<Selection> {
		const request = new GetSearchResultPositionRequest();
		request.setSearchresultid(searchResultId);

		return new Promise<Selection>((resolve, reject) => {
			this.client?.getSearchResultPosition(request, {}, (err, response) => {
				if (err) {
					reject(err);
					return;
				}

				const selection: Selection = {
					startLineNumber: response!.getStartlinenumber(),
					startColumn: response!.getStartcolumn(),
					endLineNumber: response!.getEndlinenumber(),
					endColumn: response!.getEndcolumn()
				};

				resolve(selection);
			});
		});
	}

	clearAssemblyList() : Promise<void> {
		return new Promise<void>((resolve, reject) => {
			this.client?.clearAssemblyList(new Empty(), {}, (err) => {
				if (err) {
					reject(`clearAssemblyList failed. Error: ${JSON.stringify(err)}`);
					return;
				}

				resolve();
			});
		});
	}
}
