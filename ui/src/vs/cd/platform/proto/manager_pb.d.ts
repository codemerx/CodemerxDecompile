// package: 
// file: manager.proto

import * as jspb from "google-protobuf";
import * as common_pb from "./common_pb";

export class GetServerStatusResponse extends jspb.Message {
  getStatus(): string;
  setStatus(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GetServerStatusResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GetServerStatusResponse): GetServerStatusResponse.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: GetServerStatusResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GetServerStatusResponse;
  static deserializeBinaryFromReader(message: GetServerStatusResponse, reader: jspb.BinaryReader): GetServerStatusResponse;
}

export namespace GetServerStatusResponse {
  export type AsObject = {
    status: string,
  }
}

