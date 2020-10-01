// package: 
// file: main.proto

import * as jspb from "google-protobuf";
import * as common_pb from "./common_pb";

export class GetContextAssemblyRequest extends jspb.Message {
  getContexturi(): string;
  setContexturi(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetContextAssemblyRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetContextAssemblyRequest): GetContextAssemblyRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: GetContextAssemblyRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetContextAssemblyRequest;
  static deserializeBinaryFromReader(message: GetContextAssemblyRequest, reader: jspb.BinaryReader): GetContextAssemblyRequest;
}

export namespace GetContextAssemblyRequest {
  export type AsObject = {
    contexturi: string,
  }
}

export class GetContextAssemblyResponse extends jspb.Message {
  getAssemblyname(): string;
  setAssemblyname(value: string): void;

  getAssemblyfilepath(): string;
  setAssemblyfilepath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetContextAssemblyResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetContextAssemblyResponse): GetContextAssemblyResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: GetContextAssemblyResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetContextAssemblyResponse;
  static deserializeBinaryFromReader(message: GetContextAssemblyResponse, reader: jspb.BinaryReader): GetContextAssemblyResponse;
}

export namespace GetContextAssemblyResponse {
  export type AsObject = {
    assemblyname: string,
    assemblyfilepath: string,
  }
}

export class GetProjectCreationMetadataRequest extends jspb.Message {
  getAssemblyfilepath(): string;
  setAssemblyfilepath(value: string): void;

  getProjectvisualstudioversion(): string;
  setProjectvisualstudioversion(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetProjectCreationMetadataRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetProjectCreationMetadataRequest): GetProjectCreationMetadataRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: GetProjectCreationMetadataRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetProjectCreationMetadataRequest;
  static deserializeBinaryFromReader(message: GetProjectCreationMetadataRequest, reader: jspb.BinaryReader): GetProjectCreationMetadataRequest;
}

export namespace GetProjectCreationMetadataRequest {
  export type AsObject = {
    assemblyfilepath: string,
    projectvisualstudioversion: string,
  }
}

export class GetProjectCreationMetadataResponse extends jspb.Message {
  getContainsdangerousresources(): boolean;
  setContainsdangerousresources(value: boolean): void;

  hasProjectfilemetadata(): boolean;
  clearProjectfilemetadata(): void;
  getProjectfilemetadata(): ProjectFileMetadata | undefined;
  setProjectfilemetadata(value?: ProjectFileMetadata): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetProjectCreationMetadataResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetProjectCreationMetadataResponse): GetProjectCreationMetadataResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: GetProjectCreationMetadataResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetProjectCreationMetadataResponse;
  static deserializeBinaryFromReader(message: GetProjectCreationMetadataResponse, reader: jspb.BinaryReader): GetProjectCreationMetadataResponse;
}

export namespace GetProjectCreationMetadataResponse {
  export type AsObject = {
    containsdangerousresources: boolean,
    projectfilemetadata?: ProjectFileMetadata.AsObject,
  }
}

export class ProjectFileMetadata extends jspb.Message {
  getIsdecompilersupportedprojecttype(): boolean;
  setIsdecompilersupportedprojecttype(value: boolean): void;

  getIsvssupportedprojecttype(): boolean;
  setIsvssupportedprojecttype(value: boolean): void;

  getProjecttypenotsupportederrormessage(): string;
  setProjecttypenotsupportederrormessage(value: string): void;

  getProjectfilename(): string;
  setProjectfilename(value: string): void;

  getProjectfileextension(): string;
  setProjectfileextension(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ProjectFileMetadata.AsObject;
  static toObject(includeInstance: boolean, msg: ProjectFileMetadata): ProjectFileMetadata.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: ProjectFileMetadata, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ProjectFileMetadata;
  static deserializeBinaryFromReader(message: ProjectFileMetadata, reader: jspb.BinaryReader): ProjectFileMetadata;
}

export namespace ProjectFileMetadata {
  export type AsObject = {
    isdecompilersupportedprojecttype: boolean,
    isvssupportedprojecttype: boolean,
    projecttypenotsupportederrormessage: string,
    projectfilename: string,
    projectfileextension: string,
  }
}

export class CreateProjectRequest extends jspb.Message {
  getAssemblyfilepath(): string;
  setAssemblyfilepath(value: string): void;

  getOutputpath(): string;
  setOutputpath(value: string): void;

  getDecompiledangerousresources(): boolean;
  setDecompiledangerousresources(value: boolean): void;

  getProjectvisualstudioversion(): string;
  setProjectvisualstudioversion(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CreateProjectRequest.AsObject;
  static toObject(includeInstance: boolean, msg: CreateProjectRequest): CreateProjectRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: CreateProjectRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CreateProjectRequest;
  static deserializeBinaryFromReader(message: CreateProjectRequest, reader: jspb.BinaryReader): CreateProjectRequest;
}

export namespace CreateProjectRequest {
  export type AsObject = {
    assemblyfilepath: string,
    outputpath: string,
    decompiledangerousresources: boolean,
    projectvisualstudioversion: string,
  }
}

export class CreateProjectResponse extends jspb.Message {
  getErrormessage(): string;
  setErrormessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CreateProjectResponse.AsObject;
  static toObject(includeInstance: boolean, msg: CreateProjectResponse): CreateProjectResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: CreateProjectResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CreateProjectResponse;
  static deserializeBinaryFromReader(message: CreateProjectResponse, reader: jspb.BinaryReader): CreateProjectResponse;
}

export namespace CreateProjectResponse {
  export type AsObject = {
    errormessage: string,
  }
}

export class GetAssemblyRelatedFilePathsRequest extends jspb.Message {
  getAssemblypath(): string;
  setAssemblypath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAssemblyRelatedFilePathsRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetAssemblyRelatedFilePathsRequest): GetAssemblyRelatedFilePathsRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setDecompiledassemblydirectory(value: string): void;

