import * as jspb from 'google-protobuf'



export class GetAssemblyRelatedFilePathsRequest extends jspb.Message {
  getAssemblypath(): string;
  setAssemblypath(value: string): GetAssemblyRelatedFilePathsRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAssemblyRelatedFilePathsRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetAssemblyRelatedFilePathsRequest): GetAssemblyRelatedFilePathsRequest.AsObject;
  static serializeBinaryToWriter(message: GetAssemblyRelatedFilePathsRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetAssemblyRelatedFilePathsRequest;
  static deserializeBinaryFromReader(message: GetAssemblyRelatedFilePathsRequest, reader: jspb.BinaryReader): GetAssemblyRelatedFilePathsRequest;
}

export namespace GetAssemblyRelatedFilePathsRequest {
  export type AsObject = {
    assemblypath: string,
  }
}

export class GetAssemblyRelatedFilePathsResponse extends jspb.Message {
  getDecompiledassemblydirectory(): string;
  setDecompiledassemblydirectory(value: string): GetAssemblyRelatedFilePathsResponse;

  getDecompiledassemblypath(): string;
  setDecompiledassemblypath(value: string): GetAssemblyRelatedFilePathsResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAssemblyRelatedFilePathsResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetAssemblyRelatedFilePathsResponse): GetAssemblyRelatedFilePathsResponse.AsObject;
  static serializeBinaryToWriter(message: GetAssemblyRelatedFilePathsResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetAssemblyRelatedFilePathsResponse;
  static deserializeBinaryFromReader(message: GetAssemblyRelatedFilePathsResponse, reader: jspb.BinaryReader): GetAssemblyRelatedFilePathsResponse;
}

export namespace GetAssemblyRelatedFilePathsResponse {
  export type AsObject = {
    decompiledassemblydirectory: string,
    decompiledassemblypath: string,
  }
}

export class GetAllTypeFilePathsRequest extends jspb.Message {
  getAssemblypath(): string;
  setAssemblypath(value: string): GetAllTypeFilePathsRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAllTypeFilePathsRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetAllTypeFilePathsRequest): GetAllTypeFilePathsRequest.AsObject;
  static serializeBinaryToWriter(message: GetAllTypeFilePathsRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetAllTypeFilePathsRequest;
  static deserializeBinaryFromReader(message: GetAllTypeFilePathsRequest, reader: jspb.BinaryReader): GetAllTypeFilePathsRequest;
}

export namespace GetAllTypeFilePathsRequest {
  export type AsObject = {
    assemblypath: string,
  }
}

export class GetAllTypeFilePathsResponse extends jspb.Message {
  getTypefilepathsList(): Array<string>;
  setTypefilepathsList(value: Array<string>): GetAllTypeFilePathsResponse;
  clearTypefilepathsList(): GetAllTypeFilePathsResponse;
  addTypefilepaths(value: string, index?: number): GetAllTypeFilePathsResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAllTypeFilePathsResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetAllTypeFilePathsResponse): GetAllTypeFilePathsResponse.AsObject;
  static serializeBinaryToWriter(message: GetAllTypeFilePathsResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetAllTypeFilePathsResponse;
  static deserializeBinaryFromReader(message: GetAllTypeFilePathsResponse, reader: jspb.BinaryReader): GetAllTypeFilePathsResponse;
}

export namespace GetAllTypeFilePathsResponse {
  export type AsObject = {
    typefilepathsList: Array<string>,
  }
}

export class GetMemberReferenceMetadataRequest extends jspb.Message {
  getAbsolutefilepath(): string;
  setAbsolutefilepath(value: string): GetMemberReferenceMetadataRequest;

  getLinenumber(): number;
  setLinenumber(value: number): GetMemberReferenceMetadataRequest;

  getColumn(): number;
  setColumn(value: number): GetMemberReferenceMetadataRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetMemberReferenceMetadataRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetMemberReferenceMetadataRequest): GetMemberReferenceMetadataRequest.AsObject;
  static serializeBinaryToWriter(message: GetMemberReferenceMetadataRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetMemberReferenceMetadataRequest;
  static deserializeBinaryFromReader(message: GetMemberReferenceMetadataRequest, reader: jspb.BinaryReader): GetMemberReferenceMetadataRequest;
}

export namespace GetMemberReferenceMetadataRequest {
  export type AsObject = {
    absolutefilepath: string,
    linenumber: number,
    column: number,
  }
}

export class GetMemberReferenceMetadataResponse extends jspb.Message {
  getDefinitionfilepath(): string;
  setDefinitionfilepath(value: string): GetMemberReferenceMetadataResponse;

  getMemberfullname(): string;
  setMemberfullname(value: string): GetMemberReferenceMetadataResponse;

