/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as main_pb from './main_pb';


export class RpcDecompilerClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: any; };

  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; }) {
    if (!options) options = {};
    if (!credentials) credentials = {};
    options['format'] = 'text';

    this.client_ = new grpcWeb.GrpcWebClientBase(options);
    this.hostname_ = hostname;
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodInfoGetAssemblyRelatedFilePaths = new grpcWeb.AbstractClientBase.MethodInfo(
    main_pb.GetAssemblyRelatedFilePathsResponse,
    (request: main_pb.GetAssemblyRelatedFilePathsRequest) => {
      return request.serializeBinary();
    },
    main_pb.GetAssemblyRelatedFilePathsResponse.deserializeBinary
  );

  getAssemblyRelatedFilePaths(
    request: main_pb.GetAssemblyRelatedFilePathsRequest,
    metadata: grpcWeb.Metadata | null): Promise<main_pb.GetAssemblyRelatedFilePathsResponse>;

  getAssemblyRelatedFilePaths(
    request: main_pb.GetAssemblyRelatedFilePathsRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: main_pb.GetAssemblyRelatedFilePathsResponse) => void): grpcWeb.ClientReadableStream<main_pb.GetAssemblyRelatedFilePathsResponse>;

  getAssemblyRelatedFilePaths(
    request: main_pb.GetAssemblyRelatedFilePathsRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: main_pb.GetAssemblyRelatedFilePathsResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/RpcDecompiler/GetAssemblyRelatedFilePaths',
        request,
        metadata || {},
        this.methodInfoGetAssemblyRelatedFilePaths,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/GetAssemblyRelatedFilePaths',
    request,
    metadata || {},
    this.methodInfoGetAssemblyRelatedFilePaths);
  }

  methodInfoGetAllTypeFilePaths = new grpcWeb.AbstractClientBase.MethodInfo(
    main_pb.GetAllTypeFilePathsResponse,
    (request: main_pb.GetAllTypeFilePathsRequest) => {
      return request.serializeBinary();
    },
    main_pb.GetAllTypeFilePathsResponse.deserializeBinary
  );

  getAllTypeFilePaths(
    request: main_pb.GetAllTypeFilePathsRequest,
    metadata: grpcWeb.Metadata | null): Promise<main_pb.GetAllTypeFilePathsResponse>;

  getAllTypeFilePaths(
    request: main_pb.GetAllTypeFilePathsRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: main_pb.GetAllTypeFilePathsResponse) => void): grpcWeb.ClientReadableStream<main_pb.GetAllTypeFilePathsResponse>;

  getAllTypeFilePaths(
    request: main_pb.GetAllTypeFilePathsRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: main_pb.GetAllTypeFilePathsResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/RpcDecompiler/GetAllTypeFilePaths',
        request,
        metadata || {},
        this.methodInfoGetAllTypeFilePaths,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/GetAllTypeFilePaths',
    request,
    metadata || {},
    this.methodInfoGetAllTypeFilePaths);
  }

  methodInfoDecompileType = new grpcWeb.AbstractClientBase.MethodInfo(
    main_pb.DecompileTypeResponse,
    (request: main_pb.DecompileTypeRequest) => {
      return request.serializeBinary();
    },
    main_pb.DecompileTypeResponse.deserializeBinary
  );

  decompileType(
    request: main_pb.DecompileTypeRequest,
    metadata: grpcWeb.Metadata | null): Promise<main_pb.DecompileTypeResponse>;

  decompileType(
    request: main_pb.DecompileTypeRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: main_pb.DecompileTypeResponse) => void): grpcWeb.ClientReadableStream<main_pb.DecompileTypeResponse>;

  decompileType(
    request: main_pb.DecompileTypeRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: main_pb.DecompileTypeResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/RpcDecompiler/DecompileType',
        request,
        metadata || {},
        this.methodInfoDecompileType,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/DecompileType',
    request,
    metadata || {},
    this.methodInfoDecompileType);
  }

  methodInfoGetMemberReferenceMetadata = new grpcWeb.AbstractClientBase.MethodInfo(
    main_pb.GetMemberReferenceMetadataResponse,
    (request: main_pb.GetMemberReferenceMetadataRequest) => {
      return request.serializeBinary();
    },
    main_pb.GetMemberReferenceMetadataResponse.deserializeBinary
  );

  getMemberReferenceMetadata(
    request: main_pb.GetMemberReferenceMetadataRequest,
    metadata: grpcWeb.Metadata | null): Promise<main_pb.GetMemberReferenceMetadataResponse>;

  getMemberReferenceMetadata(
    request: main_pb.GetMemberReferenceMetadataRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: main_pb.GetMemberReferenceMetadataResponse) => void): grpcWeb.ClientReadableStream<main_pb.GetMemberReferenceMetadataResponse>;

  getMemberReferenceMetadata(
    request: main_pb.GetMemberReferenceMetadataRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: main_pb.GetMemberReferenceMetadataResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/RpcDecompiler/GetMemberReferenceMetadata',
        request,
        metadata || {},
        this.methodInfoGetMemberReferenceMetadata,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/GetMemberReferenceMetadata',
    request,
    metadata || {},
    this.methodInfoGetMemberReferenceMetadata);
  }

  methodInfoGetMemberDefinitionPosition = new grpcWeb.AbstractClientBase.MethodInfo(
    main_pb.Selection,
    (request: main_pb.GetMemberDefinitionPositionRequest) => {
      return request.serializeBinary();
    },
    main_pb.Selection.deserializeBinary
  );

  getMemberDefinitionPosition(
    request: main_pb.GetMemberDefinitionPositionRequest,
    metadata: grpcWeb.Metadata | null): Promise<main_pb.Selection>;

  getMemberDefinitionPosition(
    request: main_pb.GetMemberDefinitionPositionRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: main_pb.Selection) => void): grpcWeb.ClientReadableStream<main_pb.Selection>;

  getMemberDefinitionPosition(
    request: main_pb.GetMemberDefinitionPositionRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: main_pb.Selection) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/RpcDecompiler/GetMemberDefinitionPosition',
        request,
        metadata || {},
        this.methodInfoGetMemberDefinitionPosition,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/GetMemberDefinitionPosition',
    request,
    metadata || {},
    this.methodInfoGetMemberDefinitionPosition);
  }

  methodInfoAddResolvedAssembly = new grpcWeb.AbstractClientBase.MethodInfo(
    main_pb.Empty,
    (request: main_pb.AddResolvedAssemblyRequest) => {
      return request.serializeBinary();
    },
    main_pb.Empty.deserializeBinary
  );

  addResolvedAssembly(
    request: main_pb.AddResolvedAssemblyRequest,
    metadata: grpcWeb.Metadata | null): Promise<main_pb.Empty>;

  addResolvedAssembly(
    request: main_pb.AddResolvedAssemblyRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: main_pb.Empty) => void): grpcWeb.ClientReadableStream<main_pb.Empty>;

  addResolvedAssembly(
    request: main_pb.AddResolvedAssemblyRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: main_pb.Empty) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/RpcDecompiler/AddResolvedAssembly',
        request,
        metadata || {},
        this.methodInfoAddResolvedAssembly,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/AddResolvedAssembly',
    request,
    metadata || {},
    this.methodInfoAddResolvedAssembly);
  }

}