  getDecompiledassemblypath(): string;
  setDecompiledassemblypath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAssemblyRelatedFilePathsResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetAssemblyRelatedFilePathsResponse): GetAssemblyRelatedFilePathsResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setAssemblypath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAllTypeFilePathsRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetAllTypeFilePathsRequest): GetAllTypeFilePathsRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  clearTypefilepathsList(): void;
  getTypefilepathsList(): Array<string>;
  setTypefilepathsList(value: Array<string>): void;
  addTypefilepaths(value: string, index?: number): string;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAllTypeFilePathsResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetAllTypeFilePathsResponse): GetAllTypeFilePathsResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setAbsolutefilepath(value: string): void;

  getLinenumber(): number;
  setLinenumber(value: number): void;

  getColumn(): number;
  setColumn(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetMemberReferenceMetadataRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetMemberReferenceMetadataRequest): GetMemberReferenceMetadataRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setDefinitionfilepath(value: string): void;

  getMemberfullname(): string;
  setMemberfullname(value: string): void;

  getIscrossassemblyreference(): boolean;
  setIscrossassemblyreference(value: boolean): void;

  getReferencedassemblyfullname(): string;
  setReferencedassemblyfullname(value: string): void;

  getReferencedassemblyfilepath(): string;
  setReferencedassemblyfilepath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetMemberReferenceMetadataResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetMemberReferenceMetadataResponse): GetMemberReferenceMetadataResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setAbsolutefilepath(value: string): void;

  getMemberfullname(): string;
  setMemberfullname(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetMemberDefinitionPositionRequest.AsObject;
  static toObject(includeInstance: boolean, msg: GetMemberDefinitionPositionRequest): GetMemberDefinitionPositionRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setStartlinenumber(value: number): void;

  getStartcolumn(): number;
  setStartcolumn(value: number): void;

  getEndlinenumber(): number;
  setEndlinenumber(value: number): void;

  getEndcolumn(): number;
  setEndcolumn(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Selection.AsObject;
  static toObject(includeInstance: boolean, msg: Selection): Selection.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setTypefullname(value: string): void;

  getAbsolutefilepath(): string;
  setAbsolutefilepath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): TypeFilePath.AsObject;
  static toObject(includeInstance: boolean, msg: TypeFilePath): TypeFilePath.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setFilepath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DecompileTypeRequest.AsObject;
  static toObject(includeInstance: boolean, msg: DecompileTypeRequest): DecompileTypeRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
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
  setSourcecode(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DecompileTypeResponse.AsObject;
  static toObject(includeInstance: boolean, msg: DecompileTypeResponse): DecompileTypeResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: DecompileTypeResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): DecompileTypeResponse;
  static deserializeBinaryFromReader(message: DecompileTypeResponse, reader: jspb.BinaryReader): DecompileTypeResponse;
}

export namespace DecompileTypeResponse {
  export type AsObject = {
    sourcecode: string,
  }
}

export class AddResolvedAssemblyRequest extends jspb.Message {
  getFilepath(): string;
  setFilepath(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AddResolvedAssemblyRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AddResolvedAssemblyRequest): AddResolvedAssemblyRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: AddResolvedAssemblyRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AddResolvedAssemblyRequest;
  static deserializeBinaryFromReader(message: AddResolvedAssemblyRequest, reader: jspb.BinaryReader): AddResolvedAssemblyRequest;
}

export namespace AddResolvedAssemblyRequest {
  export type AsObject = {
    filepath: string,
  }
}

export class SearchRequest extends jspb.Message {
  getAssemblyfilepath(): string;
  setAssemblyfilepath(value: string): void;

  getQuery(): string;
  setQuery(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchRequest.AsObject;
  static toObject(includeInstance: boolean, msg: SearchRequest): SearchRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: SearchRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchRequest;
  static deserializeBinaryFromReader(message: SearchRequest, reader: jspb.BinaryReader): SearchRequest;
}

export namespace SearchRequest {
  export type AsObject = {
    assemblyfilepath: string,
    query: string,
  }
}

export class SearchResultResponse extends jspb.Message {
  getFilepath(): string;
  setFilepath(value: string): void;

  getPreview(): string;
  setPreview(value: string): void;

  hasHighlightrange(): boolean;
  clearHighlightrange(): void;
  getHighlightrange(): PreviewHighlightRange | undefined;
  setHighlightrange(value?: PreviewHighlightRange): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SearchResultResponse.AsObject;
  static toObject(includeInstance: boolean, msg: SearchResultResponse): SearchResultResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: SearchResultResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SearchResultResponse;
  static deserializeBinaryFromReader(message: SearchResultResponse, reader: jspb.BinaryReader): SearchResultResponse;
}

export namespace SearchResultResponse {
  export type AsObject = {
    filepath: string,
    preview: string,
    highlightrange?: PreviewHighlightRange.AsObject,
  }
}

export class PreviewHighlightRange extends jspb.Message {
  getStartindex(): number;
  setStartindex(value: number): void;

  getEndindex(): number;
  setEndindex(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): PreviewHighlightRange.AsObject;
  static toObject(includeInstance: boolean, msg: PreviewHighlightRange): PreviewHighlightRange.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: PreviewHighlightRange, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): PreviewHighlightRange;
  static deserializeBinaryFromReader(message: PreviewHighlightRange, reader: jspb.BinaryReader): PreviewHighlightRange;
}

export namespace PreviewHighlightRange {
  export type AsObject = {
    startindex: number,
    endindex: number,
  }
}

