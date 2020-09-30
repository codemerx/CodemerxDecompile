// GENERATED CODE -- DO NOT EDIT!

// package: 
// file: main.proto

import * as main_pb from "./main_pb";
import * as common_pb from "./common_pb";
import * as grpc from "@grpc/grpc-js";

interface IRpcDecompilerService extends grpc.ServiceDefinition<grpc.UntypedServiceImplementation> {
  getAssemblyRelatedFilePaths: grpc.MethodDefinition<main_pb.GetAssemblyRelatedFilePathsRequest, main_pb.GetAssemblyRelatedFilePathsResponse>;
  getAllTypeFilePaths: grpc.MethodDefinition<main_pb.GetAllTypeFilePathsRequest, main_pb.GetAllTypeFilePathsResponse>;
  decompileType: grpc.MethodDefinition<main_pb.DecompileTypeRequest, main_pb.DecompileTypeResponse>;
  getMemberReferenceMetadata: grpc.MethodDefinition<main_pb.GetMemberReferenceMetadataRequest, main_pb.GetMemberReferenceMetadataResponse>;
  getMemberDefinitionPosition: grpc.MethodDefinition<main_pb.GetMemberDefinitionPositionRequest, main_pb.Selection>;
  addResolvedAssembly: grpc.MethodDefinition<main_pb.AddResolvedAssemblyRequest, common_pb.Empty>;
  search: grpc.MethodDefinition<main_pb.SearchRequest, main_pb.SearchResult>;
}

export const RpcDecompilerService: IRpcDecompilerService;

export class RpcDecompilerClient extends grpc.Client {
  constructor(address: string, credentials: grpc.ChannelCredentials, options?: object);
  getAssemblyRelatedFilePaths(argument: main_pb.GetAssemblyRelatedFilePathsRequest, callback: grpc.requestCallback<main_pb.GetAssemblyRelatedFilePathsResponse>): grpc.ClientUnaryCall;
  getAssemblyRelatedFilePaths(argument: main_pb.GetAssemblyRelatedFilePathsRequest, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.GetAssemblyRelatedFilePathsResponse>): grpc.ClientUnaryCall;
  getAssemblyRelatedFilePaths(argument: main_pb.GetAssemblyRelatedFilePathsRequest, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.GetAssemblyRelatedFilePathsResponse>): grpc.ClientUnaryCall;
  getAllTypeFilePaths(argument: main_pb.GetAllTypeFilePathsRequest, callback: grpc.requestCallback<main_pb.GetAllTypeFilePathsResponse>): grpc.ClientUnaryCall;
  getAllTypeFilePaths(argument: main_pb.GetAllTypeFilePathsRequest, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.GetAllTypeFilePathsResponse>): grpc.ClientUnaryCall;
  getAllTypeFilePaths(argument: main_pb.GetAllTypeFilePathsRequest, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.GetAllTypeFilePathsResponse>): grpc.ClientUnaryCall;
  decompileType(argument: main_pb.DecompileTypeRequest, callback: grpc.requestCallback<main_pb.DecompileTypeResponse>): grpc.ClientUnaryCall;
  decompileType(argument: main_pb.DecompileTypeRequest, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.DecompileTypeResponse>): grpc.ClientUnaryCall;
  decompileType(argument: main_pb.DecompileTypeRequest, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.DecompileTypeResponse>): grpc.ClientUnaryCall;
  getMemberReferenceMetadata(argument: main_pb.GetMemberReferenceMetadataRequest, callback: grpc.requestCallback<main_pb.GetMemberReferenceMetadataResponse>): grpc.ClientUnaryCall;
  getMemberReferenceMetadata(argument: main_pb.GetMemberReferenceMetadataRequest, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.GetMemberReferenceMetadataResponse>): grpc.ClientUnaryCall;
  getMemberReferenceMetadata(argument: main_pb.GetMemberReferenceMetadataRequest, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.GetMemberReferenceMetadataResponse>): grpc.ClientUnaryCall;
  getMemberDefinitionPosition(argument: main_pb.GetMemberDefinitionPositionRequest, callback: grpc.requestCallback<main_pb.Selection>): grpc.ClientUnaryCall;
  getMemberDefinitionPosition(argument: main_pb.GetMemberDefinitionPositionRequest, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.Selection>): grpc.ClientUnaryCall;
  getMemberDefinitionPosition(argument: main_pb.GetMemberDefinitionPositionRequest, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<main_pb.Selection>): grpc.ClientUnaryCall;
  addResolvedAssembly(argument: main_pb.AddResolvedAssemblyRequest, callback: grpc.requestCallback<common_pb.Empty>): grpc.ClientUnaryCall;
  addResolvedAssembly(argument: main_pb.AddResolvedAssemblyRequest, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<common_pb.Empty>): grpc.ClientUnaryCall;
  addResolvedAssembly(argument: main_pb.AddResolvedAssemblyRequest, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<common_pb.Empty>): grpc.ClientUnaryCall;
  search(argument: main_pb.SearchRequest, metadataOrOptions?: grpc.Metadata | grpc.CallOptions | null): grpc.ClientReadableStream<main_pb.SearchResult>;
  search(argument: main_pb.SearchRequest, metadata?: grpc.Metadata | null, options?: grpc.CallOptions | null): grpc.ClientReadableStream<main_pb.SearchResult>;
}
