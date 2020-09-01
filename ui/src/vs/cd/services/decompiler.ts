import { RpcDecompilerClient } from './proto/MainServiceClientPb';
import {
	GetAllTypeFilePathsRequest, DecompileTypeRequest, Selection, GetMemberDefinitionRequest, GetMemberDefinitionResponse, GetMemberDefinitionPositionRequest
} from './proto/main_pb';

const client = new RpcDecompilerClient('https://localhost:5001');

const getMemberDefinition = (relativeFilePath: string, rowNumber: number, columnNumber: number) : Promise<GetMemberDefinitionResponse> => {
	const request = new GetMemberDefinitionRequest();
	request.setFilepath(relativeFilePath);
	request.setLinenumber(rowNumber);
	request.setColumnindex(columnNumber);

	return new Promise<GetMemberDefinitionResponse>((resolve, reject) => {
		client.getMemberDefinition(request, null, (err, response) => {
			if (err) {
				reject(`getMemberDefinition failed. Error: ${err}`);
				return;
			}

			resolve(response);
		});
	})
};

const getMemberDefinitionPosition = (memberFullName: string, filePath: string) : Promise<Selection> => {
	const request = new GetMemberDefinitionPositionRequest();
	request.setMemberfullname(memberFullName);
	request.setFilepath(filePath);

	return new Promise<Selection>((resolve, reject) => {
		client.getMemberDefinitionPosition(request, null, (err, response) => {
			if (err) {
				reject(`getMemberDefinitionPosition failed. Error: ${err}`);
				return;
			}

			resolve(response);
		});
	})
};

const getAllTypeFilePaths = (assemblyPath: string, targetPath: string) : Promise<TypeFilePath[]> => {
	const request = new GetAllTypeFilePathsRequest();
	request.setAssemblypath(assemblyPath);
	request.setTargetpath(targetPath);

	return new Promise<TypeFilePath[]>((resolve, reject) => {
		client.getAllTypeFilePaths(request, null, (err, response) => {
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

const decompileType = (assemblyPath: string, typeFullName: string) : Promise<string> => {
	const request = new DecompileTypeRequest();
	request.setAssemblypath(assemblyPath);
	request.setTypefullname(typeFullName);

	return new Promise<string>((resolve, reject) => {
		client.decompileType(request, null, (err, response) => {
			if (err) {
				reject(err);
				return;
			}

			resolve(response.getSourcecode());
		});
	});
};

interface TypeFilePath {
	typeFullName: string;
	relativeFilePath: string;
}

export {
	getAllTypeFilePaths,
	decompileType,
	getMemberDefinition,
	getMemberDefinitionPosition,
	TypeFilePath
};