  getIscrossassemblyreference(): boolean;
  setIscrossassemblyreference(value: boolean): GetMemberReferenceMetadataResponse;

  getReferencedassemblyfullname(): string;
  setReferencedassemblyfullname(value: string): GetMemberReferenceMetadataResponse;

  getReferencedassemblyfilepath(): string;
  setReferencedassemblyfilepath(value: string): GetMemberReferenceMetadataResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetMemberReferenceMetadataResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetMemberReferenceMetadataResponse): GetMemberReferenceMetadataResponse.AsObject;
  static serializeBinaryToWriter(message: GetMemberReferenceMetadataResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetMemberReferenceMetadataResponse;
  static deserializeBinaryFromReader(message: GetMemberReferenceMetadataResponse, reader: jspb.BinaryReader): GetMemberReferenceMetadataResponse;
}

export namespace GetMemberReferenceMetadataResponse {
  export type AsObject = {
    definitionfilepath: string,
    memberfullname: string,
    iscrossassemblyreference: boolean,
    referencedassemblyfullname: string,
    referencedassemblyfilepath: string,
  }
}

export class GetMemberDefinitionPositionRequest extends jspb.Message {
  getAbsolutefilepath(): string;
  setAbsolutefilepath(value: string): GetMemberDefinitionPositionRequest;

  getMemberfullname(): string;
  setMemberfullname(value: string): GetMemberDefinitionPositionRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetMemberDefinitionPositionRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetMemberDefinitionPositionRequest): GetMemberDefinitionPositionRequest.AsObject;
  static serializeBinaryToWriter(message: GetMemberDefinitionPositionRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetMemberDefinitionPositionRequest;
  static deserializeBinaryFromReader(message: GetMemberDefinitionPositionRequest, reader: jspb.BinaryReader): GetMemberDefinitionPositionRequest;
}

export namespace GetMemberDefinitionPositionRequest {
  export type AsObject = {
    absolutefilepath: string,
    memberfullname: string,
  }
}

export class Selection extends jspb.Message {
  getStartlinenumber(): number;
  setStartlinenumber(value: number): Selection;

  getStartcolumn(): number;
  setStartcolumn(value: number): Selection;

  getEndlinenumber(): number;
  setEndlinenumber(value: number): Selection;

  getEndcolumn(): number;
  setEndcolumn(value: number): Selection;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Selection.AsObject;
  static toObject(includeInstance: boolean, msg: Selection): Selection.AsObject;
  static serializeBinaryToWriter(message: Selection, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Selection;
  static deserializeBinaryFromReader(message: Selection, reader: jspb.BinaryReader): Selection;
}

export namespace Selection {
  export type AsObject = {
    startlinenumber: number,
    startcolumn: number,
    endlinenumber: number,
    endcolumn: number,
  }
}

export class TypeFilePath extends jspb.Message {
  getTypefullname(): string;
  setTypefullname(value: string): TypeFilePath;

  getAbsolutefilepath(): string;
  setAbsolutefilepath(value: string): TypeFilePath;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): TypeFilePath.AsObject;
  static toObject(includeInstance: boolean, msg: TypeFilePath): TypeFilePath.AsObject;
  static serializeBinaryToWriter(message: TypeFilePath, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): TypeFilePath;
  static deserializeBinaryFromReader(message: TypeFilePath, reader: jspb.BinaryReader): TypeFilePath;
}

export namespace TypeFilePath {
  export type AsObject = {
    typefullname: string,
    absolutefilepath: string,
  }
}

export class DecompileTypeRequest extends jspb.Message {
  getFilepath(): string;
  setFilepath(value: string): DecompileTypeRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DecompileTypeRequest.AsObject;
  static toObject(includeInstance: boolean, msg: DecompileTypeRequest): DecompileTypeRequest.AsObject;
  static serializeBinaryToWriter(message: DecompileTypeRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): DecompileTypeRequest;
  static deserializeBinaryFromReader(message: DecompileTypeRequest, reader: jspb.BinaryReader): DecompileTypeRequest;
}

export namespace DecompileTypeRequest {
  export type AsObject = {
    filepath: string,
  }
}

export class DecompileTypeResponse extends jspb.Message {
  getSourcecode(): string;
  setSourcecode(value: string): DecompileTypeResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DecompileTypeResponse.AsObject;
  static toObject(includeInstance: boolean, msg: DecompileTypeResponse): DecompileTypeResponse.AsObject;
  static serializeBinaryToWriter(message: DecompileTypeResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): DecompileTypeResponse;
  static deserializeBinaryFromReader(message: DecompileTypeResponse, reader: jspb.BinaryReader): DecompileTypeResponse;
}

export namespace DecompileTypeResponse {
  export type AsObject = {
    sourcecode: string,
  }
}

