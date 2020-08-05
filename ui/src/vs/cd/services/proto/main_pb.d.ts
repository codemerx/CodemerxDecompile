import * as jspb from "google-protobuf"

export class GetAllTypeFilePathsRequest extends jspb.Message {
  getAssemblypath(): string;
  setAssemblypath(value: string): GetAllTypeFilePathsRequest;

  getTargetpath(): string;
  setTargetpath(value: string): GetAllTypeFilePathsRequest;

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
    targetpath: string,
  }
}

export class GetAllTypeFilePathsResponse extends jspb.Message {
  getTypefilepathsList(): Array<TypeFilePath>;
  setTypefilepathsList(value: Array<TypeFilePath>): GetAllTypeFilePathsResponse;
  clearTypefilepathsList(): GetAllTypeFilePathsResponse;
  addTypefilepaths(value?: TypeFilePath, index?: number): TypeFilePath;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetAllTypeFilePathsResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetAllTypeFilePathsResponse): GetAllTypeFilePathsResponse.AsObject;
  static serializeBinaryToWriter(message: GetAllTypeFilePathsResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetAllTypeFilePathsResponse;
  static deserializeBinaryFromReader(message: GetAllTypeFilePathsResponse, reader: jspb.BinaryReader): GetAllTypeFilePathsResponse;
}

export namespace GetAllTypeFilePathsResponse {
  export type AsObject = {
    typefilepathsList: Array<TypeFilePath.AsObject>,
  }
}

export class TypeFilePath extends jspb.Message {
  getTypefullname(): string;
  setTypefullname(value: string): TypeFilePath;

  getRelativefilepath(): string;
  setRelativefilepath(value: string): TypeFilePath;

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
    relativefilepath: string,
  }
}

export class DecompileTypeRequest extends jspb.Message {
  getAssemblypath(): string;
  setAssemblypath(value: string): DecompileTypeRequest;

  getTypefullname(): string;
  setTypefullname(value: string): DecompileTypeRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): DecompileTypeRequest.AsObject;
  static toObject(includeInstance: boolean, msg: DecompileTypeRequest): DecompileTypeRequest.AsObject;
  static serializeBinaryToWriter(message: DecompileTypeRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): DecompileTypeRequest;
  static deserializeBinaryFromReader(message: DecompileTypeRequest, reader: jspb.BinaryReader): DecompileTypeRequest;
}

export namespace DecompileTypeRequest {
  export type AsObject = {
    assemblypath: string,
    typefullname: string,
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

